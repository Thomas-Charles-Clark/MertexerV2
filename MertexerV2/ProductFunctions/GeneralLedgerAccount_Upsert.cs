using MertexerV2.Models.Pricing;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.ProductFunctions
{
    public static partial class GeneralLedgerAccountFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string GeneralLedgerAccountJSON)
        {
            var GeneralLedgerAccount = JsonConvert.DeserializeObject<Models.Pricing.GeneralLedgerAccountCode>(GeneralLedgerAccountJSON);

            var fileName = typeof(GeneralLedgerAccountCode).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(GeneralLedgerAccount.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(GeneralLedgerAccount.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(GeneralLedgerAccount.Key, 1);

                UniUpsert<GeneralLedgerAccountCode>.Upsert(GeneralLedgerAccountJSON, uniVerseFile, UniVerseAccount);
                
                uniVerseFile.UnlockRecord(GeneralLedgerAccount.Key);
            }

            return GeneralLedgerAccount.Key;
        }
    }
}
