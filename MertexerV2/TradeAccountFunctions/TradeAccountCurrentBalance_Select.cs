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
            FillRowMethodName = "TradeAccountCurrentBalanceSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable SelectCurrentBalance(string UniVerseAccount, string TradeAccountCodes)
        {
            if (String.IsNullOrEmpty(TradeAccountCodes))
            {
                foreach (var r in Selector<TradeAccountCurrentBalance>.SelectAllNonParallel(
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
                var results = Selector<TradeAccountCurrentBalance>.Select(
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

        public static void TradeAccountCurrentBalanceSelect_FillRow(
            object obj,
            out SqlString TradeAccountCode,
            out SqlString CustomerDetails,
            out SqlString AccountBalances,
            out SqlDecimal OutstandingOrdersValue,
            out SqlInt32 OutstandingOrdersCount,
            out SqlDecimal StatementOrdersValue,
            out SqlDecimal OutstandingReservesValue,
            out SqlDecimal ReserveStatementOrdersValue)
        {
            var result = (TradeAccountCurrentBalance)obj;

            if (result == null)
            {
                result = new TradeAccountCurrentBalance(null);
            }

            TradeAccountCode = result.Key?.TrimReduceTruncate(50);

            CustomerDetails = result.CustomerDetails.Any() ? JsonConvert.SerializeObject(result.CustomerDetails) : SqlString.Null;

            AccountBalances = result.AccountBalances.Any() ? JsonConvert.SerializeObject(result.AccountBalances) : SqlString.Null;

            OutstandingOrdersValue = result.OutstandingOrdersValue;

            OutstandingOrdersCount = result.OutstandingOrdersCount;

            StatementOrdersValue = result.StatementOrdersValue;

            OutstandingReservesValue = result.OutstandingReservesValue;

            ReserveStatementOrdersValue = result.ReserveStatementOrdersValue;
        }
    }
}
