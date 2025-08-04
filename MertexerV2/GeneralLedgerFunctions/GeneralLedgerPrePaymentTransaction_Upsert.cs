using IBMU2.UODOTNET;

using MertexerV2.Models.Credit;
using MertexerV2.Models.GeneralLedger;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.GeneralLedgerFunctions
{
    public static partial class GeneralLedgerPrePaymentTransactionFunctions
    {
        internal static string UpsertViaTransaction(ref UniSession session, string GeneralLedgerPrePaymentTransactionJSON)
        {
            var GeneralLedgerPrePaymentTransaction = JsonConvert.DeserializeObject<GeneralLedgerPrePaymentTransaction>(GeneralLedgerPrePaymentTransactionJSON);

            var fileName = typeof(GeneralLedgerPrePaymentTransaction).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            var uniVerseFile = session.CreateUniFile(fileName);

            try
            {
                var extantRecord = uniVerseFile.ReadField(GeneralLedgerPrePaymentTransaction.Key, 0).StringValue;
            }
            catch (IBMU2.UODOTNET.UniFileException)
            {
                // extant record was not found: this is a creation request
                // create first (lock the entire file for as little time as possible)
                // then lock the specific record
                // then write all the fields
                // then unlock the record
                var dataSet = new IBMU2.UODOTNET.UniDataSet(session);
                dataSet.Insert(GeneralLedgerPrePaymentTransaction.Key);

                //slcustFile.LockFile();
                uniVerseFile.WriteRecords(dataSet);
                //slcustFile.UnlockFile();
            }

            uniVerseFile.LockRecord(GeneralLedgerPrePaymentTransaction.Key, 1);

            UniUpsert<GeneralLedgerPrePaymentTransaction>.Upsert(GeneralLedgerPrePaymentTransactionJSON, uniVerseFile);

            uniVerseFile.UnlockRecord(GeneralLedgerPrePaymentTransaction.Key);


            return GeneralLedgerPrePaymentTransaction.Key;
        }

        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string GeneralLedgerPrePaymentTransactionJSON)
        {
            var GeneralLedgerPrePaymentTransaction = JsonConvert.DeserializeObject<GeneralLedgerPrePaymentTransaction>(GeneralLedgerPrePaymentTransactionJSON);

            var fileName = typeof(GeneralLedgerPrePaymentTransaction).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(GeneralLedgerPrePaymentTransaction.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(GeneralLedgerPrePaymentTransaction.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(GeneralLedgerPrePaymentTransaction.Key, 1);

                UniUpsert<GeneralLedgerPrePaymentTransaction>.Upsert(GeneralLedgerPrePaymentTransactionJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(GeneralLedgerPrePaymentTransaction.Key);

            }

            return GeneralLedgerPrePaymentTransaction.Key;
        }

        internal static string BuildKey(string documentReference, IEnumerable<string> generalLedgerCodes, IEnumerable<string> prePaymentGeneralLedgerCodes)
        {
            var result = "";

            // evidently this builds an eight-character long zero-left-padded string of numbers.

            // LIST GLPPTRANS.DES SAMPLE 20 tells us that much.



            return result;
        }
    }
}
