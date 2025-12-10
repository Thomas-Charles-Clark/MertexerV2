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
	[UniVerseFileName("STBATCHES")]
	public class Batch : UniVerseSerializationBase<string>
	{
		[JsonConstructor]
		public Batch(string key) : base(key)
		{
			Initialise();
		}

		public Batch() : base()
		{
			Initialise();
		}

		public override void Initialise()
		{
			typeof(Batch).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
		}

		public void Upsert(string uniVerseAccount = "MERTEX")
		{
			BatchFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
		}

		/// <summary>
		/// Product Code
		/// </summary>
		[UniVerseIndex(1)]
		[UniVerseFieldName("PRODCODE")]
		public string ProductCode { get; set; }

		/// <summary>
		/// Batch Number
		/// </summary>
		[UniVerseIndex(2)]
		[UniVerseFieldName("BATCH.NO")]
		public string BatchNumber { get; set; }

		[UniVerseIndex(5)]
		[UniVerseFieldName("RACK")]
		[UniVerseMultiValue]
		public IEnumerable<string> Racks { get; set; }

		[UniVerseIndex(9)]
		[UniVerseFieldName("LOCBATKEYS")]
		[UniVerseMultiValue]
		public IEnumerable<string> ProductLocationBatchKeys { get; set; }

		[JsonAlsoUpdates("ReceivedDate")]
		public DateTime? ReceivedDT { get; set; }

		[UniVerseIndex(35)]
		[UniVerseFieldName("SS.RECEIPT.DATE.INT")]
		public string ReceivedDate
		{
			get
			{
				if (ReceivedDT.HasValue)
				{
					return Framework.UniVerseDateString(ReceivedDT.Value);
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (!String.IsNullOrEmpty(value))
				{
					if (Int32.TryParse(value, out int valInt))
					{
						ReceivedDT = Framework.DateTimeFromUniVerseOffset(valInt);
					}
					else
					{
						ReceivedDT = null;
					}
				}
			}
		}

		[UniVerseIndex(44)]
		[UniVerseFieldName("RACKQTY.PHYS")]
		[UniVerseConversion("MR23")]
		[UniVerseMultiValue]
		public IEnumerable<decimal> RackQuantityPhysical { get; set; }

		[UniVerseIndex(45)]
		[UniVerseFieldName("RACKQTY.DEMS")]
		[UniVerseConversion("MR23")]
		[UniVerseMultiValue]
		public IEnumerable<decimal> RackQuantityDemands { get; set; }

		[UniVerseIndex(46)]
		[UniVerseFieldName("RACKQTY.FREE")]
		[UniVerseConversion("MR23")]
		[UniVerseMultiValue]
		public IEnumerable<decimal> RackQuantityFree { get; set; }

		[UniVerseIndex(47)]
		[UniVerseFieldName("RACK.LOCCODE")]
		[UniVerseMultiValue]
		public IEnumerable<string> StockLocationCodes { get; set; }

		[UniVerseIndex(48)]
		[UniVerseFieldName("RACKLOC.COMB")]
		[UniVerseMultiValue]
		public IEnumerable<string> RacksAndLocationCombined { get; set; }

		// Also requested was [UniVerseIndex(0)] SS.SUPPREF1 which looks to STPRODMASTER<70.1> "SUPPREF1"
		// Lift it from there in SQL if desired, it IS being emitted and eventually ends up in
		// both [Maintenance].[MertexProduct].[SupplierCodePrimary]
		// and [Mertex].[Product].[DefaultSupplierCode]

		/// <summary>
		/// Current Cost Value
		/// </summary>
		[UniVerseIndex(90)]
		[UniVerseFieldName("CURR.CVALUE")]
		[UniVerseConversion("MR22")]
		public decimal CurrentCostValue { get; set; }
	}
}
