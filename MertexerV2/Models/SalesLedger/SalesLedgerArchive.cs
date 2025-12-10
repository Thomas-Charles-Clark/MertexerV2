using System;
using System.Collections.Generic;
using System.Linq;

using MertexerV2.Utilities;

using Newtonsoft.Json;

namespace MertexerV2.Models.SalesLedger
{
    [UniVerseFileName("TSLEDG2")]
    public class SalesLedgerArchive : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public SalesLedgerArchive(string key) : base(key)
        {
            Initialise();
        }

        public SalesLedgerArchive() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(SalesLedgerArchive).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

		[JsonAlsoUpdates("PostDate")]
		public DateTime? PostDT { get; set; }

		[UniVerseIndex(35)]
		[UniVerseFieldName("SS.POSTDATE.INT")]
		public string PostDate
		{
			get
			{
				if (PostDT.HasValue)
				{
					return Framework.UniVerseDateString(PostDT.Value);
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
						PostDT = Framework.DateTimeFromUniVerseOffset(valInt);
					}
					else
					{
						PostDT = null;
					}
				}
			}
		}

		/// <summary>
		/// Memo
		/// </summary>
		[UniVerseIndex(56)]
        [UniVerseFieldName("MEMO")]
        public string Memo { get; set; }
    }
}
