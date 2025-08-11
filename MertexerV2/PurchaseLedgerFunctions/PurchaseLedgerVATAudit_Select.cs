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
            string PurchaseLedgerVATAuditCodes,
            string SupplierCode,
            DateTime? DocumentDT_From,
            DateTime? DocumentDT_To)
        {
            if (String.IsNullOrEmpty(PurchaseLedgerVATAuditCodes))
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
                        500,
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
                        500,
                        UniVerseAccount))
                    {
                        yield return invoice;
                    }
                }
            }
            else
            {
                foreach (var invoice in Selector<PurchaseLedgerVATAudit>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<PurchaseLedgerVATAuditCode>>(PurchaseLedgerVATAuditCodes).Select(p => p.Code),
                    true,
                    false,
                    500,
                    UniVerseAccount))
                {
                    yield return invoice;
                }
            }
        }

        public static void PurchaseLedgerVATAuditSelect_FillRow(
            object obj,
            out SqlString PurchaseLedgerVATAuditID,
            out SqlDateTime DocumentDT,
            out SqlString TransactionType,
            out SqlString SupplierCode,
            out SqlDecimal PurchaseLedgerTotalDueInSterling,
            out SqlString VATCodes,
            out SqlString VATableValuesInSterling,
            out SqlString VATTaxesInSterling,
            out SqlBoolean NextPeriod,
            out SqlString MertexCurrencyCode,
            out SqlString VATableValuesInCurrency,
            out SqlString VATTaxesInCurrency,
            out SqlDecimal PurchaseLedgerTotalDueInCurrency,
            out SqlDateTime CopiedDT,
            out SqlByte FinancialPeriod
            )
        {
            var result = (PurchaseLedgerVATAudit)obj;

            PurchaseLedgerVATAuditID = result.Key;

            DocumentDT = result.DocumentDT ?? SqlDateTime.Null;

            TransactionType = result.TransactionType ?? SqlString.Null;

            SupplierCode = result.SupplierCode ?? SqlString.Null;

            PurchaseLedgerTotalDueInSterling = result.PurchaseLedgerTotalDueInSterling;

            VATCodes = result.VATCodes?.Any() == true ? JsonConvert.SerializeObject(result.VATCodes) : SqlString.Null;

            VATableValuesInSterling = result.VATableValuesInSterling?.Any() == true ? JsonConvert.SerializeObject(result.VATableValuesInSterling) : SqlString.Null;

            VATTaxesInSterling = result.VATTaxesInSterling?.Any() == true ? JsonConvert.SerializeObject(result.VATTaxesInSterling) : SqlString.Null;

            NextPeriod = result.NextPeriod;

            MertexCurrencyCode = result.CurrencyCode ?? SqlString.Null;

            VATableValuesInCurrency = result.VATableValuesInCurrency?.Any() == true ? JsonConvert.SerializeObject(result.VATableValuesInCurrency) : SqlString.Null;

            VATTaxesInCurrency = result.VATTaxesInCurrency?.Any() == true ? JsonConvert.SerializeObject(result.VATTaxesInCurrency) : SqlString.Null;

            PurchaseLedgerTotalDueInCurrency = result.PurchaseLedgerTotalDueInCurrency;

            CopiedDT = result.CopiedDT ?? SqlDateTime.Null;

            FinancialPeriod = result.FinancialPeriod ?? SqlByte.Null;
        }
    }
}
