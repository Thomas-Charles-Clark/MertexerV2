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
    public class FlameRetardantDescription : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public FlameRetardantDescription(string key) : base(key)
        {
            Initialise();
        }

        public FlameRetardantDescription() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(FlameRetardantDescription).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            FlameRetardantDescriptionFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Name
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("TITLE")]
        public string Name { get; set; }
    }
}
