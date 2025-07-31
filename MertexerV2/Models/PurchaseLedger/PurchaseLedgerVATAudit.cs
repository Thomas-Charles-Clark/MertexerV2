using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.Utilities;
using Newtonsoft.Json;

namespace MertexerV2.Models.PurchaseLedger
{
    [UniVerseFileName("PLVATAUD")]
    public class PurchaseLedgerVATAudit : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public PurchaseLedgerVATAudit(string key) : base(key)
        {
            Initialise();
        }

        public PurchaseLedgerVATAudit() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(PurchaseLedgerVATAudit).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        [JsonAlsoUpdates("DocumentDate")]
        public DateTime? DocumentDT { get; set; }

        [UniVerseIndex(1)]
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

        [UniVerseIndex(2)]
        [UniVerseFieldName("TYPE")]
        public string TransactionType { get; set; }

        [UniVerseIndex(3)]
        [UniVerseFieldName("SUPREF")]
        public string SupplierReference { get; set; }

        [UniVerseIndex(4)]
        [UniVerseFieldName("TOTDUE")]
        [UniVerseConversion("MR22")]
        public decimal PurchaseLedgerTotalDue { get; set; }

        [UniVerseIndex(5)]
        [UniVerseFieldName("CODE")]
        [UniVerseMultiValue]
        public IEnumerable<string> VATCodes { get; set; }

        [UniVerseIndex(5,3)]
        [UniVerseFieldName("CODE3")]
        [UniVerseMultiValue]
        public string ThirdVATCode { get; set; }

        [UniVerseIndex(6)]
        [UniVerseFieldName("VALUE")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> VATValues { get; set; }

        [UniVerseIndex(7)]
        [UniVerseFieldName("VATAMT")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> VATAmounts { get; set; }

        [UniVerseIndex(8)]
        [UniVerseFieldName("NEXT")]
        public byte NextPeriod { get; set; }

        [UniVerseIndex(9)]
        [UniVerseFieldName("CURRENCY.DES")]
        public string CurrencyCode { get; set; }

        [UniVerseIndex(10)]
        [UniVerseFieldName("VALUE.CUR.DES")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> VATValueCurrency { get; set; }

        [UniVerseIndex(11)]
        [UniVerseFieldName("VATAMT.CUR.DES")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> VATAmountCurrency { get; set; }

        [UniVerseIndex(12)]
        [UniVerseFieldName("TOTDUE.CUR.DES")]
        public decimal PurchaseLedgerTotalDueCurrency { get; set; }

        [JsonAlsoUpdates("CopiedDate")]
        public DateTime? CopiedDT { get; set; }

        [UniVerseIndex(13)]
        [UniVerseFieldName("SS.RETURNSTAMP.INT")]
        public string CopiedDate
        {
            get
            {
                if (CopiedDT.HasValue)
                {
                    return Framework.UniVerseDateString(CopiedDT.Value);
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
                        CopiedDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        CopiedDT = null;
                    }
                }
            }
        }

        [UniVerseIndex(14)]
        [UniVerseFieldName("PERIOD")]
        public byte Period { get; set; }
    }
}
