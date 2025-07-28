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
    public class SalesAgentCode
    {
        public string Code { get; set; }
    }

    public static partial class SalesAgentFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "SalesAgentSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(
            string UniVerseAccount,
            string SalesAgentCodes)
        {
            if (String.IsNullOrEmpty(SalesAgentCodes))
            {
                foreach (var invoice in Selector<SalesAgent>.SelectAllNonParallel(
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
                foreach (var invoice in Selector<SalesAgent>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<SalesAgentCode>>(SalesAgentCodes).Select(p => p.Code),
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

        public static void SalesAgentSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name
            )
        {
            var result = (SalesAgent)obj;

            Code = result.Key?.TrimReduceTruncate(10);
            Name = result.Name?.TrimReduceTruncate(100) ?? SqlString.Null;
        }
    }
}
