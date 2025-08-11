using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Pricing
{
    [UniVerseFileName("GLALYS")]
    public class GeneralLedgerAnalysisCode : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public GeneralLedgerAnalysisCode(string key) : base(key)
        {
            Initialise();
        }

        public GeneralLedgerAnalysisCode() : base()
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
