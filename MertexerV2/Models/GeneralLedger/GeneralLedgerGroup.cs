using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.Utilities;

namespace MertexerV2.Models.GeneralLedger
{
    [UniVerseFileName("GLGROUP")]
    public class GeneralLedgerGroup : UniVerseSerializationBase<string>
    {
        public GeneralLedgerGroup(string key) : base(key)
        {

        }

        public GeneralLedgerGroup() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(GeneralLedgerGroup).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("TITLE")]
        public string Name { get; set; }
    }
}
