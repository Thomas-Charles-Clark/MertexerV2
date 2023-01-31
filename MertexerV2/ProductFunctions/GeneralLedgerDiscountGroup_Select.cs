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
    public class GeneralLedgerDiscountGroupCode
    {
        public string Code { get; set; }
    }

    public static partial class GeneralLedgerDiscountGroupFunctions    
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "GeneralLedgerDiscountGroupSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string GeneralLedgerDiscountCodeGroupCodes)
        {
            if (String.IsNullOrEmpty(GeneralLedgerDiscountCodeGroupCodes))
            {
                foreach (var q in Selector<GeneralLedgerDiscountGroup>.SelectAllNonParallel(
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
                var results = Selector<GeneralLedgerDiscountGroup>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<GeneralLedgerDiscountGroupCode>>(GeneralLedgerDiscountCodeGroupCodes).Select(p => p.Code),
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

        public static void GeneralLedgerDiscountGroupSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name
            )
        {
            var p = (GeneralLedgerDiscountGroup)obj;

            Code = p.Key?.TrimReduceTruncate(20);
            Name = p.Name?.TrimReduceTruncate(50);
        }
    }
}
