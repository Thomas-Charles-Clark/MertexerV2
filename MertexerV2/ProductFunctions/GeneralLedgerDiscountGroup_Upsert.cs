using MertexerV2.Models.Pricing;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.ProductFunctions
{
    public static partial class GeneralLedgerDiscountGroupFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string GeneralLedgerDiscountCodeGroupJSON)
        {
            var GeneralLedgerDiscountCodeGroup = JsonConvert.DeserializeObject<GeneralLedgerDiscountGroup>(GeneralLedgerDiscountCodeGroupJSON);

            var fileName = typeof(GeneralLedgerDiscountGroup).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(GeneralLedgerDiscountCodeGroup.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record2
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(GeneralLedgerDiscountCodeGroup.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(GeneralLedgerDiscountCodeGroup.Key, 1);

                UniUpsert<GeneralLedgerDiscountGroup>.Upsert(GeneralLedgerDiscountCodeGroup, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(GeneralLedgerDiscountCodeGroup.Key);

            }

            return GeneralLedgerDiscountCodeGroup.Key;
        }
    }
}
