using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MertexerV2.GeneralFunctions;
using MertexerV2.Models.Pricing;
using MertexerV2.Models.PurchaseLedger;
using MertexerV2.PricingFunctions;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

namespace MertexerV2.PurchaseLedgerFunctions
{
    public static partial class PurchaseLedgerRegisterHistoryFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "PurchaseLedgerRegisterHistorySelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(
            string UniVerseAccount,
            string PurchaseLedgerRegisterHistoryCodes)
        {
            throw new NotImplementedException();

            if (String.IsNullOrEmpty(PurchaseLedgerRegisterHistoryCodes))
            {
                int p = 0;

                var filters = new List<Tuple<string, string, string>>();

                //if (!String.IsNullOrEmpty(SupplierCode))
                //{
                //    filters.Add(new Tuple<string, string, string>("SUPREF", "eq", SupplierCode));
                //}

                //if (DocumentDT_From.HasValue)
                //{
                //    filters.Add(new Tuple<string, string, string>("SS.DOCDATE.INT", ">=", Framework.UniVerseDateString(DocumentDT_From.Value)));
                //}

                //if (DocumentDT_To.HasValue)
                //{
                //    filters.Add(new Tuple<string, string, string>("SS.DOCDATE.INT", "<=", Framework.UniVerseDateString(DocumentDT_To.Value)));
                //}

                //if (DueDT_From.HasValue)
                //{
                //    filters.Add(new Tuple<string, string, string>("SS.DUEDATE.INT", ">=", Framework.UniVerseDateString(DueDT_From.Value)));
                //}

                //if (DueDT_To.HasValue)
                //{
                //    filters.Add(new Tuple<string, string, string>("SS.DUEDATE.INT", "<=", Framework.UniVerseDateString(DueDT_To.Value)));
                //}

                //if (PostDT_From.HasValue)
                //{
                //    filters.Add(new Tuple<string, string, string>("SS.POSTDATE.INT", ">=", Framework.UniVerseDateString(PostDT_From.Value)));
                //}

                //if (PostDT_To.HasValue)
                //{
                //    filters.Add(new Tuple<string, string, string>("SS.POSTDATE.INT", "<=", Framework.UniVerseDateString(PostDT_To.Value)));
                //}

                if (filters.Any())
                {
                    foreach (var invoice in Selector<PurchaseLedgerTransaction>.FilterNonParallel(
                        filters,
                        true,
                        false,
                        100,
                        UniVerseAccount))
                    {
                        yield return invoice;
                    }
                }
                else
                {
                    foreach (var invoice in Selector<PurchaseLedgerTransaction>.SelectAllNonParallel(
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
            else
            {
                foreach (var invoice in Selector<PurchaseLedgerTransaction>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<PurchaseLedgerTransactionCode>>(PurchaseLedgerRegisterHistoryCodes).Select(p => p.Code),
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

        public static void PurchaseLedgerRegisterHistorySelect_FillRow(
            object obj,
            out SqlString Key
            )
        {
            var result = (PurchaseLedgerTransaction)obj;

            Key = result.Key?.TrimReduceTruncate(50);

            throw new NotImplementedException();
        }
    }
}
