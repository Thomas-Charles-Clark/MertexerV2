using IBMU2.UODOTNET;

using MertexerV2.Models.Pricing;
using MertexerV2.Models.PurchaseLedger;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.PurchaseLedgerFunctions
{
    public static partial class PurchaseLedgerInvoiceRegisterFunctions
    {
        internal static string UpsertViaTransaction(ref UniSession session, string PurchaseLedgerInvoiceRegisterJSON)
        {
            var purchaseLedgerInvoiceRegisterEntry = JsonConvert.DeserializeObject<PurchaseLedgerInvoiceRegister>(PurchaseLedgerInvoiceRegisterJSON);

            var fileName = typeof(PurchaseLedgerInvoiceRegister).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            var uniVerseFile = session.CreateUniFile(fileName);

            try
            {
                var extantRecord = uniVerseFile.ReadField(purchaseLedgerInvoiceRegisterEntry.Key, 0).StringValue;
            }
            catch (UniFileException)
            {
                // extant record was not found: this is a creation request
                // create first (lock the entire file for as little time as possible)
                // then lock the specific record
                // then write all the fields
                // then unlock the record
                var dataSet = new UniDataSet(session);
                dataSet.Insert(purchaseLedgerInvoiceRegisterEntry.Key);

                //slcustFile.LockFile();
                uniVerseFile.WriteRecords(dataSet);
                //slcustFile.UnlockFile();
            }

            uniVerseFile.LockRecord(purchaseLedgerInvoiceRegisterEntry.Key, 1);

            UniUpsert<PurchaseLedgerInvoiceRegister>.Upsert(PurchaseLedgerInvoiceRegisterJSON, uniVerseFile);

            return purchaseLedgerInvoiceRegisterEntry.Key;
        }

        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string PurchaseLedgerInvoiceRegisterJSON)
        {
            var invoiceRegisterEntry = JsonConvert.DeserializeObject<PurchaseLedgerInvoiceRegister>(PurchaseLedgerInvoiceRegisterJSON);

            var fileName = typeof(PurchaseLedgerInvoiceRegister).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(invoiceRegisterEntry.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(invoiceRegisterEntry.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(invoiceRegisterEntry.Key, 1);

                UniUpsert<PurchaseLedgerInvoiceRegister>.Upsert(PurchaseLedgerInvoiceRegisterJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(invoiceRegisterEntry.Key);

            }

            return invoiceRegisterEntry.Key;
        }
    }
}
