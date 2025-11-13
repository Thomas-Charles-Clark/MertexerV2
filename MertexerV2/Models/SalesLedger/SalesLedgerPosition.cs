using System.Collections.Generic;
using System.Linq;

using MertexerV2.Utilities;

using Newtonsoft.Json;

namespace MertexerV2.Models.SalesLedger
{
    [UniVerseFileName("SLPOS")]
    public class SalesLedgerPosition : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public SalesLedgerPosition(string key) : base(key)
        {
            Initialise();
        }

        public SalesLedgerPosition() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(SalesLedgerPosition).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        /// <summary>
        /// Description
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("TITLE")]
        public string Description { get; set; }
    }
}
