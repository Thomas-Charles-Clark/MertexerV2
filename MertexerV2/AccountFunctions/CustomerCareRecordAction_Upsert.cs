using MertexerV2.Models.CustomerCare;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

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
        public static string UpsertAction(string UniVerseAccount, string CustomerCareRecordActionJSON)
        {
            var CustomerCareRecordAction = JsonConvert.DeserializeObject<CustomerCareRecordAction>(CustomerCareRecordActionJSON);

            var fileName = typeof(CustomerCareRecordAction).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(CustomerCareRecordAction.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(CustomerCareRecordAction.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(CustomerCareRecordAction.Key, 1);

                UniUpsert<CustomerCareRecordAction>.Upsert(CustomerCareRecordActionJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(CustomerCareRecordAction.Key);

            }

            return CustomerCareRecordAction.Key;
        }
    }
}
