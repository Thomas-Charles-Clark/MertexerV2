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
    public static partial class PurchaseLedgerRegisterHistoryFunctions
    {
        public static string Upsert(ref UniSession session, string PurchaseLedgerRegisterHistoryJSON)
        {
            var purchaseLedgerHistoryRegisterEntry = JsonConvert.DeserializeObject<PurchaseLedgerRegisterHistory>(PurchaseLedgerRegisterHistoryJSON);

            var fileName = typeof(PurchaseLedgerRegisterHistory).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            var uniVerseFile = session.CreateUniFile(fileName);

            try
            {
                var extantRecord = uniVerseFile.ReadField(purchaseLedgerHistoryRegisterEntry.Key, 0).StringValue;
            }
            catch (UniFileException)
            {
                // extant record was not found: this is a creation request
                // create first (lock the entire file for as little time as possible)
                // then lock the specific record
                // then write all the fields
                // then unlock the record
                var dataSet = new UniDataSet(session);
                dataSet.Insert(purchaseLedgerHistoryRegisterEntry.Key);

                //slcustFile.LockFile();
                uniVerseFile.WriteRecords(dataSet);
                //slcustFile.UnlockFile();
            }

            uniVerseFile.LockRecord(purchaseLedgerHistoryRegisterEntry.Key, 1);

            UniUpsert<PurchaseLedgerRegisterHistory>.Upsert(PurchaseLedgerRegisterHistoryJSON, uniVerseFile, session.Account);

            uniVerseFile.UnlockRecord(purchaseLedgerHistoryRegisterEntry.Key);

            return purchaseLedgerHistoryRegisterEntry.Key;
        }
    }
}
