using MertexerV2.GeneralFunctions;
using MertexerV2.Models.Pricing;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace MertexerV2.PricingFunctions
{
    public class FreeOfChargeAccountingCodeCode
    {
        public string Code { get; set; }
    }

    public static partial class FreeOfChargeAccountingCodeFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "FreeOfChargeAccountingCodeSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string FreeOfChargeAccountingCodeCodes)
        {
            if (String.IsNullOrEmpty(FreeOfChargeAccountingCodeCodes))
            {
                foreach (var r in Selector<FreeOfChargeAccountingCode>.SelectAllNonParallel(
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
                var results = Selector<FreeOfChargeAccountingCode>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<FreeOfChargeAccountingCodeCode>>(FreeOfChargeAccountingCodeCodes).Select(p => p.Code),
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

        public static void FreeOfChargeAccountingCodeSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name)
        {
            var result = (FreeOfChargeAccountingCode)obj;

            Code = result.Key.TrimReduceTruncate(10);
            Name = result.Name.TrimReduceTruncate(50);
        }
    }
}
