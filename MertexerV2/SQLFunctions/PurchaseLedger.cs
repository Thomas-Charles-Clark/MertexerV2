
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using MertexerV2.Models.Credit;
using MertexerV2.Models.PurchaseLedger;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MertexerV2.SQLFunctions
{
    public static class PurchaseLedger
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "PurhcaseLedger_InsertFromInvoiceRegistry_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable InsertFromInvoiceRegistry(string UniVerseAccount, string PurchaseLedgerInvoiceRegistryJSON)
        {
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

                var session = qe.Session;

                try
                {
                    transaction.Begin();

                    var plreghist = PurchaseLedgerFunctions.PurchaseLedgerRegisterHistoryFunctions.Upsert(ref session, PurchaseLedgerInvoiceRegistryJSON);

                    var plinvregDeletion = PurchaseLedgerFunctions.PurchaseLedgerInvoiceRegisterFunctions.Delete(ref session, purchaseLedgerInvoiceRegistryKey);



                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Exception occurred while inserting into Purchase Ledger: UniVerse Transaction has been rolled back.", ex);
                }

                transaction.Commit();

                transaction.Dispose();
            }



            var purchaseLedgerKey = "";

            var result = new PurchaseLedgerInsertionResult()
            {
                Key = purchaseLedgerKey
            };

            throw new NotImplementedException();

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
