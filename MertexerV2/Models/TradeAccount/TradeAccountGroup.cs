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
    public class TradeAccountGroup : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public TradeAccountGroup(string key) : base(key)
        {
            Initialise();
        }

        public TradeAccountGroup() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(TradeAccountGroup).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            TradeAccountGroupFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("DESCRIPTION")]
        public string Name { get; set; }
    }
}
