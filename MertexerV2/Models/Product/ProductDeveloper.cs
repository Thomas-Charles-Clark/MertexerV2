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
    [UniVerseFileName("STPRODCLASS5")]
    public class ProductDeveloper : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public ProductDeveloper(string key) : base(key)
        {
            Initialise();
        }

        public ProductDeveloper() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(ProductDeveloper).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            ProductDeveloperFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Name
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("CODEDESC")]
        public string Name { get; set; }
    }
}
