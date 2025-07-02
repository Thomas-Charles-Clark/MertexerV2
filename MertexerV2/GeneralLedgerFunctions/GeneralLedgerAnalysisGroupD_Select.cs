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
    public class GeneralLedgerAnalysisGroupDCode
    {
        public string Code { get; set; }
    }

    public static partial class GeneralLedgerAnalysisGroupDFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "GeneralLedgerAnalysisGroupD_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string GeneralLedgerAnalysisGroupDCodes)
        {
            if (String.IsNullOrEmpty(GeneralLedgerAnalysisGroupDCodes))
            {
                foreach (var r in Selector<GeneralLedgerAnalysisCodeD>.SelectAllNonParallel(
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
                var results = Selector<GeneralLedgerAnalysisCodeD>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<GeneralLedgerAnalysisGroupDCode>>(GeneralLedgerAnalysisGroupDCodes).Select(p => p.Code),
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

        public static void GeneralLedgerAnalysisGroupD_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name)
        {
            var result = (GeneralLedgerAnalysisCodeD)obj;

            Code = result.Key.TrimReduceTruncate(20);
            Name = result.Name.TrimReduceTruncate(100);
        }
    }
}
