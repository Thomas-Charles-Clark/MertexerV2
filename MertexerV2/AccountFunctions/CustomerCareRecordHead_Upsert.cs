using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using MertexerV2.Models.CustomerCare;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.AccountFunctions
{
    public static partial class CustomerCareRecordFunctions
    {
        [SqlFunction]
        public static string UpsertHead(string UniVerseAccount, string CustomerCareRecordHeadJSON)
        {
            var CustomerCareRecordHead = JsonConvert.DeserializeObject<CustomerCareRecordHead>(CustomerCareRecordHeadJSON);

            var fileName = typeof(CustomerCareRecordHead).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(CustomerCareRecordHead.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(CustomerCareRecordHead.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(CustomerCareRecordHead.Key, 1);

                UniUpsert<CustomerCareRecordHead>.Upsert(CustomerCareRecordHeadJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(CustomerCareRecordHead.Key);

            }

            return CustomerCareRecordHead.Key;
        }
    }
}
