using MertexerV2.GeneralFunctions;
using MertexerV2.Models.Company;
using MertexerV2.Models.EDAReplacement;
using MertexerV2.Models.Product;
using MertexerV2.Models.PurchaseLedger;
using MertexerV2.Models.SalesAnalysis;
using MertexerV2.Models.Supplier;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Supplier = MertexerV2.Models.Supplier.Supplier;

namespace MertexerV2.CompanyFileFunctions
{
    public class CompanyStatsCode
    {
        public string Code { get; set; }
    }

    public static partial class CompanyStatsFunctions
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "CompanyStatsSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string StatsCodes)
        {
            if (String.IsNullOrEmpty(StatsCodes))
            {
                foreach (var q in Selector<CompanyStats>.SelectAllNonParallel(
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
                var results = Selector<CompanyStats>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<CompanyStatsCode>>(StatsCodes).Select(p => p.Code),
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

        public static void CompanyStatsSelect_FillRow(
            object obj,
            out SqlString Code
            )
        {
            var p = (CompanyStats)obj;

            // ADD ADDRESS DETAILS AND REMOVE EXTANT EDAREPLACEMENT VERSION

            Code = p.Key?.TrimReduceTruncate(2);

        }
    }
}
