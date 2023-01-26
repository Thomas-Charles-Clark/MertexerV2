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
    public class ProductTemplateCode
    {
        public string Code { get; set; }
    }

    public static partial class ProductTemplateFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "ProductTemplateSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string ProductTemplateCodes)
        {
            if (String.IsNullOrEmpty(ProductTemplateCodes))
            {
                foreach (var q in Selector<ProductTemplate>.SelectAllNonParallel(
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
                var results = Selector<ProductTemplate>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<ProductTemplateCode>>(ProductTemplateCodes).Select(p => p.Code),
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

        public static void ProductTemplateSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Description
            )
        {
            var p = (ProductTemplate)obj;

            Code = p.Key?.TrimReduceTruncate(50);
            Description = p.Description?.TrimReduceTruncate(50);
        }
    }
}