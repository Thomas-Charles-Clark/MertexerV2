using MertexerV2.InvoiceFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Invoices
{
    [UniVerseFileName("OEINVHEAD")]
    public class InvoiceHead : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public InvoiceHead(string key) : base(key)
        {
            Initialise();
        }

        public InvoiceHead() : base()
        {
            Initialise();
        }


        public override void Initialise()
        {
            typeof(InvoiceHead).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            InvoiceHeadFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        [JsonProperty("InvoiceID")]
		public override string Key { get => base.Key; set => base.Key = value; }

		public DateTime? InvoiceDT { get; set; }

        /// <summary>
        /// Invoice Date
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("DOCDATE")]
        public string InvoiceDate
        {
            get
            {
                if (InvoiceDT.HasValue)
                {
                    return Framework.UniVerseDateString(InvoiceDT.Value);
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
                        InvoiceDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        InvoiceDT = null;
                    }
                }
            }
        }

        /// <summary>
        /// Customer Reference
        /// </summary>
        [UniVerseIndex(4)]
        [UniVerseFieldName("TERMSCODE")]
        public string Terms { get; set; }

        /// <summary>
        /// Customer Reference
        /// </summary>
        [UniVerseIndex(5)]
        [UniVerseFieldName("CUSREF")]
        public string TradeAccountCode { get; set; }

        /// <summary>
        /// Order Number
        /// </summary>
        [UniVerseIndex(10)]
        [UniVerseFieldName("ORDENRO")]
        public string OrderID { get; set; }

        
        /// <summary>
        /// Invoice Memo Lines
        /// </summary>
        [UniVerseIndex(16)]
        [UniVerseFieldName("IMEMO")]
        [UniVerseMultiValue]
        public IEnumerable<string> InvoiceMemoLines { get; set; }

        /// <summary>
        /// Invoice or Credit
        /// </summary>
        [UniVerseIndex(18)]
        [UniVerseFieldName("TRANSTYPE")]
        public byte InvoiceOrCredit { get; set; }

        /// <summary>
        /// Carriage Value
        /// </summary>
        [UniVerseIndex(19)]
        [UniVerseConversion("MR22")]
        [UniVerseFieldName("CARRIAGE")]
        public decimal? CarriageValue { get; set; }

        /// <summary>
        /// Carriage Value (Currency)
        /// </summary>
        [UniVerseIndex(26)]
        [UniVerseConversion("MR22")]
        [UniVerseFieldName("CARRIAGE.CUR")]
        public decimal? CarriageValueCurrency { get; set; }

		// Emit <40> SS.DUEDATE.INT
		public DateTime? DueDT { get; set; }

		/// <summary>
		/// Invoice Date
		/// </summary>
		[UniVerseIndex(40)]
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

		// Emit <42> SS.DESPDATE.INT
		public DateTime? DespatchedDT { get; set; }

		/// <summary>
		/// Despatched Date
		/// </summary>
		[UniVerseIndex(42)]
		[UniVerseFieldName("SS.DESPDATE.INT")]
		public string DespatchedDate
		{
			get
			{
				if (DespatchedDT.HasValue)
				{
					return Framework.UniVerseDateString(DespatchedDT.Value);
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
						DespatchedDT = Framework.DateTimeFromUniVerseOffset(valInt);
					}
					else
					{
						DespatchedDT = null;
					}
				}
			}
		}

		/// <summary>
		/// Currency Code
		/// </summary>
		[UniVerseIndex(45)]
        [UniVerseFieldName("CURRENCY")]
        public string CurrencyCode { get; set; }


		/// <summary>
		/// Associated Invoice ID
		/// </summary>
		[UniVerseIndex(48)]
		[UniVerseFieldName("CRINVNO")]
		public string AssociatedInvoiceID { get; set; }

		/// <summary>
		/// Total Value (Sterling)
		/// </summary>
		[UniVerseIndex(61)]
        [UniVerseConversion("MR22")]
        [UniVerseFieldName("TOTVAL")]
        public decimal TotalValue { get; set; }

        /// <summary>
        /// Total Value (Sterling)
        /// </summary>
        [UniVerseIndex(64)]
        [UniVerseConversion("MR22")]
        [UniVerseFieldName("TOTVAT")]
        public decimal TotalVAT { get; set; }

        /// <summary>
        /// VAT Codes
        /// </summary>
        [UniVerseIndex(70)]
        [UniVerseFieldName("SVATCODE")]
        [UniVerseMultiValue]
        public IEnumerable<string> VATCodes { get; set; }

        public string VATCode
        {
            get
            {
                return VATCodes?.Any() == true ? VATCodes.First() : null;
            }
        }

        /// <summary>
        /// VAT Codes
        /// </summary>
        [UniVerseIndex(71)]
        [UniVerseFieldName("SVATRATE")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> VATRates { get; set; }

        public decimal VATRate
        {
            get
            {
                return VATRates?.Any() == true ? VATRates.First() : 0.0m;
            }
        }

        /// <summary>
        /// VAT Value
        /// </summary>
        [UniVerseIndex(72)]
        [UniVerseFieldName("SVATVALUE")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> VATableValues { get; set; }

        public decimal VATableValue
        {
            get
            {
                return VATableValues?.Any() == true ? VATableValues.First() : 0.0m;
            }
        }

        /// <summary>
        /// VAT Codes
        /// </summary>
        [UniVerseIndex(73)]
        [UniVerseFieldName("SVATTAX")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> VATTaxes { get; set; }

        public decimal VATTax
        {
            get
            {
                return VATTaxes?.Any() == true ? VATTaxes.First() : 0.0m;
            }
        }

        /// <summary>
        /// VAT Value
        /// </summary>
        [UniVerseIndex(72)]
        [UniVerseFieldName("SVATVALUE.CUR")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> VATableValuesCurrency { get; set; }

        public decimal VATableValueCurrency
        {
            get
            {
                return VATableValuesCurrency?.Any() == true ? VATableValuesCurrency.First() : 0.0m;
            }
        }

        /// <summary>
        /// VAT Codes
        /// </summary>
        [UniVerseIndex(73)]
        [UniVerseFieldName("SVATTAX.CUR")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> VATTaxesCurrency { get; set; }

        public decimal VATTaxCurrency
        {
            get
            {
                return VATTaxesCurrency?.Any() == true ? VATTaxesCurrency.First() : 0.0m;
            }
        }

        /// <summary>
        /// Total Value (Currency)
        /// </summary>
        [UniVerseIndex(87)]
        [UniVerseConversion("MR22")]
        [UniVerseFieldName("TOTVAL.CUR")]
        public decimal TotalValueCurrency { get; set; }

        /// <summary>
        /// Total Value (Sterling)
        /// </summary>
        [UniVerseIndex(88)]
        [UniVerseConversion("MR22")]
        [UniVerseFieldName("TOTVAT.CUR")]
        public decimal TotalVATCurrency { get; set; }

		/// <summary>
		/// Delivery Terms Code
		/// </summary>
		[UniVerseIndex(97)]
		[UniVerseFieldName("DELTERMS")]
		public string DeliveryTerms { get; set; }

		/// <summary>
		/// Invoice Clause Lines:
		/// Clause text lifted at point of creation / modification of InvoiceClauseCodes
		/// </summary>
		[UniVerseIndex(134)]
		[UniVerseFieldName("INVCLAUSE")]
		[UniVerseMultiValue]
		public IEnumerable<string> InvoiceClauseLines { get; set; }

		/// <summary>
		/// Customer Invoice Clauses:
		/// Pulled via reference from SLCUST<203>
		/// </summary>
		[UniVerseIndex(0)]
		[UniVerseFieldName("CUSREF.INVCLAUSE")]
		[UniVerseMultiValue]
		public IEnumerable<string> CustomerInvoiceClauseLines { get; set; }

		/// <summary>
		/// OrderHead Invoice Clauses:
		/// Pulled via reference from TOEHEAD<88>
		/// </summary>
		[UniVerseIndex(0)]
		[UniVerseFieldName("ORD.INVOICES")]
		[UniVerseMultiValue]
		public IEnumerable<string> OrderHeadInvoiceLines { get; set; }

		/// Miscellaneous Charge Code
		/// </summary>
		[UniVerseIndex(160)]
        [UniVerseFieldName("MCHRG.CODE")]
        public string MiscellaneousChargeCode { get; set; }

        /// <summary>
        /// Miscellaneous Charge Value
        /// </summary>
        [UniVerseIndex(161)]
        [UniVerseFieldName("MCHRG.VAL")]
        [UniVerseConversion("MR22")]
        public decimal MiscellaneousChargeValue { get; set; }

        /// <summary>
        /// Miscellaneous Charge Value
        /// </summary>
        [UniVerseIndex(162)]
        [UniVerseFieldName("MCHRG.VAL.CUR")]
        [UniVerseConversion("MR22")]
        public decimal MiscellaneousChargeValueCurrency { get; set; }

        /// <summary>
        /// Miscellaneous Charge Value
        /// </summary>
        [UniVerseIndex(161)]
        [UniVerseFieldName("MCHRG.VAT")]
        [UniVerseConversion("MR22")]
        public decimal MiscellaneousChargeVAT { get; set; }

        /// <summary>
        /// Miscellaneous Charge Value
        /// </summary>
        [UniVerseIndex(162)]
        [UniVerseFieldName("MCHRG.VAT.CUR")]
        [UniVerseConversion("MR22")]
        public decimal MiscellaneousChargeVATCurrency { get; set; }

        /// <summary>
        /// <summary>
        /// Carriage VAT Code
        /// </summary>
        [UniVerseIndex(164)]
        [UniVerseFieldName("CARR.VCODE")]
        public string CarriageVATCode { get; set; }

        /// <summary>
        /// Carriage VAT
        /// </summary>
        [UniVerseIndex(314)]
        [UniVerseConversion("MR22")]
        [UniVerseFieldName("CARRVAT")]
        public decimal CarriageVAT { get; set; }

        /// <summary>
        /// Carriage VAT
        /// </summary>
        [UniVerseIndex(315)]
        [UniVerseConversion("MR22")]
        [UniVerseFieldName("CARRVAT.CUR")]
        public decimal CarriageVATCurrency { get; set; }

        /// <summary>
        /// TotalValueSterling
        /// </summary>
        [UniVerseIndex(0)]
        [UniVerseFieldName("C.TOTVAL")]
        [UniVerseConversion("MR22")]
        public decimal CalculatedTotalValue
        {
            get
            {
                var firstMultiplier = 0;
                var secondMultiplier = 0;

                if (InvoiceOrCredit == 1)
                {
                    firstMultiplier = 1;
                    secondMultiplier = 0;
                }
                else
                {
                    firstMultiplier = 0;
                    secondMultiplier = 1;
                }

                return (firstMultiplier * TotalValue) - (secondMultiplier * TotalValue);
            }
        }

        /// <summary>
        /// TotalValueCurrency
        /// </summary>
        [UniVerseIndex(0)]
        [UniVerseFieldName("C.TOTVAL.CUR")]
        [UniVerseConversion("MR22")]
        public decimal CalculatedTotalValueCurrency
        {
            get
            {
                var firstMultiplier = 0;
                var secondMultiplier = 0;

                if (InvoiceOrCredit == 1)
                {
                    firstMultiplier = 1;
                    secondMultiplier = 0;
                }
                else
                {
                    firstMultiplier = 0;
                    secondMultiplier = 1;
                }

                return (firstMultiplier * TotalValueCurrency) - (secondMultiplier * TotalValueCurrency);
            }
        }
        /// <summary>
        /// TotalValueSterling
        /// </summary>
        [UniVerseIndex(0)]
        [UniVerseFieldName("C.TOTVAT")]
        [UniVerseConversion("MR22")]
        public decimal CalculatedTotalVAT
        {
            get
            {
                var firstMultiplier = 0;
                var secondMultiplier = 0;

                if (InvoiceOrCredit == 1)
                {
                    firstMultiplier = 1;
                    secondMultiplier = 0;
                }
                else
                {
                    firstMultiplier = 0;
                    secondMultiplier = 1;
                }

                return (firstMultiplier * TotalVAT) - (secondMultiplier * TotalVAT);
            }
        }

        /// <summary>
        /// TotalValueCurrency
        /// </summary>
        [UniVerseIndex(0)]
        [UniVerseFieldName("C.TOTVAT.CUR")]
        [UniVerseConversion("MR22")]
        public decimal CalculatedTotalVATCurrency
        {
            get
            {
                var firstMultiplier = 0;
                var secondMultiplier = 0;

                if (InvoiceOrCredit == 1)
                {
                    firstMultiplier = 1;
                    secondMultiplier = 0;
                }
                else
                {
                    firstMultiplier = 0;
                    secondMultiplier = 1;
                }

                return (firstMultiplier * TotalVATCurrency) - (secondMultiplier * TotalVATCurrency);
            }
        }
    }
}
