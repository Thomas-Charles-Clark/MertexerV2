using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Pricing
{
    [UniVerseFileName("GLSUB")]
    public class GeneralLedgerSubCode : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public GeneralLedgerSubCode(string key) : base(key)
        {

        }

        public GeneralLedgerSubCode() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(GeneralLedgerSubCode).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        /// <summary>
        /// Name
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("TITLE")]
        public string Name { get; set; }
    }
}
