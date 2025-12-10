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
	[UniVerseFileName("STPRODBATCH")]
	public class ProductBatch : UniVerseSerializationBase<string>
	{
		[JsonConstructor]
		public ProductBatch(string key) : base(key)
		{
			Initialise();
		}

		public ProductBatch() : base()
		{
			Initialise();
		}

		public override void Initialise()
		{
			typeof(ProductBatch).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
		}

		public void Upsert(string uniVerseAccount = "MERTEX")
		{
			ProductBatchFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
		}

		/// <summary>
		/// Product Code
		/// </summary>
		[UniVerseIndex(1)]
		[UniVerseFieldName("PRODCODE")]
		public string ProductCode { get; set; }

		/// <summary>
		/// Stock Location Code
		/// </summary>
		[UniVerseIndex(2)]
		[UniVerseFieldName("LOCCODE")]
		public string StockLocationCode { get; set; }

		/// <summary>
		/// Batch Number
		/// </summary>
		[UniVerseIndex(3)]
		[UniVerseFieldName("BATCH.NO")]
		public string BatchNumber { get; set; }

		[JsonAlsoUpdates("BatchDate")]
		public DateTime? BatchDT { get; set; }

		[UniVerseIndex(4)]
		[UniVerseFieldName("SS.BATCH.DATE.INT")]
		public string BatchDate
		{
			get
			{
				if (BatchDT.HasValue)
				{
					return Framework.UniVerseDateString(BatchDT.Value);
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
						BatchDT = Framework.DateTimeFromUniVerseOffset(valInt);
					}
					else
					{
						BatchDT = null;
					}
				}
			}
		}

		[JsonAlsoUpdates("BatchDate")]
		public DateTime? LastReceiptDT { get; set; }

		[UniVerseIndex(5)]
		[UniVerseFieldName("SS.LAST.RECEIPT.INT")]
		public string LastReceiptDate
		{
			get
			{
				if (LastReceiptDT.HasValue)
				{
					return Framework.UniVerseDateString(LastReceiptDT.Value);
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
						LastReceiptDT = Framework.DateTimeFromUniVerseOffset(valInt);
					}
					else
					{
						LastReceiptDT = null;
					}
				}
			}
		}

		[JsonAlsoUpdates("LastIssueDate")]
		public DateTime? LastIssueDT { get; set; }

		[UniVerseIndex(6)]
		[UniVerseFieldName("SS.LASTISSUE.INT")]
		public string LastIssueDate
		{
			get
			{
				if (LastIssueDT.HasValue)
				{
					return Framework.UniVerseDateString(LastIssueDT.Value);
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
						LastIssueDT = Framework.DateTimeFromUniVerseOffset(valInt);
					}
					else
					{
						LastIssueDT = null;
					}
				}
			}
		}

		/// <summary>
		/// Dummy
		/// </summary>
		[UniVerseIndex(7)]
		[UniVerseFieldName("DUMMY")]
		public string Dummy { get; set; }

		/// <summary>
		/// Physical Stock
		/// </summary>
		[UniVerseIndex(10)]
		[UniVerseFieldName("PHYSTOCK")]
		[UniVerseConversion("MR23")]
		public decimal StockPhysical { get; set; }

		/// <summary>
		/// Stock on Demand
		/// </summary>
		[UniVerseIndex(11)]
		[UniVerseFieldName("DEMANDS")]
		[UniVerseConversion("MR23")]
		public decimal StockDemands { get; set; }

		/// <summary>
		/// Stock on Demand
		/// </summary>
		[UniVerseIndex(12)]
		[UniVerseFieldName("FREE")]
		[UniVerseConversion("MR23")]
		public decimal StockFree { get; set; }

		/// <summary>
		/// Stock on Purchase Orders
		/// </summary>
		[UniVerseIndex(13)]
		[UniVerseFieldName("POSTOCK")]
		public decimal StockOnPurchaseOrders { get; set; }
	}
}
