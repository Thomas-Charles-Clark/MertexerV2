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
    public class RackGroupCode
    {
        public string Code { get; set; }
    }

    public static partial class RackGroupFunctions    
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "RackGroupSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string RackGroupCodes)
        {
            if (String.IsNullOrEmpty(RackGroupCodes))
            {
                foreach (var q in Selector<RackGroup>.SelectAllNonParallel(
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
                var results = Selector<RackGroup>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<RackGroupCode>>(RackGroupCodes).Select(p => p.Code),
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

        public static void RackGroupSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString PickedBy
            )
        {
            var p = (RackGroup)obj;

            Code = p.Key?.TrimReduceTruncate(5);
            PickedBy = p.PickedBy?.TrimReduceTruncate(10);
        }
    }
}
