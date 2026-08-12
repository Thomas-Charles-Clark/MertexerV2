using MertexerV2.GeneralFunctions;
using MertexerV2.Models.Carriage;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace MertexerV2.CarriageFunctions
{
    public class CarriageCode
    {
        public string Code { get; set; }
    }

    public static partial class CarriageFunctions
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "CarriageSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string CarriageCodes)
        {
            if (String.IsNullOrEmpty(CarriageCodes))
            {
                foreach (var q in Selector<Carriage>.SelectAllNonParallel(
                   true,
                   false,
                   50,
                   UniVerseAccount))
                {
                    yield return q;
                }
            }
            else
            {
                var results = Selector<Carriage>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<CarriageCode>>(CarriageCodes).Select(p => p.Code),
                    true,
                    false,
                    50,
                    UniVerseAccount)
                    .ToList();

                foreach (var r in results)
                {
                    yield return r;
                }
            }
        }

        public static void CarriageSelect_FillRow(
            object obj,
            out SqlString Key,
            out SqlString CarriageCode,
            out SqlString HaulierCode,
            out SqlString Description,
            out SqlBoolean PriorityOrder,
            out SqlBoolean BillTransportToThirdParty,
			out SqlString ThirdPartyUPSAccountNumberFreight,
			out SqlBoolean BillDutyToThirdParty,
			out SqlString ThirdPartyUPSAccountNumberDuty,
			out SqlString ThirdPartyCompanyName,
            out SqlString ThirdPartyAddressLine1,
            out SqlString ThirdPartyAddressLine2,
            out SqlString ThirdPartyAddressTown,
            out SqlString ThirdPartyAddressCounty,
            out SqlString ThirdPartyAddressPostcode,
            out SqlString ThirdPartyAddressCountry,
            out SqlString ThirdPartyPhone,
            out SqlString ThirdPartyContactName,
            out SqlString ThirdPartyEORINumber,
            out SqlString DispatchPathway,
            out SqlBoolean DisplayGBIndicator
            )
        {
            var p = (Carriage)obj;

            Key = p.Key;

            CarriageCode = String.IsNullOrEmpty(p.CarriageCode) ? SqlString.Null : new SqlString(p.CarriageCode);

            HaulierCode = String.IsNullOrEmpty(p.HaulierCode) ? SqlString.Null : new SqlString(p.HaulierCode);

			Description = String.IsNullOrEmpty(p.Description) ? SqlString.Null : new SqlString(p.Description);

            PriorityOrder = p.PriorityOrder;

			BillTransportToThirdParty = p.BillTransportToThirdParty;

            BillDutyToThirdParty = p.BillDutyToThirdParty;

            ThirdPartyCompanyName = String.IsNullOrEmpty(p.ThirdPartyCompanyName) ? SqlString.Null : new SqlString(p.ThirdPartyCompanyName);

			ThirdPartyAddressLine1 = String.IsNullOrEmpty(p.ThirdPartyAddressLine1) ? SqlString.Null : new SqlString(p.ThirdPartyAddressLine1);

            ThirdPartyAddressLine2 = String.IsNullOrEmpty(p.ThirdPartyAddressLine2) ? SqlString.Null : new SqlString(p.ThirdPartyAddressLine2);
			
            ThirdPartyAddressTown = String.IsNullOrEmpty(p.ThirdPartyAddressTown) ? SqlString.Null : new SqlString(p.ThirdPartyAddressTown);
			
            ThirdPartyAddressCounty = String.IsNullOrEmpty(p.ThirdPartyAddressCounty) ? SqlString.Null : new SqlString(p.ThirdPartyAddressCounty);
			
            ThirdPartyAddressPostcode = String.IsNullOrEmpty(p.ThirdPartyAddressPostcode) ? SqlString.Null : new SqlString(p.ThirdPartyAddressPostcode);
			
            ThirdPartyAddressCountry = String.IsNullOrEmpty(p.ThirdPartyAddressCountry) ? SqlString.Null : new SqlString(p.ThirdPartyAddressCountry);
			
            ThirdPartyPhone = String.IsNullOrEmpty(p.ThirdPartyPhone) ? SqlString.Null : new SqlString(p.ThirdPartyPhone);
			
            ThirdPartyUPSAccountNumberFreight = String.IsNullOrEmpty(p.ThirdPartyUPSAccountNumberFreight) ? SqlString.Null : new SqlString(p.ThirdPartyUPSAccountNumberFreight);
			
            ThirdPartyContactName = String.IsNullOrEmpty(p.ThirdPartyContactName) ? SqlString.Null : new SqlString(p.ThirdPartyContactName);
			
            ThirdPartyEORINumber = String.IsNullOrEmpty(p.ThirdPartyEORINumber) ? SqlString.Null : new SqlString(p.ThirdPartyEORINumber);
			
            ThirdPartyUPSAccountNumberDuty = String.IsNullOrEmpty(p.ThirdPartyUPSAccountNumberDuty) ? SqlString.Null : new SqlString(p.ThirdPartyUPSAccountNumberDuty);
			
            DispatchPathway = String.IsNullOrEmpty(p.DespatchPathway) ? SqlString.Null : new SqlString(p.DespatchPathway);
			
            DisplayGBIndicator = p.DisplayGBIndicator;
		}
    }
}
