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
    [UniVerseFileName("STFLAME.DES")]
    public class FlameRetardant : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public FlameRetardant(string key) : base(key)
        {
            Initialise();
        }

        public FlameRetardant() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(FlameRetardant).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            FlameRetardantFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Name
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("TITLE")]
        public string Name { get; set; }
    }
}
