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
    public class PurchaseLedgerInvoiceRegisterCode
    {
        public string Code { get; set; }
    }

    public static partial class PurchaseLedgerInvoiceRegisterFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "InvoiceRegisterSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(
            string UniVerseAccount,
            string InvoiceRegisterCodes,
            string SupplierCode,
            DateTime? DocumentDT_From,
            DateTime? DocumentDT_To,
            DateTime? DueDT_From,
            DateTime? DueDT_To,
            DateTime? OnFileDT_From,
            DateTime? OnFileDT_To)
        {
            if (String.IsNullOrEmpty(InvoiceRegisterCodes))
            {
                int p = 0;

                var filters = new List<Tuple<string, string, string>>();

                if (!String.IsNullOrEmpty(SupplierCode))
                {
                    filters.Add(new Tuple<string, string, string>("PSUPREF", "eq", SupplierCode));
                }

                if (DocumentDT_From.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DOCDATE.INT", ">=", Framework.UniVerseDateString(DocumentDT_From.Value)));
                }

                if (DocumentDT_To.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DOCDATE.INT", "<=", Framework.UniVerseDateString(DocumentDT_To.Value)));
                }

                if (DueDT_From.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DUEDATE.INT", ">=", Framework.UniVerseDateString(DueDT_From.Value)));
                }

                if (DueDT_To.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DUEDATE.INT", "<=", Framework.UniVerseDateString(DueDT_To.Value)));
                }

                if (OnFileDT_From.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DATEONFILE.INT", ">=", Framework.UniVerseDateString(OnFileDT_From.Value)));
                }

                if (OnFileDT_To.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DATEONFILE.INT", "<=", Framework.UniVerseDateString(OnFileDT_To.Value)));
                }

                if (filters.Any())
                {
                    foreach (var invoice in Selector<PurchaseLedgerInvoiceRegister>.FilterNonParallel(
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
                    foreach (var invoice in Selector<PurchaseLedgerInvoiceRegister>.SelectAllNonParallel(
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
                foreach (var invoice in Selector<PurchaseLedgerInvoiceRegister>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<PurchaseLedgerInvoiceRegisterCode>>(InvoiceRegisterCodes).Select(p => p.Code),
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

        public static void InvoiceRegisterSelect_FillRow(
            object obj,
            out SqlString Key,
            out SqlDateTime DocumentDT,
            out SqlDateTime OnFileDT,
            out SqlString ItemReference,
            out SqlDateTime DueDT,
            out SqlString SupplierReference,
            out SqlString AmountsExVAT,
            out SqlString GeneralLedgerCodes,
            out SqlString VATCodes,
            out SqlString VATTaxes,
            out SqlString VATValues,
            out SqlDecimal AmountSterling,
            out SqlString PaymentTermsCode,
            out SqlString Memo,
            out SqlDecimal VATBalance,
            out SqlDecimal DocumentBalance,
            out SqlString OtherMemos,
            out SqlDateTime ReceivedDT,
            out SqlString PassedTo,
            out SqlDateTime PassedToDT,
            out SqlDecimal AmountCurrency,
            out SqlDecimal ExchangeRate,
            out SqlString Dispute,
            out SqlString DisputeMemo,
            out SqlDateTime SettlementDT,
            out SqlString MertexCurrencyCode,
            out SqlDecimal DiscountPercentage,
            out SqlDecimal DiscountAmount
            )
        {
            var result = (PurchaseLedgerInvoiceRegister)obj;

            Key = result.Key?.TrimReduceTruncate(50);

            DocumentDT = result.DocumentDT ?? SqlDateTime.Null;

            OnFileDT = result.OnFileDT ?? SqlDateTime.Null;

            ItemReference = result.ItemReference?.TrimReduceTruncate(50);

            DueDT = result.DueDT ?? SqlDateTime.Null;

            SupplierReference = result.SupplierCode?.TrimReduceTruncate(50);

            AmountsExVAT = result.AmountsExVAT.Any() ? JsonConvert.SerializeObject(result.AmountsExVAT) : SqlString.Null;

            GeneralLedgerCodes = result.GeneralLedgerCodes.Any() ? JsonConvert.SerializeObject(result.GeneralLedgerCodes) : SqlString.Null;

            VATCodes = result.VATCodes.Any() ? JsonConvert.SerializeObject(result.VATCodes) : SqlString.Null;

            VATTaxes = result.VATTaxes.Any() ? JsonConvert.SerializeObject(result.VATTaxes) : SqlString.Null;

            VATValues = result.VATValues.Any() ? JsonConvert.SerializeObject(result.VATValues) : SqlString.Null;

            AmountSterling = result.AmountSterling;

            PaymentTermsCode = result.PaymentTermsCode?.TrimReduceTruncate(20);

            Memo = result.Memo?.TrimReduceTruncate(2000);

            VATBalance = result.VATBalance;

            DocumentBalance = result.DocumentBalance;

            OtherMemos = result.OtherMemos.Any() ? JsonConvert.SerializeObject(result.OtherMemos) : SqlString.Null;

            ReceivedDT = result.ReceivedDT ?? SqlDateTime.Null;

            PassedTo = result.PassedTo?.TrimReduceTruncate(50);

            PassedToDT = result.PassedToDT ?? SqlDateTime.Null;

            AmountCurrency = result.AmountCurrency;

            ExchangeRate = result.ExchangeRate;

            Dispute = result.Disputed;

            DisputeMemo = result.DisputeMemo?.TrimReduceTruncate(255) ?? SqlString.Null;

            SettlementDT = result.SettlementDT ?? SqlDateTime.Null;

            MertexCurrencyCode = result.MertexCurrencyCode?.TrimReduceTruncate(20);

            DiscountPercentage = result.DiscountPercentage ?? SqlDecimal.Null;

            DiscountAmount = result.DiscountAmount ?? SqlDecimal.Null;
        }
    }
}
