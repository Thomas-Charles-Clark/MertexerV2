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
        public decimal? CarriageValue  { get; set; }

        /// <summary>
        /// Carriage Value (Currency)
        /// </summary>
        [UniVerseIndex(26)]
        [UniVerseConversion("MR22")]
        [UniVerseFieldName("CARRIAGE.CUR")]
        public decimal? CarriageValueCurrency { get; set; }

        /// <summary>
        /// Currency Code
        /// </summary>
        [UniVerseIndex(45)]
        [UniVerseFieldName("CURRENCY")]
        public string CurrencyCode { get; set; }
    }
}
