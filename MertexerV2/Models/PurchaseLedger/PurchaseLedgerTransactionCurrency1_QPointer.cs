using MertexerV2.Utilities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.PurchaseLedger
{
    [UniVerseFileName("TPLTRANS.CUR1")]
    public class PurchaseLedgerTransactionCurrency1_QPointer : PurchaseLedgerTransactionCurrency
    {
        [JsonConstructor]
        public PurchaseLedgerTransactionCurrency1_QPointer(string key) : base(key)
        {
            Initialise();
        }

        public PurchaseLedgerTransactionCurrency1_QPointer() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(PurchaseLedgerTransactionCurrency1_QPointer).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }
    }
}
