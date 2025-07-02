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
    public class GeneralLedgerGroupCode
    {
        public string Code { get; set; }
    }

    public static partial class GeneralLedgerGroupFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "GeneralLedgerGroupSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string GeneralLedgerAccountTypeCodes)
        {
            if (String.IsNullOrEmpty(GeneralLedgerAccountTypeCodes))
            {
                foreach (var r in Selector<GeneralLedgerGroup>.SelectAllNonParallel(
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
                var results = Selector<GeneralLedgerGroup>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<GeneralLedgerGroupCode>>(GeneralLedgerAccountTypeCodes).Select(p => p.Code),
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

        public static void GeneralLedgerGroupSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name)
        {
            var result = (GeneralLedgerGroup)obj;

            Code = result.Key.TrimReduceTruncate(20);
            Name = result.Name.TrimReduceTruncate(100);
        }
    }
}
