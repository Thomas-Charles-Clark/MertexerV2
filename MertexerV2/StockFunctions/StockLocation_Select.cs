using MertexerV2.GeneralFunctions;
using MertexerV2.Models.Stock;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace MertexerV2.StockFunctions
{
    public class StockLocationCode
    {
        public string Code { get; set; }
    }

    public static partial class StockLocationFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "StockLocationSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string StockLocationCodes)
        {
            if (String.IsNullOrEmpty(StockLocationCodes))
            {
                foreach (var r in Selector<StockLocation>.SelectAllNonParallel(
                    true,
                    false,
                    50,
                    UniVerseAccount))
                {
                    yield return r;
                }
            }
            else
            {
                var results = Selector<StockLocation>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<StockLocationCode>>(StockLocationCodes).Select(p => p.Code),
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

        public static void StockLocationSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name)
        {
            var result = (StockLocation)obj;

            Code = result.Key.TrimReduceTruncate(10);
            Name = result.Name.TrimReduceTruncate(50);
        }
    }
}
