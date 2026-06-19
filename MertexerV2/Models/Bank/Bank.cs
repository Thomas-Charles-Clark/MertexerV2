using MertexerV2.InvoiceFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.Models.Bank
{
    [UniVerseFileName("BANK")]
    public class Bank : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public Bank(string key) : base(key)
        {
            Initialise();
        }

        public Bank() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(Bank).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            BankFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("TITLE")]
        public string Name { get; set; }

        [UniVerseIndex(2)]
        [UniVerseFieldName("CHQ.PRINTED")]
        public string ChequesPrinted { get; set; }

		[UniVerseIndex(3)]
		[UniVerseFieldName("CHEQUES")]
		public string ChequesAvailable { get; set; }

        [UniVerseIndex(4)]
        [UniVerseFieldName("GLEDGER")]
        public string GeneralLedgerAccountNumbers_Bank { get; set; }

        [UniVerseIndex(5)]
        [UniVerseFieldName("AUTO.PRINTED")]
        public string NatWestAutoPrinted { get; set; }

        [UniVerseIndex(6)]
        [UniVerseFieldName("ACCOUNT2")]
        public string BankAccountNumber { get; set; }

        [UniVerseIndex(7)]
        [UniVerseFieldName("AUTOPAY")]
        public string AutoPay { get; set; }

        [UniVerseIndex(8)]
        [UniVerseFieldName("GLCHQ.PRTD")]
        public string GeneralLedgerChequePrinted { get; set; }

		[UniVerseIndex(9)]
		[UniVerseFieldName("GLCHQ.AVAIL")]
		public string GeneralLedgerChequesAvailable { get; set; }

        [UniVerseIndex(10)]
        [UniVerseFieldName("CURRENCY")]
        public string Currency { get; set; }

        [UniVerseIndex(11)]
        [UniVerseFieldName("CONVERSION.CHARGE")]
        public decimal? ConversionCharge { get; set; }

        [UniVerseIndex(12)]
        [UniVerseFieldName("LAST.SHEET")]
        public int LastSheetNumber { get; set; }

        [UniVerseIndex(13)]
        [UniVerseFieldName("GLEDGER.EXCH")]
        public string GeneralLedgerAccountNumbers_ExchangeDifferences { get; set; }

		//[UniVerseIndex(14)]
		//public string UnknownFieldPosition14 { get; set; }

		[UniVerseIndex(15)]
		[UniVerseFieldName("LAST.AUDIT")]
		public string LastAudit { get; set; }

		[UniVerseIndex(16)]
		[UniVerseFieldName("PAYRUN")]
		public string PaymentRunInProgress { get; set; }

		[UniVerseIndex(17)]
		[UniVerseFieldName("SORTCODE")]
		public string SortCode { get; set; }

		[UniVerseIndex(18,1)]
		[UniVerseFieldName("BANKADD1")]
		public string BankAddressLine1 { get; set; }

		[UniVerseIndex(18, 2)]
		[UniVerseFieldName("BANKADD2")]
		public string BankAddressLine2 { get; set; }

		//[UniVerseIndex(19)]
		//public string UnknownFieldPosition14 { get; set; }

		[UniVerseIndex(20)]
        [UniVerseFieldName("FREE.LONG")]
        [UniVerseMultiValue]
        public IEnumerable<string> FreeFormatText { get; set; }

		[UniVerseIndex(21)]
		[UniVerseFieldName("INTERCO.CONTROL")]
		public string GeneralLedgerAccountNumbers_InterCompanyControl { get; set; }

        [UniVerseIndex(102)]
        [UniVerseFieldName("SS.DELETED.DATE.INT.DES")]
        public DateTime? DeletedDT { get; set; }
	}
}
