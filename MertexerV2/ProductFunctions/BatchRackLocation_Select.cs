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
    public class BatchRackLocationCode
    {
        public string Code { get; set; }
    }

    public static partial class BatchRackLocationFunctions    
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "BatchRackLocationSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string BatchRackLocationCodes)
        {
            if (String.IsNullOrEmpty(BatchRackLocationCodes))
            {
                foreach (var q in Selector<BatchRackLocation>.SelectAllNonParallel(
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
                var results = Selector<BatchRackLocation>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<BatchRackLocationCode>>(BatchRackLocationCodes).Select(p => p.Code),
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

        public static void BatchRackLocationSelect_FillRow(
            object obj,
            out SqlString Code
            )
        {
            var p = (BatchRackLocation)obj;

            Code = p.Key;
        }
    }
}
