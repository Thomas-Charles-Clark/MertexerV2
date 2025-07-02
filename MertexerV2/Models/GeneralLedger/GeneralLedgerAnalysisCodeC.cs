using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.Utilities;

namespace MertexerV2.Models.GeneralLedger
{
    [UniVerseFileName("COLIST")]
    public class GeneralLedgerAnalysisCodeC : UniVerseSerializationBase<string>
    {
        public GeneralLedgerAnalysisCodeC(string key) : base(key)
        {

        }

        public GeneralLedgerAnalysisCodeC() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(GeneralLedgerAnalysisCodeC).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("TITLE")]
        public string Name { get; set; }

        [UniVerseIndex(2)]
        [UniVerseFieldName("GLGROUP")]
        public string ReportGroup { get; set; }
    }
}
