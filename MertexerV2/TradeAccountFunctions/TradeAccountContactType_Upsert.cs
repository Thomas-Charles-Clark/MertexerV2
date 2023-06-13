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
    public static partial class TradeAccountContactTypeFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string TradeAccountContactTypeJSON)
        {
            var tradeAccountContactType = JsonConvert.DeserializeObject<TradeAccountContactType>(TradeAccountContactTypeJSON);

            var fileName = typeof(TradeAccountContactType).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(tradeAccountContactType.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(tradeAccountContactType.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(tradeAccountContactType.Key, 1);

                UniUpsert<TradeAccountContactType>.Upsert(TradeAccountContactTypeJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(tradeAccountContactType.Key);

            }

            return tradeAccountContactType.Key;
        }
    }
}
