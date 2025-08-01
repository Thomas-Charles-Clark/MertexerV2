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
    public static partial class PurchaseLedgerFunctions
    {
        internal static PurchaseLedger InsertViaTransaction(ref UniSession session, string PurchaseLedgerJSON)
        {
            var PurchaseLedgerEntry = JsonConvert.DeserializeObject<PurchaseLedger>(PurchaseLedgerJSON);

            var fileName = typeof(PurchaseLedger).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

                var uniVerseFile = session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(PurchaseLedgerEntry.Key, 0).StringValue;

                    if (!String.IsNullOrEmpty(extantRecord))
                    {
                        throw new Exception($"{PurchaseLedgerEntry.Key} exists on {fileName}, INSERT fails");
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
                    dataSet.Insert(PurchaseLedgerEntry.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(PurchaseLedgerEntry.Key, 1);

                UniUpsert<PurchaseLedger>.Upsert(PurchaseLedgerJSON, uniVerseFile);

                uniVerseFile.UnlockRecord(PurchaseLedgerEntry.Key);

            return PurchaseLedgerEntry;
        }

        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "PurchaseLedgerInsert_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Insert(string UniVerseAccount, string PurchaseLedgerJSON)
        {
            var purchaseLedgerEntry = JsonConvert.DeserializeObject<PurchaseLedger>(PurchaseLedgerJSON);

            var fileName = typeof(PurchaseLedger).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
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

                UniUpsert<PurchaseLedger>.Upsert(PurchaseLedgerJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(purchaseLedgerEntry.Key);

            }

            return new ArrayList() { purchaseLedgerEntry };
        }

        public static void PurchaseLedgerInsert_FillRow(
            object obj,
            out SqlString Code
            )
        {
            var result = (PurchaseLedger)obj;

            Code = result.Key.TrimReduceTruncate(50);
        }
    }
}
