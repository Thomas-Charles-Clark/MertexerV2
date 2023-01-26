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
    public class PatternCode
    {
        public string Code { get; set; }
    }

    public static partial class PatternFunctions    
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "PatternSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string PatternCodes)
        {
            if (String.IsNullOrEmpty(PatternCodes))
            {
                foreach (var q in Selector<Pattern>.SelectAllNonParallel(
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
                var results = Selector<Pattern>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<PatternCode>>(PatternCodes).Select(p => p.Code),
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

        public static void PatternSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Description
            )
        {
            var p = (Models.Product.Pattern)obj;

            Code = p.Key?.TrimReduceTruncate(50);
            Description = p.Description?.TrimReduceTruncate(50);
        }
    }
}
