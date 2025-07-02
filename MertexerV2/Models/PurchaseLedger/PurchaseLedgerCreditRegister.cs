using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.Utilities;
using Newtonsoft.Json;

namespace MertexerV2.Models.PurchaseLedger
{
    [UniVerseFileName("PLCRNREG")]
    public class PurchaseLedgerCreditRegister : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public PurchaseLedgerCreditRegister(string key) : base(key)
        {
            Initialise();
        }

        public PurchaseLedgerCreditRegister() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(PurchaseLedgerCreditRegister).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        [JsonAlsoUpdates("DocumentDate")]
        public DateTime? DocumentDT { get; set; }

        /// <summary>
        /// Document Date
        /// </summary>
        [UniVerseIndex(33)]
        [UniVerseFieldName("SS.DOCDATE.INT")]
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

        [JsonAlsoUpdates("OnFileDate")]
        public DateTime? OnFileDT { get; set; }

        /// <summary>
        /// Document Date
        /// </summary>
        [UniVerseIndex(36)]
        [UniVerseFieldName("SS.DATEONFILE.INT")]
        public string OnFileDate
        {
            get
            {
                if (OnFileDT.HasValue)
                {
                    return Framework.UniVerseDateString(OnFileDT.Value);
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
                        OnFileDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        OnFileDT = null;
                    }
                }
            }
        }

        [UniVerseIndex(37)]
        [UniVerseFieldName("ITEMREF")]
        public string ItemReference { get; set; }

        [JsonAlsoUpdates("DueDate")]
        public DateTime? DueDT { get; set; }

        /// <summary>
        /// Due Date
        /// </summary>
        [UniVerseIndex(38)]
        [UniVerseFieldName("SS.DUEDATE.INT")]
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

        [UniVerseIndex(39)]
        [UniVerseFieldName("PSUPREF")]
        public string SupplierReference { get; set; }

        [UniVerseIndex(43)]
        [UniVerseFieldName("PAMOUNT")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> AmountsExVat { get; set; }

        [UniVerseIndex(44)]
        [UniVerseFieldName("RFGLCODERG")]
        [UniVerseMultiValue]
        public IEnumerable<string> GeneralLedgerCodes { get; set; }

        [UniVerseIndex(47)]
        [UniVerseFieldName("VATCODE")]
        [UniVerseMultiValue]
        public IEnumerable<string> VATCodes { get; set; }

        [UniVerseIndex(48)]
        [UniVerseFieldName("VATTAX")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> VATTaxes { get; set; }

        [UniVerseIndex(49)]
        [UniVerseFieldName("VATVALUE")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> VATValues { get; set; }

        [UniVerseIndex(50)]
        [UniVerseFieldName("STGAMT")]
        [UniVerseConversion("MR22")]
        public decimal AmountSterling { get; set; }

        [UniVerseIndex(54)]
        [UniVerseFieldName("TERMS")]
        public string PaymentTermsCode { get; set; }

        [UniVerseIndex(56)]
        [UniVerseFieldName("MEMO")]
        public string Memo { get; set; }

        /// <summary>
        /// Default: S(<48>)+S(<49>)[S][M]
        /// </summary>
        [UniVerseIndex(60)]
        [UniVerseFieldName("VATBAL")]
        [UniVerseConversion("MR22")]
        public decimal VATBalance { get; set; }

        /// <summary>
        /// Default: S(<43>)+S(<48>)[S][M]
        /// </summary>
        [UniVerseIndex(61)]
        [UniVerseFieldName("DOCBAL")]
        [UniVerseConversion("MR22")]
        public decimal DocumentBalance { get; set; }

        [UniVerseIndex(82)]
        [UniVerseFieldName("MEMO1")]
        [UniVerseMultiValue]
        public IEnumerable<string> OtherMemos { get; set; }

        [UniVerseIndex(87)]
        [UniVerseFieldName("DISPUTE")]
        [UniVerseBooleanTrueWhen("D")]
        public bool Disputed { get; set; }

        [UniVerseIndex(115)]
        [UniVerseFieldName("PTO")]
        public string PassedTo { get; set; }

        [JsonAlsoUpdates("ReceivedDate")]
        public DateTime? ReceivedDT { get; set; }

        /// <summary>
        /// Document Date
        /// </summary>
        [UniVerseIndex(114)]
        [UniVerseFieldName("SS.RDATE.INT")]
        public string ReceivedDate
        {
            get
            {
                if (ReceivedDT.HasValue)
                {
                    return Framework.UniVerseDateString(ReceivedDT.Value);
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
                        ReceivedDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        ReceivedDT = null;
                    }
                }
            }
        }

        [JsonAlsoUpdates("PassedToDate")]
        public DateTime? PassedToDT { get; set; }

        [UniVerseIndex(116)]
        [UniVerseFieldName("SS.PDATE.INT")]
        public string PassedToDate
        {
            get
            {
                if (PassedToDT.HasValue)
                {
                    return Framework.UniVerseDateString(PassedToDT.Value);
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
                        PassedToDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        PassedToDT = null;
                    }
                }
            }
        }

        [UniVerseIndex(161)]
        [UniVerseFieldName("CURRENCY")]
        public string Currency { get; set; }

        /// <summary>
        /// Default: P("PG.PLVATTAX,D.47.49..160")[M]
        /// Validation: C:PG.PLVATTAX,V.47.49..160
        /// </summary>
        [UniVerseIndex(162)]
        [UniVerseFieldName("EXRATE")]
        [UniVerseConversion("MR66")]
        public decimal ExchangeRate { get; set; }

        [UniVerseIndex(250)]
        [UniVerseFieldName("CURAMT")]
        [UniVerseConversion("MR22")]
        public decimal AmountCurrency { get; set; }
    }
}
