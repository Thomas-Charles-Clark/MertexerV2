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
    public static partial class SSLPostDocumentFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "SSLPostDocumentSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string SSLPostDocumentCodes)
        {
            if (String.IsNullOrEmpty(SSLPostDocumentCodes))
            {
                foreach (var r in Selector<SSLPostDocument>.SelectAllNonParallel(
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
                var results = Selector<SSLPostDocument>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<SSLPostDocumentCode>>(SSLPostDocumentCodes).Select(p => p.Code),
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

        public static void SSLPostDocumentSelect_FillRow(
            object obj,
            out SqlString DocumentType,
            out SqlString Reference,
            out SqlString Email)
        {
            var result = (SSLPostDocument)obj;

            DocumentType = result.DocumentType.TrimReduceTruncate(10);
            Reference = result.Reference.TrimReduceTruncate(50);
            Email = result.Email?.TrimReduceTruncate(254);
        }
    }
}
