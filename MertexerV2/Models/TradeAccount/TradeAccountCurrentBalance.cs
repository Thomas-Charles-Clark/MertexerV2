using MertexerV2.Utilities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models
{
    [UniVerseFileName("SLCUST.CUR")]
    public class TradeAccountCurrentBalance : UniVerseSerializationBase<string>
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
        public TradeAccountCurrentBalance(string key) : base(key)
        {
            Initialise();
        }

        public TradeAccountCurrentBalance() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(TradeAccountCurrentBalance).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            //TradeAccountFunctions.TradeAccountFunctions(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Customer Details (Invoice Address)
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseMultiValue]
        [UniVerseFieldName("CUSDET")]
        public IEnumerable<string> CustomerDetails { get; set; }

        /// <summary>
        /// Balance(s)?
        /// </summary>
        [UniVerseIndex(18)]
        [UniVerseMultiValue]
        [UniVerseFieldName("ACBAL")]
        [UniVerseConversion("MR22")]
        public IEnumerable<decimal> AccountBalances { get; set; }

        [UniVerseIndex(40)]
        [UniVerseFieldName("ORDOH")]
        [UniVerseConversion("MR22")]
        public decimal OutstandingOrdersValue { get; set; }

        [UniVerseIndex(41)]
        [UniVerseFieldName("ORDCOUNT")]
        public int OutstandingOrdersCount { get; set; }

        [UniVerseIndex(89)]
        [UniVerseFieldName("STMT.ORDVAL")]
        [UniVerseConversion("MR22")]
        public decimal StatementOrdersValue { get; set; }

        [UniVerseIndex(802)]
        [UniVerseFieldName("RES.ORDOH.DES")]
        [UniVerseConversion("MR22")]
        public decimal OutstandingReservesValue { get; set; }

        [UniVerseIndex(803)]
        [UniVerseFieldName("RES.STMT.ORDVAL.DES")]
        [UniVerseConversion("MR22")]
        public decimal ReserveStatementOrdersValue { get; set; }
    }
}
