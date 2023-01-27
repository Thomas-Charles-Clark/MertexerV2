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
    [UniVerseFileName("SS.ORDERFORMGROUP")]
    public class OrderFormGroup : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public OrderFormGroup(string key) : base(key)
        {
            Initialise();
        }

        public OrderFormGroup() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(OrderFormGroup).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            OrderFormGroupFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Order Form Group Name
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("TITLE")]
        public string Name { get; set; }

        /// <summary>
        /// SortOrder
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("SS.SORT")]
        public int? SortOrder { get; set; }
    }
}
