using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MertexerV2.GeneralFunctions;
using MertexerV2.Models.Pricing;
using MertexerV2.Models.SalesLedger;
using MertexerV2.PricingFunctions;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

namespace MertexerV2.SalesLedgerFunctions
{
    public class SalesLedgerPositionCode
    {
        public string Code { get; set; }
    }

    public static partial class SalesLedgerPositionFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "SalesLedgerPositionSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(
            string UniVerseAccount,
            string SalesLedgerPositionCodes)
        {
            if (String.IsNullOrEmpty(SalesLedgerPositionCodes))
            {
                foreach (var invoice in Selector<SalesLedgerPosition>.SelectAllNonParallel(
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
                foreach (var invoice in Selector<SalesLedgerPosition>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<SalesLedgerPositionCode>>(SalesLedgerPositionCodes).Select(p => p.Code),
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

        public static void SalesLedgerPositionSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Description

			)
        {
            var result = (SalesLedgerPosition)obj;

            Code = result.Key;

            Description = result.Description ?? SqlString.Null;
           
        }
    }
}
