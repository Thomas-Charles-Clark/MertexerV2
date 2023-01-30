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
    public class PatternMatchCode
    {
        public string Code { get; set; }
    }

    public static partial class PatternMatchFunctions    
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "PatternMatchSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string PatternMatchCodes)
        {
            if (String.IsNullOrEmpty(PatternMatchCodes))
            {
                foreach (var q in Selector<PatternMatch>.SelectAllNonParallel(
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
                var results = Selector<PatternMatch>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<PatternMatchCode>>(PatternMatchCodes).Select(p => p.Code),
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

        public static void PatternMatchSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name,
            out SqlString Full,
            out SqlString Half,
            out SqlString Straight,
            out SqlString Match,
            out SqlString Random
            )
        {
            var p = (Models.Product.PatternMatch)obj;

            Code = p.Key?.TrimReduceTruncate(5);
            Name = p.Name?.TrimReduceTruncate(50);
            Full = p.Full?.TrimReduceTruncate(50);
            Half = p.Half?.TrimReduceTruncate(50);
            Straight = p.Straight?.TrimReduceTruncate(50);
            Match = p.Match?.TrimReduceTruncate(50);
            Random = p.Random?.TrimReduceTruncate(50);
        }
    }
}
