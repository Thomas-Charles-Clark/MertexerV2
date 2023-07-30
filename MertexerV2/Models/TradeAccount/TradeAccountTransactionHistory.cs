using MertexerV2.Utilities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models
{
    [UniVerseFileName("HISTSLCUST.CUR")]
    public class TradeAccountTransactionHistory : UniVerseSerializationBase<string>
    {
        [JsonRequired]
        public override string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }

        public string TradeAccountCode
        {
            get
            {
                return Key;
            }
            set
            {
                Key = value;
            }
        }

        [JsonConstructor]
        public TradeAccountTransactionHistory(string key) : base(key)
        {
            Initialise();
        }

        public TradeAccountTransactionHistory() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(TradeAccountTransactionHistory).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            //TradeAccountFunctions.TradeAccountFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Customer Details (Invoice Address)
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseMultiValue]
        [UniVerseFieldName("CUSDET")]
        public IEnumerable<string> DocumentHistory { get; set; }

        /// <summary>
        /// Customer Details (Invoice Address)
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("CUSDET")]
        public string MertexCurrencyCode { get; set; }
    }
}
