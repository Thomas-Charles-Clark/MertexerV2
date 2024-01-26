using MertexerV2.Models.Status;
using MertexerV2.Models.VAT;
using MertexerV2.Models.EORI;
using MertexerV2.SQLFunctions;
using MertexerV2.Utilities;
using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using MertexerV2;
using Tester.Utilities;
using MertexerV2.Models.PreAllocation;
using Tester.Tests.OrderSearch;
using Newtonsoft.Json;
using System.Collections.Generic;
using Tester.Tests.EDAReplacement;
using MertexerV2.Models.EDAReplacement;
using System.Threading.Tasks;
using System.Data.SqlTypes;
using MertexerV2.GeneralFunctions;
using MertexerV2.GeneralFunctions.Pricing;
using MertexerV2.ProductFunctions;
using MertexerV2.Models.Product;
using MertexerV2.Models.Pricing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using MertexerV2.Models.General;
using MertexerV2.Models.Invoices;
using System.Globalization;
using MertexerV2.InvoiceFunctions;
using MertexerV2.Models.Stock;
using MertexerV2.Models;
using MertexerV2.Models.Accounts;
using MertexerV2.AccountFunctions;
using MertexerV2.TradeAccountFunctions;
using System.CodeDom;
using MertexerV2.RetailFunctions;
using System.Diagnostics.Tracing;
using MertexerV2.EDAReplacement;
using static System.Collections.Specialized.BitVector32;
using MertexerV2.Models.CustomerCare;
using System.Security.Cryptography.X509Certificates;
using MertexerV2SQL;
using System.Management.Instrumentation;
using MertexerV2.PricingFunctions;

namespace Tester
{
    class Program
    {
        static void Main()
        {
            Console.ForegroundColor = ConsoleColor.Green; 

            Console.ForegroundColor = ConsoleColor.Yellow;

            var tradeAccountCode = "K0098";

            var tradeDataSourceJson = TesterFramework.FetchAccountDataSourceJson(tradeAccountCode);

            //tradeDataSourceJson = @"{""enquiryTypes"":[{""enquiryType"":""Account"",""dataSources"":[{""uniVerseAccount"":""MERTEX"",""customerReference"":""M2830""}]},{""enquiryType"":""Address"",""dataSources"":[{""uniVerseAccount"":""MERTEX"",""customerReference"":""M2830""}]},{""enquiryType"":""CheckOrders"",""dataSources"":[{""uniVerseAccount"":""MERTEX"",""customerReference"":""M2830""}]},{""enquiryType"":""Contact"",""dataSources"":[{""uniVerseAccount"":""MERTEX"",""customerReference"":""M2830""}]},{""enquiryType"":""CreditControls"",""dataSources"":[{""uniVerseAccount"":""MERTEX"",""customerReference"":""M2830""}]},{""enquiryType"":""CreditStatus"",""dataSources"":[{""uniVerseAccount"":""MERTEX"",""customerReference"":""M2830""}]},{""enquiryType"":""EndCustomers"",""dataSources"":[{""uniVerseAccount"":""MERTEX"",""customerReference"":""M2830""}]},{""enquiryType"":""General"",""dataSources"":[{""uniVerseAccount"":""MERTEX"",""customerReference"":""M2830""}]},{""enquiryType"":""Invoice"",""dataSources"":[{""uniVerseAccount"":""MERTEX"",""customerReference"":""M2830""}]},{""enquiryType"":""PlaceOrder"",""dataSources"":[{""uniVerseAccount"":""MERTEX"",""customerReference"":""M2830""}]}]}";

            var stopwatch = new System.Diagnostics.Stopwatch();

            var uniVerseAccount = "MERTEX";

            stopwatch.Start();


            //var headResult = MaxFieldLengths<CustomerCareRecordHead>.Execute();

            //Console.WriteLine(headResult);


            //var lineResult = MaxFieldLengths<CustomerCareRecordLine>.Execute();

            //Console.WriteLine(lineResult);


            //var actionResult = MaxFieldLengths<CustomerCareRecordAction>.Execute();

            //Console.WriteLine(actionResult);

            //using (var qe = new QueryExecutor(uniVerseAccount))
            //{
            //    var logCommand = qe.Session.CreateUniCommand();

            //    logCommand.Command = "PORT.STATUS";

            //    logCommand.Execute();

            //    var logCommandResult = logCommand.Response;

            //    //////var cmd = $"OE.PG.INIT.ORD1";
            //    ///
            //    /// 
            //    /////

            //    var argCount = 3;
            //    var sub = qe.Session.CreateUniSubroutine("HELLO.WORLD", argCount);

            //    var atOne = qe.Session.GetAtVariable(1);
            //    var atTwo = qe.Session.GetAtVariable(2);
            //    var atThree = qe.Session.GetAtVariable(3);
            //    var atFour = qe.Session.GetAtVariable(4);
            //    var atFive = qe.Session.GetAtVariable(5);

            //    var atSix = qe.Session.GetAtVariable(6);
            //    var atSeven = qe.Session.GetAtVariable(7);
            //    var atEight = qe.Session.GetAtVariable(8);
            //    var atNine = qe.Session.GetAtVariable(9);
            //    var atTen = qe.Session.GetAtVariable(10);

            //    sub.SetArg(0, "0");
            //    sub.SetArg(1, "TOM");
            //    sub.SetArg(2, $"{qe.Session.HostPort}");

            //    sub.Call();

            //    for (var i = 0; i < argCount; i++)
            //    {
            //        Console.WriteLine($"sub.GetArg({i}): {sub.GetArg(i)}");
            //    }
            //}



            //var deliveryAddressJson = @"{""Key"":null,""TradeAccountCode"":""S2390"",""CompanyName"":""SRA. UTE FLORY-URAK"",""ContactName"":""SRA. UTE FLORY-URAK"",""AddressLine1"":""ROMERWEG, 27"",""AddressLine2"":null,""Town"":""ST. JOHANN"",""County"":""ST. JOHANN"",""Postcode"":""A-6380"",""CountryCode"":""AT""}";

            //var result = DeliveryAddressFunctions.Upsert("MERTEX", deliveryAddressJson);

            //var json = $@"[{"OrderHeadID": 1108937001}]";

            //var results = EDAReplacement.FetchOrderLinesForOrderHeads("MERTEX", json).OfType<TOELine>().ToArray();

            //foreach(var r in results)
            //{
            //    Console.WriteLine(r);
            //}

            //var results = MertexerV2.SQLFunctions.Dedupe.CheckLine(tradeDataSourceJson, "F1190/01/C", 1.0m, "SAMPLE ORDER", 3).OfType<string>().ToList();

            //var da = new DeliveryAddress(

            //    tradeAccountCode: "K0098",
            //    addressLine1: "line1",
            //    addressLine2: "line2",
            //    town: "town",
            //    county: "county",
            //    postcode: "postcode",
            //    countryCode: "GB",
            //    contactName: "Tom",
            //    companyName: "Tom");

            //da.Upsert(uniVerseAccount);

            //EDAReplacementTestSuite.OECarriages("MERTEX");

            //Console.WriteLine("TradeAccount");
            //var result = MaxFieldLengths<TradeAccount>.Execute(null, true, false, 50);
            //Console.WriteLine(result);
            //Console.WriteLine();
            //Console.WriteLine();

            //Console.WriteLine(Selector<TradeAccount>.Select("C4103", true, false));

            //var endCustomerResults = TradeAccountFunctions.EndCustomerSelect(tradeDataSourceJson, "Jaberi").OfType<string>().ToList();

            //Console.WriteLine($"Saw {endCustomerResults.Count()} invoice accounts in {stopwatch.Elapsed}");

            //var results = DeliveryAddressFunctions.Exists(uniVerseAccount, "K0098", "COMPANY NAME", "COMPANY NAME", "ADDRESS LINE 1", "ADDRESS LINE 2", "TOWN", "COUNTY", "POSTCODE", "GB").OfType<string>().ToList();




            //var results = TillFunctions.FetchRetailTillSessions(uniVerseAccount).OfType<RetailTillSession>().ToList();
            //var tills = TillFunctions.FetchOpenTills(uniVerseAccount).OfType<RetailTill>().ToList();

            //foreach (var r in results)
            //{
            //    Console.WriteLine(r);
            //}

            //foreach (var r in tills)
            //{
            //    Console.WriteLine(r);
            //}

            //var results = DeliveryReference.Search(tradeDataSourceJson,
            //    "Mackenzie...Company",
            //    "71...Suite...1",
            //    "Sand...",
            //    "MA",
            //    "MA",
            //    "02563",
            //    "US").OfType<DeliveryAddress>().ToList();

            //foreach(var r in results)
            //{
            //    Console.WriteLine(r);
            //}

            //Console.WriteLine("OEINVHEAD");
            //var headResults = InvoiceHeadFunctions.InitialiseSelect(uniVerseAccount, DateTime.ParseExact("01/11/2021", "dd/MM/yyyy", new CultureInfo("en-GB")), DateTime.ParseExact("03/03/2023", "dd/MM/yyyy", new CultureInfo("en-GB"))).OfType<InvoiceHead>().ToList();
            //Console.WriteLine();
            //Console.WriteLine();

            //Console.WriteLine($"Saw {headResults.Count} results in {stopwatch.Elapsed}");

            //var documentNumbers = headResults.OrderBy(hr => hr.InvoiceDate).ThenBy(hr => hr.Key).Select(r => new InvoiceIDWrapper() { InvoiceID = r.Key });

            //Console.WriteLine("OEINVLINE");
            //var lineResults = InvoiceLineFunctions.Select(uniVerseAccount, null, JsonConvert.SerializeObject(documentNumbers)).OfType<InvoiceLine>().ToList();
            //Console.WriteLine();
            //Console.WriteLine();

            //var results = TillFunctions.FetchOpenTills(uniVerseAccount).OfType<RetailTill>().ToList();

            //var results = SSLPostContactFunctions.Select("MERTEX", null, null).OfType<SSLPostContact>().ToList();

            //Console.WriteLine($"Saw {results.Count} results");


            //var Code = new SqlString();
            //var DocumentType = new SqlString();
            //var TradeAccountCode = new SqlString();
            //var Email = new SqlString();

            //foreach (var r in results)
            //{
            //    SSLPostContactFunctions.SSLPostContactSelect_FillRow(r, out Code, out DocumentType, out TradeAccountCode, out Email);
            //    //Console.WriteLine(r);
            //}


            //var result = TradeAccountFunctions.InsertTransactionHistory("MERTEX", @"{""Code"":""PH03005"",""DocumentHistory"":[],""MertexCurrencyCode"":""STG""}");

            //var sessions = TillFunctions.FetchRetailTillSessions(uniVerseAccount).OfType<RetailTillSession>().ToList();

            //foreach (var r in sessions)
            //{
            //    Console.WriteLine(r);
            //}

            //lineResults.ForEach(r =>
            //{
            //    Console.WriteLine(r);

            //    var InvoiceLineKey = new SqlString();
            //    var InvoiceNo = new SqlString();
            //    var ProductCode = new SqlString();
            //    var QuantityOrdered = new SqlDecimal();
            //    var QuantityDespatched = new SqlDecimal();
            //    var QuantityInvoiced = new SqlDecimal();
            //    var UnitPriceExVAT = new SqlDecimal();
            //    var LineValueExVAT = new SqlDecimal();
            //    var DiscountPercentage = new SqlDecimal();
            //    var VATCode = new SqlString();
            //    var VATRate = new SqlDecimal();
            //    var DiscountValue = new SqlDecimal();
            //    var VATValue = new SqlDecimal();
            //    var StockUnit = new SqlString();
            //    var UnitCost = new SqlDecimal();
            //    var UnitCostCurrency = new SqlDecimal();
            //    var LineCost = new SqlDecimal();
            //    var LineCostCurrency = new SqlDecimal();
            //    var UnitPriceExVATCurrency = new SqlDecimal();
            //    var LineValueExVATCurrency = new SqlDecimal();
            //    var VATValueCurrency = new SqlDecimal();
            //    var GeneralLedgerCode = new SqlString();

            //    InvoiceLineFunctions.InvoiceLineSelect_FillRow(
            //        r,
            //        out InvoiceLineKey,
            //        out InvoiceNo,
            //        out ProductCode,
            //        out QuantityOrdered,
            //        out QuantityDespatched,
            //        out QuantityInvoiced,
            //        out UnitPriceExVAT,
            //        out LineValueExVAT,
            //        out DiscountPercentage,
            //        out VATCode,
            //        out VATRate,
            //        out DiscountValue,
            //        out VATValue,
            //        out StockUnit,
            //        out UnitCost,
            //        out UnitCostCurrency,
            //        out LineCost,
            //        out LineCostCurrency,
            //        out UnitPriceExVATCurrency,
            //        out LineValueExVATCurrency,
            //        out VATValueCurrency,
            //        out GeneralLedgerCode);
            //});
            //

            //EDAReplacementTestSuite.FetchPurchaseOrderPrices(uniVerseAccount, @"[{ ""Key"": ""A0091*P462/01""  }]");

            //var result = Selector<TradeAccount>.Select("PH03000", true, true, 1000, uniVerseAccount);

            //Console.WriteLine(JsonConvert.SerializeObject(result, Framework.IndentedFormatting));



            //var results = MertexerV2.AccountFunctions.DeliveryAddressFunctions.Search(uniVerseAccount, "K0098", "123").OfType<DeliveryAddress>().ToList();

            //results.ForEach(r => Console.WriteLine(r));


            //Tests.ValidateCSV.ValidateCSV.Run(tradeOrderImportFileId: 22941);


            //var tradeAccountJson = TesterFramework.FetchTradeAccountUpsertJSON("PH03179");

            //using (var ce = new CommandExecutor(uniVerseAccount))
            //{
            //    var indexRebuildResult = ce.Arbitrary("LOGIN");
            //}

            // need to get from "real" TCL to SB/XA
            // might need to do a whole separate SSH connection to achieve this, ugh...


            //TradeAccountFunctions.Upsert("MERTEX", tradeAccountJson);

            //var tags = TradeAccountGroupFunctions.Select(uniVerseAccount, @"[{""Code"": ""ABDFAS""}]").OfType<TradeAccountGroup>().ToList();

            //foreach (var t in tags)
            //{
            //    Console.WriteLine(t);
            //}

            //var ta = TradeAccountFunctions.Select(uniVerseAccount, @"[{""Code"":""J0650""}]").OfType<TradeAccount>().ToList();

            //foreach(var t in ta)
            //{
            //    Console.WriteLine(t);
            //}

            //            var ta = JsonConvert.DeserializeObject<TradeAccount>(tradeAccountJson);

            //            Console.WriteLine(JsonConvert.SerializeObject(ta, Framework.JsonSettingsIndentedIncludeNullsAlphabetical));

            //            ta.CompanyRegistrationNumber = "Company Registration Number";

            //            var tradeAccountUpsertJson = JsonConvert.SerializeObject(ta);

            //            Console.WriteLine(TradeAccountFunctions.Upsert(uniVerseAccount, @"{
            //    ""TradeAccountCode"": ""K0098"",
            //    ""CompanyRegistrationNumber"": ""Company Reg. No.""
            //}"));

            // That's all well and good, but...

            // ... what if we want to only affect those fields we've provided?

            // I'd go so far as to suggest that that would be the default intention: if you want to leave a field unaffected
            // don't provide it in the JSON







            //var testUpsertResult = Tests.Account.AccountTestSuite.UpsertAccount(tradeDataSourceJson, TesterFramework.FetchTradeAccountUpsertJSON());

            //var stockResults = Stock.BatchPiece("FDG2821/01", "WH", "S").OfType<BatchPieceResult>().ToList();

            ////var singularStockResult = Stock.B2CEnquiry_Singular("FRL5272/01", "WH").OfType<B2CEnquiryResult>().ToList();

            //Console.ForegroundColor = ConsoleColor.Yellow;

            //stockResults.ForEach(r => Console.WriteLine(r));

            //using (var ce = new CommandExecutor(uniVerseAccount)) 
            //{
            //    //var result = ce.Arbitrary("/REGEN.INDEX SLCUST SLCUST SORTNAME");
            //    //var result = ce.Arbitrary("/REGEN.INDEX SLCUST SLCUST SORTNAME");
            //    var result = ce.Arbitrary("SB.LOGIN tom,gui");

            //    Console.WriteLine(result);
            //}




            //var contractPriceResults = ContractPriceFunctions.Select(uniVerseAccount, null, "[{ \"Code\": \"H9018*FA*DGA\" }]").OfType<ContractPriceExplosion>().ToList();
            ////var contractPriceResults = ContractPriceFunctions.Select(uniVerseAccount, "D5150", null).OfType<ContractPriceExplosion>().ToList();
            ////var contractPriceResults = ContractPriceFunctions.Select(uniVerseAccount, null, null).OfType<ContractPriceExplosion>().ToList();

            //Console.WriteLine($"Saw {contractPriceResults.Count} ContractPriceExplosion results");

            //contractPriceResults.ForEach(r => Console.WriteLine(r));


            //var contractDiscoResults = ContractDiscountFunctions.Select(uniVerseAccount, "E9100", null).OfType<ContractDiscountExplosion>().ToList();
            ////var contractPriceResults = ContractPriceFunctions.Select(uniVerseAccount, "D5150", null).OfType<ContractPriceExplosion>().ToList();
            ////var contractPriceResults = ContractPriceFunctions.Select(uniVerseAccount, null, null).OfType<ContractPriceExplosion>().ToList();

            //Console.WriteLine($"Saw {contractDiscoResults.Count} ContractDiscountExplosion results");

            //contractDiscoResults.ForEach(r => Console.WriteLine(r));



            var results = ContractDiscountFunctions.Upsert(uniVerseAccount, @"{
	            ""Code"": ""K0098*FDG2345/01"",
	            ""TradeAccountCode"": ""K0098"",
	            ""ProductCode"": ""FDG2345/01"",
	            ""SortType"": ""Q"",
	            ""Quality"": null,
	            ""PromotionCode"": ""DG13"",
	            ""BrandCode"": null,
	            ""ProductGroupCode"": null,
	            ""ItemTypeCode"": null,
	            ""StartDTs"": [""2024/01/26""],
	            ""ExpiryDTs"": [""2024/04/26""],
	            ""ContractDiscounts"": [10.00],
	            ""LastAmendedDTs"": [""2024/01/26 17:07:00""],
	            ""QuantitiesRequired"": [0]
            }");



            //int result = 0;

            //MertexerV2.SQLFunctions.Order.Submit(1140305001, "portal.Mertex_Order_Select", out result);

            //var headresults = EDAReplacementTestSuite.FetchDespatchNoteHeads(uniVerseAccount, "[{ DespatchHeadID: \"4701346-1\" }]");

            //var lineresults = EDAReplacementTestSuite.FetchDespatchNoteLines(uniVerseAccount, "[{ DespatchLineID: \"4320536-1*1\" }]");

            //Console.WriteLine($"Saw {stockResults.Count} results in {stopwatch.Elapsed}");

            //var results = MertexerV2.SQLFunctions.QuantityBreakCostPrice.Select("MERTEX", "[{ \"Code\": \"FDG2749/31\" }]", "FA", "DG").OfType<QuantityBreakCostPriceResult>().ToList();
            //var results = MertexerV2.SQLFunctions.QuantityBreakCostPrice.Select("MERTEX", null, "FA", "EH").OfType<QuantityBreakCostPriceResult>().ToList();

            //results.ForEach(r => Console.WriteLine(r));

            //MertexerV2.Accounts.AccountFunctions.AccountSummary(tradeDataSourceJson);

            //var recordKey = "RUGDG0394*\\U0016";

            //var records = ProductExclusionFunctions.Select("MERTEX", null).OfType<ProductExclusionDerivation>().ToList();

            //var records = ProductGroupFunctions.Select("MERTEX", null).OfType<ProductGroup>().ToList();

            //Console.WriteLine($"Saw {records.Count} results in {stopwatch.Elapsed}");

            //records.ForEach(r => 
            //{
            //    Console.WriteLine(r);

            //    var Key = new SqlString();
            //    var ProductCode = new SqlString();

            //    ProductExclusionFunctions.ProductExclusionSelect_FillRow(
            //        r,
            //        out Key,
            //        out ProductCode);
            //});

            //var result = ProductExclusionFunctions.Upsert("PLAY", @"{
            // ""Key"": ""POTATO"",
            // ""ProductsExcluded"": [""FDG2345/01"", ""FDG2345/02""],
            // ""Description"": ""This potato intentionally left blank""
            //}");

            //var record = new Product(recordKey);

            //UniTestRecordBuilder.InitialiseTestRecord(typeof(Product), record);

            //record.Upsert(uniVerseAccount);

            //var results = PreAllocation.Create("TOM-TESTING", "FDG2839/01", "WH", 1, "M", null, null, DateTime.Now.AddDays(1), null, null).OfType<PreAllocationCreation>().ToList();

            //results.ForEach(r => Console.WriteLine(r));


            //var results = POPriceListFunctions.Select("MERTEX", "[{ Code: \"S0049*F1134\" }]")
            //    .OfType<POPriceListExplosion>().ToList();

            //results.ForEach(r => Console.WriteLine(r));


            //Stock.BatchPiece("F1723/57", "WH", "S");

            //Console.ForegroundColor = ConsoleColor.Magenta;

            ////Item("SLCUST", "PH03154");

            //Console.ForegroundColor = ConsoleColor.Cyan;

            //Item("SLCUST", "PH03155");

            Console.ForegroundColor = ConsoleColor.Green;

            stopwatch.Stop();

            Console.WriteLine($"Completed in {stopwatch.Elapsed}");

            Console.WriteLine();

            Console.WriteLine("Press any key to continue...");

            Console.ReadKey();
        }

        private static void Dictionary(string fileName, string database = "MERTEX")
        {
            using (var ce = new CommandExecutor(database))
            {
                string dict = ce.Dictionary(fileName);
            }
        }

        private static void DictionaryXML(string fileName, string database = "MERTEX")
        {
            using (var ce = new CommandExecutor(database))
            {
                string dict = ce.DictionaryXML(fileName);
            }
        }


        private static void DictionaryVertical(string fileName)
        {
            using (var ce = new CommandExecutor())
            {
                string dict = ce.DictionaryVertical(fileName);
            }
        }

        private static string DictionaryItem(string fileName, string[] fieldNames = null, string universeAccount = "MERTEX")
        {
            var result = "";

            using (var ce = new CommandExecutor(universeAccount))
            {
                result = ce.DictionaryItem(fileName, fieldNames);
            }

            //Console.WriteLine(item);

            return result;
        }

        private static void Item(string fileName, string recordId, bool lineSuppression = true, string universeAccount = "MERTEX")
        {
            var item = "";

            using (var ce = new CommandExecutor(universeAccount))
            {
                item = ce.Item(fileName, recordId, lineSuppression);
            }

            Console.WriteLine(item);
        }

        public static int WTAccessInternalImportDenial(string universeAccount)
        {
            int result = 0;

            using (var ce = new CommandExecutor(universeAccount))
            {
                // need to alter every record in WTACCESS

                var fields = new string[] { "IMPORT" };

                var wtaccess = ce.Session.CreateUniFile("WTACCESS");

                var keys = new List<string>();

                var listCmd = $"LIST WTACCESS PIN FMT\"40L\" ID.SUPP COL.HDR.SUPP COUNT.SUP";
                var listResult = ce.Arbitrary(listCmd);
                var theseKeys = listResult
                    .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                keys.AddRange(theseKeys);

                var batches = ListExtension.Split(keys, 1000);

                var locker = new object();

                //Parallel.ForEach(batches, (batch) =>
                //{
                foreach (var batch in batches)
                {
                    try
                    {
                        var records = wtaccess.ReadRecords(batch.ToArray(), fields);

                        foreach (var b in batch)
                        {
                            var readResult = wtaccess.ReadField(b, 116);

                            var toWrite = ce.Session.CreateUniDynArray();

                            toWrite.Insert(0, "N");

                            wtaccess.LockRecord(b, 1);
                            wtaccess.WriteField(b, 116, toWrite);
                            wtaccess.UnlockRecord(b);
                        }
                    }
                    catch (IBMU2.UODOTNET.UniFileException ufe)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ufe);
                    }
                }
                //});
            }

            return result;
        }
    }

    public class SessionResult
    {
        public long SessionNo { get; set; }
        public DateTime OpenedOn { get; set; }
        public TimeSpan OpenedAt { get; set; }
        public string Label { get; set; }
    }
}