using MertexerV2.Models.Invoices;
using MertexerV2.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.SalesLedger
{
    [UniVerseFileName("TSLTRANS")]
    public class SalesLedgerTransaction : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public SalesLedgerTransaction(string key) : base(key)
        {
            Initialise();
        }

        public SalesLedgerTransaction() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(SalesLedgerTransaction).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
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
        public decimal OriginalValueABS
        {
            get
            {
                if (Credit + Debit > 0)
                {
                    return OriginalValue;
                }
                else
                {
                    return OriginalValue * -1;
                }
            }
        }

        [UniVerseIndex(5)]
        [UniVerseFieldName("DOCNO")]
        public string DocumentReference { get; set; }

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
        public string Status { get; set; }

        [UniVerseIndex(12)]
        [UniVerseFieldName("CUSTREF")]
        public string TradeAccountCode { get; set; }

        [UniVerseIndex(13)]
        [UniVerseFieldName("AGE")]
        public int Age { get; set; }

        [UniVerseIndex(14)]
        [UniVerseFieldName("ORIGVAL")]
        [UniVerseConversion("MR22")]
        public decimal OriginalValue { get; set; }

        [UniVerseIndex(15)]
        [UniVerseFieldName("DISPMEMO")]
        public string Reason { get; set; }

        [UniVerseIndex(16)]
        [UniVerseFieldName("DISPUTE")]
        public string Dispute { get; set; }

        [UniVerseIndex(17)]
        [UniVerseFieldName("ADJUST")]
        public string Adjust { get; set; }

        [UniVerseIndex(18)]
        [UniVerseFieldName("ALLOCATE")]
        [UniVerseConversion("MR22")]
        public decimal Unallocated { get; set; }

        [JsonAlsoUpdates("PostDate")]
        public DateTime? PostDT { get; set; }

        [UniVerseIndex(19)]
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

        [JsonAlsoUpdates("CSDate")]
        public DateTime? CSDT { get; set; }

        [UniVerseIndex(20)]
        [UniVerseFieldName("SS.CSDATE.INT")]
        public string CSDate
        {
            get
            {
                if (CSDT.HasValue)
                {
                    return Framework.UniVerseDateString(CSDT.Value);
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
                        CSDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        CSDT = null;
                    }
                }
            }
        }

        [UniVerseIndex(21)]
        [UniVerseFieldName("CS.AMT")]
        [UniVerseConversion("MR22")]
        public decimal CSAmount { get; set; }

        [UniVerseIndex(22)]
        [UniVerseFieldName("CS.ACTION")]
        public string CSAction { get; set; }

        [UniVerseIndex(23)]
        [UniVerseFieldName("DOCLINKS")]
        public IEnumerable<string> AllocatedFromDocuments { get; set; }

        [UniVerseIndex(24)]
        [UniVerseFieldName("XDOCLINKS")]
        public IEnumerable<string> AllocatedToDocuments { get; set; }

        [UniVerseIndex(25)]
        [UniVerseFieldName("INV.ACCOUNT")]
        public string InvoiceAccount { get; set; }

        [UniVerseIndex(26)]
        [UniVerseFieldName("CORD")] 
        public string CustomerOrderNumber { get; set; }

        [UniVerseIndex(32)]
        [UniVerseFieldName("EXDIFFS")]
        [UniVerseConversion("MR22")]
        public decimal ExchangeRateDifferences { get; set; }

        [UniVerseIndex(33)]
        [UniVerseFieldName("BANKCHG")]
        [UniVerseConversion("MR22")]
        public decimal BankCharges { get; set; }

        [UniVerseIndex(34)]
        [UniVerseFieldName("SETTLEDIS.PC")]
        [UniVerseConversion("MR33")]
        public decimal DiscountPercentage { get; set; }


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

        [UniVerseIndex(42)]
        [UniVerseFieldName("SOURCE.DOC")]
        public string SourceDocument { get; set; }

        [UniVerseIndex(50)]
        [UniVerseFieldName("VSN")]
        public int Version { get; set; }

        [UniVerseIndex(51)]
        [UniVerseFieldName("SETDISC")]
        [UniVerseConversion("MR22")]
        public decimal SettlementDiscount { get; set; }

        [JsonAlsoUpdates("SettlementDate")]
        public DateTime? SettlementDT { get; set; }

        [UniVerseIndex(52)]
        [UniVerseFieldName("SS.SETDATE.INT")]
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

        [UniVerseIndex(53)]
        [UniVerseFieldName("DISCTAKEN")]
        [UniVerseConversion("MR22")]
        public decimal SettlementDiscountTaken { get; set; }

        [UniVerseIndex(113)]
        [UniVerseFieldName("CREDIT.CLAIM.NO")]
        public string CreditClaimNumber { get; set; }

        [UniVerseIndex(114)]
        [UniVerseFieldName("SELLCO")]
        public string SellingCompany { get; set; }

        [UniVerseIndex(116)]
        [UniVerseFieldName("POSTTIME")]
        public int PostTime { get; set; }

        #region Linked Entities

        [UniVerseForeignKey("Key", "MertexerV2.Models.Invoices.InvoiceHead")]
        public virtual InvoiceHead InvoiceHead { get; set; }

        public string Terms
        {
            get
            {
                return InvoiceHead?.Terms;
            }
        }

        #endregion

        #region Derived Fields 

        public decimal OustandingAmount
        {
            get
            {
                return Balance - Unallocated;
            }
        }

        #endregion
    }
}
