using MertexerV2.Models.General;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using MertexerV2.Models.ExchangeRate;

namespace MertexerV2.AccountFunctions
{
    public static partial class ExchangeRateFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string ExchangeRateJSON)
        {
            var ExchangeRate = JsonConvert.DeserializeObject<ExchangeRateCode>(ExchangeRateJSON);

            var fileName = typeof(ExchangeRate).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(ExchangeRate.Code, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(ExchangeRate.Code);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(ExchangeRate.Code, 1);

                UniUpsert<ExchangeRate>.Upsert(ExchangeRateJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(ExchangeRate.Code);

            }

            return ExchangeRate.Code;
        }
    }
}
