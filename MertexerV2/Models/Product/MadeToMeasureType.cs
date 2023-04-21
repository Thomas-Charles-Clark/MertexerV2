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
    [UniVerseFileName("OEMTMTYPES")]
    public class MadeToMeasureType : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public MadeToMeasureType(string key) : base(key)
        {
            Initialise();
        }

        public MadeToMeasureType() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(MadeToMeasureType).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            MadeToMeasureTypeFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Name 
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("DESCRIPTION")]
        public string Name { get; set; }
    }
}
