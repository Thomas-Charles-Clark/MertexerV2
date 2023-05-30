using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MertexerV2.Utilities;
using System.Xml.Linq;
using System.Xml.XPath;
using MertexerV2.Models.OrderImport;
using System.Text.RegularExpressions;
using System.Globalization;
using MertexerV2.Models.DataSource;
using Newtonsoft.Json;
using CsvHelper;
using System.IO;

namespace MertexerV2.SQLFunctions
{
    /*
       -------------------------------- Standard order fields
       0  TradeAccountNumber [50]
       1  OrderReference [50]
       2  DeliveryName [50]
       3  DeliveryAddressLine1 [60]
       4  DeliveryAddressLine2 [60]
       5  DeliveryTown [50]
       6  DeliveryCounty [40]
       7  DeliveryPostcode [20]
       8  DeliveryCountryCode [3]
       9  DeliveryPhoneNumber [20]
       10 Email [256]
       11 ProductCode [50]
       12 Quantity 
       13 Type 
       14 LineReference [50]
       15 Memo [1000]                   
       16 ShippingAlertEmailAddress [MAX] -- Osborne & Little may provide Despatch Alert Email in this field.
                                            Push this through to HEADER.REFS<1,26>.
                                            Change this in the file format PDF available through the new trade site.
                                            Semicolon separated list of email addresses to be alerted for dispatch notifications.
       17 ContactPhone [20]             
       -------------------------------- Distributor only fields beneath
       18 HaulierCode [50]              
       19 CarriageCode [50]             
       20 HaulierAccountNumber [50]         
       21 TaxesDutiesToInvoiceAccount [50]
       22 DeliveryTerms [50]
       23 DeliveryType [50]
       24 EndCustomer [50]
       -------------------------------- Internal only fields beneath
       25 HeldForSales       [5]
       26 PromoCode          [50]
       27 DiscountPercentage [10]
     */


    public partial class Order
    {
        internal static readonly string[] Delimiter = new string[] { "," };

        internal static readonly List<string> ValidTypes = new List<string> {
            "BUNDLE",
            "PRODUCT",
            "SAMPLES CUTTING",
            "RETURNABLE SAMPLE",
            "RESERVE",
            "RESERVE WITH BATCH CUTTING",
            "HANGER",
            "SHOWLENGTH",
            "WATERFALL ONLY"
        };

        internal static readonly List<string> ForbiddenItemTypeCodes = new List<string>() { 
            "CPS" // Old Cushion Pad code
        };

        [SqlProcedure]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static int ProcessImport(long TradeOrderImportFileID)
        {
            var fileName = "";
            var fileContents = "";
            var tradeAccountNumber = "";
            var uploadedDt = DateTime.Now;
            string orderImportFileEmail = null;

            try
            {
#if DEBUG
                using (var conn = new SqlConnection("Data Source=vm-sql1;Initial Catalog=Flock;Integrated Security=True"))
#else
            using (var conn = new SqlConnection("context connection=true"))
#endif
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            using (var comm = new SqlCommand(@"SELECT 
                                                                FileName,
                                                                Contents,
                                                                TradeAccountNumber,
                                                                UploadedDT,
                                                                Email
                                                            FROM 
                                                                portal.TradeOrderImport_File 
                                                            WHERE 
                                                                TradeOrderImportFileID = @TradeOrderImportFileID", conn))
                            {
                                comm.Transaction = transaction;

                                comm.CommandType = CommandType.Text;

                                comm.Parameters.AddWithValue("@TradeOrderImportFileID", TradeOrderImportFileID);

                                var reader = comm.ExecuteReader();

                                while (reader.Read())
                                {
                                    fileName = reader.GetString(0);
                                    fileContents = reader.GetString(1);
                                    tradeAccountNumber = reader.GetString(2);
                                    uploadedDt = reader.GetDateTime(3);
                                    orderImportFileEmail = reader.IsDBNull(4) ? null : reader.GetString(4);
                                }

                                if (!fileContents.Contains('\r'))
                                {
                                    fileContents = Regex.Replace(fileContents, "\n", "\r\n");
                                }

                                reader.Close();
                            }
                        }
                        finally
                        {
#if DEBUG
                            conn.Close();
#endif
                        }
                    }
                }

                // Right, we should now have the file contents for TradeOrderImportID #.

                // Validate it, then update the record.

                string recipientEmail = "itsupport@designersguild.com";

                string uniVerseAccount = "MERTEX";

                string tradeDataSourceJson = null;

                if (String.IsNullOrEmpty(tradeAccountNumber))
                {
#if DEBUG
                    using (var conn = new SqlConnection("Data Source=vm-sql1;Initial Catalog=Flock;Integrated Security=True"))
#else
                using (var conn = new SqlConnection("context connection=true"))
#endif
                    {
                        if (conn.State != ConnectionState.Open)
                            conn.Open();

                        using (var transaction = conn.BeginTransaction())
                        {
                            var now = DateTime.Now;

                            using (var comm = new SqlCommand(@"UPDATE portal.TradeOrderImport_File SET 
                                                            Valid = 0,
                                                            Email = ISNULL(Email, @Recipient),
                                                            Errors = N'FileName was not prefixed with ""{TRADEACCOUNTNUMBER}_"" - please correct and re-upload.',
                                                            LastProcessedDT = @Now
                                                        WHERE 
                                                            TradeOrderImportFileID = @TradeOrderImportFileID", conn))
                            {
                                comm.Transaction = transaction;

                                comm.CommandType = CommandType.Text;

                                comm.Parameters.AddWithValue("@Now", now);
                                comm.Parameters.AddWithValue("@Recipient", recipientEmail);
                                comm.Parameters.AddWithValue("@TradeOrderImportFileID", TradeOrderImportFileID);

                                var updateResult = comm.ExecuteNonQuery();

                                transaction.Commit();
                            }
                        }
                    }
                }
                else
                {
                    if (tradeAccountNumber == "PORTALIMPORT")
                    {
                        recipientEmail = orderImportFileEmail;
                    }
                    else
                    {
#if DEBUG
                        using (var conn = new SqlConnection("Data Source=vm-sql1;Initial Catalog=Flock;Integrated Security=True"))
#else
            using (var conn = new SqlConnection("context connection=true"))
#endif
                        {
                            if (conn.State != ConnectionState.Open)
                                conn.Open();

                            using (var transaction = conn.BeginTransaction())
                            {
                                try
                                {
                                    using (var comm = new SqlCommand(@"SELECT   Email,
                                                                        UniVerseDataSources
                                                               FROM     dbo.TradeAccount
                                                               WHERE    Code = @Code", conn))
                                    {
                                        comm.Transaction = transaction;

                                        comm.CommandType = CommandType.Text;

                                        comm.Parameters.AddWithValue("@Code", tradeAccountNumber);

                                        var reader = comm.ExecuteReader();

                                        while (reader.Read())
                                        {
                                            recipientEmail = reader.IsDBNull(0) ? "itsupport@designersguild.com" : reader.GetString(0);
                                            tradeDataSourceJson = reader.IsDBNull(1) ? "MERTEX" : reader.GetString(1);
                                        }

                                        reader.Close();

                                        transaction.Commit();
                                    }
                                }
                                finally
                                {
#if DEBUG
                                    conn.Close();
#endif
                                }
                            }
                        }

                        if (!String.IsNullOrEmpty(tradeDataSourceJson))
                        {
                            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

                            var dataSource = dataSources.EnquiryTypes
                                                        .Where(p => p.EnquiryType == EnquiryType.PlaceOrder)
                                                        .SingleOrDefault()
                                                        ?.DataSources
                                                        ?.SingleOrDefault();

                            if (dataSource != null)
                            {
                                uniVerseAccount = dataSource.UniVerseAccount;
                            }
                        }
                    }

                    FileValidationResult result = null;

                    if (fileContents.StartsWith("HEADER,"))
                    {
                        result = ValidateDutchFile(TradeOrderImportFileID, fileName, fileContents, uniVerseAccount);
                    }
                    else
                    {
                        result = ValidateFile(TradeOrderImportFileID, fileName, fileContents, uniVerseAccount);
                    }

#if DEBUG
                    if (result.Valid)
                    {
                        //return 0;
                    }
                    else
                    {
                        recipientEmail = "tclark@designersguild.com";
                        //return 0;
                    }
#endif

                    if (result.Valid)
                    {

                        // Great, mark the TradeOrderImport_File record as Valid = 1 and crunch all the orders in the file into portal.OrderImportHead and portal.OrderImportLine

                        result.ProcessOrders();

#if DEBUG
                        using (var conn = new SqlConnection("Data Source=vm-sql1;Initial Catalog=Flock;Integrated Security=True"))
#else
                        using (var conn = new SqlConnection("context connection=true"))
#endif
                        {
                            if (conn.State != ConnectionState.Open)
                                conn.Open();

                            using (var transaction = conn.BeginTransaction())
                            {
                                try
                                {
                                    var now = DateTime.Now;

                                    using (var comm = new SqlCommand(@"UPDATE portal.TradeOrderImport_File SET 
                                                                Valid = 1,
                                                                Email = ISNULL(Email, @Recipient),
                                                                Errors = NULL,
                                                                LastProcessedDT = @Now
                                                            WHERE 
                                                                TradeOrderImportFileID = @TradeOrderImportFileID", conn))
                                    {
                                        comm.Transaction = transaction;

                                        comm.CommandType = CommandType.Text;

                                        comm.Parameters.AddWithValue("@Now", now);
                                        comm.Parameters.AddWithValue("@Recipient", recipientEmail);
                                        comm.Parameters.AddWithValue("@TradeOrderImportFileID", TradeOrderImportFileID);

                                        var updateResult = comm.ExecuteNonQuery();

                                        transaction.Commit();
                                    }

                                    byte orderImportTypeId = 0;

                                    if (result.FileName.Contains("_SORDI_"))
                                        orderImportTypeId = 1;
                                    else if (result.FileName.Contains("_DISTRIBUTOR_"))
                                        orderImportTypeId = 2;
                                    else if (result.FileName.Contains("_INTERNAL_"))
                                        orderImportTypeId = 3;

                                    result.OrdersToImport.ForEach(oh =>
                                    {
                                        using (var ohComm = new SqlCommand(@"
                                    DECLARE @OrderImportHeadID BIGINT;

                                    INSERT INTO [portal].[OrderImportHead]
                                                ([TradeOrderImportFileID]
                                                ,[OrderImportTypeID]
                                                ,[CreatedDT]
                                                ,[Attempt]
                                                ,[OrderImportLineCount]
                                                ,[TradeAccountNumber]
                                                ,[OrderReference]
                                                ,[DeliveryName]
                                                ,[DeliveryAddressLine1]
                                                ,[DeliveryAddressLine2]
                                                ,[DeliveryTown]
                                                ,[DeliveryCounty]
                                                ,[DeliveryPostcode]
                                                ,[DeliveryCountryCode]
                                                ,[DeliveryPhoneNumber]
                                                ,[Email]
                                                ,[ShippingAlertEmailAddress]
                                                ,[ContactPhone]
                                                ,[HaulierCode]
                                                ,[CarriageCode]
                                                ,[HaulierAccountNumber]
                                                ,[TaxesDutiesToInvoiceAccount]
                                                ,[DeliveryTerms]
                                                ,[DeliveryType]
                                                ,[EndCustomer]
                                                ,[DeliveryReference]
                                                ,[FailureNotificationEmail]
                                                ,[HeldForSales])
                                    VALUES (
                                            @TradeOrderImportFileID,
                                            @OrderImportTypeID,
                                            @Now,
                                            0,
                                            @LineCount,
                                            @TradeAccountNumber,
                                            @OrderReference,
                                            @DeliveryName,
                                            @DeliveryAddressLine1,
                                            @DeliveryAddressLine2,
                                            @DeliveryTown,
                                            @DeliveryCounty,
                                            @DeliveryPostcode,
                                            @DeliveryCountryCode,
                                            @DeliveryPhoneNumber,
                                            @Email,
                                            @ShippingAlertEmailAddress,
                                            @ContactPhone,
                                            @HaulierCode,
                                            @CarriageCode,
                                            @HaulierAccountNumber,
                                            @TaxesDutiesToInvoiceAccount,
                                            @DeliveryTerms,
                                            @DeliveryType,
                                            @EndCustomer,
                                            @DeliveryReference,
                                            @RecipientEmail,
                                            @HeldForSales
                                    );

                                    SET @OrderImportHeadID = SCOPE_IDENTITY();

                                    SELECT @OrderImportHeadID", conn))
                                        {
                                            ohComm.CommandType = CommandType.Text;

                                            ohComm.Parameters.AddRange(new List<SqlParameter>()
                                        {
                                            new SqlParameter("@TradeOrderImportFileID", TradeOrderImportFileID),
                                            new SqlParameter("@OrderImportTypeID", orderImportTypeId),
                                            new SqlParameter("@Now", now),
                                            new SqlParameter("@LineCount", oh.OrderLines.Count),
                                            new SqlParameter("@TradeAccountNumber", oh.TradeAccountNumber),
                                            new SqlParameter("@OrderReference", oh.OrderReference),
                                            new SqlParameter("@DeliveryName", String.IsNullOrEmpty(oh.DeliveryName) ? (object)DBNull.Value : oh.DeliveryName),
                                            new SqlParameter("@DeliveryAddressLine1", oh.DeliveryAddressLine1),
                                            new SqlParameter("@DeliveryAddressLine2", String.IsNullOrEmpty(oh.DeliveryAddressLine2) ? (object)DBNull.Value : oh.DeliveryAddressLine2),
                                            new SqlParameter("@DeliveryTown", String.IsNullOrEmpty(oh.DeliveryTown) ? (object)DBNull.Value : oh.DeliveryTown),
                                            new SqlParameter("@DeliveryCounty", String.IsNullOrEmpty(oh.DeliveryCounty) ? (object)DBNull.Value : oh.DeliveryCounty),
                                            new SqlParameter("@DeliveryPostcode", String.IsNullOrEmpty(oh.DeliveryPostcode) ? (object)DBNull.Value : oh.DeliveryPostcode),
                                            new SqlParameter("@DeliveryCountryCode", oh.DeliveryCountryCode),
                                            new SqlParameter("@DeliveryPhoneNumber", String.IsNullOrEmpty(oh.DeliveryPhoneNumber) ? (object)DBNull.Value : oh.DeliveryPhoneNumber),
                                            new SqlParameter("@Email", String.IsNullOrEmpty(oh.Email) ? (object)DBNull.Value : oh.Email),

                                            new SqlParameter("@RecipientEmail", String.IsNullOrEmpty(recipientEmail) ? (object)DBNull.Value : recipientEmail),

                                            new SqlParameter("@ShippingAlertEmailAddress", String.IsNullOrEmpty(oh.ShippingAlertEmailAddress) ? (object)DBNull.Value : oh.ShippingAlertEmailAddress),
                                            new SqlParameter("@ContactPhone", String.IsNullOrEmpty(oh.ContactPhone) ? (object)DBNull.Value : oh.ContactPhone),
                                            new SqlParameter("@HaulierCode", String.IsNullOrEmpty(oh.HaulierCode) ? (object)DBNull.Value : oh.HaulierCode),
                                            new SqlParameter("@CarriageCode", String.IsNullOrEmpty(oh.CarriageCode) ? (object)DBNull.Value : oh.CarriageCode),
                                            new SqlParameter("@HaulierAccountNumber", String.IsNullOrEmpty(oh.HaulierAccountNumber) ? (object)DBNull.Value : oh.HaulierAccountNumber),
                                            new SqlParameter("@TaxesDutiesToInvoiceAccount", String.IsNullOrEmpty(oh.TaxesDutiesToInvoiceAccount) ? (object)DBNull.Value : oh.TaxesDutiesToInvoiceAccount),
                                            new SqlParameter("@DeliveryTerms", String.IsNullOrEmpty(oh.DeliveryTerms) ? (object)DBNull.Value : oh.DeliveryTerms),
                                            new SqlParameter("@DeliveryType", String.IsNullOrEmpty(oh.DeliveryType) ? (object)DBNull.Value : oh.DeliveryType),
                                            new SqlParameter("@EndCustomer", String.IsNullOrEmpty(oh.EndCustomer) ? (object)DBNull.Value : oh.EndCustomer),
                                            new SqlParameter("@DeliveryReference", oh.DeliveryReference ?? (object)DBNull.Value),
                                            new SqlParameter("@HeldForSales", oh.HeldForSales)
                                        }.ToArray());

                                            long orderImportHeadId = -1;

                                            if (conn.State != ConnectionState.Open)
                                                conn.Open();

                                            var ohReader = ohComm.ExecuteReader();

                                            while (ohReader.Read())
                                            {
                                                orderImportHeadId = ohReader.GetInt64(0);
                                            }

                                            ohReader.Close();

                                            oh.OrderLines.ForEach(ol =>
                                            {
                                                using (var olComm = new SqlCommand(@"
                                            INSERT INTO [portal].[OrderImportLine]
                                                                ([OrderImportHeadID]
                                                                ,[ProductCode]
                                                                ,[Quantity]
                                                                ,[Type]
                                                                ,[LineReference]
                                                                ,[Memo]
                                                                ,[DiscountPercentage]
                                                                ,[PromotionCode]
                                                                ,[FileRowNumber])
                                            VALUES (
                                                    @OrderImportHeadID,
                                                    @ProductCode,
                                                    @Quantity,
                                                    @Type,
                                                    @LineReference,
                                                    @Memo,
                                                    @DiscountPercentage,
                                                    @PromoCode,
                                                    @FileRowNumber
                                            )", conn))
                                                {
                                                    olComm.CommandType = CommandType.Text;

                                                    olComm.Parameters.AddRange(new List<SqlParameter>()
                                            {
                                                new SqlParameter("@OrderImportHeadID", orderImportHeadId),
                                                new SqlParameter("@ProductCode", ol.ProductCode),
                                                new SqlParameter("@Quantity", ol.Quantity),
                                                new SqlParameter("@Type", ol.Type),
                                                new SqlParameter("@LineReference", String.IsNullOrEmpty(ol.LineReference) ? (object)DBNull.Value : ol.LineReference),
                                                new SqlParameter("@Memo", String.IsNullOrEmpty(ol.Memo) ? (object)DBNull.Value : ol.Memo),
                                                new SqlParameter("@DiscountPercentage", ol.DiscountPercentage ?? (object)DBNull.Value),
                                                new SqlParameter("@PromoCode", ol.PromoCode ?? (object)DBNull.Value),
                                                new SqlParameter("@FileRowNumber", ol.FileLineNumber)
                                            }.ToArray());

                                                    if (conn.State != ConnectionState.Open)
                                                        conn.Open();

                                                    var olResult = olComm.ExecuteNonQuery();
                                                }
                                            });
                                        }
                                    });
                                }
                                finally
                                {
#if DEBUG
                                    conn.Close();
#endif
                                }
                            }
                        }
                    }
                    else
                    {
                        // Update database, send email to recipient on the customer record

#if DEBUG
                        using (var conn = new SqlConnection("Data Source=vm-sql1;Initial Catalog=Flock;Integrated Security=True"))
#else
                using (var conn = new SqlConnection("context connection=true"))
#endif
                        {
                            var fileRejectedEmailId = -1L;

                            if (conn.State != ConnectionState.Open)
                                conn.Open();

                            using (var transaction = conn.BeginTransaction())
                            {
                                try
                                {
                                    using (var comm = new SqlCommand(@"[portal].[Email_Insert]", conn))
                                    {
                                        comm.Transaction = transaction;

                                        comm.CommandType = CommandType.StoredProcedure;

                                        comm.Parameters.AddWithValue("@To", recipientEmail);
                                        comm.Parameters.AddWithValue("@CC", DBNull.Value);
                                        comm.Parameters.AddWithValue("@BCC", "itsupport@designersguild.com");
                                        comm.Parameters.AddWithValue("@Subject", $"Error importing file {fileName}.csv");
                                        comm.Parameters.AddWithValue("@Body", $@"
<p>
    ""{fileName}.csv"" submitted on {uploadedDt:dd MMM yyyy HH:mm} cannot be imported.
</p>
<p>
    The entire file has been rejected and all the orders it contains have not been processed.
</p>
<p>
    Please review the list of errors below, correct and resubmit the file.
</p>
<p>&nbsp;</p>
{result.Errors()}
<p>&nbsp;</p>
<p>
    For details of the order import file specification, <a href=""https://www.designersguild.com/l114"">click here</a>
</p>
<p>
For further assistance, please contact <a href=""mailto:itsupport@designersguild.com"">itsupport@designersguild.com</a>
</p>");
                                        comm.Parameters.AddWithValue("@EmailTemplateID", 1);
                                        comm.Parameters.AddWithValue("@EmailSourceID", 11);
                                        comm.Parameters.AddWithValue("@EmailTypeID", 1);

                                        var outputId = new SqlParameter("@EmailID", DBNull.Value)
                                        {
                                            Direction = ParameterDirection.Output,
                                            SqlDbType = SqlDbType.BigInt
                                        };

                                        comm.Parameters.Add(outputId);

                                        comm.ExecuteNonQuery();

                                        transaction.Commit();

                                        fileRejectedEmailId = Convert.ToInt64(outputId.Value);
                                    }
                                }
                                finally
                                {
                                    conn.Close();
                                }
                            }

                            if (conn.State != ConnectionState.Open)
                                conn.Open();

                            using (var transaction = conn.BeginTransaction())
                            {
                                try
                                {
                                    using (var comm = new SqlCommand(@"UPDATE portal.TradeOrderImport_File 
                                                                        SET
                                                                        Valid = @Valid,
                                                                        Email = ISNULL(Email, @Recipient),
                                                                        LastProcessedDT = GETDATE(),
                                                                        FileRejectedEmailID = @FileRejectedEmailID,
                                                                        Errors = @ErrorText
                                                                    WHERE 
                                                                        TradeOrderImportFileID = @TradeOrderImportFileID", conn))
                                    {
                                        if (conn.State != ConnectionState.Open)
                                            conn.Open();

                                        comm.Transaction = transaction;

                                        comm.CommandType = CommandType.Text;

                                        comm.Parameters.AddWithValue("@TradeOrderImportFileID", TradeOrderImportFileID);
                                        comm.Parameters.AddWithValue("@Recipient", recipientEmail);
                                        comm.Parameters.AddWithValue("@Valid", !String.IsNullOrEmpty(result.Exception) ? (object)DBNull.Value : result.Valid);
                                        comm.Parameters.AddWithValue("@FileRejectedEmailID", fileRejectedEmailId);
                                        comm.Parameters.AddWithValue("@ErrorText", result.Errors());

                                        var updateResult = comm.ExecuteNonQuery();

                                        transaction.Commit();
                                    }
                                }
                                finally
                                {
#if DEBUG
                                    conn.Close();
#endif
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Task.Run(() => 
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    SendErrorEmail("tclark@designersguild.com", fileName, fileContents, e.ToString());
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                });
#if DEBUG
                throw e;
#endif
            }

            return 0;
        }

        public static async Task<int> SendErrorEmail(string recipientEmail, string fileName, string fileContents, string exception)
        {
            try
            {
                using (var client = new System.Net.Mail.SmtpClient("smtp.office365.com", 587))
                {
                    System.Net.Mail.MailMessage message = null;

                    message = new System.Net.Mail.MailMessage("portal.noreply@designersguild.com", recipientEmail);
                    
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential("portal.noreply@designersguild.com", "TheCakeIsALi3");

                    message.IsBodyHtml = true;
                    message.Subject = $"EXCEPTION while processing {fileName}";
                    message.CC.Add("itsupport@designersguild.com");

                    System.IO.MemoryStream ms = new System.IO.MemoryStream();

                    using (var writer = new System.IO.StreamWriter(ms))
                    {
                        writer.Write(fileContents);

                        writer.Flush();

                        ms.Position = 0;

                        var attachment = new System.Net.Mail.Attachment(ms, fileName, System.Net.Mime.MediaTypeNames.Text.Plain);

                        message.Attachments.Add(attachment);

                            message.Body = $@"<head>
<style type=""text/css"">
    body {{ line-height: 11.75pt; font-size: 13pt; font-family: 'Courier New'}}
</style></head><body>No orders have been created as one or more exceptions were raised while processing the imported file.<br/><br/><br/>
                                {exception.Replace(Environment.NewLine, "<br/><br/>")}
</body>";

                        await client.SendMailAsync(message);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return 0;
        }

        public static FileValidationResult ValidateFile(long tradeOrderImportFileId, string fileName, string fileContents, string uniVerseAccount)
        {
            FileValidationResult result = null;

            var hasHeader = true;

            if (fileContents?.ToUpperInvariant()?.StartsWith("ACCOUNT") == false)
            {
                hasHeader = false;
            }

            var csvConfig = new CsvHelper.Configuration.Configuration()
            {
                Delimiter = ",",
                IgnoreBlankLines = true,
                DetectColumnCountChanges = true,
                HeaderValidated = null,
                MissingFieldFound = null,
                HasHeaderRecord = hasHeader,
                TrimOptions = CsvHelper.Configuration.TrimOptions.Trim | CsvHelper.Configuration.TrimOptions.InsideQuotes,
            };

            csvConfig.TypeConverterOptionsCache.GetOptions<string>().NullValues.Add("");

            csvConfig.BadDataFound = (context) => 
            {
                Console.WriteLine("CSVHelper.BadDataFound hit, but it's probably fine:\r\n{0}", context.Field);
            };

            csvConfig.RegisterClassMap<ImportLineMap>();

            try
            {
                using (var qe = new QueryExecutor(uniVerseAccount))
                {
                    var importLines = new List<ImportLine>();

                    var badDataMessage = (string)null;

                    using (var stringReader = new StringReader(fileContents))
                    {
                        try
                        {
                            using (var csvReader = new CsvReader(stringReader, csvConfig, false))
                            {
                                importLines = csvReader.GetRecords<ImportLine>().ToList();
                            }
                        }
                        catch (BadDataException bde)
                        {
                            badDataMessage = $"{bde}";
                        }
                    }

#if DEBUG
                    importLines.ForEach(il => 
                    { 
                        il.OrderReference = $"{DateTime.Now.Millisecond}";
                        il.LineReference = $"{DateTime.Now.Millisecond}";
                    });
#endif

                    result = new FileValidationResult(
                        tradeOrderImportFileId: tradeOrderImportFileId,
                        fileName: fileName, 
                        headerRow: fileContents.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[0],
                        fileContents: importLines);

                    if (!String.IsNullOrEmpty(badDataMessage))
                    {
                        result.LineValidationFailures.Add(new LineValidationResult(null, "[Unknown]", badDataMessage));
                    }

                    for (var i = 0; i < importLines.Count; i++)
                    {
                        var lineNumber = i + 1;

                        var line = importLines[i];

#if DEBUG
                        Console.WriteLine($"Processing Line Number {lineNumber}");
#endif

                        //if (fields[0].ToUpperInvariant() == "ACCOUNT NUMBER" || fields[0].ToUpperInvariant() == "TRADEACCOUNTNUMBER")
                        //{
                        //    // header line: skip
                        //    continue;
                        //}

                        //if (fields.Length < 15)
                        //{
                        //    result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "[All Fields]", "Insufficient field count. Minimum field count: 15. Please refer to the file format specification."));
                        //    // prevent unnecessary exceptions being thrown by insufficient field count
                        //    continue;
                        //}

                        var xDoc = XDocument.Parse(
                            qe.Run(
                                "SLCUST",
                                $"CUSREF=\"{line.TradeAccountNumber}\"",
                                "ARCHIVED.CUSTOMER",
                                "",
                                "XML"
                            )
                        );

                        if (String.IsNullOrEmpty(line.TradeAccountNumber))
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "1. Account Number", $"\"{line.TradeAccountNumber}\" was empty. Please refer to the file format specification."));
                        }

                        if (!xDoc.Descendants("record").Any())
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "1. Account Number", $"\"{line.TradeAccountNumber}\" was not found in our Customers file. Please report this to Designers Guild."));
                        }

                        if (xDoc.Descendants("record").Count() > 1)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "1. Account Number", $"\"{line.TradeAccountNumber}\" was found more than once in our Customers file. Please report this to Designers Guild."));
                        }

                        if (xDoc.Descendants("record").Count() == 1)
                        {
                            if (xDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value == "A")
                            {
                                result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "1. Account Number", $"\"{line.TradeAccountNumber}\" has been archived. This account is considered closed."));
                            }
                        }

                        if (line.TradeAccountNumber?.Length > 50)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "1. Account Number", $"\"{line.TradeAccountNumber}\" is over-length. Maximum length: 50"));
                        }

                        // Right, up to here we're golden. We know the SLCUST record exists (only once) and that it is not Archived.
#if DEBUG
                        //fields[1] = $"{fields[1]}{DateTime.Now.Ticks}";
#endif
                        // Where to now?

                        xDoc = XDocument.Parse(
                            qe.Run(
                                "OECUSTORD",
                                $"CUSREF=\"{line.TradeAccountNumber}\" AND CUSORDREF=\"{line.OrderReference}\"",
                                "ORDERNO ORDDATE",
                                "",
                                "XML"
                            )
                        );

                        long? orderImportHeadId = null;
#if DEBUG
                        using (var conn = new SqlConnection("Data Source=vm-sql1;Initial Catalog=Flock;Integrated Security=True"))
#else
                        using (var conn = new SqlConnection("context connection=true"))
#endif
                        {
                            if (conn.State != ConnectionState.Open)
                                conn.Open();

                            try
                            {
                                using (var comm = new SqlCommand(@"SELECT   OrderImportHeadID 
                                                                    FROM    portal.OrderImportHead 
                                                                    WHERE   TradeAccountNumber = @TradeAccountNumber AND 
                                                                            OrderReference = @OrderReference AND
                                                                            FailedValidationDT IS NULL", conn))
                                {
                                    comm.CommandType = CommandType.Text;

                                    comm.Parameters.AddWithValue("@TradeAccountNumber", line.TradeAccountNumber);
                                    comm.Parameters.AddWithValue("@OrderReference", line.OrderReference);

                                    var reader = comm.ExecuteReader();

                                    while (reader.Read())
                                    {
                                        orderImportHeadId = reader.GetInt64(0);
                                    }
                                }
                            }
                            finally
                            {
#if DEBUG
                                conn.Close();
#endif
                            }
                        }

                        if (orderImportHeadId != null)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "2. Order Reference", $"Duplicate order reference. We have previously received an order from you with the same reference \"{line.OrderReference}\". Each order reference must be unique."));
                        }
                        else
                        {
                            if (xDoc.Descendants("record").Any())
                            {
                                var extantByOrderRefDetails = new List<string>();// String.Join(Delimiter, xDoc.Descendants("record")).ToList();

                                xDoc.Descendants("record").ToList().ForEach(c =>
                                {
                                    extantByOrderRefDetails.Add($"{c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value} ({c.XPathSelectElement("descendant::fd[@id = 'C2']").Attribute("value").Value})");
                                });

                                result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "2. Order Reference", $"Duplicate order reference \"{line.OrderReference}\" already used in the following order(s): {String.Join(",", extantByOrderRefDetails.ToArray())}. Each order reference must be unique."));
                            }
                        }

                        if (line.OrderReference?.Length > 50)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "2. Order Reference", $"\"{line.OrderReference}\" is over-length. Maximum length: 50"));
                        }

                        // Great, now we know they've used a distinct order reference for this order in this file...

                        // What's next? Country.

                        if (String.IsNullOrEmpty(line.DeliveryCountryCode))
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "8. Delivery Country Code", "Delivery Country Code was empty. Delivery Country Code must be valid from the ISO 3166 Alpha-2 specification."));
                        }

                        line.DeliveryCountryCode = line.DeliveryCountryCode?.ToUpperInvariant();

                        xDoc = XDocument.Parse(
                            qe.Run(
                                "SLCOUNTRY",
                                $"@ID=\"{line.DeliveryCountryCode}\"",
                                "@ID",
                                "",
                                "XML"
                            )
                        );

                        if (!xDoc.Descendants("record").Any() && !String.IsNullOrEmpty(line.DeliveryCountryCode))
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "9. Delivery Country Code", $"\"{line.DeliveryCountryCode}\" was not found. Delivery Country Code must be valid from the ISO 3166 Alpha-2 specification."));
                        }

                        if (line.DeliveryName?.Length > 50)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "3. Delivery Name", $"\"{line.DeliveryName}\" is over-length. Maximum length: 50"));
                        }
                        if (line.DeliveryAddressLine1?.Length > 60)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "4. Delivery Address Line 1", $"\"{line.DeliveryAddressLine1}\" is over-length. Maximum length: 60"));
                        }
                        if (line.DeliveryAddressLine2?.Length > 60)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "5. Delivery Address Line 2", $"\"{line.DeliveryAddressLine2}\" is over-length. Maximum length: 60"));
                        }
                        if (line.DeliveryTown?.Length > 50)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "6. Delivery Town", $"\"{line.DeliveryTown}\" is over-length. Maximum length: 50"));
                        }
                        if (line.DeliveryCounty?.Length > 40)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "7. Delivery County", $"\"{line.DeliveryCounty}\" is over-length. Maximum length: 40"));
                        }
                        if (line.DeliveryPostcode?.Length > 20)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "8. Delivery Postcode", $"\"{line.DeliveryPostcode}\" is over-length. Maximum length: 20"));
                        }
                        if (line.DeliveryCountryCode?.Length > 3)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "9. Delivery Country Code", $"\"{line.DeliveryCountryCode}\" is over-length. Maximum length: 3"));
                        }
                        if (line.DeliveryPhoneNumber?.Length > 20)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "10. Delivery Phone Number", $"\"{line.DeliveryPhoneNumber}\" is over-length. Maximum length: 20"));
                        }
                        if (line.Email?.Length > 256)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "11. Email", $"\"{line.Email}\" is over-length. Maximum length: 256"));
                        }

                        if (line.ShippingAlertEmailAddress?.Length > 4096)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "17. Shipping Alert Email Address", $"\"{line.ShippingAlertEmailAddress}\" is over-length. Maximum length: 4096"));
                        }
                        
                        if (line.ContactPhone?.Length > 20)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "18. Contact Phone", $"\"{line.ContactPhone}\" is over-length. Maximum length: 20"));
                        }

                        // We have a country, it is valid (at least... it's valid by Mertex's standards. I don't know if SLCOUNTRY is actually Alpha-2 specced properly.

                        if (!ValidTypes.Contains(line.Type?.ToUpperInvariant()))
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "14. Type", $"\"{line.Type?.ToUpperInvariant()}\" is not a valid Type. Please refer to the file format specification."));
                        }

                        if (String.IsNullOrEmpty(line.Quantity))
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "12. Quantity", "Quantity was empty. Please refer to the file format specification."));
                        }
                        else
                        {

                            if (!Decimal.TryParse(line.Quantity, out decimal quantity))
                            {
                                result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "12. Quantity", "Quantity non-numeric. Please refer to the file format specification."));
                            }

                            if (quantity < 0.5M)
                            {
                                result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "12. Quantity", "Quantity too low. Please refer to the file format specification."));
                            }
                        }

                        if (String.IsNullOrEmpty(line.ProductCode))
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "12. Product Code", "Product Code was empty. Please refer to the file format specification."));
                        }

                        var lineType = line.Type?.ToUpperInvariant();

                        var code = line.ProductCode;

                        switch (lineType)
                        {
                            case "SAMPLES CUTTING":
                                code += "/C";
                                break;
                            case "RETURNABLE SAMPLE":
                                code += "/R";
                                break;
                            case "HANGER":
                                {
                                    code = Regex.Replace(code, @"/.*", "/H");
                                }
                                break;
                            case "SHOWLENGTH":
                                {
                                    code = Regex.Replace(code, @"/.*", "/S");
                                }
                                break;
                            case "WATERFALL ONLY":
                                {
                                    code = Regex.Replace(code, @"/.*", "/W");
                                }
                                break;
                            default:
                                break;
                        }

                        if (lineType == "BUNDLE")
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "14. Type", "All Bundles have been discontinued. Please contact Designers Guild for clarification."));
                        }

                        if (line.ProductCode?.Length > 50)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "12. Product Code", $"\"{line.ProductCode}\" is over-length. Maximum length: 50"));
                        }

                        if (line.LineReference?.Length > 50)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "15. Line Reference", $"\"{line.LineReference}\" is over-length. Maximum length: 50"));
                        }

                        if (line.Memo?.Length > 1000)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "16. Line Memo", $"\"{line.Memo}\" is over-length. Maximum length: 1000"));
                        }
                        


                        // Apparently bundles have been discontinued entirely - 15/04/2020 - Tom

                        //if (lineType == "BUNDLE" && !String.IsNullOrEmpty(fields[11])) 
                        //{
                        //    // Check that we actually have an INVSUND record, with components, for this product code.
                        //    xDoc = XDocument.Parse(
                        //        qe.Run(
                        //            "INVSUND",
                        //            $"@ID=\"{fields[11]}\"",
                        //            "PRODCODES",
                        //            "",
                        //            "XML"
                        //        )
                        //    );

                        //    if (!xDoc.Descendants("record").Any())
                        //    {
                        //        result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "12. Product Code", $"Bundle \"{fields[11]}\" was not found. Please double check the part number."));
                        //    }
                        //    else
                        //    {
                        //        if (String.IsNullOrEmpty(xDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value))
                        //        {
                        //            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "12. Product Code", $"Bundle \"{fields[11]}\" has no components. Please inform Designers Guild."));
                        //        }
                        //    }
                        //}

                        if (!String.IsNullOrEmpty(line.ProductCode))
                        {
                            using (var ce = new CommandExecutor())
                            {
                                var codeResult = ce.Arbitrary($"LIST PRODCODE STPRODMASTER WITH PRODCODE = \"{code}\" ID.SUP COL.HDR.SUPP COUNT.SUP");

                                var codeArray = codeResult.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                                if (codeArray.Count() != 1)
                                {
                                    result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "12. Product Code", $"Product \"{code}\" was not found. Please check the part number."));
                                }
                                else
                                {
                                    if (String.IsNullOrEmpty(codeArray[0]))
                                    {
                                        result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "12. Product Code", $"Product \"{code}\" was not found. Please check the part number."));
                                    }
                                }
                            }
                        }

                        if (!String.IsNullOrEmpty(line.ProductCode))
                        {
                            using (var ce = new CommandExecutor())
                            {
                                var itemTypeResult = ce.Arbitrary($"LIST ITEMTYPE STPRODMASTER WITH PRODCODE = \"{code}\" ID.SUP COL.HDR.SUPP COUNT.SUP");

                                var itemTypeResultArray = itemTypeResult.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                                if (itemTypeResultArray.Count() != 1)
                                {
                                    result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "12. Product Code", $"Product \"{code}\" Item Type Lookup Failure. Please contact Customer Services."));
                                }
                                else
                                {
                                    if (ForbiddenItemTypeCodes.Contains(itemTypeResultArray[0]))
                                    {
                                        result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "12. Product Code", $"Product \"{code}\" carries Item Type Code \"{itemTypeResultArray[0]}\", which is a forbidden Item Type Code. Please contact Customer Services to check for a new Product Code for this particular product."));
                                    }
                                }
                            }
                        }

                        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                        //  Right. Now for the Distributor specific fields. We should only be checking these if the fileName contains "_DISTRIBUTOR_".
                        //
                        //  18 HaulierCode
                        //  19 CarriageCode
                        //  20 HaulierAccountNumber
                        //  21 TaxesDutiesToInvoiceAccount
                        //  22 DeliveryTerms
                        //  23 DeliveryType
                        //  24 EndCustomer
                        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

                        if (fileName.Contains("_DISTRIBUTOR_") || fileName.Contains("_INTERNAL_"))
                        {
                            if (line.HaulierCode?.Length > 50)
                            {
                                result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "19. Haulier Code", $"\"{line.HaulierCode}\" is over-length. Maximum length: 50"));
                            }
                            if (line.CarriageCode?.Length > 50)
                            {
                                result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "20. Carriage Code", $"\"{line.CarriageCode}\" is over-length. Maximum length: 50"));
                            }
                            if (line.HaulierAccountNumber?.Length > 50)
                            {
                                result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "21. Haulier Account Number", $"\"{line.HaulierAccountNumber}\" is over-length. Maximum length: 50"));
                            }
                            if (line.TaxesDutiesToInvoiceAccount?.Length > 50)
                            {
                                result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "22. Taxes Duties to Invoice Account", $"\"{line.TaxesDutiesToInvoiceAccount}\" is over-length. Maximum length: 50"));
                            }
                            if (line.DeliveryTerms?.Length > 50)
                            {
                                result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "23. Delivery Terms", $"\"{line.DeliveryTerms}\" is over-length. Maximum length: 50"));
                            }
                            if (line.DeliveryType?.Length > 50)
                            {
                                result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "24. Delivery Type", $"\"{line.DeliveryType}\" is over-length. Maximum length: 50"));
                            }
                            if (line.EndCustomer?.Length > 50)
                            {
                                result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "25. End Customer", $"\"{line.EndCustomer}\" is over-length. Maximum length: 50"));
                            }

                            if (!String.IsNullOrEmpty(line.HaulierCode))
                            {
                                xDoc = XDocument.Parse(
                                    qe.Run(
                                        "OEHAULIER",
                                        $"@ID=\"{line.HaulierCode}\"",
                                        "@ID",
                                        "",
                                        "XML"
                                    )
                                );

                                if (!xDoc.Descendants("record").Any())
                                {
                                    // If present, Haulier Code must be valid.
                                    result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "19. Haulier Code", $"Haulier Code \"{line.HaulierCode}\" was not recognised. Please contact your Sales Representative."));
                                }
                            }

                            if (!String.IsNullOrEmpty(line.CarriageCode) && !String.IsNullOrEmpty(line.HaulierCode))
                            {
                                xDoc = XDocument.Parse(
                                    qe.Run(
                                        "OECARRIAGE",
                                        $"@ID=\"{line.CarriageCode}*{line.HaulierCode}\"",
                                        "@ID",
                                        "",
                                        "XML"
                                    )
                                );

                                if (!xDoc.Descendants("record").Any())
                                {
                                    // If present, Carriage Code must be valid.
                                    result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "19. Haulier Code, 20. Carriage Code", $"Haulier \"{line.HaulierCode}\" has no Carriage Code \"{line.CarriageCode}\". Please contact your Sales Representative."));
                                }
                            }

                            if (!String.IsNullOrEmpty(line.TaxesDutiesToInvoiceAccount))
                            {
                                // Defunct 28.09.2022 - apparently defunct since 01/10/2019, whoops!
                                //if (line.TaxesDutiesToInvoiceAccount != "Y" && line.TaxesDutiesToInvoiceAccount != "N")
                                //{
                                //    result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "22. Taxes & Duties to Invoice Account", "Taxes & Duties to Invoice Account must be either empty, \"Y\" or \"N\"."));
                                //}
                            }

                            if (!String.IsNullOrEmpty(line.DeliveryTerms))
                            {
                                xDoc = XDocument.Parse(
                                    qe.Run(
                                        "STPRODCLASS8",
                                        $"CODEDESC = \"{line.DeliveryTerms}\"",
                                        "CODEDESC",
                                        "",
                                        "XML"
                                    )
                                );

                                if (!xDoc.Descendants("record").Any())
                                {
                                    result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "23. Delivery Terms", $"Delivery Term \"{line.DeliveryTerms}\" was not recognised. Please contact your Sales Representative."));
                                }
                            }

                            if (!String.IsNullOrEmpty(line.DeliveryType))
                            {
                                if (line.DeliveryType != "C" &&
                                    line.DeliveryType != "D" &&
                                    line.DeliveryType != "N" &&
                                    line.DeliveryType != "T" &&
                                    line.DeliveryType != "W")
                                {
                                    result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "24. Delivery Type", $"Delivery Type \"{line.DeliveryType}\" was not recognised. Please refer to the file format specification."));
                                }
                            }

                            if (!String.IsNullOrEmpty(line.EndCustomer))
                            {
                                // end customer detected...
                                var endCustomer = line.EndCustomer;

                                // check on ENDCUST for this end customer reference...
                                xDoc = XDocument.Parse(
                                    qe.Run(
                                        "ENDCUST",
                                        $"@ID=\"{endCustomer}\"",
                                        "@ID",
                                        "",
                                        "XML"
                                    )
                                );

                                if (!xDoc.Descendants("record").Any())
                                {
                                    // if not found on ENDCUST, then check on SLCUST...
                                    xDoc = XDocument.Parse(
                                        qe.Run(
                                            "SLCUST",
                                            $"@ID=\"{endCustomer}\"",
                                            "@ID",
                                            "",
                                            "XML"
                                        )
                                    );

                                    if (!xDoc.Descendants("record").Any())
                                    {
                                        // if not found on SLCUST either, we have an invalid End Customer.
                                        result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "25. End Customer", $"End Customer \"{endCustomer}\" is invalid. Please contact your Sales Representative."));
                                    }
                                    else
                                    {
                                        // End Customer was found on SLCUST: we now need to test for field 751 "END.CUST.INV.DES" for the main account placing this line...
                                        xDoc = XDocument.Parse(
                                            qe.Run(
                                                "SLCUST",
                                                $"@ID=\"{line.TradeAccountNumber}\"",
                                                "END.CUST.INV.DES",
                                                "",
                                                "XML"
                                            )
                                        );

                                        // if END.CUST.INV.DES == "Y" for the account placing the order, and STATEMENT.AC on the end customer SLCUST record is not equal to the ordering account's code...
                                        // that's an error!
                                        if (xDoc.Descendants("record").Any())
                                        {
                                            if (xDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value == "Y")
                                            {
                                                xDoc = XDocument.Parse(
                                                    qe.Run(
                                                        "SLCUST",
                                                        $"@ID=\"{endCustomer}\"",
                                                        "STATEMENT.AC",
                                                        "",
                                                        "XML"
                                                    )
                                                );

                                                if (xDoc.Descendants("record").Any())
                                                {
                                                    if (xDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value != line.TradeAccountNumber)
                                                    {
                                                        // End Customer WAS found, but is NOT an invoice account for Placing customer.
                                                        result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "25. End Customer", $"End Customer \"{endCustomer}\" is not an invoice account for \"{line.TradeAccountNumber}\". Please contact your Sales Representative."));
                                                    }
                                                }
                                                else
                                                {
                                                    result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "25. End Customer", $"End Customer \"{endCustomer}\" is not an invoice account for \"{line.TradeAccountNumber}\". Please contact your Sales Representative."));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "25. End Customer", $"End Customer \"{endCustomer}\" is invalid. Please contact your Sales Representative."));
                                        }
                                    }
                                }
                                else
                                {
                                    // record was found on ENDCUST:
                                    // we should be okay!
                                }
                            }
                            else
                            {
                                // an end customer is sometimes actually required.
                                // if SLCUST<790> == "Y" && SLCUST<107> != "" for the main account number placing the order, then end cust is required.

                                xDoc = XDocument.Parse(
                                    qe.Run(
                                        "SLCUST",
                                        $"@ID=\"{line.TradeAccountNumber}\"",
                                        "END.CUST.REQ.DES INV.ACCOUNTS",
                                        "",
                                        "XML"
                                    )
                                );

                                if (xDoc.Descendants("record").Any())
                                {
                                    if (xDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value == "Y")
                                    {
                                        if (!String.IsNullOrEmpty(xDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value))
                                        {
                                            // 790 = "End Customer Req'd", 107 = "Invoice Accounts"
                                            // End Customer Required found to be Y, Invoice Accounts not empty, no End Customer provided... error.
                                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "25. End Customer", $"End Customer is required. Please refer to the file format specification."));
                                        }
                                    }
                                }
                                else
                                {
                                    // 790 = "End Customer Req'd", 107 = "Invoice Accounts"
                                    // End Customer Required found to be Y, Invoice Accounts not empty, no End Customer provided... error.
                                    result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "25. End Customer", $"End Customer is required. Please refer to the file format specification."));
                                }
                            }
                        }

                        /*
                         */


                        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                        // Internal only fields beneath
                        //
                        //   25 HeldForSales[5]
                        //   26 PromoCode[50]
                        //   27 DiscountPercentage[10]
                        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

                        if (fileName.Contains("_INTERNAL_"))
                        {
                            // do we actually want / need to be doing validation against these... yes probably?
                            // what about having > 100% discount?
                            // what about having an invalid promo code?
                            // what about held for sales being anything other than Y or N ?
                            //
                            // . . . 

                            if (!String.IsNullOrEmpty(line.HeldForSales))
                            {
                                if (line.HeldForSales != "Y" && line.HeldForSales != "N")
                                {
                                    result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "26. Held for Sales", "Held for Sales must be either empty, \"Y\" or \"N\"."));
                                }
                            }

                            if (!String.IsNullOrEmpty(line.PromoCode))
                            {
                                xDoc = XDocument.Parse(
                                    qe.Run(
                                        "OEPROMOTIONS.DES",
                                        $"@ID=\"{line.PromoCode}\" AND FOC.PROMO = \"Y\"",
                                        "ID SS.PROMO.START SS.PROMO.END",
                                        "",
                                        "XML"
                                    )
                                );

                                if (!xDoc.Descendants("record").Any())
                                {
                                    result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "27. Promo Code", $"PromoCode \"{line.PromoCode}\" is not a valid Free-of-Charge PromoCode."));
                                }
                                else
                                {
                                    // need to test here.
                                    //
                                    var now = DateTime.Now;

                                    var enGB = new CultureInfo("en-GB");

                                    if (!String.IsNullOrEmpty(xDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value))
                                    {
                                        if (DateTime.TryParseExact(xDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value, "dd/MM/yyyy", enGB, DateTimeStyles.None, out DateTime start))
                                        {
                                            // we have a start date
                                            if (start > now)
                                            {
                                                result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "27. Promo Code", $"PromoCode \"{line.PromoCode}\" is not yet active (becomes active {start:dd/MM/yyyy}."));
                                            }
                                        }
                                    }

                                    if (!String.IsNullOrEmpty(xDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value))
                                    {
                                        if (DateTime.TryParseExact(xDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value, "dd/MM/yyyy", enGB, DateTimeStyles.None, out DateTime end))
                                        {
                                            // we have an end date
                                            if (now > end)
                                            {
                                                result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "27. Promo Code", $"PromoCode \"{line.PromoCode}\" is expired (expired {end:dd/MM/yyyy}."));
                                            }
                                        }
                                    }
                                }
                            }

                            if (!String.IsNullOrEmpty(line.DiscountPercentage))
                            {
                                line.DiscountPercentage = line.DiscountPercentage.Replace("%", "");

                                if (Decimal.TryParse(line.DiscountPercentage, out decimal disco))
                                {
                                    if (disco < 0)
                                    {
                                        result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "28. Discount %", $"Discount \"{line.DiscountPercentage}\" is invalid. Min: 0"));
                                    }

                                    if (disco > 100)
                                    {
                                        result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "28. Discount %", $"Discount \"{line.DiscountPercentage}\" is invalid. Max: 100"));
                                    }

                                    if (!String.IsNullOrEmpty(line.PromoCode))
                                    {
                                        if (disco != 100)
                                        {
                                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "28. Discount %", $"Discount of \"{line.DiscountPercentage}\"% is not valid in conjunction with Free of Charge Promotion Code \"{line.PromoCode}\"."));
                                        }
                                    }
                                }
                            }
                            
                        }

#if DEBUG
                        Console.WriteLine($"Finished Processing Line Number {lineNumber}");
#endif

                    }

                    result.FileContents = importLines;
                }
            }
            catch (Exception e)
            {
                if (result == null)
                {
                    result = new FileValidationResult(tradeOrderImportFileId, fileName, null, new List<ImportLine>());
                }
                result.Exception = e.ToString();
                throw e;
            }

            return result;
        }

        /*
            Dutch file format

            Order header lines begin with HEADER
            Orders consume all following lines beginning with LINE

            Until the next HEADER or the EoF

            So... If the first word you come across is "HEADER," then you *know* it's a Dutch file... so process it separately.

            0 FileLineType  [HEADER]                    [LINE]
            1               [TradeAccountNumber]        [ProductCode]
            2               [OrderReference]            [Quantity]
            3               [DeliveryReference]         [OrderUnits]
            4               [DeliveryName]              [SampleIndicator (N / S(howlength) / H(anger) / W(aterfall) or empty)]
            5               [DeliveryAddressLine1]      [ReserveIndicator (Y / N or empty)]
            6               [DeliveryAddressLine2]      [LineMemo (Pipe Delimited?)]
            7               [DeliveryTown]              [LineReference]
            8               [DeliveryCounty]            [LinePrice]
            9               [DeliveryPostCode]       
            10              [DeliveryCountryCode]
            11              [DeliveryPhoneNumber]
            12              [Email]
            13              [DomesticTradeIndicator]
        */

        public static void ValidateDutchOrderHead(FileValidationResult result, long tradeOrderImportFileId, int lineNumber, string[] lines, string uniVerseAccount)
        {
            var thisOrderHeadGuid = Guid.NewGuid().ToString();

            if (lineNumber < lines.Length)
            {
                var l = lines[lineNumber];

                var fields = l.Split(Delimiter, StringSplitOptions.None);

#if DEBUG
                fields[2] = $"{DateTime.Now.Millisecond}";
#endif

                for (var i = 0; i < fields.Length; i++)
                {
                    fields[i] = fields[i].Trim();
                }

                if (fields[0] == "HEADER")
                {
                    using (var qe = new QueryExecutor(uniVerseAccount))
                    {
                        if (fields.Length < 13)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "[All Fields]", "Insufficient field count. Minimum field count: 13. Please refer to the file format specification."));
                        }

                        var xDoc = XDocument.Parse(
                            qe.Run(
                                "SLCUST",
                                $"CUSREF=\"{fields[1]}\"",
                                "ARCHIVED.CUSTOMER",
                                "",
                                "XML"
                            )
                        );

                        if (!xDoc.Descendants("record").Any())
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "1. Account Number", $"\"{fields[1]}\" was empty. Please refer to the file format specification."));
                        }

                        if (xDoc.Descendants("record").Count() > 1)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "1. Account Number", $"\"{fields[1]}\" was found more than once in our Customers file. Please report this to Designers Guild."));
                        }

                        if (xDoc.Descendants("record").Count() == 1)
                        {
                            if (xDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value == "A")
                            {
                                result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "1. Account Number", $"\"{fields[1]}\" has been archived. This account is considered closed."));
                            }
                        }

                        if (fields[0].Length > 50)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "1. Account Number", $"\"{fields[1]}\" is over-length. Maximum length: 50"));
                        }

                        xDoc = XDocument.Parse(
                            qe.Run(
                                "OECUSTORD",
                                $"CUSREF=\"{fields[1]}\" AND CUSORDREF=\"{fields[2]}\"",
                                "ORDERNO ORDDATE",
                                "",
                                "XML"
                            )
                        );

                        long? orderImportHeadId = null;
#if DEBUG
                        using (var conn = new SqlConnection("Data Source=vm-sql1;Initial Catalog=Flock;Integrated Security=True"))
#else
                        using (var conn = new SqlConnection("context connection=true"))
#endif
                        {
                            if (conn.State != ConnectionState.Open)
                                conn.Open();

                            try
                            {
                                using (var comm = new SqlCommand(@"SELECT   OrderImportHeadID 
                                                                    FROM    portal.OrderImportHead 
                                                                    WHERE   TradeAccountNumber = @TradeAccountNumber AND 
                                                                            OrderReference = @OrderReference AND
                                                                            FailedValidationDT IS NULL", conn))
                                {
                                    comm.CommandType = CommandType.Text;

                                    comm.Parameters.AddWithValue("@TradeAccountNumber", fields[1]);
                                    comm.Parameters.AddWithValue("@OrderReference", fields[2]);

                                    var reader = comm.ExecuteReader();

                                    while (reader.Read())
                                    {
                                        orderImportHeadId = reader.GetInt64(0);
                                    }
                                }
                            }
                            finally
                            {
#if DEBUG
                                conn.Close();
#endif
                            }
                        }

                        if (orderImportHeadId != null)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "2. Order Reference", $"Duplicate order reference. We have previously received an order from you with the same reference \"{fields[2]}\". Each order reference must be unique."));
                        }
                        else
                        {
                            if (xDoc.Descendants("record").Any())
                            {
                                var extantByOrderRefDetails = new List<string>();

                                xDoc.Descendants("record").ToList().ForEach(c =>
                                {
                                    extantByOrderRefDetails.Add($"{c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value} ({c.XPathSelectElement("descendant::fd[@id = 'C2']").Attribute("value").Value})");
                                });

                                result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "2. Order Reference", $"Duplicate order reference \"{fields[2]}\" already used in the following order(s): {String.Join(",", extantByOrderRefDetails.ToArray())}. Each order reference must be unique."));
                            }
                        }

                        if (fields[2].Length > 50)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "2. Order Reference", $"\"{fields[2]}\" is over-length. Maximum length: 50"));
                        }

                        if (String.IsNullOrEmpty(fields[10]))
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "10. Delivery Country Code", "Delivery Country Code was empty. Delivery Country Code must be valid from the ISO 3166 Alpha-2 specification."));
                        }

                        fields[10] = fields[10].ToUpperInvariant();

                        xDoc = XDocument.Parse(
                            qe.Run(
                                "SLCOUNTRY",
                                $"@ID=\"{fields[10]}\"",
                                "@ID",
                                "",
                                "XML"
                            )
                        );

                        if (!xDoc.Descendants("record").Any() && fields[10] != "")
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "10. Delivery Country Code", $"\"{fields[10]}\" was not found. Delivery Country Code must be valid from the ISO 3166 Alpha-2 specification."));
                        }

                        if (!String.IsNullOrEmpty(fields[3]))
                        {
                            xDoc = XDocument.Parse(
                                qe.Run(
                                    "DELADD",
                                    $"@ID = \"{fields[3]}\" AND CUSREF = \"{fields[1]}\"",
                                    "@ID",
                                    "",
                                    "XML"
                                )
                            );

                            if (!xDoc.Descendants("record").Any())
                            {
                                result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "3. Delivery Reference", $"\"{fields[3]}\" was not found in {fields[1]}'s address book. Delivery Reference must be valid if present."));
                            }
                        }

                        if (fields[3].Length > 20)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "3. Delivery Reference", $"\"{fields[3]}\" is over-length. Maximum length: 20"));
                        }
                        if (fields[4].Length > 50)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "4. Delivery Name", $"\"{fields[4]}\" is over-length. Maximum length: 50"));
                        }
                        if (fields[5].Length > 60)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "5. Delivery Address Line 1", $"\"{fields[5]}\" is over-length. Maximum length: 60"));
                        }
                        if (fields[6].Length > 60)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "6. Delivery Address Line 2", $"\"{fields[6]}\" is over-length. Maximum length: 60"));
                        }
                        if (fields[7].Length > 50)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "7. Delivery Town", $"\"{fields[7]}\" is over-length. Maximum length: 50"));
                        }
                        if (fields[8].Length > 40)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "8. Delivery County", $"\"{fields[8]}\" is over-length. Maximum length: 40"));
                        }
                        if (fields[9].Length > 20)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "9. Delivery Postcode", $"\"{fields[9]}\" is over-length. Maximum length: 20"));
                        }
                        if (fields[10].Length > 3)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "10. Delivery Country Code", $"\"{fields[10]}\" is over-length. Maximum length: 3"));
                        }
                        if (fields[11].Length > 20)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "11. Delivery Phone Number", $"\"{fields[11]}\" is over-length. Maximum length: 20"));
                        }
                        if (fields[12].Length > 256)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "12. Email", $"\"{fields[12]}\" is over-length. Maximum length: 256"));
                        }
                        if (fields.Length >= 14)
                        {
                            if (!String.IsNullOrEmpty(fields[13]))
                            {
                                result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "13. Domestic / Trade", $"\"{fields[13]}\" was unexpected."));
                            }
                        }
                    }

                    if (String.IsNullOrWhiteSpace(fields[3]))
                        fields[3] = null;

                    if (result.LineValidationFailures.Count == 0)
                    {
                        result.OrdersToImport.Add(new OrderHeadToImport(
                            tradeOrderImportFileId: tradeOrderImportFileId,
                            orderType: null, // this is important, orderType being null tells the public property DeliveryReference to return the private property deliveryReference. Ugly but functional.
                            tradeAccountNumber: fields[1],
                            orderReference: fields[2],
                            delRef: fields[3],
                            deliveryName: fields[4],
                            deliveryAddressLine1: fields[5],
                            deliveryAddressLine2: fields[6],
                            deliveryTown: fields[7],
                            deliveryCounty: fields[8],
                            deliveryPostcode: fields[9],
                            deliveryCountryCode: fields[10],
                            deliveryPhoneNumber: fields[11],
                            email: fields[12],
                            thisOrderHeadGuid,
                            false,
                            true
                        ));
                    }

                    Console.WriteLine($"Found a HEADER on line {lineNumber + 1}: {thisOrderHeadGuid}");
                }

                if (lineNumber < lines.Length)
                {
                    lineNumber++;

                    ValidateDutchOrderLines(thisOrderHeadGuid, result, tradeOrderImportFileId, lineNumber, lines, uniVerseAccount);
                }
            }
            else
            {
                // we're done?
                Console.WriteLine("Done with file!");
            }
        }

        public static void ValidateDutchOrderLines(string orderHeadGuid, FileValidationResult result, long tradeOrderImportFileId, int lineNumber, string[] lines, string uniVerseAccount)
        {
            for (var i = lineNumber; i < lines.Length; i++)
            {
                var fields = lines[i].Split(Delimiter, StringSplitOptions.None);

                if (fields[0] == "LINE")
                {
                    if (String.IsNullOrEmpty(fields[2]))
                    {
                        result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "2. Quantity", "Quantity was empty. Please refer to the file format specification."));
                    }
                    else
                    {

                        if (!Decimal.TryParse(fields[2], out decimal quantity))
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "2. Quantity", "Quantity non-numeric. Please refer to the file format specification."));
                        }

                        if (quantity < 0.5M)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "2. Quantity", "Quantity too low. Please refer to the file format specification."));
                        }
                    }

                    if (String.IsNullOrEmpty(fields[1]))
                    {
                        result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "1. Product Code", "Product Code was empty. Please refer to the file format specification."));
                    }

                    var sampleIndicator = fields[4];

                    var code = fields[1];

                    switch (sampleIndicator)
                    {
                        case "H":
                            {
                                code = Regex.Replace(code, @"/.*", "/H");
                                fields[1] = code;
                            }
                            break;
                        case "S":
                            {
                                code = Regex.Replace(code, @"/.*", "/S");
                                fields[1] = code;
                            }
                            break;
                        case "W":
                            {
                                code = Regex.Replace(code, @"/.*", "/W");
                                fields[1] = code;
                            }
                            break;
                        default:
                            break;
                    }

                    if (fields[6] == "CUTTING")
                    {
                        code += "/C";
                    }

                    if (fields[1].Length > 50)
                    {
                        result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "1. Product Code", $"\"{fields[1]}\" is over-length. Maximum length: 50"));
                    }

                    if (fields[6].Length > 1000)
                    {
                        result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "6. Line Memo", $"\"{fields[6]}\" is over-length. Maximum length: 1000"));
                    }

                    if (fields[7].Length > 50)
                    {
                        result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "7. Line Reference", $"\"{fields[7]}\" is over-length. Maximum length: 50"));
                    }

                    if (!String.IsNullOrEmpty(fields[8]))
                    {
                        result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "8. Line Price", $"\"{fields[8]}\" was unexpected."));
                    }

                    using (var ce = new CommandExecutor())
                    {
                        var codeResult = ce.Arbitrary($"LIST PRODCODE STPRODMASTER WITH PRODCODE = \"{fields[1]}\" ID.SUP COL.HDR.SUPP COUNT.SUP");

                        var codeArray = codeResult.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                        if (codeArray.Count() != 1)
                        {
                            result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "1. Product Code", $"Product \"{fields[1]}\" was not found. Please check the part number."));
                        }
                        else
                        {
                            if (String.IsNullOrEmpty(codeArray[0]))
                            {
                                result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "1. Product Code", $"Product \"{fields[1]}\" was not found. Please check the part number."));
                            }
                        }
                    }

                    if (!String.IsNullOrEmpty(fields[1]))
                    {
                        using (var ce = new CommandExecutor(uniVerseAccount))
                        {
                            var itemTypeResult = ce.Arbitrary($"LIST ITEMTYPE STPRODMASTER WITH PRODCODE = \"{fields[1]}\" ID.SUP COL.HDR.SUPP COUNT.SUP");

                            var itemTypeResultArray = itemTypeResult.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                            if (itemTypeResultArray.Count() != 1)
                            {
                                result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "12. Product Code", $"Product \"{fields[1]}\" Item Type Lookup Failure. Please contact Customer Services."));
                            }
                            else
                            {
                                if (ForbiddenItemTypeCodes.Contains(itemTypeResultArray[0]))
                                {
                                    result.LineValidationFailures.Add(new LineValidationResult(lineNumber, "12. Product Code", $"Product \"{fields[1]}\" carries Item Type Code \"{itemTypeResultArray[0]}\", which is a forbidden Item Type Code. Please contact Customer Services to check for a new Product Code for this particular product."));
                                }
                            }
                        }
                    }

                    if (result.LineValidationFailures.Count == 0)
                    {
                        var thisHead = result.OrdersToImport.Where(p => p.DutchOrderGuid == orderHeadGuid).SingleOrDefault();

                        if (thisHead != null)
                        {

                            var type = "PRODUCT";

                            if (fields[6] == "CUTTING")
                            {
                                type = "SAMPLES CUTTING";
                            }

                            if (fields[5] == "Y")
                            {
                                type = "RESERVE";
                            }

                            thisHead.OrderLines.Add(item: new OrderLineToImport(fileLineNumber: i,
                                                                                productCode: fields[1],
                                                                                quantity: Convert.ToDecimal(fields[2]),
                                                                                type: type,
                                                                                lineReference: fields[7],
                                                                                memo: fields[6],
                                                                                promoCode: null,
                                                                                discountPercentage: null));
                        }
                    }

                    Console.WriteLine($"Found a LINE on line {i + 1}: {orderHeadGuid}");
                }
                else if (fields[0] == "HEADER" || String.IsNullOrEmpty(fields[0]))
                {
                    lineNumber = i;
                    break;
                }
            }

            ValidateDutchOrderHead(result, tradeOrderImportFileId, lineNumber, lines, uniVerseAccount);
        }

        public static FileValidationResult ValidateDutchFile(long tradeOrderImportFileId, string fileName, string fileContents, string uniVerseAccount)
        {
            var importLines = new List<ImportLine>();

            var result = new FileValidationResult(
                tradeOrderImportFileId: tradeOrderImportFileId, 
                fileName: fileName, 
                headerRow: fileContents.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)[0],
                fileContents: importLines);

            var lineNumber = 0;

            ValidateDutchOrderHead(result, tradeOrderImportFileId, lineNumber, fileContents.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries), uniVerseAccount);

            return result;
        }
    }
}
