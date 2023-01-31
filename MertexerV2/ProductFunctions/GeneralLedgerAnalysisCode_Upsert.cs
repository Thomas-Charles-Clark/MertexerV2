using MertexerV2.Models.Pricing;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.ProductFunctions
{
    public static partial class GeneralLedgerAnalysisCodeFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAnalysis, string GeneralLedgerAnalysisJSON)
        {
            var GeneralLedgerAnalysis = JsonConvert.DeserializeObject<Models.Pricing.GeneralLedgerAnalysisCode>(GeneralLedgerAnalysisJSON);

            var fileName = typeof(GeneralLedgerAnalysisCode).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAnalysis))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(GeneralLedgerAnalysis.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(GeneralLedgerAnalysis.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(GeneralLedgerAnalysis.Key, 1);

                UniUpsert<GeneralLedgerAnalysisCode>.Upsert(GeneralLedgerAnalysis, uniVerseFile, UniVerseAnalysis);

                uniVerseFile.UnlockRecord(GeneralLedgerAnalysis.Key);
            }

            return GeneralLedgerAnalysis.Key;
        }
    }
}
