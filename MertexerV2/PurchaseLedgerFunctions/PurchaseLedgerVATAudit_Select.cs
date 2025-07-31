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
    public class PurchaseLedgerVATAuditCode
    {
        public string Code { get; set; }
    }

    public static partial class PurchaseLedgerVATAuditFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "PurchaseLedgerVATAuditSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(
            string UniVerseAccount,
            string PurchaseLedgerCodes,
            string SupplierCode,
            DateTime? DocumentDT_From,
            DateTime? DocumentDT_To)
        {
            if (String.IsNullOrEmpty(PurchaseLedgerCodes))
            {
                int p = 0;

                var filters = new List<Tuple<string, string, string>>();

                if (!String.IsNullOrEmpty(SupplierCode))
                {
                    filters.Add(new Tuple<string, string, string>("SUPREF", "eq", SupplierCode));
                }

                if (DocumentDT_From.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DOCDATE.INT", ">=", Framework.UniVerseDateString(DocumentDT_From.Value)));
                }

                if (DocumentDT_To.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DOCDATE.INT", "<=", Framework.UniVerseDateString(DocumentDT_To.Value)));
                }

                if (filters.Any())
                {
                    foreach (var invoice in Selector<PurchaseLedgerVATAudit>.FilterNonParallel(
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
                    foreach (var invoice in Selector<PurchaseLedgerVATAudit>.SelectAllNonParallel(
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
                foreach (var invoice in Selector<PurchaseLedgerVATAudit>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<PurchaseLedgerVATAuditCode>>(PurchaseLedgerCodes).Select(p => p.Code),
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

        public static void PurchaseLedgerVATAuditSelect_FillRow(
            object obj,
            out SqlString Key,
            out SqlDateTime DocumentDT
            )
        {
            var result = (PurchaseLedgerVATAudit)obj;

            Key = result.Key?.TrimReduceTruncate(50);

            DocumentDT = result.DocumentDT ?? SqlDateTime.Null;
        }
    }
}
