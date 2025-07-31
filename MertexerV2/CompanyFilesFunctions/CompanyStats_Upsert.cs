using IBMU2.UODOTNET;

using MertexerV2.Models.Company;
using MertexerV2.Models.Product;
using MertexerV2.Models.Supplier;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.CompanyFileFunctions
{
    public static partial class CompanyStatsFunctions
    {
        internal static string UpsertViaTranscation(ref UniSession session, string CompanyStatsJSON)
        {
            var cfStatsRecord = JsonConvert.DeserializeObject<CompanyStats>(CompanyStatsJSON);

            var fileName = typeof(CompanyStats).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            var uniVerseFile = session.CreateUniFile(fileName);

            try
            {
                var extantRecord = uniVerseFile.ReadField(cfStatsRecord.Key, 0).StringValue;
            }
            catch (UniFileException)
            {
                // extant record was not found: this is a creation request
                // create first (lock the entire file for as little time as possible)
                // then lock the specific record
                // then write all the fields
                // then unlock the record
                var dataSet = new UniDataSet(session);
                dataSet.Insert(cfStatsRecord.Key);

                //slcustFile.LockFile();
                uniVerseFile.WriteRecords(dataSet);
                //slcustFile.UnlockFile();
            }

            uniVerseFile.LockRecord(cfStatsRecord.Key, 1);

            UniUpsert<CompanyStats>.Upsert(CompanyStatsJSON, uniVerseFile);

            uniVerseFile.UnlockRecord(cfStatsRecord.Key);
            

            return cfStatsRecord.Key;
        }

        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string CompanyStatsJSON)
        {
            var CFVATRecord = JsonConvert.DeserializeObject<CompanyStats>(CompanyStatsJSON);

            var fileName = typeof(CompanyStats).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(CFVATRecord.Key, 0).StringValue;
                }
                catch (UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(CFVATRecord.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(CFVATRecord.Key, 1);

                UniUpsert<CompanyStats>.Upsert(CompanyStatsJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(CFVATRecord.Key);
            }

            return CFVATRecord.Key;
        }
    }
}
