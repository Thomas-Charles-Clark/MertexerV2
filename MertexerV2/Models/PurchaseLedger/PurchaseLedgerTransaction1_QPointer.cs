using MertexerV2.Utilities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.PurchaseLedger
{
    [UniVerseFileName("TPLTRANS1")]
    public class PurchaseLedgerTransaction1_QPointer : PurchaseLedgerTransaction
    {
        [JsonConstructor]
        public PurchaseLedgerTransaction1_QPointer(string key) : base(key)
        {
            Initialise();
        }

        public PurchaseLedgerTransaction1_QPointer() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(PurchaseLedgerTransaction1_QPointer).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }
    }
}
