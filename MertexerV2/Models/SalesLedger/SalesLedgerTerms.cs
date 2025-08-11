using System.Collections.Generic;
using System.Linq;

using MertexerV2.Utilities;

using Newtonsoft.Json;

namespace MertexerV2.Models.SalesLedger
{
    [UniVerseFileName("SLTERMS")]
    public class SalesLedgerTerms : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public SalesLedgerTerms(string key) : base(key)
        {
            Initialise();
        }

        public SalesLedgerTerms() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(SalesLedgerTerms).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        /// <summary>
        /// Description
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("DISC")]
        [UniVerseConversion("MR33")]
        public decimal? DiscountPercentage { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("DAYS")]
        public byte? Days { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [UniVerseIndex(3)]
        [UniVerseFieldName("PRINT")]
        public bool Print { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [UniVerseIndex(4)]
        [UniVerseFieldName("TITLE")]
        public string Title { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [UniVerseIndex(5)]
        [UniVerseFieldName("DUEDAYS")]
        public byte? DueDays { get; set; }

        [UniVerseIndex(6)]
        [UniVerseFieldName("MONTHS")]
        public byte? DueMonths { get; set; }

        [UniVerseIndex(7)]
        [UniVerseFieldName("FIXEDDAY")]
        public byte? FixedDay { get; set; }

        [UniVerseIndex(8)]
        [UniVerseFieldName("SMONTHS")]
        public byte? SettlementMonths { get; set; }

        [UniVerseIndex(9)]
        [UniVerseFieldName("SFIXEDDAY")]
        public byte? SettlementFixedDay { get; set; }

        [UniVerseIndex(10)]
        [UniVerseFieldName("PDTITLE")]
        public string PaymentTitle { get; set; }

        [UniVerseIndex(11)]
        [UniVerseFieldName("CS.PERC")]
        [UniVerseConversion("MR33")]
        public decimal? CreditSurchargePercent { get; set; }

        [UniVerseIndex(12)]
        [UniVerseFieldName("CS.DAYS")]
        public byte? CreditSurchargeDays { get; set; }

        [UniVerseIndex(13)]
        [UniVerseFieldName("CS.MONTHS")]
        public byte? CreditSurchargeMonths { get; set; }

        [UniVerseIndex(14)]
        [UniVerseFieldName("CS.FIXEDDAY")]
        public byte? CreditSurchargeFixedDay { get; set; }

        [UniVerseIndex(15)]
        [UniVerseFieldName("CS.TITLE")]
        public string CreditSurchargeTitle { get; set; }

        [UniVerseIndex(25)]
        [UniVerseFieldName("INST.DUEDAYS.DES")]
        public IEnumerable<int> InstallmentDueDays { get; set; }

        [UniVerseIndex(25, 1)]
        [UniVerseFieldName("INST.DUEDAYS.1.DES")]
        public byte? NextInstallmentDueDay { get; set; }

        [UniVerseIndex(25, 2)]
        [UniVerseFieldName("INST.DUEDAYS.2.DES")]
        public byte? SecondInstallmentDueDay { get; set; }

        [UniVerseIndex(25, 3)]
        [UniVerseFieldName("INST.DUEDAYS.3.DES")]
        public byte? ThirdInstallmentDueDay { get; set; }

        [UniVerseIndex(26)]
        [UniVerseFieldName("INST.MONTHS.DES")]
        public IEnumerable<int> InstallmentMonths { get; set; }

        [UniVerseIndex(26, 1)]
        [UniVerseFieldName("INST.MONTHS.1.DES")]
        public byte? NextInstallmentDueMonth { get; set; }

        [UniVerseIndex(26, 2)]
        [UniVerseFieldName("INST.MONTHS.2.DES")]
        public byte? SecondInstallmentDueMonth { get; set; }

        [UniVerseIndex(26, 3)]
        [UniVerseFieldName("INST.MONTHS.3.DES")]
        public byte? ThirdInstallmentDueMonth { get; set; }

        [UniVerseIndex(27)]
        [UniVerseFieldName("INST.FIXEDDAY.DES")]
        public IEnumerable<int> InstallmentFixedDays { get; set; }

        [UniVerseIndex(27, 1)]
        [UniVerseFieldName("INST.FIXEDDAY.1.DES")]
        public byte? NextInstallmentDueFixedDay { get; set; }

        [UniVerseIndex(27, 2)]
        [UniVerseFieldName("INST.FIXEDDAY.2.DES")]
        public byte? SecondInstallmentDueFixedDay { get; set; }

        [UniVerseIndex(27, 3)]
        [UniVerseFieldName("INST.FIXEDDAY.3.DES")]
        public byte? ThirdInstallmentDueFixedDay { get; set; }

        [UniVerseIndex(28)]
        [UniVerseFieldName("INST.PCNT.DES")]
        [UniVerseConversion("MR55")]
        public IEnumerable<decimal> InstallmentPercentages { get; set; }

        [UniVerseIndex(28, 1)]
        [UniVerseFieldName("INST.PCNT.1.DES")]
        [UniVerseConversion("MR55")]
        public decimal? NextInstallmentBalancePercentage { get; set; }

        [UniVerseIndex(28, 2)]
        [UniVerseFieldName("INST.PCNT.2.DES")]
        [UniVerseConversion("MR55")]
        public decimal? SecondInstallmentBalancePercentage { get; set; }

        [UniVerseIndex(28, 3)]
        [UniVerseFieldName("INST.PCNT.3.DES")]
        [UniVerseConversion("MR55")]
        public decimal? ThirdInstallmentBalancePercentage { get; set; }

        [UniVerseIndex(29)]
        [UniVerseFieldName("DDIND.DES")]
        public bool DirectDebit { get; set; }

        [UniVerseIndex(30)]
        [UniVerseFieldName("AUTODISC")]
        public bool AutoDiscountTaking { get; set; }

        [UniVerseIndex(31)]
        [UniVerseFieldName("DISCONTOTAL")]
        public bool DiscountOnTotalValue { get; set; }
    }
}
