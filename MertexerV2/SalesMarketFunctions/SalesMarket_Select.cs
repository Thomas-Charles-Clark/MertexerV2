using MertexerV2.GeneralFunctions;
using MertexerV2.Models;
using MertexerV2.Models.General;
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

namespace MertexerV2.SalesMarketFunctions
{
    public class SalesMarketCode
    {
        public string Code { get; set; }
    }

    public static partial class SalesMarketFunctions
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "SalesMarketSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string SalesMarketCodes)
        {
            if (String.IsNullOrEmpty(SalesMarketCodes))
            {
                foreach (var q in Selector<SalesMarket>.SelectAllNonParallel(
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
                var results = Selector<SalesMarket>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<SalesMarketCode>>(SalesMarketCodes).Select(p => p.Code),
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

        public static void SalesMarketSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name
            )
        {
            var p = (SalesMarket)obj;

            Code = p.Key?.TrimReduceTruncate(1);
            Name = p.Name?.TrimReduceTruncate(50);
        }
    }
}
