using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.Utilities;

using Newtonsoft.Json;

namespace MertexerV2.Models.GeneralLedger
{
    [UniVerseFileName("GLSUB")]
    public class GeneralLedgerAnalysisCodeS : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public GeneralLedgerAnalysisCodeS(string key) : base(key)
        {
            Initialise();
        }

        public GeneralLedgerAnalysisCodeS() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(GeneralLedgerAnalysisCodeS).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("TITLE")]
        public string Name { get; set; }

        [UniVerseIndex(2)]
        [UniVerseFieldName("GLGROUP")]
        public string ReportGroup { get; set; }
    }
}
