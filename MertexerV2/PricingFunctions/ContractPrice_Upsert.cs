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
    public static partial class ContractPriceFunctions
    {
        [SqlFunction]
        public static bool Upsert(string UniVerseAccount, string ContractPriceJSON)
        {
            var contractPrices = JsonConvert.DeserializeObject<IEnumerable<ContractPrice>>(ContractPriceJSON);

            foreach( var contractPrice in  contractPrices )
            {
                var fileName = typeof(ContractPrice).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                        .Cast<UniVerseFileNameAttribute>()
                        .Single()
                        .FileName;

                using (var ce = new CommandExecutor(UniVerseAccount))
                {
                    var uniVerseFile = ce.Session.CreateUniFile(fileName);

                    try
                    {
                        var extantRecord = uniVerseFile.ReadField(contractPrice.Key, 0).StringValue;
                    }
                    catch (IBMU2.UODOTNET.UniFileException)
                    {
                        // extant record was not found: this is a creation request
                        // create first (lock the entire file for as little time as possible)
                        // then lock the specific record
                        // then write all the fields
                        // then unlock the record
                        var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);

                        dataSet.Insert(contractPrice.Key);

                        uniVerseFile.WriteRecords(dataSet);
                    }

                    uniVerseFile.LockRecord(contractPrice.Key, 1);

                    UniUpsert<ContractPrice>.Upsert(JsonConvert.SerializeObject(contractPrice), uniVerseFile, UniVerseAccount);

                    uniVerseFile.UnlockRecord(contractPrice.Key);

                }
            }

            return false;
        }
    }
}
