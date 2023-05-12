using MertexerV2.TradeAccountFunctions;
using MertexerV2.Utilities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models
{
    public class SSLPostDocument : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public SSLPostDocument(string key) : base(key)
        {
            Initialise();
        }

        public SSLPostDocument() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(SSLPostDocument).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            SSLPostDocumentFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        public string DocumentType
        { 
            get
            {
                return Key.Split(Framework.AsteriskDelimiter, StringSplitOptions.RemoveEmptyEntries)[0];
            }
        }

        public string Reference
        {
            get
            {
                return Key.Split(Framework.AsteriskDelimiter, StringSplitOptions.RemoveEmptyEntries)[1];
            }
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("CONTACT.EMAIL")]
        public string Email { get; set; }
    }

}
