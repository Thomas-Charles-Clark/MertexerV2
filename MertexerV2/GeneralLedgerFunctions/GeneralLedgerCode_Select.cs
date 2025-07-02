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
    public class GeneralLedgerCodeCode
    {
        public string Code { get; set; }
    }

    public static partial class GeneralLedgerFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "GeneralLedgerSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string GeneralLedgerCodes)
        {
            if (String.IsNullOrEmpty(GeneralLedgerCodes))
            {
                foreach (var r in Selector<GeneralLedgerCode>.SelectAllNonParallel(
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
                var results = Selector<GeneralLedgerCode>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<GeneralLedgerCodeCode>>(GeneralLedgerCodes).Select(p => p.Code),
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

        public static void GeneralLedgerSelect_FillRow(
            object obj,
            out SqlString GeneralLedgerCode,
            out SqlString ReportingGroup,
            out SqlString AccountType,
            out SqlString AnalysisCodeA,
            out SqlString AnalysisCodeD,
            out SqlString AnalysisCodeC,
            out SqlString AnalysisCodeS,
            out SqlString AnalysisCodeDescription)
        {
            var result = (GeneralLedgerCode)obj;

            GeneralLedgerCode = result.Key.TrimReduceTruncate(50);
            ReportingGroup = result.ReportingGroup.TrimReduceTruncate(50);
            AccountType = String.IsNullOrEmpty(result.AccountType) ? SqlString.Null : result.AccountType.TrimReduceTruncate(10);
            AnalysisCodeA = result.AccountCode.TrimReduceTruncate(20); 
            AnalysisCodeD = result.AnalysisCodeD.TrimReduceTruncate(10);
            AnalysisCodeC = result.AnalysisCodeC.TrimReduceTruncate(10);
            AnalysisCodeS = result.AnalysisCodeS.TrimReduceTruncate(10);
            AnalysisCodeDescription = result.AnalysisCodeDescription.TrimReduceTruncate(255);
        }
    }
}
