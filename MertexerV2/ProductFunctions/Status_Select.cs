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
    public class StatusCode
    {
        public string Code { get; set; }
    }

    public static partial class StatusFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "StatusSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string ProductStatusCodes)
        {
            if (String.IsNullOrEmpty(ProductStatusCodes))
            {
                foreach (var q in Selector<Status>.SelectAllNonParallel(
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
                var results = Selector<Status>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<StatusCode>>(ProductStatusCodes).Select(p => p.Code),
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

        public static void StatusSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name
            )
        {
            var p = (Status)obj;

            Code = p.Key?.TrimReduceTruncate(1);
            Name = p.Name?.TrimReduceTruncate(50);
        }
    }
}