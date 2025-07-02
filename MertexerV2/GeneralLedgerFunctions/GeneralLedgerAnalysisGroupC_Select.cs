using MertexerV2.GeneralFunctions;
using MertexerV2.Models;
using MertexerV2.Models.Bank;
using MertexerV2.Models.GeneralLedger;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace MertexerV2.GeneralLedgerFunctions
{
    public class GeneralLedgerAnalysisGroupCCode
    {
        public string Code { get; set; }
    }

    public static partial class GeneralLedgerAnalysisGroupCFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "GeneralLedgerAnalysisGroupC_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string GeneralLedgerAnalysisGroupCCodes)
        {
            if (String.IsNullOrEmpty(GeneralLedgerAnalysisGroupCCodes))
            {
                foreach (var r in Selector<GeneralLedgerAnalysisCodeC>.SelectAllNonParallel(
                    true,
                    false,
                    50,
                    UniVerseAccount))
                {
                    yield return r;
                }
            }
            else
            {
                var results = Selector<GeneralLedgerAnalysisCodeC>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<GeneralLedgerAnalysisGroupCCode>>(GeneralLedgerAnalysisGroupCCodes).Select(p => p.Code),
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

        public static void GeneralLedgerAnalysisGroupC_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name)
        {
            var result = (GeneralLedgerAnalysisCodeC)obj;

            Code = result.Key.TrimReduceTruncate(20);
            Name = result.Name.TrimReduceTruncate(100);
        }
    }
}
