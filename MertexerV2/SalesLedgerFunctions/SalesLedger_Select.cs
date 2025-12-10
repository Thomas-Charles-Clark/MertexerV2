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
    public class SalesLedgerCode
    {
        public string Code { get; set; }
    }

    public static partial class SalesLedgerFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "SalesLedgerSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(
            string UniVerseAccount,
            string SalesLedgerCodes,
			DateTime? PostDT_From,
			DateTime? PostDT_To)
        {
            if (String.IsNullOrEmpty(SalesLedgerCodes))
			{
				var filters = new List<Tuple<string, string, string>>();

				if (PostDT_From.HasValue)
				{
					filters.Add(new Tuple<string, string, string>("SS.POSTDATE.INT", ">=", Framework.UniVerseDateString(PostDT_From.Value)));
				}

				if (PostDT_To.HasValue)
				{
					filters.Add(new Tuple<string, string, string>("SS.POSTDATE.INT", "<=", Framework.UniVerseDateString(PostDT_To.Value)));
				}

				if (filters.Any())
				{
					foreach (var invoice in Selector<SalesLedger>.FilterNonParallel(
						filters,
						true,
						false,
						500,
						UniVerseAccount))
					{
						yield return invoice;
					}
				}
				else
				{
					foreach (var invoice in Selector<SalesLedger>.SelectAllNonParallel(
						true,
						false,
						500,
						UniVerseAccount))
					{
						yield return invoice;
					}
				}
			}
            else
            {
                foreach (var invoice in Selector<SalesLedger>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<SalesLedgerCode>>(SalesLedgerCodes).Select(p => p.Code),
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

        public static void SalesLedgerSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Memo,
            out SqlDateTime PostDT
            )
        {
            var result = (SalesLedger)obj;

            int p = 0;

            Code = result.Key;

            Memo = result.Memo;

            PostDT = result.PostDT ?? SqlDateTime.Null;
        }
    }
}
