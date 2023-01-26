using MertexerV2.Models.OldTradeSiteImport;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MertexerV2.SQLFunctions
{
    public static class B2BOrderImport
    {
        // Call Get_Headers
        // Foreach outstanding header, call Get_Lines
        // Only once all data about an order has been brought into portal.OrderImportHead and portal.OrderImportLine,
        // Call Clear_Basket

#if DEBUG
        private static string sqlConnectionString = "Data Source=vm-sql1;Initial Catalog=FlockDev;Integrated Security=True";
#else
        private static string sqlConnectionString = "Data Source=vm-sql1;Initial Catalog=Flock;Integrated Security=True";
#endif

        [SqlProcedure]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static int SiphonOldTradeSiteOrders(string UniVerseAccount)
        {
            //var headers = JsonConvert.DeserializeObject<List<Header>>(System.IO.File.ReadAllText("order-imports.json"));

            var headers = new List<Header>();

            XDocument headersDoc = null;
            XDocument linesDoc = null;
            List<Line> lines = new List<Line>();

            try
            {
                using (var qe = new QueryExecutor(UniVerseAccount))
                {
                    headersDoc = XDocument.Parse(Framework.RemoveInvalidXmlChars(qe.Run("B2B_GET_BASKET_HEADERS", "")));

                    linesDoc = XDocument.Parse(Framework.RemoveInvalidXmlChars(qe.Run("B2B_GET_BASKET_LINES", "")));
                }

                linesDoc.Descendants("record").ToList().ForEach(h =>
                {
                    var line = new Line(
                        key: $"{h.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value}*{h.XPathSelectElement("descendant::fd[@id='C22']").Attribute("value").Value}",
                        tradeAccountCode: h.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value,
                        lineNumber: h.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value,
                        productCode: h.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value,
                        quantity: h.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value,
                        unit: h.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value,
                        sampleIndicator: h.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value,
                        reserveIndicator: h.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value,
                        backOrderIndicator: h.XPathSelectElement("descendant::fd[@id='C9']").Attribute("value").Value,
                        discountPercentage: h.XPathSelectElement("descendant::fd[@id='C10']").Attribute("value").Value,
                        lineMemo: h.XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value,
                        cutCounts: h.XPathSelectElement("descendant::fd[@id='C12']").Attribute("value").Value,
                        cutLengths: h.XPathSelectElement("descendant::fd[@id='C13']").Attribute("value").Value,
                        unitPriceExVat: h.XPathSelectElement("descendant::fd[@id='C14']").Attribute("value").Value,
                        customerOrderReference: h.XPathSelectElement("descendant::fd[@id='C15']").Attribute("value").Value,
                        withCutting: h.XPathSelectElement("descendant::fd[@id='C16']").Attribute("value").Value,
                        specificBatch: h.XPathSelectElement("descendant::fd[@id='C17']").Attribute("value").Value,
                        nonStockIndicator: h.XPathSelectElement("descendant::fd[@id='C18']").Attribute("value").Value,
                        cutSingleBatchIndicators: h.XPathSelectElement("descendant::fd[@id='C19']").Attribute("value").Value,
                        orderLineDiscountValue: h.XPathSelectElement("descendant::fd[@id='C20']").Attribute("value").Value,
                        lastUpdated: h.XPathSelectElement("descendant::fd[@id='C21']").Attribute("value").Value);

                    lines.Add(line);
                });

                headersDoc.Descendants("record").ToList().ForEach(h =>
                {
                    var head = new Header(
                        basketId: h.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value,
                        key: $"{h.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value}*{h.XPathSelectElement("descendant::fd[@id='C21']").Attribute("value").Value}",
                        tradeAccountCode: h.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value,
                        deliveryReference: h.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value,
                        deliveryName: h.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value,
                        deliveryAddressLine1: h.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value,
                        deliveryAddressLine2: h.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value,
                        deliveryAddressLine3: h.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value,
                        deliveryAddressLine4: h.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value,
                        deliveryPostcode: h.XPathSelectElement("descendant::fd[@id='C9']").Attribute("value").Value,
                        deliveryCountryCode: h.XPathSelectElement("descendant::fd[@id='C10']").Attribute("value").Value,
                        deliveryPhone: h.XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value,
                        domesticOrTrade: h.XPathSelectElement("descendant::fd[@id='C12']").Attribute("value").Value,
                        emailAddresses: h.XPathSelectElement("descendant::fd[@id='C13']").Attribute("value").Value,
                        customerOrderReference: h.XPathSelectElement("descendant::fd[@id='C14']").Attribute("value").Value,
                        priorityOrder: h.XPathSelectElement("descendant::fd[@id='C15']").Attribute("value").Value,
                        usTaxState: h.XPathSelectElement("descendant::fd[@id='C16']").Attribute("value").Value,
                        usTaxCounty: h.XPathSelectElement("descendant::fd[@id='C17']").Attribute("value").Value,
                        usTaxExemptionCertificate: h.XPathSelectElement("descendant::fd[@id='C18']").Attribute("value").Value,
                        usTaxDetails: h.XPathSelectElement("descendant::fd[@id='C19']").Attribute("value").Value,
                        lastUpdated: h.XPathSelectElement("descendant::fd[@id='C20']").Attribute("value").Value,
                        lines: lines,
                        database: UniVerseAccount
                    );

                    if (headers.Where(p => p.Key == head.Key).Count() == 0)
                    {
                        headers.Add(head);
                    }
                });
            }
            catch (Exception ex)
            {
                Task.Run(() =>
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    SendSiphonErrorEmail(null, null, "tclark@designersguild.com", ex);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                });
            }

#if DEBUG
            Console.WriteLine(Framework.WavyLineBreak);
            Console.WriteLine($"Saw {headers.Count} basket(s) to import");
            Console.WriteLine(Framework.WavyLineBreak);

            foreach (var h in headers)
            {
                Console.WriteLine(h.ToJSON);
                Console.WriteLine(Framework.WavyLineBreak);
            }

            var result = "";
            var validResponses = new List<string>() { "Y", "N" };

            while (!validResponses.Contains(result.ToUpperInvariant()))
            {
                Console.WriteLine("Process into SQL? Y/N");
                result = Console.ReadLine();
            }

            if (result.ToUpperInvariant() == "N")
            {
                return -1;
            }
#endif

            foreach (var h in headers)
            {
                var headerInsertResult = -1;

                var headerAlreadyExists = false;

#if DEBUG
                using (var conn = new SqlConnection(sqlConnectionString))
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
                            using (var ohComm = new SqlCommand(@"
                            SELECT  CONVERT(BIT, 1)
                            FROM    [portal].[B2BOrderImport]
                            WHERE   BasketKey = @BasketKey AND
                                    Deleted = 0", conn))
                            {
                                ohComm.Transaction = transaction;

                                ohComm.Parameters.Add(new SqlParameter("@BasketKey", h.Key));

                                var reader = ohComm.ExecuteReader();

                                while (reader.Read())
                                {
                                    headerAlreadyExists = reader.GetBoolean(0);
                                }

                                reader.Close();

                                if (headerAlreadyExists)
                                {
                                    throw new Exception($"Key {h.Key} already exists, undeleted, in portal.B2BOrderImport");
                                }
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            Task.Run(() =>
                            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                            SendSiphonErrorEmail(h.Key, h.ToJSON, "tclark@designersguild.com", ex);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                            });
                        }
                    }
                }

                if (!headerAlreadyExists)
                {
#if DEBUG
                    using (var conn = new SqlConnection(sqlConnectionString))
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
                                using (var ohComm = new SqlCommand(@"
                                    INSERT INTO [portal].[B2BOrderImport]
                                                ([BasketKey]
                                                ,[BasketJSON])
                                    VALUES 
                                    (
                                        @BasketKey,
                                        @BasketJSON
                                    )", conn))
                                {
                                    ohComm.Transaction = transaction;

                                    ohComm.Parameters.AddRange(new SqlParameter[]
                                    {
                                new SqlParameter("@BasketKey", h.Key),
                                new SqlParameter("@BasketJSON", h.ToJSON)
                                    });

                                    headerInsertResult = ohComm.ExecuteNonQuery();

                                    if (headerInsertResult != 1)
                                    {
                                        throw new Exception($"Failed to INSERT {h.Key} into portal.B2BOrderImport!");
                                    }

                                    transaction.Commit();
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                Task.Run(() =>
                                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                                SendSiphonErrorEmail(h.Key, h.ToJSON, "tclark@designersguild.com", ex);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                                });
                            }
                        }
                    }
                }

                DateTime? basketClearedDt = null;

                var clearedSuccessfully = false;

                try
                {
                    clearedSuccessfully = h.ClearBasket();
                    basketClearedDt = DateTime.Now;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    // EMAIL EXCEPTION SOMEWHERE
                    Task.Run(() =>
                    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        SendSiphonErrorEmail(h.Key, h.ToJSON, "tclark@designersguild.com", ex);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    });
                }
                
                if (clearedSuccessfully)
                {
                    try
                    {
                        var headerUpdateResult = -1;

#if DEBUG
                        using (var conn = new SqlConnection(sqlConnectionString))
#else
                        using (var conn = new SqlConnection("context connection=true"))
#endif
                        {
                            if (conn.State != ConnectionState.Open)
                                conn.Open();

                            using (var transaction = conn.BeginTransaction())
                            {
                                using (var ohComm = new SqlCommand(@"
                                UPDATE [portal].[B2BOrderImport]
                                    SET BasketClearedDT = @BasketClearedDT
                                WHERE BasketKey = @BasketKey", conn))
                                {
                                    ohComm.Transaction = transaction;

                                    ohComm.Parameters.AddRange(new SqlParameter[]
                                    {
                                new SqlParameter("@BasketClearedDT", basketClearedDt ?? (object)DBNull.Value),
                                new SqlParameter("@BasketKey", h.Key)
                                    });

                                    headerUpdateResult = ohComm.ExecuteNonQuery();

                                    if (headerUpdateResult != 1)
                                    {
                                        throw new Exception($"Failed to UPDATE SET BasketClearedDT for {h.Key} to on portal.B2BOrderImport!");
                                    }

                                    transaction.Commit();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        // EMAIL EXCEPTION SOMEWHERE
                        Task.Run(() =>
                        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                            SendSiphonErrorEmail(h.Key, h.ToJSON, "tclark@designersguild.com", ex);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        });
                    }
                }
            }

            return 0;
        }

        [SqlProcedure]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static int ProcessOldTradeSiteOrders()
        {
            var headers = new List<Header>();

            try
            {
#if DEBUG
                using (var conn = new SqlConnection(sqlConnectionString))
#else
                using (var conn = new SqlConnection("context connection=true"))
#endif
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    using (var transaction = conn.BeginTransaction())
                    {
                        var comm = new SqlCommand(@"SELECT  BasketJSON 
                                                    FROM    portal.B2BOrderImport 
                                                    WHERE   ProcessedDT IS NULL AND
                                                            Deleted = 0", conn)
                        {
                            Transaction = transaction
                        };

                        var reader = comm.ExecuteReader();

                        while (reader.Read())
                        {
                            headers.Add(JsonConvert.DeserializeObject<Header>(reader.GetString(0)));
                        }

                        reader.Close();
                    }
                }

                foreach(var h in headers)
                {
                    int olResult = -1;

#if DEBUG
                    using (var conn = new SqlConnection(sqlConnectionString))
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
                                using (var ohComm = new SqlCommand(@"
                                                DECLARE @OrderImportHeadID BIGINT;

                                                INSERT INTO [portal].[OrderImportHead]
                                                            ([OrderImportTypeID]
                                                            ,[CreatedDT]
                                                            ,[Attempt]
                                                            ,[OrderImportSourceID]
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
                                                            ,[DeliveryReference]
                                                            ,[FailureNotificationEmail]
                                                            ,[HeldForSales]
                                                            ,[ImportedTradeSiteOrderJSON])
                                                VALUES (
                                                        @OrderImportTypeID,
                                                        @Now,
                                                        0,
                                                        3,
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
                                                        @DeliveryReference,
                                                        @RecipientEmail,
                                                        @HeldForSales,
                                                        @ImportedTradeSiteOrderJSON
                                                );

                                                SET @OrderImportHeadID = SCOPE_IDENTITY();

                                                SELECT @OrderImportHeadID", conn))
                                {
                                    var now = DateTime.Now;

                                    ohComm.Transaction = transaction;

                                    ohComm.CommandType = CommandType.Text;

                                    ohComm.Parameters.AddRange(new List<SqlParameter>()
                                                {
                                                    new SqlParameter("@OrderImportTypeID", 1),
                                                    new SqlParameter("@Now", now),
                                                    new SqlParameter("@LineCount", h.Lines.Count),
                                                    new SqlParameter("@TradeAccountNumber", h.TradeAccountCode?.TrimReduceTruncate(50)),
                                                    new SqlParameter("@OrderReference", h.CustomerOrderReference?.TrimReduceTruncate(50) ?? (object)DBNull.Value),
                                                    new SqlParameter("@DeliveryName", h.DeliveryName?.TrimReduceTruncate(50) ?? (object)DBNull.Value),
                                                    new SqlParameter("@DeliveryAddressLine1", h.DeliveryAddressLine1?.TrimReduceTruncate(60) ?? (object)DBNull.Value),
                                                    new SqlParameter("@DeliveryAddressLine2", h.DeliveryAddressLine2?.TrimReduceTruncate(60) ?? (object)DBNull.Value),
                                                    new SqlParameter("@DeliveryTown", h.DeliveryAddressLine3?.TrimReduceTruncate(50) ?? (object)DBNull.Value),
                                                    new SqlParameter("@DeliveryCounty", h.DeliveryAddressLine4?.TrimReduceTruncate(40) ?? (object)DBNull.Value),
                                                    new SqlParameter("@DeliveryPostcode", h.DeliveryPostcode?.TrimReduceTruncate(20) ?? (object)DBNull.Value),
                                                    new SqlParameter("@DeliveryCountryCode", h.DeliveryCountryCode?.TrimReduceTruncate(2) ?? (object)DBNull.Value),
                                                    new SqlParameter("@DeliveryPhoneNumber", h.DeliveryPhone?.TrimReduceTruncate(20) ?? (object)DBNull.Value),
                                                    new SqlParameter("@Email", h.EmailAddresses?.TrimReduceTruncate(256) ?? (object)DBNull.Value),
                                                    new SqlParameter("@RecipientEmail", h.EmailAddresses?.TrimReduceTruncate(256) ?? (object)DBNull.Value),
                                                    new SqlParameter("@DeliveryReference", h.DeliveryReference?.TrimReduceTruncate(20) ?? (object)DBNull.Value),
                                                    new SqlParameter("@HeldForSales", false),
                                                    new SqlParameter("@ImportedTradeSiteOrderJSON", h.ToJSON)
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

                                    transaction.Commit();

                                    h.Lines.ForEach(ol =>
                                    {
                                        using (var oltransaction = conn.BeginTransaction())
                                        {
                                            using (var olComm = new SqlCommand(@"
                                                                    INSERT INTO [portal].[OrderImportLine]
                                                                                        ([OrderImportHeadID]
                                                                                        ,[ProductCode]
                                                                                        ,[Quantity]
                                                                                        ,[Type]
                                                                                        ,[LineReference]
                                                                                        ,[Memo]
                                                                                        ,[DiscountPercentage])
                                                                    VALUES (
                                                                            @OrderImportHeadID,
                                                                            @ProductCode,
                                                                            @Quantity,
                                                                            @Type,
                                                                            @LineReference,
                                                                            @Memo,
                                                                            @DiscountPercentage
                                                                    )", conn))
                                            {
                                                olComm.Transaction = oltransaction;

                                                olComm.CommandType = CommandType.Text;

                                                olComm.Parameters.AddRange(new List<SqlParameter>()
                                            {
                                                            new SqlParameter("@OrderImportHeadID", orderImportHeadId),
                                                            new SqlParameter("@ProductCode", ol.ProductCode?.TrimReduceTruncate(50)),
                                                            new SqlParameter("@Quantity", ol.Quantity),
                                                            new SqlParameter("@Type", ol.Type?.TrimReduceTruncate(30)),
                                                            new SqlParameter("@LineReference", ol.CustomerOrderReference?.TrimReduceTruncate(50) ?? (object)DBNull.Value),
                                                            new SqlParameter("@Memo", ol.LineMemo?.TrimReduceTruncate(1000) ?? (object)DBNull.Value),
                                                            new SqlParameter("@DiscountPercentage", ol.DiscountPercentage ?? (object)DBNull.Value)
                                            }.ToArray());

                                                if (conn.State != ConnectionState.Open)
                                                    conn.Open();

                                                olResult = olComm.ExecuteNonQuery();

                                                oltransaction.Commit();
                                            }
                                        }
                                    });

                                    if (orderImportHeadId > 0 &&
                                        olResult > 0)
                                    {
                                        if (conn.State != ConnectionState.Open)
                                            conn.Open();

                                        using (var processedTransaction = conn.BeginTransaction())
                                        {
                                            using (var processedComm = new SqlCommand(@"UPDATE [portal].[B2BOrderImport]
                                                                                                SET ProcessedDT = @Now,  
                                                                                                OrderImportHeadID = @OrderImportHeadID
                                                                                        WHERE BasketKey = @BasketKey", conn))
                                            {
                                                processedComm.Transaction = processedTransaction;

                                                processedComm.Parameters.AddRange(new SqlParameter[]
                                                {
                                                    new SqlParameter("@BasketKey", h.Key),
                                                    new SqlParameter("@Now", DateTime.Now),
                                                    new SqlParameter("@OrderImportHeadID", orderImportHeadId)
                                                });

                                                var updateResult = processedComm.ExecuteNonQuery();

                                                processedTransaction.Commit();

                                                Console.WriteLine(updateResult);
                                            }
                                        }
                                    }
                                }
                            }
                            catch(Exception ex)
                            {
                                Console.WriteLine(ex);
                                // EMAIL EXCEPTION SOMEWHERE
                                Task.Run(() =>
                                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                                    SendProcessingErrorEmail(h.Key, h.ToJSON, "tclark@designersguild.com", ex);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // EMAIL EXCEPTION SOMEWHERE
                Task.Run(() =>
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    SendProcessingErrorEmail(null, null, "tclark@designersguild.com", ex);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                });
            }

            return 0;
        }

        public static async Task<int> SendSiphonErrorEmail(string basketKey, string basketJson, string recipientEmail, Exception ex)
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

                    if (!String.IsNullOrEmpty(basketKey))
                    {
                        message.Subject = $"EXCEPTION while importing {basketKey} into portal.B2BOrderImport";
                    }
                    else
                    {
                        message.Subject = $"EXCEPTION while running B2B Order Siphon";
                    }

                    message.CC.Add("itsupport@designersguild.com");

                    System.IO.MemoryStream ms = new System.IO.MemoryStream();

                    using (var writer = new System.IO.StreamWriter(ms))
                    {
                        writer.Write(basketJson);

                        writer.Flush();

                        ms.Position = 0;

                        //var attachment = new System.Net.Mail.Attachment(ms, $"{basketKey}.json", System.Net.Mime.MediaTypeNames.Text.Plain);

                        //message.Attachments.Add(attachment);

                        message.Body = $@"<head>
<style type=""text/css"">
    body {{ line-height: 11.75pt; font-size: 13pt; font-family: 'Courier New'}}
</style></head><body>One or more exceptions were raised:<br/><br/><br/>
                                {ex.ToString().Replace(Environment.NewLine, "<br/><br/>")}
<br/>
<br/>
{basketJson}
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

        public static async Task<int> SendProcessingErrorEmail(string basketKey, string basketJson, string recipientEmail, Exception ex)
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

                    if (!String.IsNullOrEmpty(basketKey))
                    {
                        message.Subject = $"EXCEPTION while importing {basketKey}";
                    }
                    else
                    {
                        message.Subject = $"EXCEPTION while processing portal.B2BOrderImport";
                    }

                    message.CC.Add("itsupport@designersguild.com");

                    System.IO.MemoryStream ms = new System.IO.MemoryStream();

                    using (var writer = new System.IO.StreamWriter(ms))
                    {
                        writer.Write(basketJson);

                        writer.Flush();

                        ms.Position = 0;

                        //var attachment = new System.Net.Mail.Attachment(ms, $"{basketKey}.json", System.Net.Mime.MediaTypeNames.Text.Plain);

                        //message.Attachments.Add(attachment);

                        if (basketKey == null && basketJson == null)
                        {
                            message.Body = $@"<head>
<style type=""text/css"">
    body {{ line-height: 11.75pt; font-size: 13pt; font-family: 'Courier New'}}
</style></head><body>The following exception occurred attempting to process portal.B2BOrderImport records:<br/><br/><br/>
                                {ex.ToString().Replace(Environment.NewLine, "<br/><br/>")}
</body>";
                        }
                        else
                        {
                            message.Body = $@"<head>
<style type=""text/css"">
    body {{ line-height: 11.75pt; font-size: 13pt; font-family: 'Courier New'}}
</style></head><body>No OrderImportHead record has been created as one or more exceptions were raised while processing the basket.<br/><br/><br/>
                                {ex.ToString().Replace(Environment.NewLine, "<br/><br/>")}
<br/>
<br/>
{basketJson}
</body>";
                        }

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
    }
}
