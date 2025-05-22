using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.Utilities;
using Newtonsoft.Json;

namespace MertexerV2.Models.PurchaseLedger
{
    [UniVerseFileName("PLSEL3")]
    public class BudgetHolder : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public BudgetHolder(string key) : base(key)
        {
            Initialise();
        }

        public BudgetHolder() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(BudgetHolder).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("TITLE")]
        public string Name { get; set; }

        [UniVerseIndex(2)]
        [UniVerseFieldName("AUTOPO.EMAILADDR.DES")]
        public string PurchaseOrderEmailAddress { get; set; }
    }
}
