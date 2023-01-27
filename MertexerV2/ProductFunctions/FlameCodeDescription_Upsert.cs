using MertexerV2.Models.Product;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System.Linq;

namespace MertexerV2.ProductFunctions
{
    public static partial class AdditionalFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string AdditionalJSON)
        {
            var additional = JsonConvert.DeserializeObject<FlameCodeDescription>(AdditionalJSON);

            var fileName = typeof(FlameCodeDescription).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(additional.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(additional.Key);
                    uniVerseFile.WriteRecords(dataSet);
                }

                uniVerseFile.LockRecord(additional.Key, 1);

                UniUpsert<FlameCodeDescription>.Upsert(additional, uniVerseFile);

                uniVerseFile.UnlockRecord(additional.Key);

            }

            return additional.Key;
        }
    }
}
