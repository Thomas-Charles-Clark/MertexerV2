using MertexerV2;
using MertexerV2.Models;
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
using MertexerV2.Models.GeneralLedger;
using MertexerV2.GeneralLedgerFunctions;
using MertexerV2.PurchaseLedgerFunctions;
using MertexerV2.Models.Company;

using CFramework = ConsoleFramework.Framework;
using MertexerV2.Models.ExchangeRate;
using Newtonsoft.Json.Linq;
using static System.Collections.Specialized.BitVector32;
using MertexerV2.CompanyFileFunctions;

namespace Tester
{
    class Program
    {
        private static readonly string WigglyLine = "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~";

        static void Main()
        {
            JsonConvert.DefaultSettings = () => {
                return new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Include,
                    //ContractResolver = new OrderedContractResolver()
                };
            };

            Console.OutputEncoding = Encoding.Unicode;

            CFramework.SetConsoleColourBusy();

            CFramework.WriteHeading("MertexerV2.dll Tests");

            //var tradeAccountCode = "K0098";

            var sw = new System.Diagnostics.Stopwatch();

            var uniVerseAccount = "PLAY";

            sw.Start();

            CFramework.WriteSubHeading($"Working in {uniVerseAccount}");

            //CFramework.WriteSubHeading("Company VAT Records");

            //Console.WriteLine();

            //var cfVatRecords = Selector<CompanyVAT>.SelectAll(true, false, 1000, uniVerseAccount);

            //File.WriteAllText("CFVAT.txt", JsonConvert.SerializeObject(cfVatRecords));

            //Console.WriteLine();

            //CFramework.WriteSubHeading("End of Company VAT Records");

            var now = DateTime.Now;

            var invoiceRegisterKey = "314332";

            //var registerEntryJson = TesterFramework.FetchInvoiceRegistrationJSON(invoiceRegisterKey, "FlockDev");// Selector<PurchaseLedgerInvoiceRegister>.Select("305127", true, false, 10, uniVerseAccount);

            //Console.WriteLine($"Calling PurchaseLedgerFunctions.InsertFromInvoiceRegister for PLINVREG key {invoiceRegisterKey} in {uniVerseAccount}");

            //var transactionResults = PurchaseLedgerFunctions.InsertFromInvoiceRegistry(uniVerseAccount, registerEntryJson).OfType<PurchaseLedgerInsertionResult>().ToList();

            Console.WriteLine($"Calling IncomingEmail.InvoiceRegisterEntry_RemoveFromPurchaseLedger_Select for @InvoiceRegisterKey {invoiceRegisterKey} in FlockDev");

            var rollbackJson = TesterFramework.FetchInvoiceRegistrationRollbackJSON(invoiceRegisterKey, "FlockDev");

            Console.WriteLine($"Calling PurchaseLedgerFunctions.RollbackInvoiceRegistryInsertionTransaction for PLINVREG key {invoiceRegisterKey} in {uniVerseAccount}");

            var transactionUndoResults = PurchaseLedgerFunctions.RollbackInvoiceRegistryInsertionTransaction(uniVerseAccount, rollbackJson).OfType<PurchaseLedgerTransactionRollbackResult>().ToList();

            Console.WriteLine(JsonConvert.SerializeObject(transactionUndoResults));

            CFramework.SetConsoleColourSuccess();

            sw.Stop();

            Console.WriteLine($"Completed in {sw.Elapsed}");

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