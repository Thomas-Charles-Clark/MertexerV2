using MertexerV2.Utilities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.SalesAnalysis
{
    [UniVerseFileName("OESANAL")]
    public class SalesAnalysis : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public SalesAnalysis(string key) : base(key)
        {
            Initialise();
        }

        public SalesAnalysis() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(SalesAnalysis).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        /// <summary>
        /// Trade Account Code
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("CUSREF")]
        public string TradeAccountCode { get; set; }

        /// <summary>
        /// Sales Representative Code
        /// </summary>
        [UniVerseIndex(14)]
        [UniVerseFieldName("SEL1")]
        public string SalesRepresentativeCode { get; set; }

        /// <summary>
        /// Sales Agent Code
        /// </summary>
        [UniVerseIndex(22)]
        [UniVerseFieldName("SEL2")]
        public string SalesAgentCode { get; set; }

        /// <summary>
        /// Sales Area Code
        /// </summary>
        [UniVerseIndex(23)]
        [UniVerseFieldName("SEL3")]
        public string SalesAreaCode { get; set; }

        /// <summary>
        /// Market Code
        /// </summary>
        [UniVerseIndex(24)]
        [UniVerseFieldName("TRADE.TYPE")]
        public string MarketCode { get; set; }

        /// <summary>
        /// Country Code
        /// </summary>
        [UniVerseIndex(37)]
        [UniVerseFieldName("COUNTRY.DES")]
        public string CountryCode { get; set; }

        /// <summary>
        /// Retail or Contract Indicator
        /// </summary>
        [UniVerseIndex(124)]
        [UniVerseFieldName("RET.CONT.DES")]
        public string RetailOrContractIndicator { get; set; }

        /// <summary>
        /// Customer Type Code
        /// </summary>
        [UniVerseIndex(129)]
        [UniVerseFieldName("CUSTYPE")]
        public string CustomerTypeCode { get; set; }

        /// <summary>
        /// Customer Class Code
        /// </summary>
        [UniVerseIndex(130)]
        [UniVerseFieldName("CUST.CLASS.DES")]
        public string CustomerClassCode { get; set; }

        /// <summary>
        /// Trade Account Group Code
        /// </summary>
        [UniVerseIndex(140)]
        [UniVerseFieldName("CODE.GROUP.DES")]
        public string TradeAccountGroupCode { get; set; }

        /// <summary>
        /// End Customer Trade Account Code
        /// </summary>
        [UniVerseIndex(75)]
        [UniVerseFieldName("ENDCUST")]
        public string EndCustomerTradeAccountCode { get; set; }

        /// <summary>
        /// Product Group Code
        /// </summary>
        [UniVerseIndex(9)]
        [UniVerseFieldName("PRODGROUP")]
        public string ProductGroupCode { get; set; }

        /// <summary>
        /// Brand Code
        /// </summary>
        [UniVerseIndex(126)]
        [UniVerseFieldName("SALES.DIV.DES")]
        public string BrandCode { get; set; }

        /// <summary>
        /// Item Type Code
        /// </summary>
        [UniVerseIndex(105)]
        //[UniVerseMultiValue]
        //[UniVerseConversion("MR44")]
        [UniVerseFieldName("ITEMTYPE.DES")]
        //[JsonConverter(typeof(StringToArrayConverter<IEnumerable<decimal?>>))]
        public string ItemTypeCode { get; set; }

        /// <summary>
        /// Collection Code
        /// </summary>
        [UniVerseIndex(127)]
        [UniVerseFieldName("PROD.COLL.DES")]
        public string CollectionCode { get; set; }

        /// <summary>
        /// PatternBookProductCode
        /// </summary>
        [UniVerseIndex(168)]
        [UniVerseFieldName("PATTERN.BOOK.DES")]
        public string PatternBookProductCode { get; set; }

        /// <summary>
        /// Order ID
        /// </summary>
        [UniVerseIndex(35)]
        [UniVerseFieldName("ORDERNO")]
        public string OrderID { get; set; }

        /// <summary>
        /// Order Taker Code
        /// </summary>
        [UniVerseIndex(137)]
        [UniVerseFieldName("ORDER.TAKER.DES")]
        public string OrderTakerCode { get; set; }

        public DateTime? OrderEntryDT { get; set; }

        /// <summary>
        /// Order Entry Date
        /// </summary>
        [UniVerseIndex(17)]
        [UniVerseFieldName("SS.ORDERDATE")]
        public string OrderEntryDate
        {
            get
            {
                if (OrderEntryDT.HasValue)
                {
                    return Framework.UniVerseDateString(OrderEntryDT.Value);
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
                        OrderEntryDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        OrderEntryDT = null;
                    }
                }
            }
        }

        public DateTime? OrderDT { get; set; }

        /// <summary>
        /// Order Date
        /// </summary>
        [UniVerseIndex(16)]
        [UniVerseFieldName("SS.CUSORDERDATE")]
        public string OrderDate
        {
            get
            {
                if (OrderDT.HasValue)
                {
                    return Framework.UniVerseDateString(OrderDT.Value);
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
                        OrderDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        OrderDT = null;
                    }
                }
            }
        }

        /// <summary>
        /// Haulier Code
        /// </summary>
        [UniVerseIndex(19)]
        [UniVerseFieldName("HAULIERCODE")]
        public string HaulierCode { get; set; }

        /// <summary>
        /// Product Code
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("PRODCODE")]
        public string ProductCode { get; set; }

        /// <summary>
        /// Stock Location Code
        /// </summary>
        [UniVerseIndex(26)]
        [UniVerseFieldName("LOCCODE")]
        public string StockLocationCode { get; set; }

        /// <summary>
        /// Quantity Ordered
        /// </summary>
        [UniVerseIndex(56)]
        [UniVerseConversion("MR33")]
        [UniVerseFieldName("M.QTY")]
        public decimal? QuantityOrdered { get; set; }

        /// <summary>
        /// Stock Unit Code
        /// </summary>
        [UniVerseIndex(36)]
        [UniVerseFieldName("ORDER.UNIT")]
        public string StockUnitCode { get; set; }

        /// <summary>
        /// Sales Value Currency
        /// </summary>
        [UniVerseIndex(54)]
        [UniVerseConversion("MR22")]
        [UniVerseFieldName("M.ORDVAL.CUR")]
        public decimal? SalesValueCurrency { get; set; }

        /// <summary>
        /// Sales Value Sterling
        /// </summary>
        [UniVerseIndex(57)]
        [UniVerseConversion("MR22")]
        [UniVerseFieldName("M.SALEVAL")]
        public decimal? SalesValueSterling { get; set; }

        /// <summary>
        /// Cost Value Currency
        /// </summary>
        [UniVerseIndex(55)]
        [UniVerseConversion("MR44")]
        [UniVerseFieldName("M.COSTVAL.CUR")]
        public decimal? CostValueCurrency { get; set; }

        /// <summary>
        /// Cost Value Sterling
        /// </summary>
        [UniVerseIndex(58)]
        [UniVerseConversion("MR22")]
        [UniVerseFieldName("M.COSTVAL")]
        public decimal? CostValueSterling { get; set; }

        /// <summary>
        /// Discount Percentage
        /// </summary>
        [UniVerseIndex(66)]
        [UniVerseConversion("MR33")]
        [UniVerseFieldName("DISC%")]
        public decimal? DiscountPercentage { get; set; }

        /// <summary>
        /// Discount Value
        /// </summary>
        [UniVerseIndex(65)]
        [UniVerseConversion("MR33")]
        [UniVerseFieldName("DISCVAL")]
        public decimal? DiscountValue { get; set; }

        /// <summary>
        /// Transfer Value Currency
        /// </summary>
        [UniVerseIndex(174)]
        [UniVerseConversion("MR22")]
        [UniVerseFieldName("TRANS.IN.VAL.CUR.DES")]
        public decimal? TransferValueCurrency { get; set; }

        /// <summary>
        /// Transfer Value Sterling
        /// </summary>
        [UniVerseIndex(173)]
        [UniVerseConversion("MR22")]
        [UniVerseFieldName("TRANS.IN.VAL.DES")]
        public decimal? TransferValueSterling { get; set; }

        public DateTime? DespatchDT { get; set; }

        /// <summary>
        /// Despatch Date
        /// </summary>
        [UniVerseIndex(18,1)]
        [UniVerseFieldName("SS.DESP.DATE.DES")]
        public string DespatchDate { get; set; } = null;

        /// <summary>
        /// Credit or Invoice Indicator
        /// </summary>
        [UniVerseIndex(11)]
        [UniVerseFieldName("TYPE")]
        public string CreditOrInvoiceIndicator { get; set; }

        /// <summary>
        /// Record Type
        /// </summary>
        [UniVerseIndex(13)]
        [UniVerseFieldName("RECTYPE")]
        public byte RecordType { get; set; }

        public DateTime? InvoiceDT { get; set; }

        /// <summary>
        /// Invoice Date
        /// </summary>
        [UniVerseIndex(3)]
        [UniVerseFieldName("SS.DOCDATE.DES")]
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

        public DateTime? PostDT { get; set; }

        /// <summary>
        /// Post Date
        /// </summary>
        [UniVerseIndex(4)]
        [UniVerseFieldName("SS.POSTDATE.DES")]
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

        /// <summary>
        /// Invoice ID
        /// </summary>
        [UniVerseIndex(20)]
        [UniVerseFieldName("INVOICE.NO")]
        public string InvoiceID { get; set; }

        /// <summary>
        /// General Ledger Sales Code
        /// </summary>
        [UniVerseIndex(166)]
        [UniVerseFieldName("GL.CODE.DES")]
        public string GeneralLedgerSaleCode { get; set; }

        /// <summary>
        /// General Ledger Cost Code
        /// </summary>
        [UniVerseIndex(170)]
        [UniVerseFieldName("COS.GLCODE.DES")]
        public string GeneralLedgerCostCode { get; set; }

        /// <summary>
        /// General Ledger Promotion Code
        /// </summary>
        [UniVerseIndex(167)]
        [UniVerseFieldName("DISC.GLCODE.DES")]
        public string GeneralLedgerPromotionCode { get; set; }

        /// <summary>
        /// General Ledger Stock Code
        /// </summary>
        [UniVerseIndex(169)]
        [UniVerseFieldName("STOCK.GLCODE.DES")]
        public string GeneralLedgerStockCode { get; set; }

        /// <summary>
        /// Mertex Currency Code
        /// </summary>
        [UniVerseIndex(136)]
        [UniVerseFieldName("CURRENCY.DES")]
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Sterling Exchange ate
        /// </summary>
        [UniVerseIndex(139)]
        [UniVerseConversion("MR66")]
        [UniVerseFieldName("XRATE")]
        public decimal? ExchangeRate { get; set; }

        /// <summary>
        /// Financial Period
        /// </summary>
        [UniVerseIndex(150)]
        [UniVerseFieldName("F.PERIOD")]
        public int FinancialPeriod { get; set; }

        /// <summary>
        /// Multiplication Factor
        /// </summary>
        [UniVerseIndex(12)]
        [UniVerseFieldName("MULT.FACTOR")]
        public decimal? MultiplicationFactor { get; set; }

        /// <summary>
        /// Promotion Code
        /// </summary>
        [UniVerseIndex(131)]
        [UniVerseFieldName("PROMO.CODE.DES")]
        public string PromotionCode { get; set; }

        /// <summary>
        /// Promotion Type Code
        /// </summary>
        [UniVerseIndex(138)]
        [UniVerseFieldName("PROMO.DISC.TYPE")]
        public string PromotionTypeCode { get; set; }

        /// <summary>
        /// Free of Charge Indicator
        /// </summary>
        [UniVerseIndex(132)]
        [UniVerseFieldName("FOC.INDICATOR.DES")]
        public bool FreeOfChargeIndicator { get; set; }

        /// <summary>
        /// Order Category Code
        /// </summary>
        [UniVerseIndex(123)]
        [UniVerseFieldName("ORDER.CATAGORY.DES")]
        public byte OrderCategoryCode { get; set; }
    }
}
