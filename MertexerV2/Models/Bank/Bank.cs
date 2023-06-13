using MertexerV2.InvoiceFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

using System.Linq;

namespace MertexerV2.Models.Bank
{
    [UniVerseFileName("BANK")]
    public class Bank : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public Bank(string key) : base(key)
        {
            Initialise();
        }

        public Bank() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(Bank).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            BankFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("TITLE")]
        public string Name { get; set; }
    }
}
