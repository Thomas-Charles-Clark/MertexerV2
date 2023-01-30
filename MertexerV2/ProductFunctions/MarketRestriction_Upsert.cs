using MertexerV2.Models.Product;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.ProductFunctions
{
    public static partial class MarketRestrictionFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string MarketRestrictionJSON)
        {
            var marketRestriction = JsonConvert.DeserializeObject<MarketRestriction>(MarketRestrictionJSON);

            var fileName = typeof(MarketRestriction).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(marketRestriction.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(marketRestriction.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(marketRestriction.Key, 1);

                UniUpsert<MarketRestriction>.Upsert(marketRestriction, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(marketRestriction.Key);

            }

            return marketRestriction.Key;
        }
    }
}
