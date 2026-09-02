using MertexerV2.InvoiceFunctions;
using MertexerV2.OrderHeadFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models
{
	[UniVerseFileName("TOEHEAD")]
	public class OrderHead : UniVerseSerializationBase<string>
	{
		[JsonConstructor]
		public OrderHead(string key) : base(key)
		{
			Initialise();
		}

		public OrderHead() : base()
		{
			Initialise();
		}


		public override void Initialise()
		{
			typeof(OrderHead).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
		}

		public void Upsert(string uniVerseAccount = "MERTEX")
		{
			OrderHeadFunctions.OrderHeadFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
		}

		[JsonProperty("OrderID")]
		public override string Key { get => base.Key; set => base.Key = value; }

		[UniVerseIndex(1)]
		[UniVerseFieldName("CUSREF")]
		public string TradeAccountCode { get; set; }

		[UniVerseIndex(78)]
		[UniVerseFieldName("IMEMO")]
		[UniVerseMultiValue]
		public IEnumerable<string> InvoiceMemoLines { get; set; }

		[UniVerseIndex(440)]
		[UniVerseFieldName("DELTERMS")]
		public string DeliveryTerms { get; set; }
	}
}
