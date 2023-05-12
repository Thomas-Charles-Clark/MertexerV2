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
    [UniVerseFileName("STPRODCLASS8")]
    public class ShippingTerms : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public ShippingTerms(string key) : base(key)
        {
            Initialise();
        }

        public ShippingTerms() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(ShippingTerms).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            ShippingTermsFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Name
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("CODEDESC")]
        public string Name { get; set; }
    }
}
