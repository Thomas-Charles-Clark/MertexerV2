using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MertexerV2.CompanyFileFunctions;
using MertexerV2.GeneralFunctions;
using MertexerV2.Models.Company;
using MertexerV2.Models.PurchaseLedger;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;

using Newtonsoft.Json.Linq;

using Newtonsoft.Json;

namespace MertexerV2.PurchaseLedgerFunctions
{
    public static partial class PurchaseLedgerFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "PurchaseLedger_RollbackInvoiceRegistryInsertionTransaction_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable RollbackInvoiceRegistryInsertionTransaction(string UniVerseAccount, string TransactionJSON)
        {
            var results = new List<PurchaseLedgerTransactionRollbackResult>();

            // Oh, my...

            #region Properties & Initialisation

            var jObj = JObject.Parse(TransactionJSON);

            var purchaseLedgerInvoiceRegisterKey = jObj["purchaseLedgerInvoiceRegisterKey"].Value<string>();

            var purchaseLedgerKey = jObj["purchaseLedgerKey"].Value<string>();

            var purchaseLedgerRegisterHistoryKey = jObj["purchaseLedgerRegisterHistoryKey"].Value<string>();

            var purchaseLedgerTransactionKey = jObj["purchaseLedgerTransactionKey"].Value<string>();

            var purchaseLedgerTransactionCurrencyKey = jObj["purchaseLedgerTransactionCurrencyKey"].Value<string>();

            var purchaseLedgerVATAuditKey = jObj["purchaseLedgerVATAuditKey"].Value<string>();

            var generalLedgerBatchKey = jObj["generalLedgerBatchKey"].Value<string>();

            var generalLedgerBatchDataKey = jObj["generalLedgerBatchDataKey"].Value<string>();

            var purchaseLedgerAdjustmentAuditKey = jObj["purchaseLedgerAdjustmentAuditKey"].Value<string>();

            var purchaseLedgerSupplierInvoiceKey = jObj["purchaseLedgerSupplierInvoiceKey"].Value<string>();

            var generalLedgerPrePaymentTransactionKeys = jObj["generalLedgerPrePaymentTransactionKeys"].Values<string>().ToArray<string>();

            var supplierCode = jObj["supplierCode"].Value<string>();

            var registerEntry = jObj["purchaseLedgerInvoiceRegisterEntry"].ToObject<PurchaseLedgerInvoiceRegister>();

            var purchaseLedgerEntry = jObj["purchaseLedgerEntry"].ToObject<PurchaseLedger>();

            var companyEntry = Selector<Company>.Select("1", true, false, 1, UniVerseAccount);

            var companyStatsEntry = Selector<CompanyStats>.Select("1", true, false, 1, UniVerseAccount);

            var companyVATEntries = Selector<CompanyVAT>.Select(registerEntry.VATCodes, true, false, 1, UniVerseAccount);

            var currentPeriod = companyEntry.ThisPurchaseLedgerPeriod ?? companyEntry.ThisGeneralLedgerPeriod;

            #endregion

            #region Die Early if Necessary

            if (String.IsNullOrEmpty(purchaseLedgerKey))
            {
                throw new Exception($"PurchaseLedgerKey was NULL or empty: aborting rollback");
            }

            if (String.IsNullOrEmpty(purchaseLedgerRegisterHistoryKey))
            {
                throw new Exception($"PurchaseLedgerRegisterHistoryKey was NULL or empty: aborting rollback");
            }

            if (String.IsNullOrEmpty(purchaseLedgerInvoiceRegisterKey))
            {
                throw new Exception($"PurchaseLedgerInvoiceRegisterKey was NULL or empty: aborting rollback");
            }

            if (String.IsNullOrEmpty(purchaseLedgerTransactionKey))
            {
                throw new Exception($"PurchaseLedgerTransactionKey was NULL or empty: aborting rollback");
            }

            if (String.IsNullOrEmpty(purchaseLedgerTransactionCurrencyKey))
            {
                throw new Exception($"PurchaseLedgerTransactionCurrencyKey was NULL or empty: aborting rollback");
            }

            if (String.IsNullOrEmpty(supplierCode))
            {
                throw new Exception("SupplierCode was NULL or empty; aborting rollback");
            }

            if (String.IsNullOrEmpty(purchaseLedgerVATAuditKey))
            {
                throw new Exception($"PurchaseLedgerVATAuditKey was NULL or empty: aborting rollback");
            }

            if (String.IsNullOrEmpty(generalLedgerBatchKey))
            {
                throw new Exception($"GeneralLedgerBatchKey was NULL or empty: aborting rollback");
            }

            if (String.IsNullOrEmpty(generalLedgerBatchDataKey))
            {
                throw new Exception($"GeneralLedgerBatchDataKey was NULL or empty: aborting rollback");
            }

            if (String.IsNullOrEmpty(purchaseLedgerAdjustmentAuditKey))
            {
                throw new Exception($"PurchaseLedgerAdjustmentAuditKey was NULL or empty: aborting rollback");
            }

            if (String.IsNullOrEmpty(purchaseLedgerSupplierInvoiceKey))
            {
                throw new Exception($"PurchaseLedgerSupplierInvoiceKey was NULL or empty: aborting rollback");
            }

            #endregion

            using (var qe = new QueryExecutor(UniVerseAccount))
            {
                var session = qe.Session;

                #region Step 01. Delete the Purchase Ledger entry (DELETE from TPLEDG)

                var purchaseLedgerEntryDeletionCommand = session.CreateUniCommand();

                purchaseLedgerEntryDeletionCommand.Command = $"DELETE TPLEDG {purchaseLedgerKey}";

                purchaseLedgerEntryDeletionCommand.Execute();

                results.Add(new PurchaseLedgerTransactionRollbackResult(
                    purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                    fileKey: purchaseLedgerKey,
                    fileName: "TPLEDG",
                    operationsPerformed: new[] { "DELETE" },
                    success: purchaseLedgerEntryDeletionCommand.Response.Contains("1 records DELETEd"),
                    uniVerseResponse: purchaseLedgerEntryDeletionCommand.Response,
                    uniVerseCommandStatus: purchaseLedgerEntryDeletionCommand.CommandStatus
                ));

                #endregion

                #region Step 02. Delete the Purchase Ledger Register History entry (DELETE from PLREGISTER.HIST)

                var purchaseLedgerRegisterHistoryEntryDeletionCommand = session.CreateUniCommand();

                purchaseLedgerRegisterHistoryEntryDeletionCommand.Command = $"DELETE PLREGISTER.HIST {purchaseLedgerRegisterHistoryKey}";

                purchaseLedgerRegisterHistoryEntryDeletionCommand.Execute();

                results.Add(new PurchaseLedgerTransactionRollbackResult(
                    purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                    fileKey: purchaseLedgerRegisterHistoryKey,
                    fileName: "PLREGISTER.HIST",
                    operationsPerformed: new[] { "DELETE" },
                    success: purchaseLedgerRegisterHistoryEntryDeletionCommand.Response.Contains("1 records DELETEd"),
                    uniVerseResponse: purchaseLedgerRegisterHistoryEntryDeletionCommand.Response,
                    uniVerseCommandStatus: purchaseLedgerRegisterHistoryEntryDeletionCommand.CommandStatus
                ));

                #endregion

                #region Step 03. Create the given Purchase Ledger Invoice Register entry (INSERT to PLINVREG)

                var purchaseLedgerInvoiceRegisterInsertionResultKey = PurchaseLedgerInvoiceRegisterFunctions.InsertViaTransaction(ref session, JsonConvert.SerializeObject(registerEntry));

                results.Add(new PurchaseLedgerTransactionRollbackResult(
                    purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                    fileKey: purchaseLedgerInvoiceRegisterKey,
                    fileName: "PLINVREG",
                    operationsPerformed: new[] { "INSERT" },
                    success: !String.IsNullOrEmpty(purchaseLedgerInvoiceRegisterInsertionResultKey),
                    uniVerseResponse: null,
                    uniVerseCommandStatus: null
                ));

                #endregion

                #region Step 04. Delete the Purchase Ledger Transaction entry (DELETE from TPLTRANS) 

                var purchaseLedgerTransactionDeletionCommand = session.CreateUniCommand();

                purchaseLedgerTransactionDeletionCommand.Command = $"DELETE TPLTRANS {purchaseLedgerTransactionKey}";

                purchaseLedgerTransactionDeletionCommand.Execute();

                results.Add(new PurchaseLedgerTransactionRollbackResult(
                    purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                    fileKey: purchaseLedgerTransactionKey,
                    fileName: "TPLTRANS",
                    operationsPerformed: new[] { "DELETE" },
                    success: purchaseLedgerTransactionDeletionCommand.Response.Contains("1 records DELETEd"),
                    uniVerseResponse: purchaseLedgerTransactionDeletionCommand.Response,
                    uniVerseCommandStatus: purchaseLedgerTransactionDeletionCommand.CommandStatus
                ));

                #endregion

                #region Step 05. Delete the Purchase Ledger Transaction Currency entry (DELETE from TPLTRANS.CUR)

                var purchaseLedgerTransactionCurrencyDeletionCommand = session.CreateUniCommand();

                purchaseLedgerTransactionCurrencyDeletionCommand.Command = $"DELETE TPLTRANS.CUR {purchaseLedgerTransactionCurrencyKey}";

                purchaseLedgerTransactionCurrencyDeletionCommand.Execute();

                results.Add(new PurchaseLedgerTransactionRollbackResult(
                    purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                    fileKey: purchaseLedgerTransactionCurrencyKey,
                    fileName: "TPLTRANS.CUR",
                    operationsPerformed: new[] { "DELETE" },
                    success: purchaseLedgerTransactionCurrencyDeletionCommand.Response.Contains("1 records DELETEd"),
                    uniVerseResponse: purchaseLedgerTransactionCurrencyDeletionCommand.Response,
                    uniVerseCommandStatus: purchaseLedgerTransactionCurrencyDeletionCommand.CommandStatus
                ));

                #endregion

                #region Step 06. Update the relevant Supplier record, reversing the transaction's effect (UPDATE to PLSUP)

                // It is imperative that we are NOT replacing data with the prior recorded state; changes might have been made subsequently.
                // We must UNDO what was done by the transaction above.

                var supplierEntry = Selector<Models.Supplier.Supplier>.Select(supplierCode, true, false, 1, UniVerseAccount);

                var supplierEntryBalanceReduction = purchaseLedgerEntry.DocumentBalance;

                supplierEntry.Balance -= supplierEntryBalanceReduction;

                var supplierEntryNextPeriodBalanceReduction = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.TotalDueInSterling }, // <50> "CHQTOTAL/DTOTAL/RDTOTAL/STGAMT/STGCHQTOTAL/TOTDUE"
                                                                                // the subroutine itself explicitly returns early on line 40 if you're in the final period: IF PER=NOPER THEN RETURN        
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "N",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                ); // PLAMOUNT,N50

                supplierEntry.NextPeriodBalance -= supplierEntryNextPeriodBalanceReduction;

                var supplierEntryThisPeriodBalanceReduction = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.TotalDueInSterling },
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: null,
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                ); // PLAMOUNT,50

                supplierEntry.ThisPeriodBalance -= supplierEntryThisPeriodBalanceReduction;

                var supplierDocumentReferences = supplierEntry.DocumentReferences.ToList();

                supplierDocumentReferences.Remove(purchaseLedgerEntry.Key);

                supplierEntry.DocumentReferences = supplierDocumentReferences;

                var supplierEntryPurchasesNextPeriodReduction = CalculatePurchaseLedgerAmount(
                    amounts: purchaseLedgerEntry.Amounts, // <43> "PAMOUNT"
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "N",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: "A"
                ); // PLAMOUNT,N43.A

                supplierEntry.PurchasesNextPeriod -= supplierEntryPurchasesNextPeriodReduction;

                var supplierEntryPurchasesThisPeriodReduction = CalculatePurchaseLedgerAmount(
                    amounts: purchaseLedgerEntry.Amounts,
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: null,
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: "A"
                ); // PLAMOUNT,43.A

                supplierEntry.PurchasesThisPeriod -= supplierEntryPurchasesThisPeriodReduction;

                var supplierEntryPurchasesYearToDateReduction = CalculatePurchaseLedgerAmount(
                    amounts: purchaseLedgerEntry.Amounts,
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "Y",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: "A"
                ); // PLAMOUNT,Y43.A

                supplierEntry.PurchasesYearToDate -= supplierEntryPurchasesYearToDateReduction;

                supplierEntry.PeriodsInactive = purchaseLedgerEntry.Future == true ? null : (sbyte?)-1;

                var supplierUpsertResponseKey = SupplierFunctions.SupplierFunctions.UpsertViaTransaction(ref session, JsonConvert.SerializeObject(supplierEntry));

                results.Add(new PurchaseLedgerTransactionRollbackResult(
                    purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                    fileKey: supplierCode,
                    fileName: "PLSUP",
                    operationsPerformed: new[]
                    {
                        $"<15> \"BALANCE\" (Balance) -= {supplierEntryBalanceReduction}",
                        $"<18,1> \"NEXT\" (NextPeriodBalance) -= {supplierEntryNextPeriodBalanceReduction}",
                        $"<18,2> \"CURR\" (ThisPeriodBalance) -= {supplierEntryThisPeriodBalanceReduction}",
                        $"<19> \"TLINK\" -= \"{purchaseLedgerEntry.Key}\"",
                        $"<21,1> \"PNEXT\" (PurchasesNextPeriod) -= {supplierEntryPurchasesNextPeriodReduction}",
                        $"<21,2> \"PCURR\" (PurchasesThisPeriod) -= {supplierEntryPurchasesThisPeriodReduction}",
                        $"<21,3> \"PYTD\" (PurchasesYearToDate) -= {supplierEntryPurchasesYearToDateReduction}",
                        $"<23> \"INACT\" (PeriodsInactive) = \"{supplierEntry.PeriodsInactive}\""
                    },
                    success: !String.IsNullOrEmpty(supplierUpsertResponseKey),
                    uniVerseResponse: null,
                    uniVerseCommandStatus: null
                ));

                #endregion

                #region Step 07. Update the relevant Supplier Currency record, reversing the transaction's effect (UPDATE to PLSUP.CUR)

                // It is imperative that we are NOT replacing data with the prior recorded state; changes might have been made subsequently.
                // We must UNDO what was done by the transaction above.

                // Skip the futile re-checking supplierCode being null or empty, please.

                var supplierCurrencyEntry = Selector<Models.Supplier.SupplierCurrency>.Select(supplierCode, true, false, 1, UniVerseAccount);

                var supplierCurrencyEntryBalanceReduction = purchaseLedgerEntry.AmountInCurrency;

                supplierCurrencyEntry.Balance -= supplierCurrencyEntryBalanceReduction; // <250> "CURAMT"

                var supplierCurrencyEntryNextPeriodBalanceReduction = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.AmountInCurrency }, // <250> "CURAMT"
                                                                             // the subroutine itself explicitly returns early on line 40 if you're in the final period: IF PER=NOPER THEN RETURN        
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "N",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                ); // PLAMOUNT,N250

                supplierCurrencyEntry.NextPeriodBalance -= supplierCurrencyEntryNextPeriodBalanceReduction;

                var supplierCurrencyEntryThisPeriodBalanceReduction = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.AmountInCurrency }, // <250> "CURAMT"
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: null,
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                ); // PLAMOUNT,250

                supplierCurrencyEntry.ThisPeriodBalance -= supplierCurrencyEntryThisPeriodBalanceReduction;

                var supplierCurrencyDocumentReferences = supplierCurrencyEntry.DocumentReferences.ToList();

                supplierCurrencyDocumentReferences.Remove(purchaseLedgerEntry.Key);

                supplierCurrencyEntry.DocumentReferences = supplierCurrencyDocumentReferences;

                var supplierCurrencyEntryPurchasesNextPeriodReduction = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.AmountInCurrency }, // <250> "CURAMT"
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "N",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                ); // PLAMOUNT,N250

                supplierCurrencyEntry.PurchasesNextPeriod -= supplierCurrencyEntryPurchasesNextPeriodReduction;

                var supplierCurrencyEntryPurchasesNextPeriodAddition = CalculatePurchaseLedgerAmount(
                    amounts: purchaseLedgerEntry.VATTaxesInCurrency, // <248> "VATTAX.CUR"
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "N",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: "A"
                ); // PLAMOUNT,N248.A

                supplierCurrencyEntry.PurchasesNextPeriod += supplierCurrencyEntryPurchasesNextPeriodAddition;

                var supplierCurrencyEntryPurchasesThisPeriodReduction = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.AmountInCurrency }, // <250> "CURAMT"
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: null,
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                ); // PLAMOUNT,250

                supplierCurrencyEntry.PurchasesThisPeriod -= supplierCurrencyEntryPurchasesThisPeriodReduction;

                var supplierCurrencyEntryPurchasesThisPeriodAddition = CalculatePurchaseLedgerAmount(
                    amounts: purchaseLedgerEntry.VATTaxesInCurrency, // <248> "VATTAX.CUR"
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: null,
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: "A"
                ); // PLAMOUNT,248.A

                supplierCurrencyEntry.PurchasesThisPeriod += supplierCurrencyEntryPurchasesThisPeriodAddition;

                var supplierCurrencyEntryPurchasesYearToDateReduction = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.AmountInCurrency }, // <250> "CURAMT"
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "Y",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                ); // PLAMOUNT,Y250

                supplierCurrencyEntry.PurchasesYearToDate -= supplierCurrencyEntryPurchasesYearToDateReduction;

                var supplierCurrencyEntryPurchasesYearToDateAddition = CalculatePurchaseLedgerAmount(
                    amounts: purchaseLedgerEntry.VATTaxesInCurrency, // <248> "VATTAX.CUR"
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "Y",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: "A"
                ); // PLAMOUNT,Y248.A

                supplierCurrencyEntry.PurchasesYearToDate += supplierCurrencyEntryPurchasesYearToDateAddition;

                supplierCurrencyEntry.PeriodsInactive = purchaseLedgerEntry.Future == true ? null : (sbyte?)-1;

                var supplierCurrencyUpsertResponseKey = SupplierFunctions.SupplierCurrencyFunctions.UpsertViaTransaction(ref session, JsonConvert.SerializeObject(supplierCurrencyEntry));

                results.Add(new PurchaseLedgerTransactionRollbackResult(
                    purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                    fileKey: supplierCode,
                    fileName: "PLSUP.CUR",
                    operationsPerformed: new[]
                    {
                            $"<15> \"BALANCE\" (Balance) -= {supplierCurrencyEntryBalanceReduction}",
                            $"<18,1> \"NEXT\" (NextPeriodBalance) -= {supplierCurrencyEntryNextPeriodBalanceReduction}",
                            $"<18,2> \"CURR\" (ThisPeriodBalance) -= {supplierCurrencyEntryThisPeriodBalanceReduction}",
                            $"<19> \"TLINK\" -= \"{purchaseLedgerEntry.Key}\"",
                            $"<21,1> \"PNEXT\" (PurchasesNextPeriod) -= {supplierCurrencyEntryPurchasesNextPeriodReduction}",
                            $"<21,1> \"PNEXT\" (PurchasesNextPeriod) += {supplierCurrencyEntryPurchasesNextPeriodAddition}",
                            $"<21,2> \"PCURR\" (PurchasesThisPeriod) -= {supplierCurrencyEntryPurchasesThisPeriodReduction}",
                            $"<21,2> \"PCURR\" (PurchasesThisPeriod) += {supplierCurrencyEntryPurchasesThisPeriodAddition}",
                            $"<21,3> \"PYTD\" (PurchasesYearToDate) -= {supplierCurrencyEntryPurchasesYearToDateReduction}",
                            $"<21,3> \"PYTD\" (PurchasesYearToDate) += {supplierCurrencyEntryPurchasesYearToDateAddition}",
                            $"<23> \"INACT\" (PeriodsInactive) = \"{supplierCurrencyEntry.PeriodsInactive}\""
                    },
                    success: !String.IsNullOrEmpty(supplierCurrencyUpsertResponseKey),
                    uniVerseResponse: null,
                    uniVerseCommandStatus: null
                ));

                #endregion

                #region Step 08. Update the relevant CFVAT record(s), reversing the transaction's effect (UPDATE(s) to CFVAT)

                // It is imperative that we are NOT replacing data with the prior recorded state; changes might have been made subsequently.
                // We must UNDO what was done by the transaction above.

                foreach (var vatEntry in companyVATEntries)
                {
                    var purchasesInvoicesTaxReduction = CalculatePurchaseLedgerAmount(
                        amounts: purchaseLedgerEntry.VATTaxes, // <48> "VATTAX"
                        currentPeriod: currentPeriod,
                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                        operationPeriod: null,
                        future: purchaseLedgerEntry.Future == true,
                        mvOperationType: "X"
                    ); // PLAMOUNT,48X

                    vatEntry.PurchasesInvoicesTax -= purchasesInvoicesTaxReduction;


                    var purchasesInvoicesValueReduction = CalculatePurchaseLedgerAmount(
                        amounts: purchaseLedgerEntry.VATableValues, // <49> "VATVALUE"
                        currentPeriod: currentPeriod,
                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                        operationPeriod: null,
                        future: purchaseLedgerEntry.Future == true,
                        mvOperationType: "X"
                    ); // PLAMOUNT,48X

                    vatEntry.PurchasesInvoicesValue -= purchasesInvoicesValueReduction;


                    var purchasesQuarterTaxTotalReduction = CalculatePurchaseLedgerAmount(
                        amounts: purchaseLedgerEntry.VATTaxes, // <48> "VATTAX"
                        currentPeriod: currentPeriod,
                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                        operationPeriod: null,
                        future: purchaseLedgerEntry.Future == true,
                        mvOperationType: "X"
                    ); // PLAMOUNT,48X

                    vatEntry.PurchasesQuarterTaxTotal -= purchasesQuarterTaxTotalReduction;


                    var purchasesQuarterValueTotalReduction = CalculatePurchaseLedgerAmount(
                        amounts: purchaseLedgerEntry.VATableValues, // <49> "VATVALUE"
                        currentPeriod: currentPeriod,
                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                        operationPeriod: null,
                        future: purchaseLedgerEntry.Future == true,
                        mvOperationType: "X"
                    ); // PLAMOUNT,48X

                    vatEntry.PurchasesQuarterValueTotal -= purchasesQuarterValueTotalReduction;

                    var purchasesNextPeriodInvoicesTaxReduction = CalculatePurchaseLedgerAmount(
                        amounts: purchaseLedgerEntry.VATTaxes, // <48> "VATTAX"
                        currentPeriod: currentPeriod,
                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                        operationPeriod: "N",
                        future: purchaseLedgerEntry.Future == true,
                        mvOperationType: "X"
                    ); // PLAMOUNT,N48X

                    vatEntry.PurchasesNextPeriodInvoicesTax -= purchasesNextPeriodInvoicesTaxReduction;

                    var purchasesNextPeriodInvoicesValueReduction = CalculatePurchaseLedgerAmount(
                        amounts: purchaseLedgerEntry.VATableValues, // <49> "VATVALUE"
                        currentPeriod: currentPeriod,
                        numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                        operationPeriod: "N",
                        future: purchaseLedgerEntry.Future == true,
                        mvOperationType: "X"
                    ); // PLAMOUNT,N48X

                    vatEntry.PurchasesNextPeriodInvoicesValue -= purchasesNextPeriodInvoicesValueReduction;

                    var vatEntryJson = JsonConvert.SerializeObject(vatEntry);

                    var vatEntryUpsertResponse = CompanyVATFunctions.UpsertViaTransaction(ref session, vatEntryJson);

                    results.Add(new PurchaseLedgerTransactionRollbackResult(
                        purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                        fileKey: vatEntry.Key,
                        fileName: "CFVAT",
                        operationsPerformed: new[] {
                            $"<30> \"PINVT\" (PurchasesInvoicesTax) -= {purchasesInvoicesTaxReduction}",
                            $"<34> \"PINVV\" (PurchasesInvoicesValue) -= {purchasesInvoicesValueReduction}",
                            $"<38> \"PQTTOT\" (PurchasesQuarterTaxTotal) -= {purchasesQuarterTaxTotalReduction}",
                            $"<39> \"PQVTOT\" (PurchasesQuarterValueTotal) -= {purchasesQuarterValueTotalReduction}",
                            $"<40> \"PNINVT\" (PurchasesNextPeriodInvoicesTax) -= {purchasesNextPeriodInvoicesTaxReduction}",
                            $"<44> \"PNINVV\" (PurchasesNextPeriodInvoicesValue) -= {purchasesNextPeriodInvoicesValueReduction}",
                        },
                        success: !String.IsNullOrEmpty(vatEntryUpsertResponse),
                        uniVerseResponse: null,
                        uniVerseCommandStatus: null
                    ));
                }


                #endregion

                #region Step 09. Delete the Purchase Ledger VAT Audit Entry (DELETE from PLVATAUD)

                var purchaseLedgerVATAuditDeletionCommand = session.CreateUniCommand();

                purchaseLedgerVATAuditDeletionCommand.Command = $"DELETE PLVATAUD {purchaseLedgerVATAuditKey}";

                purchaseLedgerVATAuditDeletionCommand.Execute();

                results.Add(new PurchaseLedgerTransactionRollbackResult(
                    purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                    fileKey: purchaseLedgerVATAuditKey,
                    fileName: "PLVATAUD",
                    operationsPerformed: new[] { "DELETE" },
                    success: purchaseLedgerVATAuditDeletionCommand.Response.Contains("1 records DELETEd"),
                    uniVerseResponse: purchaseLedgerVATAuditDeletionCommand.Response,
                    uniVerseCommandStatus: purchaseLedgerVATAuditDeletionCommand.CommandStatus
                ));

                #endregion

                #region Step 10. Update the Company Statistics record, reversing the transaction's effect (UPDATE to CFSTATS) 

                // It is imperative that we are NOT replacing data with the prior recorded state; changes might have been made subsequently.
                // We must UNDO what was done by the transaction above.

                var companyStatsEntryPurchaseLedgerInvoicesReduction = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.TotalDueInSterling },
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: null,
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                );

                companyStatsEntry.PurchaseLedgerInvoices -= companyStatsEntryPurchaseLedgerInvoicesReduction;

                var companyStatsEntryNextPurchaseLedgerInvoicesReduction = CalculatePurchaseLedgerAmount(
                    amounts: new[] { purchaseLedgerEntry.TotalDueInSterling },
                    currentPeriod: currentPeriod,
                    numberOfPeriods: companyEntry.NumberOfFinancialPeriods,
                    operationPeriod: "N",
                    future: purchaseLedgerEntry.Future == true,
                    mvOperationType: null
                );

                companyStatsEntry.NextPurchaseLedgerInvoices -= companyStatsEntryNextPurchaseLedgerInvoicesReduction;

                var companyStatsEntryJson = JsonConvert.SerializeObject(companyStatsEntry);

                var companyStatusUpsertResultKey = CompanyStatsFunctions.UpsertViaTransaction(ref session, companyStatsEntryJson);

                results.Add(new PurchaseLedgerTransactionRollbackResult(
                    purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                    fileKey: "1",
                    fileName: "CFSTATS",
                    operationsPerformed: new[]
                    {
                        $"<11> \"PLINV\" (PurchaseLedgerInvoices) -= {companyStatsEntryPurchaseLedgerInvoicesReduction}",
                        $"<21> \"NPLINV\" (NextPurchaseLedgerInvoices) -= {companyStatsEntryNextPurchaseLedgerInvoicesReduction}"
                    },
                    success: !String.IsNullOrEmpty(companyStatusUpsertResultKey),
                    uniVerseResponse: null,
                    uniVerseCommandStatus: null
                ));

                #endregion

                #region Step 11. Delete the General Ledger Batch entry (DELETE from GLBATCH)

                var generalLedgerBatchDeletionCommand = session.CreateUniCommand();

                generalLedgerBatchDeletionCommand.Command = $"DELETE GLBATCH {generalLedgerBatchKey}";

                generalLedgerBatchDeletionCommand.Execute();

                results.Add(new PurchaseLedgerTransactionRollbackResult(
                    purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                    fileKey: generalLedgerBatchKey,
                    fileName: "GLBATCH",
                    operationsPerformed: new[] { "DELETE" },
                    success: generalLedgerBatchDeletionCommand.Response.Contains("1 records DELETEd"),
                    uniVerseResponse: generalLedgerBatchDeletionCommand.Response,
                    uniVerseCommandStatus: generalLedgerBatchDeletionCommand.CommandStatus
                ));
                
                #endregion

                #region Step 12. Delete the General Ledger Batch Data entry (DELETE from GLBATCHDATA)

                var generalLedgerBatchDataDeletionCommand = session.CreateUniCommand();

                generalLedgerBatchDataDeletionCommand.Command = $"DELETE GLBATCHDATA {generalLedgerBatchDataKey}";

                generalLedgerBatchDataDeletionCommand.Execute();

                results.Add(new PurchaseLedgerTransactionRollbackResult(
                    purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                    fileKey: generalLedgerBatchDataKey,
                    fileName: "GLBATCHDATA",
                    operationsPerformed: new[] { "DELETE" },
                    success: generalLedgerBatchDataDeletionCommand.Response.Contains("1 records DELETEd"),
                    uniVerseResponse: generalLedgerBatchDataDeletionCommand.Response,
                    uniVerseCommandStatus: generalLedgerBatchDataDeletionCommand.CommandStatus
                ));

                #endregion

                #region Step 13. Delete the Purchase Ledger Adjustment Audit entry (DELETE from PLADJAUDIT)

                var purchaseLedgerAdjustmentAuditDeletionCommand = session.CreateUniCommand();

                purchaseLedgerAdjustmentAuditDeletionCommand.Command = $"DELETE PLADJAUDIT {purchaseLedgerAdjustmentAuditKey}";

                purchaseLedgerAdjustmentAuditDeletionCommand.Execute();

                results.Add(new PurchaseLedgerTransactionRollbackResult(
                    purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                    fileKey: purchaseLedgerAdjustmentAuditKey,
                    fileName: "PLADJAUDIT",
                    operationsPerformed: new[] { "DELETE" },
                    success: purchaseLedgerAdjustmentAuditDeletionCommand.Response.Contains("1 records DELETEd"),
                    uniVerseResponse: purchaseLedgerAdjustmentAuditDeletionCommand.Response,
                    uniVerseCommandStatus: purchaseLedgerAdjustmentAuditDeletionCommand.CommandStatus
                ));

                #endregion

                #region Step 14. Delete the Purchase Ledger Supplier Invoice entry (DELETE from PLSUPINV)

                var purchaseLedgerSupplierInvoiceDeletionCommand = session.CreateUniCommand();

                purchaseLedgerSupplierInvoiceDeletionCommand.Command = $"DELETE PLSUPINV {purchaseLedgerSupplierInvoiceKey}";

                purchaseLedgerSupplierInvoiceDeletionCommand.Execute();

                results.Add(new PurchaseLedgerTransactionRollbackResult(
                    purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                    fileKey: purchaseLedgerSupplierInvoiceKey,
                    fileName: "PLSUPINV",
                    operationsPerformed: new[] { "DELETE" },
                    success: purchaseLedgerSupplierInvoiceDeletionCommand.Response.Contains("1 records DELETEd"),
                    uniVerseResponse: purchaseLedgerSupplierInvoiceDeletionCommand.Response,
                    uniVerseCommandStatus: purchaseLedgerSupplierInvoiceDeletionCommand.CommandStatus
                ));

                #endregion

                #region Step 15. Delete any GLPPTRANS.DES entries which the transaction happened to create, if any (DELETE from GLPPTRANS.DES)

                if (generalLedgerPrePaymentTransactionKeys.Any())
                {
                    var keys = String.Join(" ", generalLedgerPrePaymentTransactionKeys);

                    var generalLedgerPrePaymentTransactionDeletionCommand = session.CreateUniCommand();

                    generalLedgerPrePaymentTransactionDeletionCommand.Command = $"DELETE GLPPTRANS.DES {keys}";

                    generalLedgerPrePaymentTransactionDeletionCommand.Execute();

                    results.Add(new PurchaseLedgerTransactionRollbackResult(
                        purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                        fileKey: keys,
                        fileName: "GLPPTRANS.DES",
                        operationsPerformed: new[] { "DELETE" },
                        success: generalLedgerPrePaymentTransactionDeletionCommand.Response.Contains("1 records DELETEd"),
                        uniVerseResponse: generalLedgerPrePaymentTransactionDeletionCommand.Response,
                        uniVerseCommandStatus: generalLedgerPrePaymentTransactionDeletionCommand.CommandStatus
                    ));
                }

                #endregion
            }

            return results;
        }

        public static void PurchaseLedger_RollbackInvoiceRegistryInsertionTransaction_FillRow(
            object obj,
            out SqlString PurchaseLedgerInvoiceRegistryKey,
            out SqlString FileKey,
            out SqlString FileName,
            out SqlString OperationsPerformed,
            out SqlBoolean Success,
            out SqlString UniCommandResponse,
            out SqlInt32 UniCommandStatus
            )
        {
            var p = (PurchaseLedgerTransactionRollbackResult)obj;

            PurchaseLedgerInvoiceRegistryKey = p.PurchaseLedgerInvoiceRegistryKey;

            FileKey = p.FileKey;

            FileName = p.FileName;

            OperationsPerformed = p.OperationsPerformed?.Any() == true ? JsonConvert.SerializeObject(p.OperationsPerformed) : SqlString.Null;

            Success = p.Success;

            UniCommandResponse = String.IsNullOrEmpty(p.UniCommandResponse) ? SqlString.Null : p.UniCommandResponse;

            UniCommandStatus = p.UniCommandStatus ?? SqlInt32.Null;
        }
    }
}
