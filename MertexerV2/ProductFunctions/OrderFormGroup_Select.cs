using MertexerV2.GeneralFunctions;
using MertexerV2.Models.EDAReplacement;
using MertexerV2.Models.Product;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MertexerV2.ProductFunctions
{
    public class OrderFormGroupCode
    {
        public string Code { get; set; }
    }

    public static partial class OrderFormGroupFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "OrderFormGroupSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string OrderFormGroupCodes)
        {
            if (String.IsNullOrEmpty(OrderFormGroupCodes))
            {
                foreach (var q in Selector<OrderFormGroup>.SelectAllNonParallel(
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
                var results = Selector<OrderFormGroup>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<OrderFormGroupCode>>(OrderFormGroupCodes).Select(p => p.Code),
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

        public static void OrderFormGroupSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name,
            out SqlInt32 SortOrder
            )
        {
            var p = (OrderFormGroup)obj;

            Code = p.Key?.TrimReduceTruncate(10);
            Name = p.Name?.TrimReduceTruncate(50);
            SortOrder = p.SortOrder ?? SqlInt32.Null;
        }
    }
}