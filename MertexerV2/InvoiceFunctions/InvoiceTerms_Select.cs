using MertexerV2.GeneralFunctions;
using MertexerV2.Models;
using MertexerV2.Models.Invoices;
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
    public class InvoiceTermsCode
    {
        public string Code { get; set; }
    }

    public static partial class InvoiceTermsFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "InvoiceTermsSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string InvoiceTermsCodes)
        {
            if (String.IsNullOrEmpty(InvoiceTermsCodes))
            {
                foreach (var r in Selector<InvoiceTerms>.SelectAllNonParallel(
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
                var results = Selector<InvoiceTerms>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<InvoiceTermsCode>>(InvoiceTermsCodes).Select(p => p.Code),
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

        public static void InvoiceTermsSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name)
        {
            var result = (InvoiceTerms)obj;

            Code = result.Key.TrimReduceTruncate(10);
            Name = result.Name.TrimReduceTruncate(50);
        }
    }
}
