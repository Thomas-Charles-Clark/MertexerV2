using MertexerV2.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.ExchangeRate
{
    [UniVerseFileName("XRATE")]
    public class ExchangeRate : UniVerseSerializationBase<string>
    {
        public ExchangeRate(string key) : base(key)
        {

        }

        public ExchangeRate() : base()
        {
            InitialiseFromEmptyConstructor();
        }

        public override void InitialiseFromEmptyConstructor()
        {
            typeof(ExchangeRate).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        /// <summary>
        /// Description
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("DESC")]
        public string Description { get; set; }

        /// <summary>
        /// Rate
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseConversion("MR66")]
        [UniVerseFieldName("RATE")]
        public string Rate { get; set; }
    }
}
