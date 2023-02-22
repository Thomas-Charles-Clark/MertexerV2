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
    public class ProductExclusionCode
    {
        public string Code { get; set; }
    }

    public static partial class ProductExclusionFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "ProductExclusionSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string ProductExclusionCodes)
        {
            if (String.IsNullOrEmpty(ProductExclusionCodes))
            {
                foreach(var q in Selector<ProductExclusion>.SelectAllNonParallel(
                   true,
                   false,
                   50,
                   UniVerseAccount)
                   .SelectMany(r => r.ProductsExcluded.Select(px => new ProductExclusionDerivation()
                   {
                       Key = r.Key,
                       ProductCode = px
                   })))
                {
                    yield return q;
                }
            }
            else
            {
                var results = Selector<ProductExclusion>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<ProductExclusionCode>>(ProductExclusionCodes).Select(p => p.Code),
                    true,
                    false,
                    50,
                    UniVerseAccount)
                    .SelectMany(r => r.ProductsExcluded.Select(px => new ProductExclusionDerivation() 
                    { 
                        Key = r.Key,
                        ProductCode = px
                    }))
                    .ToList();

                foreach (var r in results)
                {
                    yield return r;
                }
            }
        }

        public static void ProductExclusionSelect_FillRow(
            object obj,
            out SqlString Key,
            out SqlString ProductCode
            )
        {
            var p = (ProductExclusionDerivation)obj;

            Key = p.Key?.TrimReduceTruncate(50);
            ProductCode = p.ProductCode?.TrimReduceTruncate(50);
        }
    }
}