using MertexerV2.GeneralFunctions;
using MertexerV2.Models.EDAReplacement;
using MertexerV2.Models.Pricing;
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

namespace MertexerV2.ProductFunctions
{
    public class GLCodeListCodeCode
    {
        public string Code { get; set; }
    }

    public static partial class GeneralLedgerCodeListFunctions    
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "GeneralLedgerCodeListSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseCodeList, string GeneralLedgerCodeListCodes)
        {
            if (String.IsNullOrEmpty(GeneralLedgerCodeListCodes))
            {
                foreach (var q in Selector<GeneralLedgerCodeList>.SelectAllNonParallel(
                   true,
                   false,
                   50,
                   UniVerseCodeList))
                {
                    yield return q;
                }
            }
            else
            {
                var results = Selector<GeneralLedgerCodeList>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<GLCodeListCodeCode>>(GeneralLedgerCodeListCodes).Select(p => p.Code),
                    true,
                    false,
                    50,
                    UniVerseCodeList)
                    .ToList();

                foreach (var r in results)
                {
                    yield return r;
                }
            }
        }

        public static void GeneralLedgerCodeListSelect_FillRow(
            object obj,
            out SqlInt32 Code,
            out SqlString Name
            )
        {
            var p = (GeneralLedgerCodeList)obj;

            Code = Convert.ToInt16(p.Key);
            Name = p.Name?.TrimReduceTruncate(50);
        }
    }
}
