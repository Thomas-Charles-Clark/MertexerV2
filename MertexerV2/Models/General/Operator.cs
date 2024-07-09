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
    [UniVerseFileName("TSOPCODES")]
    public class Operator : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public Operator(string key) : base(key)
        {
            Initialise();
        }

        public Operator() : base()
        { 
            Initialise();
        }


        public override void Initialise()
        {
            typeof(Operator).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            RadioTerminalUserFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Name
        /// </summary>
        [UniVerseIndex(3)]
        [UniVerseFieldName("OPERATORNAME")]
        public string Name { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [UniVerseIndex(15)]
        [UniVerseFieldName("TDW.EMAIL")]
        public string EmailAddress { get; set; }
    }
}
