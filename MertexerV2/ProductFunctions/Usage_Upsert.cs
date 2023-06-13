using MertexerV2.Models.Product;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System.Linq;

namespace MertexerV2.ProductFunctions
{
    public static partial class UsageFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string UsageJSON)
        {
            var usage = JsonConvert.DeserializeObject<Usage>(UsageJSON);

            var fileName = typeof(Usage).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(usage.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(usage.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(usage.Key, 1);

                UniUpsert<Usage>.Upsert(UsageJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(usage.Key);

            }

            return usage.Key;
        }
    }
}
