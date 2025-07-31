using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.Utilities;

namespace MertexerV2.Models.GeneralLedger
{
    [UniVerseFileName("GLPPTRANS.DES")]
    public class GeneralLedgerPrePaymentTransaction : UniVerseSerializationBase<string>
    {
        public GeneralLedgerPrePaymentTransaction(string key) : base(key)
        {

        }

        public GeneralLedgerPrePaymentTransaction() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(GeneralLedgerPrePaymentTransaction).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("PREPAY.SUPREF")]
        public string SupplierCode { get; set; }

        [UniVerseIndex(2)]
        [UniVerseFieldName("PREPAY.GLCODE")]
        public string GeneralLedgerCode { get; set; }

        [UniVerseIndex(3)]
        [UniVerseFieldName("PREPAY.AMOUNT")]
        [UniVerseConversion("MR22")]
        public decimal Amount { get; set; }

        [UniVerseIndex(4)]
        [UniVerseFieldName("PREPAY.MEMO")]
        public string Memo { get; set; }

        [UniVerseIndex(5)]
        [UniVerseFieldName("PREPAY.TRANSNO")]
        public string TransactionNumber { get; set; }

        [JsonAlsoUpdates("DocumentDate")]
        public DateTime? DocumentDT { get; set; }

        [UniVerseIndex(6)]
        [UniVerseFieldName("SS.PREPAY.DOCDATE.INT")]
        public string DocumentDate
        {
            get
            {
                if (DocumentDT.HasValue)
                {
                    return Framework.UniVerseDateString(DocumentDT.Value);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (Int32.TryParse(value, out int valInt))
                    {
                        DocumentDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        DocumentDT = null;
                    }
                }
            }
        }

        [JsonAlsoUpdates("DueDate")]
        public DateTime? DueDT { get; set; }

        [UniVerseIndex(7)]
        [UniVerseFieldName("SS.PREPAY.DUEDATE.INT")]
        public string DueDate
        {
            get
            {
                if (DueDT.HasValue)
                {
                    return Framework.UniVerseDateString(DueDT.Value);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (Int32.TryParse(value, out int valInt))
                    {
                        DueDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        DueDT = null;
                    }
                }
            }
        }

        [UniVerseIndex(8)]
        [UniVerseFieldName("PREPAY.DOCTYPE")]
        public string TransactionType { get; set; }

        [UniVerseIndex(20)]
        [UniVerseFieldName("APPORTIONED")]
        public bool Apportioned { get; set; }

        [UniVerseIndex(21)]
        [UniVerseFieldName("START.PD")]
        public byte StartPeriod { get; set; }

        [UniVerseIndex(22)]
        [UniVerseFieldName("NO.PDS")]
        public byte NumberOfPeriods { get; set; }

        [UniVerseIndex(23)]
        [UniVerseFieldName("SPLIT.EQUAL")]
        public bool SplitEqually { get; set; }

        [UniVerseIndex(24)]
        [UniVerseFieldName("SPLIT.AMOUNT")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> SplitAmounts { get; set; }

        [UniVerseIndex(25)]
        [UniVerseFieldName("SPLIT.MEMO")]
        [UniVerseMultiValue]
        public IEnumerable<string> SplitMemos { get; set; }

        [JsonAlsoUpdates("SplitDocmentDates")]
        public IEnumerable<DateTime?> SplitDocumentDTs { get; set; }

        [UniVerseIndex(26)]
        [UniVerseFieldName("SS.SPLIT.DOCDATE.INT")]
        [UniVerseMultiValue]
        public IEnumerable<string> SplitDocumentDates
        {
            get
            {
                var result = new List<string>();

                if (SplitDocumentDTs.Any())
                {
                    foreach(var splitDocumentDt in SplitDocumentDTs)
                    {
                        if (splitDocumentDt.HasValue)
                        {
                            result.Add(Framework.UniVerseDateString(splitDocumentDt.Value));
                        }
                        else
                        {
                            result.Add(null);
                        }
                    }

                    return result;
                }
                else
                {
                    return result;
                }
            }
            set
            {
                var results = new List<DateTime?>();

                if (value.Any())
                {
                    foreach(var splitDocumentDate in value)
                    {
                        if (Int32.TryParse(splitDocumentDate, out int valInt))
                        {
                            results.Add(Framework.DateTimeFromUniVerseOffset(valInt));
                        }
                        else
                        {
                            results.Add(null);
                        }
                    }
                }

                SplitDocumentDTs = results;
            }
        }

        [UniVerseIndex(27)]
        [UniVerseFieldName("EXP.GLCODE")]
        public string ExpenseGeneralLedgerCode { get; set; }

        [UniVerseIndex(0)]
        [UniVerseFieldName("SPLIT.TOTAL")]
        public decimal SplitTotal
        {
            get 
            {
                if (SplitAmounts.Any())
                {
                    return SplitAmounts.Sum();
                }
                else
                {
                    return 0.0m;
                }
            }
        }

        // why would you have two terms for precisely the same thing...?
        // hmph
        [UniVerseIndex(0)]
        [UniVerseFieldName("POSTED")]
        public bool Posted 
        { 
            get
            {
                return Apportioned;
            }
        }

        // There's also one here named W.PPGLCODE.DESC.DES
        // which looks up the General Ledger Code description 
        // do that somewhere else, perhaps
        /*
         * 
         * [UniVerseIndex(0)]
         * [UniVerseFieldName("W.PPGLCODE.DESC.DES")]
         * public string GeneralLedgerCodeDescription { get; set; }
         * 
         * Derived Field: (F("GLEDGER",W.PPGLCODE.DES)<6>)
         * 
         * Also it's a (W)orking field, so I don't know if it really makes sense to try and pull it. 
         * Because Working Fields depend upon an extant screen context to have a value.
         * 
         */
    }
}
