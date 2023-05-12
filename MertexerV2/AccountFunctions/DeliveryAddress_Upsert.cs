using MertexerV2.Models;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.AccountFunctions
{
    public static partial class DeliveryAddressFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string DeliveryAddressJSON)
        {
            var DeliveryAddress = JsonConvert.DeserializeObject<DeliveryAddress>(DeliveryAddressJSON);

            var fileName = typeof(DeliveryAddress).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(DeliveryAddress.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(DeliveryAddress.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(DeliveryAddress.Key, 1);

                UniUpsert<DeliveryAddress>.Upsert(DeliveryAddress, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(DeliveryAddress.Key);

            }

            return DeliveryAddress.Key;
        }
    }
}
