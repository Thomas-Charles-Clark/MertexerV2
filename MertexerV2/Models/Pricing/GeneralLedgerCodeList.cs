using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Pricing
{
    [UniVerseFileName("COLIST")]
    public class GeneralLedgerCodeList : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public GeneralLedgerCodeList(string key) : base(key)
        {

        }

        public GeneralLedgerCodeList() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(GeneralLedgerCodeList).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        /// <summary>
        /// Name
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("TITLE")]
        public string Name { get; set; }
    }
}
