using MertexerV2.Models.Interfaces;
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
        public IEnumerable<string> Quantity { get; set; }

        /// <summary>
        /// Price (MV)
        /// </summary>
        [UniVerseIndex(4)]
        [UniVerseMultiValue]
        [UniVerseConversion("MR44")]
        [UniVerseFieldName("PRICE")]
        public IEnumerable<string> Price { get; set; }

        /// <summary>
        /// Last Amended (MV)
        /// </summary>
        [UniVerseIndex(5)]
        [UniVerseMultiValue]
        [UniVerseFieldName("LAST.AMEND")]
        public IEnumerable<string> LastAmended { get; set; }

        /// <summary>
        /// Open Order Price
        /// </summary>
        [UniVerseIndex(6)]
        [UniVerseFieldName("SS.OPEN.ORD.PRICE")]
        public string OpenOrderPrice { get; set; }

        /// <summary>
        /// Future Order Price
        /// </summary>
        [UniVerseIndex(7)]
        [UniVerseFieldName("SS.FUT.ORD.PRICE")]
        public string FutureOrderPrice { get; set; }
    }
}
