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
    public class StockValuationCode
    {
        public string Code { get; set; }
    }

    public static partial class StockValuationFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "StockValuationSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string StockValuationCodes)
        {
            if (String.IsNullOrEmpty(StockValuationCodes))
            {
                foreach (var r in Selector<StockValuation>.SelectAllNonParallel(
                    true,
                    false,
                    100,
                    UniVerseAccount))
                {
                    yield return r;
                }
            }
            else
            {
                var results = Selector<StockValuation>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<StockValuationCode>>(StockValuationCodes).Select(p => p.Code),
                    true,
                    false,
                    100,
                    UniVerseAccount);

                foreach (var r in results)
                {
                    yield return r;
                }
            }
        }

        public static void StockValuationSelect_FillRow(
            object obj,
            out SqlString StockValuationID,
            out SqlString StockLocationCode,
            out SqlString ProductCode,
            out SqlDecimal TotalPhysicalStockQuantity,
            out SqlDecimal TotalPhysicalStockValue)
        {
            var result = (StockValuation)obj;

            StockValuationID = result.Key;

            StockLocationCode = result.StockLocationCode ?? SqlString.Null;

            ProductCode = result.ProductCode ?? SqlString.Null;

            TotalPhysicalStockQuantity = result.TotalPhysicalStockQuantity;

            TotalPhysicalStockValue = result.TotalPhysicalStockValue;
        }
    }
}
