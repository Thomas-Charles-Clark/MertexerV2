using MertexerV2.Models;
using MertexerV2.Models.General;
using MertexerV2.Models.Accounts;
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
using MertexerV2.GeneralFunctions;
using MertexerV2.Models.ExchangeRate;

namespace MertexerV2.AccountFunctions
{
    public class ExchangeRateCode
    {
        public string Code { get; set; }
    }

    public static partial class ExchangeRateFunctions
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "ExchangeRateSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string ExchangeRateCodes)
        {
            if (String.IsNullOrEmpty(ExchangeRateCodes))
            {
                foreach (var q in Selector<ExchangeRate>.SelectAllNonParallel(
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
                var results = Selector<ExchangeRate>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<ExchangeRateCode>>(ExchangeRateCodes).Select(p => p.Code),
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

        public static void ExchangeRateSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name,
            out SqlDecimal Rate
            )
        {
            var p = (ExchangeRate)obj;

            Code = p.Key?.TrimReduceTruncate(5);
            Name = p.Description?.TrimReduceTruncate(50);
            Rate = p.Rate;
        }
    }
}
