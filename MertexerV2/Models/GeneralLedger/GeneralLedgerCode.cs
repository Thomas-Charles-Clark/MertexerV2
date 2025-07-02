using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.AccountFunctions;
using MertexerV2.Utilities;

using Newtonsoft.Json;

namespace MertexerV2.Models.GeneralLedger
{
    [UniVerseFileName("GLEDGER")]
    public class GeneralLedgerCode : UniVerseSerializationBase<string>
    {
        public GeneralLedgerCode(string key) : base(key)
        {

        }

        public GeneralLedgerCode() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(GeneralLedgerCode).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("ACCOUNT")]
        public string AccountCode { get; set; }

        [UniVerseIndex(2)]
        [UniVerseFieldName("GROUP")]
        public string ReportingGroup { get; set; }

        [UniVerseIndex(3)]
        [UniVerseFieldName("TYPE")]
        public string AccountType { get; set; }

        [UniVerseIndex(5)]
        [UniVerseFieldName("ANALYSIS")]
        public string AnalysisCodeD { get; set; }

        [UniVerseIndex(6)]
        [UniVerseFieldName("BSDESC")]
        public string AnalysisCodeDescription { get; set; }

        [UniVerseIndex(47)]
        [UniVerseFieldName("GLSUB")]
        public string AnalysisCodeS { get; set; }

        [UniVerseIndex(48)]
        [UniVerseFieldName("COLIST")]
        public string AnalysisCodeC { get; set; }
    }
}
