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
    public class BatchCode
    {
        public string Code { get; set; }
    }

    public static partial class BatchFunctions    
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "BatchSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string BatchCodes, DateTime? ReceivedDT_From, DateTime? ReceivedDT_To)
        {
            if (String.IsNullOrEmpty(BatchCodes))
			{
				var filters = new List<Tuple<string, string, string>>();

				if (ReceivedDT_From.HasValue)
				{
					filters.Add(new Tuple<string, string, string>("SS.RECEIPT.DATE.INT", ">=", Framework.UniVerseDateString(ReceivedDT_From.Value)));
				}

				if (ReceivedDT_To.HasValue)
				{
					filters.Add(new Tuple<string, string, string>("SS.RECEIPT.DATE.INT", "<=", Framework.UniVerseDateString(ReceivedDT_To.Value)));
				}

                if (filters.Any())
				{
					foreach (var q in Selector<Batch>.FilterNonParallel(
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
					foreach (var q in Selector<Batch>.SelectAllNonParallel(
					   true,
					   false,
					   500,
					   UniVerseAccount))
					{
						yield return q;
					}
				}
            }
            else
            {
                var results = Selector<Batch>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<BatchCode>>(BatchCodes).Select(p => p.Code),
                    true,
                    false,
                    500,
                    UniVerseAccount);

                foreach (var r in results)
                {
                    yield return r;
                }
            }
        }

        public static void BatchSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString ProductCode,
            out SqlString BatchNumber,
            out SqlString Racks,
            out SqlString StockLocationCodes,
            out SqlString RacksAndLocationsCombined,
            out SqlString ProductLocationBatchKeys,
            out SqlDecimal CurrentUnitValue,
			out SqlDateTime ReceivedDT,
			out SqlString RackQuantityPhysicalJSON,
            out SqlString RackQuantityDemandsJSON,
            out SqlString RackQuantityFreeJSON
			)
        {
            var p = (Batch)obj;

            Code = p.Key;
            ProductCode = p.ProductCode;
            BatchNumber = p.BatchNumber;
            Racks = p.Racks?.Any() == true ? JsonConvert.SerializeObject(p.Racks) : SqlString.Null;
            StockLocationCodes = p.StockLocationCodes?.Any() == true ? JsonConvert.SerializeObject(p.StockLocationCodes) : SqlString.Null; ;
            RacksAndLocationsCombined = p.RacksAndLocationCombined?.Any() == true ? JsonConvert.SerializeObject(p.RacksAndLocationCombined) : SqlString.Null;
			ProductLocationBatchKeys = p.ProductLocationBatchKeys?.Any() == true ? JsonConvert.SerializeObject(p.ProductLocationBatchKeys) : SqlString.Null;
			CurrentUnitValue = p.CurrentCostValue;
			ReceivedDT = p.ReceivedDT ?? SqlDateTime.Null;
			RackQuantityPhysicalJSON = p.RackQuantityPhysical?.Any() == true ? JsonConvert.SerializeObject(p.RackQuantityPhysical) : SqlString.Null;
            RackQuantityDemandsJSON = p.RackQuantityDemands?.Any() == true ? JsonConvert.SerializeObject(p.RackQuantityDemands) : SqlString.Null;
			RackQuantityFreeJSON = p.RackQuantityFree?.Any() == true ? JsonConvert.SerializeObject(p.RackQuantityFree) : SqlString.Null;
        }
    }
}
