using MertexerV2.Models.Pricing;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.PricingFunctions
{
    public static partial class FreeOfChargeAccountingCodeFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string FreeOfChargeAccountingCodeJSON)
        {
            var record = JsonConvert.DeserializeObject<FreeOfChargeAccountingCode>(FreeOfChargeAccountingCodeJSON);

            var fileName = typeof(FreeOfChargeAccountingCode).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(record.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(record.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(record.Key, 1);

                UniUpsert<FreeOfChargeAccountingCode>.Upsert(record, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(record.Key);

            }

            return record.Key;
        }
    }
}
