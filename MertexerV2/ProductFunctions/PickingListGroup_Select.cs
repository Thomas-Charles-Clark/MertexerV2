using MertexerV2.GeneralFunctions;
using MertexerV2.Models.EDAReplacement;
using MertexerV2.Models.PickingList;
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
    public class PickingListGroupCode
    {
        public string Code { get; set; }
    }

    public static partial class PickingListGroupFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "PickingListGroupSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string PickingListGroupCodes)
        {
            if (String.IsNullOrEmpty(PickingListGroupCodes))
            {
                foreach (var q in Selector<PickingListGroup>.SelectAllNonParallel(
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
                var results = Selector<PickingListGroup>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<PickingListGroupCode>>(PickingListGroupCodes).Select(p => p.Code),
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

        public static void PickingListGroupSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name
            )
        {
            var p = (PickingListGroup)obj;

            Code = p.Key?.TrimReduceTruncate(1);
            Name = p.Name?.TrimReduceTruncate(50);
        }
    }
}