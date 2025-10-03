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

            //var results = PurchaseLedgerInvoiceRegisterFunctions.Insert(uniVerseAccount, @"{
            //    ""ReceivedDT"": ""2025-09-12"",
            //    ""OnFileDT"": ""2025-09-23"",
            //    ""DocumentDT"": ""2025-08-06"",
            //    ""DueDT"": ""2025-09-30"",
            //    ""SettlementDT"": ""2025-09-30"",
            //    ""PassedToDT"": ""2025-09-23"",
            //    ""PassedTo"": ""Sarah Waller"",
            //    ""SupplierCode"": ""C0307"",
            //    ""ItemReference"": ""123456"",
            //    ""PaymentTermsCode"": ""MN"",
            //    ""AmountCurrency"": 96.0,
            //    ""ExchangeRate"": 1.0,
            //    ""AmountSterling"": 96.0,
            //    ""VATBalance"": 96.0,
            //    ""DocumentBalance"": 96.0,
            //    ""Disputed"": null,
            //    ""DisputeMemo"": null,
            //    ""GeneralLedgerCodes"": [
            //    ""9999999999""
            //    ],
            //    ""AmountsExVAT"": [
            //    80.0
            //    ],
            //    ""VATCodes"": [
            //    ""S""
            //    ],
            //    ""VATableValues"": [
            //    80.0
            //    ],
            //    ""VATTaxes"": [
            //    16.0
            //    ],
            //    ""Memo"": null,
            //    ""OtherMemos"": null,
            //    ""MertexCurrencyCode"": ""STG"",
            //    ""discountAmount"": 0.0,
            //    ""discountPercentage"": 0.0
            //}").OfType<PurchaseLedgerInvoiceRegister>().ToList();

            //Console.WriteLine(JsonConvert.SerializeObject(results));



            //var periods = PeriodsFunctions.Select("MERTEX", null).OfType<PeriodExplosion>().ToList();

            //Console.WriteLine(JsonConvert.SerializeObject(periods));


            /*
             * 
             * 
                306687
                306688
                306689
                306690
                306691
                306692
                306693
                306694
                306695
                306696
             * 
             * 
             */

            var registerKeys = new[] {
                306726
            };

            var now = DateTime.Now;

            var invoiceRegisterKey = registerKeys.First().ToString();

            Console.WriteLine($"Calling PurchaseLedgerInvoiceRegisterFunctions.Select for PLINVREG key {invoiceRegisterKey} in {uniVerseAccount}");

            var rec = Selector<PurchaseLedgerInvoiceRegister>.Select(invoiceRegisterKey, true, false, 1, uniVerseAccount);

            var json = TesterFramework.FetchInvoiceRegistrationJSON(invoiceRegisterKey, "FlockDev");

            var transactionResults = PurchaseLedgerFunctions.InsertFromInvoiceRegistry(uniVerseAccount, json).OfType<PurchaseLedgerInsertionResult>().ToList();

            Console.WriteLine(JsonConvert.SerializeObject(transactionResults));



            //SqlString Key;
            //SqlDateTime DocumentDT;
            //SqlDateTime OnFileDT;
            //SqlString ItemReference;
            //SqlDateTime DueDT;
            //SqlString SupplierReference;
            //SqlString AmountsExVAT;
            //SqlString GeneralLedgerCodes;
            //SqlString VATCodes;
            //SqlString VATTaxes;
            //SqlString VATableValues;
            //SqlDecimal AmountSterling;
            //SqlString PaymentTermsCode;
            //SqlString Memo;
            //SqlDecimal VATBalance;
            //SqlDecimal DocumentBalance;
            //SqlString OtherMemos;
            //SqlDateTime ReceivedDT;
            //SqlString PassedTo;
            //SqlDateTime PassedToDT;
            //SqlDecimal AmountCurrency;
            //SqlDecimal ExchangeRate;
            //SqlString Dispute;
            //SqlString DisputeMemo;
            //SqlDateTime SettlementDT;
            //SqlString MertexCurrencyCode;
            //SqlDecimal DiscountPercentage;
            //SqlDecimal DiscountAmount;
            //SqlString RegisterKey;

            //PurchaseLedgerInvoiceRegisterFunctions.InvoiceRegisterSelect_FillRow(
            //    (object)rec,
            //    out Key,
            //    out DocumentDT,
            //    out OnFileDT,
            //    out ItemReference,
            //    out DueDT,
            //    out SupplierReference,
            //    out AmountsExVAT,
            //    out GeneralLedgerCodes,
            //    out VATCodes,
            //    out VATTaxes,
            //    out VATableValues,
            //    out AmountSterling,
            //    out PaymentTermsCode,
            //    out Memo,
            //    out VATBalance,
            //    out DocumentBalance,
            //    out OtherMemos,
            //    out ReceivedDT,
            //    out PassedTo,
            //    out PassedToDT,
            //    out AmountCurrency,
            //    out ExchangeRate,
            //    out Dispute,
            //    out DisputeMemo,
            //    out SettlementDT,
            //    out MertexCurrencyCode,
            //    out DiscountPercentage,
            //    out DiscountAmount,
            //    out RegisterKey
            //    );


            //var invoiceRegisterKey = "306697";


            //Console.WriteLine($"Calling IncomingEmail.InvoiceRegisterEntry_RemoveFromPurchaseLedger_Select for @InvoiceRegisterKey {invoiceRegisterKey} in FlockDev");

            //var rollbackJson = TesterFramework.FetchInvoiceRegistrationRollbackJSON(invoiceRegisterKey, "FlockDev");

            //Console.WriteLine($"Calling PurchaseLedgerFunctions.RollbackInvoiceRegistryInsertionTransaction for PLINVREG key {invoiceRegisterKey} in {uniVerseAccount}");

            //var transactionUndoResults = PurchaseLedgerFunctions.RollbackInvoiceRegistryInsertionTransaction(uniVerseAccount, rollbackJson).OfType<PurchaseLedgerTransactionRollbackResult>().ToList();

            //Console.WriteLine(JsonConvert.SerializeObject(transactionUndoResults));




            //using (var qe = new QueryExecutor(uniVerseAccount))
            //{
            //    var controlFile = qe.Session.CreateUniFile("PLCONTROL");

            //    var gennoRecords = controlFile.ReadRecords(new[] { "GENNO" });

            //    var gennoRecord = gennoRecords.GetRecord(0);

            //    var fieldPosition = 5; // PLINVREG

            //    var numberField = gennoRecord.Record.Extract(fieldPosition);

            //    var currentKey = Convert.ToInt32(numberField.StringValue);

            //    var newKey = currentKey += 1;

            //    gennoRecord.Record.Replace(5, $"{newKey}");

            //    controlFile.WriteRecords(gennoRecords);

            //    Console.WriteLine($"Read PLCONTROL<5> @ID = \"GENNO\", saw: {numberField}. New key is: {newKey}; has been written back to same.");
            //}

            //Test between for SQL DateTime Overflow: 2024 / 11 / 30 and 2024 / 12 / 31

            //var postDt_From = DateTime.ParseExact("2024/08/31", "yyyy/MM/dd", DateTimeFormatInfo.InvariantInfo);

            //var postDt_To = DateTime.ParseExact("2024/10/01", "yyyy/MM/dd", DateTimeFormatInfo.InvariantInfo);

            //var results = SalesLedgerTransactionHistoryFunctions
            //    .Select("MERTEX", null, null, null, null, null, null, postDt_From, postDt_To)
            //    .OfType<SalesLedgerTransactionHistory>()
            //    .ToList();

            //Console.WriteLine($"Saw {results.Count} results between PostDT {postDt_From:dd/MM/yyyy} and {postDt_To:dd/MM/yyyy}");

            //var bits = new int[4] { 0, 0, 0, 0 };

            //SqlString Key;
            //SqlDateTime DocumentDT;
            //SqlDateTime DueDT;
            //SqlString TransactionType;
            //SqlString ItemReference;
            //SqlDecimal OriginalValue = new SqlDecimal(28, 4, true, bits);
            //SqlString DocumentReference;
            //SqlDecimal VATDiscount = new SqlDecimal(28, 4, true, bits);
            //SqlDecimal Debit = new SqlDecimal(28, 4, true, bits);
            //SqlDecimal Credit = new SqlDecimal(28, 4, true, bits);
            //SqlDecimal Balance = new SqlDecimal(28, 4, true, bits);
            //SqlByte Period;
            //SqlString Status;
            //SqlString TradeAccountCode;
            //SqlInt32 Age;
            //SqlDecimal OriginalValueABS = new SqlDecimal(28, 4, true, bits);
            //SqlString DisputeMemo;
            //SqlString Dispute;
            //SqlString Adjust;
            //SqlDecimal Unallocated = new SqlDecimal(28, 4, true, bits);
            //SqlDecimal OutstandingAmount = new SqlDecimal(28, 4, true, bits);
            //SqlDateTime PostDT;
            //SqlDateTime CSDT;
            //SqlDecimal CSAmount = new SqlDecimal(28, 4, true, bits);
            //SqlString CSAction;
            //SqlString AllocatedFromDocuments;
            //SqlString AllocatedToDocuments;
            //SqlString InvoiceAccount;
            //SqlString CustomerOrderNumber;
            //SqlDecimal ExchangeRateDifferences = new SqlDecimal(28, 4, true, bits);
            //SqlDateTime PaidDT;
            //SqlDecimal RoundingErrors = new SqlDecimal(28, 4, true, bits);
            //SqlString SourceDocument;
            //SqlInt32 Version;
            //SqlDecimal SettlementDiscount = new SqlDecimal(28, 4, true, bits);
            //SqlDateTime SettlementDT;
            //SqlDecimal SettlementDiscountTaken = new SqlDecimal(28, 4, true, bits);
            //SqlString CreditClaimNumber;
            //SqlString SellingCompany;
            //SqlInt32 PostTime;

            //foreach (var r in results)
            //{
            //    try
            //    {
            //        SalesLedgerTransactionHistoryFunctions.SalesLedgerTransactionHistorySelect_FillRow(r,
            //            out Key,
            //            out DocumentDT,
            //            out DueDT,
            //            out TransactionType,
            //            out ItemReference,
            //            out OriginalValue,
            //            out DocumentReference,
            //            out VATDiscount,
            //            out Debit,
            //            out Credit,
            //            out Balance,
            //            out Period,
            //            out Status,
            //            out TradeAccountCode,
            //            out Age,
            //            out OriginalValueABS,
            //            out DisputeMemo,
            //            out Dispute,
            //            out Adjust,
            //            out Unallocated,
            //            out OutstandingAmount,
            //            out PostDT,
            //            out CSDT,
            //            out CSAmount,
            //            out CSAction,
            //            out AllocatedFromDocuments,
            //            out AllocatedToDocuments,
            //            out InvoiceAccount,
            //            out CustomerOrderNumber,
            //            out ExchangeRateDifferences,
            //            out PaidDT,
            //            out RoundingErrors,
            //            out SourceDocument,
            //            out Version,
            //            out SettlementDiscount,
            //            out SettlementDT,
            //            out SettlementDiscountTaken,
            //            out CreditClaimNumber,
            //            out SellingCompany,
            //            out PostTime
            //        );
            //    }
            //    catch (Exception ex)
            //    {
            //        CFramework.SetConsoleColourError();
            //        Console.WriteLine($"HISTTSLTRANS key: {r.Key}\r\n\r\n{ex}");
            //    }
            //}

            //Console.WriteLine(JsonConvert.SerializeObject(results));

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