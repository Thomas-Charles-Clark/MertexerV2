using IBMU2.UODOTNET;

using MertexerV2.GeneralFunctions;
using MertexerV2.Models.Pricing;
using MertexerV2.Models.PurchaseLedger;
using MertexerV2.Models.Supplier;
using MertexerV2.SupplierFunctions;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.PurchaseLedgerFunctions
{
    public static partial class PurchaseLedgerAdjustmentAuditFunctions
    {
        internal static PurchaseLedgerAdjustmentAudit InsertViaTransaction(ref UniSession session, string PurchaseLedgerAdjustmentAuditJSON)
        {
            var purchaseLedgerAdjustmentAuditEntry = JsonConvert.DeserializeObject<PurchaseLedgerAdjustmentAudit>(PurchaseLedgerAdjustmentAuditJSON);

            var fileName = typeof(PurchaseLedgerAdjustmentAudit).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            var uniVerseFile = session.CreateUniFile(fileName);

            try
            {
                var extantRecord = uniVerseFile.ReadField(purchaseLedgerAdjustmentAuditEntry.Key, 0).StringValue;

                if (!String.IsNullOrEmpty(extantRecord))
                {
                    throw new Exception($"{purchaseLedgerAdjustmentAuditEntry.Key} exists on {fileName}, INSERT fails");
                }
            }
            catch (IBMU2.UODOTNET.UniFileException)
            {
                // extant record was not found: this is a creation request
                // create first (lock the entire file for as little time as possible)
                // then lock the specific record
                // then write all the fields
                // then unlock the record
                var dataSet = new IBMU2.UODOTNET.UniDataSet(session);
                dataSet.Insert(purchaseLedgerAdjustmentAuditEntry.Key);

                //slcustFile.LockFile();
                uniVerseFile.WriteRecords(dataSet);
                //slcustFile.UnlockFile();
            }

            uniVerseFile.LockRecord(purchaseLedgerAdjustmentAuditEntry.Key, 1);

            UniUpsert<PurchaseLedgerAdjustmentAudit>.Upsert(PurchaseLedgerAdjustmentAuditJSON, uniVerseFile);

            return purchaseLedgerAdjustmentAuditEntry;
        }

        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "AdjustmentAuditInsert_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Insert(string UniVerseAccount, string PurchaseLedgerAdjustmentAuditJSON)
        {
            var purchaseLedgerEntry = JsonConvert.DeserializeObject<PurchaseLedgerAdjustmentAudit>(PurchaseLedgerAdjustmentAuditJSON);

            var fileName = typeof(PurchaseLedgerAdjustmentAudit).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(purchaseLedgerEntry.Key, 0).StringValue;

                    if (!String.IsNullOrEmpty(extantRecord))
                    {
                        throw new Exception($"{purchaseLedgerEntry.Key} exists on {fileName}, INSERT fails");
                    }
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(purchaseLedgerEntry.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(purchaseLedgerEntry.Key, 1);

                UniUpsert<PurchaseLedgerAdjustmentAudit>.Upsert(PurchaseLedgerAdjustmentAuditJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(purchaseLedgerEntry.Key);

            }

            return new ArrayList() { purchaseLedgerEntry };
        }

        public static void AdjustmentAuditInsert_FillRow(
            object obj,
            out SqlString Code
            )
        {
            var result = (PurchaseLedgerAdjustmentAudit)obj;

            Code = result.Key.TrimReduceTruncate(50);
        }
    }
}
