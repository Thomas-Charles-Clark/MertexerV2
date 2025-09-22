using IBMU2.UODOTNET;

using MertexerV2.GeneralFunctions;
using MertexerV2.Models.Pricing;
using MertexerV2.Models.PurchaseLedger;
using MertexerV2.Models.Supplier;
using MertexerV2.SupplierFunctions;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.PurchaseLedgerFunctions
{
    public static partial class PurchaseLedgerInvoiceRegisterFunctions
    {
        internal static string InsertViaTransaction(ref UniSession session, string purchaseLedgerInvoiceRegisterJSON)
        {
            var invoiceRegisterEntry = JsonConvert.DeserializeObject<PurchaseLedgerInvoiceRegister>(purchaseLedgerInvoiceRegisterJSON);

            var fileName = typeof(PurchaseLedgerInvoiceRegister).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            var uniVerseFile = session.CreateUniFile(fileName);

            try
            {
                // We must ignore / not be presented with [Key] (and [RegisterKey]).
                // Instead we must determine which is the next key to use by reading a PLCONTROL <5> for record "GENNO". 
                // We must increment that number, save the record, and use that as our key.
                // This is because of PRUNIQUE calling REFGENKEY from within the PL subsystem.
                // Don't believe me? Don't care. Doing it anyway.
                
                var controlFile = session.CreateUniFile("PLCONTROL");

                var gennoRecords = controlFile.ReadRecords(new[] { "GENNO" });

                var gennoRecord = gennoRecords.GetRecord(0);

                var numberField = gennoRecord.Record.Extract(5);

                var currentKey = Convert.ToInt32(numberField.StringValue);

                var newKey = currentKey += 1;

                gennoRecord.Record.Replace(5, $"{newKey}");

                controlFile.WriteRecords(gennoRecords);

                invoiceRegisterEntry.Key = $"{newKey}";

                invoiceRegisterEntry.RegisterKey = invoiceRegisterEntry.Key;

                var extantRecord = uniVerseFile.ReadField(invoiceRegisterEntry.Key, 0).StringValue;

                if (!String.IsNullOrEmpty(extantRecord))
                {
                    throw new Exception($"{invoiceRegisterEntry.Key} exists on {fileName}, INSERT fails");
                }
            }
            catch (UniFileException)
            {
                // extant record was not found: this is a creation request
                // create first (lock the entire file for as little time as possible)
                // then lock the specific record
                // then write all the fields
                // then unlock the record
                var dataSet = new UniDataSet(session);
                dataSet.Insert(invoiceRegisterEntry.Key);

                //slcustFile.LockFile();
                uniVerseFile.WriteRecords(dataSet);
                //slcustFile.UnlockFile();
            }

            uniVerseFile.LockRecord(invoiceRegisterEntry.Key, 1);

            UniUpsert<PurchaseLedgerInvoiceRegister>.Upsert(purchaseLedgerInvoiceRegisterJSON, uniVerseFile);

            return invoiceRegisterEntry.Key;
        }

        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "InvoiceRegisterInsert_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Insert(string UniVerseAccount, string PurchaseLedgerInvoiceRegisterJSON)
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
                    var controlFile = ce.Session.CreateUniFile("PLCONTROL");

                    var gennoRecords = controlFile.ReadRecords(new[] { "GENNO" });

                    var gennoRecord = gennoRecords.GetRecord(0);

                    var numberField = gennoRecord.Record.Extract(5);

                    var currentKey = Convert.ToInt32(numberField.StringValue);

                    var newKey = currentKey += 1;

                    gennoRecord.Record.Replace(5, $"{newKey}");

                    controlFile.WriteRecords(gennoRecords);

                    invoiceRegisterEntry.Key = $"{newKey}";

                    invoiceRegisterEntry.RegisterKey = invoiceRegisterEntry.Key;

                    var extantRecord = uniVerseFile.ReadField(invoiceRegisterEntry.Key, 0).StringValue;

                    if (!String.IsNullOrEmpty(extantRecord))
                    {
                        throw new Exception($"{invoiceRegisterEntry.Key} exists on {fileName}, INSERT fails");
                    }
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

            return new ArrayList() { invoiceRegisterEntry };
        }

        public static void InvoiceRegisterInsert_FillRow(
            object obj,
            out SqlString Code
            )
        {
            var result = (PurchaseLedgerInvoiceRegister)obj;

            Code = result.Key.TrimReduceTruncate(50);
        }
    }
}
