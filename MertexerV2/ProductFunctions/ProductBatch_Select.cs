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
    public class ProductBatchCode
    {
        public string Code { get; set; }
    }

    public static partial class ProductBatchFunctions    
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "ProductBatchSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string ProductBatchCodes, DateTime? BatchDT_From, DateTime? BatchDT_To)
        {
            if (String.IsNullOrEmpty(ProductBatchCodes))
			{
				var filters = new List<Tuple<string, string, string>>();

				if (BatchDT_From.HasValue)
				{
					filters.Add(new Tuple<string, string, string>("SS.RECEIPT.DATE.INT", ">=", Framework.UniVerseDateString(BatchDT_From.Value)));
				}

				if (BatchDT_To.HasValue)
				{
					filters.Add(new Tuple<string, string, string>("SS.RECEIPT.DATE.INT", "<=", Framework.UniVerseDateString(BatchDT_To.Value)));
				}

				if (filters.Any())
				{
					foreach (var q in Selector<ProductBatch>.FilterNonParallel(
					   filters,
					   true,
					   false,
					   500,
					   UniVerseAccount))
					{
						yield return q;
					}
				}
				else
				{
					foreach (var q in Selector<ProductBatch>.SelectAllNonParallel(
					   true,
					   false,
					   50,
					   UniVerseAccount))
					{
						yield return q;
					}
				}
            }
            else
            {
                var results = Selector<ProductBatch>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<ProductBatchCode>>(ProductBatchCodes).Select(p => p.Code),
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

        public static void ProductBatchSelect_FillRow(
            object obj,
            out SqlString Key,
            out SqlString ProductCode,
			out SqlString StockLocationCode,
			out SqlString BatchNumber,
            out SqlDateTime BatchDT,
            out SqlDateTime LastReceiptDT,
            out SqlDateTime LastIssueDT,
            out SqlString Dummy,
            out SqlDecimal StockPhysical,
            out SqlDecimal StockDemands,
            out SqlDecimal StockFree,
            out SqlDecimal StockOnPurchaseOrders
			)
        {
            var p = (ProductBatch)obj;

            Key = p.Key;
            ProductCode = p.ProductCode;
            StockLocationCode = p.StockLocationCode;
            BatchNumber = p.BatchNumber;
            BatchDT = p.BatchDT ?? SqlDateTime.Null;
            LastReceiptDT = p.LastReceiptDT ?? SqlDateTime.Null;
            LastIssueDT = p.LastIssueDT ?? SqlDateTime.Null;
            Dummy = p.Dummy;
            StockPhysical = p.StockPhysical;
            StockDemands = p.StockDemands;
            StockFree = p.StockFree;
            StockOnPurchaseOrders = p.StockOnPurchaseOrders;
        }
    }
}
