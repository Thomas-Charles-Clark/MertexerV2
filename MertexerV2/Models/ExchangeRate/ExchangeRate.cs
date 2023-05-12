using MertexerV2.AccountFunctions;
using MertexerV2.GeneralFunctions;
using MertexerV2.Utilities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.ExchangeRate
{
    [UniVerseFileName("XRATE")]
    public class ExchangeRate : UniVerseSerializationBase<string>
    {
        public ExchangeRate(string key) : base(key)
        {

        }

        public ExchangeRate() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(ExchangeRate).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            ExchangeRateFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Rate
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseConversion("MR66")]
        [UniVerseFieldName("RATE")]
        public decimal Rate { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [UniVerseIndex(12)]
        [UniVerseFieldName("WEB.DESC")]
        public string Description { get; set; }

    }
}
