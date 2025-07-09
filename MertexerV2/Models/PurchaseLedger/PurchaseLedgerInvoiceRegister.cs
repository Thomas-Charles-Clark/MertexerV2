using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.Utilities;
using Newtonsoft.Json;

namespace MertexerV2.Models.PurchaseLedger
{
    [UniVerseFileName("PLINVREG")]
    public class PurchaseLedgerInvoiceRegister : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public PurchaseLedgerInvoiceRegister(string key) : base(key)
        {
            Initialise();
        }

        public PurchaseLedgerInvoiceRegister() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(PurchaseLedgerInvoiceRegister).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        [JsonAlsoUpdates("DocumentDate")]
        public DateTime? DocumentDT { get; set; }

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

        /// <summary>
        /// [no default]
        /// </summary>
        [UniVerseIndex(43)]
        [UniVerseFieldName("PAMOUNT")]
        [UniVerseMultiValue]
        [UniVerseConversion("MR22")]
        public IEnumerable<decimal> AmountsExVAT { get; set; }

        /// <summary>
        /// Default: B("PL.B.DEFGLCODE")
        /// Validation: F:GLEDGER AND B:PLUFO01,3{Must Exist on 'GLEDGER'}
        /// </summary>
        [UniVerseIndex(44)]
        [UniVerseFieldName("RFGLCODERG")]
        [UniVerseMultiValue]
        public IEnumerable<string> GeneralLedgerCodes { get; set; }

        /// <summary>
        /// Default: P("PG.DPLVATCODE,39")
        /// Validation: f:CFVAT,,Y AND B:ZKMULTI,47{}
        /// </summary>
        [UniVerseIndex(47)]
        [UniVerseFieldName("VATCODE")]
        [UniVerseMultiValue]
        public IEnumerable<string> VATCodes { get; set; }

        /// <summary>
        /// Default: P("PG.PLVATTAX,D.47.49..160")[M]
        /// Validation: C:PG.PLVATTAX,V.47.49..160
        /// </summary>
        [UniVerseIndex(48)]
        [UniVerseFieldName("VATTAX")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> VATTaxes { get; set; }

        /// <summary>
        /// [no default]
        /// [no validation]
        /// </summary>
        [UniVerseIndex(49)]
        [UniVerseFieldName("VATVALUE")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> VATValues { get; set; }

        /// <summary>
        /// Default: IF(FOREIGN # "Y",STGAMT[S],STGAMT)[M]
        /// Validation: M:
        /// </summary>
        [UniVerseIndex(50)]
        [UniVerseFieldName("STGAMT")]
        [UniVerseConversion("MR22")]
        public decimal AmountSterling { get; set; }

        /// <summary>
        /// Default: F("PLSUP",<39>)<20>
        /// Validation: M: AND F:PLTERMS,4,,41{Must Exist on 'PLTERMS'}
        /// </summary>
        [UniVerseIndex(54)]
        [UniVerseFieldName("TERMS")]
        public string PaymentTermsCode { get; set; }

        [JsonAlsoUpdates("SettlementDate")]
        public DateTime? SettlementDT { get; set; }

        [UniVerseIndex(55)]
        [UniVerseFieldName("SS.SETTLEMENTDATE.INT")]
        public string SettlementDate
        {
            get
            {
                if (SettlementDT.HasValue)
                {
                    return Framework.UniVerseDateString(SettlementDT.Value);
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
                        SettlementDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        SettlementDT = null;
                    }
                }
            }
        }

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

        [UniVerseIndex(80)]
        [UniVerseFieldName("DISCAMT")]
        [UniVerseConversion("MR22")]
        public decimal? DiscountAmount { get; set; }

        [UniVerseIndex(82)]
        [UniVerseFieldName("MEMO1")]
        [UniVerseMultiValue]
        public IEnumerable<string> OtherMemos { get; set; }

        [UniVerseIndex(87)]
        [UniVerseFieldName("DISPUTE")]
        public string Disputed { get; set; }

        [UniVerseIndex(88)]
        [UniVerseFieldName("DISPMEMO")]
        public string DisputeMemo { get; set; }

        [JsonAlsoUpdates("ReceivedDate")]
        public DateTime? ReceivedDT { get; set; }

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

        [UniVerseIndex(115)]
        [UniVerseFieldName("PTO")]
        public string PassedTo { get; set; }

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

        [UniVerseIndex(160)]
        [UniVerseFieldName("DISCPERC")]
        [UniVerseConversion("MR33")]
        public decimal? DiscountPercentage { get; set; }

        [UniVerseIndex(161)]
        [UniVerseFieldName("CURRENCY")]
        public string MertexCurrencyCode { get; set; }

        /// <summary>
        /// Default: P("PG.PLVATTAX,D.47.49..160")[M]
        /// Validation: C:PG.PLVATTAX,V.47.49..160
        /// </summary>
        [UniVerseIndex(162,1)]
        [UniVerseFieldName("XRATE.1")]
        [UniVerseConversion("MR66")]
        public decimal ExchangeRate { get; set; }

        /// <summary>
        /// Validation: M: AND P:VAL.CURRAMT,161
        /// </summary>
        [UniVerseIndex(250)]
        [UniVerseFieldName("CURAMT")]
        [UniVerseConversion("MR22")]
        public decimal AmountCurrency { get; set; }
    }
}
