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
    public class BankCode
    {
        public string Code { get; set; }
    }

    public static partial class BankFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "BankSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string BankCodes)
        {
            if (String.IsNullOrEmpty(BankCodes))
            {
                foreach (var r in Selector<Bank>.SelectAllNonParallel(
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
                var results = Selector<Bank>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<BankCode>>(BankCodes).Select(p => p.Code),
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

        public static void BankSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name,
            out SqlString DescriptionLines)
        {
            var result = (Bank)obj;

            Code = result.Key;
            Name = result.Name;
            DescriptionLines = result.DescriptionLines?.Any() == true ? JsonConvert.SerializeObject(result.DescriptionLines) : SqlString.Null;
        }
    }
}
