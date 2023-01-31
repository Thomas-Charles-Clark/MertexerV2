using MertexerV2.Models.General;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.ProductFunctions
{
    public static partial class RadioTerminalUserFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string RadioTerminalUserJSON)
        {
            var RadioTerminalUser = JsonConvert.DeserializeObject<RadioTerminalUser>(RadioTerminalUserJSON);

            var fileName = typeof(RadioTerminalUser).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(RadioTerminalUser.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(RadioTerminalUser.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(RadioTerminalUser.Key, 1);

                UniUpsert<RadioTerminalUser>.Upsert(RadioTerminalUser, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(RadioTerminalUser.Key);

            }

            return RadioTerminalUser.Key;
        }
    }
}
