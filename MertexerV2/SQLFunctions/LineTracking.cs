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
    public class OrderLineTrackingRecord
    {
        public string LineKey { get; set; }
        public long OrderLineID { get; set; }
        public string UniVerseAccount { get; set; }

        public OrderLineTrackingRecord(
            string lineKey,
            long orderLineId,
            string uniVerseAccount
            )
        {
            LineKey = lineKey;
            OrderLineID = orderLineId;
            UniVerseAccount = uniVerseAccount;
        }
    }

    public class LineTracking
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "LineTracking_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Check(string UniVerseAccount, int OrderHeadID, short LineNumber, long OrderLineID)
        {
            var toelineId = $"{OrderHeadID}*{LineNumber}";

            var results = new List<LineTrackingResult>();

            using (var qe = new QueryExecutor(UniVerseAccount))
            {
                var xDoc = XDocument.Parse(
                    qe.Run(
                        "TOELINE",
                        $"@ID=\"{toelineId}\"",
                        "LINEKEY ORDNO LINE DESDATE SS.DESPDATE4 WI.PARCEL.TRACK.URL WI.HAULIERCODE PL.LINE.KEY", // Sarah wanted us to use DESDATE over any of the "suggested" delivery dates
                        "",
                        "XML",
                        "",
                        false,
                        3
                    )
                );

                /*
                    // This is an interesting one: WI.PARCEL.TRACK.URL replaces @VM with "{UV_URL_Separator}", across the results of calling PARCEL.TRACK.URL.WI with WI.HAULIER.AND.PARCEL.
                    // WI.HAULIER.AND.PARCEL is defined thus: 

	                (
		                CASE
		                (
			                STATUS = "5",
				                DUP((F("OEDNHHIST",DNNUMBER)<13>:","),F("OEDNLHIST",DN.LINE.KEY)<97>):F("OEDNLHIST",DN.LINE.KEY)<97>; 
			                 STATUS = "4",
				                DUP((F("OEDNHEAD",DNNUMBER)<13>:","),F("OEDNLINE",DN.LINE.KEY)<97>):F("OEDNLINE",DN.LINE.KEY)<97>; 
			                 1,
				                DUP((F("OEPLHEAD",PL.DOC.NO)<13>:","),F("OEPLLINE",PL.LINE.KEY)<97>):F("OEPLLINE",PL.LINE.KEY)<97>
		                )
	                )
	
                    // Which is essentially this:
	                
	                Status = 5
	                    DeliveryNoteHeadHistory, DeliveryNoteLineHistory
	
	                Status = 4
	                    DeliveryNoteHead, DeliveryNoteLine,
	
	                Else,
	                    ParcelHead, ParcelLine
                 */

                xDoc.Descendants("record").ToList().ForEach(c =>
                {
                    var trackingResults = c.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value;

                    // Apparently WI.PARCEL.TRACK.URL can reply with: WI.PARCEL.TRACK.URL{UV_URL_Seperator}#
                    // where # is some number
                    // this I've seen against at least one Flight Logistics line - 992440901. I need to arrive at some way of
                    // knowing "this is a FL line" and then I can pull the PL.LINE.KEY from TOELINE and trim the trailing *N from it and THEN
                    // form the FL tracking URL, PHEW!

                    var haulierCode = c.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value;

                    if (haulierCode == "FL") // The whole... Mertex tracking thing is b0rked when it comes to Flight Logistics
                    {
                        results.Add(new LineTrackingResult
                        (
                            id: c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value, // LINEKEY e.g. 991234501*1 
                            orderHeadId: c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value, // ORDNO e.g. '991234501' 
                            orderLineId: OrderLineID,
                            lineNumber: c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value, // LINE e.g. '991234501' 
                            estimatedDeliveryDate: c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value,// CURSUG.DELDATE e.g. '16/04/2021'
                            despatchedDate: c.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value, // DESP.DATE e.g. '10/01/2019'
                            parcelTrackingUrl: null,  // set in ctor
                            haulierCode: c.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value,
                            pickingListId: c.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value
                        ));
                    }
                    else
                    {
                        // Do the normal dance
                        var trackingSplit = trackingResults.Split(new string[] { "{UV_URL_Seperator}" }, StringSplitOptions.RemoveEmptyEntries);

                        foreach (string trackingUrl in trackingSplit)
                        {
                            results.Add(new LineTrackingResult
                            (
                                id: c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value, // LINEKEY e.g. 991234501*1 
                                orderHeadId: c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value, // ORDNO e.g. '991234501' 
                                orderLineId: OrderLineID,
                                lineNumber: c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value, // LINE e.g. '991234501' 
                                estimatedDeliveryDate: c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value,// CURSUG.DELDATE e.g. '16/04/2021'
                                despatchedDate: c.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value, // DESP.DATE e.g. '10/01/2019'
                                parcelTrackingUrl: trackingUrl,  // e.g. "https://..."
                                haulierCode: c.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value,
                                pickingListId: c.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value
                            ));
                        }
                    }
                });
            }

            return new ArrayList(results);
        }

        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "LineTracking_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable BatchCheck(byte? TransactionSourceID, DateTime? FromDT)
        {

#if DEBUG
            var stopWatch = new Stopwatch();
            Console.WriteLine("Fetching LineKeys to query for...");
            stopWatch.Start();
#endif

            var lineKeys = new List<OrderLineTrackingRecord>();

#if DEBUG
            using (var conn = new SqlConnection("Data Source=vm-sql1;Initial Catalog=FlockDev;Integrated Security=True"))
#else
            using (var conn = new SqlConnection("context connection=true"))
#endif
            {
                try
                {
                    using (var comm = new SqlCommand("portal.OrderLine_TrackingUpdate_SelectList", conn))
                    {
                        comm.CommandType = CommandType.StoredProcedure;

                        comm.Parameters.AddWithValue("@TransactionSourceID", TransactionSourceID ?? (object)DBNull.Value);
                        comm.Parameters.AddWithValue("@FromDT", FromDT ?? (object)DBNull.Value);

                        conn.Open();

                        var reader = comm.ExecuteReader();

                        while (reader.Read())
                        {
                            lineKeys.Add(new OrderLineTrackingRecord(reader.GetString(0), reader.GetInt64(1), reader.GetString(2)));
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

            var results = new List<LineTrackingResult>();

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
                        var theseResults = new List<LineTrackingResult>();

                        using (var qe = new QueryExecutor(account.Key))
                        {
                            var xDoc = XDocument.Parse(
                                qe.Run(
                                    "TOELINE",
                                    $"@ID = {String.Join(" ", batch.Select(p => $"\"{p.LineKey}\"").ToList())}",
                                    "LINEKEY ORDNO LINE DESDATE SS.DESPDATE4 WI.PARCEL.TRACK.URL WI.HAULIERCODE PL.LINE.KEY",
                                    "",
                                    "XML",
                                    "",
                                    false,
                                    10
                                )
                            );

                            // see above single implementation for notes on WI.PARCEL.TRACK.URL 

                            xDoc.Descendants("record").ToList().ForEach(c =>
                            {
                                var haulierCode = c.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value;

                                if (haulierCode == "FL") // The whole... Mertex tracking thing is b0rked when it comes to Flight Logistics
                                {
                                    results.Add(new LineTrackingResult
                                    (
                                        id: c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value, // LINEKEY e.g. 991234501*1 
                                        orderHeadId: c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value, // ORDNO e.g. '991234501' 
                                        orderLineId: batch.Where(p => p.LineKey == c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value).Select(p => p.OrderLineID).Single(),
                                        lineNumber: c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value, // LINE e.g. '991234501' 
                                        estimatedDeliveryDate: c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value,// CURSUG.DELDATE e.g. '16/04/2021'
                                        despatchedDate: c.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value, // DESP.DATE e.g. '10/01/2019'
                                        parcelTrackingUrl: null,  // set in ctor
                                        haulierCode: c.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value,
                                        pickingListId: c.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value
                                    ));
                                }
                                else
                                {
                                    var trackingResults = c.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value;

                                    var trackingSplit = trackingResults.Split(new string[] { "{UV_URL_Seperator}" }, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (string trackingUrl in trackingSplit)
                                    {
                                        results.Add(new LineTrackingResult
                                        (
                                            id: c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value, // LINEKEY e.g. 991234501*1 
                                            orderHeadId: c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value, // ORDNO e.g. '991234501' 
                                            orderLineId: batch.Where(p => p.LineKey == c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value).Select(p => p.OrderLineID).Single(),
                                            lineNumber: c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value, // LINE e.g. '991234501' 
                                            estimatedDeliveryDate: c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value,// CURSUG.DELDATE e.g. '16/04/2021'
                                            despatchedDate: c.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value, // DESP.DATE e.g. '10/01/2019'
                                            parcelTrackingUrl: trackingUrl, // e.g. "https://..."
                                            haulierCode: c.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value,
                                            pickingListId: c.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value
                                        ));
                                    }
                                }
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

        public static void LineTracking_FillRow(
            object obj,
            out SqlString ToelineID,
            out SqlInt32 OrderHeadID,
            out SqlInt64 OrderLineID,
            out SqlInt16 LineNumber,
            out SqlDateTime EstimatedDeliveryDate,
            out SqlDateTime DespatchedDT,
            out SqlString ParcelTrackingURL
            )
        {
            var lineResult = (LineTrackingResult)obj;

            ToelineID = lineResult.ToelineID;
            OrderHeadID = lineResult.OrderHeadID;
            OrderLineID = lineResult.OrderLineID;
            LineNumber = lineResult.LineNumber;
            EstimatedDeliveryDate = lineResult.EstimatedDeliveryDate ?? SqlDateTime.Null;
            DespatchedDT = lineResult.DespatchedDT ?? SqlDateTime.Null;
            ParcelTrackingURL = lineResult.ParcelTrackingURL?.TrimReduceTruncate(500);
        }
    }
}