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
    public class ProductDeveloperCode
    {
        public string Code { get; set; }
    }

    public static partial class ProductDeveloperFunctions
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "ProductDeveloperSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string ProductDeveloperCodes)
        {
            if (String.IsNullOrEmpty(ProductDeveloperCodes))
            {
                foreach (var q in Selector<ProductDeveloper>.SelectAllNonParallel(
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
                var results = Selector<ProductDeveloper>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<ProductDeveloperCode>>(ProductDeveloperCodes).Select(p => p.Code),
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

        public static void ProductDeveloperSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name
            )
        {
            var p = (ProductDeveloper)obj;

            Code = p.Key?.TrimReduceTruncate(20);
            Name = p.Name?.TrimReduceTruncate(50);
        }
    }
}