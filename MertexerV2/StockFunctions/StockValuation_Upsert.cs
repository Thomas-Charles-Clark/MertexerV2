using MertexerV2.Models.Stock;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System.Linq;

namespace MertexerV2.StockFunctions
{
    public static partial class StockValuationFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string StockValuationJSON)
        {
            var stockValuation = JsonConvert.DeserializeObject<StockValuation>(StockValuationJSON);

            var fileName = typeof(StockValuation).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(stockValuation.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(stockValuation.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(stockValuation.Key, 1);

                UniUpsert<StockValuation>.Upsert(StockValuationJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(stockValuation.Key);

            }

            return stockValuation.Key;
        }
    }
}
