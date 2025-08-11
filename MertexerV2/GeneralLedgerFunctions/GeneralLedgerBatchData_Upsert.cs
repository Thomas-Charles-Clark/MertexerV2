using IBMU2.UODOTNET;

using MertexerV2.Models.Credit;
using MertexerV2.Models.GeneralLedger;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System.Linq;

namespace MertexerV2.GeneralLedgerFunctions
{
    public static partial class GeneralLedgerBatchDataFunctions
    {
        internal static string UpsertViaTransaction(ref UniSession session, string GeneralLedgerBatchDataJSON)
        {
            var GeneralLedgerBatchData = JsonConvert.DeserializeObject<GeneralLedgerBatchData>(GeneralLedgerBatchDataJSON);

            var fileName = typeof(GeneralLedgerBatchData).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            var uniVerseFile = session.CreateUniFile(fileName);

            try
            {
                var extantRecord = uniVerseFile.ReadField(GeneralLedgerBatchData.Key, 0).StringValue;
            }
            catch (IBMU2.UODOTNET.UniFileException)
            {
                // extant record was not found: this is a creation request
                // create first (lock the entire file for as little time as possible)
                // then lock the specific record
                // then write all the fields
                // then unlock the record
                var dataSet = new IBMU2.UODOTNET.UniDataSet(session);
                dataSet.Insert(GeneralLedgerBatchData.Key);

                //slcustFile.LockFile();
                uniVerseFile.WriteRecords(dataSet);
                //slcustFile.UnlockFile();
            }

            uniVerseFile.LockRecord(GeneralLedgerBatchData.Key, 1);

            UniUpsert<GeneralLedgerBatchData>.Upsert(GeneralLedgerBatchDataJSON, uniVerseFile);

            return GeneralLedgerBatchData.Key;
        }

        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string GeneralLedgerBatchDataJSON)
        {
            var GeneralLedgerBatchData = JsonConvert.DeserializeObject<GeneralLedgerBatchData>(GeneralLedgerBatchDataJSON);

            var fileName = typeof(GeneralLedgerBatchData).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(GeneralLedgerBatchData.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(GeneralLedgerBatchData.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(GeneralLedgerBatchData.Key, 1);

                UniUpsert<GeneralLedgerBatchData>.Upsert(GeneralLedgerBatchDataJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(GeneralLedgerBatchData.Key);

            }

            return GeneralLedgerBatchData.Key;
        }
    }
}
