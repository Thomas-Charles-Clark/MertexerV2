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
    public static partial class CompanyVATFunctions
    {
        internal static string UpsertViaTransaction(ref UniSession session, string CompanyVATJSON)
        {
            var CFVATRecord = JsonConvert.DeserializeObject<CompanyVAT>(CompanyVATJSON);

            var fileName = typeof(CompanyVAT).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            var uniVerseFile = session.CreateUniFile(fileName);

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
                var dataSet = new UniDataSet(session);
                dataSet.Insert(CFVATRecord.Key);

                //slcustFile.LockFile();
                uniVerseFile.WriteRecords(dataSet);
                //slcustFile.UnlockFile();
            }

            uniVerseFile.LockRecord(CFVATRecord.Key, 1);

            UniUpsert<CompanyVAT>.Upsert(CompanyVATJSON, uniVerseFile);

            uniVerseFile.UnlockRecord(CFVATRecord.Key);
            

            return CFVATRecord.Key;
        }

        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string CompanyVATJSON)
        {
            var CFVATRecord = JsonConvert.DeserializeObject<CompanyVAT>(CompanyVATJSON);

            var fileName = typeof(CompanyVAT).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
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

                UniUpsert<CompanyVAT>.Upsert(CompanyVATJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(CFVATRecord.Key);
            }

            return CFVATRecord.Key;
        }
    }
}
