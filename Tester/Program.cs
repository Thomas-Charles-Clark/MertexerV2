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

namespace Tester
{
    class Program
    {
        static void Main()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.ForegroundColor = ConsoleColor.Yellow;

            var tradeAccountNumber = "J0650";

            var tradeDataSourceJson = TesterFramework.FetchAccountDataSourceJson(tradeAccountNumber);

            var stopwatch = new System.Diagnostics.Stopwatch();

            var uniVerseAccount = "PLAY";

            stopwatch.Start();

            //EDAReplacementTestSuite.OECarriages("MERTEX");

            //Console.WriteLine("POCost");
            //Console.WriteLine(MaxFieldLengths<POCost>.Execute(null, true, false));
            //Console.WriteLine();
            //Console.WriteLine();

            //Console.WriteLine("OEINVHEAD");
            //Console.WriteLine(MaxFieldLengths<InvoiceHead>.Execute(null, true, false, 50));
            //Console.WriteLine();
            //Console.WriteLine();

            Console.WriteLine("OEINVHEAD");
            var headResults = InvoiceHeadFunctions.InitialiseSelect(uniVerseAccount, DateTime.ParseExact("01/11/2021", "dd/MM/yyyy", new CultureInfo("en-GB")), DateTime.ParseExact("03/03/2023", "dd/MM/yyyy", new CultureInfo("en-GB"))).OfType<InvoiceHead>().ToList();
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine($"Saw {headResults.Count} results in {stopwatch.Elapsed}");

            var documentNumbers = headResults.OrderBy(hr => hr.InvoiceDate).ThenBy(hr => hr.Key).Select(r => new InvoiceIDWrapper() { InvoiceID = r.Key });

            Console.WriteLine("OEINVLINE");
            var lineResults = InvoiceLineFunctions.Select(uniVerseAccount, null, JsonConvert.SerializeObject(documentNumbers)).OfType<InvoiceLine>().ToList();
            Console.WriteLine();
            Console.WriteLine();

            lineResults.ForEach(r =>
            {
                Console.WriteLine(r);

                var InvoiceLineKey = new SqlString();
                var InvoiceNo = new SqlString();
                var ProductCode = new SqlString();
                var QuantityOrdered = new SqlDecimal();
                var QuantityDespatched = new SqlDecimal();
                var QuantityInvoiced = new SqlDecimal();
                var UnitPriceExVAT = new SqlDecimal();
                var LineValueExVAT = new SqlDecimal();
                var DiscountPercentage = new SqlDecimal();
                var VATCode = new SqlString();
                var VATRate = new SqlDecimal();
                var DiscountValue = new SqlDecimal();
                var VATValue = new SqlDecimal();
                var StockUnit = new SqlString();
                var UnitCost = new SqlDecimal();
                var UnitCostCurrency = new SqlDecimal();
                var LineCost = new SqlDecimal();
                var LineCostCurrency = new SqlDecimal();
                var UnitPriceExVATCurrency = new SqlDecimal();
                var LineValueExVATCurrency = new SqlDecimal();
                var VATValueCurrency = new SqlDecimal();
                var GeneralLedgerCode = new SqlString();

                InvoiceLineFunctions.InvoiceLineSelect_FillRow(
                    r,
                    out InvoiceLineKey,
                    out InvoiceNo,
                    out ProductCode,
                    out QuantityOrdered,
                    out QuantityDespatched,
                    out QuantityInvoiced,
                    out UnitPriceExVAT,
                    out LineValueExVAT,
                    out DiscountPercentage,
                    out VATCode,
                    out VATRate,
                    out DiscountValue,
                    out VATValue,
                    out StockUnit,
                    out UnitCost,
                    out UnitCostCurrency,
                    out LineCost,
                    out LineCostCurrency,
                    out UnitPriceExVATCurrency,
                    out LineValueExVATCurrency,
                    out VATValueCurrency,
                    out GeneralLedgerCode);
            });

            Console.WriteLine($"Saw {lineResults.Count} results in {stopwatch.Elapsed}");

            //var results = MertexerV2.SQLFunctions.QuantityBreakCostPrice.Select("MERTEX", "[{ \"Code\": \"FDG2749/31\" }]", "FA", "DG").OfType<QuantityBreakCostPriceResult>().ToList();

            //results.ForEach(r => Console.WriteLine(r));

            //MertexerV2.Accounts.AccountFunctions.AccountSummary(tradeDataSourceJson);

            //var recordKey = "RUGDG0394*\\U0016";

            //var records = ProductExclusionFunctions.Select("MERTEX", null).OfType<ProductExclusionDerivation>().ToList();

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
                foreach(var batch in batches)
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