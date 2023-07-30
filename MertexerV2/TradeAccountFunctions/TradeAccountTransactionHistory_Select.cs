using MertexerV2.GeneralFunctions;
using MertexerV2.Models;
using MertexerV2.Models.DataSource;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MertexerV2.TradeAccountFunctions
{
    public static partial class TradeAccountFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "TradeAccountTransactionHistorySelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable SelectTransactionHistory(string UniVerseAccount, string TradeAccountCodes)
        {
            if (String.IsNullOrEmpty(TradeAccountCodes))
            {
                foreach (var r in Selector<TradeAccountTransactionHistory>.SelectAllNonParallel(
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
                var results = Selector<TradeAccountTransactionHistory>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<TradeAccountCode>>(TradeAccountCodes).Select(p => p.Code),
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

        public static void TradeAccountTransactionHistorySelect_FillRow(
            object obj,
            out SqlString TradeAccountCode,
            out SqlString DocumentHistory,
            out SqlString MertexCurrencyCode)
        {
            var result = (TradeAccountTransactionHistory)obj;

            if (result == null)
            {
                result = new TradeAccountTransactionHistory(null);
            }

            TradeAccountCode = result.Key?.TrimReduceTruncate(50);

            DocumentHistory = result.DocumentHistory.Any() ? JsonConvert.SerializeObject(result.DocumentHistory) : SqlString.Null;

            MertexCurrencyCode = result.MertexCurrencyCode;
        }
    }
}
