using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MertexerV2.GeneralFunctions;
using MertexerV2.Models.General;
using MertexerV2.Models.Pricing;
using MertexerV2.Models.SalesLedger;
using MertexerV2.PricingFunctions;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

namespace MertexerV2.SalesLedgerFunctions
{
    public class SalesAreaCode
    {
        public string Code { get; set; }
    }

    public static partial class SalesAreaFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "SalesAreaSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(
            string UniVerseAccount,
            string SalesAreaCodes)
        {
            if (String.IsNullOrEmpty(SalesAreaCodes))
            {
                foreach (var invoice in Selector<SalesArea>.SelectAllNonParallel(
                    true,
                    false,
                    100,
                    UniVerseAccount)
                    .ToList())
                {
                    yield return invoice;
                }
            }
            else
            {
                foreach (var invoice in Selector<SalesArea>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<SalesAreaCode>>(SalesAreaCodes).Select(p => p.Code),
                    true,
                    false,
                    100,
                    UniVerseAccount)
                    .ToList())
                {
                    yield return invoice;
                }
            }
        }

        public static void SalesAreaSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name
            )
        {
            var result = (SalesArea)obj;

            Code = result.Key?.TrimReduceTruncate(50);
            Name = result.Name?.TrimReduceTruncate(100) ?? SqlString.Null;
        }
    }
}
