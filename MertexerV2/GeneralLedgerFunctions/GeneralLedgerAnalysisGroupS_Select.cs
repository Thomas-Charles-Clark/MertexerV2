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
    public class GeneralLedgerAnalysisGroupSCode
    {
        public string Code { get; set; }
    }

    public static partial class GeneralLedgerAnalysisGroupSFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "GeneralLedgerAnalysisGroupS_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string GeneralLedgerAnalysisGroupSCodes)
        {
            if (String.IsNullOrEmpty(GeneralLedgerAnalysisGroupSCodes))
            {
                foreach (var r in Selector<GeneralLedgerAnalysisCodeS>.SelectAllNonParallel(
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
                var results = Selector<GeneralLedgerAnalysisCodeS>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<GeneralLedgerAnalysisGroupSCode>>(GeneralLedgerAnalysisGroupSCodes).Select(p => p.Code),
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

        public static void GeneralLedgerAnalysisGroupS_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name)
        {
            var result = (GeneralLedgerAnalysisCodeS)obj;

            Code = result.Key.TrimReduceTruncate(20);
            Name = result.Name.TrimReduceTruncate(100);
        }
    }
}
