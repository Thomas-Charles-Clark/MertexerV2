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
    [UniVerseFileName("STPRODCLASS2")]
    public class GeneralLedgerDiscountCodeGroup : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public GeneralLedgerDiscountCodeGroup(string key) : base(key)
        {
            Initialise();
        }

        public GeneralLedgerDiscountCodeGroup() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(GeneralLedgerDiscountCodeGroup).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            GeneralLedgerDiscountCodeGroupFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Name
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("CODEDESC")]
        public string Name { get; set; }
    }
}
