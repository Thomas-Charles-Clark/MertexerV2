using MertexerV2.GeneralFunctions;
using MertexerV2.Models.EDAReplacement;
using MertexerV2.Models.Pricing;
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
    public class POCostCode
    {
        public string Code { get; set; }
    }

    public static partial class POCostFunctions    
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "POCostSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string POCostCodes)
        {
            if (String.IsNullOrEmpty(POCostCodes))
            {
                foreach (var q in Selector<POCost>.SelectAllNonParallel(
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
                var results = Selector<POCost>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<POCostCode>>(POCostCodes).Select(p => p.Code),
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

        public static void POCostSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Quality,
            out SqlString Country,
            out SqlString MethodOfTransport,
            out SqlDecimal DutyPercentage,
            out SqlDecimal InlandCharges,
            out SqlDecimal PackagingCharges,
            out SqlDecimal FreightCharges,
            out SqlDecimal QuotaLevy,
            out SqlString SupplierReference
            )
        {
            var p = (POCost)obj;

            Code = p.Key?.TrimReduceTruncate(10);
            Quality = p.Quality?.TrimReduceTruncate(50);
            Country = p.Country?.TrimReduceTruncate(2);
            MethodOfTransport = p.MethodOfTransport?.TrimReduceTruncate(20);
            DutyPercentage = p.DutyPercentage ?? SqlDecimal.Null;
            InlandCharges = p.InlandCharges ?? SqlDecimal.Null;
            PackagingCharges = p.PackagingCharges ?? SqlDecimal.Null;
            FreightCharges = p.FreightCharges ?? SqlDecimal.Null;
            QuotaLevy = p.QuotaLevy ?? SqlDecimal.Null;
            SupplierReference = p.SupplierReference?.TrimReduceTruncate(20);
        }
    }
}
