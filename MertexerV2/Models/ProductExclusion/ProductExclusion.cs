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
    [UniVerseFileName("STPRODEXCLUDE")]
    public class ProductExclusion : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public ProductExclusion(string key) : base(key)
        {
            Initialise();
        }

        public ProductExclusion() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(ProductExclusion).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            ProductExclusionFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Name 
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseMultiValue]
        [UniVerseFieldName("EXCLUDE.LIST")]
        public IEnumerable<string> ProductsExcluded { get; set; }

        /// <summary>
        /// Name 
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("EXCLUSION.DESC")]
        public string Description { get; set; }
    }

    public class ProductExclusionDerivation
    {
        public string Key { get; set; }
        public string ProductCode { get; set; }
    }
}
