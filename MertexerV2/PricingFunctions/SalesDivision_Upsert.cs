using MertexerV2.Models.Pricing;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.PricingFunctions
{
    public static partial class SalesDivisionFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string SalesDivisionJSON)
        {
            var salesDivision = JsonConvert.DeserializeObject<SalesDivision>(SalesDivisionJSON);

            var fileName = typeof(SalesDivision).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(salesDivision.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(salesDivision.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(salesDivision.Key, 1);

                UniUpsert<SalesDivision>.Upsert(SalesDivisionJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(salesDivision.Key);

            }

            return salesDivision.Key;
        }
    }
}
