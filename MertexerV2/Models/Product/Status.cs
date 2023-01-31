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
    [UniVerseFileName("STPRODSTATUS")]
    public class Status : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public Status(string key) : base(key)
        {
            Initialise();
        }

        public Status() : base()
        {
            Initialise();
        }


        public override void Initialise()
        {
            typeof(Status).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            StatusFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Name
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("DESCRIPTION")]
        public string Name { get; set; }
    }
}
