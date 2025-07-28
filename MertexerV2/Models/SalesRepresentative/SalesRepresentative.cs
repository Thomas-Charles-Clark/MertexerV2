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
    [UniVerseFileName("SLSEL1")]
    public class SalesRepresentative : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public SalesRepresentative(string key) : base(key)
        {
            Initialise();
        }

        public SalesRepresentative() : base()
        {
            Initialise();
        }


        public override void Initialise()
        {
            typeof(SalesAgent).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        //public void Upsert(string uniVerseAccount = "MERTEX")
        //{
        //    SalesRepresentativeFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        //}

        /// <summary>
        /// Description
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("TITLE")]
        public string Name { get; set; }
    }
}