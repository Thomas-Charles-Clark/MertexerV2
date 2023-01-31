using MertexerV2.ProductFunctions;
using MertexerV2.Utilities;

using Newtonsoft.Json;

using System.Linq;

namespace MertexerV2.Models.Product
{
    [UniVerseFileName("STUK.FLAME")]
    public class UKFlameRetardancyImage : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public UKFlameRetardancyImage(string key) : base(key)
        {
            Initialise();
        }

        public UKFlameRetardancyImage() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(UKFlameRetardancyImage).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            UKFlameRetardancyImageFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Description
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("DESCRIPTION")]
        public string Description { get; set; }
        
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
