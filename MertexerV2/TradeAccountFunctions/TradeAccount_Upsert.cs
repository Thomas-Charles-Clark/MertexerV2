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
        public static string Upsert(string UniVerseAccount, string TradeAccountJSON)
        {
            var tradeAccount = JsonConvert.DeserializeObject<TradeAccount>(TradeAccountJSON);

            var fileName = typeof(TradeAccount).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

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
                }

                uniVerseFile.LockRecord(tradeAccount.Key, 1);

                UniUpsert<TradeAccount>.Upsert(TradeAccountJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(tradeAccount.Key);

//#if DEBUG
//                Console.WriteLine(indexRebuildResult);
//#endif
            }

            return tradeAccount.Key;
        }
    }
}
