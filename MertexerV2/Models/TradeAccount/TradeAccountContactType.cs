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
    public class TradeAccountContactTypeCode
    {
        public string Code { get; set; }
    }

    [UniVerseFileName("SLPOS")]
    public class TradeAccountContactType : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public TradeAccountContactType(string key) : base(key)
        {
            Initialise();
        }

        public TradeAccountContactType() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(TradeAccountContactType).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            TradeAccountContactTypeFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("TITLE")]
        public string Name { get; set; }
    }
}
