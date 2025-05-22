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
    public class PurchaseLedgerTransfer : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public PurchaseLedgerTransfer(string key) : base(key)
        {
            Initialise();
        }

        public PurchaseLedgerTransfer() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(PurchaseLedgerTransfer).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
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

        [UniVerseIndex(6)]
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

        [UniVerseIndex(12)]
        [UniVerseFieldName("SUPREF")]
        public string SupplierReference { get; set; }

        [UniVerseIndex(14)]
        [UniVerseFieldName("ORIGVAL")]
        [UniVerseConversion("MR22")]
        public decimal OriginalValueABS { get; set; }

        [UniVerseIndex(16)]
        [UniVerseFieldName("DISPUTE")]
        public string Disputed { get; set; }

        [UniVerseIndex(22)]
        [UniVerseFieldName("UNALLOCATED")]
        [UniVerseConversion("MR22")]
        public decimal Unallocated { get; set; }

        [UniVerseIndex(114)]
        [UniVerseFieldName("DIVISION")]
        public string Division { get; set; }
    }


    [UniVerseFileName("TPLTRANS.CUR")]
    public class PurchaseLedgerTransferCurrency : PurchaseLedgerTransfer
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
