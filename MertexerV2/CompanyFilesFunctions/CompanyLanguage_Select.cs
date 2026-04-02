using MertexerV2.GeneralFunctions;
using MertexerV2.Models.Company;
using MertexerV2.Models.CompanyFiles;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

using Supplier = MertexerV2.Models.Supplier.Supplier;

namespace MertexerV2.CompanyFileFunctions
{
    public class CompanyLanguageCode
    {
        public string Code { get; set; }
    }

    public static partial class CompanyLanguageFunctions
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "CompanyLanguageSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string LanguageCodes)
        {
            if (String.IsNullOrEmpty(LanguageCodes))
            {
                foreach (var q in Selector<CompanyLanguage>.SelectAllNonParallel(
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
                var results = Selector<CompanyLanguage>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<CompanyLanguageCode>>(LanguageCodes).Select(p => p.Code.ToLowerInvariant()),
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

        public static void CompanyLanguageSelect_FillRow(
            object obj,
            out SqlString Key,
            out SqlString Name,
            out SqlString MertexLanguageCode

            )
        {
            var p = (CompanyLanguage)obj;

            Key = p.Key;

            Name = p.Name;

            MertexLanguageCode = p.MertexLanguageCode;
        }
    }
}
