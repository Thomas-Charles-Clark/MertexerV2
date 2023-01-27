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
    [UniVerseFileName("STMISC2")]
    public class MertexType : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public MertexType(string key) : base(key)
        {

        }

        public MertexType() : base()
        {
            InitialiseFromEmptyConstructor();
        }

        public override void InitialiseFromEmptyConstructor()
        {
            typeof(MertexType).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            MertexTypeFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Description
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("DESCRIPTION")]
        public string Name { get; set; }
    }
}
