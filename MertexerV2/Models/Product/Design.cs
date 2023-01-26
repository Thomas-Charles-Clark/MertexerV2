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
    [UniVerseFileName("STDESIGN.DES")]
    public class Design : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public Design(string key) : base(key)
        {

        }

        public Design() : base()
        {
            InitialiseFromEmptyConstructor();
        }

        public override void InitialiseFromEmptyConstructor()
        {
            typeof(Design).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            DesignFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Description
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("CODEDESC")]
        public string Description { get; set; }

        /// <summary>
        /// Sort Order
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("SORT.SEQUENCE")]
        public int? SortOrder { get; set; }
    }
}
