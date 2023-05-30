using MertexerV2.GeneralFunctions;
using MertexerV2.Models.Pricing;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace MertexerV2.PricingFunctions
{
    public class SalesDivisionCode
    {
        public string Code { get; set; }
    }

    public static partial class SalesDivisionFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "SalesDivisionSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string SalesDivisionCodes)
        {
            if (String.IsNullOrEmpty(SalesDivisionCodes))
            {
                foreach (var r in Selector<SalesDivision>.SelectAllNonParallel(
                    true,
                    false,
                    50,
                    UniVerseAccount))
                {
                    yield return r;
                }
            }
            else
            {
                var results = Selector<SalesDivision>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<SalesDivisionCode>>(SalesDivisionCodes).Select(p => p.Code),
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

        public static void SalesDivisionSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name)
        {
            var result = (SalesDivision)obj;

            Code = result.Key.TrimReduceTruncate(10);
            Name = result.Name.TrimReduceTruncate(50);
        }
    }
}
