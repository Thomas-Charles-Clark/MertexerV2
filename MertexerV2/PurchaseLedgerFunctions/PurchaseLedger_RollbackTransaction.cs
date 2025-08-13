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

            var purchaseLedgerEntryKey = jObj["purchaseLedgerKey"].Value<string>();

            var purchaseLedgerRegisterHistoryEntryKey = jObj["purchaseLedgerRegisterHistoryKey"].Value<string>();

            var purchaseLedgerTransactionEntryKey = jObj["purchaseLedgerTransactionKey"].Value<string>();

            var purchaseLedgerTransactionCurrencyEntryKey = jObj["purchaseLedgerTransactionCurrencyKey"].Value<string>();

            var purchaseLedgerVATAuditEntryKey = jObj["purchaseLedgerVATAuditKey"].Value<string>();

            var generalLedgerBatchEntryKey = jObj["generalLedgerBatchKey"].Value<string>();

            var generalLedgerBatchDataEntryKey = jObj["generalLedgerBatchDataKey"].Value<string>();

            var purchaseLedgerAdjustmentAuditEntryKey = jObj["purchaseLedgerAdjustmentAuditKey"].Value<string>();

            var purchaseLedgerSupplierInvoiceEntryKey = jObj["purchaseLedgerSupplierInvoiceKey"].Value<string>();

            var generalLedgerPrePaymentTransactionKeys = jObj["generalLedgerPrePaymentTransactionKeys"].Values<string>().ToArray<string>();

            var supplierCode = jObj["supplierCode"].Value<string>();

            var registerEntry = jObj["purchaseLedgerInvoiceRegisterEntry"].ToObject<PurchaseLedgerInvoiceRegister>();

            var purchaseLedgerEntry = jObj["purchaseLedgerEntry"].ToObject<PurchaseLedger>();

            var companyEntry = Selector<Company>.Select("1", true, false, 1, UniVerseAccount);

            var companyStatsEntry = Selector<CompanyStats>.Select("1", true, false, 1, UniVerseAccount);

            var companyVATEntries = Selector<CompanyVAT>.Select(registerEntry.VATCodes, true, false, 1, UniVerseAccount);

            var currentPeriod = companyEntry.ThisPurchaseLedgerPeriod ?? companyEntry.ThisGeneralLedgerPeriod;

            #endregion

            using (var qe = new QueryExecutor(UniVerseAccount))
            {
                var session = qe.Session;

                var transaction = session.CreateUniTransaction();

                #region Step 01. Delete the Purchase Ledger entry (DELETE from TPLEDG)

                if (String.IsNullOrEmpty(purchaseLedgerEntryKey))
                {
                    throw new Exception($"PurchaseLedgerEntryKey was NULL or empty: aborting rollback");
                }
                else
                {
                    var purchaseLedgerEntryDeletionCommand = session.CreateUniCommand();

                    purchaseLedgerEntryDeletionCommand.Command = $"DELETE TPLEDG {purchaseLedgerEntryKey}";

                    purchaseLedgerEntryDeletionCommand.Execute();

                    if (purchaseLedgerEntryDeletionCommand.Response.Contains("1 records DELETEd"))
                    {
                        results.Add(new PurchaseLedgerTransactionRollbackResult(
                            purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                            fileKey: purchaseLedgerEntryKey,
                            fileName: "TPLEDG",
                            operationsPerformed: new[] { "DELETE" }
                        ));
                    }
                    else
                    {
                        throw new Exception($"Failed to DELETE {purchaseLedgerEntryKey} from TPLEDG; response from UniVerse was: {purchaseLedgerEntryDeletionCommand.Response}");
                    }
                }

                #endregion

                #region Step 02. Delete the Purchase Ledger Register History entry (DELETE from PLREGISTER.HIST)

                if (String.IsNullOrEmpty(purchaseLedgerRegisterHistoryEntryKey))
                {
                    throw new Exception($"PurchaseLedgerRegisterHistoryEntryKey was NULL or empty: aborting rollback");
                }
                else
                {
                    var purchaseLedgerRegisterHistoryEntryDeletionCommand = session.CreateUniCommand();

                    purchaseLedgerRegisterHistoryEntryDeletionCommand.Command = $"DELETE PLREGISTER.HIST {purchaseLedgerRegisterHistoryEntryKey}";

                    purchaseLedgerRegisterHistoryEntryDeletionCommand.Execute();

                    if (purchaseLedgerRegisterHistoryEntryDeletionCommand.Response.Contains("1 records DELETEd"))
                    {
                        results.Add(new PurchaseLedgerTransactionRollbackResult(
                            purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                            fileKey: purchaseLedgerRegisterHistoryEntryKey,
                            fileName: "PLREGISTER.HIST",
                            operationsPerformed: new[] { "DELETE" }
                        ));
                    }
                    else
                    {
                        throw new Exception($"Failed to DELETE {purchaseLedgerRegisterHistoryEntryKey} from PLREGISTER.HIST; response from UniVerse was: {purchaseLedgerRegisterHistoryEntryDeletionCommand.Response}");
                    }
                }

                #endregion

                #region Step 03. Create the given Purchase Ledger Invoice Register entry (INSERT to PLINVREG)

                if (String.IsNullOrEmpty(purchaseLedgerInvoiceRegisterKey))
                {
                    throw new Exception($"PurchaseLedgerInvoiceRegisterKey was NULL or empty: aborting rollback");
                }
                else
                {
                    _ = PurchaseLedgerInvoiceRegisterFunctions.InsertViaTransaction(ref session, JsonConvert.SerializeObject(registerEntry));

                    results.Add(new PurchaseLedgerTransactionRollbackResult(
                        purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                        fileKey: purchaseLedgerInvoiceRegisterKey,
                        fileName: "PLINVREG",
                        operationsPerformed: new[] { "INSERT" }
                    ));
                }

                #endregion

                #region Step 04. Delete the Purchase Ledger Transaction entry (DELETE from TPLTRANS) 

                if (String.IsNullOrEmpty(purchaseLedgerTransactionEntryKey))
                {
                    throw new Exception($"PurchaseLedgerTransactionEntryKey was NULL or empty: aborting rollback");
                }
                else
                {
                    var purchaseLedgerTransactionDeletionCommand = session.CreateUniCommand();

                    purchaseLedgerTransactionDeletionCommand.Command = $"DELETE TPLTRANS {purchaseLedgerTransactionEntryKey}";

                    purchaseLedgerTransactionDeletionCommand.Execute();

                    if (purchaseLedgerTransactionDeletionCommand.Response.Contains("1 records DELETEd"))
                    {
                        results.Add(new PurchaseLedgerTransactionRollbackResult(
                            purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                            fileKey: purchaseLedgerTransactionEntryKey,
                            fileName: "TPLTRANS",
                            operationsPerformed: new[] { "DELETE" }
                        ));
                    }
                    else
                    {
                        throw new Exception($"Failed to DELETE {purchaseLedgerTransactionEntryKey} from TPLTRANS; response from UniVerse was: {purchaseLedgerTransactionDeletionCommand.Response}");
                    }
                }

                #endregion

                #region Step 05. Delete the Purchase Ledger Transaction Currency entry (DELETE from TPLTRANS.CUR)

                if (String.IsNullOrEmpty(purchaseLedgerTransactionCurrencyEntryKey))
                {
                    throw new Exception($"PurchaseLedgerTransactionCurrencyEntryKey was NULL or empty: aborting rollback");
                }
                else
                {
                    var purchaseLedgerTransactionCurrencyDeletionCommand = session.CreateUniCommand();

                    purchaseLedgerTransactionCurrencyDeletionCommand.Command = $"DELETE TPLTRANS.CUR {purchaseLedgerTransactionCurrencyEntryKey}";

                    purchaseLedgerTransactionCurrencyDeletionCommand.Execute();

                    if (purchaseLedgerTransactionCurrencyDeletionCommand.Response.Contains("1 records DELETEd"))
                    {
                        results.Add(new PurchaseLedgerTransactionRollbackResult(
                            purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                            fileKey: purchaseLedgerTransactionCurrencyEntryKey,
                            fileName: "TPLTRANS.CUR",
                            operationsPerformed: new[] { "DELETE" }
                        ));
                    }
                    else
                    {
                        throw new Exception($"Failed to DELETE {purchaseLedgerTransactionCurrencyEntryKey} from TPLTRANS.CUR; response from UniVerse was: {purchaseLedgerTransactionCurrencyDeletionCommand.Response}");
                    }
                }

                #endregion

                #region Step 06. Update the relevant Supplier record, reversing the transaction's effect (UPDATE to PLSUP)

                // It is imperative that we are NOT replacing data with the prior recorded state; changes might have been made subsequently.
                // We must UNDO what was done by the transaction above.

                if (String.IsNullOrEmpty(supplierCode))
                {
                    throw new Exception("SupplierCode was NULL or empty; aborting rollback");
                }
                else
                {
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

                    SupplierFunctions.SupplierFunctions.UpsertViaTransaction(ref session, JsonConvert.SerializeObject(supplierEntry));

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
                        }
                    ));
                }


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

                SupplierFunctions.SupplierCurrencyFunctions.UpsertViaTransaction(ref session, JsonConvert.SerializeObject(supplierCurrencyEntry));

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
                    }
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

                    CompanyVATFunctions.UpsertViaTransaction(ref session, vatEntryJson);

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
                            }
                        ));
                }


                #endregion

                #region Step 09. Delete the Purchase Ledger VAT Audit Entry (DELETE from PLVATAUD)

                if (String.IsNullOrEmpty(purchaseLedgerVATAuditEntryKey))
                {
                    throw new Exception($"PurchaseLedgerVATAuditEntryKey was NULL or empty: aborting rollback");
                }
                else
                {
                    var purchaseLedgerVATAuditDeletionCommand = session.CreateUniCommand();

                    purchaseLedgerVATAuditDeletionCommand.Command = $"DELETE PLVATAUD {purchaseLedgerVATAuditEntryKey}";

                    purchaseLedgerVATAuditDeletionCommand.Execute();

                    if (purchaseLedgerVATAuditDeletionCommand.Response.Contains("1 records DELETEd"))
                    {
                        results.Add(new PurchaseLedgerTransactionRollbackResult(
                        purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                            fileKey: purchaseLedgerVATAuditEntryKey,
                            fileName: "PLVATAUD",
                            operationsPerformed: new[] { "DELETE" }
                        ));
                    }
                    else
                    {
                        throw new Exception($"Failed to DELETE {purchaseLedgerVATAuditEntryKey} from PLVATAUD; response from UniVerse was: {purchaseLedgerVATAuditDeletionCommand.Response}");
                    }
                }

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

                CompanyStatsFunctions.UpsertViaTransaction(ref session, companyStatsEntryJson);

                results.Add(new PurchaseLedgerTransactionRollbackResult(
                    purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                    fileKey: "1",
                    fileName: "CFSTATS",
                    operationsPerformed: new[]
                    {
                        $"<11> \"PLINV\" (PurchaseLedgerInvoices) -= {companyStatsEntryPurchaseLedgerInvoicesReduction}",
                        $"<21> \"NPLINV\" (NextPurchaseLedgerInvoices) -= {companyStatsEntryNextPurchaseLedgerInvoicesReduction}"
                    }
                ));

                #endregion

                #region Step 11. Delete the General Ledger Batch entry (DELETE from GLBATCH)

                if (String.IsNullOrEmpty(generalLedgerBatchEntryKey))
                {
                    throw new Exception($"GeneralLedgerBatchEntryKey was NULL or empty: aborting rollback");
                }
                else
                {
                    var generalLedgerBatchDeletionCommand = session.CreateUniCommand();

                    generalLedgerBatchDeletionCommand.Command = $"DELETE GLBATCH {generalLedgerBatchEntryKey}";

                    generalLedgerBatchDeletionCommand.Execute();

                    if (generalLedgerBatchDeletionCommand.Response.Contains("1 records DELETEd"))
                    {
                        results.Add(new PurchaseLedgerTransactionRollbackResult(
                            purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                            fileKey: generalLedgerBatchEntryKey,
                            fileName: "GLBATCH",
                            operationsPerformed: new[] { "DELETE" }
                        ));
                    }
                    else
                    {
                        throw new Exception($"Failed to DELETE {generalLedgerBatchEntryKey} from GLBATCH; response from UniVerse was: {generalLedgerBatchDeletionCommand.Response}");
                    }
                }

                #endregion

                #region Step 12. Delete the General Ledger Batch Data entry (DELETE from GLBATCHDATA)

                if (String.IsNullOrEmpty(generalLedgerBatchDataEntryKey))
                {
                    throw new Exception($"GeneralLedgerBatchDataEntryKey was NULL or empty: aborting rollback");
                }
                else
                {
                    var generalLedgerBatchDataDeletionCommand = session.CreateUniCommand();

                    generalLedgerBatchDataDeletionCommand.Command = $"DELETE GLBATCHDATA {generalLedgerBatchDataEntryKey}";

                    generalLedgerBatchDataDeletionCommand.Execute();

                    if (generalLedgerBatchDataDeletionCommand.Response.Contains("1 records DELETEd"))
                    {
                        results.Add(new PurchaseLedgerTransactionRollbackResult(
                            purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                            fileKey: generalLedgerBatchDataEntryKey,
                            fileName: "GLBATCHDATA",
                            operationsPerformed: new[] { "DELETE" }
                        ));
                    }
                    else
                    {
                        throw new Exception($"Failed to DELETE {generalLedgerBatchDataEntryKey} from GLBATCHDATA; response from UniVerse was: {generalLedgerBatchDataDeletionCommand.Response}");
                    }
                }

                #endregion

                #region Step 13. Delete the Purchase Ledger Adjustment Audit entry (DELETE from PLADJAUDIT)

                if (String.IsNullOrEmpty(purchaseLedgerAdjustmentAuditEntryKey))
                {
                    throw new Exception($"PurchaseLedgerAdjustmentAuditEntryKey was NULL or empty: aborting rollback");
                }
                else
                {
                    var purchaseLedgerAdjustmentAuditDeletionCommand = session.CreateUniCommand();

                    purchaseLedgerAdjustmentAuditDeletionCommand.Command = $"DELETE PLADJAUDIT {purchaseLedgerAdjustmentAuditEntryKey}";

                    purchaseLedgerAdjustmentAuditDeletionCommand.Execute();

                    if (purchaseLedgerAdjustmentAuditDeletionCommand.Response.Contains("1 records DELETEd"))
                    {
                        results.Add(new PurchaseLedgerTransactionRollbackResult(
                            purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                            fileKey: purchaseLedgerAdjustmentAuditEntryKey,
                            fileName: "PLADJAUDIT",
                            operationsPerformed: new[] { "DELETE" }
                        ));
                    }
                    else
                    {
                        throw new Exception($"Failed to DELETE {purchaseLedgerAdjustmentAuditEntryKey} from PLADJAUDIT; response from UniVerse was: {purchaseLedgerAdjustmentAuditDeletionCommand.Response}");
                    }
                }

                #endregion

                #region Step 14. Delete the Purchase Ledger Supplier Invoice entry (DELETE from PLSUPINV)

                if (String.IsNullOrEmpty(purchaseLedgerSupplierInvoiceEntryKey))
                {
                    throw new Exception($"PurchaseLedgerSupplierInvoiceEntryKey was NULL or empty: aborting rollback");
                }
                else
                {
                    var purchaseLedgerSupplierInvoiceDeletionCommand = session.CreateUniCommand();

                    purchaseLedgerSupplierInvoiceDeletionCommand.Command = $"DELETE PLSUPINV {purchaseLedgerSupplierInvoiceEntryKey}";

                    purchaseLedgerSupplierInvoiceDeletionCommand.Execute();

                    if (purchaseLedgerSupplierInvoiceDeletionCommand.Response.Contains("1 records DELETEd"))
                    {
                        results.Add(new PurchaseLedgerTransactionRollbackResult(
                            purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                            fileKey: purchaseLedgerSupplierInvoiceEntryKey,
                            fileName: "PLSUPINV",
                            operationsPerformed: new[] { "DELETE" }
                        ));
                    }
                    else
                    {
                        throw new Exception($"Failed to DELETE {purchaseLedgerSupplierInvoiceEntryKey} from PLSUPINV; response from UniVerse was: {purchaseLedgerSupplierInvoiceDeletionCommand.Response}");
                    }
                }

                #endregion

                #region Step 15. Delete any GLPPTRANS.DES entries which the transaction happened to create, if any (DELETE from GLPPTRANS.DES)

                if (generalLedgerPrePaymentTransactionKeys.Any())
                {
                    var keys = String.Join(" ", generalLedgerPrePaymentTransactionKeys);

                    var generalLedgerPrePaymentTransactionDeletionCommand = session.CreateUniCommand();

                    generalLedgerPrePaymentTransactionDeletionCommand.Command = $"DELETE GLPPTRANS.DES {keys}";

                    generalLedgerPrePaymentTransactionDeletionCommand.Execute();

                    if (generalLedgerPrePaymentTransactionDeletionCommand.Response.Contains($"{generalLedgerPrePaymentTransactionKeys.Count()} records DELETEd"))
                    {
                        results.Add(new PurchaseLedgerTransactionRollbackResult(
                            purchaseLedgerInvoiceRegistryKey: registerEntry.Key,
                            fileKey: keys,
                            fileName: "GLPPTRANS.DES",
                            operationsPerformed: new[] { "DELETE" }
                        ));
                    }
                    else
                    {
                        throw new Exception($"Failed to DELETE {keys} from GLPPTRANS.DES; response from UniVerse was: {generalLedgerPrePaymentTransactionDeletionCommand.Response}");
                    }
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
            out SqlString OperationsPerformed
            )
        {
            var p = (PurchaseLedgerTransactionRollbackResult)obj;

            PurchaseLedgerInvoiceRegistryKey = p.PurchaseLedgerInvoiceRegistryKey;

            FileKey = p.FileKey;

            FileName = p.FileName;

            OperationsPerformed = p.OperationsPerformed?.Any() == true ? JsonConvert.SerializeObject(p.OperationsPerformed) : SqlString.Null;
        }
    }
}
