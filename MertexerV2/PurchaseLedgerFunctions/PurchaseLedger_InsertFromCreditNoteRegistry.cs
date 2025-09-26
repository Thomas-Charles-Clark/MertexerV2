/*
 * 
 *  ABANDONED
 *  Because I don't know what should fill fields <123> and <151>, <223> and <231> on TPLEDG with, I can't 
 *  These fields have *stupid* names like RF134 and descriptions such as Zaplcashall. Useless.
 *  However, some of them are then actually used in PCRNOIR.CUR.DES!
 *  Without knowing what to write to them, I can't read anything useful from them with 
 *  which to enact the Q-Pointer stages of PCRNOIR.CUR.DES such as TPLTRANS1 and TPLTRANS.CUR1.
 *  
 *  There's no data in those fields on TPLEDG at present, however there are TPLTRANS records with STATUS
 *  which is determined by a Default process which... you guessed it... reads some of these insanely named fields.
 * 
 */




//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Data.SqlTypes;
//using System.Linq;

//using MertexerV2.CompanyFileFunctions;
//using MertexerV2.GeneralFunctions;
//using MertexerV2.SupplierFunctions;
//using MertexerV2.GeneralLedgerFunctions;
//using MertexerV2.Models.Company;
//using MertexerV2.Models.GeneralLedger;
//using MertexerV2.Models.PurchaseLedger;
//using MertexerV2.Utilities;

//using Microsoft.SqlServer.Server;

//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

//namespace MertexerV2.PurchaseLedgerFunctions
//{
//    public static partial class PurchaseLedgerFunctions
//    {
//        [Microsoft.SqlServer.Server.SqlFunction(
//            DataAccess = DataAccessKind.Read,
//            FillRowMethodName = "PurchaseLedger_InsertFromCreditNoteRegistry_FillRow")]
//        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
//        public static IEnumerable InsertFromCreditNoteRegistry(string UniVerseAccount, string PurchaseLedgerCreditNoteRegistryJSON)
//        {
//            var results = new List<PurchaseLedgerInsertionResult>();

//            var now = DateTime.Now;

//            var registerEntryJson = PurchaseLedgerCreditNoteRegistryJSON;

//            var registerEntryJObj = JObject.Parse(registerEntryJson);

//            var registerEntry = registerEntryJObj["creditNoteRegisterEntry"].ToObject<PurchaseLedgerCreditNoteRegister>();

//            var vatableAmountsInCurrency = registerEntryJObj["vatEntries"].Children()["vatableAmountCurrency"].Values<decimal>().ToArray();

//            var vatTaxesInCurrency = registerEntryJObj["vatEntries"].Children()["vatAmountCurrency"].Values<decimal>().ToArray();

//            using (var qe = new QueryExecutor(UniVerseAccount))
//            {
//                var session = qe.Session;

//                var transaction = session.CreateUniTransaction();

//                transaction.Begin();

//                #region Properties & Initialisation

//                #region Keys

//                // Read the most recently held TPLEDG CRN key from PLCONTROL<8> WITH @ID = "GENNO"

//                var purchaseLedgerControlFileFieldPosition = 8;

//                var purchaseLedgerControlFile = session.CreateUniFile("PLCONTROL");

//                var numberGeneratorRecords = purchaseLedgerControlFile.ReadRecords(new[] { "GENNO" });

//                var numberGeneratorRecord = numberGeneratorRecords.GetRecord(0);

//                var numberField = numberGeneratorRecord.Record.Extract(purchaseLedgerControlFileFieldPosition); // Field 9 on the record "GENNO"

//                var currentKey = Convert.ToInt32(numberField.StringValue);

//                var newKey = currentKey += 1;

//                numberGeneratorRecord.Record.Replace(purchaseLedgerControlFileFieldPosition, $"{newKey}");

//                purchaseLedgerControlFile.WriteRecords(numberGeneratorRecords);

//                var purchaseLedgerKey = $"PN{newKey.ToString().PadLeft(5, '0')}";

//                #endregion

//                #region Other Files and Errata

//                var companyEntry = Selector<Company>.Select("1", true, false, 1, UniVerseAccount);

//                var generalLedgerPrePaymentCodes = companyEntry.PrepaymentGeneralLedgerCodes.ToList();

//                var companyStatsEntry = Selector<CompanyStats>.Select("1", true, false, 1, UniVerseAccount);

//                var supplierEntry = Selector<Models.Supplier.Supplier>.Select(registerEntry.SupplierCode, true, false, 1, UniVerseAccount);

//                var supplierCurrencyEntry = Selector<Models.Supplier.SupplierCurrency>.Select(registerEntry.SupplierCode, true, false, 1, UniVerseAccount);

//                var companyVATEntries = Selector<CompanyVAT>.Select(registerEntry.VATCodes, true, false, 1, UniVerseAccount);

//                var currentPeriod = companyEntry.ThisPurchaseLedgerPeriod ?? companyEntry.ThisGeneralLedgerPeriod;

//                var purchaseLedgerTransactionPeriod = currentPeriod;

//                var prePaymentPurchaseLedgerCodes = companyEntry.PrepaymentGeneralLedgerCodes.ToList();

//                #endregion

//                #endregion

//                #region Step 01. Create a Purchase Ledger entry (INSERT to TPLEDG)

//                var purchaseLedgerEntry = new PurchaseLedger(purchaseLedgerKey)
//                {
//                    DocumentNumber = $"{newKey}",                    // Field 34 DOCNO is set to the ledger key without the PI prefix
//                    DocumentReference = purchaseLedgerKey,           // Field 53 PINVDOC is set to the @ID of the TPLEDG record
//                    PurchaseLedgerRegisterKey = registerEntry.Key,   // Field 199 PLINVREG is set to the registry entry's key
//                    SourceTransactionDefinition = "PCRNOIR.CUR.DES", // Not described by the dictionary, but reading the data tells us it should be set accordingly.
//                    DocumentDT = registerEntry.DocumentDT,           // Field 33 DOCDATE is set to the document date as declared on the register entry.
//                    OnFileDT = registerEntry.OnFileDT,               // Field 36 DATEONFILE is set to the date... that the... registry entry was created? Deosn't sound right.
//                                                                     // Should that not be "DateTime.Now"? NO: Determined by cross referencing a TPLEDG record with a PLREGISTER.HIST record
//                                                                     // the PLREGISTER.HIST record retains the date the register entry was created; this matches the DATEONFILE for the given TPLEDG rec.
//                                                                     // POSTDATE is the one we want to set to DateTime.Now.
//                    SupplierInvoiceReference = registerEntry.ItemReference, // Field 37 SUPINVNO is carried in registerEntry.ItemReference.
//                    InvoiceDueDT = registerEntry.DueDT,              // Field 38 INVDUE is carried in registerEntry.DueDT
//                    SupplierCode = registerEntry.SupplierCode,       // Field 39 PSUPREF is carried on registerEntry.SupplierCode
//                    Amounts = registerEntry.AmountsExVAT,
//                    GeneralLedgerCodes = registerEntry.GeneralLedgerCodes,
//                    VATCodes = registerEntry.VATCodes,
//                    VATableValues = registerEntry.VATableValues,
//                    TotalDueInSterling = registerEntry.AmountSterling,
//                    SettlementTerms = registerEntry.PaymentTermsCode,
//                    PostDT = now,                                    // See comments above
//                    SettlementDueDTs = new List<DateTime?>() { registerEntry.SettlementDT }, // Fields 55 and 55,1 are both being set here, which is a bit redundant.
//                    DueDT = registerEntry.SettlementDT,                                      // I've decided to do this simply because if we use purchaseLedgerEntry.DueDT
//                                                                                             // later it's a cleaner way of pulling it, instead of asking for SettlementDTs.First()
//                    Memo = registerEntry.Memo,
//                    VATBalance = registerEntry.VATBalance,
//                    VATTaxes = registerEntry.VATTaxes,
//                    VATTaxesInCurrency = vatTaxesInCurrency,
//                    DocumentBalance = registerEntry.DocumentBalance,
//                    DiscountAmount = 0.0m, // we don't do discounts here: LIST PLTERMS DISC
//                    DiscountAmountInCurrency = 0.0M, // ""
//                    OtherMemos = registerEntry.OtherMemos,
//                    DisputeIndicator = registerEntry.Disputed,
//                    DisputeMemo = registerEntry.DisputeMemo,
//                    ReceivedDT = registerEntry.ReceivedDT,
//                    PassedTo = registerEntry.PassedTo,
//                    PassedToDT = registerEntry.PassedToDT,
//                    DiscountPercentage = registerEntry.DiscountPercentage ?? 0.0m,
//                    CurrencyCode = registerEntry.MertexCurrencyCode,
//                    ExchangeRates = new List<decimal>() { registerEntry.ExchangeRate },
//                    AmountInCurrency = registerEntry.AmountCurrency,
//                    UnallocatedCreditNoteAmountInCurrency = registerEntry.AmountCurrency,
//                    // Prognostication: is this for some future period to worry about, or is it in the current period?
//                    // All I can do is compare the Invoice Date (DocumentDT) with the current Month.
//                    Future = registerEntry.DocumentDT?.Month > now.Month
//                };

//                var purchaseLedgerEntryJson = JsonConvert.SerializeObject(purchaseLedgerEntry);

//                PurchaseLedgerFunctions.InsertViaTransaction(ref session, purchaseLedgerEntryJson);

//                results.Add(new PurchaseLedgerInsertionResult(
//                    fileKey: purchaseLedgerEntry.Key,
//                    fileName: "TPLEDG",
//                    operationsPerformed: new[] { "INSERT" },
//                    jsonBefore: null,
//                    jsonAfterwards: purchaseLedgerEntryJson,
//                    purchaseLedgerRegistryKey: registerEntry.Key
//                ));

//                if (purchaseLedgerEntry.Future == true)
//                {
//                    purchaseLedgerTransactionPeriod += 1;

//                    if (currentPeriod > companyEntry.NumberOfFinancialPeriods)
//                    {
//                        purchaseLedgerTransactionPeriod = 1;
//                    }
//                }

//                #endregion

//                #region Step 02. Create a Purchase Ledger Register History entry (INSERT to PLREGISTER.HIST)

//                var purchaseLedgerRegisterHistoryEntry = new PurchaseLedgerRegisterHistory($"CRN*{purchaseLedgerEntry.PurchaseLedgerRegisterKey}")
//                {
//                    RegisterNumber = purchaseLedgerEntry.PurchaseLedgerRegisterKey,
//                    OnFileDT = purchaseLedgerEntry.OnFileDT,
//                    SupplierCode = purchaseLedgerEntry.SupplierCode,
//                    ConvertedDT = now,
//                    DocumentReference = purchaseLedgerKey,
//                    UserID = "BOT",
//                    TransactionType = "CRN",
//                    DeletedDT = null,
//                    DocumentTotal = purchaseLedgerEntry.DocumentBalance
//                };

//                var purchaseLedgerRegisterHistoryEntryJson = JsonConvert.SerializeObject(purchaseLedgerRegisterHistoryEntry);

//                PurchaseLedgerRegisterHistoryFunctions.InsertViaTransaction(ref session, purchaseLedgerRegisterHistoryEntryJson);

//                results.Add(new PurchaseLedgerInsertionResult(
//                    fileKey: purchaseLedgerRegisterHistoryEntry.Key,
//                    fileName: "PLREGISTER.HIST",
//                    operationsPerformed: new[] { "INSERT" },
//                    jsonBefore:null,
//                    jsonAfterwards: purchaseLedgerRegisterHistoryEntryJson,
//                    purchaseLedgerRegistryKey: registerEntry.Key
//                ));

//                #endregion

//                #region Step 03. Delete the given Purchase Ledger Invoice Register entry (DELETE from PLCRNREG)

//                var purchaseRegisterEntryJson = JsonConvert.SerializeObject(registerEntry);

//                PurchaseLedgerCreditNoteRegisterFunctions.DeleteViaTransaction(ref session, JsonConvert.SerializeObject(registerEntry));

//                results.Add(new PurchaseLedgerInsertionResult(
//                    fileKey: registerEntry.Key,
//                    fileName: "PLCRNREG",
//                    operationsPerformed: new[] { "DELETE" },
//                    jsonBefore: purchaseRegisterEntryJson,
//                    jsonAfterwards: null,
//                    purchaseLedgerRegistryKey: registerEntry.Key
//                ));

//                #endregion

//                #region Step 04. Create a Purchase Ledger Transaction entry (INSERT into TPLTRANS)

//                // Key == TPLEDG<53> "Document Reference"
//                var purchaseLedgerTransactionEntry = new PurchaseLedgerTransaction(purchaseLedgerEntry.DocumentReference)
//                {
//                    DocumentDT = purchaseLedgerEntry.DocumentDT,                        // <33>
//                    TransactionType = "CRN",                                            // "CRN"
//                    DocumentNumber = purchaseLedgerEntry.DocumentNumber,                // <34>
//                    ItemReference = purchaseLedgerEntry.SupplierInvoiceReference,       // <37>
//                    VATTaxesTotal = 0 - purchaseLedgerEntry.VATTaxes.Sum(),             // 0 - S(<48>)
//                    Debit = purchaseLedgerEntry.Amounts.Sum(),                          // S(<43>)
//                    Period = purchaseLedgerTransactionPeriod,                           // B("PL.B.PERIOD")
//                    SupplierReference = purchaseLedgerEntry.SupplierCode,               // <39>
//                    OriginalValueABS = purchaseLedgerEntry.TotalDueInSterling,          // <50>
//                    Unallocated = purchaseLedgerEntry.CashOnAccount ?? 0.0m,            // <64>
//                    Balance = 0.0m,                                                     // "0"
//                    PostDT = purchaseLedgerEntry.PostDT,                                // <35>
//                    CurrencyCode = purchaseLedgerEntry.CurrencyCode,                    // <161> -- Why this isn't entered for the invoice registration I don't know, but it isn't
//                    ExchangeRates = purchaseLedgerEntry.ExchangeRates,                  // <162>
//                    ExchangeRateDifferences = purchaseLedgerEntry.ExchangeRateDifferences, // <164>
//                    PurchaseLedgerRegisterNumber = $"CRN*{purchaseLedgerEntry.PurchaseLedgerRegisterKey}" // "CRN*":<199>
//                };

//                var purchaseLedgerTransactionEntryJson = JsonConvert.SerializeObject(purchaseLedgerTransactionEntry);

//                PurchaseLedgerTransactionFunctions.UpsertViaTransaction(ref session, purchaseLedgerTransactionEntryJson);

//                results.Add(new PurchaseLedgerInsertionResult(
//                    fileKey: purchaseLedgerTransactionEntry.Key,
//                    fileName:  "TPLTRANS",
//                    operationsPerformed: new [] { "INSERT" },
//                    jsonBefore: null,
//                    jsonAfterwards: purchaseLedgerTransactionEntryJson,
//                    purchaseLedgerRegistryKey: registerEntry.Key
//                ));

//                #endregion

//                #region Step 05. Create a Purchase Ledger Transaction Currency entry (INSERT into TPLTRANS.CUR)

//                // Key == TPLEDG<53> "Document Reference"
//                var purchaseLedgerTransactionCurrencyEntry = new PurchaseLedgerTransactionCurrency(purchaseLedgerEntry.DocumentReference)
//                {
//                    DocumentDT = purchaseLedgerEntry.DocumentDT,                        // <33>
//                    TransactionType = "CRN",                                            // "CRN"
//                    DocumentNumber = purchaseLedgerEntry.DocumentNumber,                // <34>
//                    ItemReference = purchaseLedgerEntry.SupplierInvoiceReference,       // <37>
//                    VATTaxesTotal = 0 - purchaseLedgerEntry.VATTaxesInCurrency.Sum(),   // 0 - S(<248>)
//                    Debit = purchaseLedgerEntry.AmountInCurrency - purchaseLedgerEntry.VATTaxesInCurrency.Sum(),    // <250> - S(<248>)
//                    Period = purchaseLedgerTransactionPeriod,                           // B("PL.B.PERIOD")
//                    SupplierReference = purchaseLedgerEntry.SupplierCode,               // <39>
//                    OriginalValueABS = purchaseLedgerEntry.AmountInCurrency,            // <250>
//                    Unallocated = purchaseLedgerEntry.UnallocatedAmountInCurrency,      // <264>
//                    Balance = 0.0m,                                                     // "0"
//                    PostDT = purchaseLedgerEntry.PostDT,                                // <35>
//                    CurrencyCode = purchaseLedgerEntry.CurrencyCode,                    // <161>
//                    ExchangeRates = purchaseLedgerEntry.ExchangeRates,                  // <162>
//                    ExchangeRateDifferences = purchaseLedgerEntry.ExchangeRateDifferencesInCurrency, // <234>
//                    PurchaseLedgerRegisterNumber = $"CRN*{purchaseLedgerEntry.PurchaseLedgerRegisterKey}", // "CRN*":<199>
//                    //Statuses = 
//                    //
//                };

//                throw new Exception(@"Because I don't know what should fill fields <123> and <151>, <223> and <231> on TPLEDG with, I can't 
//reasonably continue. There's a STATUS field on TPLTRANS which is updated via the PCRNOIR.CUR.DES transaction (via calling the Default definition ""DINVRST"") which relies on 
//these fields having sane values in order to determine PL or CL, whatever they each mean.");

//                var purchaseLedgerTransactionCurrencyEntryJson = JsonConvert.SerializeObject(purchaseLedgerTransactionCurrencyEntry);

//                PurchaseLedgerTransactionCurrencyFunctions.UpsertViaTransaction(ref session, purchaseLedgerTransactionCurrencyEntryJson);

//                results.Add(new PurchaseLedgerInsertionResult(
//                    fileKey: purchaseLedgerTransactionCurrencyEntry.Key,
//                    fileName: "TPLTRANS.CUR",
//                    operationsPerformed: new[] { "INSERT" },
//                    jsonBefore: null,
//                    jsonAfterwards: purchaseLedgerTransactionCurrencyEntryJson,
//                    purchaseLedgerRegistryKey: registerEntry.Key
//                ));

//                #endregion

//                #region Step 06. Create a Purchase Ledger Transaction 1 Entry (INSERT into TPLTRANS1)



//                #endregion

//                #region Step 07. Create a Purchase Ledger Transaction Currency 1 Entry (INSERT into TPLTRANS1.CUR)



//                #endregion

//                #region Step 08. Update the relevant Supplier record (UPDATE to PLSUP)

//                var supplierEntryBeforeModificationJSON = JsonConvert.SerializeObject(supplierEntry);

//                supplierEntry.Balance += purchaseLedgerEntry.DocumentBalance;

//                var supplierEntryNextPeriodBalance = CalculatePurchaseLedgerAmount(
//                    amounts: new[] { purchaseLedgerEntry.TotalDueInSterling }, // <50> "CHQTOTAL/DTOTAL/RDTOTAL/STGAMT/STGCHQTOTAL/TOTDUE"
//                                                                               // the subroutine itself explicitly returns early on line 40 if you're in the final period: IF PER=NOPER THEN RETURN        
//                    currentPeriod: currentPeriod,
//                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
//                    operationPeriod: "N",
//                    future: purchaseLedgerEntry.Future == true,
//                    mvOperationType: null
//                );

//                supplierEntry.NextPeriodBalance += supplierEntryNextPeriodBalance; // PLAMOUNT,N50

//                var supplierEntryThisPeriodBalance = CalculatePurchaseLedgerAmount(
//                    amounts: new[] { purchaseLedgerEntry.TotalDueInSterling },
//                    currentPeriod: currentPeriod,
//                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
//                    operationPeriod: null,
//                    future: purchaseLedgerEntry.Future == true,
//                    mvOperationType: null
//                );

//                supplierEntry.ThisPeriodBalance += supplierEntryThisPeriodBalance; // PLAMOUNT,50

//                supplierEntry.DocumentReferences = supplierEntry.DocumentReferences.Prepend(purchaseLedgerEntry.Key);

//                var supplierEntryPurchasesNextPeriod = CalculatePurchaseLedgerAmount(
//                    amounts: purchaseLedgerEntry.Amounts, // <43> "PAMOUNT"
//                    currentPeriod: currentPeriod,
//                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
//                    operationPeriod: "N",
//                    future: purchaseLedgerEntry.Future == true,
//                    mvOperationType: "A"
//                );

//                supplierEntry.PurchasesNextPeriod += supplierEntryPurchasesNextPeriod; // PLAMOUNT,N43.A

//                var supplierEntryPurchasesThisPeriod = CalculatePurchaseLedgerAmount(
//                    amounts: purchaseLedgerEntry.Amounts,
//                    currentPeriod: currentPeriod,
//                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
//                    operationPeriod: null,
//                    future: purchaseLedgerEntry.Future == true,
//                    mvOperationType: "A"
//                );

//                supplierEntry.PurchasesThisPeriod += supplierEntryPurchasesThisPeriod; // PLAMOUNT,43.A

//                var supplierEntryPurchasesYearToDate = CalculatePurchaseLedgerAmount(
//                    amounts: purchaseLedgerEntry.Amounts,
//                    currentPeriod: currentPeriod,
//                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
//                    operationPeriod: "Y",
//                    future: purchaseLedgerEntry.Future == true,
//                    mvOperationType: "A"
//                );

//                supplierEntry.PurchasesYearToDate += supplierEntryPurchasesYearToDate; // PLAMOUNT,Y43.A

//                // P("INACT")
//                supplierEntry.PeriodsInactive = purchaseLedgerEntry.Future == true ? null : (sbyte?)-1;

//                var supplierEntryJson = JsonConvert.SerializeObject(supplierEntry);

//                SupplierFunctions.SupplierFunctions.UpsertViaTransaction(ref session, supplierEntryJson);

//                results.Add(new PurchaseLedgerInsertionResult(
//                    purchaseLedgerRegistryKey: registerEntry.Key,
//                    fileKey: supplierEntry.Key,
//                    fileName: "PLSUP",
//                    operationsPerformed: new[] {
//                        $"<15> \"BALANCE\" (Balance) += {purchaseLedgerEntry.DocumentBalance}",
//                        $"<18,1> \"NEXT\" (NextPeriodBalance) += {supplierEntryNextPeriodBalance}",
//                        $"<18,2> \"CURR\" (ThisPeriodBalance) += {supplierEntryThisPeriodBalance}",
//                        $"<19> \"TLINK\" += \"{purchaseLedgerEntry.Key}\"",
//                        $"<21,1> \"PNEXT\" (PurchasesNextPeriod) += {supplierEntryPurchasesNextPeriod}",
//                        $"<21,2> \"PCURR\" (PurchasesThisPeriod) += {supplierEntryPurchasesThisPeriod}",
//                        $"<21,3> \"PYTD\" (PurchasesYearToDate) += {supplierEntryPurchasesYearToDate}",
//                        $"<23> \"INACT\" (PeriodsInactive) = \"{supplierEntry.PeriodsInactive}\""
//                    },
//                    jsonBefore: supplierEntryBeforeModificationJSON,
//                    jsonAfterwards: supplierEntryJson
//                ));

//                #endregion

//                #region Step 09. Update the relevant Supplier Currency record (UPDATE to PLSUP.CUR)

//                var supplierCurrencyEntryBeforeModificationJSON = JsonConvert.SerializeObject(supplierCurrencyEntry);

//                /* Update the Supplier Currency record */

//                var supplierCurrencyEntryBalanceAddition = purchaseLedgerEntry.AmountInCurrency;

//                supplierCurrencyEntry.Balance += supplierCurrencyEntryBalanceAddition; // <250> "CURAMT"

//                var supplierCurrencyEntryNextPeriodBalanceAddition = CalculatePurchaseLedgerAmount(
//                    amounts: new[] { purchaseLedgerEntry.AmountInCurrency }, // <250> "CURAMT"
//                                                                             // the subroutine itself explicitly returns early on line 40 if you're in the final period: IF PER=NOPER THEN RETURN        
//                    currentPeriod: currentPeriod,
//                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
//                    operationPeriod: "N",
//                    future: purchaseLedgerEntry.Future == true,
//                    mvOperationType: null
//                );

//                supplierCurrencyEntry.NextPeriodBalance += supplierCurrencyEntryNextPeriodBalanceAddition; // PLAMOUNT,N250

//                var supplierCurrencyEntryThisPeriodBalanceAddition = CalculatePurchaseLedgerAmount(
//                    amounts: new[] { purchaseLedgerEntry.AmountInCurrency }, // <250> "CURAMT"
//                    currentPeriod: currentPeriod,
//                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
//                    operationPeriod: null,
//                    future: purchaseLedgerEntry.Future == true,
//                    mvOperationType: null
//                );

//                supplierCurrencyEntry.ThisPeriodBalance += supplierCurrencyEntryThisPeriodBalanceAddition; // PLAMOUNT,250

//                supplierCurrencyEntry.DocumentReferences = supplierCurrencyEntry.DocumentReferences.Prepend(purchaseLedgerEntry.Key);

//                var supplierCurrencyEntryPurchasesNextPeriodAddition = CalculatePurchaseLedgerAmount(
//                    amounts: new[] { purchaseLedgerEntry.AmountInCurrency }, // <250> "CURAMT"
//                    currentPeriod: currentPeriod,
//                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
//                    operationPeriod: "N",
//                    future: purchaseLedgerEntry.Future == true,
//                    mvOperationType: null
//                );

//                supplierCurrencyEntry.PurchasesNextPeriod += supplierCurrencyEntryPurchasesNextPeriodAddition; // PLAMOUNT,N250

//                var supplierCurrencyEntryPurchasesNextPeriodReduction = CalculatePurchaseLedgerAmount(
//                    amounts: purchaseLedgerEntry.VATTaxesInCurrency, // <248> "VATTAX.CUR"
//                    currentPeriod: currentPeriod,
//                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
//                    operationPeriod: "N",
//                    future: purchaseLedgerEntry.Future == true,
//                    mvOperationType: "A"
//                );

//                supplierCurrencyEntry.PurchasesNextPeriod -= supplierCurrencyEntryPurchasesNextPeriodReduction; // PLAMOUNT,N248.A

//                var supplierCurrencyEntryPurchasesThisPeriodAddition = CalculatePurchaseLedgerAmount(
//                    amounts: new[] { purchaseLedgerEntry.AmountInCurrency }, // <250> "CURAMT"
//                    currentPeriod: currentPeriod,
//                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
//                    operationPeriod: null,
//                    future: purchaseLedgerEntry.Future == true,
//                    mvOperationType: null
//                );

//                supplierCurrencyEntry.PurchasesThisPeriod += supplierCurrencyEntryPurchasesThisPeriodAddition; // PLAMOUNT,250

//                var supplierCurrencyEntryPurchasesThisPeriodReduction = CalculatePurchaseLedgerAmount(
//                    amounts: purchaseLedgerEntry.VATTaxesInCurrency, // <248> "VATTAX.CUR"
//                    currentPeriod: currentPeriod,
//                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
//                    operationPeriod: null,
//                    future: purchaseLedgerEntry.Future == true,
//                    mvOperationType: "A"
//                );

//                supplierCurrencyEntry.PurchasesThisPeriod -= supplierCurrencyEntryPurchasesThisPeriodReduction; // PLAMOUNT,248.A

//                var supplierCurrencyEntryPurchasesYearToDateAddition = CalculatePurchaseLedgerAmount(
//                    amounts: new[] { purchaseLedgerEntry.AmountInCurrency }, // <250> "CURAMT"
//                    currentPeriod: currentPeriod,
//                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
//                    operationPeriod: "Y",
//                    future: purchaseLedgerEntry.Future == true,
//                    mvOperationType: null
//                );

//                supplierCurrencyEntry.PurchasesYearToDate += supplierCurrencyEntryPurchasesYearToDateAddition; // PLAMOUNT,Y250

//                var supplierCurrencyEntryPurchasesYearToDateReduction = CalculatePurchaseLedgerAmount(
//                    amounts: purchaseLedgerEntry.VATTaxesInCurrency, // <248> "VATTAX.CUR"
//                    currentPeriod: currentPeriod,
//                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
//                    operationPeriod: "Y",
//                    future: purchaseLedgerEntry.Future == true,
//                    mvOperationType: "A"
//                );

//                supplierCurrencyEntry.PurchasesYearToDate -= supplierCurrencyEntryPurchasesYearToDateReduction; // PLAMOUNT,Y248.A

//                supplierCurrencyEntry.PeriodsInactive = purchaseLedgerEntry.Future == true ? null : (sbyte?)-1;



//                // 07. UPDATE PLSUP

//                var supplierCurrencyEntryJson = JsonConvert.SerializeObject(supplierCurrencyEntry);

//                SupplierCurrencyFunctions.UpsertViaTransaction(ref session, supplierCurrencyEntryJson);

//                results.Add(new PurchaseLedgerInsertionResult(
//                    purchaseLedgerRegistryKey: registerEntry.Key,
//                    fileKey: supplierEntry.Key,
//                    fileName: "PLSUP.CUR",
//                    operationsPerformed: new[] {
//                        $"<15> \"BALANCE\" (Balance) += {supplierCurrencyEntryBalanceAddition}",
//                        $"<18,1> \"NEXT\" (NextPeriodBalance) += {supplierCurrencyEntryNextPeriodBalanceAddition}",
//                        $"<18,2> \"CURR\" (ThisPeriodBalance) += {supplierCurrencyEntryThisPeriodBalanceAddition}",
//                        $"<19> \"TLINK\" += \"{purchaseLedgerEntry.Key}\"",
//                        $"<21,1> \"PNEXT\" (PurchasesNextPeriod) += {supplierCurrencyEntryPurchasesNextPeriodAddition}",
//                        $"<21,1> \"PNEXT\" (PurchasesNextPeriod) -= {supplierCurrencyEntryPurchasesNextPeriodReduction}",
//                        $"<21,2> \"PCURR\" (PurchasesThisPeriod) += {supplierCurrencyEntryPurchasesThisPeriodAddition}",
//                        $"<21,2> \"PCURR\" (PurchasesThisPeriod) -= {supplierCurrencyEntryPurchasesThisPeriodReduction}",
//                        $"<21,3> \"PYTD\" (PurchasesYearToDate) += {supplierCurrencyEntryPurchasesYearToDateAddition}",
//                        $"<21,3> \"PYTD\" (PurchasesYearToDate) -= {supplierCurrencyEntryPurchasesYearToDateReduction}",
//                        $"<23> \"INACT\" (PeriodsInactive) = \"{supplierCurrencyEntry.PeriodsInactive}\""
//                    },
//                    jsonBefore: supplierCurrencyEntryBeforeModificationJSON,
//                    jsonAfterwards: supplierCurrencyEntryJson
//                ));

//                #endregion

//                #region Step 10. Update the relevant CFVAT record(s), for each of the VAT codes declared on the given invoice registry entry (UPDATE(s) to CFVAT)

//                foreach (var vatEntry in companyVATEntries)
//                {
//                    var vatEntryBeforeModificationJSON = JsonConvert.SerializeObject(companyVATEntries);

//                    var purchasesInvoicesTaxAddition = CalculatePurchaseLedgerAmount(
//                        amounts: purchaseLedgerEntry.VATTaxes, // <48> "VATTAX"
//                        currentPeriod: currentPeriod,
//                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
//                        operationPeriod: null,
//                        future: purchaseLedgerEntry.Future == true,
//                        mvOperationType: "X"
//                    );

//                    vatEntry.PurchasesInvoicesTax += purchasesInvoicesTaxAddition; // PLAMOUNT,48X

//                    var purchasesInvoicesValueAddition = CalculatePurchaseLedgerAmount(
//                        amounts: purchaseLedgerEntry.VATableValues, // <49> "VATVALUE"
//                        currentPeriod: currentPeriod,
//                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
//                        operationPeriod: null,
//                        future: purchaseLedgerEntry.Future == true,
//                        mvOperationType: "X"
//                    );

//                    vatEntry.PurchasesInvoicesValue += purchasesInvoicesValueAddition; // PLAMOUNT,48X

//                    var purchasesQuarterTaxTotalAddition = CalculatePurchaseLedgerAmount(
//                        amounts: purchaseLedgerEntry.VATTaxes, // <48> "VATTAX"
//                        currentPeriod: currentPeriod,
//                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
//                        operationPeriod: null,
//                        future: purchaseLedgerEntry.Future == true,
//                        mvOperationType: "X"
//                    );

//                    vatEntry.PurchasesQuarterTaxTotal += purchasesQuarterTaxTotalAddition; // PLAMOUNT,48X

//                    var purchasesQuarterValueTotalAddition = CalculatePurchaseLedgerAmount(
//                        amounts: purchaseLedgerEntry.VATableValues, // <49> "VATVALUE"
//                        currentPeriod: currentPeriod,
//                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
//                        operationPeriod: null,
//                        future: purchaseLedgerEntry.Future == true,
//                        mvOperationType: "X"
//                    );

//                    vatEntry.PurchasesQuarterValueTotal += purchasesQuarterValueTotalAddition; // PLAMOUNT,48X

//                    var purchasesNextPeriodInvoicesTaxAddition = CalculatePurchaseLedgerAmount(
//                        amounts: purchaseLedgerEntry.VATTaxes, // <48> "VATTAX"
//                        currentPeriod: currentPeriod,
//                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
//                        operationPeriod: "N",
//                        future: purchaseLedgerEntry.Future == true,
//                        mvOperationType: "X"
//                    );

//                    vatEntry.PurchasesNextPeriodInvoicesTax += purchasesNextPeriodInvoicesTaxAddition; // PLAMOUNT,N48X

//                    var purchasesNextPeriodInvoicesValueAddition = CalculatePurchaseLedgerAmount(
//                        amounts: purchaseLedgerEntry.VATableValues, // <49> "VATVALUE"
//                        currentPeriod: currentPeriod,
//                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
//                        operationPeriod: "N",
//                        future: purchaseLedgerEntry.Future == true,
//                        mvOperationType: "X"
//                    );

//                    vatEntry.PurchasesNextPeriodInvoicesValue += purchasesNextPeriodInvoicesValueAddition; // PLAMOUNT,N48X

//                    var vatEntryJson = JsonConvert.SerializeObject(vatEntry);

//                    CompanyVATFunctions.UpsertViaTransaction(ref session, vatEntryJson);

//                    results.Add(new PurchaseLedgerInsertionResult(
//                        purchaseLedgerRegistryKey: registerEntry.Key,
//                        fileKey: vatEntry.Key,
//                        fileName: "CFVAT",
//                        operationsPerformed: new[] {
//                            $"<30> \"PINVT\" (PurchasesInvoicesTax) += {purchasesInvoicesTaxAddition}",
//                            $"<34> \"PINVV\" (PurchasesInvoicesValue) += {purchasesInvoicesValueAddition}",
//                            $"<38> \"PQTTOT\" (PurchasesQuarterTaxTotal) += {purchasesQuarterTaxTotalAddition}",
//                            $"<39> \"PQVTOT\" (PurchasesQuarterValueTotal) += {purchasesQuarterValueTotalAddition}",
//                            $"<40> \"PNINVT\" (PurchasesNextPeriodInvoicesTax) += {purchasesNextPeriodInvoicesTaxAddition}",
//                            $"<44> \"PNINVV\" (PurchasesNextPeriodInvoicesValue) += {purchasesNextPeriodInvoicesValueAddition}", },
//                        jsonBefore: vatEntryBeforeModificationJSON,
//                        jsonAfterwards: vatEntryJson
//                    ));
//                }

//                #endregion

//                #region Step 11. Create a Purchase Ledger VAT Audit entry (INSERT to PLVATAUD)

//                /* Insert the PurchaseLedgerVATAudit record */

//                var purchaseLedgerVATAuditEntry = new PurchaseLedgerVATAudit(purchaseLedgerEntry.DocumentReference)
//                {
//                    DocumentDT = purchaseLedgerEntry.DocumentDT,
//                    TransactionType = "INV",
//                    SupplierCode = purchaseLedgerEntry.SupplierCode,
//                    PurchaseLedgerTotalDueInSterling = purchaseLedgerEntry.TotalDueInSterling,
//                    VATCodes = purchaseLedgerEntry.VATCodes,
//                    VATableValuesInSterling = purchaseLedgerEntry.VATableValues,
//                    VATTaxesInSterling = purchaseLedgerEntry.VATTaxes,
//                    NextPeriod = purchaseLedgerEntry.Future == true
//                };

//                var purchaseLedgerVATAuditEntryJson = JsonConvert.SerializeObject(purchaseLedgerVATAuditEntry);

//                PurchaseLedgerVATAuditFunctions.UpsertViaTransaction(ref session, purchaseLedgerVATAuditEntryJson);

//                results.Add(new PurchaseLedgerInsertionResult(
//                    purchaseLedgerRegistryKey: registerEntry.Key,
//                    fileKey: purchaseLedgerVATAuditEntry.Key,
//                    fileName: "PLVATAUD",
//                    operationsPerformed: new[] { "INSERT" },
//                    jsonBefore: null,
//                    jsonAfterwards: purchaseLedgerVATAuditEntryJson
//                ));

//                #endregion

//                #region Step 12. Update the Company Statistics record (UPDATE to CFSTATS)

//                var companyStatsEntryBeforeModificationJSON = JsonConvert.SerializeObject(companyStatsEntry);

//                var companyStatsEntryPurchaseLedgerInvoicesAddition = CalculatePurchaseLedgerAmount(
//                    amounts: new[] { purchaseLedgerEntry.TotalDueInSterling },
//                    currentPeriod: currentPeriod,
//                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
//                    operationPeriod: null,
//                    future: purchaseLedgerEntry.Future == true,
//                    mvOperationType: null
//                );

//                companyStatsEntry.PurchaseLedgerInvoices += companyStatsEntryPurchaseLedgerInvoicesAddition;

//                var companyStatsEntryNextPurchaseLedgerInvoicesAddition = CalculatePurchaseLedgerAmount(
//                    amounts: new[] { purchaseLedgerEntry.TotalDueInSterling },
//                    currentPeriod: currentPeriod,
//                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
//                    operationPeriod: "N",
//                    future: purchaseLedgerEntry.Future == true,
//                    mvOperationType: null
//                );

//                companyStatsEntry.NextPurchaseLedgerInvoices += companyStatsEntryNextPurchaseLedgerInvoicesAddition;

//                var companyStatsEntryJson = JsonConvert.SerializeObject(companyStatsEntry);

//                CompanyStatsFunctions.UpsertViaTransaction(ref session, companyStatsEntryJson);

//                results.Add(new PurchaseLedgerInsertionResult(
//                    purchaseLedgerRegistryKey: registerEntry.Key,
//                    fileKey: companyStatsEntry.Key,
//                    fileName: "CFSTATS",
//                    operationsPerformed: new string[] {
//                        $"<11> \"PLINV\" (PurchaseLedgerInvoices) += {companyStatsEntryPurchaseLedgerInvoicesAddition}",
//                        $"<21> \"NPLINV\" (NextPurchaseLedgerInvoices) += {companyStatsEntryNextPurchaseLedgerInvoicesAddition}"
//                    },
//                    jsonBefore: companyStatsEntryBeforeModificationJSON,
//                    jsonAfterwards: companyStatsEntryJson
//                ));

//                #endregion

//                #region Step 13. Create a General Ledger Batch entry (INSERT to GLBATCH)

//                var generalLedgerBatchEntry = new GeneralLedgerBatch(purchaseLedgerKey)
//                {
//                    TransactionType = "RFPOST",
//                    DataKeys = purchaseLedgerEntry.DocumentReferences,
//                    NumberOfRecords = 1
//                };

//                var generalLedgerBatchEntryJson = JsonConvert.SerializeObject(generalLedgerBatchEntry);

//                GeneralLedgerBatchFunctions.UpsertViaTransaction(ref session, generalLedgerBatchEntryJson);

//                results.Add(new PurchaseLedgerInsertionResult(
//                    purchaseLedgerRegistryKey: registerEntry.Key,
//                    fileKey: generalLedgerBatchEntry.Key,
//                    fileName: "GLBATCH",
//                    operationsPerformed: new[] { "INSERT" },
//                    jsonBefore: null,
//                    jsonAfterwards: generalLedgerBatchEntryJson
//                ));

//                #endregion

//                #region Step 14. Create a the General Ledger Batch Data entry (INSERT to GLBATCHDATA)

//                var generalLedgerBatchDataEntry = new GeneralLedgerBatchData(purchaseLedgerKey)
//                {
//                    Type = "RFPOST",
//                    DocumentNumber = purchaseLedgerEntry.DocumentReference,
//                    Prefix = "P",
//                    DocumentDT = purchaseLedgerEntry.DocumentDT,
//                    PostDT = purchaseLedgerEntry.PostDT,
//                    SupplierCode = purchaseLedgerEntry.SupplierCode,
//                    GeneralLedgerCodes = purchaseLedgerEntry.GeneralLedgerCodes,
//                    VATTax = purchaseLedgerEntry.VATTaxes.Sum(),
//                    DocumentTotal = purchaseLedgerEntry.TotalDueInSterling,
//                    SupplierName = supplierEntry.Name,
//                    TransactionType = "PINV",
//                    GeneralLedgerSubCodes = purchaseLedgerEntry.GeneralLedgerSubCodes,
//                    Memo = purchaseLedgerEntry.Memo,
//                    OtherMemos = purchaseLedgerEntry.OtherMemos,
//                    ItemReference = purchaseLedgerEntry.SupplierInvoiceReference
//                };

//                // 12. INSERT to GLBATCHDATA

//                var generalLedgerBatchDataEntryJson = JsonConvert.SerializeObject(generalLedgerBatchDataEntry);

//                GeneralLedgerBatchDataFunctions.UpsertViaTransaction(ref session, generalLedgerBatchDataEntryJson);

//                results.Add(new PurchaseLedgerInsertionResult(
//                    purchaseLedgerRegistryKey: registerEntry.Key,
//                    fileKey: generalLedgerBatchDataEntry.Key,
//                    fileName: "GLBATCHDATA",
//                    operationsPerformed: new[] { "INSERT" },
//                    jsonBefore: null,
//                    jsonAfterwards: generalLedgerBatchDataEntryJson
//                ));

//                #endregion

//                #region Step 15. Create a Purchase Ledger Adjustment Audit entry (INSERT to PLADJAUDIT)

//                var purchaseLedgerAdjustmentAuditEntry = new PurchaseLedgerAdjustmentAudit($"*{purchaseLedgerEntry.DocumentReference}")
//                {
//                    DocumentTotal = purchaseLedgerEntry.TotalDueInSterling,
//                    TransactionType = "INV",
//                    DocumentDT = purchaseLedgerEntry.DocumentDT,
//                    PostDT = now,
//                    VATTaxesTotalInSterling = purchaseLedgerTransactionEntry.VATTaxesTotal,
//                    SupplierCode = purchaseLedgerEntry.SupplierCode,
//                    Balances = new[] { purchaseLedgerTransactionEntry.Balance },
//                    GeneralLedgerCodes = purchaseLedgerEntry.GeneralLedgerCodes,
//                    Discounts = new[] { purchaseLedgerTransactionEntry.DiscountAmount },
//                    GeneralLedgerAmounts = purchaseLedgerEntry.Amounts,
//                    Ports = new[] { session.HostPort },
//                    Time = now.ToString("HH:mm"),
//                    Dispute = purchaseLedgerEntry.DisputeIndicator,
//                    DueDT = purchaseLedgerEntry.InvoiceDueDT,
//                    CurrencyCode = purchaseLedgerEntry.CurrencyCode,
//                    VATTaxesTotalInCurrency = purchaseLedgerTransactionCurrencyEntry.VATTaxesTotal,
//                    DocumentTotalInCurrency = purchaseLedgerEntry.AmountInCurrency,
//                    ExchangeRates = purchaseLedgerEntry.ExchangeRates
//                };

//                var purchaseLedgerAdjustmentAuditEntryJson = JsonConvert.SerializeObject(purchaseLedgerAdjustmentAuditEntry);

//                PurchaseLedgerAdjustmentAuditFunctions.UpsertViaTransaction(ref session, purchaseLedgerAdjustmentAuditEntryJson);

//                results.Add(new PurchaseLedgerInsertionResult(
//                    purchaseLedgerRegistryKey: registerEntry.Key,
//                    fileKey: purchaseLedgerAdjustmentAuditEntry.Key,
//                    fileName: "PLADJAUDIT",
//                    operationsPerformed: new[] { "INSERT" },
//                    jsonBefore: null,
//                    jsonAfterwards: purchaseLedgerAdjustmentAuditEntryJson
//                ));

//                #endregion

//                #region Step 16. Create a Purchase Ledger Adjustment Audit 1 entry (INSERT to PLADJAUDIT1)



//                #endregion

//                #region Step 17. Create a Purchase Ledger Supplier Invoice entry (INSERT into PLSUPINV)

//                var purchaseLedgerSupplierInvoiceEntry = new PurchaseLedgerSupplierInvoice($"{purchaseLedgerEntry.SupplierCode}*{purchaseLedgerEntry.SupplierInvoiceReference}")
//                {
//                    SupplierCode = purchaseLedgerEntry.SupplierCode,
//                    SupplierInvoiceReference = purchaseLedgerEntry.SupplierInvoiceReference,
//                    TransactionType = "INV",
//                    DocumentDT = purchaseLedgerEntry.DocumentDT,
//                    DocumentNumber = purchaseLedgerEntry.DocumentReference,
//                    PostDT = purchaseLedgerEntry.PostDT
//                };

//                var purchaseLedgerSupplierInvoiceEntryJson = JsonConvert.SerializeObject(purchaseLedgerSupplierInvoiceEntry);

//                results.Add(new PurchaseLedgerInsertionResult(
//                    purchaseLedgerRegistryKey: registerEntry.Key,
//                    fileKey: purchaseLedgerSupplierInvoiceEntry.Key,
//                    fileName: "PLSUPINV",
//                    operationsPerformed: new[] { "INSERT" },
//                    jsonBefore: null,
//                    jsonAfterwards: purchaseLedgerSupplierInvoiceEntryJson
//                ));

//                PurchaseLedgerSupplierInvoiceFunctions.UpsertViaTransaction(ref session, purchaseLedgerSupplierInvoiceEntryJson);

//                #endregion

//                #region Step 18. Create the necessary GLPPTRANS.DES entries (if any of the provided GL codes are PrePayment codes)

//                // This is a bit messy: the data in GLPPTRANS.DES (even very recent data) contravenes the dictionary entries for the fields
//                // all I can hope to do is faithfully follow what the transcation definition is attempting to do, so I shall.

//                var generalLedgerPrepaymentTransactionEntries = new List<GeneralLedgerPrePaymentTransaction>();

//                for (var i = 0; i < purchaseLedgerEntry.GeneralLedgerCodes.Count(); i++)
//                {
//                    var glc = purchaseLedgerEntry.GeneralLedgerCodes.ElementAt(i);

//                    if (prePaymentPurchaseLedgerCodes.Contains(glc))
//                    {
//                        // Create a GLPPTRANS.DES record for this GLC.
//                        // Must have a new, unique key - the largest key in GLPPTRANS.CUR file, incremented by one.

//                        var glpKeys = session.CreateUniCommand();

//                        glpKeys.Command = "LIST GLPPTRANS.DES BY-DSND COL.HDR.SUPP";

//                        glpKeys.Execute();

//                        var latestKeyString = glpKeys.Response.Trim().Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).First();

//                        var latestKeyInt = Convert.ToInt32(latestKeyString);

//                        var nextKeyInt = latestKeyInt + 1;

//                        var nextKeyString = $"{latestKeyInt}".PadLeft(8, '0');

//                        var prePaymentAmountSplits = purchaseLedgerEntry.PrepaymentSplitAmounts.ElementAtOrDefault(i);

//                        var prePaymentMemoSplits = purchaseLedgerEntry.PrepaymentSplitMemos.ElementAtOrDefault(i);

//                        var prePaymentMemo = purchaseLedgerEntry.PrepaymentMemos.ElementAtOrDefault(i);

//                        var otherPrePaymentMemo = purchaseLedgerEntry.OtherPrepaymentMemos.ElementAtOrDefault(i);

//                        var generalLedgerPrepaymentTransactionEntry = new GeneralLedgerPrePaymentTransaction(nextKeyString)
//                        {
//                            SupplierCode = purchaseLedgerEntry.SupplierCode,
//                            GeneralLedgerCodes = new string[] { glc },
//                            AmountTotal = purchaseLedgerEntry.Amounts.ElementAt(i),
//                            DocumentNumber = purchaseLedgerEntry.DocumentReference,
//                            TransactionType = "INV",
//                            DocumentDT = purchaseLedgerEntry.DocumentDT,
//                            DueDT = purchaseLedgerEntry.InvoiceDueDT,
//                            StartPeriod = purchaseLedgerEntry.PrepaymentStartPeriods.ElementAt(i),
//                            NumberOfPeriods = purchaseLedgerEntry.PrepaymentNumberOfPeriods.ElementAt(i),
//                            SplitEqually = purchaseLedgerEntry.PrepaymentsSplitEqually.ElementAt(i),
//                            SplitAmounts = prePaymentAmountSplits?.Split(Framework.SubValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries).Select(a => Convert.ToDecimal(a)),
//                            SplitMemos = prePaymentMemoSplits?.Split(Framework.SubValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries),
//                            Memo = prePaymentMemo ?? otherPrePaymentMemo,
//                        };

//                        generalLedgerPrepaymentTransactionEntries.Add(generalLedgerPrepaymentTransactionEntry);
//                    }
//                }

//                foreach(var glppt in generalLedgerPrepaymentTransactionEntries)
//                {
//                    var glpptJson = JsonConvert.SerializeObject(glppt);

//                    // 15. INSERT into GLPPTRANS.DES

//                    results.Add(new PurchaseLedgerInsertionResult(
//                        purchaseLedgerRegistryKey: registerEntry.Key,
//                        fileKey: glppt.Key,
//                        fileName: "GLPPTRANS.DES",
//                        operationsPerformed: new[] { "INSERT" },
//                        jsonBefore: null,
//                        jsonAfterwards: glpptJson
//                    ));

//                    GeneralLedgerPrePaymentTransactionFunctions.UpsertViaTransaction(ref session, glpptJson);
//                }

//                #endregion

//                transaction.Commit();
//            }

//            return new ArrayList(results);
//        }

//        public static void PurchaseLedger_InsertFromCreditNoteRegistry_FillRow(
//            object obj,
//            out SqlString PurchaseLedgerCreditNoteRegistryKey,
//            out SqlString FileKey,
//            out SqlString FileName,
//            out SqlString OperationsPerformed,
//            out SqlString JSONBefore,
//            out SqlString JSONAfterwards
//            )
//        {
//            var p = (PurchaseLedgerInsertionResult)obj;

//            PurchaseLedgerCreditNoteRegistryKey = p.PurchaseLedgerRegistryKey;

//            FileKey = p.FileKey;

//            FileName = p.FileName;

//            OperationsPerformed = p.OperationsPerformed?.Any() == true ? JsonConvert.SerializeObject(p.OperationsPerformed) : SqlString.Null;

//            JSONBefore = p.JSONBefore ?? SqlString.Null;

//            JSONAfterwards = p.JSONAfterwards ?? SqlString.Null;
//        }
//    }
//}
