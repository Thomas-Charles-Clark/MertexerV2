using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.CompanyFiles
{
	[UniVerseFileName("CFLANGUAGES")]
	public class CompanyLanguage : UniVerseSerializationBase<string>
	{
		[JsonConstructor]
		public CompanyLanguage(string key) : base(key)
		{
			Initialise();
		}

		public CompanyLanguage() : base()
		{
			Initialise();
		}

		public override void Initialise()
		{
			typeof(CompanyLanguage).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
		}

		[UniVerseIndex(1)]
		[UniVerseFieldName("DESCRIPTION")]
		public string Name { get; set; }

		[UniVerseIndex(2)]
		[UniVerseFieldName("CODE.TABLE")]
		public string MertexLanguageCode { get; set; }
	}
}
