using MertexerV2.Utilities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.PurchaseLedger
{
    [UniVerseFileName("TPLTRANS")]
    public class PurchaseLedgerTransaction : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public PurchaseLedgerTransaction(string key) : base(key)
        {
            Initialise();
        }

        public PurchaseLedgerTransaction() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(PurchaseLedgerTransaction).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public DateTime? DocumentDT { get; set; }

        /// <summary>
        /// Document Date
        /// </summary>
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

        public DateTime? DueDT { get; set; }

        /// <summary>
        /// Due Date
        /// </summary>
        [UniVerseIndex(2)]
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
        /// Sales Representative Code
        /// </summary>
        [UniVerseIndex(3)]
        [UniVerseFieldName("TYPE")]
        public string TransactionType { get; set; }

        /// <summary>
        /// Sales Agent Code
        /// </summary>
        [UniVerseIndex(4)]
        [UniVerseFieldName("ITEMREF")]
        public string ItemReference { get; set; }

        // This is NOT mapped to a concrete field;
        // it is built as part of a Mertex internal report
        // and so I have transposed it for use in a report I re-built for Sarah
        public decimal OriginalValue
        {
            get
            {
                if (Credit + Debit > 0)
                {
                    return OriginalValueABS;
                }
                else
                {
                    return OriginalValueABS * -1;
                }
            }
        }

        [UniVerseIndex(5)]
        [UniVerseFieldName("DOCNO")]
        public string DocumentNumber { get; set; }

        [UniVerseIndex(6)]
        [UniVerseFieldName("VATDIS")]
        [UniVerseConversion("MR22")] 
        public decimal VATDiscount { get; set; }

        [UniVerseIndex(7)]
        [UniVerseFieldName("DEBIT")]
        [UniVerseConversion("MR22")]
        public decimal Debit { get; set; }

        [UniVerseIndex(8)]
        [UniVerseFieldName("CREDIT")]
        [UniVerseConversion("MR22")]
        public decimal Credit { get; set; }

        [UniVerseIndex(9)]
        [UniVerseFieldName("BALANCE")]
        [UniVerseConversion("MR22")]
        public decimal Balance { get; set; }

        [UniVerseIndex(10)]
        [UniVerseFieldName("PERIOD")]
        public byte Period { get; set; }

        [UniVerseIndex(11)]
        [UniVerseFieldName("STATUS")]
        [UniVerseMultiValue]
        public IEnumerable<string> Statuses { get; set; }

        [UniVerseIndex(11,1)]
        [UniVerseFieldName("STATUS")]
        public string Status { get; set; }

        [UniVerseIndex(12)]
        [UniVerseFieldName("SUPREF")]
        public string SupplierReference { get; set; }

        [UniVerseIndex(13)]
        [UniVerseFieldName("AGE")]
        public int Age { get; set; }

        [UniVerseIndex(14)]
        [UniVerseFieldName("ORIGVAL")]
        [UniVerseConversion("MR22")]
        public decimal OriginalValueABS { get; set; }

        [UniVerseIndex(15)]
        [UniVerseFieldName("DISPMEMO")]
        public string DisputeMemo { get; set; }

        [UniVerseIndex(16)]
        [UniVerseFieldName("DISPUTE")]
        public string Dispute { get; set; }

        [UniVerseIndex(17)]
        [UniVerseFieldName("ADJUST")]
        public string Adjust { get; set; }

        [UniVerseIndex(18)]
        [UniVerseFieldName("DISC")]
        [UniVerseConversion("MR33")]
        public decimal DiscountPercentage { get; set; }

        [UniVerseIndex(19)]
        [UniVerseFieldName("SELCHQ")]
        public string SelectedForCheque { get; set; }

        [UniVerseIndex(20)]
        [UniVerseFieldName("DISCAMT")]
        [UniVerseConversion("MR22")]
        public decimal DiscountAmount { get; set; }

        [UniVerseIndex(21)]
        [UniVerseFieldName("NEXTAUTO")]
        [UniVerseConversion("MR22")]
        public decimal Payment { get; set; } // "next auto" ???
        
        [UniVerseIndex(22)]
        [UniVerseFieldName("UNALLOCATED")]
        [UniVerseConversion("MR22")]
        public decimal Unallocated { get; set; }

        [UniVerseIndex(23)]
        [UniVerseFieldName("DISCTAKEN")]
        [UniVerseConversion("MR22")]
        public decimal SDTaken { get; set; }

        [UniVerseIndex(24)]
        [UniVerseFieldName("DISCT")]
        [UniVerseConversion("MR22")]
        public decimal DiscountTaken { get; set; }

        [JsonAlsoUpdates("PostDate")]
        public DateTime? PostDT { get; set; }

        [UniVerseIndex(25)]
        [UniVerseFieldName("SS.POSTDATE.INT")]
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

        [UniVerseIndex(26)]
        [UniVerseFieldName("MEMO")]
        public string Memo { get; set; }

        [JsonAlsoUpdates("DiscountCutOffDate")]
        public DateTime? DiscountCutOffDT { get; set; }

        [UniVerseIndex(27)]
        [UniVerseFieldName("SS.DISCDATE.INT")]
        public string DiscountCutOffDate
        {
            get
            {
                if (DiscountCutOffDT.HasValue)
                {
                    return Framework.UniVerseDateString(DiscountCutOffDT.Value);
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
                        DiscountCutOffDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        DiscountCutOffDT = null;
                    }
                }
            }
        }

        [UniVerseIndex(29)]
        [UniVerseFieldName("CURRENCY")]
        public string CurrencyCode { get; set; }

        [UniVerseIndex(30)]
        [UniVerseFieldName("XRATE")]
        [UniVerseMultiValue]
        [UniVerseConversion("MR66")]

        public IEnumerable<decimal> ExchangeRates { get; set; }

        [UniVerseIndex(30,1)]
        [UniVerseFieldName("XRATE.1")]
        [UniVerseConversion("MR66")]
        public decimal MostRecentExchangeRate { get; set; }

        [UniVerseIndex(32)]
        [UniVerseFieldName("EXDIFFS")]
        [UniVerseConversion("MR22")]

        public decimal ExchangeRateDifferences { get; set; }

        [UniVerseIndex(33)]
        [UniVerseFieldName("BANKCHG")]
        [UniVerseConversion("MR22")]

        public decimal BankCharges { get; set; }

        [JsonAlsoUpdates("DatePaid")]
        public DateTime? PaidDT { get; set; }

        [UniVerseIndex(35)]
        [UniVerseFieldName("SS.DATE.PAID.INT")]
        public string DatePaid
        {
            get
            {
                if (PaidDT.HasValue)
                {
                    return Framework.UniVerseDateString(PaidDT.Value);
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
                        PaidDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        PaidDT = null;
                    }
                }
            }
        }

        [UniVerseIndex(36)]
        [UniVerseFieldName("RND.ERR.BCR")]
        [UniVerseConversion("MR22")]

        public decimal RoundingErrors { get; set; }

        // Fields 40 through 41 are MV and are PRINT related - they're used in generating Mertex Reports.
        // Not mapping them.

        [UniVerseIndex(50)]
        [UniVerseFieldName("VSN")]
        public int Version { get; set; }

        [UniVerseIndex(51)]
        [UniVerseFieldName("NP.KEEP.FLAG")]
        public string NextKeepFlag { get; set; }

        [UniVerseIndex(52)]
        [UniVerseFieldName("PONO")]
        [UniVerseMultiValue]
        public IEnumerable<string> PurchaseOrderNumbers { get; set; }

        [UniVerseIndex(114)]
        [UniVerseFieldName("DIVISION")]
        public string Division { get; set; }

        [UniVerseIndex(115)]
        [UniVerseFieldName("SCALLNOS")]
        [UniVerseMultiValue]
        public IEnumerable<string> SupplierCallNoteReferences { get; set; }

        [UniVerseIndex(120)]
        [UniVerseFieldName("REGNO")]
        public string PurchaseLedgerRegisterNumber { get; set; }

        [UniVerseIndex(123)]
        [UniVerseFieldName("REFENQ.W9")]
        [UniVerseConversion("MR22")]
        public decimal OutstandingAmount { get; set; }
    }

    [UniVerseFileName("TPLTRANS.CUR")]
    public class PurchaseLedgerTransferCurrency : PurchaseLedgerTransaction
    {
        [JsonConstructor]
        public PurchaseLedgerTransferCurrency(string key) : base(key)
        {
            Initialise();
        }

        public PurchaseLedgerTransferCurrency() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(PurchaseLedgerTransferCurrency).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }
    }
}
