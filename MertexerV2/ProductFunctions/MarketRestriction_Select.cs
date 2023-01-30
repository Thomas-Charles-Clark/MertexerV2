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
    public class MarketRestrictionCode
    {
        public string Code { get; set; }
    }

    public static partial class MarketRestrictionFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "MarketRestrictionSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string MarketRestrictionCodes)
        {
            if (String.IsNullOrEmpty(MarketRestrictionCodes))
            {
                foreach (var q in Selector<MarketRestriction>.SelectAllNonParallel(
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
                var results = Selector<MarketRestriction>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<MarketRestrictionCode>>(MarketRestrictionCodes).Select(p => p.Code),
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

        public static void MarketRestrictionSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name
            )
        {
            var p = (MarketRestriction)obj;

            Code = p.Key?.TrimReduceTruncate(5);
            Name = p.Name?.TrimReduceTruncate(50);
        }
    }
}