using MertexerV2.Models.DataSource;
using MertexerV2.Models.Tracking;
using MertexerV2.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MertexerV2.Tracking
{
    public static class Tracking
    {
        public static OrderTrackingSearchResultWrapper OrderTrackingSearch(
            string tradeDataSourceJson,
            PassAutocompleteTerm objSearch)
        {
            var overallResult = new OrderTrackingSearchResultWrapper();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.CheckOrders)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                var result = new OrderTrackingSearchResultWrapper();

                using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
                {
                    #region Visit TOEHEAD with an order number, if not found search MertexReportData for CustomerOrderReference

                    var xOrderHeadDoc = XDocument.Parse(qe.Run(
                                                        "TOEHEAD",
                                                        $"@ID = {objSearch.searchTerm}",
                                                        "ORDERNO " +
                                                        "SS.DATEONFILE " +
                                                        "CUSREF " +
                                                        "CUSNAME " +
                                                        "DELNAME " +
                                                        "DELADDR " +
                                                        "ECOMM.EMAIL.DES " +
                                                        "HEADER.REF26 " +
                                                        "CUS.ORD.REF " +
                                                        "HAULIERCODE " +
                                                        "HAULIERDESC "));

                    if (!xOrderHeadDoc.Descendants("record").Any())
                    {
                        result.ResultStatus = $"Order {objSearch.searchTerm} was not found.";
                        // This order was not found at this data source.
                        return;
                    }

                    #endregion

                    #region We found an Order record on TOEHEAD: Go ahead and set some properties on our result

                    var orderHeadNode = xOrderHeadDoc.Descendants("record").First();

                    result.OrderNumber = orderHeadNode.XPathSelectElement("descendant::fd[@id='C1']")?.Attribute("value")?.Value;

                    result.DateOnFile = orderHeadNode.XPathSelectElement("descendant::fd[@id='C2']")?.Attribute("value")?.Value;

                    result.CustomerReference = orderHeadNode.XPathSelectElement("descendant::fd[@id='C3']")?.Attribute("value")?.Value;

                    result.CustomerName = orderHeadNode.XPathSelectElement("descendant::fd[@id='C4']")?.Attribute("value")?.Value;

                    result.DeliveryName = orderHeadNode.XPathSelectElement("descendant::fd[@id='C5']")?.Attribute("value")?.Value;

                    result.DeliveryAddress = String.Join(System.Environment.NewLine,
                                                            orderHeadNode.XPathSelectElement("descendant::fd[@id='C6']")?.Attribute("value")?.Value
                                                                        .Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries)
                                                                        .Select(p => p.Trim()));

                    result.EmailAddress = orderHeadNode.XPathSelectElement("descendant::fd[@id='C7']")?.Attribute("value")?.Value ??
                                                    orderHeadNode.XPathSelectElement("descendant::fd[@id='C8']")?.Attribute("value")?.Value;

                    result.CustomerOrderReference = orderHeadNode.XPathSelectElement("descendant::fd[@id='C9']")?.Attribute("value")?.Value.Split(Framework.ValueMarkDelimiter)[0]; // Why is this split necessary? Goodness knows, it IS an MV field though.

                    result.HaulierCode = orderHeadNode.XPathSelectElement("descendant::fd[@id='C10']")?.Attribute("value")?.Value;

                    result.HaulierName = orderHeadNode.XPathSelectElement("descendant::fd[@id='C11']")?.Attribute("value")?.Value;

                    var ti = new System.Globalization.CultureInfo("en-GB").TextInfo;

                    result.HaulierName = ti.ToTitleCase(result.HaulierName.ToLowerInvariant());

                    #endregion

                    #region Now visit TOELINE to fetch *all* lines about this order, cancelled and not. Fetch all the properties we might want from TOELINE at this time.

                    var xOrderLineDoc = XDocument.Parse(qe.Run(
                                                        "TOELINE",
                                                        $"ORDERNO = {result.OrderNumber}",
                                                        "LINE " +
                                                        "LINEKEY " +
                                                        "PRODCODE " +
                                                        "PRODDESC " +
                                                        "QTYORD.ORD " +
                                                        "QTYCAN.ORD " +
                                                        "STOCK.UNIT " +
                                                        "STATUS " +
                                                        "STATUSDESC " +
                                                        "DNNUMBER " +
                                                        "DN.LINE.KEY " +
                                                        "SS.CURSUG.DELDATE.D4 "));

                    xOrderLineDoc.Descendants("record").ToList().ForEach(ol =>
                    {
                        result.OrderLines.Add(new OrderLineRecord(
                            lineNumber: Convert.ToInt32(ol.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value),
                            orderNumber: result.OrderNumber,
                            lineKey: ol.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value,
                            productCode: ol.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value,
                            productDescription: ol.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value,
                            quantityOrdered: Convert.ToDecimal(ol.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value),
                            quantityCancelled: Convert.ToDecimal(ol.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value),
                            stockUnit: ol.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value,
                            status: ol.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value,
                            statusDescription: ol.XPathSelectElement("descendant::fd[@id='C9']").Attribute("value").Value,
                            deliveryNoteNumber: ol.XPathSelectElement("descendant::fd[@id='C10']").Attribute("value").Value,
                            deliveryNoteLineNumber: ol.XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value,
                            estimatedDespatchDateOnFile: ol.XPathSelectElement("descendant::fd[@id='C12']").Attribute("value").Value
                        ));
                    });

                    // We can now build our collection of DeliveryNotes, as each line carries a DeliveryNoteNumber and DeliveryNoteLineKey.
                    result.DeliveryNotes = result.OrderLines
                                                    .GroupBy(ol => ol.DeliveryNoteNumber)
                                                    .Where(g => !String.IsNullOrEmpty(g.Key)) // If one or more lines haven't get been despatched, they cannot and do not have Delivery Note information
                                                    .Select(g => new DeliveryNoteHeadRecord(g.Key, g.Select(ol => ol.DeliveryNoteLineNumber))).ToList();

                    #endregion

                    #region Now visit DNL[HEAD/HIST] to pick up... oh, lots of things.

                    result = FetchParcelsFromDeliveryNoteHeaders(dataSource.UniVerseAccount, result);

                    #endregion

                    #region We need to set the ParcelContents' RemainingToDespatchQuantity

                    var allParcelContents = result.DeliveryNotes.SelectMany(p => p.ParcelHead.Parcels.SelectMany(q => q.ParcelContents).ToList()).ToList();

                    var distinctOrderLineIdentifiers = allParcelContents.Select(p => p.OrderLineKey).Distinct().ToList();

                    foreach (var ol in distinctOrderLineIdentifiers)
                    {
                        var orderedQuantity = allParcelContents.Where(p => p.OrderLineKey == ol).First().OrderLineQuantity;

                        var totalParcelledQuantity = allParcelContents.Where(p => p.OrderLineKey == ol).Select(p => p.Quantity).Sum();

                        var remainingToDespatch = orderedQuantity - totalParcelledQuantity;

                        allParcelContents.Where(p => p.OrderLineKey == ol).ToList().ForEach(pc =>
                        {
                            pc.RemainingToDespatchQuantity = remainingToDespatch;
                        });
                    }

                    #endregion
                }

                // If found, great. Pick up Customer details, Delivery Details, etc. from Header.
                //  
                //  TOEHEAD
                //      ORDERNO SS.DATEONFILE CUSREF CUSNAME DELNAME DELADDR ECOMM.EMAIL.DES HEADER.REF26 CUS.ORDER.REF HAULIERCODE
                //      [ORD.PLHEAD]? This can take me to OEPLHEAD / OEDNHEAD / OEDNHHIST but is it the *right* way to get there?
                // 
                //  TOELINE
                //      PRODCODE PRODDESC STOCK.UNIT QTYORD.ORD QTYCAN.ORD DNNUMBER DN.LINE.KEY
                //
                //  DNL[HEAD/HIST]
                //      PARCEL  ...  STUFF
                //
                //  OEFINDPARCEL
                //      PIECE.NO WEB.URL || CUSTOM.TRACK.URL.DES 
                //

                // Now, if CUSTOM.TRACK.URL.DES is NULL for any given parcel, you can fall back to forming WEBPARCEL{PARCEL}WEBPARCEL2 from OEHAULIER
                // Which is actually precisely what WEB.URL does!

                // to be a faithful accounting per order line of how much was ordered
                // reduced by how much has been parcelled.
                

                // if we get here, then the order was found on one data source. Set it to the overall result.
                lock (locker)
                {
                    overallResult = result;
                }
            });

            return overallResult;
        }

        public static OrderTrackingSearchResultWrapper FetchParcelsFromDeliveryNoteHeaders(
            string uniVerseAccount,
            OrderTrackingSearchResultWrapper result)
        {
            result.DeliveryNotes.ForEach(deliveryNoteHead =>
            {
                var parcelHead = new ParcelHeadResult();

                using (var ce = new CommandExecutor(uniVerseAccount))
                {
                    var cmd = $"LIST.ITEM OEDNHEAD {deliveryNoteHead.DeliveryNoteIdentifier} COL.HDR.SUPP ID.SUP COUNT.SUP";

                    var resultString = ce.Arbitrary(cmd);

                    // Return early if the record was not found
                    if (resultString.ToLowerInvariant().Contains("not found"))
                    {
                        cmd = cmd.Replace("OEDNHEAD", "OEDNHHIST USING DICT OEPLHEAD");
                        resultString = ce.Arbitrary(cmd);
                    }

                    // Delivery Note Head
                    var dnh = CommandExecutor.CrunchTCLResult(resultString);

                    // <10>OEPLLINEP
                    parcelHead.PickingLines = dnh.Columns.Where(p => p.Index == 10)
                                                                        .Single()
                                                                        .MultiValues
                                                                        .Select(p => new PickingListLineIdentifier(
                                                                            pickingListLineId: p.Value.Replace(" ", "")
                                                                        )).ToList();

                    // <13>HAULIERCODE
                    parcelHead.HaulierCode = dnh.Columns.Where(p => p.Index == 13).Single().Value;

                    //  <25>DESPDATE
                    parcelHead.DespatchedDateOnFile = dnh.Columns.Where(p => p.Index == 25).Single().Value;

                    //  <535>CUSTOM.TRACK.URL.DES
                    parcelHead.TrackingURL = dnh.Columns.Where(p => p.Index == 535).SingleOrDefault()?.Value;

                    CheckHaulierTrackable(uniVerseAccount, ref parcelHead); // This will tell us if the haulier at play is going to be able to provide us with a tracking url. Another "return early".

                    if (!String.IsNullOrEmpty(parcelHead.Untrackable))
                    {
                        // No point processing this delivery note header further; the haulier is not going to provide us with a tracking url.
                        return;
                    }

                    // Now, parcel numbers are a bit tricky. The prime source for them is <191>PARCEL.FULL.
                    // Helpfully, however, this can be empty. If that's the case, the next place to look is <40>PCL.NO

                    var parcels = new List<OrderParcelDetail>();

                    if (dnh.Columns.Where(p => p.Index == 191).Any())
                    {
                        // <191>PARCEL.FULL
                        parcels = dnh.Columns.Where(p => p.Index == 191)
                                             .Single()
                                             .MultiValues
                                             .Select((p, i) => new OrderParcelDetail(parcelIndex: i,
                                                                                     parcelNumber: p.Value,
                                                                                     haulierCode: parcelHead.HaulierCode,
                                                                                     despatchedDateOnFile: parcelHead.DespatchedDateOnFile,
                                                                                     trackingUrl: null)).ToList();
                    }
                    else
                    {
                        // 40: PCL.NO
                        parcels = dnh.Columns.Where(p => p.Index == 40)
                                             .Single()
                                             .MultiValues
                                             .Select((p, i) => new OrderParcelDetail(parcelIndex: i,
                                                                                    parcelNumber: p.Value,
                                                                                    haulierCode: parcelHead.HaulierCode,
                                                                                    despatchedDateOnFile: parcelHead.DespatchedDateOnFile,
                                                                                    trackingUrl: null)).ToList();
                    }


                    // <10>OEPLLINE now needs to take me to OEDNLINE or OEDNLHIST
                    // I have to get a list of ParcelNumbers from the Picking List Line level, as this is the only way I have
                    // direct access to the TOELINE identifiers belonging to a particular parcel.
                    var lines = dnh.Columns.Where(p => p.Index == 10)
                                    .Single()
                                    .MultiValues
                                    .Select(p => p.Value.Replace(" ", ""))
                                    .ToList();

                    foreach (var dnLineId in lines)
                    {
                        var plCmd = $"LIST.ITEM OEDNLINE {dnLineId} COL.HDR.SUPP ID.SUP COUNT.SUP";

                        var dnlString = ce.Arbitrary(plCmd);

                        if (dnlString.Contains("not found"))
                        {
                            plCmd = plCmd.Replace("OEDNLINE", "OEDNLHIST USING DICT OEPLLINE");
                            dnlString = ce.Arbitrary(plCmd);
                        }

                        var dnl = CommandExecutor.CrunchTCLResult(dnlString);

                        // <97>PARCEL.FULL contains all those parcels which this Picking Line gave product to
                        var deliveryNoteLineParcels = dnl.Columns.Where(p => p.Index == 97)
                                                         .Select(p => p.MultiValues)
                                                         .Single();

                        foreach (var dnlp in deliveryNoteLineParcels)
                        {
                            // <0>LINEKEY
                            var pickingListLineIdentifier = dnLineId;

                            // <3>TOELINP
                            var toelineId = dnl.Columns.Where(p => p.Index == 3)
                                                        .Single()
                                                        .Value;

                            // <4>PRODCODE
                            var prodCode = dnl.Columns.Where(p => p.Index == 4)
                                                      .Single()
                                                      .Value;

                            var thisParcel = parcels.Where(p => p.ParcelNumber == dnlp.Value).Single();

                            thisParcel.OrderLines.Add(new OrderLineStub(toelineId, thisParcel.ParcelNumber, prodCode, pickingListLineIdentifier));

                            // at this point in time, I *know* that Parcel 023120 had at least part of Toeline 999018001*1 put in it.
                            // use that information!
                        }
                    }

                    // We should now have a distinct list of parcels for this Picking List Head.
                    // Fetch the contents of these parcels.
                    parcels.ForEach(parcel =>
                    {
                        // We now want to know the parcel contents.

                        // <48>PCL.PRODCODES
                        // Only interested in those product codes which correspond to this parcel's sv index.
                        var productCodes = dnh.Columns.Where(p => p.Index == 48)
                                                        .SelectMany(p => p.MultiValues.Where(q => q.Index == parcel.ParcelIndex))
                                                        .SelectMany(p => p.SubValues)
                                                        .ToList();

                        // Got a list of product codes? Wonderful.

                        foreach (var codeEntry in productCodes)
                        {
                            // Check for pieces.
                            // <51>PCL.PIECES
                            var pieces = dnh.Columns.Where(p => p.Index == 51)
                                                    .SelectMany(p => p.MultiValues)
                                                    .Where(mv => mv.Index == parcel.ParcelIndex)
                                                    .ToList();

                            // This is where it gets a bit murky.
                            // If there exists more than one product code in this parcel, then we want pieces only for the current product code.
                            // If there was only one product code in this parcel, then we have one less level of SV to traverse.
                            if (productCodes.Count > 1)
                            {
                                var piecesForThisPartNumber = pieces.SelectMany(p => p.SubValues
                                                                                    .Where(sv => sv.Index == codeEntry.Index))
                                                                                    .ToList();

                                // Are there any pieces at this depth for this part number?
                                if (piecesForThisPartNumber.Any())
                                {
                                    parcel.ParcelContents.AddRange(ProcessParcelPieces(parcel, piecesForThisPartNumber, codeEntry, dnh, result.OrderLines));
                                }
                                else
                                {
                                    var c = ProcessParcelQuantity(parcel, codeEntry, dnh, result.OrderLines);
                                    if (c != null)
                                        parcel.ParcelContents.Add(c);
                                }
                            }
                            else
                            {
                                if (pieces.Any())
                                {
                                    parcel.ParcelContents.AddRange(ProcessParcelPieces(parcel, pieces, codeEntry, dnh, result.OrderLines));
                                }
                            }
                        }

                        #region Visit OEFINDPARCEL and null-coalesce between CUSTOM.TRACK.URL.DES and WEB.URL

                        FetchTrackingURL(uniVerseAccount, ref parcel);

                        #endregion

                        // Just for testing purposes
                        // This will cause the parcel head's child parcels to adopt their head's tracking URL
                        // if (parcelHead.TrackingURL != null)
                        // {
                        //     parcel.TrackingURL = parcelHead.TrackingURL;
                        // }
                    });

                    parcelHead.Parcels = parcels;
                }

                deliveryNoteHead.ParcelHead = parcelHead;
            });

            return result;
        }

        private static IEnumerable<ParcelContent> ProcessParcelPieces(
            OrderParcelDetail parcel,
            List<MertexerV2.Models.TCL.SubSubValue> piecesForThisPartNumber,
            MertexerV2.Models.TCL.SubSubValue codeEntry,
            MertexerV2.Models.TCL.TCLItemResult dnh,
            List<OrderLineRecord> orderLines)
        {
            var result = new List<ParcelContent>();

            foreach (var piece in piecesForThisPartNumber)
            {
                var productCode = codeEntry.Value.Replace(" ", ""); // Product codes should not contain spaces.

                // <53>PCL.PCELENS
                // Find the corresponding piece length for this parcel for this product code for this piece.
                var quantity = dnh.Columns.Where(p => p.Index == 53)
                                            .SelectMany(p => p.MultiValues)
                                            .Where(p => p.Index == parcel.ParcelIndex)
                                            .Select(p => p.SubValues.Count() > 1
                                                        ? p.SubValues
                                                            .Where(sv => sv.Index == piece.Index)
                                                            .Single()
                                                            .Value
                                                        : p.Value).Single();

                var orderLineStub = parcel.OrderLines.Where(p => p.ParcelNumber == parcel.ParcelNumber && p.ProductCode == productCode).FirstOrDefault();

                // The SingleOrDefault is because Shipments can contain multiple Orders
                // Our user is searching for *just one* order, and I don't agree with polluting their results
                // with orders other than the one for which they searched.

                // This null testing is necessary because Mertex sometimes creates 'ghost' parcels with no contents:
                // the PickingList header will describe say, two parcels -- and the picking lines will describe themselves
                // as contributing to both, but this fact will conflict with the MV fields on the header which can indicate that one parcel 
                // was in fact empty.
                if (orderLineStub != null)
                {
                    var orderLine = orderLines.Where(p => p.LineKey == orderLineStub.OrderLineKey).SingleOrDefault();

                    if (orderLine != null)
                    {
                        result.Add(new ParcelContent(
                            index: parcel.ParcelIndex,
                            productCode: productCode,
                            productDescription: orderLine.ProductDescription,
                            quantity: Convert.ToDecimal(quantity.Replace(" ", "")) / 1000,
                            parcelNumber: parcel.ParcelNumber,
                            orderLineKey: orderLineStub.OrderLineKey,
                            orderLineQuantity: orderLine.Quantity,
                            stockUnit: orderLine.StockUnit,
                            pieceNumber: piece.Value.Replace(" ", ""),
                            pickingListIdentifier: orderLineStub.PickingListLineIdentifier,
                            remainingToDespatchQuantity: null
                        ));
                    }
                }
            }

            return result;
        }

        public static List<ParcelContent> ProcessParcelPieces(
            OrderParcelDetail parcel,
            List<MertexerV2.Models.TCL.SubValue> pieces,
            MertexerV2.Models.TCL.SubSubValue codeEntry,
            MertexerV2.Models.TCL.TCLItemResult dnh,
            List<OrderLineRecord> orderLines)
        {
            var result = new List<ParcelContent>();

            foreach (var piece in pieces.SelectMany(p => p.SubValues))
            {
                var productCode = codeEntry.Value.Replace(" ", ""); // Product codes should not contain spaces.

                // <53>PCL.PCELENS
                // Find the corresponding piece length for this parcel for this product code for this piece.
                var quantity = dnh.Columns.Where(p => p.Index == 53)
                                            .SelectMany(p => p.MultiValues)
                                            .Where(p => p.Index == parcel.ParcelIndex)
                                            .Select(p => p.SubValues.Count() > 1
                                                        ? p.SubValues
                                                            .Where(sv => sv.Index == piece.Index)
                                                            .Single()
                                                            .Value
                                                        : p.Value).Single();

                var orderLineStub = parcel.OrderLines.Where(p => p.ParcelNumber == parcel.ParcelNumber && p.ProductCode == productCode).FirstOrDefault();

                if (orderLineStub != null)
                {
                    var orderLine = orderLines.Where(p => p.LineKey == orderLineStub?.OrderLineKey).SingleOrDefault();

                    if (orderLine != null)
                    {
                        result.Add(new ParcelContent(
                            index: parcel.ParcelIndex,
                            productCode: productCode,
                            productDescription: orderLine.ProductDescription,
                            quantity: Convert.ToDecimal(quantity.Replace(" ", "")) / 1000,
                            parcelNumber: parcel.ParcelNumber,
                            orderLineKey: orderLineStub.OrderLineKey,
                            orderLineQuantity: orderLine.Quantity,
                            stockUnit: orderLine.StockUnit,
                            pieceNumber: piece.Value.Replace(" ", ""),
                            pickingListIdentifier: orderLineStub.PickingListLineIdentifier,
                            remainingToDespatchQuantity: null
                        ));
                    }
                }
            }

            return result;
        }

        public static ParcelContent ProcessParcelQuantity(
            OrderParcelDetail parcel,
            MertexerV2.Models.TCL.SubSubValue codeEntry,
            MertexerV2.Models.TCL.TCLItemResult dnh,
            List<OrderLineRecord> orderLines)
        {
            // We should be dealing with a non-piece order line

            var productCode = codeEntry.Value.Replace(" ", ""); // Product codes should not contain spaces.

            // <54>PCL.QUANTITY
            // Find the corresponding quantity for this product code.
            var quantity = dnh.Columns.Where(p => p.Index == 54)
                                        .SelectMany(p => p.MultiValues)
                                        .Where(p => p.Index == parcel.ParcelIndex)
                                        .Select(p => p.SubValues.Count() > 1
                                                    ? p.SubValues
                                                        .Where(sv => sv.Index == codeEntry.Index)
                                                        .Single()
                                                        .Value
                                                    : p.Value).Single();

            var orderLineStub = parcel.OrderLines.Where(p => p.ParcelNumber == parcel.ParcelNumber && p.ProductCode == productCode).FirstOrDefault();

            if (orderLineStub != null)
            {
                var orderLine = orderLines.Where(p => p.LineKey == orderLineStub?.OrderLineKey).SingleOrDefault();

                if (orderLine != null)
                {
                    return new ParcelContent(
                        index: parcel.ParcelIndex,
                        productCode: productCode,
                        productDescription: orderLine.ProductDescription,
                        quantity: Convert.ToDecimal(quantity.Replace(" ", "")) / 1000,
                        parcelNumber: parcel.ParcelNumber,
                        orderLineKey: orderLineStub.OrderLineKey,
                        orderLineQuantity: orderLine.Quantity,
                        stockUnit: orderLine.StockUnit,
                        pieceNumber: null,
                        pickingListIdentifier: orderLineStub.PickingListLineIdentifier,
                        remainingToDespatchQuantity: null
                    );
                }
            }

            return null;
        }

        public static void FetchTrackingURL(
            string uniVerseAccount,
            ref OrderParcelDetail detail)
        {
            using (var ce = new CommandExecutor(uniVerseAccount))
            {
                var results = ce.Arbitrary($@"LIST OEFINDPARCEL WEB.URL CUSTOM.TRACK.URL.DES {detail.ParcelIdentifier}")
                                .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                .ToList();

                if (results.Count == 2)
                {
                    string ReflexTrackingURL = results[0];
                    string DGTrackingURL = results[1];

                    detail.TrackingURL = detail.ParcelIdentifier.StartsWith("FL/") ? DGTrackingURL : ReflexTrackingURL;
                }
            }
        }

        public static void CheckHaulierTrackable(
            string uniVerseAccount,
            ref ParcelHeadResult head)
        {
            using (var ce = new CommandExecutor(uniVerseAccount))
            {
                var results = ce.Arbitrary($@"LIST OEHAULIER NAME WEBPARCEL {head.HaulierCode}")
                                .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                                .ToList();

                string parcelUrlPart1 = null;

                if (results.Count == 2)
                {
                    head.HaulierName = results[0];
                    parcelUrlPart1 = results[1];
                }

                if (!String.IsNullOrEmpty(head.HaulierName))
                {
                    var ti = new System.Globalization.CultureInfo("en-GB").TextInfo;

                    head.HaulierName = ti.ToTitleCase(head.HaulierName.ToLowerInvariant());
                }

                if (String.IsNullOrEmpty(parcelUrlPart1) && head.HaulierCode != "FL")
                {
                    head.Untrackable = $"Sorry, this was despatched via {head.HaulierCode} {(String.IsNullOrEmpty(head.HaulierName) ? "" : $" - {head.HaulierName}")} and is not trackable.";
                }
            }
        }
    }
}
