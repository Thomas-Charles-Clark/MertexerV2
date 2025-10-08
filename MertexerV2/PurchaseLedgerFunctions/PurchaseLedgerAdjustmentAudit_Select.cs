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
    public class PurchaseLedgerAdjustmentAuditCode
    {
        public string Code { get; set; }
    }

    public static partial class PurchaseLedgerAdjustmentAuditFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "PurchaseLedgerAdjustmentAuditSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(
            string UniVerseAccount,
            string PurchaseLedgerAdjustmentAuditCodes,
            string SupplierCode,
            DateTime? PostDT_From,
            DateTime? PostDT_To,
            DateTime? DocumentDT_From,
            DateTime? DocumentDT_To)
        {
            if (String.IsNullOrEmpty(PurchaseLedgerAdjustmentAuditCodes))
            {
                int p = 0;

                var filters = new List<Tuple<string, string, string>>();

                if (!String.IsNullOrEmpty(SupplierCode))
                {
                    filters.Add(new Tuple<string, string, string>("SUPREF", "eq", SupplierCode));
                }

                if (PostDT_From.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.POSTDAT.INT", ">=", Framework.UniVerseDateString(PostDT_From.Value)));
                }

                if (PostDT_To.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.POSTDAT.INT", "<=", Framework.UniVerseDateString(PostDT_To.Value)));
                }

                if (DocumentDT_From.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DOCDAT.INT", ">=", Framework.UniVerseDateString(DocumentDT_From.Value)));
                }

                if (DocumentDT_To.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DOCDAT.INT", "<=", Framework.UniVerseDateString(DocumentDT_To.Value)));
                }

                if (filters.Any())
                {
                    foreach (var invoice in Selector<PurchaseLedgerAdjustmentAudit>.FilterNonParallel(
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
                    foreach (var invoice in Selector<PurchaseLedgerAdjustmentAudit>.SelectAllNonParallel(
                        true,
                        false,
                        100,
                        UniVerseAccount))
                    {
                        yield return invoice;
                    }
                }
            }
            else
            {
                foreach (var invoice in Selector<PurchaseLedgerAdjustmentAudit>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<PurchaseLedgerAdjustmentAuditCode>>(PurchaseLedgerAdjustmentAuditCodes).Select(p => p.Code),
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

        public static void PurchaseLedgerAdjustmentAuditSelect_FillRow(
            object obj,
            out SqlString PurchaseLedgerAdjustmentAuditID,
            out SqlDecimal DocumentTotal,
            out SqlString TransactionType,
            out SqlString Ports,
            out SqlDateTime DocumentDT,
            out SqlDateTime PostDT,
            out SqlByte Period,
            out SqlDecimal VATTaxesInSterling,
            out SqlString SupplierCode,
            out SqlString UnallocatedAmounts,
            out SqlString Balances,
            out SqlString GeneralLedgerCodes,
            out SqlString Time,
            out SqlDateTime DueDT,
            out SqlString MertexCurrencyCode,
            out SqlDecimal DocumentTotalInCurrency,
            out SqlDecimal VATTaxesTotalInCurrency,
            out SqlString ExchangeRates,
            out SqlDecimal ExchangeRate
            )
        {
            var result = (PurchaseLedgerAdjustmentAudit)obj;

            PurchaseLedgerAdjustmentAuditID = result.Key;

            DocumentTotal = result.DocumentTotal;

            TransactionType = result.TransactionType ?? SqlString.Null;

            Ports = result.Ports?.Any() == true ? JsonConvert.SerializeObject(result.Ports) : SqlString.Null;

            DocumentDT = result.DocumentDT ?? SqlDateTime.Null;

            PostDT = result.PostDT ?? SqlDateTime.Null;

            Period = result.FinancialPeriod;

            VATTaxesInSterling = result.VATTaxesTotalInSterling;

            SupplierCode = result.SupplierCode ?? SqlString.Null;

            UnallocatedAmounts = result.UnallocatedAmounts?.Any() == true ? JsonConvert.SerializeObject(result.UnallocatedAmounts) : SqlString.Null;

            Balances = result.Balances?.Any() == true ? JsonConvert.SerializeObject(result.Balances) : SqlString.Null;

            GeneralLedgerCodes = result.GeneralLedgerCodes?.Any() == true ? JsonConvert.SerializeObject(result.GeneralLedgerCodes) : SqlString.Null;

            Time = result.Time ?? SqlString.Null;

            DueDT = result.DueDT ?? SqlDateTime.Null;

            MertexCurrencyCode = result.CurrencyCode ?? SqlString.Null;

            DocumentTotalInCurrency = result.DocumentTotalInCurrency;

            VATTaxesTotalInCurrency = result.VATTaxesTotalInCurrency;

            ExchangeRates = result.ExchangeRates?.Any() == true ? JsonConvert.SerializeObject(result.ExchangeRates) : SqlString.Null;

            ExchangeRate = result.ExchangeRate;
        }
    }
}
