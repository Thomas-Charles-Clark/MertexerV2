using MertexerV2.TradeAccountFunctions;
using MertexerV2.Utilities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models
{
    [UniVerseFileName("SLCUSCLASS")]
    public class TradeAccountClass : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public TradeAccountClass(string key) : base(key)
        {
            Initialise();
        }

        public TradeAccountClass() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(TradeAccountClass).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            TradeAccountClassFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("DESCRIPTION")]
        public string Name { get; set; }
    }
}
