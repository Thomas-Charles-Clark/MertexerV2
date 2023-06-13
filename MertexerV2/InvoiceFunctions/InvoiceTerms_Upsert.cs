using MertexerV2.Models.Invoices;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.TradeAccountFunctions
{
    public static partial class InvoiceTermsFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string InvoiceTermsJSON)
        {
            var invoiceTerms = JsonConvert.DeserializeObject<InvoiceTerms>(InvoiceTermsJSON);

            var fileName = typeof(InvoiceTerms).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(invoiceTerms.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(invoiceTerms.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(invoiceTerms.Key, 1);

                UniUpsert<InvoiceTerms>.Upsert(InvoiceTermsJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(invoiceTerms.Key);

            }

            return invoiceTerms.Key;
        }
    }
}
