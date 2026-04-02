using MertexerV2.InvoiceFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.Models.Bank
{
    [UniVerseFileName("OECLAUSES")]
    public class InvoiceClause : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public InvoiceClause(string key) : base(key)
        {
            Initialise();
        }

        public InvoiceClause() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(InvoiceClause).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            BankFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("DESCRIPTION")]
        public string Description { get; set; }

		[UniVerseIndex(2)]
		[UniVerseFieldName("CLAUSES")]
        [UniVerseMultiValue]
		public IEnumerable<string> Clauses { get; set; }

		[UniVerseIndex(3)]
		[UniVerseFieldName("DOCUMENTS.DES")]
		[UniVerseMultiValue]
		public IEnumerable<string> AppliesToDocuments { get; set; }

		[UniVerseIndex(4)]
		[UniVerseFieldName("OE.REFS.DES")]
		[UniVerseMultiValue]
		public IEnumerable<string> OrderEntryReferences { get; set; }
	}
}
