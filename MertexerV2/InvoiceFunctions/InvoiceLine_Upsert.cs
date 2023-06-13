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
    public static partial class InvoiceLineFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string InvoiceLineJSON)
        {
            var invoiceLine = JsonConvert.DeserializeObject<InvoiceLine>(InvoiceLineJSON);

            var fileName = typeof(InvoiceLine).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(invoiceLine.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request33
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(invoiceLine.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(invoiceLine.Key, 1);

                UniUpsert<InvoiceLine>.Upsert(InvoiceLineJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(invoiceLine.Key);

            }

            return invoiceLine.Key;
        }
    }
}
