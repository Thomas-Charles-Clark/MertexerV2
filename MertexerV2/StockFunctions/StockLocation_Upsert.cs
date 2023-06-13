using MertexerV2.Models.Stock;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System.Linq;

namespace MertexerV2.StockFunctions
{
    public static partial class StockLocationFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string StockLocationJSON)
        {
            var stockLocation = JsonConvert.DeserializeObject<StockLocation>(StockLocationJSON);

            var fileName = typeof(StockLocation).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(stockLocation.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(stockLocation.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(stockLocation.Key, 1);

                UniUpsert<StockLocation>.Upsert(StockLocationJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(stockLocation.Key);

            }

            return stockLocation.Key;
        }
    }
}
