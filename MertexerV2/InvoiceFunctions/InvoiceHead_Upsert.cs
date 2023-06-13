using MertexerV2.Models.Invoices;
using MertexerV2.Models.Product;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.InvoiceFunctions
{
    public static partial class InvoiceHeadFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string InvoiceHeadJSON)
        {
            var invoiceHead = JsonConvert.DeserializeObject<InvoiceHead>(InvoiceHeadJSON);

            var fileName = typeof(InvoiceHead).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(invoiceHead.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(invoiceHead.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(invoiceHead.Key, 1);

                UniUpsert<InvoiceHead>.Upsert(InvoiceHeadJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(invoiceHead.Key);

            }

            return invoiceHead.Key;
        }
    }
}
