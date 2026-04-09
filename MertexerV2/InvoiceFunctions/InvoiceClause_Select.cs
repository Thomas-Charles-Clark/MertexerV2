using MertexerV2.GeneralFunctions;
using MertexerV2.Models;
using MertexerV2.Models.Bank;
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
    public class InvoiceClauseCode
    {
        public string Code { get; set; }
    }

    public static partial class InvoiceClauseFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "InvoiceClauseSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string InvoiceClauseCodes)
        {
            if (String.IsNullOrEmpty(InvoiceClauseCodes))
            {
                foreach (var r in Selector<InvoiceClause>.SelectAllNonParallel(
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
                var results = Selector<InvoiceClause>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<InvoiceClauseCode>>(InvoiceClauseCodes).Select(p => p.Code),
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

        public static void InvoiceClauseSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Description,
            out SqlString ClauseLines,
            out SqlString AppliesToDocuments,
            out SqlString OrderEntryReferences
            )
        {
            var result = (InvoiceClause)obj;

            Code = result.Key;
            Description = result.Description;
			ClauseLines = result.ClauseLines?.Any() == true ? JsonConvert.SerializeObject(result.ClauseLines) : SqlString.Null;
            AppliesToDocuments = result.AppliesToDocuments?.Any() == true ? JsonConvert.SerializeObject(result.AppliesToDocuments) : SqlString.Null;
            OrderEntryReferences = result.OrderEntryReferences?.Any() == true ? JsonConvert.SerializeObject(result.OrderEntryReferences) : SqlString.Null;
        }
    }
}
