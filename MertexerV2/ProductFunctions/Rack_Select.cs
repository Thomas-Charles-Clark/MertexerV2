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
    public class RackCode
    {
        public string Code { get; set; }
    }

    public static partial class RackFunctions    
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "RackSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string RackCodes)
        {
            if (String.IsNullOrEmpty(RackCodes))
            {
                foreach (var q in Selector<Rack>.SelectAllNonParallel(
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
                var results = Selector<Rack>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<RackCode>>(RackCodes).Select(p => p.Code),
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

        public static void RackSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name,
            out SqlString WarehouseRoute,
            out SqlBoolean Pickable,
            out SqlString Type
            )
        {
            var p = (Rack)obj;

            Code = p.Key?.TrimReduceTruncate(10);
            Name = p.Name?.TrimReduceTruncate(50);
            WarehouseRoute = p.WarehouseRoute?.TrimReduceTruncate(50);
            Pickable = p.Pickable;
            Type = p.Type?.TrimReduceTruncate(1);
        }
    }
}
