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
using MertexerV2.Models.Company;

using CFramework = ConsoleFramework.Framework;
using MertexerV2.Models.ExchangeRate;
using Newtonsoft.Json.Linq;
using static System.Collections.Specialized.BitVector32;

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

            var invoiceRegisterKey = "316332";

            var registerEntryJson = TesterFramework.FetchInvoiceRegistrationJSON(invoiceRegisterKey);// Selector<PurchaseLedgerInvoiceRegister>.Select("305127", true, false, 10, uniVerseAccount);

            var registerEntryJObj = JObject.Parse(registerEntryJson);

            var registerEntry = registerEntryJObj["invoiceRegisterEntry"].ToObject<PurchaseLedgerInvoiceRegister>();

            var vatableValuesInCurrency = registerEntryJObj["vatEntries"].Children()["vatableAmountCurrency"].Values<decimal>().ToArray();

            var vatTaxesInCurrency = registerEntryJObj["vatEntries"].Children()["vatAmountCurrency"].Values<decimal>().ToArray();

            Console.WriteLine(JsonConvert.SerializeObject(registerEntryJObj));

            var purchaseLedgerKey = $"PI{registerEntry.Key}";

            var purchaseLedgerInvoiceKey = $"INV*{registerEntry.Key}";

            var companyEntry = Selector<Company>.Select("1", true, false, 1, uniVerseAccount);

            var purchaseLedgerEntry = new MertexerV2.Models.PurchaseLedger.PurchaseLedger(purchaseLedgerKey)
            {
                DocumentDT = registerEntry.DocumentDT,
                OnFileDT = registerEntry.OnFileDT,
                SupplierInvoiceReference = registerEntry.ItemReference,
                InvoiceDueDT = registerEntry.DueDT,
                SupplierReference = registerEntry.SupplierCode,
                SupplierCode = registerEntry.SupplierCode,
                Amounts = registerEntry.AmountsExVAT,
                GeneralLedgerCodes = registerEntry.GeneralLedgerCodes,
                VATCodes = registerEntry.VATCodes,
                // if I don't write VATTAXES, will it just... somehow magically execute the process PG.PLVATTAX,D.47.49..160[M] ? No, not likely.
                VATableValues = registerEntry.VATableValues,
                DocumentTotal = registerEntry.AmountSterling,
                SettlementTerms = registerEntry.PaymentTermsCode,
                PostDT = now,
                // Would be good to settle this once and for all:
                // SettlementDTs is 55.0M
                // DueDT is field 55.1... so I imagine that if I only set 55 and then overwrite 55.1 with NULL,
                // The end result is that 55 will be empty. So, belt and braces it is.
                SettlementDueDTs = new List<DateTime?>() { registerEntry.SettlementDT },
                DueDT = registerEntry.SettlementDT,
                //
                //

                Memo = registerEntry.Memo,
                VATBalance = registerEntry.VATBalance,
                VATTaxes = registerEntry.VATTaxes,
                VATTaxesInCurrency = vatTaxesInCurrency,
                DocumentBalance = registerEntry.DocumentBalance,
                DiscountAmount = 0.0m, // we don't do discounts: LIST PLTERMS DISC
                DiscountAmountInCurrency = 0.0M, // ""
                OtherMemos = registerEntry.OtherMemos,
                DisputeIndicator = registerEntry.Disputed,
                DisputeMemo = registerEntry.DisputeMemo,
                ReceivedDT = registerEntry.ReceivedDT,
                PassedTo = registerEntry.PassedTo,
                PassedToDT = registerEntry.PassedToDT,
                DiscountPercentage = registerEntry.DiscountPercentage ?? 0.0m,
                CurrencyCode = registerEntry.MertexCurrencyCode,
                ExchangeRates = new List<decimal>() { registerEntry.ExchangeRate },
                AmountInCurrency = registerEntry.AmountCurrency,

                // Prognostication: is this for some future period to worry about, or is it in the current period?
                // All I can do is compare the Invoice Date (DocumentDT) with the current Month.
                Future = registerEntry.DocumentDT?.Month > now.Month,

                // Here we have exhausted the fields on PLINVREG.
                // What further fields might TPLEDG need to provide to the subsequent... 14 operations in the transaction?
                PurchaseLedgerRegisterKey = registerEntry.Key,
                TransactionTypes = new List<string>() { "INV" },
                DocumentReference = registerEntry.Key,
                DocumentNumber = registerEntry.Key
            };

            var purchaseLedgerHistoryEntry = new PurchaseLedgerRegisterHistory(purchaseLedgerInvoiceKey)
            {
                RegistryNumber = registerEntry.Key,
                OnFileDT = registerEntry.OnFileDT,
                SupplierReference = registerEntry.SupplierCode,
                ConvertedDT = now,
                CreditNumber = null,
                InvoiceNumber = registerEntry.Key,
                UserID = "TOM",
                TransactionType = "INV",
                DeletedDT = null,
                DocumentTotal = registerEntry.AmountSterling
            };

            var currentPeriod = companyEntry.ThisPurchaseLedgerPeriod ?? companyEntry.ThisGeneralLedgerPeriod;

            if (purchaseLedgerEntry.Future)
            {
                currentPeriod += 1;

                if (currentPeriod > companyEntry.NumberOfFinancialPeriods)
                {
                    currentPeriod = 1;
                }
            }

            var purchaseLedgerTransactionEntry = new PurchaseLedgerTransaction(purchaseLedgerEntry.DocumentReference)
            {
                DocumentDT = purchaseLedgerEntry.DocumentDT,
                DueDT = purchaseLedgerEntry.InvoiceDueDT,
                DiscountPercentage = purchaseLedgerEntry.DiscountPercentage_Again,
                TransactionType = "INV",
                ItemReference = purchaseLedgerEntry.SupplierInvoiceReference,
                DocumentReference = purchaseLedgerEntry.DocumentNumber,
                VATDiscount = purchaseLedgerEntry.VATTaxes.Sum(),
                Credit = purchaseLedgerEntry.Amounts.Sum(),
                Balance = purchaseLedgerEntry.DocumentTotal,
                OriginalValueABS = purchaseLedgerEntry.DocumentTotal,
                Period = currentPeriod,
                SupplierReference = purchaseLedgerEntry.SupplierReference,
                Age = purchaseLedgerEntry.Future == true ? 0 : 1,
                Unallocated = 0.0m,
                DiscountAmount = purchaseLedgerEntry.DiscountAmount,
                PostDT = purchaseLedgerEntry.PostDT,
                Memo = purchaseLedgerEntry.Memo,
                DiscountCutOffDT = purchaseLedgerEntry.DueDT,
                Dispute = purchaseLedgerEntry.DisputeIndicator,
                DisputeMemo = purchaseLedgerEntry.DisputeMemo,
                PurchaseLedgerRegisterNumber = purchaseLedgerInvoiceKey
            };

            var purchaseLedgerTransactionCurrencyEntry = new PurchaseLedgerTransactionCurrency(purchaseLedgerEntry.DocumentReference)
            {
                DocumentDT = purchaseLedgerEntry.DocumentDT,
                DueDT = purchaseLedgerEntry.InvoiceDueDT,
                DiscountPercentage = purchaseLedgerEntry.DiscountPercentage_Again,
                TransactionType = "INV",
                ItemReference = purchaseLedgerEntry.SupplierInvoiceReference,
                DocumentReference = purchaseLedgerEntry.DocumentNumber,
                VATDiscount = purchaseLedgerEntry.VATTaxesInCurrency.Sum(),
                Credit = purchaseLedgerEntry.AmountInCurrency - purchaseLedgerEntry.VATTaxesInCurrency.Sum(),
                Balance = purchaseLedgerEntry.AmountInCurrency,
                OriginalValueABS = purchaseLedgerEntry.AmountInCurrency,
                Period = currentPeriod,
                SupplierReference = purchaseLedgerEntry.SupplierReference,
                Age = purchaseLedgerEntry.Future == true ? 0 : 1,
                Unallocated = 0.0m,
                DiscountAmount = purchaseLedgerEntry.DiscountAmountInCurrency,
                PostDT = purchaseLedgerEntry.PostDT,
                Memo = purchaseLedgerEntry.Memo,
                DiscountCutOffDT = purchaseLedgerEntry.DueDT,
                Dispute = purchaseLedgerEntry.DisputeIndicator,
                DisputeMemo = purchaseLedgerEntry.DisputeMemo,
                CurrencyCode = purchaseLedgerEntry.CurrencyCode,
                ExchangeRates = purchaseLedgerEntry.ExchangeRates,
                PurchaseLedgerRegisterNumber = purchaseLedgerInvoiceKey
            };

            var supplierCurrencyEntry = Selector<MertexerV2.Models.Supplier.SupplierCurrency>.Select(purchaseLedgerEntry.SupplierCode, true, false, 1, uniVerseAccount);

            var companyFilesVATEntry = Selector<CompanyVAT>.Select(purchaseLedgerEntry.VATCodes, true, false, 1, uniVerseAccount);

            var purchaseLedgerVATAuditEntry = new PurchaseLedgerVATAudit(purchaseLedgerEntry.DocumentReference)
            {

            };


            var supplierEntry = Selector<MertexerV2.Models.Supplier.Supplier>.Select(purchaseLedgerEntry.SupplierCode, true, false, 1, uniVerseAccount);


            using (var qe = new QueryExecutor(uniVerseAccount))
            {
                var session = qe.Session;

                var transaction = session.CreateUniTransaction();

                transaction.Begin();

                PurchaseLedgerFunctions.InsertViaTransaction(ref session, JsonConvert.SerializeObject(purchaseLedgerEntry));

                PurchaseLedgerRegisterHistoryFunctions.InsertViaTransaction(ref session, JsonConvert.SerializeObject(purchaseLedgerHistoryEntry));

                PurchaseLedgerInvoiceRegisterFunctions.DeleteViaTransaction(ref session, JsonConvert.SerializeObject(registerEntry));

                PurchaseLedgerTransactionFunctions.InsertViaTransaction(ref session, JsonConvert.SerializeObject(purchaseLedgerTransactionEntry));

                PurchaseLedgerTransactionCurrencyFunctions.InsertViaTransaction(ref session, JsonConvert.SerializeObject(purchaseLedgerTransactionCurrencyEntry));

                /* Update the Supplier record */
                supplierEntry.Balance += purchaseLedgerEntry.DocumentBalance;

                supplierEntry.NextPeriodBalance += PurchaseLedgerFunctions.CalculatePurchaseLedgerAmount(
                    session:            ref session,
                    type:               "N",
                    incomingValue:      purchaseLedgerEntry.DocumentTotal,
                    sumMultiValues:     false,
                    countMultiValues:   false
                );




                SupplierFunctions.UpsertViaTransaction(ref session, JsonConvert.SerializeObject(supplierEntry));

                SupplierCurrencyFunctions.UpsertViaTransaction(ref session, JsonConvert.SerializeObject(supplierCurrencyEntry));



                transaction.Commit();
            }

            Console.WriteLine();

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