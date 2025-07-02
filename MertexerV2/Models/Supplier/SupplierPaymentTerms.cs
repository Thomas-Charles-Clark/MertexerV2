using System.Linq;

using MertexerV2.Utilities;
using Newtonsoft.Json;

namespace MertexerV2.Models.Supplier
{

    [UniVerseFileName("PLTERMS")]
    public class SupplierPaymentTerms : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public SupplierPaymentTerms(string key) : base(key)
        {
            Initialise();
        }

        public SupplierPaymentTerms() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(SupplierPaymentTerms).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("DISC")]
        [UniVerseConversion("MR33")]
        public decimal? DiscountPercentage { get; set; }

        [UniVerseIndex(2)]
        [UniVerseFieldName("DAYS")]
        [UniVerseConversion("MR33")]
        public byte? Days { get; set; }

        [UniVerseIndex(3)]
        [UniVerseFieldName("PRINT")]
        public bool Print { get; set; }

        [UniVerseIndex(4)]
        [UniVerseFieldName("TITLE")]
        public string Title { get; set; }

        [UniVerseIndex(5)]

        [UniVerseFieldName("DUEDAYS")]
        public byte? DueDays { get; set; }

        [UniVerseIndex(6)]
        [UniVerseFieldName("MONTHS")]
        public byte? Months { get; set; }

        [UniVerseIndex(7)]
        [UniVerseFieldName("FIXEDDAYS")]
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
    }
}
