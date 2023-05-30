using MertexerV2.Models.General;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.GeneralFunctions
{
    public static partial class DeliveryTermsFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string DeliveryTermsJSON)
        {
            var DeliveryTerms = JsonConvert.DeserializeObject<DeliveryTermsCode>(DeliveryTermsJSON);

            var fileName = typeof(DeliveryTerms).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(DeliveryTerms.Code, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(DeliveryTerms.Code);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(DeliveryTerms.Code, 1);

                UniUpsert<DeliveryTerms>.Upsert(DeliveryTerms, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(DeliveryTerms.Code);

            }

            return DeliveryTerms.Code;
        }
    }
}
