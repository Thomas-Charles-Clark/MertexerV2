using MertexerV2.GeneralFunctions;
using MertexerV2.Models.Accounts;
using MertexerV2.Models.Credit;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace MertexerV2.InvoiceFunctions
{
    public class CreditControllerCode
    {
        public string Code { get; set; }
    }

    public static partial class CreditControllerFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "CreditControllerSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string CreditControllerCodes)
        {
            if (String.IsNullOrEmpty(CreditControllerCodes))
            {
                foreach (var r in Selector<CreditController>.SelectAllNonParallel(
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
                var results = Selector<CreditController>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<CreditControllerCode>>(CreditControllerCodes).Select(p => p.Code),
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

        public static void CreditControllerSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name,
            out SqlString Email,
            out SqlString Region,
            out SqlString MarketCodes,
            out SqlString TradeAccountGroupCodes,
            out SqlString MertexLanguageCodes)
        {
            var result = (CreditController)obj;

            Code = result.Key.TrimReduceTruncate(10);
            Name = result.Name?.TrimReduceTruncate(100) ?? SqlString.Null;
            Email = result.Email?.TrimReduceTruncate(254) ?? SqlString.Null;
            Region = result.Region?.TrimReduceTruncate(200) ?? SqlString.Null;
            MarketCodes = result.MarketCodes.Any() ? JsonConvert.SerializeObject(result.MarketCodes) : SqlString.Null;
            TradeAccountGroupCodes = result.TradeAccountGroupCodes.Any() ? JsonConvert.SerializeObject(result.TradeAccountGroupCodes) : SqlString.Null;
            MertexLanguageCodes = result.MertexLanguageCodes.Any() ? JsonConvert.SerializeObject(result.MertexLanguageCodes) : SqlString.Null;
        }
    }
}
