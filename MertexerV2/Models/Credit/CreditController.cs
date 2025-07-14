using MertexerV2.InvoiceFunctions;
using MertexerV2.StockFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Credit
{
    [UniVerseFileName("SL.CONTROLLER")]
    public class CreditController : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public CreditController(string key) : base(key)
        {
            Initialise();
        }

        public CreditController() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(CreditController).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            CreditControllerFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("CONT.NAME")]
        public string Name { get; set; }

        [UniVerseIndex(2)]
        [UniVerseFieldName("CONT.EMAIL")]
        public string Email { get; set; }

        [UniVerseIndex(3)]
        [UniVerseFieldName("CONT.REGION")]
        public string Region { get; set; }

        [UniVerseIndex(4)]
        [UniVerseFieldName("MARKTYPES")]
        [UniVerseMultiValue]
        public IEnumerable<string> MarketCodes { get; set; }

        [UniVerseIndex(5)]
        [UniVerseFieldName("OECODEGROUPS")]
        [UniVerseMultiValue]
        public IEnumerable<string> TradeAccountGroupCodes { get; set; }

        [UniVerseIndex(6)]
        [UniVerseFieldName("LANGUAGES")]
        [UniVerseMultiValue]
        public IEnumerable<string> MertexLanguageCodes { get; set; }
    }
}
