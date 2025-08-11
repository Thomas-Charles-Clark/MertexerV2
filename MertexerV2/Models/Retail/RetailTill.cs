using MertexerV2.Utilities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models
{
    [UniVerseFileName("OETILLNO")]
    public class RetailTill : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public RetailTill(string key) : base(key)
        {
            Initialise();
        }

        public RetailTill() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(RetailTill).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        /// <summary>
        /// Description
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("DESCRIPTION")]
        public string Description { get; set; }
    }
}
