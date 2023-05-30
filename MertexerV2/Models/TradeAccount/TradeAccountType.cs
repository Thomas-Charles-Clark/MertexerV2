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
    [UniVerseFileName("SLTYPE")]
    public class TradeAccountType : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public TradeAccountType(string key) : base(key)
        {
            Initialise();
        }

        public TradeAccountType() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(TradeAccountType).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            TradeAccountTypeFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("TITLE")]
        public string Name { get; set; }
    }

}
