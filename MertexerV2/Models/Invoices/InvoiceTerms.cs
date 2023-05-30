using MertexerV2.TradeAccountFunctions;
using MertexerV2.Utilities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Invoices
{
    [UniVerseFileName("SLTERMS")]
    public class InvoiceTerms : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public InvoiceTerms(string key) : base(key)
        {
            Initialise();
        }

        public InvoiceTerms() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(InvoiceTerms).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            InvoiceTermsFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        [UniVerseIndex(4)]
        [UniVerseFieldName("TITLE")]
        public string Name { get; set; }
    }
}
