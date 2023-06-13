using MertexerV2.Models;
using MertexerV2.Models.ExchangeRate;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.AccountFunctions
{
    public static partial class InvoiceAddressFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string InvoiceAddressJSON)
        {
            var InvoiceAddress = JsonConvert.DeserializeObject<InvoiceAddress>(InvoiceAddressJSON);

            var fileName = typeof(InvoiceAddress).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    // If you've done something silly such as called this without a TradeAccountCode in the JSON, there'll be an uncaught exception thrown here because
                    // ReadField doesn't allow you to provide a NULL key. It doesn't throw a UniFileException though which is... just... crazy. But useful in this instance.
                    var extantRecord = uniVerseFile.ReadField(InvoiceAddress.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(InvoiceAddress.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(InvoiceAddress.Key, 1);

                UniUpsert<InvoiceAddress>.Upsert(InvoiceAddressJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(InvoiceAddress.Key);
            }

            return InvoiceAddress.Key;
        }
    }
}
