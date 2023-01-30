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
    [UniVerseFileName("STFORM")]
    public class Form : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public Form(string key) : base(key)
        {
            Initialise();
        }

        public Form() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(Form).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            FormFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Name
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("FORM.DESC")]
        public string Name { get; set; }
    }
}
