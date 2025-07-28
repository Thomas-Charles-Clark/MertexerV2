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
    [UniVerseFileName("SLSEL3")]
    public class SalesArea : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public SalesArea(string key) : base(key)
        {
            Initialise();
        }

        public SalesArea() : base()
        {
            Initialise();
        }


        public override void Initialise()
        {
            typeof(SalesAgent).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
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