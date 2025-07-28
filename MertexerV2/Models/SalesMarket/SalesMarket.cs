using MertexerV2.GeneralFunctions;
using MertexerV2.ProductFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.General
{
    [UniVerseFileName("CLMARKTYPE")]
    public class SalesMarket : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public SalesMarket(string key) : base(key)
        {
            Initialise();
        }

        public SalesMarket() : base()
        {
            Initialise();
        }


        public override void Initialise()
        {
            typeof(SalesMarket).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        //public void Upsert(string uniVerseAccount = "MERTEX")
        //{
        //    SalesMarketFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        //}

        /// <summary>
        /// Description
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("DESC")]
        public string Name { get; set; }
    }
}