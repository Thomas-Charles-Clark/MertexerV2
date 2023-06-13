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
    public static partial class SSLPostContactFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "SSLPostContactSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string TradeAccountCode, string SSLPostContactCodes)
        {
            if (String.IsNullOrEmpty(TradeAccountCode))
            {
                if (String.IsNullOrEmpty(SSLPostContactCodes))
                {
                    foreach (var r in Selector<SSLPostContact>.SelectAllNonParallel(
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
                    var results = Selector<SSLPostContact>.Select(
                        JsonConvert.DeserializeObject<IEnumerable<SSLPostContactKey>>(SSLPostContactCodes).Select(p => p.Key),
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
            else
            {
                if (String.IsNullOrEmpty(SSLPostContactCodes))
                {
                    foreach (var r in Selector<SSLPostContact>.Filter(
                        new[]
                        {
                            new Tuple<string, string, string>("CUSREF", "=", TradeAccountCode),
                        },
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
                    foreach (var r in Selector<SSLPostContact>.Filter(
                        new[]
                        {
                            new Tuple<string, string, string>("CUSREF", "=", TradeAccountCode),
                            new Tuple<string, string, string>("@ID", "=", $"{String.Join(" ", JsonConvert.DeserializeObject<IEnumerable<SSLPostContactKey>>(SSLPostContactCodes).Select(p => p.Key))}")
                        },
                        true,
                        false,
                        50,
                        UniVerseAccount))
                    {
                        yield return r;
                    }
                }
            }
        }

        public static void SSLPostContactSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString DocumentType,
            out SqlString TradeAccountCode,
            out SqlString Email)
        {
            {
                var result = (SSLPostContact)obj;

                Code = result.Key?.TrimReduceTruncate(60);
                DocumentType = result.DocumentType.TrimReduceTruncate(10);
                TradeAccountCode = result.TradeAccountCode.TrimReduceTruncate(50);
                Email = result.Email?.TrimReduceTruncate(254);
            }
        }
    }
}