using MertexerV2.Models.Interfaces;
using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Pricing
{
    [UniVerseFileName("POCOSTS")]
    public class POCost : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public POCost(string key) : base(key)
        {

        }

        public POCost() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(POCost).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        /// <summary>
        /// 
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("QUALITY")]
        public string Quality { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("COUNTRY")]
        public string Country { get; set; }

        /// <summary>
        /// Method of Transport
        /// </summary>
        [UniVerseIndex(3)]
        [UniVerseFieldName("MOD")]
        public string MethodOfTransport { get; set; }

        /// <summary>
        /// Duty Percentage
        /// </summary>
        [UniVerseIndex(4, 1)]
        [UniVerseConversion("MR33")]
        [UniVerseFieldName("DUTY%")]
        public decimal? DutyPercentage { get; set; }

        /// <summary>
        /// Inland Charges
        /// </summary>
        [UniVerseIndex(4, 2)]
        [UniVerseConversion("MR33")]
        [UniVerseFieldName("INLAND%")]
        public decimal? InlandCharges { get; set; }

        /// <summary>
        /// Packaging Charges
        /// </summary>
        [UniVerseIndex(4, 3)]
        [UniVerseConversion("MR33")]
        [UniVerseFieldName("PACKAGING%")]
        public decimal? PackagingCharges { get; set; }

        /// <summary>
        /// Freight Rate
        /// </summary>
        [UniVerseIndex(5, 1)]
        [UniVerseConversion("MR22")]
        [UniVerseFieldName("FREIGHTR")]
        public decimal? FreightCharges { get; set; }

        /// <summary>
        /// Quota Levy
        /// </summary>
        [UniVerseIndex(5, 2)]
        [UniVerseConversion("MR22")]
        [UniVerseFieldName("QUOTAR")]
        public decimal? QuotaLevy { get; set; }

        /// <summary>
        /// Supplier Reference
        /// </summary>
        [UniVerseIndex(6)]
        [UniVerseFieldName("SUPREF")]
        public string SupplierReference { get; set; }
    }
}
