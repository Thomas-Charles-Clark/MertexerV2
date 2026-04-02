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
    [UniVerseFileName("SLCUST")]
    public class TradeAccountCodeStub : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public TradeAccountCodeStub(string key) : base(key)
        {
            Initialise();
        }

        public TradeAccountCodeStub() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(TradeAccountCodeStub).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }
    }
}
