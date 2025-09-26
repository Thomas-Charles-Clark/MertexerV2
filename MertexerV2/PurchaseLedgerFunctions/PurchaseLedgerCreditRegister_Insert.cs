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
    public static partial class PurchaseLedgerCreditRegisterFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "CreditRegisterInsert_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Insert(string UniVerseAccount, string PurchaseLedgerCreditRegisterJSON)
        {
            var creditRegisterEntry = JsonConvert.DeserializeObject<PurchaseLedgerCreditRegister>(PurchaseLedgerCreditRegisterJSON);

            var fileName = typeof(PurchaseLedgerCreditRegister).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
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

                    var fieldPosition = 8;

                    var numberField = gennoRecord.Record.Extract(fieldPosition);

                    var currentKey = Convert.ToInt32(numberField.StringValue);

                    var newKey = currentKey += 1;

                    gennoRecord.Record.Replace(fieldPosition, $"{newKey}");

                    controlFile.WriteRecords(gennoRecords);

                    creditRegisterEntry.Key = $"{newKey}";

                    creditRegisterEntry.RegisterKey = creditRegisterEntry.Key;

                    var extantRecord = uniVerseFile.ReadField(creditRegisterEntry.Key, 0).StringValue;

                    if (!String.IsNullOrEmpty(extantRecord))
                    {
                        throw new Exception($"{creditRegisterEntry.Key} exists on {fileName}, INSERT fails");
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
                    dataSet.Insert(creditRegisterEntry.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(creditRegisterEntry.Key, 1);

                UniUpsert<PurchaseLedgerCreditRegister>.Upsert(PurchaseLedgerCreditRegisterJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(creditRegisterEntry.Key);

            }

            return new ArrayList() { creditRegisterEntry };
        }

        public static void CreditRegisterInsert_FillRow(
            object obj,
            out SqlString Code
            )
        {
            var result = (PurchaseLedgerCreditRegister)obj;

            Code = result.Key.TrimReduceTruncate(50);
        }
    }
}
