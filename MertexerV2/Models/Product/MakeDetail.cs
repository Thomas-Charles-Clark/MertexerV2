using MertexerV2.Models.Interfaces;
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
    [UniVerseFileName("STMAKE")]
    public class MakeDetail : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public MakeDetail(string key) : base(key)
        {

        }

        public MakeDetail() : base()
        {
            InitialiseFromEmptyConstructor();
        }

        public override void InitialiseFromEmptyConstructor()
        {
            typeof(MakeDetail).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            MakeDetailFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Product Code
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("PRODCODE")]
        public string ProductCode { get; set; }

        /// <summary>
        /// Process Code
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("PROCESS")]
        public string ProcessCode { get; set; }

        /// <summary>
        /// Product Codes (MV)
        /// </summary>
        [UniVerseIndex(3)]
        [UniVerseMultiValue]
        [UniVerseFieldName("PRODCODES")]
        public IEnumerable<string> ProductCodes { get; set; }

        /// <summary>
        /// Quantities (MV)
        /// </summary>
        [UniVerseIndex(4)]
        [UniVerseMultiValue]
        [UniVerseFieldName("QUANTITY")]
        [UniVerseConversion("MR33")]
        public IEnumerable<decimal?> Quantities { get; set; }

        /// <summary>
        /// Units (cm) (MV)
        /// </summary>
        [UniVerseIndex(5)]
        [UniVerseMultiValue]
        [UniVerseFieldName("UNIT")]
        public IEnumerable<string> Units { get; set; }

        /// <summary>
        /// Comments (MV)
        /// </summary>
        [UniVerseIndex(10)]
        [UniVerseMultiValue]
        [UniVerseFieldName("COMMENTS")]
        public IEnumerable<string> Comments { get; set; }

        /// <summary>
        /// Hidden Read Field
        /// </summary>
        [UniVerseIndex(12)]
        [UniVerseFieldName("READ.FLD")]
        public string HiddenReadField { get; set; }

        /// <summary>
        /// Agreed Percentage Loss (MV)
        /// </summary>
        [UniVerseIndex(14)]
        [UniVerseMultiValue]
        [UniVerseFieldName("AGREED")]
        [UniVerseConversion("MR22")]
        public IEnumerable<decimal?> AgreedPercentageLoss { get; set; }

        /// <summary>
        /// Expected Percentage Loss (MV)
        /// </summary>
        [UniVerseIndex(15)]
        [UniVerseMultiValue]
        [UniVerseFieldName("EXPECTED")]
        [UniVerseConversion("MR22")]
        public IEnumerable<decimal?> ExpectedPercentageLoss { get; set; }
    }
}
