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
    public class PurchaseLedgerSupplierInvoiceCode
    {
        public string Code { get; set; }
    }

    public static partial class PurchaseLedgerSupplierInvoiceFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "PurchaseLedgerSupplierInvoiceSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(
            string UniVerseAccount,
            string PurchaseLedgerSupplierInvoiceCodes,
            string SupplierCode,
            DateTime? PostDT_From,
            DateTime? PostDT_To,
            DateTime? DocumentDT_From,
            DateTime? DocumentDT_To)
        {
            if (String.IsNullOrEmpty(PurchaseLedgerSupplierInvoiceCodes))
            {
                int p = 0;

                var filters = new List<Tuple<string, string, string>>();

                if (!String.IsNullOrEmpty(SupplierCode))
                {
                    filters.Add(new Tuple<string, string, string>("SUPREF", "eq", SupplierCode));
                }

                if (PostDT_From.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.POSTDATE.INT", ">=", Framework.UniVerseDateString(PostDT_From.Value)));
                }

                if (PostDT_To.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.POSTDATE.INT", "<=", Framework.UniVerseDateString(PostDT_To.Value)));
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
                    foreach (var invoice in Selector<PurchaseLedgerSupplierInvoice>.FilterNonParallel(
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
                    foreach (var invoice in Selector<PurchaseLedgerSupplierInvoice>.SelectAllNonParallel(
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
                foreach (var invoice in Selector<PurchaseLedgerSupplierInvoice>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<PurchaseLedgerSupplierInvoiceCode>>(PurchaseLedgerSupplierInvoiceCodes).Select(p => p.Code),
                    true,
                    false,
                    500,
                    UniVerseAccount))
                {
                    yield return invoice;
                }
            }
        }

        public static void PurchaseLedgerSupplierInvoiceSelect_FillRow(
            object obj,
            out SqlString PurchaseLedgerSupplierInvoiceID,
            out SqlString SupplierCode,
            out SqlString SupplierInvoiceReference,
            out SqlString TransactionType,
            out SqlDateTime DocumentDT,
            out SqlString DocumentNumber,
            out SqlDateTime PostDT
            )
        {
            var result = (PurchaseLedgerSupplierInvoice)obj;

            PurchaseLedgerSupplierInvoiceID = result.Key;

            SupplierCode = result.SupplierCode ?? SqlString.Null;

            SupplierInvoiceReference = result.SupplierInvoiceReference ?? SqlString.Null;

            TransactionType = result.TransactionType ?? SqlString.Null;

            DocumentDT = result.DocumentDT ?? SqlDateTime.Null;

            DocumentNumber = result.DocumentNumber ?? SqlString.Null;

            PostDT = result.PostDT ?? SqlDateTime.Null;
        }
    }
}
