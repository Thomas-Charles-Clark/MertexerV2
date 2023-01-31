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
    public class BrandCode
    {
        public string Code { get; set; }
    }

    public static partial class BrandFunctions    
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "BrandSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string BrandCodes)
        {
            if (String.IsNullOrEmpty(BrandCodes))
            {
                foreach (var q in Selector<Brand>.SelectAllNonParallel(
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
                var results = Selector<Brand>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<BrandCode>>(BrandCodes).Select(p => p.Code),
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

        public static void BrandSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name,
            out SqlByte SortOrder
            )
        {
            var p = (Brand)obj;

            Code = p.Key?.TrimReduceTruncate(10);
            Name = p.Name?.TrimReduceTruncate(50);
            SortOrder = p.SortOrder ?? SqlByte.Null;
        }
    }
}
