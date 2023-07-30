using MertexerV2.Models;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.TradeAccountFunctions
{
    public static partial class TradeAccountFunctions
    {
        [SqlFunction]
        public static string InsertTransactionHistory(string UniVerseAccount, string TradeAccountTransactionHistoryJSON)
        {
            var tradeAccount = JsonConvert.DeserializeObject<TradeAccountTransactionHistory>(TradeAccountTransactionHistoryJSON);

            var fileName = typeof(TradeAccountTransactionHistory).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                var caughtException = false;

                try
                {
                    var extantRecord = uniVerseFile.ReadField(tradeAccount.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);

                    dataSet.Insert(tradeAccount.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                    caughtException = true;
                }

                if (!caughtException)
                {
                    throw new Exception("Extant record detected: updating Transaction History records from SQL is not allowed.");
                }

                uniVerseFile.LockRecord(tradeAccount.Key, 1);

                UniUpsert<TradeAccountTransactionHistory>.Upsert(TradeAccountTransactionHistoryJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(tradeAccount.Key);
            }

            return tradeAccount.Key;
        }
    }
}
