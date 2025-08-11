using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.StockFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

namespace MertexerV2.Models.Stock
{
    [UniVerseFileName("STVALUATION")]
    public class StockValuation : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public StockValuation(string key) : base(key)
        {
            Initialise();
        }

        public StockValuation() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(StockValuation).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            StockValuationFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("A.VALUE")]
        [UniVerseConversion("MR22")]
        public decimal TotalPhysicalStockValue { get; set; }

        [UniVerseIndex(5)]
        [UniVerseFieldName("PHYSICAL")]
        [UniVerseConversion("MR23")]
        public decimal TotalPhysicalStockQuantity { get; set; }

        [UniVerseIndex(7)]
        [UniVerseFieldName("LOCCODE")]
        public string StockLocationCode { get; set; }

        [UniVerseIndex(9)]
        [UniVerseFieldName("PRODUCT")]
        public string ProductCode { get; set; }


    }
}
