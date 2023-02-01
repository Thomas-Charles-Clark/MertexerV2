using MertexerV2.Models.Interfaces;
using MertexerV2.ProductFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Pricing
{
    [UniVerseFileName("PO.PRICE.LIST")]
    public class POPriceList : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public POPriceList(string key) : base(key)
        {

        }

        public POPriceList() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(POPriceList).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            POPriceListFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Purchase Unit
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("PUNIT")]
        public string PurchaseUnit { get; set; }

        /// <summary>
        /// Quantity (MV)
        /// </summary>
        [UniVerseIndex(3)]
        [UniVerseMultiValue]
        [UniVerseConversion("MR23")]
        [UniVerseFieldName("QUANTITY")]
        [JsonConverter(typeof(StringToArrayConverter<IEnumerable<string>>))]
        public IEnumerable<string> Quantities { get; set; }

        /// <summary>
        /// Price (MV)
        /// </summary>
        [UniVerseIndex(4)]
        [UniVerseMultiValue]
        [UniVerseConversion("MR44")]
        [UniVerseFieldName("PRICE")]
        [JsonConverter(typeof(StringToArrayConverter<IEnumerable<string>>))]
        public IEnumerable<string> Prices { get; set; }

        /// <summary>
        /// Last Amended (MV)
        /// </summary>
        [UniVerseIndex(5)]
        [UniVerseMultiValue]
        [UniVerseFieldName("LAST.AMEND")]
        [JsonConverter(typeof(StringToArrayConverter<IEnumerable<string>>))]
        public IEnumerable<string> LastAmendedDates { get; set; }

        /// <summary>
        /// Open Order Price
        /// </summary>
        [UniVerseIndex(6)]
        [UniVerseFieldName("SS.OPEN.ORD.PRICE")]
        public decimal? OpenOrderPrice { get; set; }

        /// <summary>
        /// Future Order Price
        /// </summary>
        [UniVerseIndex(7)]
        [UniVerseFieldName("SS.FUT.ORD.PRICE")]
        public decimal? FutureOrderPrice { get; set; }
    }
}
