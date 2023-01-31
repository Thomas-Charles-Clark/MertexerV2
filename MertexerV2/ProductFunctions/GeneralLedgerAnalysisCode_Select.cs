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
    public class GLAnalysisCodeCode
    {
        public string Code { get; set; }
    }

    public static partial class GeneralLedgerAnalysisCodeFunctions    
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "GeneralLedgerAnalysisCodeSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAnalysis, string GeneralLedgerAnalysisCodes)
        {
            if (String.IsNullOrEmpty(GeneralLedgerAnalysisCodes))
            {
                foreach (var q in Selector<Models.Pricing.GeneralLedgerAnalysisCode>.SelectAllNonParallel(
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
                var results = Selector<GeneralLedgerAnalysisCode>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<GLAnalysisCodeCode>>(GeneralLedgerAnalysisCodes).Select(p => p.Code),
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

        public static void GeneralLedgerAnalysisCodeSelect_FillRow(
            object obj,
            out SqlInt32 Code,
            out SqlString Name
            )
        {
            var p = (GeneralLedgerAnalysisCode)obj;

            Code = Convert.ToInt32(p.Key);
            Name = p.Name?.TrimReduceTruncate(50);
        }
    }
}
