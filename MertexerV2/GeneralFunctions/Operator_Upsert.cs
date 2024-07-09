using MertexerV2.Models.General;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.GeneralFunctions
{
    public static partial class OperatorFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string OperatorJSON)
        {
            var Operator = JsonConvert.DeserializeObject<Operator>(OperatorJSON);

            var fileName = typeof(Operator).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(Operator.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(Operator.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(Operator.Key, 1);

                UniUpsert<Operator>.Upsert(OperatorJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(Operator.Key);

            }

            return Operator.Key;
        }
    }
}
