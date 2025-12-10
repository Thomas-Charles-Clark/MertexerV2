using System;
using System.Collections.Generic;
using System.Linq;

using MertexerV2.Utilities;

using Newtonsoft.Json;

namespace MertexerV2.Models.SalesLedger
{
    [UniVerseFileName("TSLEDG")]
    public class SalesLedger : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public SalesLedger(string key) : base(key)
        {
            Initialise();
        }

        public SalesLedger() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(SalesLedger).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
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
