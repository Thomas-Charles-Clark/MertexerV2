using IBMU2.UODOTNET;

using MertexerV2.Models.Credit;
using MertexerV2.Models.GeneralLedger;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System.Linq;

namespace MertexerV2.GeneralLedgerFunctions
{
    public static partial class GeneralLedgerBatchFunctions
    {
        internal static string UpsertViaTransaction(ref UniSession session, string GeneralLedgerBatchJSON)
        {
            var generalLedgerBatch = JsonConvert.DeserializeObject<GeneralLedgerBatch>(GeneralLedgerBatchJSON);

            var fileName = typeof(GeneralLedgerBatch).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            var uniVerseFile = session.CreateUniFile(fileName);

            try
            {
                var extantRecord = uniVerseFile.ReadField(generalLedgerBatch.Key, 0).StringValue;
            }
            catch (IBMU2.UODOTNET.UniFileException)
            {
                // extant record was not found: this is a creation request
                // create first (lock the entire file for as little time as possible)
                // then lock the specific record
                // then write all the fields
                // then unlock the record
                var dataSet = new IBMU2.UODOTNET.UniDataSet(session);
                dataSet.Insert(generalLedgerBatch.Key);

                //slcustFile.LockFile();
                uniVerseFile.WriteRecords(dataSet);
                //slcustFile.UnlockFile();
            }

            uniVerseFile.LockRecord(generalLedgerBatch.Key, 1);

            UniUpsert<GeneralLedgerBatch>.Upsert(GeneralLedgerBatchJSON, uniVerseFile);

            uniVerseFile.UnlockRecord(generalLedgerBatch.Key);


            return generalLedgerBatch.Key;
        }

        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string GeneralLedgerBatchJSON)
        {
            var generalLedgerBatch = JsonConvert.DeserializeObject<GeneralLedgerBatch>(GeneralLedgerBatchJSON);

            var fileName = typeof(GeneralLedgerBatch).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(generalLedgerBatch.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(generalLedgerBatch.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(generalLedgerBatch.Key, 1);

                UniUpsert<GeneralLedgerBatch>.Upsert(GeneralLedgerBatchJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(generalLedgerBatch.Key);

            }

            return generalLedgerBatch.Key;
        }
    }
}
