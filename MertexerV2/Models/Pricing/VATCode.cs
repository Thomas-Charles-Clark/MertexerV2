using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Pricing
{
    [UniVerseFileName("CFVAT")]
    public class VATCode : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public VATCode(string key) : base(key)
        {

        }

        public VATCode() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(VATCode).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        /// <summary>
        /// Sort Code
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("SORTCODE")]
        public byte? SortOrder { get; set; }

        /// <summary>
        /// Report Indicator
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("VATREQD")]
        public string ReportIndicator { get; set; }

        /// <summary>
        /// VAT Rate
        /// </summary>
        [UniVerseIndex(3)]
        [UniVerseConversion("MR33")]
        [UniVerseFieldName("VATRATE")]

        public decimal? Rate { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [UniVerseIndex(5)]
        [UniVerseFieldName("TITLE")]
        public string Name { get; set; }
    }
}
