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
    [UniVerseFileName("STPRODSTATUS")]
    public class ProductStatus : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public ProductStatus(string key) : base(key)
        {
            Initialise();
        }

        public ProductStatus() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(ProductStatus).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            ProductStatusFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("DESCRIPTION")]
        public string Name { get; set; }
    }
}
