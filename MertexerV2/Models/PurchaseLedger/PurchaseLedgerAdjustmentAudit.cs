using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.Utilities;
using Newtonsoft.Json;

namespace MertexerV2.Models.PurchaseLedger
{
    [UniVerseFileName("PLADJAUDIT")]
    public class PurchaseLedgerAdjustmentAudit : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public PurchaseLedgerAdjustmentAudit(string key) : base(key)
        {
            Initialise();
        }

        public PurchaseLedgerAdjustmentAudit() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(PurchaseLedgerAdjustmentAudit).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("DOCTOT")]
        [UniVerseConversion("MR22")]
        public decimal DocumentTotal { get; set; }

        [UniVerseIndex(2)]
        [UniVerseFieldName("TYPE")]
        public string TransactionType { get; set; }

        [UniVerseIndex(3)]
        [UniVerseFieldName("PORT")]
        public IEnumerable<int> Ports { get; set; }

        [JsonAlsoUpdates("DocumentDate")]
        public DateTime? DocumentDT { get; set; }

        [UniVerseIndex(4)]
        [UniVerseFieldName("SS.DOCDAT.INT")]
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

        [JsonAlsoUpdates("PostDate")]
        public DateTime? PostDT { get; set; }

        [UniVerseIndex(5)]
        [UniVerseFieldName("SS.POSTDAT.INT")]
        public string PostDate
        {
            get
            {
                if (PostDT.HasValue)
                {
                    return Framework.UniVerseDateString(PostDT.Value);
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
                        PostDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        PostDT = null;
                    }
                }
            }
        }

        [UniVerseIndex(6)]
        [UniVerseFieldName("VATDIS")]
        [UniVerseConversion("MR22")]
        public decimal VATDiscount { get; set; }

        [UniVerseIndex(7)]
        [UniVerseFieldName("SUPREF")]
        public string SupplierReference { get; set; }

        [UniVerseIndex(8)]
        [UniVerseFieldName("UNALLOCATED")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> UnallocatedAmounts { get; set; }

        [UniVerseIndex(9)]
        [UniVerseFieldName("BALANCE")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> Balances { get; set; }

        [UniVerseIndex(10)]
        [UniVerseFieldName("GLCODE")]
        [UniVerseMultiValue]
        public IEnumerable<string> GeneralLedgerCodes { get; set; }

        [UniVerseIndex(12)]
        [UniVerseFieldName("DISCOUNTS")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> Discounts { get; set; }

        [UniVerseIndex(13)]
        [UniVerseFieldName("GLAMOUNT")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> GeneralLedgerAmounts { get; set; }

        [UniVerseIndex(14)]
        [UniVerseFieldName("TIME")]
        public int Time { get; set; }

        [UniVerseIndex(20)]
        [UniVerseFieldName("DISPUTE")]
        public string Dispute { get; set; }

        [JsonAlsoUpdates("DueDate")]
        public DateTime? DueDT { get; set; }

        [UniVerseIndex(21)]
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

        /// <summary>
        /// Default Value: "STG"
        /// </summary>
        [UniVerseIndex(30)]
        [UniVerseFieldName("CURR")]
        public string Currency { get; set; }

        [UniVerseIndex(31)]
        [UniVerseFieldName("DOCTOT.CUR")]
        [UniVerseConversion("MR22")]
        public decimal DocumentTotalCurrency { get; set; }

        [UniVerseIndex(32)]
        [UniVerseFieldName("VATDIS.CUR")]
        [UniVerseConversion("MR22")]
        public decimal VATDiscountCurrency { get; set; }

        [UniVerseIndex(34)]
        [UniVerseFieldName("XRATE")]
        [UniVerseConversion("MR66")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> ExchangeRates { get; set; } = new List<decimal>();

        public decimal ExchangeRate
        {
            get
            {
                if (!ExchangeRates.Any())
                {
                    return 1.0m;
                }
                else
                {
                    return ExchangeRates.First();
                }
            }
        }
    }
}
