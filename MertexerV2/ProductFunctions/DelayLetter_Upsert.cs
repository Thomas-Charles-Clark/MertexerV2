using MertexerV2.Models.Product;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.ProductFunctions
{
    public static partial class DelayLetterFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string DelayLetterJSON)
        {
            var DelayLetter = JsonConvert.DeserializeObject<DelayLetter>(DelayLetterJSON);

            var fileName = typeof(DelayLetter).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(DelayLetter.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(DelayLetter.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(DelayLetter.Key, 1);

                UniUpsert<DelayLetter>.Upsert(DelayLetterJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(DelayLetter.Key);

            }

            return DelayLetter.Key;
        }
    }
}
