using MertexerV2.GeneralFunctions;
using MertexerV2.Models;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace MertexerV2.TradeAccountFunctions
{
    public static partial class TradeAccountGroupFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "TradeAccountGroupSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string TradeAccountGroupCodes)
        {
            if (String.IsNullOrEmpty(TradeAccountGroupCodes))
            {
                foreach (var r in Selector<TradeAccountClass>.SelectAllNonParallel(
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
                var results = Selector<TradeAccountClass>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<TradeAccountClassCode>>(TradeAccountGroupCodes).Select(p => p.Code),
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

        public static void TradeAccountGroupSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name)
        {
            var result = (TradeAccountClass)obj;

            Code = result.Key.TrimReduceTruncate(10);
            Name = result.Name.TrimReduceTruncate(50);
        }
    }
}
