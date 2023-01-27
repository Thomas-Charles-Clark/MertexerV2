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
    public class PatternBookCode
    {
        public string Code { get; set; }
    }

    public static partial class PatternBookFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "PatternBookSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string PatternBookCodes)
        {
            if (String.IsNullOrEmpty(PatternBookCodes))
            {
                foreach (var q in Selector<PatternBook>.SelectAllNonParallel(
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
                var results = Selector<PatternBook>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<PatternBookCode>>(PatternBookCodes).Select(p => p.Code),
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

        public static void PatternBookSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString SortOrders,
            out SqlString FabricCodes
            )
        {
            var p = (PatternBook)obj;

            Code = p.Key?.TrimReduceTruncate(20);
            SortOrders = p.SortOrders.Any() ? JsonConvert.SerializeObject(p.SortOrders) : SqlString.Null;
            FabricCodes = p.FabricCodes.Any() ? JsonConvert.SerializeObject(p.FabricCodes) : SqlString.Null;
        }
    }
}