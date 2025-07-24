using MertexerV2;
using MertexerV2.Models.Status;
using MertexerV2.Models.VAT;
using MertexerV2.Models.EORI;
using MertexerV2.SQLFunctions;
using MertexerV2.Utilities;
using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
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
using MertexerV2.Models.CustomerCare;
using System.Security.Cryptography.X509Certificates;
//using MertexerV2SQL;
using System.Management.Instrumentation;
using MertexerV2.PricingFunctions;
using System.Text;
using Tester.Tests.LineStatus;
using System.IO;
using MertexerV2.Models.SalesAnalysis;
using MertexerV2.Models.PurchaseLedger;
using MertexerV2.SupplierFunctions;
using MertexerV2.Models.Supplier;
using MertexerV2.SalesLedgerFunctions;
using MertexerV2.Models.SalesLedger;
using MertexerV2.Models.Credit;
using MertexerV2.PurchaseLedgerFunctions;

namespace Tester
{
    class Program
    {
        static void Main()
        {
            JsonConvert.DefaultSettings = () => {
                return new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Include
                };
            };

            Console.ForegroundColor = ConsoleColor.Green; 

            Console.ForegroundColor = ConsoleColor.Yellow;

            var tradeAccountCode = "K0098";

            //var tradeDataSourceJson = TesterFramework.FetchAccountDataSourceJson(tradeAccountCode);

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

            //var purchaseOrderDetails = EDAReplacement.FetchPurchaseOrderDetails("MERTEX", @"[{ ""PurchaseOrderHeadID"": ""P259783"" }]").OfType<PODetail>();

            //foreach(var pod in purchaseOrderDetails)
            //{
            //    Console.WriteLine(pod);
            //}

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

            //var stockResults = Stock.B2CEnquiry(@"{ ""ProductGroups"": [{""Name"":""Fabric"",""StockLocations"":[{""Code"":""WH""}]},{""Name"":""Wallpaper"",""StockLocations"":[{""Code"":""WH""}]},{""Name"":""Samples"",""StockLocations"":[{""Code"":""SR""}],""ProductCodes"":[{""ProductCode"":""P559\/08\/R"",""StockCheckMinimum"":-9999.00}]},{""Name"":""Furniture"",""StockLocations"":[{""Code"":""KRFURN""},{""Code"":""MHSFURN""},{""Code"":""PRF""},{""Code"":""WH""}]},{""Name"":""Fenwick"",""StockLocations"":[{""Code"":""FOL""},{""Code"":""FTW""},{""Code"":""FKI""},{""Code"":""FBC""},{""Code"":""FCA""},{""Code"":""FNC""},{""Code"":""FCO""},{""Code"":""FBO""},{""Code"":""FYO""},{""Code"":""FBR""}]},{""Name"":""EverythingElse"",""StockLocations"":[{""Code"":""EC""},{""Code"":""GPS""},{""Code"":""KR""},{""Code"":""MHS""},{""Code"":""NFS""},{""Code"":""NGS""},{""Code"":""WH""}]}] }").OfType<B2CEnquiryResult>().ToList();

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



            //var results = ContractDiscountFunctions.Upsert(uniVerseAccount, @"{
            // ""Code"": ""K0098*FDG2345/01"",
            // ""TradeAccountCode"": ""K0098"",
            // ""ProductCode"": ""FDG2345/01"",
            // ""SortType"": ""Q"",
            // ""Quality"": null,
            // ""PromotionCode"": ""DG13"",
            // ""BrandCode"": null,
            // ""ProductGroupCode"": null,
            // ""ItemTypeCode"": null,
            // ""StartDTs"": [""2024/01/26""],
            // ""ExpiryDTs"": [""2024/04/26""],
            // ""ContractDiscounts"": [10.00],
            // ""LastAmendedDTs"": [""2024/01/26 17:07:00""],
            // ""QuantitiesRequired"": [0]
            //}");


            //var result = ContractDiscountFunctions.Delete(uniVerseAccount, "K0098", null);


            //var results = ContractPriceFunctions.Upsert(uniVerseAccount, @"{
            // ""Code"": ""K0098*FDG2345/01"",
            // ""TradeAccountCode"": ""K0098"",
            // ""ProductCode"": ""FDG2345/01"",
            // ""SortType"": ""Q"",
            // ""Quality"": null,
            // ""PromotionCode"": ""DG13"",
            // ""BrandCode"": null,
            // ""ProductGroupCode"": null,
            // ""ItemTypeCode"": null,
            // ""StartDTs"": [""2024/01/26""],
            // ""ExpiryDTs"": [""2024/04/26""],
            // ""ContractDiscounts"": [10.00],
            // ""LastAmendedDTs"": [""2024/01/26 17:07:00""],
            // ""QuantitiesRequired"": [0]
            //}");

            //var result = ContractDiscountFunctions.Upsert(uniVerseAccount, @"[{""Code"":""PH03039*BAGDG0115"",""TradeAccountCode"":""PH03039"",""SortType"":""P"",""ProductCode"":""BAGDG0115"",""StartDTs"":[""2024/03/14""],""ExpiryDTs"":[""2024/04/08""],""ContractDiscounts"":[20.00],""LastAmendedDTs"":[""2024/03/11 12:00:00""],""QuantitiesRequired"":[0]},{""Code"":""PH03039*BLDG0137"",""TradeAccountCode"":""PH03039"",""SortType"":""P"",""ProductCode"":""BLDG0137"",""StartDTs"":[""2024/03/14""],""ExpiryDTs"":[""2024/04/08""],""ContractDiscounts"":[20.00],""LastAmendedDTs"":[""2024/03/11 12:00:00""],""QuantitiesRequired"":[0]},{""Code"":""PH03039*BLDG0156"",""TradeAccountCode"":""PH03039"",""SortType"":""P"",""ProductCode"":""BLDG0156"",""StartDTs"":[""2024/03/14""],""ExpiryDTs"":[""2024/04/08""],""ContractDiscounts"":[20.00],""LastAmendedDTs"":[""2024/03/11 12:00:00""],""QuantitiesRequired"":[0]},{""Code"":""PH03039*BLDG0157"",""TradeAccountCode"":""PH03039"",""SortType"":""P"",""ProductCode"":""BLDG0157"",""StartDTs"":[""2024/03/14""],""ExpiryDTs"":[""2024/04/08""],""ContractDiscounts"":[20.00],""LastAmendedDTs"":[""2024/03/11 12:00:00""],""QuantitiesRequired"":[0]},{""Code"":""PH03039*BLDG0159"",""TradeAccountCode"":""PH03039"",""SortType"":""P"",""ProductCode"":""BLDG0159"",""StartDTs"":[""2024/03/14""],""ExpiryDTs"":[""2024/04/08""],""ContractDiscounts"":[20.00],""LastAmendedDTs"":[""2024/03/11 12:00:00""],""QuantitiesRequired"":[0]},{""Code"":""PH03039*BLDG0249"",""TradeAccountCode"":""PH03039"",""SortType"":""P"",""ProductCode"":""BLDG0249"",""StartDTs"":[""2024/03/14""],""ExpiryDTs"":[""2024/04/08""],""ContractDiscounts"":[20.00],""LastAmendedDTs"":[""2024/03/11 12:00:00""],""QuantitiesRequired"":[0]},{""Code"":""PH03039*BLDG0253"",""TradeAccountCode"":""PH03039"",""SortType"":""P"",""ProductCode"":""BLDG0253"",""StartDTs"":[""2024/03/14""],""ExpiryDTs"":[""2024/04/08""],""ContractDiscounts"":[20.00],""LastAmendedDTs"":[""2024/03/11 12:00:00""],""QuantitiesRequired"":[0]},{""Code"":""PH03039*BLDG0254"",""TradeAccountCode"":""PH03039"",""SortType"":""P"",""ProductCode"":""BLDG0254"",""StartDTs"":[""2024/03/14""],""ExpiryDTs"":[""2024/04/08""],""ContractDiscounts"":[20.00],""LastAmendedDTs"":[""2024/03/11 12:00:00""],""QuantitiesRequired"":[0]},{""Code"":""PH03039*BLDG0256"",""TradeAccountCode"":""PH03039"",""SortType"":""P"",""ProductCode"":""BLDG0256"",""StartDTs"":[""2024/03/14""],""ExpiryDTs"":[""2024/04/08""],""ContractDiscounts"":[20.00],""LastAmendedDTs"":[""2024/03/11 12:00:00""],""QuantitiesRequired"":[0]},{""Code"":""PH03039*BLDG0258"",""TradeAccountCode"":""PH03039"",""SortType"":""P"",""ProductCode"":""BLDG0258"",""StartDTs"":[""2024/03/14""],""ExpiryDTs"":[""2024/04/08""],""ContractDiscounts"":[20.00],""LastAmendedDTs"":[""2024/03/11 12:00:00""],""QuantitiesRequired"":[0]},{""Code"":""PH03039*BLDG0268"",""TradeAccountCode"":""PH03039"",""SortType"":""P"",""ProductCode"":""BLDG0268"",""StartDTs"":[""2024/03/14""],""ExpiryDTs"":[""2024/04/08""],""ContractDiscounts"":[20.00],""LastAmendedDTs"":[""2024/03/11 12:00:00""],""QuantitiesRequired"":[0]},{""Code"":""PH03039*BLDG0270"",""TradeAccountCode"":""PH03039"",""SortType"":""P"",""ProductCode"":""BLDG0270"",""StartDTs"":[""2024/03/14""],""ExpiryDTs"":[""2024/04/08""],""ContractDiscounts"":[20.00],""LastAmendedDTs"":[""2024/03/11 12:00:00""],""QuantitiesRequired"":[0]},{""Code"":""PH03039*BLDG0272"",""TradeAccountCode"":""PH03039"",""SortType"":""P"",""ProductCode"":""BLDG0272"",""StartDTs"":[""2024/03/14""],""ExpiryDTs"":[""2024/04/08""],""ContractDiscounts"":[20.00],""LastAmendedDTs"":[""2024/03/11 12:00:00""],""QuantitiesRequired"":[0]},{""Code"":""PH03039*BLDG0273"",""TradeAccountCode"":""PH03039"",""SortType"":""P"",""ProductCode"":""BLDG0273"",""StartDTs"":[""2024/03/14""],""ExpiryDTs"":[""2024/04/08""],""ContractDiscounts"":[20.00],""LastAmendedDTs"":[""2024/03/11 12:00:00""],""QuantitiesRequired"":[0]},{""Code"":""PH03039*BLDG0276"",""TradeAccountCode"":""PH03039"",""SortType"":""P"",""ProductCode"":""BLDG0276"",""StartDTs"":[""2024/03/14""],""ExpiryDTs"":[""2024/04/08""],""ContractDiscounts"":[20.00],""LastAmendedDTs"":[""2024/03/11 12:00:00""],""QuantitiesRequired"":[0]},{""Code"":""PH03039*BLDG0277"",""TradeAccountCode"":""PH03039"",""SortType"":""P"",""ProductCode"":""BLDG0277"",""StartDTs"":[""2024/03/14""],""ExpiryDTs"":[""2024/04/08""],""ContractDiscounts"":[20.00],""LastAmendedDTs"":[""2024/03/11 12:00:00""],""QuantitiesRequired"":[0]}]");

            //Tests.Stock.StockTestSuite.Run(uniVerseAccount);


            //using (var qe = new QueryExecutor())
            //{
            //    var result = qe.Run("SQL_CONNECTION", "KEY = \"1\"", "SQL.CONNECTION");

            //    Console.WriteLine(result);
            //}

            //using (var ce = new CommandExecutor("MERTEX_INC"))
            //{
            //    var result = ce.Arbitrary("LIST SLCUST WITH MARKTYPE = U BY CUSREF");

            //    System.IO.File.WriteAllText("MERTEX_INC-U.csv", result);
            //}



            //EDAReplacementTestSuite.TOELINESearchByOrderNumbersTest("MERTEX", TesterFramework.FetchOrderHeadIDs(10));

            //            var toelineResults = EDAReplacementTestSuite.TOELINESearchByOrderNumbersTest("MERTEX", @"[{ ""OrderID"": ""4722124"" },
            //{ ""OrderID"": ""4715953"" },
            //{ ""OrderID"": ""4714655"" },
            //{ ""OrderID"": ""4714613"" },
            //{ ""OrderID"": ""4712492"" },
            //{ ""OrderID"": ""4710839"" },
            //{ ""OrderID"": ""4706903"" },
            //{ ""OrderID"": ""1140263001"" },
            //{ ""OrderID"": ""4697012"" },
            //{ ""OrderID"": ""4684166"" },
            //{ ""OrderID"": ""4683508"" },
            //{ ""OrderID"": ""4683282"" },
            //{ ""OrderID"": ""4681161"" },
            //{ ""OrderID"": ""1102698501"" },
            //{ ""OrderID"": ""1102697901"" },
            //{ ""OrderID"": ""1102698201"" },
            //{ ""OrderID"": ""1102698101"" },
            //{ ""OrderID"": ""1102697801"" },
            //{ ""OrderID"": ""1102698001"" },
            //{ ""OrderID"": ""1102698301"" },
            //{ ""OrderID"": ""1102698401"" },
            //{ ""OrderID"": ""1102697601"" },
            //{ ""OrderID"": ""4674697"" },
            //{ ""OrderID"": ""4669819"" },
            //{ ""OrderID"": ""4621816"" },
            //{ ""OrderID"": ""4625327"" },
            //{ ""OrderID"": ""4625332"" },
            //{ ""OrderID"": ""4631570"" },
            //{ ""OrderID"": ""4631574"" },
            //{ ""OrderID"": ""4631577"" },
            //{ ""OrderID"": ""4631799"" },
            //{ ""OrderID"": ""4631929"" },
            //{ ""OrderID"": ""4632745"" }]");

            //var results = SupplierPaymentTermsFunctions.Select("MERTEX", null).OfType<SupplierPaymentTerms>().ToList();

            //Console.WriteLine(JsonConvert.SerializeObject(results));


            //var results = SalesLedgerTermsFunctions.Select("MERTEX", null).OfType<SalesLedgerTerms>().ToList();

            //Console.WriteLine(JsonConvert.SerializeObject(results));

            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine();

            //var codes = new List<SalesLedgerTransactionCode>() 
            //{ 
            //    new SalesLedgerTransactionCode()
            //    {
            //        Code = "SI5406614"
            //    }
            //};

            //var transactionResults = SalesLedgerTransactionFunctions.Select("MERTEX", null, null, null, null, null, null, null, null).OfType<SalesLedgerTransaction>().ToList();

            //Console.WriteLine($"Saw {transactionResults.Count} TSLTRANS records.");

            //B2BOrderImport.SiphonOldTradeSiteOrders("MERTEX");

            //var toelineResults = EDAReplacementTestSuite.TOELINESearchByOrderNumbersTest("MERTEX", @"[{ ""OrderID"": ""1158027901"" }]");

            //var accounts = TradeAccountFunctions.Select("MERTEX", null).OfType<TradeAccount>();

            //var offenders = accounts.Where(a =>
            //    a.InvoiceAddressPostcode?.Length > 20 ||
            //    a.DefaultDeliveryReference?.Length > 20 ||
            //    a.Bankrupt?.Length > 20 ||
            //    a.StatementAddressPostcode?.Length > 20 ||
            //    a.UPSInvoiceAccountNumber?.Length > 20 ||
            //    a.UPSTaxesDutyAccountNumber?.Length > 20 ||
            //    a.DHLAccountNumber?.Length > 20 ||
            //    a.TNTAccountNumber?.Length > 20 ||
            //    a.DirectToCustomer?.Length > 20 ||
            //    a.PartnerCompany?.Length > 20 ||
            //    a.UserID?.Length > 20 ||
            //    a.StoreName?.Length > 20 ||
            //    a.TransferAddress?.Length > 20 ||
            //    a.TransferPassword?.Length > 20
            //).ToList();

            //var sb = new StringBuilder();

            //offenders.ForEach(o => { Console.WriteLine(o); sb.AppendLine(o.ToString()); });

            //System.IO.File.WriteAllText("offenders.txt", sb.ToString());

            //var toelineResults = EDAReplacementTestSuite.TOELINESearchByOrderNumbersTest("MERTEX", @"[{ ""OrderID"": ""4312243"" }]");

            //var result = ContractPriceFunctions.Delete(uniVerseAccount, "K0098", null);


            //int result = 0;

            //MertexerV2.SQLFunctions.Order.Submit(1140305001, "portal.Mertex_Order_Select", out result);

            //var headresults = EDAReplacementTestSuite.FetchDespatchNoteHeads(uniVerseAccount, "[{ DespatchHeadID: \"4701346-1\" }]");

            //var lineresults = EDAReplacementTestSuite.FetchDespatchNoteLines(uniVerseAccount, @"[{ 
            //    DespatchLineID: ""1145660302-2*1"", 
            //    HaulierCode: ""UWE""
            //}, {
            //    DespatchLineID: ""1145660302-2*2"", 
            //    HaulierCode: ""UWE""
            //}, {
            //    DespatchLineID: ""1145660302-2*3"", 
            //    HaulierCode: ""UWE""
            //}, {
            //    DespatchLineID: ""1145660302-2*4"", 
            //    HaulierCode: ""UWE""
            //}, {
            //    DespatchLineID: ""1145660302-2*5"", 
            //    HaulierCode: ""UWE""
            //}, {
            //    DespatchLineID: ""1145660302-2*6"", 
            //    HaulierCode: ""UWE""
            //}, {
            //    DespatchLineID: ""1145660302-3*1"", 
            //    HaulierCode: ""UWE""
            //}]");



            //var operators = OperatorFunctions.Select(uniVerseAccount, null).OfType<Operator>().ToList();

            //operators.ForEach(op => Console.WriteLine(op));

            //Console.WriteLine($"Saw {operators.Count()} results in {stopwatch.Elapsed}");

            //var dfi = System.Globalization.DateTimeFormatInfo.InvariantInfo;

            //var oesanalLines = SalesAnalysisTestSuite.HistorySelect(uniVerseAccount, @"[{ ""SalesAnalysisID"": ""922969"" }]", null, null);

            //var oesanalLines = SalesAnalysisTestSuite.HistorySelect(uniVerseAccount, null, DateTime.ParseExact("2011/01/01", "yyyy/MM/dd", dfi), DateTime.ParseExact("2011/03/01", "yyyy/MM/dd", dfi));

            //System.IO.File.WriteAllText("oesanal-history.json", JsonConvert.SerializeObject(oesanalLines));


            //var sb = new StringBuilder();

            //using (var sw = new StreamWriter("oesanal-history.csv", true))
            //{
            //    var t = typeof(SalesAnalysisHistory);

            //    var properties = t.GetProperties();

            //    var data = new List<object>();

            //    foreach (var f in properties)
            //    {
            //        data.Add(f.Name);
            //    }

            //    sw.WriteLine(String.Join(",", data));

            //    foreach (var ol in oesanalLines)
            //    {
            //        sw.WriteLine(ol.ToString());
            //    }
            //}

            //Console.WriteLine($"Saw {oesanalLines.Count()} history results in {stopwatch.Elapsed}");


            //var suppliers = SupplierFunctions.Select(uniVerseAccount, null).OfType<MertexerV2.Models.Product.Supplier>().ToList();

            //suppliers.ForEach(op => Console.WriteLine(op));

            //Console.WriteLine($"Saw {suppliers.Count()} results in {stopwatch.Elapsed}");


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

            //var results = Stock.BatchPiece("PDG718/01", "EMB", "S", 1).OfType<BatchPieceResult>().ToList();

            //results.ForEach(r =>
            //{

            //    SqlString Type = null;
            //    SqlString ProductCode = null;
            //    SqlDecimal StockCheckMinimum = 0.0M;
            //    SqlString LocationCode = null;
            //    SqlString Batch = null;
            //    SqlString Piece = null;
            //    SqlBoolean Pickable = false;
            //    SqlString Rack = null;
            //    SqlString RackType = null;
            //    SqlDecimal Quantity = 0.0M;
            //    SqlDecimal OnReserve = 0.0M;
            //    SqlDateTime PODate = DateTime.Now;
            //    SqlString POMessage = null;
            //    SqlBoolean POStatus = false;
            //    SqlBoolean CallOff = false;

            //    Stock.BatchPiece_FillRow(r, out Type, out ProductCode, out StockCheckMinimum, out LocationCode,
            //        out Batch,
            //        out Piece,
            //        out Pickable,
            //        out Rack,
            //        out RackType,
            //        out Quantity,
            //        out OnReserve,
            //        out PODate,
            //        out POMessage,
            //        out POStatus,
            //        out CallOff);
            //});

            //var despatchLines = MertexerV2.EDAReplacement.EDAReplacement.FetchDespatchNoteLines(uniVerseAccount, $@"[{{
            //    ""DespatchLineID"": ""628499*3"",
            //    ""HaulierCode"":""UPS""
            //}},{{
            //    ""DespatchLineID"": ""628499*4"",
            //    ""HaulierCode"":""UPS""
            //}},{{
            //    ""DespatchLineID"": ""628499*1"",
            //    ""HaulierCode"":""UPS""
            //}},{{
            //    ""DespatchLineID"": ""628499*2"",
            //    ""HaulierCode"":""UPS""
            //}},{{
            //    ""DespatchLineID"": ""628499*5"",
            //    ""HaulierCode"":""UPS""
            //}}]").OfType<OEDNLine>().ToList();

            //foreach (var dl in despatchLines)
            //{
            //    Console.WriteLine(dl);
            //}

            //Stock.BatchPiece("F1723/57", "WH", "S");

            //Console.ForegroundColor = ConsoleColor.Magenta;

            ////Item("SLCUST", "PH03154");

            //Console.ForegroundColor = ConsoleColor.Cyan;

            //Item("SLCUST", "PH03155");


            //var toelineResults = EDAReplacementTestSuite.TOELINESearchByOrderNumbersTest("MERTEX", @"[{ ""OrderID"": ""1148767201"" }]", null, null, null).OfType<TOELine>();

            //Item("DMPROCESS", "RF.PDF.MANAGER", true);


            //var supplier = Selector<MertexerV2.Models.Supplier.Supplier>.Select("L0150", true, true, 1, uniVerseAccount);

            //Console.WriteLine(supplier);

            //var results = EDAReplacementTestSuite.PurchaseOrderHeadSearch(uniVerseAccount, @"[{""PurchaseOrderHeadID"":""P265482""},{""PurchaseOrderHeadID"":""P265483""},{""PurchaseOrderHeadID"":""P265484""},{""PurchaseOrderHeadID"":""P265485""},{""PurchaseOrderHeadID"":""P265486""},{""PurchaseOrderHeadID"":""P265487""},{""PurchaseOrderHeadID"":""P265488""},{""PurchaseOrderHeadID"":""P265489""},{""PurchaseOrderHeadID"":""P265490""},{""PurchaseOrderHeadID"":""P265491""},{""PurchaseOrderHeadID"":""P265492""},{""PurchaseOrderHeadID"":""P265493""},{""PurchaseOrderHeadID"":""P265494""},{""PurchaseOrderHeadID"":""P265495""},{""PurchaseOrderHeadID"":""P265496""},{""PurchaseOrderHeadID"":""P265497""},{""PurchaseOrderHeadID"":""P265498""},{""PurchaseOrderHeadID"":""P265499""},{""PurchaseOrderHeadID"":""P265500""},{""PurchaseOrderHeadID"":""P265501""},{""PurchaseOrderHeadID"":""P265502""},{""PurchaseOrderHeadID"":""P265503""},{""PurchaseOrderHeadID"":""P265504""},{""PurchaseOrderHeadID"":""P265505""},{""PurchaseOrderHeadID"":""P265506""},{""PurchaseOrderHeadID"":""P265507""},{""PurchaseOrderHeadID"":""P265508""},{""PurchaseOrderHeadID"":""P265509""},{""PurchaseOrderHeadID"":""P265510""},{""PurchaseOrderHeadID"":""P265511""},{""PurchaseOrderHeadID"":""P265512""},{""PurchaseOrderHeadID"":""P265513""},{""PurchaseOrderHeadID"":""P265514""},{""PurchaseOrderHeadID"":""P265515""},{""PurchaseOrderHeadID"":""P265516""},{""PurchaseOrderHeadID"":""P265517""},{""PurchaseOrderHeadID"":""P265518""},{""PurchaseOrderHeadID"":""P265519""},{""PurchaseOrderHeadID"":""P265520""},{""PurchaseOrderHeadID"":""P265521""},{""PurchaseOrderHeadID"":""P265522""},{""PurchaseOrderHeadID"":""P265523""},{""PurchaseOrderHeadID"":""P265524""},{""PurchaseOrderHeadID"":""P265525""},{""PurchaseOrderHeadID"":""P265526""},{""PurchaseOrderHeadID"":""P265527""},{""PurchaseOrderHeadID"":""P265528""},{""PurchaseOrderHeadID"":""P265529""},{""PurchaseOrderHeadID"":""P265530""},{""PurchaseOrderHeadID"":""P265531""},{""PurchaseOrderHeadID"":""P265532""},{""PurchaseOrderHeadID"":""P265533""},{""PurchaseOrderHeadID"":""P265534""},{""PurchaseOrderHeadID"":""P265535""},{""PurchaseOrderHeadID"":""P265536""},{""PurchaseOrderHeadID"":""P265537""},{""PurchaseOrderHeadID"":""P265538""},{""PurchaseOrderHeadID"":""P265539""},{""PurchaseOrderHeadID"":""P265540""},{""PurchaseOrderHeadID"":""P265541""},{""PurchaseOrderHeadID"":""P265542""},{""PurchaseOrderHeadID"":""P265543""},{""PurchaseOrderHeadID"":""P265544""},{""PurchaseOrderHeadID"":""P265545""},{""PurchaseOrderHeadID"":""P265546""},{""PurchaseOrderHeadID"":""P265547""},{""PurchaseOrderHeadID"":""P265548""},{""PurchaseOrderHeadID"":""P265549""},{""PurchaseOrderHeadID"":""P265550""},{""PurchaseOrderHeadID"":""P265551""},{""PurchaseOrderHeadID"":""P265552""},{""PurchaseOrderHeadID"":""P265553""},{""PurchaseOrderHeadID"":""P265554""},{""PurchaseOrderHeadID"":""P265555""},{""PurchaseOrderHeadID"":""P265556""},{""PurchaseOrderHeadID"":""P265557""},{""PurchaseOrderHeadID"":""P265558""},{""PurchaseOrderHeadID"":""P265559""},{""PurchaseOrderHeadID"":""P265560""},{""PurchaseOrderHeadID"":""P265561""},{""PurchaseOrderHeadID"":""P265562""},{""PurchaseOrderHeadID"":""P265563""},{""PurchaseOrderHeadID"":""P265564""},{""PurchaseOrderHeadID"":""P265565""},{""PurchaseOrderHeadID"":""P265566""},{""PurchaseOrderHeadID"":""P265567""},{""PurchaseOrderHeadID"":""P265568""},{""PurchaseOrderHeadID"":""P265569""},{""PurchaseOrderHeadID"":""P265570""},{""PurchaseOrderHeadID"":""P265571""},{""PurchaseOrderHeadID"":""P265572""},{""PurchaseOrderHeadID"":""P265573""},{""PurchaseOrderHeadID"":""P265574""},{""PurchaseOrderHeadID"":""P265575""},{""PurchaseOrderHeadID"":""P265576""},{""PurchaseOrderHeadID"":""P265577""},{""PurchaseOrderHeadID"":""P265578""},{""PurchaseOrderHeadID"":""P265579""},{""PurchaseOrderHeadID"":""P265580""},{""PurchaseOrderHeadID"":""P265581""},{""PurchaseOrderHeadID"":""P265582""},{""PurchaseOrderHeadID"":""P265583""},{""PurchaseOrderHeadID"":""P265584""},{""PurchaseOrderHeadID"":""P265585""},{""PurchaseOrderHeadID"":""P265586""},{""PurchaseOrderHeadID"":""P265587""},{""PurchaseOrderHeadID"":""P265588""},{""PurchaseOrderHeadID"":""P265589""},{""PurchaseOrderHeadID"":""P265590""},{""PurchaseOrderHeadID"":""P265591""},{""PurchaseOrderHeadID"":""P265592""},{""PurchaseOrderHeadID"":""P265593""},{""PurchaseOrderHeadID"":""P265594""},{""PurchaseOrderHeadID"":""P265595""},{""PurchaseOrderHeadID"":""P265596""},{""PurchaseOrderHeadID"":""P265597""},{""PurchaseOrderHeadID"":""P265598""},{""PurchaseOrderHeadID"":""P265599""},{""PurchaseOrderHeadID"":""P265600""},{""PurchaseOrderHeadID"":""P265601""},{""PurchaseOrderHeadID"":""P265602""},{""PurchaseOrderHeadID"":""P265603""},{""PurchaseOrderHeadID"":""P265604""},{""PurchaseOrderHeadID"":""P265605""},{""PurchaseOrderHeadID"":""P265606""},{""PurchaseOrderHeadID"":""P265607""},{""PurchaseOrderHeadID"":""P265608""},{""PurchaseOrderHeadID"":""P265609""},{""PurchaseOrderHeadID"":""P265610""},{""PurchaseOrderHeadID"":""P265611""},{""PurchaseOrderHeadID"":""P265612""},{""PurchaseOrderHeadID"":""P265613""},{""PurchaseOrderHeadID"":""P265614""},{""PurchaseOrderHeadID"":""P265615""},{""PurchaseOrderHeadID"":""P265616""},{""PurchaseOrderHeadID"":""P265617""},{""PurchaseOrderHeadID"":""P265618""},{""PurchaseOrderHeadID"":""P265619""},{""PurchaseOrderHeadID"":""P265620""},{""PurchaseOrderHeadID"":""P265621""},{""PurchaseOrderHeadID"":""P265622""},{""PurchaseOrderHeadID"":""P265623""},{""PurchaseOrderHeadID"":""P265624""},{""PurchaseOrderHeadID"":""P265625""},{""PurchaseOrderHeadID"":""P265626""},{""PurchaseOrderHeadID"":""P265627""},{""PurchaseOrderHeadID"":""P265628""},{""PurchaseOrderHeadID"":""P265629""},{""PurchaseOrderHeadID"":""P265630""},{""PurchaseOrderHeadID"":""P265631""},{""PurchaseOrderHeadID"":""P265632""},{""PurchaseOrderHeadID"":""P265633""},{""PurchaseOrderHeadID"":""P265634""},{""PurchaseOrderHeadID"":""P265635""},{""PurchaseOrderHeadID"":""P265636""},{""PurchaseOrderHeadID"":""P265637""},{""PurchaseOrderHeadID"":""P265638""},{""PurchaseOrderHeadID"":""P265639""},{""PurchaseOrderHeadID"":""P265640""},{""PurchaseOrderHeadID"":""P265641""},{""PurchaseOrderHeadID"":""P265642""},{""PurchaseOrderHeadID"":""P265643""},{""PurchaseOrderHeadID"":""P265644""},{""PurchaseOrderHeadID"":""P265645""},{""PurchaseOrderHeadID"":""P265646""},{""PurchaseOrderHeadID"":""P265647""},{""PurchaseOrderHeadID"":""P265648""},{""PurchaseOrderHeadID"":""P265649""},{""PurchaseOrderHeadID"":""P265650""},{""PurchaseOrderHeadID"":""P265651""},{""PurchaseOrderHeadID"":""P265652""},{""PurchaseOrderHeadID"":""P265653""},{""PurchaseOrderHeadID"":""P265654""},{""PurchaseOrderHeadID"":""P265655""},{""PurchaseOrderHeadID"":""P265656""},{""PurchaseOrderHeadID"":""P265657""},{""PurchaseOrderHeadID"":""P265658""},{""PurchaseOrderHeadID"":""P265659""},{""PurchaseOrderHeadID"":""P265660""},{""PurchaseOrderHeadID"":""P265661""},{""PurchaseOrderHeadID"":""P265662""},{""PurchaseOrderHeadID"":""P265663""},{""PurchaseOrderHeadID"":""P265664""},{""PurchaseOrderHeadID"":""P265665""},{""PurchaseOrderHeadID"":""P265666""},{""PurchaseOrderHeadID"":""P265667""},{""PurchaseOrderHeadID"":""P265668""},{""PurchaseOrderHeadID"":""P265669""},{""PurchaseOrderHeadID"":""P26567""},{""PurchaseOrderHeadID"":""P265670""},{""PurchaseOrderHeadID"":""P265671""},{""PurchaseOrderHeadID"":""P265672""},{""PurchaseOrderHeadID"":""P265673""},{""PurchaseOrderHeadID"":""P265674""},{""PurchaseOrderHeadID"":""P265675""},{""PurchaseOrderHeadID"":""P265676""},{""PurchaseOrderHeadID"":""P265677""},{""PurchaseOrderHeadID"":""P265678""},{""PurchaseOrderHeadID"":""P265679""},{""PurchaseOrderHeadID"":""P265680""},{""PurchaseOrderHeadID"":""P265681""},{""PurchaseOrderHeadID"":""P265682""},{""PurchaseOrderHeadID"":""P265683""},{""PurchaseOrderHeadID"":""P265684""},{""PurchaseOrderHeadID"":""P265685""},{""PurchaseOrderHeadID"":""P265686""},{""PurchaseOrderHeadID"":""P265687""},{""PurchaseOrderHeadID"":""P265688""},{""PurchaseOrderHeadID"":""P265689""},{""PurchaseOrderHeadID"":""P265690""},{""PurchaseOrderHeadID"":""P265691""},{""PurchaseOrderHeadID"":""P265692""},{""PurchaseOrderHeadID"":""P265693""},{""PurchaseOrderHeadID"":""P265694""},{""PurchaseOrderHeadID"":""P265695""},{""PurchaseOrderHeadID"":""P265696""},{""PurchaseOrderHeadID"":""P265697""},{""PurchaseOrderHeadID"":""P265698""},{""PurchaseOrderHeadID"":""P265699""},{""PurchaseOrderHeadID"":""P265700""},{""PurchaseOrderHeadID"":""P265701""},{""PurchaseOrderHeadID"":""P265702""},{""PurchaseOrderHeadID"":""P265703""},{""PurchaseOrderHeadID"":""P265704""},{""PurchaseOrderHeadID"":""P265705""},{""PurchaseOrderHeadID"":""P265706""},{""PurchaseOrderHeadID"":""P265707""},{""PurchaseOrderHeadID"":""P265708""},{""PurchaseOrderHeadID"":""P265709""},{""PurchaseOrderHeadID"":""P265710""},{""PurchaseOrderHeadID"":""P265711""},{""PurchaseOrderHeadID"":""P265712""},{""PurchaseOrderHeadID"":""P265713""},{""PurchaseOrderHeadID"":""P265714""},{""PurchaseOrderHeadID"":""P265715""},{""PurchaseOrderHeadID"":""P265716""},{""PurchaseOrderHeadID"":""P265717""},{""PurchaseOrderHeadID"":""P265718""},{""PurchaseOrderHeadID"":""P265719""},{""PurchaseOrderHeadID"":""P265720""},{""PurchaseOrderHeadID"":""P265721""},{""PurchaseOrderHeadID"":""P265722""},{""PurchaseOrderHeadID"":""P265723""},{""PurchaseOrderHeadID"":""P265724""},{""PurchaseOrderHeadID"":""P265725""},{""PurchaseOrderHeadID"":""P265726""},{""PurchaseOrderHeadID"":""P265727""},{""PurchaseOrderHeadID"":""P265728""},{""PurchaseOrderHeadID"":""P265729""},{""PurchaseOrderHeadID"":""P265730""},{""PurchaseOrderHeadID"":""P265731""},{""PurchaseOrderHeadID"":""P265732""},{""PurchaseOrderHeadID"":""P265733""},{""PurchaseOrderHeadID"":""P265734""},{""PurchaseOrderHeadID"":""P265735""},{""PurchaseOrderHeadID"":""P265736""},{""PurchaseOrderHeadID"":""P265737""},{""PurchaseOrderHeadID"":""P265738""},{""PurchaseOrderHeadID"":""P265739""},{""PurchaseOrderHeadID"":""P265740""},{""PurchaseOrderHeadID"":""P265741""},{""PurchaseOrderHeadID"":""P265742""},{""PurchaseOrderHeadID"":""P265743""},{""PurchaseOrderHeadID"":""P265744""},{""PurchaseOrderHeadID"":""P265745""},{""PurchaseOrderHeadID"":""P265746""},{""PurchaseOrderHeadID"":""P265747""},{""PurchaseOrderHeadID"":""P265748""},{""PurchaseOrderHeadID"":""P265749""},{""PurchaseOrderHeadID"":""P265750""},{""PurchaseOrderHeadID"":""P265751""},{""PurchaseOrderHeadID"":""P265752""},{""PurchaseOrderHeadID"":""P265753""},{""PurchaseOrderHeadID"":""P265754""},{""PurchaseOrderHeadID"":""P265755""},{""PurchaseOrderHeadID"":""P265756""},{""PurchaseOrderHeadID"":""P265757""},{""PurchaseOrderHeadID"":""P265758""},{""PurchaseOrderHeadID"":""P265759""},{""PurchaseOrderHeadID"":""P265760""},{""PurchaseOrderHeadID"":""P265761""},{""PurchaseOrderHeadID"":""P265762""},{""PurchaseOrderHeadID"":""P265763""},{""PurchaseOrderHeadID"":""P265764""},{""PurchaseOrderHeadID"":""P265765""},{""PurchaseOrderHeadID"":""P265766""},{""PurchaseOrderHeadID"":""P265767""},{""PurchaseOrderHeadID"":""P265768""},{""PurchaseOrderHeadID"":""P265769""},{""PurchaseOrderHeadID"":""P265770""},{""PurchaseOrderHeadID"":""P265771""},{""PurchaseOrderHeadID"":""P265772""},{""PurchaseOrderHeadID"":""P265773""},{""PurchaseOrderHeadID"":""P265774""},{""PurchaseOrderHeadID"":""P265775""},{""PurchaseOrderHeadID"":""P265776""},{""PurchaseOrderHeadID"":""P265777""},{""PurchaseOrderHeadID"":""P265778""},{""PurchaseOrderHeadID"":""P265779""},{""PurchaseOrderHeadID"":""P265780""},{""PurchaseOrderHeadID"":""P265781""},{""PurchaseOrderHeadID"":""P265804""},{""PurchaseOrderHeadID"":""P265807""},{""PurchaseOrderHeadID"":""P265808""},{""PurchaseOrderHeadID"":""P265810""},{""PurchaseOrderHeadID"":""P265811""},{""PurchaseOrderHeadID"":""P265812""},{""PurchaseOrderHeadID"":""P265813""},{""PurchaseOrderHeadID"":""P265814""},{""PurchaseOrderHeadID"":""P265815""},{""PurchaseOrderHeadID"":""P265816""},{""PurchaseOrderHeadID"":""P265817""},{""PurchaseOrderHeadID"":""P265818""},{""PurchaseOrderHeadID"":""P265819""},{""PurchaseOrderHeadID"":""P265820""},{""PurchaseOrderHeadID"":""P265821""},{""PurchaseOrderHeadID"":""P265822""},{""PurchaseOrderHeadID"":""P265823""},{""PurchaseOrderHeadID"":""P265824""},{""PurchaseOrderHeadID"":""P265825""},{""PurchaseOrderHeadID"":""P265826""},{""PurchaseOrderHeadID"":""P265828""},{""PurchaseOrderHeadID"":""P265831""},{""PurchaseOrderHeadID"":""P265832""},{""PurchaseOrderHeadID"":""P265838""},{""PurchaseOrderHeadID"":""P265839""},{""PurchaseOrderHeadID"":""P265841""},{""PurchaseOrderHeadID"":""P265843""},{""PurchaseOrderHeadID"":""P265853""},{""PurchaseOrderHeadID"":""P265874""},{""PurchaseOrderHeadID"":""P265880""},{""PurchaseOrderHeadID"":""P265890""},{""PurchaseOrderHeadID"":""P265898""},{""PurchaseOrderHeadID"":""P265916""},{""PurchaseOrderHeadID"":""P265920""},{""PurchaseOrderHeadID"":""P265952""},{""PurchaseOrderHeadID"":""P265959""},{""PurchaseOrderHeadID"":""P265992""},{""PurchaseOrderHeadID"":""P265993""},{""PurchaseOrderHeadID"":""P265994""},{""PurchaseOrderHeadID"":""P265995""},{""PurchaseOrderHeadID"":""P265996""},{""PurchaseOrderHeadID"":""P265997""},{""PurchaseOrderHeadID"":""P265998""},{""PurchaseOrderHeadID"":""P265999""},{""PurchaseOrderHeadID"":""P266000""},{""PurchaseOrderHeadID"":""P266027""},{""PurchaseOrderHeadID"":""P266030""},{""PurchaseOrderHeadID"":""P266032""},{""PurchaseOrderHeadID"":""P266045""},{""PurchaseOrderHeadID"":""P266046""},{""PurchaseOrderHeadID"":""P266047""},{""PurchaseOrderHeadID"":""P266048""},{""PurchaseOrderHeadID"":""P266088""},{""PurchaseOrderHeadID"":""P266090""},{""PurchaseOrderHeadID"":""P266105""},{""PurchaseOrderHeadID"":""P266113""},{""PurchaseOrderHeadID"":""P266124""},{""PurchaseOrderHeadID"":""P266129""},{""PurchaseOrderHeadID"":""P266130""},{""PurchaseOrderHeadID"":""P266131""},{""PurchaseOrderHeadID"":""P266132""},{""PurchaseOrderHeadID"":""P266133""},{""PurchaseOrderHeadID"":""P266172""},{""PurchaseOrderHeadID"":""P266174""},{""PurchaseOrderHeadID"":""P266196""},{""PurchaseOrderHeadID"":""P266227""},{""PurchaseOrderHeadID"":""P266235""},{""PurchaseOrderHeadID"":""P266236""},{""PurchaseOrderHeadID"":""P266237""},{""PurchaseOrderHeadID"":""P266238""},{""PurchaseOrderHeadID"":""P266239""},{""PurchaseOrderHeadID"":""P266240""},{""PurchaseOrderHeadID"":""P266241""},{""PurchaseOrderHeadID"":""P266242""},{""PurchaseOrderHeadID"":""P266243""},{""PurchaseOrderHeadID"":""P266244""},{""PurchaseOrderHeadID"":""P266245""},{""PurchaseOrderHeadID"":""P266246""},{""PurchaseOrderHeadID"":""P266247""},{""PurchaseOrderHeadID"":""P266274""},{""PurchaseOrderHeadID"":""P266278""},{""PurchaseOrderHeadID"":""P266279""},{""PurchaseOrderHeadID"":""P266286""},{""PurchaseOrderHeadID"":""P266295""},{""PurchaseOrderHeadID"":""P266314""},{""PurchaseOrderHeadID"":""P266342""},{""PurchaseOrderHeadID"":""P266351""},{""PurchaseOrderHeadID"":""P266352""},{""PurchaseOrderHeadID"":""P266378""},{""PurchaseOrderHeadID"":""P266392""},{""PurchaseOrderHeadID"":""P266414""},{""PurchaseOrderHeadID"":""P266442""},{""PurchaseOrderHeadID"":""P266462""},{""PurchaseOrderHeadID"":""P266467""},{""PurchaseOrderHeadID"":""P266469""},{""PurchaseOrderHeadID"":""P266470""},{""PurchaseOrderHeadID"":""P266471""},{""PurchaseOrderHeadID"":""P266473""},{""PurchaseOrderHeadID"":""P266475""},{""PurchaseOrderHeadID"":""P266476""},{""PurchaseOrderHeadID"":""P266479""},{""PurchaseOrderHeadID"":""P266480""},{""PurchaseOrderHeadID"":""P266482""},{""PurchaseOrderHeadID"":""P266484""},{""PurchaseOrderHeadID"":""P266485""},{""PurchaseOrderHeadID"":""P266494""},{""PurchaseOrderHeadID"":""P266495""},{""PurchaseOrderHeadID"":""P266519""},{""PurchaseOrderHeadID"":""P266529""},{""PurchaseOrderHeadID"":""P266552""},{""PurchaseOrderHeadID"":""P266557""}]", null, null);

            //var purchaseLedgerInvoiceRegisterJson = TesterFramework.FetchPurchaseOrderInvoiceRegisterJson();

            //var results = MertexerV2.PurchaseLedgerFunctions.PurchaseLedgerInvoiceRegisterFunctions.Upsert("MERTEX", purchaseLedgerInvoiceRegisterJson).OfType<string>().ToList();

            //var purchaseLedgerCreditRegisterJson = TesterFramework.FetchPurchaseOrderInvoiceRegisterJson();

            //var controllers = CreditControllerFunctions.Select(uniVerseAccount, @"[{ ""Code"": ""02"" }]").OfType<CreditController>().ToList();

            //Console.WriteLine(JsonConvert.SerializeObject(controllers));

            //var results = SalesLedgerTransactionCurrencyFunctions.Select("MERTEX",
            //    //@"[{ ""Code"": ""PI324493"" }]",
            //    null,
            //    null,
            //    null,
            //    null,
            //    null,
            //    null,
            //    null,
            //    null).OfType<SalesLedgerTransactionCurrency>().ToList();

            ////Console.WriteLine(JsonConvert.SerializeObject(results));

            //Console.WriteLine($"{results.Count} TPLTRANS.CUR records read");

            var results = PurchaseLedgerTransactionCurrencyFunctions.Select("MERTEX",
                //@"[{ ""Code"": ""PI324493"" }]",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null).OfType<PurchaseLedgerTransactionCurrency>().ToList();

            //Console.WriteLine(JsonConvert.SerializeObject(results));

            Console.WriteLine($"{results.Count} TPLTRANS.CUR records read");




            //var results = MertexerV2.PurchaseLedgerFunctions.PurchaseLedgerCreditRegisterFunctions.Upsert("MERTEX", purchaseLedgerCreditRegisterJson).OfType<string>().ToList();

            //var resultA = MertexerV2.PurchaseLedgerFunctions.PurchaseLedgerInvoiceRegisterFunctions.Insert(uniVerseAccount, "314260");

            //var resultB = MertexerV2.PurchaseLedgerFunctions.PurchaseLedgerInvoiceRegisterFunctions.Insert(uniVerseAccount, "DELETEME").OfType<string>().ToList();

            //Console.WriteLine($"{resultB.Count} results");

            //foreach (var s in resultB)
            //{
            //    Console.WriteLine(s);
            //}

            //var results = EDAReplacementTestSuite.PurchaseOrderDetailSearch(uniVerseAccount, @"[{""PurchaseOrderHeadID"":""P265482""},{""PurchaseOrderHeadID"":""P265483""},{""PurchaseOrderHeadID"":""P265484""},{""PurchaseOrderHeadID"":""P265485""},{""PurchaseOrderHeadID"":""P265486""},{""PurchaseOrderHeadID"":""P265487""},{""PurchaseOrderHeadID"":""P265488""},{""PurchaseOrderHeadID"":""P265489""},{""PurchaseOrderHeadID"":""P265490""},{""PurchaseOrderHeadID"":""P265491""},{""PurchaseOrderHeadID"":""P265492""},{""PurchaseOrderHeadID"":""P265493""},{""PurchaseOrderHeadID"":""P265494""},{""PurchaseOrderHeadID"":""P265495""},{""PurchaseOrderHeadID"":""P265496""},{""PurchaseOrderHeadID"":""P265497""},{""PurchaseOrderHeadID"":""P265498""},{""PurchaseOrderHeadID"":""P265499""},{""PurchaseOrderHeadID"":""P265500""},{""PurchaseOrderHeadID"":""P265501""},{""PurchaseOrderHeadID"":""P265502""},{""PurchaseOrderHeadID"":""P265503""},{""PurchaseOrderHeadID"":""P265504""},{""PurchaseOrderHeadID"":""P265505""},{""PurchaseOrderHeadID"":""P265506""},{""PurchaseOrderHeadID"":""P265507""},{""PurchaseOrderHeadID"":""P265508""},{""PurchaseOrderHeadID"":""P265509""},{""PurchaseOrderHeadID"":""P265510""},{""PurchaseOrderHeadID"":""P265511""},{""PurchaseOrderHeadID"":""P265512""},{""PurchaseOrderHeadID"":""P265513""},{""PurchaseOrderHeadID"":""P265514""},{""PurchaseOrderHeadID"":""P265515""},{""PurchaseOrderHeadID"":""P265516""},{""PurchaseOrderHeadID"":""P265517""},{""PurchaseOrderHeadID"":""P265518""},{""PurchaseOrderHeadID"":""P265519""},{""PurchaseOrderHeadID"":""P265520""},{""PurchaseOrderHeadID"":""P265521""},{""PurchaseOrderHeadID"":""P265522""},{""PurchaseOrderHeadID"":""P265523""},{""PurchaseOrderHeadID"":""P265524""},{""PurchaseOrderHeadID"":""P265525""},{""PurchaseOrderHeadID"":""P265526""},{""PurchaseOrderHeadID"":""P265527""},{""PurchaseOrderHeadID"":""P265528""},{""PurchaseOrderHeadID"":""P265529""},{""PurchaseOrderHeadID"":""P265530""},{""PurchaseOrderHeadID"":""P265531""},{""PurchaseOrderHeadID"":""P265532""},{""PurchaseOrderHeadID"":""P265533""},{""PurchaseOrderHeadID"":""P265534""},{""PurchaseOrderHeadID"":""P265535""},{""PurchaseOrderHeadID"":""P265536""},{""PurchaseOrderHeadID"":""P265537""},{""PurchaseOrderHeadID"":""P265538""},{""PurchaseOrderHeadID"":""P265539""},{""PurchaseOrderHeadID"":""P265540""},{""PurchaseOrderHeadID"":""P265541""},{""PurchaseOrderHeadID"":""P265542""},{""PurchaseOrderHeadID"":""P265543""},{""PurchaseOrderHeadID"":""P265544""},{""PurchaseOrderHeadID"":""P265545""},{""PurchaseOrderHeadID"":""P265546""},{""PurchaseOrderHeadID"":""P265547""},{""PurchaseOrderHeadID"":""P265548""},{""PurchaseOrderHeadID"":""P265549""},{""PurchaseOrderHeadID"":""P265550""},{""PurchaseOrderHeadID"":""P265551""},{""PurchaseOrderHeadID"":""P265552""},{""PurchaseOrderHeadID"":""P265553""},{""PurchaseOrderHeadID"":""P265554""},{""PurchaseOrderHeadID"":""P265555""},{""PurchaseOrderHeadID"":""P265556""},{""PurchaseOrderHeadID"":""P265557""},{""PurchaseOrderHeadID"":""P265558""},{""PurchaseOrderHeadID"":""P265559""},{""PurchaseOrderHeadID"":""P265560""},{""PurchaseOrderHeadID"":""P265561""},{""PurchaseOrderHeadID"":""P265562""},{""PurchaseOrderHeadID"":""P265563""},{""PurchaseOrderHeadID"":""P265564""},{""PurchaseOrderHeadID"":""P265565""},{""PurchaseOrderHeadID"":""P265566""},{""PurchaseOrderHeadID"":""P265567""},{""PurchaseOrderHeadID"":""P265568""},{""PurchaseOrderHeadID"":""P265569""},{""PurchaseOrderHeadID"":""P265570""},{""PurchaseOrderHeadID"":""P265571""},{""PurchaseOrderHeadID"":""P265572""},{""PurchaseOrderHeadID"":""P265573""},{""PurchaseOrderHeadID"":""P265574""},{""PurchaseOrderHeadID"":""P265575""},{""PurchaseOrderHeadID"":""P265576""},{""PurchaseOrderHeadID"":""P265577""},{""PurchaseOrderHeadID"":""P265578""},{""PurchaseOrderHeadID"":""P265579""},{""PurchaseOrderHeadID"":""P265580""},{""PurchaseOrderHeadID"":""P265581""},{""PurchaseOrderHeadID"":""P265582""},{""PurchaseOrderHeadID"":""P265583""},{""PurchaseOrderHeadID"":""P265584""},{""PurchaseOrderHeadID"":""P265585""},{""PurchaseOrderHeadID"":""P265586""},{""PurchaseOrderHeadID"":""P265587""},{""PurchaseOrderHeadID"":""P265588""},{""PurchaseOrderHeadID"":""P265589""},{""PurchaseOrderHeadID"":""P265590""},{""PurchaseOrderHeadID"":""P265591""},{""PurchaseOrderHeadID"":""P265592""},{""PurchaseOrderHeadID"":""P265593""},{""PurchaseOrderHeadID"":""P265594""},{""PurchaseOrderHeadID"":""P265595""},{""PurchaseOrderHeadID"":""P265596""},{""PurchaseOrderHeadID"":""P265597""},{""PurchaseOrderHeadID"":""P265598""},{""PurchaseOrderHeadID"":""P265599""},{""PurchaseOrderHeadID"":""P265600""},{""PurchaseOrderHeadID"":""P265601""},{""PurchaseOrderHeadID"":""P265602""},{""PurchaseOrderHeadID"":""P265603""},{""PurchaseOrderHeadID"":""P265604""},{""PurchaseOrderHeadID"":""P265605""},{""PurchaseOrderHeadID"":""P265606""},{""PurchaseOrderHeadID"":""P265607""},{""PurchaseOrderHeadID"":""P265608""},{""PurchaseOrderHeadID"":""P265609""},{""PurchaseOrderHeadID"":""P265610""},{""PurchaseOrderHeadID"":""P265611""},{""PurchaseOrderHeadID"":""P265612""},{""PurchaseOrderHeadID"":""P265613""},{""PurchaseOrderHeadID"":""P265614""},{""PurchaseOrderHeadID"":""P265615""},{""PurchaseOrderHeadID"":""P265616""},{""PurchaseOrderHeadID"":""P265617""},{""PurchaseOrderHeadID"":""P265618""},{""PurchaseOrderHeadID"":""P265619""},{""PurchaseOrderHeadID"":""P265620""},{""PurchaseOrderHeadID"":""P265621""},{""PurchaseOrderHeadID"":""P265622""},{""PurchaseOrderHeadID"":""P265623""},{""PurchaseOrderHeadID"":""P265624""},{""PurchaseOrderHeadID"":""P265625""},{""PurchaseOrderHeadID"":""P265626""},{""PurchaseOrderHeadID"":""P265627""},{""PurchaseOrderHeadID"":""P265628""},{""PurchaseOrderHeadID"":""P265629""},{""PurchaseOrderHeadID"":""P265630""},{""PurchaseOrderHeadID"":""P265631""},{""PurchaseOrderHeadID"":""P265632""},{""PurchaseOrderHeadID"":""P265633""},{""PurchaseOrderHeadID"":""P265634""},{""PurchaseOrderHeadID"":""P265635""},{""PurchaseOrderHeadID"":""P265636""},{""PurchaseOrderHeadID"":""P265637""},{""PurchaseOrderHeadID"":""P265638""},{""PurchaseOrderHeadID"":""P265639""},{""PurchaseOrderHeadID"":""P265640""},{""PurchaseOrderHeadID"":""P265641""},{""PurchaseOrderHeadID"":""P265642""},{""PurchaseOrderHeadID"":""P265643""},{""PurchaseOrderHeadID"":""P265644""},{""PurchaseOrderHeadID"":""P265645""},{""PurchaseOrderHeadID"":""P265646""},{""PurchaseOrderHeadID"":""P265647""},{""PurchaseOrderHeadID"":""P265648""},{""PurchaseOrderHeadID"":""P265649""},{""PurchaseOrderHeadID"":""P265650""},{""PurchaseOrderHeadID"":""P265651""},{""PurchaseOrderHeadID"":""P265652""},{""PurchaseOrderHeadID"":""P265653""},{""PurchaseOrderHeadID"":""P265654""},{""PurchaseOrderHeadID"":""P265655""},{""PurchaseOrderHeadID"":""P265656""},{""PurchaseOrderHeadID"":""P265657""},{""PurchaseOrderHeadID"":""P265658""},{""PurchaseOrderHeadID"":""P265659""},{""PurchaseOrderHeadID"":""P265660""},{""PurchaseOrderHeadID"":""P265661""},{""PurchaseOrderHeadID"":""P265662""},{""PurchaseOrderHeadID"":""P265663""},{""PurchaseOrderHeadID"":""P265664""},{""PurchaseOrderHeadID"":""P265665""},{""PurchaseOrderHeadID"":""P265666""},{""PurchaseOrderHeadID"":""P265667""},{""PurchaseOrderHeadID"":""P265668""},{""PurchaseOrderHeadID"":""P265669""},{""PurchaseOrderHeadID"":""P26567""},{""PurchaseOrderHeadID"":""P265670""},{""PurchaseOrderHeadID"":""P265671""},{""PurchaseOrderHeadID"":""P265672""},{""PurchaseOrderHeadID"":""P265673""},{""PurchaseOrderHeadID"":""P265674""},{""PurchaseOrderHeadID"":""P265675""},{""PurchaseOrderHeadID"":""P265676""},{""PurchaseOrderHeadID"":""P265677""},{""PurchaseOrderHeadID"":""P265678""},{""PurchaseOrderHeadID"":""P265679""},{""PurchaseOrderHeadID"":""P265680""},{""PurchaseOrderHeadID"":""P265681""},{""PurchaseOrderHeadID"":""P265682""},{""PurchaseOrderHeadID"":""P265683""},{""PurchaseOrderHeadID"":""P265684""},{""PurchaseOrderHeadID"":""P265685""},{""PurchaseOrderHeadID"":""P265686""},{""PurchaseOrderHeadID"":""P265687""},{""PurchaseOrderHeadID"":""P265688""},{""PurchaseOrderHeadID"":""P265689""},{""PurchaseOrderHeadID"":""P265690""},{""PurchaseOrderHeadID"":""P265691""},{""PurchaseOrderHeadID"":""P265692""},{""PurchaseOrderHeadID"":""P265693""},{""PurchaseOrderHeadID"":""P265694""},{""PurchaseOrderHeadID"":""P265695""},{""PurchaseOrderHeadID"":""P265696""},{""PurchaseOrderHeadID"":""P265697""},{""PurchaseOrderHeadID"":""P265698""},{""PurchaseOrderHeadID"":""P265699""},{""PurchaseOrderHeadID"":""P265700""},{""PurchaseOrderHeadID"":""P265701""},{""PurchaseOrderHeadID"":""P265702""},{""PurchaseOrderHeadID"":""P265703""},{""PurchaseOrderHeadID"":""P265704""},{""PurchaseOrderHeadID"":""P265705""},{""PurchaseOrderHeadID"":""P265706""},{""PurchaseOrderHeadID"":""P265707""},{""PurchaseOrderHeadID"":""P265708""},{""PurchaseOrderHeadID"":""P265709""},{""PurchaseOrderHeadID"":""P265710""},{""PurchaseOrderHeadID"":""P265711""},{""PurchaseOrderHeadID"":""P265712""},{""PurchaseOrderHeadID"":""P265713""},{""PurchaseOrderHeadID"":""P265714""},{""PurchaseOrderHeadID"":""P265715""},{""PurchaseOrderHeadID"":""P265716""},{""PurchaseOrderHeadID"":""P265717""},{""PurchaseOrderHeadID"":""P265718""},{""PurchaseOrderHeadID"":""P265719""},{""PurchaseOrderHeadID"":""P265720""},{""PurchaseOrderHeadID"":""P265721""},{""PurchaseOrderHeadID"":""P265722""},{""PurchaseOrderHeadID"":""P265723""},{""PurchaseOrderHeadID"":""P265724""},{""PurchaseOrderHeadID"":""P265725""},{""PurchaseOrderHeadID"":""P265726""},{""PurchaseOrderHeadID"":""P265727""},{""PurchaseOrderHeadID"":""P265728""},{""PurchaseOrderHeadID"":""P265729""},{""PurchaseOrderHeadID"":""P265730""},{""PurchaseOrderHeadID"":""P265731""},{""PurchaseOrderHeadID"":""P265732""},{""PurchaseOrderHeadID"":""P265733""},{""PurchaseOrderHeadID"":""P265734""},{""PurchaseOrderHeadID"":""P265735""},{""PurchaseOrderHeadID"":""P265736""},{""PurchaseOrderHeadID"":""P265737""},{""PurchaseOrderHeadID"":""P265738""},{""PurchaseOrderHeadID"":""P265739""},{""PurchaseOrderHeadID"":""P265740""},{""PurchaseOrderHeadID"":""P265741""},{""PurchaseOrderHeadID"":""P265742""},{""PurchaseOrderHeadID"":""P265743""},{""PurchaseOrderHeadID"":""P265744""},{""PurchaseOrderHeadID"":""P265745""},{""PurchaseOrderHeadID"":""P265746""},{""PurchaseOrderHeadID"":""P265747""},{""PurchaseOrderHeadID"":""P265748""},{""PurchaseOrderHeadID"":""P265749""},{""PurchaseOrderHeadID"":""P265750""},{""PurchaseOrderHeadID"":""P265751""},{""PurchaseOrderHeadID"":""P265752""},{""PurchaseOrderHeadID"":""P265753""},{""PurchaseOrderHeadID"":""P265754""},{""PurchaseOrderHeadID"":""P265755""},{""PurchaseOrderHeadID"":""P265756""},{""PurchaseOrderHeadID"":""P265757""},{""PurchaseOrderHeadID"":""P265758""},{""PurchaseOrderHeadID"":""P265759""},{""PurchaseOrderHeadID"":""P265760""},{""PurchaseOrderHeadID"":""P265761""},{""PurchaseOrderHeadID"":""P265762""},{""PurchaseOrderHeadID"":""P265763""},{""PurchaseOrderHeadID"":""P265764""},{""PurchaseOrderHeadID"":""P265765""},{""PurchaseOrderHeadID"":""P265766""},{""PurchaseOrderHeadID"":""P265767""},{""PurchaseOrderHeadID"":""P265768""},{""PurchaseOrderHeadID"":""P265769""},{""PurchaseOrderHeadID"":""P265770""},{""PurchaseOrderHeadID"":""P265771""},{""PurchaseOrderHeadID"":""P265772""},{""PurchaseOrderHeadID"":""P265773""},{""PurchaseOrderHeadID"":""P265774""},{""PurchaseOrderHeadID"":""P265775""},{""PurchaseOrderHeadID"":""P265776""},{""PurchaseOrderHeadID"":""P265777""},{""PurchaseOrderHeadID"":""P265778""},{""PurchaseOrderHeadID"":""P265779""},{""PurchaseOrderHeadID"":""P265780""},{""PurchaseOrderHeadID"":""P265781""},{""PurchaseOrderHeadID"":""P265804""},{""PurchaseOrderHeadID"":""P265807""},{""PurchaseOrderHeadID"":""P265808""},{""PurchaseOrderHeadID"":""P265810""},{""PurchaseOrderHeadID"":""P265811""},{""PurchaseOrderHeadID"":""P265812""},{""PurchaseOrderHeadID"":""P265813""},{""PurchaseOrderHeadID"":""P265814""},{""PurchaseOrderHeadID"":""P265815""},{""PurchaseOrderHeadID"":""P265816""},{""PurchaseOrderHeadID"":""P265817""},{""PurchaseOrderHeadID"":""P265818""},{""PurchaseOrderHeadID"":""P265819""},{""PurchaseOrderHeadID"":""P265820""},{""PurchaseOrderHeadID"":""P265821""},{""PurchaseOrderHeadID"":""P265822""},{""PurchaseOrderHeadID"":""P265823""},{""PurchaseOrderHeadID"":""P265824""},{""PurchaseOrderHeadID"":""P265825""},{""PurchaseOrderHeadID"":""P265826""},{""PurchaseOrderHeadID"":""P265828""},{""PurchaseOrderHeadID"":""P265831""},{""PurchaseOrderHeadID"":""P265832""},{""PurchaseOrderHeadID"":""P265838""},{""PurchaseOrderHeadID"":""P265839""},{""PurchaseOrderHeadID"":""P265841""},{""PurchaseOrderHeadID"":""P265843""},{""PurchaseOrderHeadID"":""P265853""},{""PurchaseOrderHeadID"":""P265874""},{""PurchaseOrderHeadID"":""P265880""},{""PurchaseOrderHeadID"":""P265890""},{""PurchaseOrderHeadID"":""P265898""},{""PurchaseOrderHeadID"":""P265916""},{""PurchaseOrderHeadID"":""P265920""},{""PurchaseOrderHeadID"":""P265952""},{""PurchaseOrderHeadID"":""P265959""},{""PurchaseOrderHeadID"":""P265992""},{""PurchaseOrderHeadID"":""P265993""},{""PurchaseOrderHeadID"":""P265994""},{""PurchaseOrderHeadID"":""P265995""},{""PurchaseOrderHeadID"":""P265996""},{""PurchaseOrderHeadID"":""P265997""},{""PurchaseOrderHeadID"":""P265998""},{""PurchaseOrderHeadID"":""P265999""},{""PurchaseOrderHeadID"":""P266000""},{""PurchaseOrderHeadID"":""P266027""},{""PurchaseOrderHeadID"":""P266030""},{""PurchaseOrderHeadID"":""P266032""},{""PurchaseOrderHeadID"":""P266045""},{""PurchaseOrderHeadID"":""P266046""},{""PurchaseOrderHeadID"":""P266047""},{""PurchaseOrderHeadID"":""P266048""},{""PurchaseOrderHeadID"":""P266088""},{""PurchaseOrderHeadID"":""P266090""},{""PurchaseOrderHeadID"":""P266105""},{""PurchaseOrderHeadID"":""P266113""},{""PurchaseOrderHeadID"":""P266124""},{""PurchaseOrderHeadID"":""P266129""},{""PurchaseOrderHeadID"":""P266130""},{""PurchaseOrderHeadID"":""P266131""},{""PurchaseOrderHeadID"":""P266132""},{""PurchaseOrderHeadID"":""P266133""},{""PurchaseOrderHeadID"":""P266172""},{""PurchaseOrderHeadID"":""P266174""},{""PurchaseOrderHeadID"":""P266196""},{""PurchaseOrderHeadID"":""P266227""},{""PurchaseOrderHeadID"":""P266235""},{""PurchaseOrderHeadID"":""P266236""},{""PurchaseOrderHeadID"":""P266237""},{""PurchaseOrderHeadID"":""P266238""},{""PurchaseOrderHeadID"":""P266239""},{""PurchaseOrderHeadID"":""P266240""},{""PurchaseOrderHeadID"":""P266241""},{""PurchaseOrderHeadID"":""P266242""},{""PurchaseOrderHeadID"":""P266243""},{""PurchaseOrderHeadID"":""P266244""},{""PurchaseOrderHeadID"":""P266245""},{""PurchaseOrderHeadID"":""P266246""},{""PurchaseOrderHeadID"":""P266247""},{""PurchaseOrderHeadID"":""P266274""},{""PurchaseOrderHeadID"":""P266278""},{""PurchaseOrderHeadID"":""P266279""},{""PurchaseOrderHeadID"":""P266286""},{""PurchaseOrderHeadID"":""P266295""},{""PurchaseOrderHeadID"":""P266314""},{""PurchaseOrderHeadID"":""P266342""},{""PurchaseOrderHeadID"":""P266351""},{""PurchaseOrderHeadID"":""P266352""},{""PurchaseOrderHeadID"":""P266378""},{""PurchaseOrderHeadID"":""P266392""},{""PurchaseOrderHeadID"":""P266414""},{""PurchaseOrderHeadID"":""P266442""},{""PurchaseOrderHeadID"":""P266462""},{""PurchaseOrderHeadID"":""P266467""},{""PurchaseOrderHeadID"":""P266469""},{""PurchaseOrderHeadID"":""P266470""},{""PurchaseOrderHeadID"":""P266471""},{""PurchaseOrderHeadID"":""P266473""},{""PurchaseOrderHeadID"":""P266475""},{""PurchaseOrderHeadID"":""P266476""},{""PurchaseOrderHeadID"":""P266479""},{""PurchaseOrderHeadID"":""P266480""},{""PurchaseOrderHeadID"":""P266482""},{""PurchaseOrderHeadID"":""P266484""},{""PurchaseOrderHeadID"":""P266485""},{""PurchaseOrderHeadID"":""P266494""},{""PurchaseOrderHeadID"":""P266495""},{""PurchaseOrderHeadID"":""P266519""},{""PurchaseOrderHeadID"":""P266529""},{""PurchaseOrderHeadID"":""P266552""},{""PurchaseOrderHeadID"":""P266557""}]");

            //var accountDetails = new List<SupplierAccountDetail>();

            ////accountDetails.AddRange(SupplierFunctions.AccountDetailsSelect(uniVerseAccount, "L0150").OfType<SupplierAccountDetail>().ToList());

            //accountDetails.AddRange(SupplierFunctions.AccountDetails_Currency_Select(uniVerseAccount, null).OfType<SupplierAccountDetail>().ToList());

            //foreach (var ad in accountDetails)
            //{
            //    var SupplierReference = new SqlString();
            //    var SupplierName = new SqlString();
            //    var SupplierCurrency = new SqlString();
            //    var RecordCurrency = new SqlString();
            //    var DocumentReference = new SqlString();
            //    var DocumentType = new SqlString();
            //    var Sel1 = new SqlString();
            //    var Sel1D = new SqlString();
            //    var Sel2 = new SqlString();
            //    var Sel2D = new SqlString();
            //    var Sel3 = new SqlString();
            //    var Sel3D = new SqlString();
            //    var DocumentDT = new SqlDateTime();
            //    var DueDT = new SqlDateTime();
            //    var ItemReference = new SqlString();
            //    var OriginalValue = new SqlDecimal();
            //    var Balance = new SqlDecimal();
            //    var Disputed = new SqlBoolean();
            //    var Division = new SqlString();
            //    var Unallocated = new SqlDecimal();
            //    var Approved = new SqlBoolean();
            //    var PassedTo = new SqlString();

            //    SupplierFunctions.AccountDetailsSelect_Currency_FillRow(
            //        ad,
            //        out SupplierReference,
            //        out SupplierName,
            //        out SupplierCurrency,
            //        out RecordCurrency,
            //        out DocumentReference,
            //        out DocumentType,
            //        out Sel1,
            //        out Sel1D,
            //        out Sel2,
            //        out Sel2D,
            //        out Sel3,
            //        out Sel3D,
            //        out DocumentDT,
            //        out DueDT,
            //        out ItemReference,
            //        out OriginalValue,
            //        out Balance,
            //        out Disputed,
            //        out Division,
            //        out Unallocated,
            //        out Approved,
            //        out PassedTo);
            ////}

            //File.WriteAllText("TPLTRANS.DICT.txt", Dictionary("TPLTRANS"));

            //File.WriteAllText("HISTTPLTRANS.DICT.txt", Dictionary("HISTTPLTRANS"));

            //var recordId = "PI324493";

            //File.WriteAllText($"LIST.ITEM TPLTRANS {recordId}.txt", Item("TPLTRANS", recordId, false));
            //File.WriteAllText($"LIST.ITEM TPLTRANS.CUR {recordId}.txt", Item("TPLTRANS.CUR", recordId, false));
            //File.WriteAllText($"LIST.ITEM HISTTPLTRANS {recordId}.txt", Item("HISTTPLTRANS", recordId, false));
            //File.WriteAllText($"LIST.ITEM HISTTPLTRANS.CUR {recordId}.txt", Item("HISTTPLTRANS.CUR", recordId, false));

            Console.ForegroundColor = ConsoleColor.Green;

            stopwatch.Stop();

            Console.WriteLine($"Completed in {stopwatch.Elapsed}");

            Console.WriteLine();

            Console.WriteLine("Press any key to continue...");

            Console.ReadKey();
        }

        private static string Dictionary(string fileName, string database = "MERTEX")
        {
            using (var ce = new CommandExecutor(database))
            {
                return ce.Dictionary(fileName);
            }
        }

        private static string DictionaryXML(string fileName, string database = "MERTEX")
        {
            using (var ce = new CommandExecutor(database))
            {
                return ce.DictionaryXML(fileName);
            }
        }


        private static string DictionaryVertical(string fileName)
        {
            using (var ce = new CommandExecutor())
            {
                return ce.DictionaryVertical(fileName);
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

        private static string Item(string fileName, string recordId, bool lineSuppression = true, string universeAccount = "MERTEX")
        {
            var item = "";

            using (var ce = new CommandExecutor(universeAccount))
            {
                item = ce.Item(fileName, recordId, lineSuppression);
            }

            return item;
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