using MertexerV2.PricingFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Pricing
{
    [UniVerseFileName("SLDIVISIONS")]
    public class SalesDivision : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public SalesDivision(string key) : base(key)
        {
            Initialise();
        }

        public SalesDivision() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(SalesDivision).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            SalesDivisionFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("DIVISION.DESC.TDW")]
        public string Name { get; set; }
    }
}
