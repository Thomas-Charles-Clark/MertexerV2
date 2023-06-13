using MertexerV2.Models.General;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.GeneralFunctions
{
    public static partial class OrderCategoryFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string OrderCategoryJSON)
        {
            var OrderCategory = JsonConvert.DeserializeObject<OrderCategoryCode>(OrderCategoryJSON);

            var fileName = typeof(OrderCategory).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(OrderCategory.Code, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(OrderCategory.Code);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(OrderCategory.Code, 1);

                UniUpsert<OrderCategory>.Upsert(OrderCategoryJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(OrderCategory.Code);

            }

            return OrderCategory.Code;
        }
    }
}
