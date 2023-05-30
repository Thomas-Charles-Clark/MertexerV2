using MertexerV2.PricingFunctions;
using MertexerV2.TradeAccountFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Pricing
{
    [UniVerseFileName("OEPROMOTIONS.DES")]
    public class FreeOfChargeAccountingCode : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public FreeOfChargeAccountingCode(string key) : base(key)
        {
            Initialise();
        }

        public FreeOfChargeAccountingCode() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(FreeOfChargeAccountingCode).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            FreeOfChargeAccountingCodeFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("PROMOTION.DESC")]
        public string Name { get; set; }
    }
}
