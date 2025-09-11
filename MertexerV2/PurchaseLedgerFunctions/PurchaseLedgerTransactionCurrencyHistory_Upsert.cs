using IBMU2.UODOTNET;

using MertexerV2.Models.Pricing;
using MertexerV2.Models.PurchaseLedger;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.PurchaseLedgerFunctions
{
    public static partial class PurchaseLedgerTransactionCurrencyHistoryFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string PurchaseLedgerTransactionJSON)
        {
            var purchaseLedgerEntry = JsonConvert.DeserializeObject<PurchaseLedgerTransactionCurrencyHistory>(PurchaseLedgerTransactionJSON);

            var fileName = typeof(PurchaseLedgerTransactionCurrencyHistory).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(purchaseLedgerEntry.Key, 0).StringValue;
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

                UniUpsert<PurchaseLedgerTransactionCurrencyHistory>.Upsert(PurchaseLedgerTransactionJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(purchaseLedgerEntry.Key);

            }

            return purchaseLedgerEntry.Key;
        }
    }
}
