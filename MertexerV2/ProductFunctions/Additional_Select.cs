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
    public class AdditionalCode
    {
        public string Code { get; set; }
    }

    public static partial class AdditionalFunctions    
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "AdditionalSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string AdditionalCodes)
        {
            if (String.IsNullOrEmpty(AdditionalCodes))
            {
                foreach (var q in Selector<Additional>.SelectAllNonParallel(
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
                var results = Selector<Additional>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<AdditionalCode>>(AdditionalCodes).Select(p => p.Code),
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

        public static void AdditionalSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Detail,
            out SqlString BackCoating,
            out SqlString BackCoatingRequired,
            out SqlString LabelDescription
            )
        {
            var p = (Additional)obj;

            Code = p.Key?.TrimReduceTruncate(10);
            Detail = p.Detail?.TrimReduceTruncate(100);
            BackCoating = p.BackCoating?.TrimReduceTruncate(50);
            BackCoatingRequired = p.BackCoatingRequired?.TrimReduceTruncate(1);
            LabelDescription = p.LabelDescription?.TrimReduceTruncate(100);
        }
    }
}
