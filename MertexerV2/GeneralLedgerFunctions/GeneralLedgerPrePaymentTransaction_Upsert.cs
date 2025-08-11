using IBMU2.UODOTNET;

using MertexerV2.Models.Credit;
using MertexerV2.Models.GeneralLedger;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
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

        /*
         * 

            ÚÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄProcess Defn - ParagraphÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄ¿ 
            ³ Process Name    PG.BLD.GLPPTRANS.KEY                                        ³ 
            ³ Description     _________________________________                           ³ 
            ³ Dict File Name                                                              ³ 
            ³ ÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄ ³ 
            ³ Paragraph Lines                                                             ³ 
            ³ LOCAL L.TYPE, L.TRANSNO, L.GLCODES, L.GLAMOUNT, L.GLMEMOS, L.PP.GLCODES     ³ 
            ³ LOCAL L.KEYS, L.POS, L.MAX                                                  ³ 
            ³ *                                                                           ³ 
            ³ L.TYPE = @PARAM                                                             ³ 
            ³ @RTN.FLAG = 0                                                               ³ 
            ³ *                                                                           ³ 
            ³ BEGIN CASE                                                                  ³ 
            ³ CASE L.TYPE = "PIR" ;* Purchase Invoice Register                            ³ 
            ³    L.TRANSNO  = <53>                                                        ³ 
            ³    L.GLCODES  = <44>                                                        ³ 
            ³ CASE L.TYPE = "PI"  ;* Purchase Invoice                                     ³ 
            ³    L.TRANSNO  = <53>                                                        ³ 
            ³    L.GLCODES  = <44>                                                        ³ 
            ³ CASE L.TYPE = "CNR" ;* Credit Note Register                                 ³ 
            ³    L.TRANSNO  = <53>_______________________________________________________ ³ 
            ³    L.GLCODES  = <44>                                                        ³ 
            ³ CASE L.TYPE = "CRN" ;* Credit Note                                          ³ 
            ³    L.TRANSNO  = <53>                                                        ³ 
            ³    L.GLCODES  = <44>                                                        ³ 
            ³ CASE 1                                                                      ³ 
            ³    L.TRANSNO  = ""                                                          ³ 
            ³    L.GLCODES  = ""                                                          ³ 
            ³ END CASE                                                                    ³ 
            ³ *                                                                           ³ 
            ³ L.PP.GLCODES = F("COMPANY","1")<205>                                        ³ 
            ³ *                                                                           ³ 
            ³ L.KEYS = ""                                                                 ³ 
            ³ *                                                                           ³ 
            ³ DISP 9,"Checking for Prepayments..."_______________________________________ ³ 
            ³ L.POS = 1                                                                   ³ 
            ³ L.MAX = DCOUNT(L.GLCODES,@VM)                                               ³ 
            ³ WHILE L.POS <= L.MAX DO                                                     ³ 
            ³    IF LOC(L.GLCODES<1,L.POS>,L.PP.GLCODES,@VM) THEN                         ³ 
            ³       L.KEYS<1,L.POS> = B("RFGENKEY,GLPPTRANS.DES&GL:4&8")                  ³ 
            ³    END                                                                      ³ 
            ³    L.POS = L.POS + 1                                                        ³ 
            ³ REPEAT                                                                      ³ 
            ³ DISP 9,""                                                                   ³ 
            ³ *                                                                           ³ 
            ³ @VALUE = L.KEYS                                                             ³ 
            ³                                                                             ³ 
            ³                                                                             ³ 
            ³ Source Code Id                                                              ³ 
            ÀÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÄÙ 
         */
        /// <summary>
        /// Builds the General Ledger PrePaid Transaction Key
        /// </summary>
        /// <param name="documentReference">The document reference we're handling</param>
        /// <param name="generalLedgerCodes">The General Ledger Codes given to this document by the user processing it</param>
        /// <param name="generalLedgerPrePaymentCodes">The PrePayment General Ledger Codes defined in our COMPANY file under record with key "1".</param>
        /// <returns></returns>
        internal static IEnumerable<string> BuildKeysViaTransaction(
            ref UniSession session,  
            IEnumerable<string> generalLedgerCodes, 
            IEnumerable<string> generalLedgerPrePaymentCodes)
        {
            // evidently this builds an eight-character long zero-left-padded string of numbers.

            // LIST GLPPTRANS.DES SAMPLE 20 tells us that much.

            // /PD P PG.BLD.GLPPTRANS.KEY  -- given param "PIR"
            // 
            // a case statement is in place which switches on param, in all identified cases it causes TRANSNO  and GLCODES to be lifted from the same
            // two fields on TPLEDG : 53 and 44 respectively.
            // L.PP.GLCODES is lifted from COMPANY<205>.
            // Therefore I have omitted those steps from this procedure and parameterised their results as inputs.

            // We begin at 'DISP 9,"Checking for Prepayments..."
            //

            // we are building a single value "L.KEYS" as a return; this... is... initially an array? Ah, no, it's sort of treated as
            // an MV field which is building a string.

            // "CONTROL.FILE" = GLCONTROL
            // ACCNO = 4 // accumulator field number
            // PADDING = 8
            // so this reads field <4> from GLCONTROl which (at least presently) contains '16'.

            var keys = new List<string>();

            var glppTransDesFile = session.CreateUniFile("GLPPTRANS.DES");

            for (var i = 0; i < generalLedgerCodes.Count(); i++)
            {
                var glCode = generalLedgerCodes.ElementAt(i);

                if (generalLedgerPrePaymentCodes.Contains(glCode))
                {


                    // keys.Add(B("RFGENKEY,GLPPTRANS.DES&GL:4&8") !
                    // So what the hell does that do.
                    // Essentially... a whole 312 lines of UniBasic just to derive a unique key, potentially padded, from a given control file
                    // potentially in given subsystem. In this case the control file is GLCONTROL and the AccNo is 4, whatever that means.
                    // How do we best mimic this behaviour?
                    // 
                    var allKeysCommand = session.CreateUniCommand();

                    allKeysCommand.Command = "LIST GLPPTRANS.DES BY-DSND @ID COL.HDR.SUPP";

                    allKeysCommand.Execute();

                    var response = allKeysCommand.Response.Trim();

                    var mostRecentKey = Convert.ToInt32(response.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).First());

                    var nextKey = mostRecentKey + 1;

                    var thisKey = $"{nextKey}".PadLeft(8, '0');

                    keys.Add(thisKey);

                    var dataSet = new UniDataSet(session);

                    dataSet.Insert(thisKey);

                    //slcustFile.LockFile();
                    glppTransDesFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();

                }
                else
                {
                    /// ....?????
                }
            }

            return keys;
        }
    }
}
