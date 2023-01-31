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
    [UniVerseFileName("STPRODCLASS2")]
    public class GeneralLedgerDiscountGroup : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public GeneralLedgerDiscountGroup(string key) : base(key)
        {
            Initialise();
        }

        public GeneralLedgerDiscountGroup() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(GeneralLedgerDiscountGroup).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            GeneralLedgerDiscountGroupFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Name
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("CODEDESC")]
        public string Name { get; set; }
    }
}
