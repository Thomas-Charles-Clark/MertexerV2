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
    public class GeneralLedgerPrePaymentTransactionCode
    {
        public string Code { get; set; }
    }

    public static partial class GeneralLedgerPrePaymentTransactionFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "GeneralLedgerPrePaymentTransactionSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string GeneralLedgerPrePaymentTransactionCodes)
        {
            if (String.IsNullOrEmpty(GeneralLedgerPrePaymentTransactionCodes))
            {
                foreach (var r in Selector<GeneralLedgerPrePaymentTransaction>.SelectAllNonParallel(
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
                var results = Selector<GeneralLedgerPrePaymentTransaction>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<GeneralLedgerPrePaymentTransactionCode>>(GeneralLedgerPrePaymentTransactionCodes).Select(p => p.Code),
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

        public static void GeneralLedgerPrePaymentTransactionSelect_FillRow(
            object obj,
            out SqlString Code)
        {
            var result = (GeneralLedgerPrePaymentTransaction)obj;

            Code = result.Key.TrimReduceTruncate(20);
        }
    }
}
