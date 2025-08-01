
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;

using MertexerV2.Models.PurchaseLedger;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json.Linq;

namespace MertexerV2.SQLFunctions
{
    public static partial class PurchaseLedger
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "PurhcaseLedger_InsertFromInvoiceRegistry_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable InsertFromInvoiceRegistry(string UniVerseAccount, string PurchaseLedgerInvoiceRegistryJSON)
        {
            throw new NotImplementedException();

            /*
                In order, we need to UPSERT to the following files.

                Should ANY of the following fail, we need to behave as though we're within a Transaction.
                The entire Transaction must fail and not be committed.

                PLREGISTER.HIST
                PLINVREG
                TPLTRANS
                TPLTRANS.CUR
                PLSUP
                PLSUP.CUR
                CFVAT
                PLVATAUD
                CFSTATS
                GLBATCH
                GLBATCHDATA
                PLADJAUDIT
                PLSUPINV
                GLPPTRANS.DES
             
                So how can we handle that requirement - and it is a hard-and-fast requirement - that we maintain and act within one transaction.

             */

            using (var qe = new QueryExecutor(UniVerseAccount))
            {
                var transaction = qe.Session.CreateUniTransaction();

                var purchaseLedgerInvoiceRegistryObj = JObject.Parse(PurchaseLedgerInvoiceRegistryJSON);

                var purchaseLedgerInvoiceRegistryKey = purchaseLedgerInvoiceRegistryObj["PurchaseLedgerInvoiceRegistryKey"].Value<string>();

                var purchaseLedgerJSON = @"";

                var purchaseLedgerTransactionJSON = @"";

                var purchaseLedgerTransactionCurrencyJSON = @"";

                var supplierUpsertJSON = @"";

                var companyFileVATJSON = @"";

                var purchaseLedgerVATAuditJSON = @"";

                var companyStatsJSON = @"";

                var generalLedgerBatchJSON = @"";

                var generalLedgerBatchDataJSON = @"";

                var purchaseLedgerAdjustmentJSON = @"";

                var purchaseLedgerSupplierInvoiceJSON = @"";

                var purchaseLedgerPrePaymentTransactionJSON = @"";

                var session = qe.Session;

                try
                {
                    transaction.Begin();

                    // 0.
                    var purchaseLedgerUpsert = PurchaseLedgerFunctions.PurchaseLedgerFunctions.UpsertViaTransaction(ref session, purchaseLedgerJSON);

                    // 1.
                    var purchaseLedgerRegistryHistoryUpsert = PurchaseLedgerFunctions.PurchaseLedgerRegisterHistoryFunctions.UpsertViaTransaction(ref session, PurchaseLedgerInvoiceRegistryJSON);

                    // 2.
                    var purchaseLedgerRegistryDeletion = PurchaseLedgerFunctions.PurchaseLedgerInvoiceRegisterFunctions.DeleteViaTransaction(ref session, purchaseLedgerInvoiceRegistryKey);

                    // 3.
                    var purchaseLedgerTransactionUpsert = PurchaseLedgerFunctions.PurchaseLedgerTransactionFunctions.InsertViaTransaction(ref session, purchaseLedgerTransactionJSON);

                    // 4.
                    var purchaseLedgerTransactionCurrencyUpsert = PurchaseLedgerFunctions.PurchaseLedgerTransactionCurrencyFunctions.InsertViaTransaction(ref session, purchaseLedgerTransactionCurrencyJSON);

                    // 5.
                    var supplierUpsert = SupplierFunctions.SupplierFunctions.UpsertViaTransaction(ref session, supplierUpsertJSON);

                    // 6.
                    var supplierCurrencyUpsert = SupplierFunctions.SupplierCurrencyFunctions.UpsertViaTransaction(ref session, supplierUpsertJSON);

                    // 7.
                    var companyFilesVATUpsert = CompanyFileFunctions.CompanyVATFunctions.UpsertViaTransaction(ref session, companyFileVATJSON);

                    // 8.
                    var purchaseLedgerVATAuditUpsert = PurchaseLedgerFunctions.PurchaseLedgerVATAuditFunctions.UpsertViaTransaction(ref session, purchaseLedgerVATAuditJSON);

                    // 9.
                    var companyFilesStatsUpsert = CompanyFileFunctions.CompanyStatsFunctions.UpsertViaTranscation(ref session, companyStatsJSON);

                    // 10.
                    var generalLedgerBatchUpsert = GeneralLedgerFunctions.GeneralLedgerBatchFunctions.UpsertViaTransaction(ref session, generalLedgerBatchJSON);

                    // 11.
                    var generalLedgerBatchDataUpsert = GeneralLedgerFunctions.GeneralLedgerBatchDataFunctions.UpsertViaTransaction(ref session, generalLedgerBatchDataJSON);

                    // 12.
                    var purchaseLedgerAdjustmentUpsert = PurchaseLedgerFunctions.PurchaseLedgerAdjustmentAuditFunctions.UpsertViaTransaction(ref session, purchaseLedgerAdjustmentJSON);

                    // 13.
                    var purchaseLedgerSupplierInvoiceUpsert = PurchaseLedgerFunctions.PurchaseLedgerSupplierInvoiceFunctions.UpsertViaTransaction(ref session, purchaseLedgerSupplierInvoiceJSON);

                    // 14.
                    var generalLedgerPrePaymentTransactionUpsert = GeneralLedgerFunctions.GeneralLedgerPrePaymentTransactionFunctions.UpsertViaTransaction(ref session, purchaseLedgerPrePaymentTransactionJSON);
                    
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Exception occurred while moving from Purchase Invoice Register into Purchase Ledger: UniVerse Transaction has been rolled back.", ex);
                }

                transaction.Commit();

                transaction.Dispose();
            }



            var purchaseLedgerKey = "";

            var result = new PurchaseLedgerInsertionResult()
            {
                Key = purchaseLedgerKey
            };

            return new ArrayList(new List<PurchaseLedgerInsertionResult>() { result });
        }

        public static void PurhcaseLedger_InsertFromInvoiceRegistry_FillRow(
            object obj,
            out SqlString Key
            )
        {
            var insertionResult = (PurchaseLedgerInsertionResult)obj;

            Key = insertionResult.Key;
        }
    }
}
