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
    public static partial class PurchaseLedgerTransactionCurrencyFunctions
    {
        internal static PurchaseLedgerTransactionCurrency InsertViaTransaction(ref UniSession session, string PurchaseLedgerTransactionCurrencyJSON)
        {
            var purchaseLedgerTransactionCurrencyEntry = JsonConvert.DeserializeObject<PurchaseLedgerTransactionCurrency>(PurchaseLedgerTransactionCurrencyJSON);

            var fileName = typeof(PurchaseLedgerTransactionCurrency).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

                var uniVerseFile = session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(purchaseLedgerTransactionCurrencyEntry.Key, 0).StringValue;

                    if (!String.IsNullOrEmpty(extantRecord))
                    {
                        throw new Exception($"{purchaseLedgerTransactionCurrencyEntry.Key} exists on {fileName}, INSERT fails");
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
                    dataSet.Insert(purchaseLedgerTransactionCurrencyEntry.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(purchaseLedgerTransactionCurrencyEntry.Key, 1);

                UniUpsert<PurchaseLedgerTransactionCurrency>.Upsert(PurchaseLedgerTransactionCurrencyJSON, uniVerseFile);

                uniVerseFile.UnlockRecord(purchaseLedgerTransactionCurrencyEntry.Key);

            return purchaseLedgerTransactionCurrencyEntry;
        }

        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "TransactionCurrencyInsert_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Insert(string UniVerseAccount, string PurchaseLedgerTransactionCurrencyJSON)
        {
            var purchaseLedgerEntry = JsonConvert.DeserializeObject<PurchaseLedgerTransactionCurrency>(PurchaseLedgerTransactionCurrencyJSON);

            var fileName = typeof(PurchaseLedgerTransactionCurrency).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
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

                UniUpsert<PurchaseLedgerTransactionCurrency>.Upsert(PurchaseLedgerTransactionCurrencyJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(purchaseLedgerEntry.Key);

            }

            return new ArrayList() { purchaseLedgerEntry };
        }

        public static void TransactionCurrencyInsert_FillRow(
            object obj,
            out SqlString Code
            )
        {
            var result = (PurchaseLedgerTransactionCurrency)obj;

            Code = result.Key.TrimReduceTruncate(50);
        }
    }
}
