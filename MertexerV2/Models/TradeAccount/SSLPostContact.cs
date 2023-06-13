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
    [UniVerseFileName("SSLPOST.CONTACTS")]
    public class SSLPostContact : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public SSLPostContact(string key) : base(key)
        {
            Initialise();
        }

        public SSLPostContact() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(SSLPostContact).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            SSLPostContactFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        public string DocumentType
        { 
            get
            {
                return Key.Split(Framework.AsteriskDelimiter, StringSplitOptions.RemoveEmptyEntries)[0];
            }
            set
            {
                Key = $"{value}*{TradeAccountCode}";
            }
        }

        public string TradeAccountCode
        {
            get
            {
                return Key.Split(Framework.AsteriskDelimiter, StringSplitOptions.RemoveEmptyEntries)[1];
            }
            set
            {
                Key = $"{DocumentType}*{value}";
            }
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("CONTACT.EMAIL")]
        public string Email { get; set; }
    }

}
