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
    [UniVerseFileName("HISTTSLTRANS.CUR")]
    public class SalesLedgerTransactionHistoryCurrency : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public SalesLedgerTransactionHistoryCurrency(string key) : base(key)
        {
            Initialise();
        }

        public SalesLedgerTransactionHistoryCurrency() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(SalesLedgerTransactionHistoryCurrency).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

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

        [UniVerseIndex(12)]
        [UniVerseFieldName("CUSTREF")]
        public string TradeAccountCode { get; set; }

        [UniVerseIndex(14)]
        [UniVerseFieldName("ORIGVAL")]
        [UniVerseConversion("MR22")]
        public decimal OriginalValue { get; set; }

        [UniVerseIndex(18)]
        [UniVerseFieldName("ALLOCATE")]
        [UniVerseConversion("MR22")]
        public decimal Unallocated { get; set; }

        [UniVerseIndex(21)]
        [UniVerseFieldName("CS.AMT")]
        [UniVerseConversion("MR22")]
        public decimal CSAmount { get; set; }

		[UniVerseIndex(22)]
		[UniVerseFieldName("CS.ACTION")]
		public string CSAction { get; set; }

		[UniVerseIndex(23)]
		[UniVerseFieldName("DOCLINKS")]
		[UniVerseMultiValue]
		public IEnumerable<string> AllocatedFromDocuments { get; set; }

		[UniVerseIndex(24)]
		[UniVerseFieldName("XDOCLINKS")]
		[UniVerseMultiValue]
		public IEnumerable<string> AllocatedToDocuments { get; set; }

		[UniVerseIndex(30)]
        [UniVerseFieldName("CURR.CODE")]
        public string CurrencyCode { get; set; }

        [UniVerseIndex(31)]
        [UniVerseFieldName("XRATE")]
        [UniVerseConversion("MR66")]
        public decimal ExchangeRate { get; set; }

        [UniVerseIndex(36)]
        [UniVerseFieldName("RND.ERR.BCR")]
        [UniVerseConversion("MR22")]
        public decimal RoundingErrors { get; set; }

        [UniVerseIndex(51)]
        [UniVerseFieldName("SETDISC")]
        [UniVerseConversion("MR22")]
        public decimal SettlementDiscount { get; set; }

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
