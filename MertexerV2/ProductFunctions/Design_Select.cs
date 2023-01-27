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
    public class DesignCode
    {
        public string Code { get; set; }
    }

    public static partial class DesignFunctions    
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "DesignSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string DesignCodes)
        {
            if (String.IsNullOrEmpty(DesignCodes))
            {
                foreach (var q in Selector<Design>.SelectAllNonParallel(
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
                var results = Selector<Design>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<DesignCode>>(DesignCodes).Select(p => p.Code),
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

        public static void DesignSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name,
            out SqlInt32 SortOrder
            )
        {
            var p = (Design)obj;

            Code = p.Key?.TrimReduceTruncate(10);
            Name = p.Name?.TrimReduceTruncate(30);
            SortOrder = p.SortOrder ?? SqlInt32.Null;
        }
    }
}
