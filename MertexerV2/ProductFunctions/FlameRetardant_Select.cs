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
    public class FlameRetardantCode
    {
        public string Code { get; set; }
    }

    public static partial class FlameRetardantFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "FlameRetardantSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string FlameRetardantDescriptionCodes)
        {
            if (String.IsNullOrEmpty(FlameRetardantDescriptionCodes))
            {
                foreach (var q in Selector<FlameRetardant>.SelectAllNonParallel(
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
                var results = Selector<FlameRetardant>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<FlameRetardantCode>>(FlameRetardantDescriptionCodes).Select(p => p.Code),
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

        public static void FlameRetardantSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name
            )
        {
            var p = (FlameRetardant)obj;

            Code = p.Key?.TrimReduceTruncate(10);
            Name = p.Name?.TrimReduceTruncate(50);
        }
    }
}