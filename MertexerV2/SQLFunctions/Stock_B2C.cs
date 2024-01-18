using MertexerV2.Models.Stock;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MertexerV2.SQLFunctions
{
    public partial class Stock
    {
        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "B2CEnquiry_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable B2CEnquiry()
        {
            var dataFetch = new B2CEnquiryDataFetch();

#if DEBUG
            using (var conn = new SqlConnection("Data Source=vm-sql1;Initial Catalog=Flock;Integrated Security=True"))
#else
            using (var conn = new SqlConnection("context connection=true"))
#endif
            {
                try
                {
                    using (var comm = new SqlCommand("portal.StockCheckProductList", conn))
                    {
                        comm.CommandType = CommandType.StoredProcedure;

#if DEBUG
                        //comm.Parameters.AddWithValue("@Code", "SFDG1270");
                        comm.Parameters.AddWithValue("@CheckAllProducts", 1);
#endif

                        conn.Open();

                        var reader = comm.ExecuteReader();

                        while (reader.Read())
                        {
                            dataFetch.ProductCodes.Add(reader.GetString(0));
                        }

                        reader.NextResult();

                        while (reader.Read())
                        {
                            dataFetch.StockLocations.Add(reader.GetString(0));
                        }

                        reader.NextResult();

                        while (reader.Read())
                        {
                            dataFetch.SampleProductCodes.Add(reader.GetString(0));
                        }

                        reader.NextResult();

                        while (reader.Read())
                        {
                            dataFetch.SampleStockLocations.Add(reader.GetString(0));
                        }

                        reader.NextResult();

                        while (reader.Read())
                        {
                            dataFetch.PaintProductCodes.Add(reader.GetString(0));
                        }

                        reader.NextResult();

                        while (reader.Read())
                        {
                            dataFetch.PaintStockLocations.Add(reader.GetString(0));
                        }
                    }
                }
                finally
                {
                    conn.Close();
                }
            }

            // right the... lists are no longer batched. So batch them. The product codes are no longer wrapped either, so wrap them.

            var productCodeBatches = ListExtension.Split(dataFetch.ProductCodes, 1000);
            var sampleCodeBatches = ListExtension.Split(dataFetch.SampleProductCodes, 1000);
            var paintCodeBatches = ListExtension.Split(dataFetch.PaintProductCodes, 1000);

            var productCodes = new List<string>();
            var sampleProductCodes = new List<string>();
            var paintProductCodes = new List<string>();

            foreach (var chunk in productCodeBatches)
            {
                productCodes.Add(String.Join(" ", chunk));
            }

            foreach (var chunk in sampleCodeBatches)
            {
                sampleProductCodes.Add(String.Join(" ", chunk));
            }

            foreach (var chunk in paintCodeBatches)
            {
                paintProductCodes.Add(String.Join(" ", chunk));
            }

            Console.WriteLine($"Checking {productCodes.Count} Product part numbers");
            Console.WriteLine($"Checking {paintProductCodes.Count} Paint part numbers");
            Console.WriteLine($"Checking {sampleProductCodes.Count} Sample part numbers");

            var totalToCheck = productCodes.Count + paintProductCodes.Count + sampleProductCodes.Count;

            Console.WriteLine($"Checking {totalToCheck} in total.");

            var objLock = new object();

            // nested parallel loops?

            var chunkyList = new List<Tuple<List<string>, List<string>>>()
            {
                new Tuple<List<string>, List<string>>(productCodes, dataFetch.StockLocations),
                new Tuple<List<string>, List<string>>(sampleProductCodes, dataFetch.SampleStockLocations),
                new Tuple<List<string>, List<string>>(paintProductCodes, dataFetch.PaintStockLocations)
            };

            var finalResults = new List<B2CEnquiryResult>();

            var finalResultsLock = new object();

            Parallel.ForEach(
                chunkyList,
                (tuple) =>
                {
                    var tupleResults = new List<B2CEnquiryResult>();

                    Parallel.ForEach(
                        tuple.Item2,
                        (stockLocation) =>
                        {
                            var stockLocationResults = new List<B2CEnquiryResult>();

                            using (var queryExecutor = new QueryExecutor())
                            {
                                foreach (var codes in tuple.Item1)
                                {
                                    var result = queryExecutor.Run(
                                        uvFileName: "B2C_STOCK_ENQUIRY",
                                        uvSelectStatement: codes,
                                        fieldsToReturn: "PRODCODE WI.TOTFREE WI.DUEIN.SEL WI.DUEIN.OSQTY WI.DUEIN.OSQTY.TOTAL WI.DUEIN.POCONF",
                                        webData: $"LOC={(String.IsNullOrEmpty(stockLocation) ? "*" : stockLocation)};MSL=-9999",
                                        returnType: "XML",
                                        "STOCK_ENQUIRY");

                                    var xDoc = XDocument.Parse(result);

                                    if (!xDoc.Descendants("record").Any())
                                    {
                                        SendBatchProblemEmail(stockLocation, codes, "Stock Check Problem Batch");
                                    }

                                    xDoc.Descendants("record").ToList().ForEach(c =>
                                    {
                                        var productCode = c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value;
                                        var totalFree = c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value;
                                        var nextDueDate = c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value;
                                        var nextDueQuantity = c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value;
                                        var totalDueQuantity = c.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value;
                                        var poStatus = c.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value;

                                        bool? poStatusBool = null;

                                        if (!String.IsNullOrEmpty(poStatus))
                                        {
                                            switch (poStatus)
                                            {
                                                case "PO Conf":
                                                    poStatusBool = true;
                                                    break;
                                                case "PO Unconf":
                                                    poStatusBool = false;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }

                                        stockLocationResults.Add(new B2CEnquiryResult(
                                            stockLocationCode: stockLocation,
                                            productCode: productCode,
                                            totalFreeStock: Convert.ToDecimal(totalFree),
                                            nextPODueDate: nextDueDate,
                                            nextPOQuantity: Convert.ToDecimal(nextDueQuantity),
                                            totalPOQuantity: Convert.ToDecimal(totalDueQuantity),
                                            poStatus: poStatusBool
                                        ));
                                    });
                                }
                            }

                            lock (objLock)
                            {
                                tupleResults.AddRange(stockLocationResults);
                            }
                        }
                    );

                    lock (finalResultsLock)
                    {
                        finalResults.AddRange(tupleResults);
                    }
                });

            var overLength = finalResults.Where(p => p.TotalFreeStock > 99999 || p.NextPOQuantity > 99999 || p.TotalPOQuantity > 99999).ToList();

            if (overLength.Any())
            {
                SendProblemEmail(overLength);
            }

            finalResults = finalResults.Except(overLength).ToList();

            return new ArrayList(finalResults);
        }

        [SqlFunction(DataAccess = DataAccessKind.Read,
            FillRowMethodName = "B2CEnquiry_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable B2CEnquiry_Singular(string ProductCode, string StockLocation)
        {
            var results = new List<B2CEnquiryResult>();

            using (var queryExecutor = new QueryExecutor())
            {
                var result = queryExecutor.Run(
                    uvFileName: "B2C_STOCK_ENQUIRY",
                    uvSelectStatement: $"\"{ProductCode}\"",
                    fieldsToReturn: "PRODCODE WI.TOTFREE WI.DUEIN.SEL WI.DUEIN.OSQTY WI.DUEIN.OSQTY.TOTAL WI.DUEIN.POCONF",
                    webData: $"LOC={StockLocation};MSL=-9999",
                    returnType: "XML",
                    "STOCK_ENQUIRY");

                var xDoc = XDocument.Parse(result);

                xDoc.Descendants("record").ToList().ForEach(c =>
                {
                    var productCode = c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value;
                    var totalFree = c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value;
                    var nextDueDate = c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value;
                    var nextDueQuantity = c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value;
                    var totalDueQuantity = c.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value;
                    var poStatus = c.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value;

                    bool? poStatusBool = null;

                    if (!String.IsNullOrEmpty(poStatus))
                    {
                        switch (poStatus)
                        {
                            case "PO Conf":
                                poStatusBool = true;
                                break;
                            case "PO Unconf":
                                poStatusBool = false;
                                break;
                            default:
                                break;
                        }
                    }

                    results.Add(new B2CEnquiryResult(
                        stockLocationCode: StockLocation,
                        productCode: productCode,
                        totalFreeStock: Convert.ToDecimal(totalFree),
                        nextPODueDate: nextDueDate,
                        nextPOQuantity: Convert.ToDecimal(nextDueQuantity),
                        totalPOQuantity: Convert.ToDecimal(totalDueQuantity),
                        poStatus: poStatusBool
                    ));
                });
            }

            var overLength = results.Where(p => p.TotalFreeStock > 99999 || p.NextPOQuantity > 99999 || p.TotalPOQuantity > 99999).ToList();

            if (overLength.Any())
            {
                SendProblemEmail(overLength);
            }

            results = results.Except(overLength).ToList();

            return new ArrayList(results);
        }

        public static void B2CEnquiry_FillRow(
            object obj,
            out SqlString StockLocationCode,
            out SqlString ProductCode,
            out SqlDecimal TotalFreeStock,
            out SqlDateTime PODate,
            out SqlString POMessage,
            out SqlDecimal NextPOQuantity,
            out SqlDecimal TotalPOQuantity,
            out SqlBoolean PurchaseOrderStatus
            )
        {
            var enquiryResult = (B2CEnquiryResult)obj;

            StockLocationCode = enquiryResult.StockLocationCode;
            ProductCode = enquiryResult.ProductCode;
            PODate = enquiryResult.PurchaseOrderDate ?? SqlDateTime.Null;
            POMessage = enquiryResult.PurchaseOrderMessage ?? SqlString.Null;
            TotalFreeStock = enquiryResult.TotalFreeStock;
            NextPOQuantity = enquiryResult.NextPOQuantity;
            TotalPOQuantity = enquiryResult.TotalPOQuantity;
            PurchaseOrderStatus = enquiryResult.PurchaseOrderStatus ?? SqlBoolean.Null;
        }

        public static void SendProblemEmail(List<B2CEnquiryResult> discards, string subject = "MertexerV2 Stock Record(s) Discarded", string body = "Muppets wielding scanners? Possibly...")
        {
            using (var client = new SmtpClient("smtp.office365.com", 587))
            {
                // set smtp-client with basicAuthentication
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                var basicAuthenticationInfo = new
                    System.Net.NetworkCredential("portal.noreply@designersguild.com", "TheCakeIsALi3");
                client.Credentials = basicAuthenticationInfo;

                var from = new MailAddress("portal.noreply@designersguild.com", "Designers Guild Portal");
                var to = new MailAddress("itsupport@designersguild.com", "IT Support");

                var mail = new MailMessage(from, to);

                mail.CC.Add("tclark@designersguild.com,mchristie@designersguild.com");
                mail.Subject = subject;
                mail.SubjectEncoding = System.Text.Encoding.UTF8;

                discards.ForEach(c => 
                { 
                    body += "<br/></br>" + c.ToString().Replace(System.Environment.NewLine, "<br/>");
                });

                mail.Body = body;
                mail.BodyEncoding = System.Text.Encoding.UTF8;

                mail.IsBodyHtml = true;

                client.Send(mail);
            }
        }

        public static void SendBatchProblemEmail(string stockLocation, string productCodeString, string subject = "MertexerV2 Stock Record(s) Discarded")
        {
            using (var client = new SmtpClient("smtp.office365.com", 587))
            {
                // set smtp-client with basicAuthentication
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                var basicAuthenticationInfo = new
                    System.Net.NetworkCredential("portal.noreply@designersguild.com", "TheCakeIsALi3");
                client.Credentials = basicAuthenticationInfo;

                var from = new MailAddress("portal.noreply@designersguild.com", "Designers Guild Portal");
                var to = new MailAddress("itsupport@designersguild.com", "IT Support");

                var mail = new MailMessage(from, to);

                mail.CC.Add("tclark@designersguild.com,mchristie@designersguild.com");
                mail.Subject = subject;
                mail.SubjectEncoding = System.Text.Encoding.UTF8;

                var body = "Muppets wielding scanners? Possibly...";

                body += $"<br/></br><strong>Stock Location: {stockLocation}</strong><br/>";

                body += $"<br/></br>{productCodeString}<br/>";

                mail.Body = body;
                mail.BodyEncoding = System.Text.Encoding.UTF8;

                mail.IsBodyHtml = true;

                client.Send(mail);
            }
        }
    }
}
