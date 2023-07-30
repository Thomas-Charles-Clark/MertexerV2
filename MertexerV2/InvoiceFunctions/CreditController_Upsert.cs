using MertexerV2.Models.Credit;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System.Linq;

namespace MertexerV2.InvoiceFunctions
{
    public static partial class CreditControllerFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string CreditControllerJSON)
        {
            var CreditController = JsonConvert.DeserializeObject<CreditController>(CreditControllerJSON);

            var fileName = typeof(CreditController).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(CreditController.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(CreditController.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(CreditController.Key, 1);

                UniUpsert<CreditController>.Upsert(CreditControllerJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(CreditController.Key);

            }

            return CreditController.Key;
        }
    }
}
