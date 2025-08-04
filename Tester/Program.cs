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
using MertexerV2.PurchaseLedgerFunctions;
using MertexerV2.Models.Company;

using CFramework = ConsoleFramework.Framework;
using MertexerV2.Models.ExchangeRate;
using Newtonsoft.Json.Linq;
using static System.Collections.Specialized.BitVector32;
using MertexerV2.CompanyFileFunctions;
using MertexerV2.Models.GeneralLedger;
using MertexerV2.GeneralLedgerFunctions;

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

            var uniVerseAccount = "MERTEX";

            sw.Start();

            CFramework.WriteSubHeading($"Working in {uniVerseAccount}");

            //CFramework.WriteSubHeading("Company VAT Records");

            //Console.WriteLine();

            //var cfVatRecords = Selector<CompanyVAT>.SelectAll(true, false, 1000, uniVerseAccount);

            //File.WriteAllText("CFVAT.txt", JsonConvert.SerializeObject(cfVatRecords));

            //Console.WriteLine();

            //CFramework.WriteSubHeading("End of Company VAT Records");

            var now = DateTime.Now;

            var invoiceRegisterKey = "316132"; // Supplier B0036

            var registerEntryJson = TesterFramework.FetchInvoiceRegistrationJSON(invoiceRegisterKey);// Selector<PurchaseLedgerInvoiceRegister>.Select("305127", true, false, 10, uniVerseAccount);

            var registerEntryJObj = JObject.Parse(registerEntryJson);

            var registerEntry = registerEntryJObj["invoiceRegisterEntry"].ToObject<PurchaseLedgerInvoiceRegister>();

            var vatableValuesInCurrency = registerEntryJObj["vatEntries"].Children()["vatableAmountCurrency"].Values<decimal>().ToArray();

            var vatTaxesInCurrency = registerEntryJObj["vatEntries"].Children()["vatAmountCurrency"].Values<decimal>().ToArray();

            //Console.WriteLine(JsonConvert.SerializeObject(registerEntryJObj));

            using (var qe = new QueryExecutor(uniVerseAccount))
            {
                var session = qe.Session;


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
                    TotalDueInSterling = registerEntry.AmountSterling,
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
                    DocumentReference = purchaseLedgerKey,
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

                var thisPeriod = currentPeriod;

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
                    DocumentNumber = purchaseLedgerEntry.DocumentNumber,
                    VATDiscount = purchaseLedgerEntry.VATTaxes.Sum(),
                    Credit = purchaseLedgerEntry.Amounts.Sum(),
                    Balance = purchaseLedgerEntry.TotalDueInSterling,
                    OriginalValueABS = purchaseLedgerEntry.TotalDueInSterling,
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
                    DocumentNumber = purchaseLedgerEntry.DocumentNumber,
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

                var supplierEntry = Selector<MertexerV2.Models.Supplier.Supplier>.Select(purchaseLedgerEntry.SupplierCode, true, false, 1, uniVerseAccount);

                var supplierCurrencyEntry = Selector<MertexerV2.Models.Supplier.SupplierCurrency>.Select(purchaseLedgerEntry.SupplierCode, true, false, 1, uniVerseAccount);

                Console.WriteLine();

                CFramework.WriteSubHeading("PurchaseLedgerEntry");

                Console.WriteLine();

                Console.WriteLine(purchaseLedgerEntry);

                Console.WriteLine(WigglyLine);

                Console.WriteLine();

                CFramework.WriteSubHeading("SupplierEntry Currently");

                Console.WriteLine();

                Console.WriteLine(supplierEntry);

                Console.WriteLine(WigglyLine);

                var companyFilesVATEntries = Selector<CompanyVAT>.Select(purchaseLedgerEntry.VATCodes, true, false, 1, uniVerseAccount);

                var purchaseLedgerVATAuditEntry = new PurchaseLedgerVATAudit(purchaseLedgerEntry.DocumentReference)
                {
                    DocumentDT = purchaseLedgerEntry.DocumentDT,
                    TransactionType = "INV",
                    SupplierReference = purchaseLedgerEntry.SupplierReference,
                    PurchaseLedgerTotalDue = purchaseLedgerEntry.TotalDueInSterling,
                    VATCodes = purchaseLedgerEntry.VATCodes,
                    VATableValues = purchaseLedgerEntry.VATableValues,
                    VATTaxes = purchaseLedgerEntry.VATTaxes,
                    NextPeriod = purchaseLedgerEntry.Future
                };

                var companyFilesStatsEntry = Selector<CompanyStats>.Select("1", true, false, 1, uniVerseAccount);

                var generalLedgerBatchEntry = new GeneralLedgerBatch(purchaseLedgerKey)
                {
                    TransactionType = "RFPOST",
                    DataKeys = purchaseLedgerEntry.DocumentReferences,
                    NumberOfRecords = 1
                };

                var generalLedgerBatchDataEntry = new GeneralLedgerBatchData(purchaseLedgerKey)
                {
                    Type = "RFPOST",
                    DocumentReference = purchaseLedgerEntry.DocumentReference,
                    Prefix = "P",
                    DocumentDT = purchaseLedgerEntry.DocumentDT,
                    PostDT = purchaseLedgerEntry.PostDT,
                    SupplierReference = purchaseLedgerEntry.SupplierReference,
                    GeneralLedgerCodes = purchaseLedgerEntry.GeneralLedgerCodes,
                    VATTax = purchaseLedgerEntry.VATTaxes.Sum(),
                    DocumentTotal = purchaseLedgerEntry.TotalDueInSterling,
                    SupplierName = supplierEntry.Name,
                    TransactionType = "PINV",
                    GeneralLedgerSubCodes = purchaseLedgerEntry.GeneralLedgerSubCodes,
                    Memo = purchaseLedgerEntry.Memo,
                    OtherMemos = purchaseLedgerEntry.OtherMemos,
                    ItemReference = purchaseLedgerEntry.SupplierInvoiceReference
                };

                var purchaseLedgerAdjustmentAuditEntry = new PurchaseLedgerAdjustmentAudit($"*{purchaseLedgerEntry.DocumentReference}")
                {
                    DocumentTotal = purchaseLedgerEntry.TotalDueInSterling,
                    TransactionType = "INV",
                    DocumentDT = purchaseLedgerEntry.DocumentDT,
                    PostDT = now,
                    VATDiscount = purchaseLedgerTransactionEntry.VATDiscount,
                    SupplierReference = purchaseLedgerEntry.SupplierReference,
                    Balances = new[] { purchaseLedgerTransactionEntry.Balance },
                    GeneralLedgerCodes = purchaseLedgerEntry.GeneralLedgerCodes,
                    Discounts = new[] { purchaseLedgerTransactionEntry.DiscountAmount },
                    GeneralLedgerAmounts = purchaseLedgerEntry.Amounts,
                    Ports = new[] { session.HostPort },
                    Time = now.ToString("HH:mm"),
                    Dispute = purchaseLedgerEntry.DisputeIndicator,
                    DueDT = purchaseLedgerEntry.InvoiceDueDT,
                    Currency = purchaseLedgerEntry.CurrencyCode,
                    VATDiscountCurrency = purchaseLedgerTransactionCurrencyEntry.VATDiscount,
                    DocumentTotalCurrency = purchaseLedgerEntry.AmountInCurrency,
                    ExchangeRates = purchaseLedgerEntry.ExchangeRates
                };

                var purchaseLedgerSupplierInvoiceEntry = new PurchaseLedgerSupplierInvoice($"{purchaseLedgerEntry.SupplierReference}*{purchaseLedgerEntry.SupplierInvoiceReference}")
                {
                    SupplierCode = purchaseLedgerEntry.SupplierReference,
                    SupplierInvoiceReference = purchaseLedgerEntry.SupplierInvoiceReference,
                    TransactionType = "INV",
                    DocumentDT = purchaseLedgerEntry.DocumentDT,
                    DocumentReference = purchaseLedgerEntry.DocumentReference,
                    PostDT = purchaseLedgerEntry.PostDT
                };

                var generalLedgerPrePaymentTransaction = new GeneralLedgerPrePaymentTransaction(GeneralLedgerPrePaymentTransactionFunctions.BuildKey())
                {

                }


























                //var transaction = session.CreateUniTransaction();

                //transaction.Begin();

                //PurchaseLedgerFunctions.InsertViaTransaction(ref session, JsonConvert.SerializeObject(purchaseLedgerEntry));

                //PurchaseLedgerRegisterHistoryFunctions.InsertViaTransaction(ref session, JsonConvert.SerializeObject(purchaseLedgerHistoryEntry));

                //PurchaseLedgerInvoiceRegisterFunctions.DeleteViaTransaction(ref session, JsonConvert.SerializeObject(registerEntry));

                //PurchaseLedgerTransactionFunctions.InsertViaTransaction(ref session, JsonConvert.SerializeObject(purchaseLedgerTransactionEntry));

                //PurchaseLedgerTransactionCurrencyFunctions.InsertViaTransaction(ref session, JsonConvert.SerializeObject(purchaseLedgerTransactionCurrencyEntry));

                /* Update the Supplier record */
                supplierEntry.Balance += purchaseLedgerEntry.DocumentBalance;

                supplierEntry.NextPeriodBalance += PurchaseLedgerFunctions.CalculatePurchaseLedgerAmount(
                    amounts:       new[] { purchaseLedgerEntry.TotalDueInSterling }, // <50> "CHQTOTAL/DTOTAL/RDTOTAL/STGAMT/STGCHQTOTAL/TOTDUE"
                                        // it is critical that we do not read "currentPeriod" as defined above into this function call;
                                        // the subroutine itself explicitly returns early on line 40 if you're in the final period: IF PER=NOPER THEN RETURN        
                    currentPeriod:      thisPeriod,
                    numberOfPeriods:    companyEntry.NumberOfFinancialPeriods,
                    operationPeriod:    "N",
                    future:             purchaseLedgerEntry.Future,
                    mvOperationType:    null
                ); // PLAMOUNT,N50

                supplierEntry.ThisPeriodBalance += PurchaseLedgerFunctions.CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.TotalDueInSterling }, 
                    currentPeriod: thisPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: null,
                    future: purchaseLedgerEntry.Future,
                    mvOperationType: null
                ); // PLAMOUNT,50

                supplierEntry.DocumentReferences = supplierEntry.DocumentReferences.Prepend(purchaseLedgerEntry.Key);

                supplierEntry.PurchasesNextPeriod += PurchaseLedgerFunctions.CalculatePurchaseLedgerAmount(
                    amounts: purchaseLedgerEntry.Amounts, // <43> "PAMOUNT"
                    currentPeriod: thisPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "N",
                    future: purchaseLedgerEntry.Future,
                    mvOperationType: "A"
                ); // PLAMOUNT,N43.A

                supplierEntry.PurchasesThisPeriod += PurchaseLedgerFunctions.CalculatePurchaseLedgerAmount(
                    amounts: purchaseLedgerEntry.Amounts,
                    currentPeriod: thisPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: null,
                    future: purchaseLedgerEntry.Future,
                    mvOperationType: "A"
                ); // PLAMOUNT,43.A

                supplierEntry.PurchasesYearToDate += PurchaseLedgerFunctions.CalculatePurchaseLedgerAmount(
                    amounts: purchaseLedgerEntry.Amounts,
                    currentPeriod: thisPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "Y",
                    future: purchaseLedgerEntry.Future,
                    mvOperationType: "A"
                ); // PLAMOUNT,Y43.A

                // P("INACT")
                supplierEntry.PeriodsInactive = purchaseLedgerEntry.Future ? null : (sbyte?)-1;

                Console.WriteLine();

                CFramework.WriteSubHeading("SupplierEntry Afterwards");

                Console.WriteLine();

                CFramework.WriteSubHeading("SupplierCurrencyEntry Before");

                Console.WriteLine();

                Console.WriteLine(supplierCurrencyEntry);

                Console.WriteLine(WigglyLine);

                supplierCurrencyEntry.Balance += purchaseLedgerEntry.AmountInCurrency; // <250> "CURAMT"

                supplierCurrencyEntry.NextPeriodBalance += PurchaseLedgerFunctions.CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.AmountInCurrency }, // <250> "CURAMT"
                                                                             // it is critical that we do not read "currentPeriod" as defined above into this function call;
                                                                             // the subroutine itself explicitly returns early on line 40 if you're in the final period: IF PER=NOPER THEN RETURN        
                    currentPeriod: thisPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "N",
                    future: purchaseLedgerEntry.Future,
                    mvOperationType: null
                ); // PLAMOUNT,N250

                supplierCurrencyEntry.ThisPeriodBalance += PurchaseLedgerFunctions.CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.AmountInCurrency }, // <250> "CURAMT"
                    currentPeriod: thisPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: null,
                    future: purchaseLedgerEntry.Future,
                    mvOperationType: null
                ); // PLAMOUNT,250

                supplierCurrencyEntry.DocumentReferences = supplierCurrencyEntry.DocumentReferences.Prepend(purchaseLedgerEntry.Key);

                supplierCurrencyEntry.PurchasesNextPeriod += PurchaseLedgerFunctions.CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.AmountInCurrency }, // <250> "CURAMT"
                    currentPeriod: thisPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "N",
                    future: purchaseLedgerEntry.Future,
                    mvOperationType: null
                ); // PLAMOUNT,N250

                supplierCurrencyEntry.PurchasesNextPeriod -= PurchaseLedgerFunctions.CalculatePurchaseLedgerAmount(
                    amounts: purchaseLedgerEntry.VATTaxesInCurrency, // <248> "VATTAX.CUR"
                    currentPeriod: thisPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "N",
                    future: purchaseLedgerEntry.Future,
                    mvOperationType: "A"
                ); // PLAMOUNT,N248.A

                supplierCurrencyEntry.PurchasesThisPeriod += PurchaseLedgerFunctions.CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.AmountInCurrency }, // <250> "CURAMT"
                    currentPeriod: thisPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: null,
                    future: purchaseLedgerEntry.Future,
                    mvOperationType: null
                ); // PLAMOUNT,250

                supplierCurrencyEntry.PurchasesThisPeriod -= PurchaseLedgerFunctions.CalculatePurchaseLedgerAmount(
                    amounts: purchaseLedgerEntry.VATTaxesInCurrency, // <248> "VATTAX.CUR"
                    currentPeriod: thisPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: null,
                    future: purchaseLedgerEntry.Future,
                    mvOperationType: "A"
                ); // PLAMOUNT,248.A

                supplierCurrencyEntry.PurchasesYearToDate += PurchaseLedgerFunctions.CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.AmountInCurrency }, // <250> "CURAMT"
                    currentPeriod: thisPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "Y",
                    future: purchaseLedgerEntry.Future,
                    mvOperationType: null
                ); // PLAMOUNT,Y250

                supplierCurrencyEntry.PurchasesYearToDate -= PurchaseLedgerFunctions.CalculatePurchaseLedgerAmount(
                    amounts: purchaseLedgerEntry.VATTaxesInCurrency, // <248> "VATTAX.CUR"
                    currentPeriod: thisPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "Y",
                    future: purchaseLedgerEntry.Future,
                    mvOperationType: "A"
                ); // PLAMOUNT,Y248.A

                supplierCurrencyEntry.PeriodsInactive = purchaseLedgerEntry.Future ? null : (sbyte?)-1;

                //Console.WriteLine();

                //CFramework.WriteSubHeading("SupplierCurrencyEntry Afterwards");

                //Console.WriteLine();

                //Console.WriteLine(supplierCurrencyEntry);

                //SupplierFunctions.UpsertViaTransaction(ref session, JsonConvert.SerializeObject(supplierEntry));

                //SupplierCurrencyFunctions.UpsertViaTransaction(ref session, JsonConvert.SerializeObject(supplierCurrencyEntry));

                // Update the Company Files VAT Entry...

                foreach(var vatEntry in companyFilesVATEntries)
                {
                    Console.WriteLine("vatEntry before\r\n");

                    Console.WriteLine(vatEntry);

                    vatEntry.PurchasesInvoicesTax += PurchaseLedgerFunctions.CalculatePurchaseLedgerAmount(
                        amounts: purchaseLedgerEntry.VATTaxes, // <48> "VATTAX"
                        currentPeriod: thisPeriod,
                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                        operationPeriod: null,
                        future: purchaseLedgerEntry.Future,
                        mvOperationType: "X"
                    ); // PLAMOUNT,48X

                    vatEntry.PurchasesInvoicesValue += PurchaseLedgerFunctions.CalculatePurchaseLedgerAmount(
                        amounts: purchaseLedgerEntry.VATableValues, // <49> "VATVALUE"
                        currentPeriod: thisPeriod,
                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                        operationPeriod: null,
                        future: purchaseLedgerEntry.Future,
                        mvOperationType: "X"
                    ); // PLAMOUNT,48X

                    vatEntry.PurchasesQuarterTaxTotal += PurchaseLedgerFunctions.CalculatePurchaseLedgerAmount(
                        amounts: purchaseLedgerEntry.VATTaxes, // <48> "VATTAX"
                        currentPeriod: thisPeriod,
                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                        operationPeriod: null,
                        future: purchaseLedgerEntry.Future,
                        mvOperationType: "X"
                    ); // PLAMOUNT,48X

                    vatEntry.PurchasesQuarterValueTotal += PurchaseLedgerFunctions.CalculatePurchaseLedgerAmount(
                        amounts: purchaseLedgerEntry.VATableValues, // <49> "VATVALUE"
                        currentPeriod: thisPeriod,
                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                        operationPeriod: null,
                        future: purchaseLedgerEntry.Future,
                        mvOperationType: "X"
                    ); // PLAMOUNT,48X

                    vatEntry.PurchasesNextPeriodInvoicesTax += PurchaseLedgerFunctions.CalculatePurchaseLedgerAmount(
                        amounts: purchaseLedgerEntry.VATTaxes, // <48> "VATTAX"
                        currentPeriod: thisPeriod,
                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                        operationPeriod: "N",
                        future: purchaseLedgerEntry.Future,
                        mvOperationType: "X"
                    ); // PLAMOUNT,N48X

                    vatEntry.PurchasesNextPeriodInvoicesValue += PurchaseLedgerFunctions.CalculatePurchaseLedgerAmount(
                        amounts: purchaseLedgerEntry.VATableValues, // <49> "VATVALUE"
                        currentPeriod: thisPeriod,
                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                        operationPeriod: "N",
                        future: purchaseLedgerEntry.Future,
                        mvOperationType: "X"
                    ); // PLAMOUNT,N48X

                    Console.WriteLine("vatEntry afterwards\r\n");

                    Console.WriteLine(vatEntry);

                    //CompanyVATFunctions.UpsertViaTransaction(ref session, JsonConvert.SerializeObject(vatEntry));
                }

                companyFilesStatsEntry.PurchaseLedgerInvoices += PurchaseLedgerFunctions.CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.TotalDueInSterling },
                    currentPeriod: thisPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: null,
                    future: purchaseLedgerEntry.Future,
                    mvOperationType: null
                );

                companyFilesStatsEntry.NextPurchaseLedgerInvoices += PurchaseLedgerFunctions.CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.TotalDueInSterling },
                    currentPeriod: thisPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "N",
                    future: purchaseLedgerEntry.Future,
                    mvOperationType: null
                );

                //CompanyStatsFunctions.UpsertViaTranscation(ref session, JsonConvert.SerializeObject(companyFilesStatsEntry));

                //GeneralLedgerBatchFunctions.UpsertViaTransaction(ref session, JsonConvert.SerializeObject(generalLedgerBatchEntry));

                //GeneralLedgerBatchDataFunctions.UpsertViaTransaction(ref session, JsonConvert.SerializeObject(generalLedgerBatchDataEntry));

                //PurchaseLedgerAdjustmentAuditFunctions.UpsertViaTransaction(ref session, JsonConvert.SerializeObject(purchaseLedgerAdjustmentAuditEntry));

                //PurchaseLedgerSupplierInvoiceFunctions.UpsertViaTransaction(ref session, JsonConvert.SerializeObject(purchaseLedgerSupplierInvoiceEntry));




                // But don't you dare write anything you madman!
                //CompanyVATFunctions.UpsertViaTransaction(ref session, JsonConvert.SerializeObject(companyFilesVATEntry));

                //transaction.Commit();
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