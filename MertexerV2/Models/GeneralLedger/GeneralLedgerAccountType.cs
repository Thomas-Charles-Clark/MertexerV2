using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.Utilities;

namespace MertexerV2.Models.GeneralLedger
{
    [UniVerseFileName("GLTYPE")]
    public class GeneralLedgerAccountType : UniVerseSerializationBase<string>
    {
        public GeneralLedgerAccountType(string key) : base(key)
        {

        }

        public GeneralLedgerAccountType() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(GeneralLedgerAccountType).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("TITLE")]
        public string Name { get; set; }

        [UniVerseIndex(2)]
        [UniVerseFieldName("SEQUENCE")]
        public byte Sequence { get; set; }
    }
}
