using MertexerV2.GeneralFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.General
{
    [UniVerseFileName("STPRODCLASS8")]
    public class DeliveryTerms : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public DeliveryTerms(string key) : base(key)
        {
            Initialise();
        }

        public DeliveryTerms() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(DeliveryTerms).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            DeliveryTermsFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Name
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("CODEDESC")]
        public string Name { get; set; }
    }
}
