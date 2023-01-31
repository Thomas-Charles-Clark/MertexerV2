using MertexerV2.ProductFunctions;
using MertexerV2.Utilities;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.Models.Product
{
    [UniVerseFileName("STUSAGE")]
    public class Usage : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public Usage(string key) : base(key)
        {
            Initialise();
        }

        public Usage() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(Usage).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            UsageFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Description
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseMultiValue]
        [UniVerseFieldName("DESCRIPTION")]
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
    }
}
