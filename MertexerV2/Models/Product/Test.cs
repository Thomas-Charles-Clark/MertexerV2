using MertexerV2.ProductFunctions;
using MertexerV2.Utilities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Product
{
    [UniVerseFileName("STMISC1")]
    public class Test : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public Test(string key) : base(key)
        {

        }

        public Test() : base()
        {
            InitialiseFromEmptyConstructor();
        }


        public override void InitialiseFromEmptyConstructor()
        {
            typeof(Test).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            TestFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Description
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("DESCRIPTION")]
        public string Description { get; set; }

        /// <summary>
        /// Market
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("Market")]
        public string Market { get; set; }
    }
}
