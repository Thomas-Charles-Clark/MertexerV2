using MertexerV2.Utilities;

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using MertexerV2.Models.PurchaseLedger;

namespace MertexerV2.Models.Supplier
{
    [UniVerseFileName("PLSUP")]
    public class Supplier : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public Supplier(string key) : base(key)
        {
            Initialise();
        }

        public Supplier() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(Supplier).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            SupplierFunctions.SupplierFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Currency
        /// </summary>
        [UniVerseIndex(1,1)]
        [UniVerseFieldName("SUPNAME")]
        public string Name { get; set; }

        /// <summary>
        /// AddressLine1
        /// </summary>
        [UniVerseIndex(1, 2)]
        [UniVerseFieldName("SUPADD1")]
        public string AddressLine1 { get; set; }

        /// <summary>
        /// AddressLine2
        /// </summary>
        [UniVerseIndex(1, 3)]
        [UniVerseFieldName("SUPADD2")]
        public string AddressLine2 { get; set; }

        /// <summary>
        /// Town
        /// </summary>
        [UniVerseIndex(1, 4)]
        [UniVerseFieldName("SUPADD3")]
        public string Town { get; set; }

        /// <summary>
        /// County
        /// </summary>
        [UniVerseIndex(1, 5)]
        [UniVerseFieldName("SUPADD4")]
        public string County { get; set; }

        /// <summary>
        /// Postcode
        /// </summary>
        [UniVerseIndex(1, 6)]
        [UniVerseFieldName("Postcode")]
        public string Postcode { get; set; }

        /// <summary>
        /// Postcode
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("SORTNAME")]
        public string SortName { get; set; }

        /// <summary>
        /// Balance
        /// </summary>
        [UniVerseIndex(15)]
        [UniVerseConversion("MR22")]
        [UniVerseFieldName("BALANCE")]
        public decimal Balance { get; set; }

        [UniVerseIndex(18)]
        [UniVerseFieldName("ACBAL")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> AccountPeriodBalances { get; set; }

        [UniVerseIndex(18,1)]
        [UniVerseFieldName("NEXT")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public decimal NextPeriodBalance { get; set; }

        [UniVerseIndex(18, 2)]
        [UniVerseFieldName("CURR")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public decimal ThisPeriodBalance { get; set; }

        /// <summary>
        /// Documents
        /// </summary>
        [UniVerseIndex(19)]
        [UniVerseFieldName("TLINK")]
        [UniVerseMultiValue]
        public IEnumerable<string> DocumentReferences { get; set; }

        [UniVerseIndex(21)]
        [UniVerseFieldName("PURCH")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> Purchases { get; set; }

        [UniVerseIndex(21, 1)]
        [UniVerseFieldName("PNEXT")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public decimal PurchasesNextPeriod { get; set; }

        [UniVerseIndex(21, 2)]
        [UniVerseFieldName("PCURR")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public decimal PurchasesThisPeriod { get; set; }

        [UniVerseIndex(21, 3)]
        [UniVerseFieldName("PYTD")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public decimal PurchasesYearToDate { get; set; }

        [UniVerseIndex(22)]
        [UniVerseFieldName("INACT")]
        public byte PeriodsInactive { get; set; }

        /// <summary>
        /// CountryCode
        /// </summary>
        [UniVerseIndex(41)]
        [UniVerseFieldName("CountryCode")]
        public string CountryCode { get; set; }

        /// <summary>
        /// Duty Percentage
        /// </summary>
        [UniVerseIndex(80)]
        [UniVerseConversion("MR11")]
        [UniVerseFieldName("DUTY.PERC")]
        public decimal? DutyPercentage { get; set; }

        /// <summary>
        /// Inland Charges Percentage
        /// </summary>
        [UniVerseIndex(81)]
        [UniVerseConversion("MR11")]
        [UniVerseFieldName("ANTIDMP.PERC")]
        public decimal? InlandChargesPercentage { get; set; }

        /// <summary>
        /// Miscellaneous Percentage
        /// </summary>
        [UniVerseIndex(82)]
        [UniVerseConversion("MR11")]
        [UniVerseFieldName("MISC.PERC")]
        public decimal? MiscellaneousPercentage { get; set; }

        /// <summary>
        /// Import Charge per Unit
        /// </summary>
        [UniVerseIndex(83)]
        [UniVerseConversion("MR24")]
        [UniVerseFieldName("IMP.CHARGE")]
        public decimal? ImportChargePerUnit { get; set; }

        /// <summary>
        /// Miscellaneous Charge per Unit
        /// </summary>
        [UniVerseIndex(84)]
        [UniVerseConversion("MR24")]
        [UniVerseFieldName("MISC.CHARGE")]
        public decimal? MiscellaneousCharge { get; set; }

        /// <summary>
        /// Currency
        /// </summary>
        [UniVerseIndex(95)]
        [UniVerseFieldName("EMAIL")]
        public string Email { get; set; }

        /// <summary>
        /// Currency
        /// </summary>
        [UniVerseIndex(100)]
        [UniVerseFieldName("CURRENCY")]
        public string Currency { get; set; }

        /// <summary>
        /// Currency
        /// </summary>
        [UniVerseIndex(124)]
        [UniVerseFieldName("STD.TELEPHONE")]
        public string PhoneNumber { get; set; }

        [UniVerseIndex(11)]
        [UniVerseFieldName("SEL1")]
        public string SEL1 { get; set; }

        public string DSEL1
        {
            get
            {
                return PLSEL1?.Description;
            }
        }

        [UniVerseIndex(12)]
        [UniVerseFieldName("SEL2")]
        public string SEL2 { get; set; }

        public string DSEL2
        {
            get
            {
                return PLSEL2?.Description;
            }
        }

        [UniVerseIndex(13)]
        [UniVerseFieldName("SEL3")]
        public string SEL3 { get; set; }

        public string DSEL3
        {
            get
            {
                return PLSEL3?.Description;
            }
        }

        [UniVerseIndex(20)]
        [UniVerseFieldName("TERMS")]
        public string PaymentTermsCode { get; set; }


        [UniVerseIndex(58)]
        [UniVerseFieldName("VATNORM")]
        public string VATCode { get; set; }

        [UniVerseIndex(132)]
        [UniVerseFieldName("PERM.FAX")]
        public bool AccountClosed { get; set; }

        [UniVerseForeignKey("SEL1", "MertexerV2.Models.Supplier.PLSEL1")]
        public virtual SupplierType PLSEL1 { get; set; }

        [UniVerseForeignKey("SEL2", "MertexerV2.Models.Supplier.PLSEL2")]
        public virtual PLSEL2 PLSEL2 { get; set; }

        [UniVerseForeignKey("SEL3", "MertexerV2.Models.Supplier.PLSEL3")]
        public virtual PLSEL3 PLSEL3 { get; set; }

        [UniVerseForeignKey("Currency", "MertexerV2.Models.ExchangeRate.ExchangeRate")]
        public virtual ExchangeRate.ExchangeRate ExchangeRate { get; set; }

        [UniVerseForeignKey("DocumentReferences", "MertexerV2.Models.PurchaseLedger.PurchaseLedgerTransfer")]
        public virtual IEnumerable<PurchaseLedgerTransaction> PurchaseLedgerTransferRecords { get; set; } = new List<PurchaseLedgerTransaction>();

        [UniVerseForeignKey("DocumentReferences", "MertexerV2.Models.PurchaseLedger.PurchaseLedgerTransfer")]
        public virtual IEnumerable<PurchaseLedgerTransaction> PurchaseLedgerTransferRecordsCurrency { get; set; } = new List<PurchaseLedgerTransaction>();

        public virtual IEnumerable<PurchaseLedgerInvoiceRegister> PurchaseLedgerRegisterEntries_Invoice { get; set; } = new List<PurchaseLedgerInvoiceRegister>();

        public virtual IEnumerable<PurchaseLedgerCreditRegister> PurchaseLedgerRegisterEntries_Credit { get; set; } = new List<PurchaseLedgerCreditRegister>();
    }
}