using MertexerV2.TradeAccountFunctions;
using MertexerV2.Utilities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models
{
    [UniVerseFileName("CUST.PART.PAYMENTS.DES")]
    public class TradeAccountPartPaymentPlan : UniVerseSerializationBase<string>
	{
		[JsonConstructor]
		public TradeAccountPartPaymentPlan(string key) : base(key)
		{
			Initialise();
		}

		public TradeAccountPartPaymentPlan() : base()
		{
			Initialise();
		}

		public override void Initialise()
		{
			typeof(TradeAccountPartPaymentPlan).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
		}

		public void Upsert(string uniVerseAccount = "MERTEX")
		{
			TradeAccountPartPaymentPlanFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
		}

        /// <summary>
		/// Trade Account
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("CUST.REF")]
        public string TradeAccountCode { get; set; }

		/// <summary>
		/// Pay Reason
		/// </summary>
		[UniVerseIndex(2)]
		[UniVerseFieldName("PAY.REASON")]
		public string PayReason { get; set; }

		/// <summary>
		/// Contract "Number"
		/// </summary>
		[UniVerseIndex(3)]
		[UniVerseFieldName("CONT.NUMBER")]
		public string ContractNumber { get; set; }

		/// <summary>
		/// Contract Description
		/// </summary>
		[UniVerseIndex(4)]
		[UniVerseFieldName("CONT.DESCRIPTION")]
		public string ContractDescription { get; set; }

		/// <summary>
		/// Total Amount to Bill
		/// </summary>
		[UniVerseIndex(5)]
		[UniVerseFieldName("TOT.AMOUNT.TO.BILL")]
		[UniVerseConversion("MR23")]
		public decimal TotalAmountToBill { get; set; }

		/// <summary>
		/// Total Billed to Date
		/// </summary>
		[UniVerseIndex(6)]
		[UniVerseFieldName("TOT.AMOUNT.TO.BILL")]
		[UniVerseConversion("MR23")]
		public decimal TotalBilledToDate { get; set; }

		/// <summary>
		/// Number of Periods
		/// </summary>
		[UniVerseIndex(7)]
		[UniVerseFieldName("NO.OF.PERIODS")]
		public int NumberOfPeriods { get; set; }

		/// <summary>
		/// Current(?) Period Amount
		/// </summary>
		[UniVerseIndex(8)]
		[UniVerseFieldName("PERIOD.AMOUNT")]
		[UniVerseConversion("MR23")]
		public decimal CurrentPeriodAmount { get; set; }

		public DateTime? PaymentStartDT { get; set; }

		[UniVerseIndex(9)]
		[UniVerseFieldName("SS.PAY.START.DATE.INT")]
		public string PaymentStartDate
		{
			get
			{
				if (PaymentStartDT.HasValue)
				{
					return Framework.UniVerseDateString(PaymentStartDT.Value);
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
						PaymentStartDT = Framework.DateTimeFromUniVerseOffset(valInt);
					}
					else
					{
						PaymentStartDT = null;
					}
				}
			}
		}

		public DateTime? LastInvoiceRaisedDT { get; set; }

		[UniVerseIndex(10)]
		[UniVerseFieldName("SS.LAST.INV.RAISED.INT")]
		public string LastInvoiceRaisedDate
		{
			get
			{
				if (LastInvoiceRaisedDT.HasValue)
				{
					return Framework.UniVerseDateString(LastInvoiceRaisedDT.Value);
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
						LastInvoiceRaisedDT = Framework.DateTimeFromUniVerseOffset(valInt);
					}
					else
					{
						LastInvoiceRaisedDT = null;
					}
				}
			}
		}


		// Continue mapping fields 11m, 12m, 13m, 14m, 15m, 16b, 17d
	}
}
