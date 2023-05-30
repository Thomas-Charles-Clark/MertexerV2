using MertexerV2.StockFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Stock
{
    [UniVerseFileName("STLOCATION")]
    public class StockLocation : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public StockLocation(string key) : base(key)
        {
            Initialise();
        }

        public StockLocation() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(StockLocation).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            StockLocationFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("TITLE")]
        public string Name { get; set; }
    }
}
