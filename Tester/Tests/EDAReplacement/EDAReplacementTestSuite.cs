using MertexerV2.Models.EDAReplacement;
using MertexerV2.Models.General;
using MertexerV2.Models.PurchaseOrder;

using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tester.Utilities;

namespace Tester.Tests.EDAReplacement
{
    public static class EDAReplacementTestSuite
    {
        public static List<TOEHead> TOEHEADSearchTest(string uniVerseAccount, string orderNumbers, DateTime? fromDt, DateTime? toDt, string tradeAccountCode)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite TOEHEAD Search Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchOrderHeads(uniVerseAccount, orderNumbers, fromDt, toDt, tradeAccountCode)
                .OfType<TOEHead>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            var nocode = results.Where(p => p.TradeAccountCode == null).ToList();

            var oh = results.Where(p => p.OrderID == "1017933602").SingleOrDefault();

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite TOEHEAD parallel operation returned {results.Count} in {stopWatch.Elapsed:g}");

            if (nocode.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine($"Of which {nocode.Count} were lacking a Trade Account Code!");

                Console.ForegroundColor = ConsoleColor.Green;
            }

            Console.WriteLine($"EDAReplacementTestSuite TOEHEAD Search Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            return results;
        }

        //public static void TOELINESearchTest(string uniVerseAccount, string lineKey, int? orderNumber, byte? lineStatusId, DateTime? fromDt, DateTime? toDt)
        //{
        //    Console.WriteLine(TesterFramework.WavyLineBreak);

        //    Console.WriteLine("Beginning EDAReplacementTestSuite TOELINE Search Tests...");

        //    Console.WriteLine(TesterFramework.WavyLineBreak);

        //    var stopWatch = new Stopwatch();

        //    stopWatch.Start();

        //    var results = MertexerV2.EDAReplacement.EDAReplacement.FetchOrderLines(uniVerseAccount, lineKey, orderNumber, lineStatusId, fromDt, toDt)
        //        .OfType<TOELine>().ToList();

        //    stopWatch.Stop();

        //    Console.ForegroundColor = ConsoleColor.Yellow;

        //    Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

        //    Console.ForegroundColor = ConsoleColor.Green;

        //    Console.WriteLine($"EDAReplacementTestSuite TOELINE parallel operation returned {results.Count} in {stopWatch.Elapsed:g}");

        //    Console.WriteLine($"EDAReplacementTestSuite TOELINE Search Test Complete.");

        //    Console.WriteLine(TesterFramework.WavyLineBreak);
        //}

        public static List<TOELine> TOELINESearchByOrderNumbersTest(string uniVerseAccount, string orderHeadIds, bool? lineStatusLessThan5, DateTime? fromDt, DateTime? toDt)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite TOELINE Search Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchOrderLinesForOrderHeads(uniVerseAccount, orderHeadIds, lineStatusLessThan5, fromDt, toDt)
                .OfType<TOELine>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite TOELINE Search parallel operation returned {results.Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"EDAReplacementTestSuite TOELINE Search Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            return results;
        }

        public static void STPRODLOCSearchByProductCodeLocationCodeTest(string uniVerseAccount, string productCodes, string locationCodes)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite STPRODLOC Search by Product Codes / Location Codes Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchProductLocations(uniVerseAccount, productCodes, locationCodes)
                .OfType<STProdLoc>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            //Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite STPRODLOC Search by Product Codes / Location Codes parallel operation returned {results.Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"EDAReplacementTestSuite STPRODLOC Search by Product Codes / Location Codes Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);
        }

        public static List<PurchaseOrderHead> PurchaseOrderHeadSearch(string uniVerseAccount, string purchaseOrderHeads, DateTime? fromDt, DateTime? toDt)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite POHEAD Search Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.POFunctions.PurchaseOrderHeadFunctions.Select(uniVerseAccount, purchaseOrderHeads, fromDt, toDt)
                .OfType<PurchaseOrderHead>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            var headers = results.Where(p => p.OrderID != null).ToList();

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite POHEAD Search parallel operation returned {results.Count()} in {stopWatch.Elapsed:g}");

            var orderNumbers = results.Where(p => p.OrderID != null).ToList();

            Console.WriteLine($"EDAReplacementTestSuite POHEAD Search Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            return results;
        }

        public static List<PurchaseOrderDetail> PurchaseOrderDetailSearch(string uniVerseAccount, string purchaseOrderHeads)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite PODETAIL Search Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.POFunctions.PurchaseOrderDetailFunctions.Select(uniVerseAccount, purchaseOrderHeads)
                .OfType<PurchaseOrderDetail>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite PODETAIL Search parallel operation returned {results.Count()} in {stopWatch.Elapsed:g}");

            var orderNumbers = results.Where(p => p.OrderID != null).ToList();

            Console.WriteLine($"EDAReplacementTestSuite PODETAIL Search Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            return results;
        }

        public static List<STProdMaster> FetchProducts(string uniVerseAccount, string productCodes)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite STPRODMASTER Search Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchProductRecords(uniVerseAccount, productCodes)
                .OfType<STProdMaster>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            if (productCodes != null)
            {
                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
            }

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite STPRODMASTER Search parallel operation returned {results.Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"EDAReplacementTestSuite STPRODMASTER Search Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            return results;
        }

        public static List<ProdCode> FetchProductCodes(string uniVerseAccount)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite STPRODMASTER List Code Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchProductCodes(uniVerseAccount)
                .OfType<ProdCode>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite STPRODMASTER List Code Complete.");

            Console.WriteLine($"EDAReplacementTestSuite STPRODMASTER List Code Fetched {results.Count} Codes in {stopWatch.Elapsed}");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            return results;
        }

        public static void OECarriages(string uniVerseAccount)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite OECARRIAGE Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchCarriageRecords(uniVerseAccount)
                .OfType<CodeDescription>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite OECARRIAGE parallel operation returned {results.Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"EDAReplacementTestSuite OECARRIAGE Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);
        }


        public static void STLocation(string uniVerseAccount)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite STLOCATION Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchStockLocations(uniVerseAccount)
                .OfType<CodeDescription>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite STLOCATION parallel operation returned {results.Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"EDAReplacementTestSuite STLOCATION Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);
        }

        public static void SalesMarkets(string uniVerseAccount)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite SalesMarkets Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchSalesMarkets(uniVerseAccount)
                .OfType<CodeDescription>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite SalesMarkets parallel operation returned {results.Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"EDAReplacementTestSuite SalesMarkets Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);
        }

        public static void ItemTypes(string uniVerseAccount)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite ItemTypes Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchItemTypes(uniVerseAccount)
                .OfType<CodeDescription>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite ItemTypes parallel operation returned {results.Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"EDAReplacementTestSuite ItemTypes Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);
        }

        public static void FetchQualities(string uniVerseAccount)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite Quality Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchQualities(uniVerseAccount)
                .OfType<CLQualDes>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite Quality parallel operation returned {results.Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"EDAReplacementTestSuite Quality Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);
        }

        public static void FetchCompositionCodes(string uniVerseAccount)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite Composition Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchCompositions(uniVerseAccount)
                .OfType<CLComp>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite Composition parallel operation returned {results.Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"EDAReplacementTestSuite Composition Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);
        }

        public static List<OEDNHead> FetchDespatchNoteHeads(string uniVerseAccount, string despatchIds)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite Despatch Head Fetch Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchDespatchNoteHeads(uniVerseAccount, despatchIds)
                .OfType<OEDNHead>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite Despatch Head Fetch parallel operation returned {results.Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"EDAReplacementTestSuite Despatch Head Fetch Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            return results;
        }


        public static List<OEDNLine> FetchDespatchNoteLines(string uniVerseAccount, string despatchLineIds)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite Despatch Line Fetch Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchDespatchNoteLines(uniVerseAccount, despatchLineIds)
                .OfType<OEDNLine>().ToList();

            results.ForEach(o => 
            {
                SqlString DespatchHeadID,
                          DespatchLineID,
                          Source,
                          ProductCode,
                          OriginalParcelID,
                          ParcelJSON;

                MertexerV2.EDAReplacement.EDAReplacement.FetchDespatchNoteLines_FillRow(
                    (object)o,
                    out DespatchHeadID,
                    out DespatchLineID,
                    out Source,
                    out ProductCode,
                    out OriginalParcelID,
                    out ParcelJSON);
            });

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            var hasParcels = results.Where(p => p.Parcels.Any()).ToList();

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite Despatch Line Fetch parallel operation returned {results.Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"EDAReplacementTestSuite Despatch Line Fetch Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            return results;
        }

        public static void FetchHauliers(string uniVerseAccount)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite Haulier Fetch Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchHauliers(uniVerseAccount)
                .OfType<OEHaulier>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite Haulier Fetch operation returned {results.Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"EDAReplacementTestSuite Haulier Fetch Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);
        }

        public static void FetchSuppliers(string uniVerseAccount)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite Supplier Fetch Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchSuppliers(uniVerseAccount)
                .OfType<Supplier>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite Supplier Fetch operation returned {results.Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"EDAReplacementTestSuite Supplier Fetch Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);
        }


        public static void FetchPurchaseOrderPrices(string uniVerseAccount, string supplierProductCodes)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite PurchasePrice Fetch Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchPurchasePrices(uniVerseAccount, supplierProductCodes)
                .OfType<POListPrice>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine($"EDAReplacementTestSuite PurchasePrice Fetch operation returned {results.Count} in {stopWatch.Elapsed:g}");

            //Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite PurchasePrice Fetch Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);
        }

        public static void FetchExchangeRates(string uniVerseAccount)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite ExchangeRate Fetch Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchExchangeRates(uniVerseAccount)
                .OfType<XRate>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite ExchangeRate Fetch operation returned {results.Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"EDAReplacementTestSuite ExchangeRate Fetch Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);
        }

        public static void FetchDeliveryAddresses(string uniVerseAccount, string deliveryReference, string customers, string markets)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite DeliveryAddress Fetch Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchDeliveryAddresses(uniVerseAccount, deliveryReference, customers, markets)
                .OfType<DelAdd>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite DeliveryAddress Fetch operation returned {results.Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"EDAReplacementTestSuite DeliveryAddress Fetch Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);
        }

        public static List<TCCHead> FetchCCRHeaders(string uniVerseAccount, string ccrNumbers, string marketCodes, DateTime? fromDt, DateTime? toDt)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite CRR Header Fetch Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchCCRHeaders(uniVerseAccount, ccrNumbers, marketCodes, fromDt, toDt)
                .OfType<TCCHead>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            //Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite CRR Header Fetch operation returned {results.Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"EDAReplacementTestSuite CRR Header Fetch Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            return results;
        }

        public static List<TCCLine> FetchCCRLines(string uniVerseAccount, string ccrNumbers)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite CRR Line Fetch Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchCCRLines(uniVerseAccount, ccrNumbers)
                .OfType<TCCLine>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite CRR Line Fetch operation returned {results.Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"EDAReplacementTestSuite CRR Line Fetch Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            return results;
        }

        public static List<TCCAction> FetchCCRActions(string uniVerseAccount, string ccrNumbers)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite CRR Action Fetch Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchCCRActions(uniVerseAccount, ccrNumbers)
                .OfType<TCCAction>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            //Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite CRR Action Fetch operation returned {results.Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"EDAReplacementTestSuite CRR Action Fetch Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            return results;
        }

        public static List<EDAINVSUND> FetchSundries(string uniVerseAccount, string sundryCodes)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite Sundries Fetch Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchSundries(uniVerseAccount, sundryCodes)
                .OfType<EDAINVSUND>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite Sundries Fetch operation returned {results.Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"EDAReplacementTestSuite Sundries Fetch Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            return results;
        }

        public static List<TOEHead> FetchTuffnellsOrderHeads(string uniVerseAccount, DateTime? fromDt)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite Tuffnells OrderHead Fetch Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchTuffnellsOrderHeads(uniVerseAccount, fromDt)
                .OfType<TOEHead>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            Console.ForegroundColor = ConsoleColor.Green;

            var oh = results.Where(p => p.OrderID == "1079337602").SingleOrDefault();

            Console.WriteLine($"EDAReplacementTestSuite Tuffnells OrderHead Fetch operation returned {results.Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"EDAReplacementTestSuite Tuffnells OrderHead Fetch Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            return results;
        }

        public static List<OEDNHead> FetchTuffnellsDespatchHeads(string uniVerseAccount, string despatchIds, DateTime? fromDt)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite Tuffnells Despatch Fetch Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchTuffnellsDespatchNoteHeads(uniVerseAccount, despatchIds, fromDt)
                .OfType<OEDNHead>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite Tuffnells Despatch Fetch operation returned {results.Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"EDAReplacementTestSuite Tuffnells Despatch Fetch Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            return results;
        }

        public static List<EDAADDITIONAL> FetchFlamCodes(string uniVerseAccount)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite FlamCode Fetch Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchFlamCodes(uniVerseAccount)
                .OfType<EDAADDITIONAL>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite FlamCode Fetch operation returned {results.Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"EDAReplacementTestSuite FlamCode Fetch Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            return results;
        }

        public static List<STProdAftCare> FetchAftercares(string uniVerseAccount)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite Aftercare Fetch Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchAftercares(uniVerseAccount)
                .OfType<STProdAftCare>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite Aftercare Fetch operation returned {results.Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"EDAReplacementTestSuite Aftercare Fetch Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            return results;
        }

        public static List<OEOANALInvoiceMovement> FetchDespatchStationsForDespatches(string uniVerseAccount, string despatchHeadIds)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite DespatchStation Fetch Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var inputDespatchHeadIDs = JsonConvert.DeserializeObject<List<DespatchHeadIDOrderID>>(despatchHeadIds);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.EDAReplacement.EDAReplacement.FetchDespatchStationsForDespatches(uniVerseAccount, despatchHeadIds)
                .OfType<OEOANALInvoiceMovement>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite DespatchStation operation returned {results.Count} in {stopWatch.Elapsed:g}");

            var invoiced = results.Where(p => p.DespatchStation != null).Count();

            Console.WriteLine($"Out of {inputDespatchHeadIDs.Count} input DespatchHeadIDs, we found an PIK movement for: {invoiced}.");

            Console.WriteLine($"EDAReplacementTestSuite DespatchStation Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            return results;
        }

        public static string JohnLewisProductReference(string uniVerseAccount, string johnLewisOrderNumber, short lineNumber)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite JohnLewisProductReference Fetch Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var result = MertexerV2.EDAReplacement.EDAReplacement.JohnLewisProductReference(uniVerseAccount, johnLewisOrderNumber, lineNumber);

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite JohnLewisProductReference operation returned {result} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"EDAReplacementTestSuite JohnLewisProductReference Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            return result;
        }

        public static List<PRICECROSS> FetchPriceCrosses(string uniVerseAccount)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning EDAReplacementTestSuite PriceCrosses Fetch Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var result = MertexerV2.EDAReplacement.EDAReplacement.FetchPriceCrosses(uniVerseAccount)
                .OfType<PRICECROSS>().ToList();

            stopWatch.Stop();

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"EDAReplacementTestSuite PriceCrosses operation returned {result.Count()} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"EDAReplacementTestSuite PriceCrosses Test Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            return result;
        }
    }
}
