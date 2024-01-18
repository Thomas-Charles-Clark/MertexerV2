using MertexerV2.Models.DataSource;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.SQLFunctions
{
    public partial class Order
    {
        //[SqlFunction(DataAccess = DataAccessKind.Read,
        //    FillRowMethodName = "Submit_FillRow",
        //    SystemDataAccess = SystemDataAccessKind.Read,
        //    TableDefinition = "")]
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OrderHeadID">The OrderHeadID to process</param>
        /// <param name="SelectProcedure">Either 
        ///                                 "portal.Mertex_Order_Select" or 
        ///                                 "adm.Mertex_Order_Select",
        ///                                 depending on the output of portal.Mertex_Order_DetermineSelectProcedure
        /// </param>
        /// <param name="ResultantOrderHeadID">The output</param>
        /// <returns></returns>
        [SqlProcedure]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static int Submit(int OrderHeadID, string SelectProcedure, out int ResultantOrderHeadID)
        {
            var results = new List<int>();

            var dataSet = new DataSet();

            //var dataSourceJsonTable = TableBuilder.DataSourceJsonTable();

            var orderHeadTable = TableBuilder.OrderHeaderTable();

            var orderLineTable = TableBuilder.OrderLineTable();

            var giftBoxTable = TableBuilder.GiftBoxTable();

            //dataSet.Tables.Add(dataSourceJsonTable);

            dataSet.Tables.Add(orderHeadTable);

            dataSet.Tables.Add(orderLineTable);

            dataSet.Tables.Add(giftBoxTable);

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
                        using (var comm = new SqlCommand(SelectProcedure, conn))
                        {
                            comm.Transaction = transaction;

                            comm.CommandType = CommandType.StoredProcedure;

                            comm.Parameters.Add(new SqlParameter("@OrderHeadID", OrderHeadID));

                            var adapter = new SqlDataAdapter(comm);

                            //adapter.TableMappings.Add("Table", "DataSourceJson");
                            adapter.TableMappings.Add("Table", "OrderHead");
                            adapter.TableMappings.Add("Table1", "OrderLines");
                            adapter.TableMappings.Add("Table2", "GiftBoxes");

                            adapter.Fill(dataSet);
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

            //string tradeDataSourceJson = dataSet.Tables["DataSourceJson"].AsEnumerable().First()["UniVerseDataSources"].ToString();

            //var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            //var locker = new object();

            //var dataSource = dataSources.EnquiryTypes
            //                            .Where(p => p.EnquiryType == EnquiryType.PlaceOrder)
            //                            .Single()
            //                            .DataSources
            //                            .First();

            var ordersPlaced = new List<int>();

            var ordersFailed = new List<int>();

            var allOrders = new List<Models.Order>();

            foreach (var orderHead in dataSet.Tables["OrderHead"].AsEnumerable())
            {
                var customerReference = orderHead["CustomerReference"].ToString();

                var orderLines = dataSet.Tables["OrderLines"]; // Not quite. We now want each line separately ordered! Okay... How.

                var giftBoxes = dataSet.Tables["GiftBoxes"];

                foreach (var line in orderLines.AsEnumerable())
                {
                    line["CustomerReference"] = customerReference;
                }

                foreach (var line in giftBoxes.AsEnumerable())
                {
                    line["CustomerReference"] = customerReference;
                }

                var uniVerseAccount = "MERTEX";

                try
                {
                    uniVerseAccount = orderHead["UniVerseAccount"]?.ToString();
                }
                catch
                {

                }

#if DEBUG
                using (var oc = new OrderController(uniVerseAccount)) // dataSource.UniVerseAccount))
#else
                using (var oc = new OrderController(uniVerseAccount))
#endif
                {
                    var order = new Models.Order(
                        orderHead,
                        orderLines,
                        giftBoxTable);

                    allOrders.Add(order);

//#if DEBUG
//                    ResultantOrderHeadID = 0;
//                    return 0;
//#endif 
                    
                    try
                    {

#if DEBUG
                        order.BasketID = "PH03047~1140305001~b1f0906d-c8df-4009-9fd6-926d70897e6e";
#endif


                        var result = oc.PlaceOrder(order);

                        if (result.Contains(order.OrderNumber.ToString()))
                        {
                            ordersPlaced.Add(Convert.ToInt32(order.OrderNumber));
                        }
                        else
                        {
                            ordersFailed.Add(Convert.ToInt32(order.OrderNumber));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;

                        Console.WriteLine(ex);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                        SendErrorEmail(order, "itsupport@designersguild.com", ex);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    }
                }
            }

            if (ordersPlaced.Count == 1)
                ResultantOrderHeadID = ordersPlaced[0];
            else
                ResultantOrderHeadID = 0;

            return 0;
            //return new ArrayList(ordersPlaced);
        }

        public static void Submit_FillRow(
            object obj,
            out int OrderHeadID
            )
        {
            OrderHeadID = (int)obj;
        }

        public static async Task<int> SendErrorEmail(MertexerV2.Models.Order order, string recipientEmail, Exception ex)
        {
            try
            {
                var newlineDelimiter = new string[] { System.Environment.NewLine };

                using (var client = new System.Net.Mail.SmtpClient("smtp.office365.com", 587))
                {
                    System.Net.Mail.MailMessage message = null;

                    message = new System.Net.Mail.MailMessage("portal.noreply@designersguild.com", recipientEmail);

                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new System.Net.NetworkCredential("portal.noreply@designersguild.com", "TheCakeIsALi3");

                    message.IsBodyHtml = true;
#if DEBUG
                    message.Subject = $"TESTING - EXCEPTION while submitting {order.OrderNumber} to Mertex!";
#else
                    message.Subject = $"EXCEPTION while submitting {order} to Mertex!";
#endif
                    message.CC.Add("itsupport@designersguild.com");
                    message.CC.Add("tclark@designersguild.com");
                    message.CC.Add("mchristie@designersguild.com");

                    System.IO.MemoryStream ms = new System.IO.MemoryStream();

                    using (var writer = new System.IO.StreamWriter(ms))
                    {
                        writer.WriteLine($"WebTex BasketID: {order.BasketID}");
                        writer.WriteLine();
                        writer.WriteLine("WebData for SALES_ORDER_DELDETS");
                        writer.WriteLine(Framework.WavyLineBreak);
                        writer.WriteLine();

                        var deldets = String.Join(Environment.NewLine, order.DeliveryDetailsWebData().Split(newlineDelimiter, StringSplitOptions.None).Select((p, i) => {
                            return $"{i+1,3}: {p}";
                        }).ToArray());

                        writer.Write(deldets);

                        writer.WriteLine();
                        writer.WriteLine();
                        writer.WriteLine("WebData for SALES_ORDER_ALLOC");
                        writer.WriteLine(Framework.WavyLineBreak);

                        order.OrderLines.ForEach(c =>
                        {
                            writer.WriteLine();
                            writer.WriteLine($"Line {c.LineNumber}:");
                            writer.WriteLine();
                            var line = String.Join(Environment.NewLine, c.WebData().Split(newlineDelimiter, StringSplitOptions.None).Select((p, i) => {
                                return $"{i + 1,3}: {p}";
                            }).ToArray());
                            writer.WriteLine(line);
                        });

                        order.GiftBoxes.ForEach(c =>
                        {
                            writer.WriteLine();
                            writer.WriteLine($"GiftBox {c.GiftBoxID}:");
                            writer.WriteLine();
                            var line = String.Join(Environment.NewLine, c.WebData().Split(newlineDelimiter, StringSplitOptions.None).Select((p, i) => {
                                return $"{i + 1,3}: {p}";
                            }).ToArray());
                            writer.WriteLine(line);
                        });

                        writer.WriteLine();
                        writer.WriteLine();
                        writer.WriteLine("WebData for SALES_ORDER_CREATE");
                        writer.WriteLine(Framework.WavyLineBreak);
                        writer.WriteLine();

                        var createDets = String.Join(Environment.NewLine, order.CreateOrderWebData().Split(newlineDelimiter, StringSplitOptions.None).Select((p, i) => {
                            return $"{i + 1,3}: {p}";
                        }).ToArray());

                        writer.Write(createDets);

                        writer.WriteLine();
                        writer.WriteLine();

                        writer.WriteLine("Exception:");
                        writer.WriteLine(Framework.WavyLineBreak);
                        writer.WriteLine();
                        writer.WriteLine(ex);

                        writer.Flush();

                        ms.Position = 0;

#if DEBUG
                        var fileName = $"TESTING-{order.OrderNumber}-WebData.txt";
#else
                        var fileName = $"{order.OrderNumber}-WebData.txt";
#endif

                        var attachment = new System.Net.Mail.Attachment(ms, fileName, System.Net.Mime.MediaTypeNames.Text.Plain);

                        message.Attachments.Add(attachment);

                        message.Body = $@"<head>
<style type=""text/css"">
    body {{ line-height: 11.75pt; font-size: 13pt; font-family: 'Courier New'}}
</style></head><body>The following exception occurred attempting to submit Order Number {order.OrderNumber}:<br/><br/><br/>
                                {ex.ToString().Replace(Environment.NewLine, "<br/><br/>")}<br/><br/><br/>
Please check /PHLOG in Mertex around the time of this exception for calls relating to the particular paragraph execution attempt which failed.
<br/><br/><br/>
Please also find attached WebData.txt file.
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
    }
}
