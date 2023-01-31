using MertexerV2.GeneralFunctions;
using MertexerV2.Models.EDAReplacement;
using MertexerV2.Models.Pricing;
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
    public class VATCodeCode
    {
        public string Code { get; set; }
    }

    public static partial class VATCodeFunctions    
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "VATCodeSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string VATCodeCodes)
        {
            if (String.IsNullOrEmpty(VATCodeCodes))
            {
                foreach (var q in Selector<VATCode>.SelectAllNonParallel(
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
                var results = Selector<VATCode>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<VATCodeCode>>(VATCodeCodes).Select(p => p.Code),
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

        public static void VATCodeSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name,
            out SqlDecimal Rate,
            out SqlString ReportIndicator,
            out SqlByte SortOrder
            )
        {
            var p = (VATCode)obj;

            Code = p.Key?.TrimReduceTruncate(10);
            Name = p.Name?.TrimReduceTruncate(50);
            Rate = p.Rate ?? SqlDecimal.Null;
            ReportIndicator = p.ReportIndicator?.TrimReduceTruncate(1);
            SortOrder = p.SortOrder ?? SqlByte.Null;
        }
    }
}
