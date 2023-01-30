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
    public class GeneralLedgerDiscountCodeGroupCode
    {
        public string Code { get; set; }
    }

    public static partial class GeneralLedgerDiscountCodeGroupFunctions    
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "GeneralLedgerDiscountCodeGroupSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string GeneralLedgerDiscountCodeGroupCodes)
        {
            if (String.IsNullOrEmpty(GeneralLedgerDiscountCodeGroupCodes))
            {
                foreach (var q in Selector<GeneralLedgerDiscountCodeGroup>.SelectAllNonParallel(
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
                var results = Selector<GeneralLedgerDiscountCodeGroup>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<GeneralLedgerDiscountCodeGroupCode>>(GeneralLedgerDiscountCodeGroupCodes).Select(p => p.Code),
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

        public static void GeneralLedgerDiscountCodeGroupSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name
            )
        {
            var p = (GeneralLedgerDiscountCodeGroup)obj;

            Code = p.Key?.TrimReduceTruncate(10);
            Name = p.Name?.TrimReduceTruncate(50);
        }
    }
}
