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
    [UniVerseFileName("ORTRANSPORT")]
    public class Transport : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public Transport(string key) : base(key)
        {
            Initialise();
        }

        public Transport() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(Transport).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            TransportFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Name
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("NAME")]
        public string Name { get; set; }
    }
}
