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
    [UniVerseFileName("STPRODCLASS6")]
    public class MarketRestriction : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public MarketRestriction(string key) : base(key)
        {
            Initialise();
        }

        public MarketRestriction() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(MarketRestriction).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            MarketRestrictionFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Name
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("CODEDESC")]
        public string Name { get; set; }
    }
}
