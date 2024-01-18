using MertexerV2.AccountFunctions;
using MertexerV2.Utilities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.CustomerCare
{
    [UniVerseFileName("TCCLINE")]
    public class CustomerCareRecordLine : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public CustomerCareRecordLine(string key) : base(key)
        {
            Initialise();
        }

        public CustomerCareRecordLine() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(CustomerCareRecordLine).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            CustomerCareRecordFunctions.UpsertLine(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("PRODCODE")]
        public string ProductCode { get; set; }

        [UniVerseIndex(2)]
        [UniVerseFieldName("PRODDESC")]
        public string ProductDescription { get; set; }

        [UniVerseIndex(3)]
        [UniVerseFieldName("QTY.CLAIM")]
        [UniVerseConversion("MR33")]
        public decimal? QuantityClaimed { get; set; }

        [UniVerseIndex(4)]
        [UniVerseMultiValue]
        [UniVerseFieldName("FAULTCODE")]
        public IEnumerable<string> FaultCodes { get; set; }

        [UniVerseIndex(4, 1)]
        [UniVerseFieldName("FAULCODE1")]
        public string FaultCode_First { get; set; }

        [UniVerseIndex(4, 2)]
        [UniVerseFieldName("FAULCODE2")]
        public string FaultCode_Second { get; set; }

        [UniVerseIndex(5)]
        [UniVerseFieldName("INVOICENO")]
        public string InvoiceNumber { get; set; }

        [UniVerseIndex(6)]
        [UniVerseFieldName("QTY.ACCEPT")]
        [UniVerseConversion("MR33")]
        public decimal? QuantityAccepted { get; set; }

        [UniVerseIndex(7)]
        [UniVerseFieldName("RETURN.STOCK")]
        public bool ReturnToStock { get; set; }

        [UniVerseIndex(8)]
        [UniVerseFieldName("ORDPRICE")]
        [UniVerseConversion("MR44")]
        public decimal? Price { get; set; }

        [UniVerseIndex(9)]
        [UniVerseFieldName("PERC.DISC")]
        [UniVerseConversion("MR33")]
        public decimal? PercentageDiscount { get; set; }

        [UniVerseIndex(10)]
        [UniVerseFieldName("MESSLINE")]
        public string MessageLine { get; set; }

        [UniVerseIndex(11)]
        [UniVerseFieldName("TEXT")]
        public string Text { get; set; }

        public DateTime? InvoiceDT { get; set; }

        /// <summary>
        /// Invoice Date
        /// </summary>
        [UniVerseIndex(12)]
        [UniVerseFieldName("INVDATE")]
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

        [UniVerseIndex(20)]
        [UniVerseFieldName("S.SUPREF")]
        public string SupplierReference { get; set; }

        [UniVerseIndex(21)]
        [UniVerseFieldName("S.CONTACT")]
        public string ContactName { get; set; }

        [UniVerseIndex(22)]
        [UniVerseFieldName("S.RETNO")]
        public string SupplierReturnNumber { get; set; }

        public DateTime? ReturnDT { get; set; }

        /// <summary>
        /// Return Date
        /// </summary>
        [UniVerseIndex(23)]
        [UniVerseFieldName("S.RETDATE")]
        public string ReturnDate
        {
            get
            {
                if (ReturnDT.HasValue)
                {
                    return Framework.UniVerseDateString(ReturnDT.Value);
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
                        ReturnDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        ReturnDT = null;
                    }
                }
            }
        }

        [UniVerseIndex(24)]
        [UniVerseFieldName("S.CARRIER")]
        public string Carrier { get; set; }

        [UniVerseIndex(25)]
        [UniVerseMultiValue]
        [UniVerseFieldName("S.COMMENTS")]
        public IEnumerable<string> Comments { get; set; }
        
        [UniVerseIndex(26)]
        [UniVerseFieldName("S.CREDAMT")]
        [UniVerseConversion("MR44")]
        public decimal? CreditAmount { get; set; }

        [UniVerseIndex(27)]
        [UniVerseFieldName("S.STOCKPRICE")]
        [UniVerseConversion("MR44")]
        public decimal? StockPrice { get; set; }

        [UniVerseIndex(47)]
        [UniVerseFieldName("ORDPRICE.CUR")]
        [UniVerseConversion("MR24")]
        public decimal? OrderPriceCurrency { get; set; }

        [UniVerseIndex(48)]
        [UniVerseFieldName("NSPRODCODE")]
        public string NSProductCode { get; set; }

        [UniVerseIndex(49)]
        [UniVerseFieldName("RECTYPE")]
        public string RecordType { get; set; }

        [UniVerseIndex(50)]
        [UniVerseFieldName("SUNDRY.UNIT")]
        public string SundryUnit { get; set; }

        [UniVerseIndex(51)]
        [UniVerseFieldName("NS.COSTPRICE")]
        public decimal? NSCostPrice { get; set; }

        [UniVerseIndex(52)]
        [UniVerseFieldName("NS.WEIGHT")]
        public decimal? NSWeight { get; set; }

        [UniVerseIndex(53)]
        [UniVerseMultiValue]
        [UniVerseFieldName("LNMEMO")]
        public IEnumerable<string> LineMemos { get; set; }

        [UniVerseIndex(54)]
        [UniVerseFieldName("QTY.RETURNED")]
        [UniVerseConversion("MR33")]
        public decimal? QuantityReturned { get; set; }

        [UniVerseIndex(55)]
        [UniVerseMultiValue]
        [UniVerseFieldName("SORDLINE")]
        public IEnumerable<string> OrderLineIDs { get; set; }

        [UniVerseIndex(55, 1)]
        [UniVerseFieldName("SORDLINE.SEL")]
        public string OrderLineID { get; set; }

        [UniVerseIndex(56)]
        [UniVerseMultiValue]
        [UniVerseFieldName("RETURNNOS")]
        public IEnumerable<string> CustomerReturnNumbers { get; set; }

        [UniVerseIndex(57)]
        [UniVerseMultiValue]
        [UniVerseFieldName("RETURNQTY")]
        [UniVerseConversion("MR33")]
        public IEnumerable<decimal> CustomerReturnQuantities { get; set; }

        [UniVerseIndex(58)]
        [UniVerseFieldName("SUN.VATCODE")]
        public string SundryVATCode { get; set; }

        [UniVerseIndex(59)]
        [UniVerseFieldName("SUN.TARIFF.CODE")]
        public string SundryTariffCode { get; set; }

        [UniVerseIndex(60)]
        [UniVerseFieldName("SUN.VATVALUE")]
        [UniVerseConversion("MR22")]
        public decimal? SundryVATValue { get; set; }

        [UniVerseIndex(62)]
        [UniVerseFieldName("VAT.INCLUSIVE")]
        public bool VATInclusive { get; set; }

        [UniVerseIndex(63)]
        [UniVerseFieldName("CUSPRODCODE")]
        public string CustomerProductCode { get; set; }

        [UniVerseIndex(66)]
        [UniVerseFieldName("INVOICE.NO.DES")]
        public string InvoiceLineNumber { get; set; }

        [UniVerseIndex(75)]
        [UniVerseMultiValue]
        [UniVerseFieldName("COMP.PIECES")]
        public IEnumerable<string> ComplaintPieces { get; set; }
        
        [UniVerseIndex(76)]
        [UniVerseFieldName("BESPOKE.LEN")]
        public decimal? BespokeLength { get; set; }

        [UniVerseIndex(77)]
        [UniVerseFieldName("BESPOKE.WID")]
        public decimal? BespokeWidth { get; set; }

        [UniVerseIndex(78)]
        [UniVerseFieldName("BESPOKE.SIZE")]
        public decimal? BespokeSize { get; set; }
    }
}
