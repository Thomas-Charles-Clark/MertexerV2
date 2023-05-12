using MertexerV2.Models.Product;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.ProductFunctions
{
    public static partial class ShippingTermsFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string ShippingTermsJSON)
        {
            var ShippingTerms = JsonConvert.DeserializeObject<ShippingTerms>(ShippingTermsJSON);

            var fileName = typeof(ShippingTerms).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(ShippingTerms.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(ShippingTerms.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(ShippingTerms.Key, 1);

                UniUpsert<ShippingTerms>.Upsert(ShippingTerms, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(ShippingTerms.Key);

            }

            return ShippingTerms.Key;
        }
    }
}
