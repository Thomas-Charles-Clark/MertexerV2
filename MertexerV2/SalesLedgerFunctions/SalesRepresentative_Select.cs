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
    public class SalesRepresentativeCode
    {
        public string Code { get; set; }
    }

    public static partial class SalesRepresentativeFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "SalesRepresentativeSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(
            string UniVerseAccount,
            string SalesRepresentativeCodes)
        {
            if (String.IsNullOrEmpty(SalesRepresentativeCodes))
            {
                foreach (var invoice in Selector<SalesRepresentative>.SelectAllNonParallel(
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
                foreach (var invoice in Selector<SalesRepresentative>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<SalesRepresentativeCode>>(SalesRepresentativeCodes).Select(p => p.Code),
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

        public static void SalesRepresentativeSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name
            )
        {
            var result = (SalesRepresentative)obj;

            Code = result.Key?.TrimReduceTruncate(10);
            Name = result.Name?.TrimReduceTruncate(100) ?? SqlString.Null;
        }
    }
}
