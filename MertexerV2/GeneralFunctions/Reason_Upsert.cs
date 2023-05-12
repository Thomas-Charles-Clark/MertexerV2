using MertexerV2.Models.General;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.GeneralFunctions
{
    public static partial class ReasonFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string ReasonJSON)
        {
            var Reason = JsonConvert.DeserializeObject<ReasonCode>(ReasonJSON);

            var fileName = typeof(Reason).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(Reason.Code, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(Reason.Code);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(Reason.Code, 1);

                UniUpsert<Reason>.Upsert(Reason, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(Reason.Code);

            }

            return Reason.Code;
        }
    }
}
