using MertexerV2.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.Models.ExchangeRate;
using Newtonsoft.Json;
using MertexerV2.ProductFunctions;

namespace MertexerV2.Models.Product
{
    [UniVerseFileName("PLSUP")]
    public class Supplier : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public Supplier(string key) : base(key)
        {

        }

        public Supplier() : base()
        {
            InitialiseFromEmptyConstructor();
        }

        public override void InitialiseFromEmptyConstructor()
        {
            typeof(Supplier).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            SupplierFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Currency
        /// </summary>
        [UniVerseIndex(1,1)]
        [UniVerseFieldName("SUPNAME")]
        public string Name { get; set; }

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
        [UniVerseIndex(100)]
        [UniVerseFieldName("CURRENCY")]
        public string Currency { get; set; }

        [UniVerseForeignKey("Currency", "MertexerV2.Models.ExchangeRate.ExchangeRate")]
        public virtual ExchangeRate.ExchangeRate ExchangeRate { get; set; }
    }
}