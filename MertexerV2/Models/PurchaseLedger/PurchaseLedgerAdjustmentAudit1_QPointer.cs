using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.Utilities;
using Newtonsoft.Json;

namespace MertexerV2.Models.PurchaseLedger
{
    [UniVerseFileName("PLADJAUDIT1")]
    public class PurchaseLedgerAdjustmentAudit1_QPointer : PurchaseLedgerAdjustmentAudit
    {
        [JsonConstructor]
        public PurchaseLedgerAdjustmentAudit1_QPointer(string key) : base(key)
        {
            Initialise();
        }

        public PurchaseLedgerAdjustmentAudit1_QPointer() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(PurchaseLedgerAdjustmentAudit1_QPointer).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }
    }
}
