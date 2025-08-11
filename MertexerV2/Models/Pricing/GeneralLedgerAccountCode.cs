using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Pricing
{
    [UniVerseFileName("GLACC")]
    public class GeneralLedgerAccountCode : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public GeneralLedgerAccountCode(string key) : base(key)
        {
            Initialise();
        }

        public GeneralLedgerAccountCode() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(GeneralLedgerAccountCode).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        /// <summary>
        /// Name
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("TITLE")]
        public string Name { get; set; }
    }
}
