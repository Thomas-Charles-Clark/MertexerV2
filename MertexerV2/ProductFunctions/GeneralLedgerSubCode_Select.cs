using MertexerV2.GeneralFunctions;
using MertexerV2.Models.EDAReplacement;
using MertexerV2.Models.Pricing;
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
    public class GLSubCodeCode
    {
        public string Code { get; set; }
    }

    public static partial class GeneralLedgerSubFunctions    
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "GeneralLedgerSubSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAnalysis, string GeneralLedgerSubCodes)
        {
            if (String.IsNullOrEmpty(GeneralLedgerSubCodes))
            {
                foreach (var q in Selector<GeneralLedgerSubCode>.SelectAllNonParallel(
                   true,
                   false,
                   50,
                   UniVerseAnalysis))
                {
                    yield return q;
                }
            }
            else
            {
                var results = Selector<GeneralLedgerSubCode>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<GLSubCodeCode>>(GeneralLedgerSubCodes).Select(p => p.Code),
                    true,
                    false,
                    50,
                    UniVerseAnalysis)
                    .ToList();

                foreach (var r in results)
                {
                    yield return r;
                }
            }
        }

        public static void GeneralLedgerSubSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name
            )
        {
            var p = (GeneralLedgerSubCode)obj;

            Code = p.Key?.TrimReduceTruncate(2);
            Name = p.Name?.TrimReduceTruncate(50);
        }
    }
}
