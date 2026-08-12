using MertexerV2.Utilities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Carriage
{
	[UniVerseFileName("OECARRIAGE")]
	public class Carriage : UniVerseSerializationBase<string>
	{
		[JsonConstructor]
		public Carriage(string key) : base(key)
		{
			Initialise();
		}

		public Carriage() : base()
		{
			Initialise();
		}

		public override void Initialise()
		{
			typeof(Carriage).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
		}

		public void Upsert(string uniVerseAccount = "MERTEX")
		{
			CarriageFunctions.CarriageFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
		}

		public string CarriageCode
		{
			get
			{
				var split = Key.Split(Framework.AsteriskDelimiter);

				if (split.Length == 2)
				{
					return split[0];
				}
				else
				{
					return null;
				}
			}
		}

		public string HaulierCode
		{
			get
			{
				var split = Key.Split(Framework.AsteriskDelimiter);

				if (split.Length == 2)
				{
					return split[1];
				}
				else
				{
					return null;
				}
			}
		}

		[UniVerseIndex(1)]
		[UniVerseFieldName("DESCRIPTION")]
		public string Description { get; set; }

		[UniVerseIndex(46)]
		[UniVerseFieldName("PRTY.ORDER.DES")]
		public bool PriorityOrder { get; set; }

		[UniVerseIndex(58)]
		[UniVerseFieldName("BILL.3RD.TRANSPORT.DES")]
		public bool BillTransportToThirdParty { get; set; }

		[UniVerseIndex(59)]
		[UniVerseFieldName("BILL.3RD.DUTY.DES")]
		public bool BillDutyToThirdParty { get; set; }

		[UniVerseIndex(60,1)]
		[UniVerseFieldName("TP.COMPANY.NAME.DES")]
		public string ThirdPartyCompanyName { get; set; }

		[UniVerseIndex(60,2)]
		[UniVerseFieldName("TP.ADDRESS.LINE1.DES")]
		public string ThirdPartyAddressLine1 { get; set; }

		[UniVerseIndex(60, 3)]
		[UniVerseFieldName("TP.ADDRESS.LINE2.DES")]
		public string ThirdPartyAddressLine2 { get; set; }

		[UniVerseIndex(60, 4)]
		[UniVerseFieldName("TP.CITY.TOWN.DES")]
		public string ThirdPartyAddressTown { get; set; }

		[UniVerseIndex(60, 5)]
		[UniVerseFieldName("TP.STATE.DES")]
		public string ThirdPartyAddressCounty { get; set; }

		[UniVerseIndex(60, 6)]
		[UniVerseFieldName("TP.POSTCODE.DES")]
		public string ThirdPartyAddressPostcode { get; set; }

		[UniVerseIndex(61)]
		[UniVerseFieldName("TP.COUNTRY.DES")]
		public string ThirdPartyAddressCountry { get; set; }

		[UniVerseIndex(62)]
		[UniVerseFieldName("TP.TELEPHONE.DES")]
		public string ThirdPartyPhone { get; set; }

		[UniVerseIndex(63)]
		[UniVerseFieldName("TP.UPS.ACCOUNT.NO.DES")]
		public string ThirdPartyUPSAccountNumberFreight { get; set; }

		[UniVerseIndex(64)]
		[UniVerseFieldName("TP.CONTACT.DES")]
		public string ThirdPartyContactName { get; set; }

		[UniVerseIndex(65)]
		[UniVerseFieldName("EORI.NUMBER")]
		public string ThirdPartyEORINumber { get; set; }

		[UniVerseIndex(66)]
		[UniVerseFieldName("TP.DUTY.ACC")]
		public string ThirdPartyUPSAccountNumberDuty { get; set; }

		[UniVerseIndex(67)]
		[UniVerseFieldName("ADDITIONAL.LABEL.DES")]
		public string DespatchPathway { get; set; }

		[UniVerseIndex(68)]
		[UniVerseFieldName("DISP.GB.IND.DES")]
		public bool DisplayGBIndicator { get; set; }
	}
}
