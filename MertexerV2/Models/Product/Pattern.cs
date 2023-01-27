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
    [UniVerseFileName("STPATTERN.DES")]
    public class Pattern : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public Pattern(string key) : base(key)
        {

        }

        public Pattern() : base()
        {
            InitialiseFromEmptyConstructor();
        }

        public override void InitialiseFromEmptyConstructor()
        {
            typeof(Pattern).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            PatternFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Description
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("CODEDESC")]
        public string Name { get; set; }

        /// <summary>
        /// Sort Order
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("SORT.SEQUENCE")]
        public int SortOrder { get; set; }
    }
}
