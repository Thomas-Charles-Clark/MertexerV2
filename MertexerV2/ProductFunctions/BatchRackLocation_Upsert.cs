using MertexerV2.Models.Product;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System.Linq;

namespace MertexerV2.ProductFunctions
{
    public static partial class BatchRackLocationFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string BatchRackLocationJSON)
        {
            var BatchRackLocation = JsonConvert.DeserializeObject<BatchRackLocation>(BatchRackLocationJSON);

            var fileName = typeof(BatchRackLocation).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(BatchRackLocation.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(BatchRackLocation.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(BatchRackLocation.Key, 1);

                UniUpsert<BatchRackLocation>.Upsert(BatchRackLocationJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(BatchRackLocation.Key);

            }

            return BatchRackLocation.Key;
        }
    }
}
