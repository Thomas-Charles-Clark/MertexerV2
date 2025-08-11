using MertexerV2.Utilities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Company
{
    [UniVerseFileName("CFSTATS")]
    public class CompanyStats : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public CompanyStats(string key) : base(key)
        {
            Initialise();
        }

        public CompanyStats() :base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(CompanyStats).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("SLBFWD")]
        [UniVerseConversion("MR22")]
        public decimal SalesLedgerBroughtForward { get; set; }

        [UniVerseIndex(2)]
        [UniVerseFieldName("SLCRS")]
        [UniVerseConversion("MR22")]
        public decimal SalesLedgerCreditSales { get; set; }

        [UniVerseIndex(3)]
        [UniVerseFieldName("SLCRN")]
        [UniVerseConversion("MR22")]
        public decimal SalesLedgerCreditNotes { get; set; }

        [UniVerseIndex(4)]
        [UniVerseFieldName("SLCSH")]
        [UniVerseConversion("MR22")]
        public decimal SalesLedgerCashReceipts { get; set; }

        [UniVerseIndex(5)]
        [UniVerseFieldName("SLADJ")]
        [UniVerseConversion("MR22")]
        public decimal SalesLedgerAdjustments { get; set; }

        [UniVerseIndex(6)]
        [UniVerseFieldName("STCSH")]
        [UniVerseConversion("MR22")]
        public decimal SalesLedgerTotalCashPayments { get; set; }

        [UniVerseIndex(10)]
        [UniVerseFieldName("PLBFWD")]
        [UniVerseConversion("MR22")]
        public decimal PurchaseLedgerBroughtForward { get; set; }

        [UniVerseIndex(11)]
        [UniVerseFieldName("PLINV")]
        [UniVerseConversion("MR22")]
        public decimal PurchaseLedgerInvoices { get; set; }

        [UniVerseIndex(12)]
        [UniVerseFieldName("PLCRN")]
        [UniVerseConversion("MR22")]
        public decimal PurchaseLedgerCreditNotes { get; set; }

        [UniVerseIndex(13)]
        [UniVerseFieldName("PLPAY")]
        [UniVerseConversion("MR22")]
        public decimal PurchaseLedgerPayments { get; set; }

        [UniVerseIndex(14)]
        [UniVerseFieldName("PLADJ")]
        [UniVerseConversion("MR22")]
        public decimal PurchaseLedgerAdjustments { get; set; }

        [UniVerseIndex(15)]
        [UniVerseFieldName("PLPETTY")]
        [UniVerseConversion("MR22")]
        public decimal PurchaseLedgerPettyCash { get; set; }

        [UniVerseIndex(16)]
        [UniVerseFieldName("PLOTHER")]
        [UniVerseConversion("MR22")]
        public decimal PurchaseLedgerOtherPayments { get; set; }

        [UniVerseIndex(21)]
        [UniVerseFieldName("NPLINV")]
        [UniVerseConversion("MR22")]
        public decimal NextPurchaseLedgerInvoices { get; set; }

        [UniVerseIndex(22)]
        [UniVerseFieldName("NPLCRN")]
        [UniVerseConversion("MR22")]
        public decimal NextPurchaseLedgerCreditNotes { get; set; }

        [UniVerseIndex(23)]
        [UniVerseFieldName("NPLPAY")]
        [UniVerseConversion("MR22")]
        public decimal NextPurchaseLedgerPayments { get; set; }

        [UniVerseIndex(24)]
        [UniVerseFieldName("NPLADJ")]
        [UniVerseConversion("MR22")]
        public decimal NextPurchaseLedgerAdjustments { get; set; }

        [UniVerseIndex(25)]
        [UniVerseFieldName("NPLPETTY")]
        [UniVerseConversion("MR22")]
        public decimal NextPurchaseLedgerPettyCash { get; set; }

        [UniVerseIndex(26)]
        [UniVerseFieldName("NPLOTHER")]
        [UniVerseConversion("MR22")]
        public decimal NextPurchaseLedgerOtherPayments { get; set; }

        public decimal SalesLedgerCarriedForward
        {
            get
            {
                return SalesLedgerBroughtForward +
                       SalesLedgerCreditSales +
                       SalesLedgerCreditNotes +
                       SalesLedgerCashReceipts +
                       SalesLedgerAdjustments;
            }
        }

        public decimal PurchaseLedgerCarriedForward
        {
            get
            {
                return PurchaseLedgerBroughtForward +
                       PurchaseLedgerInvoices +
                       PurchaseLedgerCreditNotes +
                       PurchaseLedgerPayments +
                       PurchaseLedgerAdjustments;
            }
        }

        public decimal PurchaseLedgerNextPeriod
        {
            get 
            {
                return NextPurchaseLedgerInvoices +
                       NextPurchaseLedgerCreditNotes +
                       NextPurchaseLedgerPayments +
                       NextPurchaseLedgerAdjustments;
            }
        }
    }
}
