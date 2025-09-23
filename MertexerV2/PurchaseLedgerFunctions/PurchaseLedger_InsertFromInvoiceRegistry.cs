
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

using MertexerV2.CompanyFileFunctions;
using MertexerV2.GeneralFunctions;
using MertexerV2.SupplierFunctions;
using MertexerV2.GeneralLedgerFunctions;
using MertexerV2.Models.Company;
using MertexerV2.Models.GeneralLedger;
using MertexerV2.Models.PurchaseLedger;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MertexerV2.PurchaseLedgerFunctions
{
    public static partial class PurchaseLedgerFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "PurchaseLedger_InsertFromInvoiceRegistry_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable InsertFromInvoiceRegistry(string UniVerseAccount, string PurchaseLedgerInvoiceRegistryJSON)
        {
            var results = new List<PurchaseLedgerInsertionResult>();

            var now = DateTime.Now;

            var registerEntryJson = PurchaseLedgerInvoiceRegistryJSON;

            var registerEntryJObj = JObject.Parse(registerEntryJson);

            var registerEntry = registerEntryJObj["invoiceRegisterEntry"].ToObject<PurchaseLedgerInvoiceRegister>();

            var vatableAmountsInCurrency = registerEntryJObj["vatEntries"].Children()["vatableAmountCurrency"].Values<decimal>().ToArray();

            var vatTaxesInCurrency = registerEntryJObj["vatEntries"].Children()["vatAmountCurrency"].Values<decimal>().ToArray();

            using (var qe = new QueryExecutor(UniVerseAccount))
            {
                var session = qe.Session;

                var transaction = session.CreateUniTransaction();

                transaction.Begin();

                #region Properties & Initialisation

                #region Keys

                // Read the most recently held TPLEDG key from PLCONTROL<9> WITH @ID = "GENNO"

                var fieldPosition = 9;

                var purchaseLedgerControlFile = qe.Session.CreateUniFile("PLCONTROL");

                var numberGeneratorRecords = purchaseLedgerControlFile.ReadRecords(new[] { "GENNO" });

                var numberGeneratorRecord = numberGeneratorRecords.GetRecord(0);

                var numberField = numberGeneratorRecord.Record.Extract(fieldPosition); // Field 9 on the record "GENNO"

                var currentKey = Convert.ToInt32(numberField.StringValue);

                var newKey = currentKey += 1;

                numberGeneratorRecord.Record.Replace(fieldPosition, $"{newKey}");

                purchaseLedgerControlFile.WriteRecords(numberGeneratorRecords);

                var purchaseLedgerKey = $"PI{newKey}";

                #endregion

                #region Other Files and Errata

                var companyEntry = Selector<Company>.Select("1", true, false, 1, UniVerseAccount);

                var generalLedgerPrePaymentCodes = companyEntry.PrepaymentGeneralLedgerCodes.ToList();

                var companyStatsEntry = Selector<CompanyStats>.Select("1", true, false, 1, UniVerseAccount);

                var supplierEntry = Selector<Models.Supplier.Supplier>.Select(registerEntry.SupplierCode, true, false, 1, UniVerseAccount);

                var supplierCurrencyEntry = Selector<Models.Supplier.SupplierCurrency>.Select(registerEntry.SupplierCode, true, false, 1, UniVerseAccount);

                var companyVATEntries = Selector<CompanyVAT>.Select(registerEntry.VATCodes, true, false, 1, UniVerseAccount);

                var currentPeriod = companyEntry.ThisPurchaseLedgerPeriod ?? companyEntry.ThisGeneralLedgerPeriod;

                var purchaseLedgerTransactionPeriod = currentPeriod;

                var prePaymentPurchaseLedgerCodes = companyEntry.PrepaymentGeneralLedgerCodes.ToList();

                #endregion

                #endregion

                #region Step 01. Create a Purchase Ledger entry (INSERT to TPLEDG)

                var purchaseLedgerEntry = new PurchaseLedger(purchaseLedgerKey)
                {
                    PurchaseLedgerRegisterKey = registerEntry.Key,
                    DocumentReference = purchaseLedgerKey,
                    DocumentNumber = registerEntry.Key,
                    SourceTransactionDefinition = "PINVIR.CUR.DES",
                    DocumentDT = registerEntry.DocumentDT,
                    OnFileDT = registerEntry.OnFileDT,
                    SupplierInvoiceReference = registerEntry.ItemReference,
                    InvoiceDueDT = registerEntry.DueDT,
                    SupplierCode = registerEntry.SupplierCode,
                    Amounts = registerEntry.AmountsExVAT,
                    GeneralLedgerCodes = registerEntry.GeneralLedgerCodes,
                    VATCodes = registerEntry.VATCodes,
                    VATableValues = registerEntry.VATableValues,
                    TotalDueInSterling = registerEntry.AmountSterling,
                    SettlementTerms = registerEntry.PaymentTermsCode,
                    PostDT = now,
                    SettlementDueDTs = new List<DateTime?>() { registerEntry.SettlementDT },
                    DueDT = registerEntry.SettlementDT,
                    Memo = registerEntry.Memo,
                    VATBalance = registerEntry.VATBalance,
                    VATTaxes = registerEntry.VATTaxes,
                    VATTaxesInCurrency = vatTaxesInCurrency,
                    DocumentBalance = registerEntry.DocumentBalance,
                    DiscountAmount = 0.0m, // we don't do discounts here: LIST PLTERMS DISC
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
                    Future = registerEntry.DocumentDT?.Month > now.Month
                };

                var purchaseLedgerEntryJson = JsonConvert.SerializeObject(purchaseLedgerEntry);

                PurchaseLedgerFunctions.InsertViaTransaction(ref session, purchaseLedgerEntryJson);

                results.Add(new PurchaseLedgerInsertionResult(
                    fileKey: purchaseLedgerEntry.Key,
                    fileName: "TPLEDG",
                    operationsPerformed: new[] { "INSERT" },
                    jsonBefore: null,
                    jsonAfterwards: purchaseLedgerEntryJson,
                    purchaseLedgerRegistryKey: registerEntry.Key
                ));

                if (purchaseLedgerEntry.Future == true)
                {
                    purchaseLedgerTransactionPeriod += 1;

                    if (currentPeriod > companyEntry.NumberOfFinancialPeriods)
                    {
                        purchaseLedgerTransactionPeriod = 1;
                    }
                }

                #endregion

                #region Step 02. Create a Purchase Ledger Register History entry (INSERT to PLREGISTER.HIST)

                // Key = INV*TPLEDG<199>
                var purchaseLedgerRegisterHistoryEntry = new PurchaseLedgerRegisterHistory($"INV*{purchaseLedgerEntry.PurchaseLedgerRegisterKey}")
                {
                    RegisterNumber = registerEntry.Key,
                    OnFileDT = registerEntry.OnFileDT,
                    SupplierCode = registerEntry.SupplierCode,
                    ConvertedDT = now,
                    DocumentReference = purchaseLedgerKey,
                    UserID = "TOM",
                    TransactionType = "INV",
                    DeletedDT = null,
                    DocumentTotal = registerEntry.AmountSterling
                };

                var purchaseLedgerRegisterHistoryEntryJson = JsonConvert.SerializeObject(purchaseLedgerRegisterHistoryEntry);

                PurchaseLedgerRegisterHistoryFunctions.InsertViaTransaction(ref session, purchaseLedgerRegisterHistoryEntryJson);

                results.Add(new PurchaseLedgerInsertionResult(
                    purchaseLedgerRegistryKey: registerEntry.Key,
                    fileKey: purchaseLedgerRegisterHistoryEntry.Key,
                    fileName: "PLREGISTER.HIST",
                    operationsPerformed: new[] { "INSERT" },
                    jsonBefore:null,
                    jsonAfterwards: purchaseLedgerRegisterHistoryEntryJson
                ));

                #endregion

                #region Step 03. Delete the given Purchase Ledger Invoice Register entry (DELETE from PLINVREG)

                var purchaseRegisterEntryJson = JsonConvert.SerializeObject(registerEntry);

                PurchaseLedgerInvoiceRegisterFunctions.DeleteViaTransaction(ref session, JsonConvert.SerializeObject(registerEntry));

                results.Add(new PurchaseLedgerInsertionResult(
                    fileKey: registerEntry.Key,
                    fileName: "PLINVREG",
                    operationsPerformed: new[] { "DELETE" },
                    jsonBefore: purchaseRegisterEntryJson,
                    jsonAfterwards: null,
                    purchaseLedgerRegistryKey: registerEntry.Key
                ));

                #endregion

                #region Step 04. Create a Purchase Ledger Transaction entry (INSERT into TPLTRANS)

                // Key == TPLEDG<53> "Document Reference"
                var purchaseLedgerTransactionEntry = new PurchaseLedgerTransaction(purchaseLedgerEntry.DocumentReference)
                {
                    DocumentDT = purchaseLedgerEntry.DocumentDT,
                    DueDT = purchaseLedgerEntry.InvoiceDueDT,
                    DiscountPercentage = purchaseLedgerEntry.DiscountPercentage_Again,
                    TransactionType = "INV",
                    ItemReference = purchaseLedgerEntry.SupplierInvoiceReference,
                    DocumentNumber = purchaseLedgerEntry.DocumentNumber,
                    VATTaxesTotal = purchaseLedgerEntry.VATTaxes.Sum(),
                    Credit = purchaseLedgerEntry.Amounts.Sum(),
                    Balance = purchaseLedgerEntry.TotalDueInSterling,
                    OriginalValueABS = purchaseLedgerEntry.TotalDueInSterling,
                    Period = purchaseLedgerTransactionPeriod,
                    SupplierReference = purchaseLedgerEntry.SupplierCode,
                    Age = purchaseLedgerEntry.Future == true ? 0 : 1,
                    Unallocated = 0.0m,
                    DiscountAmount = purchaseLedgerEntry.DiscountAmount,
                    PostDT = purchaseLedgerEntry.PostDT,
                    Memo = purchaseLedgerEntry.Memo,
                    DiscountCutOffDT = purchaseLedgerEntry.DueDT,
                    Dispute = purchaseLedgerEntry.DisputeIndicator,
                    DisputeMemo = purchaseLedgerEntry.DisputeMemo,
                    PurchaseLedgerRegisterNumber = $"INV*{purchaseLedgerEntry.PurchaseLedgerRegisterKey}"
                };

                var purchaseLedgerTransactionEntryJson = JsonConvert.SerializeObject(purchaseLedgerTransactionEntry);

                PurchaseLedgerTransactionFunctions.UpsertViaTransaction(ref session, purchaseLedgerTransactionEntryJson);

                results.Add(new PurchaseLedgerInsertionResult(
                    fileKey: purchaseLedgerTransactionEntry.Key,
                    fileName:  "TPLTRANS",
                    operationsPerformed: new [] { "INSERT" },
                    jsonBefore: null,
                    jsonAfterwards: purchaseLedgerTransactionEntryJson,
                    purchaseLedgerRegistryKey: registerEntry.Key
                ));

                #endregion

                #region Step 05. Create a Purchase Ledger Transaction Currency entry (INSERT into TPLTRANS.CUR)

                var purchaseLedgerTransactionCurrencyEntry = new PurchaseLedgerTransactionCurrency(purchaseLedgerEntry.DocumentReference)
                {
                    DocumentDT = purchaseLedgerEntry.DocumentDT,
                    DueDT = purchaseLedgerEntry.InvoiceDueDT,
                    DiscountPercentage = purchaseLedgerEntry.DiscountPercentage_Again,
                    TransactionType = "INV",
                    ItemReference = purchaseLedgerEntry.SupplierInvoiceReference,
                    DocumentNumber = purchaseLedgerEntry.DocumentNumber,
                    VATTaxesTotal = purchaseLedgerEntry.VATTaxesInCurrency.Sum(),
                    Credit = purchaseLedgerEntry.AmountInCurrency - purchaseLedgerEntry.VATTaxesInCurrency.Sum(),
                    Balance = purchaseLedgerEntry.AmountInCurrency,
                    OriginalValueABS = purchaseLedgerEntry.AmountInCurrency,
                    Period = purchaseLedgerTransactionPeriod,
                    SupplierReference = purchaseLedgerEntry.SupplierCode,
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
                    PurchaseLedgerRegisterNumber = $"INV*{purchaseLedgerEntry.PurchaseLedgerRegisterKey}"
                };

                var purchaseLedgerTransactionCurrencyEntryJson = JsonConvert.SerializeObject(purchaseLedgerTransactionCurrencyEntry);

                PurchaseLedgerTransactionCurrencyFunctions.UpsertViaTransaction(ref session, purchaseLedgerTransactionCurrencyEntryJson);

                results.Add(new PurchaseLedgerInsertionResult(
                    fileKey: purchaseLedgerTransactionCurrencyEntry.Key,
                    fileName: "TPLTRANS.CUR",
                    operationsPerformed: new[] { "INSERT" },
                    jsonBefore: null,
                    jsonAfterwards: purchaseLedgerTransactionCurrencyEntryJson,
                    purchaseLedgerRegistryKey: registerEntry.Key
                ));

                #endregion

                #region Step 06. Update the relevant Supplier record (UPDATE to PLSUP)

                var supplierEntryBeforeModificationJSON = JsonConvert.SerializeObject(supplierEntry);

                supplierEntry.Balance += purchaseLedgerEntry.DocumentBalance;

                var supplierEntryNextPeriodBalance = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.TotalDueInSterling }, // <50> "CHQTOTAL/DTOTAL/RDTOTAL/STGAMT/STGCHQTOTAL/TOTDUE"
                                                                               // the subroutine itself explicitly returns early on line 40 if you're in the final period: IF PER=NOPER THEN RETURN        
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "N",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                );

                supplierEntry.NextPeriodBalance += supplierEntryNextPeriodBalance; // PLAMOUNT,N50

                var supplierEntryThisPeriodBalance = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.TotalDueInSterling },
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: null,
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                );

                supplierEntry.ThisPeriodBalance += supplierEntryThisPeriodBalance; // PLAMOUNT,50

                supplierEntry.DocumentReferences = supplierEntry.DocumentReferences.Prepend(purchaseLedgerEntry.Key);

                var supplierEntryPurchasesNextPeriod = CalculatePurchaseLedgerAmount(
                    amounts: purchaseLedgerEntry.Amounts, // <43> "PAMOUNT"
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "N",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: "A"
                );

                supplierEntry.PurchasesNextPeriod += supplierEntryPurchasesNextPeriod; // PLAMOUNT,N43.A

                var supplierEntryPurchasesThisPeriod = CalculatePurchaseLedgerAmount(
                    amounts: purchaseLedgerEntry.Amounts,
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: null,
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: "A"
                );

                supplierEntry.PurchasesThisPeriod += supplierEntryPurchasesThisPeriod; // PLAMOUNT,43.A

                var supplierEntryPurchasesYearToDate = CalculatePurchaseLedgerAmount(
                    amounts: purchaseLedgerEntry.Amounts,
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "Y",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: "A"
                );

                supplierEntry.PurchasesYearToDate += supplierEntryPurchasesYearToDate; // PLAMOUNT,Y43.A

                // P("INACT")
                supplierEntry.PeriodsInactive = purchaseLedgerEntry.Future == true ? null : (sbyte?)-1;

                var supplierEntryJson = JsonConvert.SerializeObject(supplierEntry);

                SupplierFunctions.SupplierFunctions.UpsertViaTransaction(ref session, supplierEntryJson);

                results.Add(new PurchaseLedgerInsertionResult(
                    fileKey: supplierEntry.Key,
                    fileName: "PLSUP",
                    operationsPerformed: new[] {
                        $"<15> \"BALANCE\" (Balance) += {purchaseLedgerEntry.DocumentBalance}",
                        $"<18,1> \"NEXT\" (NextPeriodBalance) += {supplierEntryNextPeriodBalance}",
                        $"<18,2> \"CURR\" (ThisPeriodBalance) += {supplierEntryThisPeriodBalance}",
                        $"<19> \"TLINK\" += \"{purchaseLedgerEntry.Key}\"",
                        $"<21,1> \"PNEXT\" (PurchasesNextPeriod) += {supplierEntryPurchasesNextPeriod}",
                        $"<21,2> \"PCURR\" (PurchasesThisPeriod) += {supplierEntryPurchasesThisPeriod}",
                        $"<21,3> \"PYTD\" (PurchasesYearToDate) += {supplierEntryPurchasesYearToDate}",
                        $"<23> \"INACT\" (PeriodsInactive) = \"{supplierEntry.PeriodsInactive}\""
                    },
                    jsonBefore: supplierEntryBeforeModificationJSON,
                    jsonAfterwards: supplierEntryJson,
                    purchaseLedgerRegistryKey: registerEntry.Key
                ));

                #endregion

                #region Step 07. Update the relevant Supplier Currency record (UPDATE to PLSUP.CUR)

                var supplierCurrencyEntryBeforeModificationJSON = JsonConvert.SerializeObject(supplierCurrencyEntry);

                /* Update the Supplier Currency record */

                var supplierCurrencyEntryBalanceAddition = purchaseLedgerEntry.AmountInCurrency;

                supplierCurrencyEntry.Balance += supplierCurrencyEntryBalanceAddition; // <250> "CURAMT"

                var supplierCurrencyEntryNextPeriodBalanceAddition = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.AmountInCurrency }, // <250> "CURAMT"
                                                                             // the subroutine itself explicitly returns early on line 40 if you're in the final period: IF PER=NOPER THEN RETURN        
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "N",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                );

                supplierCurrencyEntry.NextPeriodBalance += supplierCurrencyEntryNextPeriodBalanceAddition; // PLAMOUNT,N250

                var supplierCurrencyEntryThisPeriodBalanceAddition = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.AmountInCurrency }, // <250> "CURAMT"
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: null,
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                );

                supplierCurrencyEntry.ThisPeriodBalance += supplierCurrencyEntryThisPeriodBalanceAddition; // PLAMOUNT,250

                supplierCurrencyEntry.DocumentReferences = supplierCurrencyEntry.DocumentReferences.Prepend(purchaseLedgerEntry.Key);

                var supplierCurrencyEntryPurchasesNextPeriodAddition = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.AmountInCurrency }, // <250> "CURAMT"
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "N",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                );

                supplierCurrencyEntry.PurchasesNextPeriod += supplierCurrencyEntryPurchasesNextPeriodAddition; // PLAMOUNT,N250

                var supplierCurrencyEntryPurchasesNextPeriodReduction = CalculatePurchaseLedgerAmount(
                    amounts: purchaseLedgerEntry.VATTaxesInCurrency, // <248> "VATTAX.CUR"
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "N",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: "A"
                );

                supplierCurrencyEntry.PurchasesNextPeriod -= supplierCurrencyEntryPurchasesNextPeriodReduction; // PLAMOUNT,N248.A

                var supplierCurrencyEntryPurchasesThisPeriodAddition = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.AmountInCurrency }, // <250> "CURAMT"
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: null,
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                );

                supplierCurrencyEntry.PurchasesThisPeriod += supplierCurrencyEntryPurchasesThisPeriodAddition; // PLAMOUNT,250

                var supplierCurrencyEntryPurchasesThisPeriodReduction = CalculatePurchaseLedgerAmount(
                    amounts: purchaseLedgerEntry.VATTaxesInCurrency, // <248> "VATTAX.CUR"
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: null,
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: "A"
                );

                supplierCurrencyEntry.PurchasesThisPeriod -= supplierCurrencyEntryPurchasesThisPeriodReduction; // PLAMOUNT,248.A

                var supplierCurrencyEntryPurchasesYearToDateAddition = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.AmountInCurrency }, // <250> "CURAMT"
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "Y",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                );

                supplierCurrencyEntry.PurchasesYearToDate += supplierCurrencyEntryPurchasesYearToDateAddition; // PLAMOUNT,Y250

                var supplierCurrencyEntryPurchasesYearToDateReduction = CalculatePurchaseLedgerAmount(
                    amounts: purchaseLedgerEntry.VATTaxesInCurrency, // <248> "VATTAX.CUR"
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "Y",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: "A"
                );

                supplierCurrencyEntry.PurchasesYearToDate -= supplierCurrencyEntryPurchasesYearToDateReduction; // PLAMOUNT,Y248.A

                supplierCurrencyEntry.PeriodsInactive = purchaseLedgerEntry.Future == true ? null : (sbyte?)-1;



                // 07. UPDATE PLSUP

                var supplierCurrencyEntryJson = JsonConvert.SerializeObject(supplierCurrencyEntry);

                SupplierCurrencyFunctions.UpsertViaTransaction(ref session, supplierCurrencyEntryJson);

                results.Add(new PurchaseLedgerInsertionResult(
                    fileKey: supplierEntry.Key,
                    fileName: "PLSUP.CUR",
                    operationsPerformed: new[] {
                        $"<15> \"BALANCE\" (Balance) += {supplierCurrencyEntryBalanceAddition}",
                        $"<18,1> \"NEXT\" (NextPeriodBalance) += {supplierCurrencyEntryNextPeriodBalanceAddition}",
                        $"<18,2> \"CURR\" (ThisPeriodBalance) += {supplierCurrencyEntryThisPeriodBalanceAddition}",
                        $"<19> \"TLINK\" += \"{purchaseLedgerEntry.Key}\"",
                        $"<21,1> \"PNEXT\" (PurchasesNextPeriod) += {supplierCurrencyEntryPurchasesNextPeriodAddition}",
                        $"<21,1> \"PNEXT\" (PurchasesNextPeriod) -= {supplierCurrencyEntryPurchasesNextPeriodReduction}",
                        $"<21,2> \"PCURR\" (PurchasesThisPeriod) += {supplierCurrencyEntryPurchasesThisPeriodAddition}",
                        $"<21,2> \"PCURR\" (PurchasesThisPeriod) -= {supplierCurrencyEntryPurchasesThisPeriodReduction}",
                        $"<21,3> \"PYTD\" (PurchasesYearToDate) += {supplierCurrencyEntryPurchasesYearToDateAddition}",
                        $"<21,3> \"PYTD\" (PurchasesYearToDate) -= {supplierCurrencyEntryPurchasesYearToDateReduction}",
                        $"<23> \"INACT\" (PeriodsInactive) = \"{supplierCurrencyEntry.PeriodsInactive}\""
                    },
                    jsonBefore: supplierCurrencyEntryBeforeModificationJSON,
                    jsonAfterwards: supplierCurrencyEntryJson,
                    purchaseLedgerRegistryKey: registerEntry.Key
                ));

                #endregion

                #region Step 08. Update the relevant CFVAT record(s), for each of the VAT codes declared on the given invoice registry entry (UPDATE(s) to CFVAT)

                foreach (var vatEntry in companyVATEntries)
                {
                    var vatEntryBeforeModificationJSON = JsonConvert.SerializeObject(companyVATEntries);

                    var purchasesInvoicesTaxAddition = CalculatePurchaseLedgerAmount(
                        amounts: purchaseLedgerEntry.VATTaxes, // <48> "VATTAX"
                        currentPeriod: currentPeriod,
                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                        operationPeriod: null,
                        future: purchaseLedgerEntry.Future == true,
                        mvOperationType: "X"
                    );

                    vatEntry.PurchasesInvoicesTax += purchasesInvoicesTaxAddition; // PLAMOUNT,48X

                    var purchasesInvoicesValueAddition = CalculatePurchaseLedgerAmount(
                        amounts: purchaseLedgerEntry.VATableValues, // <49> "VATVALUE"
                        currentPeriod: currentPeriod,
                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                        operationPeriod: null,
                        future: purchaseLedgerEntry.Future == true,
                        mvOperationType: "X"
                    );

                    vatEntry.PurchasesInvoicesValue += purchasesInvoicesValueAddition; // PLAMOUNT,48X

                    var purchasesQuarterTaxTotalAddition = CalculatePurchaseLedgerAmount(
                        amounts: purchaseLedgerEntry.VATTaxes, // <48> "VATTAX"
                        currentPeriod: currentPeriod,
                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                        operationPeriod: null,
                        future: purchaseLedgerEntry.Future == true,
                        mvOperationType: "X"
                    );

                    vatEntry.PurchasesQuarterTaxTotal += purchasesQuarterTaxTotalAddition; // PLAMOUNT,48X

                    var purchasesQuarterValueTotalAddition = CalculatePurchaseLedgerAmount(
                        amounts: purchaseLedgerEntry.VATableValues, // <49> "VATVALUE"
                        currentPeriod: currentPeriod,
                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                        operationPeriod: null,
                        future: purchaseLedgerEntry.Future == true,
                        mvOperationType: "X"
                    );

                    vatEntry.PurchasesQuarterValueTotal += purchasesQuarterValueTotalAddition; // PLAMOUNT,48X

                    var purchasesNextPeriodInvoicesTaxAddition = CalculatePurchaseLedgerAmount(
                        amounts: purchaseLedgerEntry.VATTaxes, // <48> "VATTAX"
                        currentPeriod: currentPeriod,
                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                        operationPeriod: "N",
                        future: purchaseLedgerEntry.Future == true,
                        mvOperationType: "X"
                    );

                    vatEntry.PurchasesNextPeriodInvoicesTax += purchasesNextPeriodInvoicesTaxAddition; // PLAMOUNT,N48X

                    var purchasesNextPeriodInvoicesValueAddition = CalculatePurchaseLedgerAmount(
                        amounts: purchaseLedgerEntry.VATableValues, // <49> "VATVALUE"
                        currentPeriod: currentPeriod,
                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                        operationPeriod: "N",
                        future: purchaseLedgerEntry.Future == true,
                        mvOperationType: "X"
                    );

                    vatEntry.PurchasesNextPeriodInvoicesValue += purchasesNextPeriodInvoicesValueAddition; // PLAMOUNT,N48X

                    var vatEntryJson = JsonConvert.SerializeObject(vatEntry);

                    CompanyVATFunctions.UpsertViaTransaction(ref session, vatEntryJson);

                    results.Add(new PurchaseLedgerInsertionResult(
                        fileKey: vatEntry.Key,
                        fileName: "CFVAT",
                        operationsPerformed: new[] {
                            $"<30> \"PINVT\" (PurchasesInvoicesTax) += {purchasesInvoicesTaxAddition}",
                            $"<34> \"PINVV\" (PurchasesInvoicesValue) += {purchasesInvoicesValueAddition}",
                            $"<38> \"PQTTOT\" (PurchasesQuarterTaxTotal) += {purchasesQuarterTaxTotalAddition}",
                            $"<39> \"PQVTOT\" (PurchasesQuarterValueTotal) += {purchasesQuarterValueTotalAddition}",
                            $"<40> \"PNINVT\" (PurchasesNextPeriodInvoicesTax) += {purchasesNextPeriodInvoicesTaxAddition}",
                            $"<44> \"PNINVV\" (PurchasesNextPeriodInvoicesValue) += {purchasesNextPeriodInvoicesValueAddition}", },
                        jsonBefore: vatEntryBeforeModificationJSON,
                        jsonAfterwards: vatEntryJson,
                        purchaseLedgerRegistryKey: registerEntry.Key
                    ));
                }

                #endregion

                #region Step 09. Create a Purchase Ledger VAT Audit entry (INSERT to PLVATAUD)

                /* Insert the PurchaseLedgerVATAudit record */

                var purchaseLedgerVATAuditEntry = new PurchaseLedgerVATAudit(purchaseLedgerEntry.DocumentReference)
                {
                    DocumentDT = purchaseLedgerEntry.DocumentDT,
                    TransactionType = "INV",
                    SupplierCode = purchaseLedgerEntry.SupplierCode,
                    PurchaseLedgerTotalDueInSterling = purchaseLedgerEntry.TotalDueInSterling,
                    VATCodes = purchaseLedgerEntry.VATCodes,
                    VATableValuesInSterling = purchaseLedgerEntry.VATableValues,
                    VATTaxesInSterling = purchaseLedgerEntry.VATTaxes,
                    NextPeriod = purchaseLedgerEntry.Future == true
                };

                var purchaseLedgerVATAuditEntryJson = JsonConvert.SerializeObject(purchaseLedgerVATAuditEntry);

                PurchaseLedgerVATAuditFunctions.UpsertViaTransaction(ref session, purchaseLedgerVATAuditEntryJson);

                results.Add(new PurchaseLedgerInsertionResult(
                    fileKey: purchaseLedgerVATAuditEntry.Key,
                    fileName: "PLVATAUD",
                    operationsPerformed: new[] { "INSERT" },
                    jsonBefore: null,
                    jsonAfterwards: purchaseLedgerVATAuditEntryJson,
                        purchaseLedgerRegistryKey: registerEntry.Key
                ));

                #endregion

                #region Step 10. Update the Company Statistics record (UPDATE to CFSTATS)

                var companyStatsEntryBeforeModificationJSON = JsonConvert.SerializeObject(companyStatsEntry);

                var companyStatsEntryPurchaseLedgerInvoicesAddition = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.TotalDueInSterling },
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: null,
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                );

                companyStatsEntry.PurchaseLedgerInvoices += companyStatsEntryPurchaseLedgerInvoicesAddition;

                var companyStatsEntryNextPurchaseLedgerInvoicesAddition = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.TotalDueInSterling },
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "N",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                );

                companyStatsEntry.NextPurchaseLedgerInvoices += companyStatsEntryNextPurchaseLedgerInvoicesAddition;

                var companyStatsEntryJson = JsonConvert.SerializeObject(companyStatsEntry);

                CompanyStatsFunctions.UpsertViaTransaction(ref session, companyStatsEntryJson);

                results.Add(new PurchaseLedgerInsertionResult(
                    fileKey: companyStatsEntry.Key,
                    fileName: "CFSTATS",
                    operationsPerformed: new string[] {
                        $"<11> \"PLINV\" (PurchaseLedgerInvoices) += {companyStatsEntryPurchaseLedgerInvoicesAddition}",
                        $"<21> \"NPLINV\" (NextPurchaseLedgerInvoices) += {companyStatsEntryNextPurchaseLedgerInvoicesAddition}"
                    },
                    jsonBefore: companyStatsEntryBeforeModificationJSON,
                    jsonAfterwards: companyStatsEntryJson,
                    purchaseLedgerRegistryKey: registerEntry.Key
                ));

                #endregion

                #region Step 11. Create a General Ledger Batch entry (INSERT to GLBATCH)

                var generalLedgerBatchEntry = new GeneralLedgerBatch(purchaseLedgerKey)
                {
                    TransactionType = "RFPOST",
                    DataKeys = purchaseLedgerEntry.DocumentReferences,
                    NumberOfRecords = 1
                };

                var generalLedgerBatchEntryJson = JsonConvert.SerializeObject(generalLedgerBatchEntry);

                GeneralLedgerBatchFunctions.UpsertViaTransaction(ref session, generalLedgerBatchEntryJson);

                results.Add(new PurchaseLedgerInsertionResult(
                    fileKey: generalLedgerBatchEntry.Key,
                    fileName: "GLBATCH",
                    operationsPerformed: new[] { "INSERT" },
                    jsonBefore: null,
                    jsonAfterwards: generalLedgerBatchEntryJson,
                        purchaseLedgerRegistryKey: registerEntry.Key
                ));

                #endregion

                #region Step 12. Create a the General Ledger Batch Data entry (INSERT to GLBATCHDATA)

                var generalLedgerBatchDataEntry = new GeneralLedgerBatchData(purchaseLedgerKey)
                {
                    Type = "RFPOST",
                    DocumentNumber = purchaseLedgerEntry.DocumentReference,
                    Prefix = "P",
                    DocumentDT = purchaseLedgerEntry.DocumentDT,
                    PostDT = purchaseLedgerEntry.PostDT,
                    SupplierCode = purchaseLedgerEntry.SupplierCode,
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

                // 12. INSERT to GLBATCHDATA

                var generalLedgerBatchDataEntryJson = JsonConvert.SerializeObject(generalLedgerBatchDataEntry);

                GeneralLedgerBatchDataFunctions.UpsertViaTransaction(ref session, generalLedgerBatchDataEntryJson);

                results.Add(new PurchaseLedgerInsertionResult(
                    fileKey: generalLedgerBatchDataEntry.Key,
                    fileName: "GLBATCHDATA",
                    operationsPerformed: new[] { "INSERT" },
                    jsonBefore: null,
                    jsonAfterwards: generalLedgerBatchDataEntryJson,
                        purchaseLedgerRegistryKey: registerEntry.Key
                ));

                #endregion

                #region Step 13. Create a Purchase Ledger Adjustment Audit entry (INSERT to PLADJAUDIT)

                var purchaseLedgerAdjustmentAuditEntry = new PurchaseLedgerAdjustmentAudit($"*{purchaseLedgerEntry.DocumentReference}")
                {
                    DocumentTotal = purchaseLedgerEntry.TotalDueInSterling,
                    TransactionType = "INV",
                    DocumentDT = purchaseLedgerEntry.DocumentDT,
                    PostDT = now,
                    VATTaxesTotalInSterling = purchaseLedgerTransactionEntry.VATTaxesTotal,
                    SupplierCode = purchaseLedgerEntry.SupplierCode,
                    Balances = new[] { purchaseLedgerTransactionEntry.Balance },
                    GeneralLedgerCodes = purchaseLedgerEntry.GeneralLedgerCodes,
                    Discounts = new[] { purchaseLedgerTransactionEntry.DiscountAmount },
                    GeneralLedgerAmounts = purchaseLedgerEntry.Amounts,
                    Ports = new[] { session.HostPort },
                    Time = now.ToString("HH:mm"),
                    Dispute = purchaseLedgerEntry.DisputeIndicator,
                    DueDT = purchaseLedgerEntry.InvoiceDueDT,
                    CurrencyCode = purchaseLedgerEntry.CurrencyCode,
                    VATTaxesTotalInCurrency = purchaseLedgerTransactionCurrencyEntry.VATTaxesTotal,
                    DocumentTotalInCurrency = purchaseLedgerEntry.AmountInCurrency,
                    ExchangeRates = purchaseLedgerEntry.ExchangeRates
                };

                var purchaseLedgerAdjustmentAuditEntryJson = JsonConvert.SerializeObject(purchaseLedgerAdjustmentAuditEntry);

                PurchaseLedgerAdjustmentAuditFunctions.UpsertViaTransaction(ref session, purchaseLedgerAdjustmentAuditEntryJson);

                results.Add(new PurchaseLedgerInsertionResult(
                    fileKey: purchaseLedgerAdjustmentAuditEntry.Key,
                    fileName: "PLADJAUDIT",
                    operationsPerformed: new[] { "INSERT" },
                    jsonBefore: null,
                    jsonAfterwards: purchaseLedgerAdjustmentAuditEntryJson,
                        purchaseLedgerRegistryKey: registerEntry.Key
                ));

                #endregion

                #region Step 14. Create a Purchase Ledger Supplier Invoice entry (INSERT into PLSUPINV)

                var purchaseLedgerSupplierInvoiceEntry = new PurchaseLedgerSupplierInvoice($"{purchaseLedgerEntry.SupplierCode}*{purchaseLedgerEntry.SupplierInvoiceReference}")
                {
                    SupplierCode = purchaseLedgerEntry.SupplierCode,
                    SupplierInvoiceReference = purchaseLedgerEntry.SupplierInvoiceReference,
                    TransactionType = "INV",
                    DocumentDT = purchaseLedgerEntry.DocumentDT,
                    DocumentNumber = purchaseLedgerEntry.DocumentReference,
                    PostDT = purchaseLedgerEntry.PostDT
                };

                var purchaseLedgerSupplierInvoiceEntryJson = JsonConvert.SerializeObject(purchaseLedgerSupplierInvoiceEntry);

                results.Add(new PurchaseLedgerInsertionResult(
                    fileKey: purchaseLedgerSupplierInvoiceEntry.Key,
                    fileName: "PLSUPINV",
                    operationsPerformed: new[] { "INSERT" },
                    jsonBefore: null,
                    jsonAfterwards: purchaseLedgerSupplierInvoiceEntryJson,
                        purchaseLedgerRegistryKey: registerEntry.Key
                ));

                PurchaseLedgerSupplierInvoiceFunctions.UpsertViaTransaction(ref session, purchaseLedgerSupplierInvoiceEntryJson);

                #endregion

                #region Step 15. Create the necessary GLPPTRANS.DES entries (if any of the provided GL codes are PrePayment codes)

                // This is a bit messy: the data in GLPPTRANS.DES (even very recent data) contravenes the dictionary entries for the fields
                // all I can hope to do is faithfully follow what the transcation definition is attempting to do, so I shall.

                var generalLedgerPrepaymentTransactionEntries = new List<GeneralLedgerPrePaymentTransaction>();

                for (var i = 0; i < purchaseLedgerEntry.GeneralLedgerCodes.Count(); i++)
                {
                    var glc = purchaseLedgerEntry.GeneralLedgerCodes.ElementAt(i);

                    if (prePaymentPurchaseLedgerCodes.Contains(glc))
                    {
                        // Create a GLPPTRANS.DES record for this GLC.
                        // Must have a new, unique key - the largest key in GLPPTRANS.CUR file, incremented by one.

                        var glpKeys = session.CreateUniCommand();

                        glpKeys.Command = "LIST GLPPTRANS.DES BY-DSND COL.HDR.SUPP";

                        glpKeys.Execute();

                        var latestKeyString = glpKeys.Response.Trim().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).First();

                        var latestKeyInt = Convert.ToInt32(latestKeyString);

                        var nextKeyInt = latestKeyInt + 1;

                        var nextKeyString = $"{latestKeyInt}".PadLeft(8, '0');

                        var prePaymentAmountSplits = purchaseLedgerEntry.PrepaymentSplitAmounts.ElementAtOrDefault(i);

                        var prePaymentMemoSplits = purchaseLedgerEntry.PrepaymentSplitMemos.ElementAtOrDefault(i);

                        var prePaymentMemo = purchaseLedgerEntry.PrepaymentMemos.ElementAtOrDefault(i);

                        var otherPrePaymentMemo = purchaseLedgerEntry.OtherPrepaymentMemos.ElementAtOrDefault(i);

                        var generalLedgerPrepaymentTransactionEntry = new GeneralLedgerPrePaymentTransaction(nextKeyString)
                        {
                            SupplierCode = purchaseLedgerEntry.SupplierCode,
                            GeneralLedgerCodes = new string[] { glc },
                            AmountTotal = purchaseLedgerEntry.Amounts.ElementAt(i),
                            DocumentNumber = purchaseLedgerEntry.DocumentReference,
                            TransactionType = "INV",
                            DocumentDT = purchaseLedgerEntry.DocumentDT,
                            DueDT = purchaseLedgerEntry.InvoiceDueDT,
                            StartPeriod = purchaseLedgerEntry.PrepaymentStartPeriods.ElementAt(i),
                            NumberOfPeriods = purchaseLedgerEntry.PrepaymentNumberOfPeriods.ElementAt(i),
                            SplitEqually = purchaseLedgerEntry.PrepaymentsSplitEqually.ElementAt(i),
                            SplitAmounts = prePaymentAmountSplits?.Split(Framework.SubValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries).Select(a => Convert.ToDecimal(a)),
                            SplitMemos = prePaymentMemoSplits?.Split(Framework.SubValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries),
                            Memo = prePaymentMemo ?? otherPrePaymentMemo,
                        };

                        generalLedgerPrepaymentTransactionEntries.Add(generalLedgerPrepaymentTransactionEntry);
                    }
                }

                foreach(var glppt in generalLedgerPrepaymentTransactionEntries)
                {
                    var glpptJson = JsonConvert.SerializeObject(glppt);

                    // 15. INSERT into GLPPTRANS.DES

                    results.Add(new PurchaseLedgerInsertionResult(
                        fileKey: glppt.Key,
                        fileName: "GLPPTRANS.DES",
                        operationsPerformed: new[] { "INSERT" },
                        jsonBefore: null,
                        jsonAfterwards: glpptJson,
                        purchaseLedgerRegistryKey: registerEntry.Key
                    ));

                    GeneralLedgerPrePaymentTransactionFunctions.UpsertViaTransaction(ref session, glpptJson);
                }

                #endregion

                transaction.Commit();
            }

            return new ArrayList(results);
        }

        public static void PurchaseLedger_InsertFromInvoiceRegistry_FillRow(
            object obj,
            out SqlString PurchaseLedgerInvoiceRegistryKey,
            out SqlString FileKey,
            out SqlString FileName,
            out SqlString OperationsPerformed,
            out SqlString JSONBefore,
            out SqlString JSONAfterwards
            )
        {
            var p = (PurchaseLedgerInsertionResult)obj;

            FileKey = p.FileKey;

            FileName = p.FileName;

            OperationsPerformed = p.OperationsPerformed?.Any() == true ? JsonConvert.SerializeObject(p.OperationsPerformed) : SqlString.Null;

            JSONBefore = p.JSONBefore ?? SqlString.Null;

            JSONAfterwards = p.JSONAfterwards ?? SqlString.Null;

            PurchaseLedgerInvoiceRegistryKey = p.PurchaseLedgerRegistryKey;
        }
    }
}
