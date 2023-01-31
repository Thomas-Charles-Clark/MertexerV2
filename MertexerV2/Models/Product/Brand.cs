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
    [UniVerseFileName("SL.SALES.DIV.DES")]
    public class Brand : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public Brand(string key) : base(key)
        {
            Initialise();
        }

        public Brand() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(Brand).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            BrandFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Name
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("SALES.DIV.DESC.DES")]
        public string Name { get; set; }

        /// <summary>
        /// Sort Order
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("SORT.SEQUENCE")]
        public byte? SortOrder { get; set; }
    }
}
