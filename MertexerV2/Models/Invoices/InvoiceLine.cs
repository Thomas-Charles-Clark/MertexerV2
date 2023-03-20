using MertexerV2.InvoiceFunctions;
using MertexerV2.ProductFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Invoices
{
    [UniVerseFileName("OEINVLINE")]
    public class InvoiceLine : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public InvoiceLine(string key) : base(key)
        {
            Initialise();
        }

        public InvoiceLine() : base()
        {
            Initialise();
        }


        public override void Initialise()
        {
            typeof(InvoiceLine).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            InvoiceLineFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Document Number
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("DOCNO")]
        public string InvoiceNo { get; set; }

        /// <summary>
        /// Product Code
        /// </summary>
        [UniVerseIndex(3)]
        [UniVerseFieldName("PRODCODE")]
        public string ProductCode { get; set; }

        /// <summary>
        /// Quantity Ordered
        /// </summary>
        [UniVerseIndex(5)]
        [UniVerseConversion("MR33")]
        [UniVerseFieldName("QTYORD.ORD")]
        public decimal? QuantityOrdered { get; set; }

        /// <summary>
        /// Quantity Despatched
        /// </summary>
        [UniVerseIndex(6)]
        [UniVerseConversion("MR33")]
        [UniVerseFieldName("QTYDES.ORD")]
        public decimal? QuantityDespatched  { get; set; }

        /// <summary>
        /// Quantity Invoiced
        /// </summary>
        [UniVerseIndex(7)]
        [UniVerseConversion("MR33")]
        [UniVerseFieldName("QTYINV.ORD")]
        public decimal? QuantityInvoiced { get; set; }

        /// <summary>
        /// Unit Price Ex VAT
        /// </summary>
        [UniVerseIndex(8)]
        [UniVerseConversion("MR44")]
        [UniVerseFieldName("PRICE")]
        public decimal? UnitPriceExVAT { get; set; }

        /// <summary>
        /// Line Value Ex VAT
        /// </summary>
        [UniVerseIndex(9)]
        [UniVerseConversion("MR22")]
        [UniVerseFieldName("VALUE")]
        public decimal? LineValueExVAT { get; set; }

        /// <summary>
        /// Discount Percentage
        /// </summary>
        [UniVerseIndex(10)]
        [UniVerseConversion("MR23")]
        [UniVerseFieldName("DISC%")]
        public decimal? DiscountPercentage { get; set; }

        /// <summary>
        /// VAT Code
        /// </summary>
        [UniVerseIndex(11)]
        [UniVerseFieldName("VATCODE")]
        public string VATCode { get; set; }

        /// <summary>
        /// VAT Rate
        /// </summary>
        [UniVerseIndex(12)]
        [UniVerseConversion("MR23")]
        [UniVerseFieldName("VATRATE")]
        public decimal? VATRate { get; set; }

        /// <summary>
        /// Discount Value
        /// </summary>
        [UniVerseIndex(13)]
        [UniVerseConversion("MR22")]
        [UniVerseFieldName("DISCVAL")]
        public decimal? DiscountValue { get; set; }

        /// <summary>
        /// VAT Value
        /// </summary>
        [UniVerseIndex(14)]
        [UniVerseConversion("MR22")]
        [UniVerseFieldName("VATVAL")]
        public decimal? VATValue { get; set; }

        /// <summary>
        /// Stock Unit
        /// </summary>
        [UniVerseIndex(22)]
        [UniVerseFieldName("UNIT")]
        public string StockUnit { get; set; }

        /// <summary>
        /// Unit Cost
        /// </summary>
        [UniVerseIndex(23)]
        [UniVerseConversion("MR44")]
        [UniVerseFieldName("COSTPRICE")]
        public decimal? UnitCost { get; set; }

        /// <summary>
        /// Unit Cost (Currency)
        /// </summary>
        [UniVerseIndex(24)]
        [UniVerseConversion("MR44")]
        [UniVerseFieldName("COSTPRICE.CUR")]
        public decimal? UnitCostCurrency { get; set; }

        /// <summary>
        /// Line Cost
        /// </summary>
        [UniVerseIndex(25)]
        [UniVerseConversion("MR44")]
        [UniVerseFieldName("COSTVALUE")]
        public decimal? LineCost { get; set; }

        /// <summary>
        /// Line Cost (Currency)
        /// </summary>
        [UniVerseIndex(26)]
        [UniVerseConversion("MR44")]
        [UniVerseFieldName("COSTVALUE.CUR")]
        public decimal? LineCostCurrency { get; set; }

        /// <summary>
        /// Unit Price Ex VAT (Currency)
        /// </summary>
        [UniVerseIndex(27)]
        [UniVerseConversion("MR44")]
        [UniVerseFieldName("PRICE.CUR")]
        public decimal? UnitPriceExVATCurrency { get; set; }

        /// <summary>
        /// Line Value Ex VAT (Currency)
        /// </summary>
        [UniVerseIndex(28)]
        [UniVerseConversion("MR22")]
        [UniVerseFieldName("VALUE.CUR")]
        public decimal? LineValueExVATCurrency { get; set; }

        /// <summary>
        /// VAT Value (Currency)
        /// </summary>
        [UniVerseIndex(30)]
        [UniVerseConversion("MR22")]
        [UniVerseFieldName("VATVAL.CUR")]
        public decimal? VATValueCurrency { get; set; }

        /// <summary>
        /// Order Line Pointer
        /// </summary>
        [UniVerseIndex(81)]
        [UniVerseFieldName("TOELINEP")]
        public string OrderLineID { get; set; }

        /// <summary>
        /// General Ledger Code
        /// </summary>
        [UniVerseIndex(136)]
        [UniVerseFieldName("LN.GLCODE.DES")]
        public string GeneralLedgerCode { get; set; }
    }
}
