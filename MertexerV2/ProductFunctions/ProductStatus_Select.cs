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

namespace MertexerV2.ProductFunctions
{
    public class ProductStatusCode
    {
        public string Code { get; set; }
    }

    public static partial class ProductStatusFunctions
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "ProductStatusSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string ProductStatusCodes)
        {
            if (String.IsNullOrEmpty(ProductStatusCodes))
            {
                foreach (var q in Selector<ProductStatus>.SelectAllNonParallel(
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
                var results = Selector<ProductStatus>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<ProductStatusCode>>(ProductStatusCodes).Select(p => p.Code),
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


        public static void ProductStatusSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name
            )
        {
            var p = (ProductStatus)obj;

            Code = p.Key?.TrimReduceTruncate(1);
            Name = p.Name?.TrimReduceTruncate(50);
        }
    }
}
