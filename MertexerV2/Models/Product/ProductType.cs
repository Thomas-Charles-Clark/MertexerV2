using MertexerV2.ProductFunctions;
using MertexerV2.Utilities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Product
{
    [UniVerseFileName("SS.PRODUCT.TYPE")]
    public class ProductType : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public ProductType(string key) : base(key)
        {
            Initialise();
        }

        public ProductType() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(ProductType).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            ProductTypeFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("TITLE")]
        public string Name { get; set; }

        /// <summary>
        /// These multi-valued fields are some crazy BS
        /// They dictate which fields to lift, separated by "SP" indicating a space.
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseMultiValue]
        [UniVerseFieldName("DESC1")]
        public IEnumerable<string> MertexDescription { get; set; }

        [UniVerseIndex(3)]
        [UniVerseMultiValue]
        [UniVerseFieldName("PRICE1")]
        public IEnumerable<string> PriceListDescription { get; set; }

        [UniVerseIndex(4)]
        [UniVerseMultiValue]
        [UniVerseFieldName("ONLINE.BASK")]
        public IEnumerable<string> OnlineProductDescription { get; set; }

        [UniVerseIndex(7)]
        [UniVerseMultiValue]
        [UniVerseFieldName("DISP.NAME")]
        public IEnumerable<string> DisplayName { get; set; }

        [UniVerseIndex(8)]
        [UniVerseMultiValue]
        [UniVerseFieldName("DISP.NAME.PRICE")]
        public IEnumerable<string> DisplayNamePriceList { get; set; }

        [UniVerseIndex(9)]
        [UniVerseMultiValue]
        [UniVerseFieldName("DISP.NAME.BASK")]
        public IEnumerable<string> DisplayNameBasket { get; set; }

        [UniVerseIndex(10)]
        [UniVerseMultiValue]
        [UniVerseFieldName("SORTNAME")]
        public IEnumerable<string> MertexSearchDescription { get; set; }

        [UniVerseIndex(11)]
        [UniVerseMultiValue]
        [UniVerseFieldName("DISP.SORT")]
        public IEnumerable<string> DisplayNameSort { get; set; }

        [UniVerseIndex(30)]
        [UniVerseFieldName("FILENAME")]
        public string FileName { get; set; }

        [UniVerseIndex(31)]
        [UniVerseMultiValue]
        [UniVerseFieldName("PRICE.TD")]
        public IEnumerable<string> PriceListDescription2 { get; set; }
    }
}
