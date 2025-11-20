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

		/// <summary>
		/// "Pay Amount"
		/// </summary>
		[UniVerseIndex(11)]
		[UniVerseFieldName("PAY.AMOUNT")]
		[UniVerseConversion("MR23")]
		[UniVerseMultiValue]
		public IEnumerable<decimal> PaymentAmounts { get; set; }

		/// <summary>
		/// Invoice Numbers
		/// </summary>
		[UniVerseIndex(12)]
		[UniVerseFieldName("INVOICE.NO")]
		[UniVerseMultiValue]
		public IEnumerable<string> InvoiceNumbers { get; set; }
		// Continue mapping fields 11m, 12m, 13m, 14m, 15m, 16b, 17d

		/// <summary>
		/// Invoice Payment Dates
		/// </summary>
		[UniVerseIndex(13)]
		[UniVerseFieldName("SS.INV.PAY.DATE.INT")]
		[UniVerseMultiValue]
		public IEnumerable<string> InvoicePaymentDates
		{
			get
			{
				if (InvoicePaymentDTs.Any())
				{
					return InvoicePaymentDTs.Select(dt => Framework.UniVerseDateString(dt));
				}
				else
				{
					return new List<string>();
				}
			}
			set
			{
				if (value != null)
				{
					InvoicePaymentDTs = value.Select(v => Framework.DateTimeFromUniVerseOffset(Convert.ToInt32(v)));
				}
				else
				{
					InvoicePaymentDTs = new List<DateTime>();
				}
			}
		}

		public IEnumerable<DateTime> InvoicePaymentDTs { get; set; }

		/// <summary>
		/// Invoice Sequence Numbers
		/// </summary>
		[UniVerseIndex(14)]
		[UniVerseFieldName("INV.SEQ.NO")]
		[UniVerseMultiValue]
		public IEnumerable<int> InvoiceSequenceNumbers { get; set; }

		/// <summary>
		/// Invoice Text
		/// </summary>
		[UniVerseIndex(15)]
		[UniVerseFieldName("INVOICE.TEXT")]
		[UniVerseMultiValue]
		public IEnumerable<string> InvoiceTextContents { get; set; }

		/// <summary>
		/// Active Plan
		/// </summary>
		[UniVerseIndex(16)]
		[UniVerseFieldName("ACTIVE.PLAN")]
		public bool ActivePlan { get; set; }

		public DateTime? CreatedDT { get; set; }

		[UniVerseIndex(17)]
		[UniVerseFieldName("SS.CREATED.DATE.INT")]
		public string CreatedDate
		{
			get
			{
				if (CreatedDT.HasValue)
				{
					return Framework.UniVerseDateString(CreatedDT.Value);
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
						CreatedDT = Framework.DateTimeFromUniVerseOffset(valInt);
					}
					else
					{
						CreatedDT = null;
					}
				}
			}
		}
	}
}
