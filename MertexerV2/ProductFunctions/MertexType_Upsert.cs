using MertexerV2.Models.Product;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.ProductFunctions
{
    public static partial class MertexTypeFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string MertexTypeJSON)
        {
            var mertexType = JsonConvert.DeserializeObject<MertexType>(MertexTypeJSON);

            var fileName = typeof(MertexType).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(mertexType.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(mertexType.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(mertexType.Key, 1);

                UniUpsert<MertexType>.Upsert(mertexType, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(mertexType.Key);

            }

            return mertexType.Key;
        }
    }
}
