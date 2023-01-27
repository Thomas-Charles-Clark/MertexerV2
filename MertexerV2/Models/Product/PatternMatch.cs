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
    [UniVerseFileName("STPATTERN")]
    public class PatternMatch : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public PatternMatch(string key) : base(key)
        {

        }

        public PatternMatch() : base()
        {
            InitialiseFromEmptyConstructor();
        }

        public override void InitialiseFromEmptyConstructor()
        {
            typeof(PatternMatch).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            PatternMatchFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Description
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("DESCRIPTION")]
        public string Name { get; set; }

        /// <summary>
        /// Image Name
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("PATT.FULL")]
        public string Full { get; set; }

        /// <summary>
        /// Image Font (Must be in the range "01" to "40")
        /// </summary>
        [UniVerseIndex(3)]
        [UniVerseFieldName("PATT.HALF")]
        public string Half { get; set; }

        /// <summary>
        /// Image Scaling Factor
        /// </summary>
        [UniVerseIndex(4)]
        [UniVerseFieldName("PATT.STRAIGHT")]
        public string Straight { get; set; }

        /// <summary>
        /// Image Offset X
        /// </summary>
        [UniVerseIndex(5)]
        [UniVerseFieldName("PATT.MATCH")]
        public string Match { get; set; }

        /// <summary>
        /// Image Offset Y
        /// </summary>
        [UniVerseIndex(6)]
        [UniVerseFieldName("PATT.RANDOM")]
        public string Random { get; set; }
    }
}
