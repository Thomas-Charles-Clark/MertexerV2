using MertexerV2.ProductFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Product
{
	[UniVerseFileName("OEMTMTYPES")]
	public class BatchRackLocation : UniVerseSerializationBase<string>
	{
		[JsonConstructor]
		public BatchRackLocation(string key) : base(key)
		{
			Initialise();
		}

		public BatchRackLocation() : base()
		{
			Initialise();
		}

		public override void Initialise()
		{
			typeof(BatchRackLocation).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
		}

		public void Upsert(string uniVerseAccount = "MERTEX")
		{
			BatchRackLocationFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
		}

		/// <summary>
		/// Name 
		/// </summary>
		[UniVerseIndex(1)]
		[UniVerseFieldName("DESCRIPTION")]
		public string Name { get; set; }
	}
}
