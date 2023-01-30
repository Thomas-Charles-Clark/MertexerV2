using MertexerV2.Models.Product;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.ProductFunctions
{
    public static partial class ProductGroupFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string ProductGroupJSON)
        {
            var productGroup = JsonConvert.DeserializeObject<ProductGroup>(ProductGroupJSON);

            var fileName = typeof(ProductGroup).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(productGroup.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(productGroup.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(productGroup.Key, 1);

                UniUpsert<ProductGroup>.Upsert(productGroup, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(productGroup.Key);

            }

            return productGroup.Key;
        }
    }
}
