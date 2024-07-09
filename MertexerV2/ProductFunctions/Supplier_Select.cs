using MertexerV2.GeneralFunctions;
using MertexerV2.Models.EDAReplacement;
using MertexerV2.Models.Product;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.ProductFunctions
{
    public class SupplierCode
    {
        public string Code { get; set; }
    }

    public static partial class SupplierFunctions    
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "SupplierSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string SupplierCodes)
        {
            if (String.IsNullOrEmpty(SupplierCodes))
            {
                foreach (var q in Selector<Models.Product.Supplier>.SelectAllNonParallel(
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
                var results = Selector<Models.Product.Supplier>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<SupplierCode>>(SupplierCodes).Select(p => p.Code),
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

        public static void SupplierSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name,
            out SqlString AddressLine1,
            out SqlString AddressLine2,
            out SqlString Town,
            out SqlString County,
            out SqlString Postcode,
            out SqlString CountryCode,
            out SqlString Currency,
            out SqlString PhoneNumber,
            out SqlString EmailAddress,
            out SqlDecimal DutyPercentage,
            out SqlDecimal InlandChargesPercentage,
            out SqlDecimal MiscellaneousPercentage,
            out SqlDecimal ImportChargePerUnit,
            out SqlDecimal MiscellaneousCharge
            )
        {
            var p = (Models.Product.Supplier)obj;

            // ADD ADDRESS DETAILS AND REMOVE EXTANT EDAREPLACEMENT VERSION

            Code = p.Key?.TrimReduceTruncate(50);
            Name = p.Name?.TrimReduceTruncate(100);
            AddressLine1 = p.AddressLine1?.TrimReduceTruncate(60);
            AddressLine2 = p.AddressLine2?.TrimReduceTruncate(60);
            Town = p.Town?.TrimReduceTruncate(50);
            County = p.County?.TrimReduceTruncate(40);
            Postcode = p.Postcode?.TrimReduceTruncate(20);
            CountryCode = p.CountryCode?.TrimReduceTruncate(3);
            Currency = p.Currency?.TrimReduceTruncate(10);
            DutyPercentage = p.DutyPercentage ?? SqlDecimal.Null;
            InlandChargesPercentage = p.InlandChargesPercentage ?? SqlDecimal.Null;
            MiscellaneousPercentage = p.MiscellaneousPercentage ?? SqlDecimal.Null;
            ImportChargePerUnit = p.ImportChargePerUnit ?? SqlDecimal.Null;
            MiscellaneousCharge = p.MiscellaneousCharge ?? SqlDecimal.Null;
            PhoneNumber = p.PhoneNumber?.TrimReduceTruncate(20);
            EmailAddress = p.Email?.TrimReduceTruncate(254);
        }
    }
}
