using MertexerV2.Models;
using MertexerV2.Models.General;
using MertexerV2.Models.Product;
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

namespace MertexerV2.GeneralFunctions
{
    public class ReasonCode
    {
        public string Code { get; set; }
    }

    public static partial class ReasonFunctions
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "ReasonSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string ReasonCodes)
        {
            if (String.IsNullOrEmpty(ReasonCodes))
            {
                foreach (var q in Selector<Reason>.SelectAllNonParallel(
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
                var results = Selector<Reason>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<ReasonCode>>(ReasonCodes).Select(p => p.Code),
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

        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "ReasonSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Search(string UniVerseAccount, string SearchTerm, int? SampleSize)
        {
            SearchTerm = SearchTerm?.ToLowerInvariant();

            var filters = new List<Tuple<string, string, string>>
            {
                new Tuple<string, string, string>($"(SS.REASON.MCL LIKE \"...{SearchTerm}...\")", "", "")
            };

            var results = Selector<Reason>.Filter(
                filters,
                true,
                false,
                50,
                UniVerseAccount,
                SampleSize)
                .ToList();

            foreach (var r in results)
            {
                yield return r;
            }
        }

        public static void ReasonSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name
            )
        {
            var p = (Reason)obj;

            Code = p.Key?.TrimReduceTruncate(5);
            Name = p.Name?.TrimReduceTruncate(200);

            int f = 3;
        }
    }
}
