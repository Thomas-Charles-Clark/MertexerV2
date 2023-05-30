using MertexerV2.GeneralFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.General
{
    [UniVerseFileName("ORD.CATAGORY.DES")]
    public class OrderCategory : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public OrderCategory(string key) : base(key)
        {
            Initialise();
        }

        public OrderCategory() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(OrderCategory).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            OrderCategoryFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Name
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("ORD.CAT.DESC.DES")]
        public string Name { get; set; }
    }
}
