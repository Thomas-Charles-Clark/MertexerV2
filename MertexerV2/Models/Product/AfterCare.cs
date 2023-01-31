using MertexerV2.ProductFunctions;
using MertexerV2.Utilities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Product
{
    [UniVerseFileName("CLAFTCARE")]
    public class AfterCare : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public AfterCare(string key) : base(key)
        {
            Initialise();
        }

        public AfterCare() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(AfterCare).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            AfterCareFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Description
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseMultiValue]
        [UniVerseFieldName("DESC")]
        [JsonConverter(typeof(StringToArrayConverter<IEnumerable<string>>))]
        public IEnumerable<string> Descriptions { get; set; }
        
        /// <summary>
        /// Quick Description
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("QUICKDESC")]
        public string QuickDescription { get; set; }

        /// <summary>
        /// Image Name
        /// </summary>
        [UniVerseIndex(3)]
        [UniVerseFieldName("IMAGENAME")]
        public string ImageName { get; set; }

        /// <summary>
        /// Image Font (Must be in the range "01" to "40")
        /// </summary>
        [UniVerseIndex(4)]
        [UniVerseFieldName("IM.CHARSET")]
        public byte? ImageFont { get; set; }

        /// <summary>
        /// Image Scaling Factor
        /// </summary>
        [UniVerseIndex(5)]
        [UniVerseFieldName("IM.SCALEFACT")]
        public decimal? ImageScalingFactor { get; set; }

        /// <summary>
        /// Image Offset X
        /// </summary>
        [UniVerseIndex(6)]
        [UniVerseFieldName("IM.XOFF")]
        public decimal? OffsetX { get; set; }

        /// <summary>
        /// Image Offset Y
        /// </summary>
        [UniVerseIndex(7)]
        [UniVerseFieldName("IM.YOFF")]
        public decimal? OffsetY { get; set; }

        /// <summary>
        /// Image Character
        /// </summary>
        [UniVerseIndex(8)]
        [UniVerseFieldName("IM.CHAR")]
        public string ImageCharacter { get; set; }

        /// <summary>
        /// Used
        /// </summary>
        [UniVerseIndex(9)]
        [UniVerseFieldName("USED")]
        public bool Used { get; set; }
    }
}
