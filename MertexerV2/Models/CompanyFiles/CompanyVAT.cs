using MertexerV2.Utilities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Company
{
    [UniVerseFileName("CFVAT")]
    public class CompanyVAT : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public CompanyVAT(string key) : base(key)
        {
            Initialise();
        }

        public CompanyVAT() :base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(CompanyVAT).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("SORTCODE")]
        public byte SortCode { get; set; }

        [UniVerseIndex(2)]
        [UniVerseFieldName("VATREQD")]
        public string ReportIndicator { get; set; }

        [UniVerseIndex(3)]
        [UniVerseFieldName("VATRATE")]
        [UniVerseConversion("MR33")]
        public decimal VATRate { get; set; }

        [UniVerseIndex(4)]
        [UniVerseFieldName("ECVAT")]
        public string ECVAT { get; set; }

        [UniVerseIndex(5)]
        [UniVerseFieldName("TITLE")]
        public string Description { get; set; }

        [UniVerseIndex(7)]
        [UniVerseFieldName("SINVT")]
        [UniVerseConversion("MR22")]
        public decimal SalesInvoicesTax { get; set; }

        [UniVerseIndex(8)]
        [UniVerseFieldName("SCRST")]
        [UniVerseConversion("MR22")]
        public decimal SalesCreditSalesTax { get; set; }

        [UniVerseIndex(9)]
        [UniVerseFieldName("SCRNT")]
        [UniVerseConversion("MR22")]
        public decimal SalesCreditNotesTax { get; set; }

        [UniVerseIndex(10)]
        [UniVerseFieldName("SCSHT")]
        [UniVerseConversion("MR22")]
        public decimal SalesCashSalesTax { get; set; }

        [UniVerseIndex(11)]
        [UniVerseFieldName("SINVV")]
        [UniVerseConversion("MR22")]
        public decimal SalesInvoicesValue { get; set; }

        [UniVerseIndex(12)]
        [UniVerseFieldName("SCRSV")]
        [UniVerseConversion("MR22")]
        public decimal SalesCreditSalesValue { get; set; }

        [UniVerseIndex(13)]
        [UniVerseFieldName("SCRNV")]
        [UniVerseConversion("MR22")]
        public decimal SalesCreditNotesValue { get; set; }

        [UniVerseIndex(14)]
        [UniVerseFieldName("SCSHV")]
        [UniVerseConversion("MR22")]
        public decimal SalesCashSalesValue { get; set; }

        [UniVerseIndex(15)]
        [UniVerseFieldName("SQTTOT")]
        [UniVerseConversion("MR22")]
        public decimal SalesQuarterTaxTotal { get; set; }

        [UniVerseIndex(16)]
        [UniVerseFieldName("SQVTOT")]
        [UniVerseConversion("MR22")]
        public decimal SalesQuarterValueTotal { get; set; }

        [UniVerseIndex(17)]
        [UniVerseFieldName("SNINVT")]
        [UniVerseConversion("MR22")]
        public decimal SalesNextPeriodInvoicesTax { get; set; }

        [UniVerseIndex(18)]
        [UniVerseFieldName("SNCRST")]
        [UniVerseConversion("MR22")]
        public decimal SalesNextPeriodCreditSalesTax { get; set; }

        [UniVerseIndex(19)]
        [UniVerseFieldName("SNCRNT")]
        [UniVerseConversion("MR22")]
        public decimal SalesNextPeriodCreditNotesTax { get; set; }

        [UniVerseIndex(20)]
        [UniVerseFieldName("SNCSHT")]
        [UniVerseConversion("MR22")]
        public decimal SalesNextPeriodCashSalesTax { get; set; }

        [UniVerseIndex(21)]
        [UniVerseFieldName("SNINVV")]
        [UniVerseConversion("MR22")]
        public decimal SalesNextPeriodInvoicesValue { get; set; }

        [UniVerseIndex(22)]
        [UniVerseFieldName("SNCRSV")]
        [UniVerseConversion("MR22")]
        public decimal SalesNextPeriodCreditSalesValue { get; set; }

        [UniVerseIndex(23)]
        [UniVerseFieldName("SNCRNV")]
        [UniVerseConversion("MR22")]
        public decimal SalesNextPeriodCreditNotesValue { get; set; }

        [UniVerseIndex(24)]
        [UniVerseFieldName("SNCSHV")]
        [UniVerseConversion("MR22")]
        public decimal SalesNextPeriodCashSalesValue { get; set; }

        [UniVerseIndex(30)]
        [UniVerseFieldName("PINVT")]
        [UniVerseConversion("MR22")]
        public decimal PurchasesInvoicesTax { get; set; }

        [UniVerseIndex(31)]
        [UniVerseFieldName("PCRNT")]
        [UniVerseConversion("MR22")]
        public decimal PurchasesCreditNotesTax { get; set; }

        [UniVerseIndex(32)]
        [UniVerseFieldName("PPETT")]
        [UniVerseConversion("MR22")]
        public decimal PurchasesPettyCashTax { get; set; }

        [UniVerseIndex(33)]
        [UniVerseFieldName("POTHT")]
        [UniVerseConversion("MR22")]
        public decimal PurchasesOtherPaymentsTax { get; set; }

        [UniVerseIndex(34)]
        [UniVerseFieldName("PINVV")]
        [UniVerseConversion("MR22")]
        public decimal PurchasesInvoicesValue { get; set; }

        [UniVerseIndex(35)]
        [UniVerseFieldName("PCRNV")]
        [UniVerseConversion("MR22")]
        public decimal PurchasesCreditNotesValue { get; set; }

        [UniVerseIndex(36)]
        [UniVerseFieldName("PPETV")]
        [UniVerseConversion("MR22")]
        public decimal PurchasesPettyCashValue { get; set; }

        [UniVerseIndex(37)]
        [UniVerseFieldName("POTHT")]
        [UniVerseConversion("MR22")]
        public decimal PurchasesOtherPaymentsValue { get; set; }

        [UniVerseIndex(38)]
        [UniVerseFieldName("PQTTOT")]
        [UniVerseConversion("MR22")]
        public decimal PurchasesQuarterTaxTotal { get; set; }

        [UniVerseIndex(39)]
        [UniVerseFieldName("PQVTOT")]
        [UniVerseConversion("MR22")]
        public decimal PurchasesQuarterValueTotal { get; set; }
        
        [UniVerseIndex(40)]
        [UniVerseFieldName("PNINVT")]
        [UniVerseConversion("MR22")]
        public decimal PurchasesNextPeriodInvoicesTax { get; set; }

        [UniVerseIndex(41)]
        [UniVerseFieldName("PNCRNT")]
        [UniVerseConversion("MR22")]
        public decimal PurchasesNextPeriodCreditNotesTax { get; set; }

        [UniVerseIndex(42)]
        [UniVerseFieldName("PNPETT")]
        [UniVerseConversion("MR22")]
        public decimal PurchasesNextPeriodPettyCashTax { get; set; }

        [UniVerseIndex(43)]
        [UniVerseFieldName("PNOTHT")]
        [UniVerseConversion("MR22")]
        public decimal PurchasesNextPeriodOtherPaymentsTax { get; set; }

        [UniVerseIndex(44)]
        [UniVerseFieldName("PNINVV")]
        [UniVerseConversion("MR22")]
        public decimal PurchasesNextPeriodInvoicesValue { get; set; }

        [UniVerseIndex(45)]
        [UniVerseFieldName("PNCRNV")]
        [UniVerseConversion("MR22")]
        public decimal PurchasesNextPeriodCreditNotesValue { get; set; }

        [UniVerseIndex(46)]
        [UniVerseFieldName("PNPETV")]
        [UniVerseConversion("MR22")]
        public decimal PurchasesNextPeriodPettyCashValue { get; set; }

        [UniVerseIndex(47)]
        [UniVerseFieldName("PNOTHV")]
        [UniVerseConversion("MR22")]
        public decimal PurchasesNextPeriodOtherPaymentsValue { get; set; }
    }
}
