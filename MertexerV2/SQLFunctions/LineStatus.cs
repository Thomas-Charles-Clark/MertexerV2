using MertexerV2.Models.Status;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MertexerV2.SQLFunctions
{
    internal class OrderLineRecord
    {
        public string LineKey { get; set; }
        public long OrderLineID { get; set; }
        public string UniVerseAccount { get; set; }

        public OrderLineRecord(
            string lineKey,
            long orderLineId, 
            string uniVerseAccount)
        {
            LineKey = lineKey;
            OrderLineID = orderLineId;
            UniVerseAccount = uniVerseAccount;
        }
    }

    public class LineStatus
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "LineStatus_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Check(string UniVerseAccount, int OrderHeadID, short LineNumber, long OrderLineID)
        {
            var toelineId = $"{OrderHeadID}*{LineNumber}";

            var results = new List<LineStatusResult>();

            using (var qe = new QueryExecutor(UniVerseAccount))
            {
                var xDoc = XDocument.Parse(
                    qe.Run(
                        "TOELINE",
                        $"@ID=\"{toelineId}\"",
                        "LINEKEY ORDNO LINE PRODCODE STATUS LINE.STAT.DES QTYORD.ORD QTYCAN.ORD SS.LINE.STAT.DATE.DES LINE.STAT.TIME.DES",
                        "",
                        "XML",
                        "",
                        false,
                        3
                    )
                );

                xDoc.Descendants("record").ToList().ForEach(c =>
                {
                    results.Add(new LineStatusResult
                    (
                        id: c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value, // LINEKEY e.g. 991234501*1 
                        orderHeadId: c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value, // ORDNO e.g. '991234501' 
                        orderLineId: OrderLineID,
                        lineNumber: c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value, // LINE e.g. '991234501' 
                        prodCode: c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value, // PRODCODE e.g. 'FDG2345/01' 
                        status: c.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value, // STATUS e.g. '5'
                        detailedStatusDescription: c.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value, // LINE.STAT.DES e.g. 'Invoiced'
                        quantity: c.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value, // QTYORD.ORD e.g. 1
                        cancelledQuantity: c.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value, // QTYCAN.ORD e.g. '0.5'
                        date: c.XPathSelectElement("descendant::fd[@id='C9']").Attribute("value").Value, //  SS.LINE.STAT.DATE.DES e.g. '10/01/2019'
                        time: c.XPathSelectElement("descendant::fd[@id='C10']").Attribute("value").Value // LINE.STAT.TIME.DES e.g. 12:43:29
                        //estimatedDeliveryDate: c.XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value,// CURSUG.DELDATE e.g. '16/04/2021'
                        //despatchedDate: c.XPathSelectElement("descendant::fd[@id='C12']").Attribute("value").Value, // DESP.DATE e.g. '10/01/2019'
                        //parcelTrackingUrl: c.XPathSelectElement("descendant::fd[@id='C13']").Attribute("value").Value // WI.TRACK.PARCEL.URL e.g. 'http://...'
                    ));
                });
            }

            return new ArrayList(results);
        }

        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "LineStatus_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable BatchCheck(byte? TransactionSourceID, DateTime? FromDT)
        {

#if DEBUG
            var stopWatch = new Stopwatch();
            Console.WriteLine("Fetching LineKeys to query for...");
            stopWatch.Start();
#endif

            var lineKeys = new List<OrderLineRecord>();

#if DEBUG
            using (var conn = new SqlConnection("Data Source=vm-sql1;Initial Catalog=FlockDev;Integrated Security=True"))
#else
            using (var conn = new SqlConnection("context connection=true"))
#endif
            {
                try
                {
                    using (var comm = new SqlCommand("portal.OrderLine_StatusUpdate_SelectList", conn))
                    {
                        comm.CommandType = CommandType.StoredProcedure;

                        comm.Parameters.AddWithValue("@TransactionSourceID", TransactionSourceID ?? (object)DBNull.Value);
                        comm.Parameters.AddWithValue("@FromDT", FromDT ?? (object)DBNull.Value);

                        conn.Open();

                        var reader = comm.ExecuteReader();

                        while (reader.Read())
                        {
                            lineKeys.Add(new OrderLineRecord(reader.GetString(0), reader.GetInt64(1), reader.GetString(2)));
                        }

                        reader.Close();
                    }
                }
                finally
                {
                    conn.Close();
                }
            }

#if DEBUG
            stopWatch.Stop();
            Console.WriteLine($"Fetched {lineKeys.Count} line keys in {stopWatch.Elapsed}");
            stopWatch.Reset();
            Console.WriteLine("Querying Mertex...");
            stopWatch.Start();
#endif

            var groupedByAccount = lineKeys.GroupBy(p => p.UniVerseAccount).ToList();

            var objLock = new object();

            var results = new List<LineStatusResult>();

            Parallel.ForEach(groupedByAccount, (account) =>
            {
                var lineKeyBatches = ListExtension.Split(account.ToList(), 1000);

                // Be resilient about a particular batch failing
                // Do not error - just don't return results for that batch

                // for later consideration - can we efficiently use just OrderHeadID to query for ALL lines

                // Hook it up to SQL

                Parallel.ForEach(lineKeyBatches, (batch) =>
                {
                    try
                    {
                        var theseResults = new List<LineStatusResult>();

                        using (var qe = new QueryExecutor(account.Key))
                        {
                            var xDoc = XDocument.Parse(
                                qe.Run(
                                    "TOELINE",
                                    $"@ID = {String.Join(" ", batch.Select(p => $"\"{p.LineKey}\"").ToList())}",
                                    "LINEKEY ORDNO LINE PRODCODE STATUS LINE.STAT.DES QTYORD.ORD QTYCAN.ORD SS.LINE.STAT.DATE.DES LINE.STAT.TIME.DES",
                                    "",
                                    "XML",
                                    "",
                                    false,
                                    10
                                )
                            );

                            xDoc.Descendants("record")?.ToList().ForEach(c =>
                            {
                                results.Add(new LineStatusResult(
                                    id: c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value, // LINEKEY e.g. 991234501*1 
                                    orderHeadId: c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value, // ORDNO e.g. '991234501' 
                                    orderLineId: batch.Where(p => p.LineKey == c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value).Select(p => p.OrderLineID).Single(),
                                    lineNumber: c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value, // LINE e.g. '991234501' 
                                    prodCode: c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value, // PRODCODE e.g. 'FDG2345/01' 
                                    status: c.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value, // STATUS e.g. '5'
                                    detailedStatusDescription: c.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value, // LINE.STAT.DES e.g. 'Invoiced'
                                    quantity: c.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value, // QTYORD.ORD e.g. 1
                                    cancelledQuantity: c.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value, // QTYCAN.ORD e.g. '0.5'
                                    date: c.XPathSelectElement("descendant::fd[@id='C9']").Attribute("value").Value, //  SS.LINE.STAT.DATE.DES e.g. '10/01/2019'
                                    time: c.XPathSelectElement("descendant::fd[@id='C10']").Attribute("value").Value // LINE.STAT.TIME.DES e.g. 12:43:29
                                                                                                                     //estimatedDeliveryDate: c.XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value,// CURSUG.DELDATE e.g. '16/04/2021'
                                                                                                                     //despatchedDate: c.XPathSelectElement("descendant::fd[@id='C12']").Attribute("value").Value, // DESP.DATE e.g. '10/01/2019'
                                                                                                                     //parcelTrackingUrl: c.XPathSelectElement("descendant::fd[@id='C13']").Attribute("value").Value // WI.TRACK.PARCEL.URL e.g. 'http://...'
                                ));
                            });

                            lock (objLock)
                            {
                                results.AddRange(theseResults);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Console.WriteLine("Oh bugger, an exception occurred. This batch was skipped.", ex);
#endif
                    }
                });
            });

#if DEBUG
            stopWatch.Stop();
            Console.WriteLine($"Completed in {stopWatch.Elapsed}");
            Console.WriteLine($"Got {results.Count} results.");
            stopWatch.Reset();
#endif

            results = results.Where(p => p != null).ToList();

            return new ArrayList(results);
        }

        public static void LineStatus_FillRow(
            object obj,
            out SqlString ToelineID,
            out SqlInt32 OrderHeadID,
            out SqlInt64 OrderLineID,
            out SqlInt16 LineNumber,
            out SqlString ProductCode,
            out SqlByte OrderStatusID,
            out SqlString DetailedStatus,
            out SqlDateTime StatusChangeDT,
            out SqlDecimal Quantity,
            out SqlDecimal CancelledQuantity
            )
        {
            var lineResult = (LineStatusResult)obj;

            ToelineID = lineResult.ToelineID;
            OrderHeadID = lineResult.OrderHeadID;
            OrderLineID = lineResult.OrderLineID;
            LineNumber = lineResult.LineNumber;
            ProductCode = lineResult.ProductCode;
            OrderStatusID = lineResult.OrderStatusID;
            DetailedStatus = lineResult.StatusDescription?.TrimReduceTruncate(500);
            StatusChangeDT = lineResult.StatusChangedDT ?? SqlDateTime.Null;
            Quantity = lineResult.Quantity ?? SqlDecimal.Null;
            CancelledQuantity = lineResult.CancelledQuantity ?? SqlDecimal.Null;
        }
    }
}