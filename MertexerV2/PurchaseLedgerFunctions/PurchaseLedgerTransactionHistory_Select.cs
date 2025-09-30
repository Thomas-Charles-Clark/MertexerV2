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
    public class PurchaseLedgerTransactionHistoryCode
    {
        public string Code { get; set; }
    }

    public static partial class PurchaseLedgerTransactionHistoryFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "PurchaseLedgerTransactionHistorySelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(
            string UniVerseAccount,
            string PurchaseLedgerTransactionHistoryCodes,
            string SupplierCode,
            DateTime? DocumentDT_From,
            DateTime? DocumentDT_To,
            DateTime? DueDT_From,
            DateTime? DueDT_To,
            DateTime? PostDT_From,
            DateTime? PostDT_To,
            string PurchaseLedgerRegisterNumber)
        {
            if (String.IsNullOrEmpty(PurchaseLedgerTransactionHistoryCodes))
            {
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

                if (DueDT_From.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DUEDATE.INT", ">=", Framework.UniVerseDateString(DueDT_From.Value)));
                }

                if (DueDT_To.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DUEDATE.INT", "<=", Framework.UniVerseDateString(DueDT_To.Value)));
                }

                if (PostDT_From.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.POSTDATE.INT", ">=", Framework.UniVerseDateString(PostDT_From.Value)));
                }

                if (PostDT_To.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.POSTDATE.INT", "<=", Framework.UniVerseDateString(PostDT_To.Value)));
                }
                
                if (!String.IsNullOrEmpty(PurchaseLedgerRegisterNumber))
                {
                    filters.Add(new Tuple<string, string, string>("REGNO", "eq", PurchaseLedgerRegisterNumber));
                }

                if (filters.Any())
                {
                    foreach (var invoice in Selector<PurchaseLedgerTransactionHistory>.FilterNonParallel(
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
                    foreach (var invoice in Selector<PurchaseLedgerTransactionHistory>.SelectAllNonParallel(
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
                foreach (var invoice in Selector<PurchaseLedgerTransactionHistory>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<PurchaseLedgerTransactionHistoryCode>>(PurchaseLedgerTransactionHistoryCodes).Select(p => p.Code),
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

        public static void PurchaseLedgerTransactionHistorySelect_FillRow(
            object obj,
            out SqlString Key,
            out SqlDateTime DocumentDT,
            out SqlString TransactionHistoryType,
            out SqlString ItemReference,
            out SqlDecimal OriginalValue,
            out SqlString DocumentNumber,
            out SqlDecimal VATTaxesTotal,
            out SqlDecimal Debit,
            out SqlDecimal Credit,
            out SqlDecimal Balance,
            out SqlByte Period,
            out SqlString Status,
            out SqlString Statuses,
            out SqlString SupplierReference,
            out SqlInt32 Age,
            out SqlDecimal OriginalValueABS,
            out SqlString DisputeMemo,
            out SqlString Dispute,
            out SqlString Adjust,
            out SqlDecimal DiscountPercentage,
            out SqlString SelectedForCheque,
            out SqlDecimal DiscountAmount,
            out SqlDecimal Payment,
            out SqlDecimal Unallocated,
            out SqlDecimal SDTaken,
            out SqlDecimal DiscountTaken,
            out SqlDateTime PostDT,
            out SqlDateTime DiscountCutOffDT,
            out SqlString CurrencyCode,
            out SqlString ExchangeRates,
            out SqlDecimal MostRecentExchangeRate,
            out SqlDecimal ExchangeRateDifferences,
            out SqlDecimal BankCharges,
            out SqlDateTime PaidDT,
            out SqlDecimal RoundingErrors,
            out SqlInt32 Version,
            out SqlString NextKeepFlag,
            out SqlString PurchaseOrderNumbers,
            out SqlString Division,
            out SqlString SupplierCallNoteReferences,
            out SqlString PurchaseLedgerRegisterNumber,
            out SqlDecimal OutstandingAmount
            )
        {
            var result = (PurchaseLedgerTransactionHistory)obj;

            Key = result.Key?.TrimReduceTruncate(50);

            DocumentDT = result.DocumentDT ?? SqlDateTime.Null;

            TransactionHistoryType = result.TransactionType?.TrimReduceTruncate(10) ?? SqlString.Null;
            ItemReference = result.ItemReference?.TrimReduceTruncate(50) ?? SqlString.Null;
            OriginalValue = result.OriginalValue;
            DocumentNumber = result.DocumentNumber?.TrimReduceTruncate(50) ?? SqlString.Null;
            VATTaxesTotal = result.VATTaxesTotal;
            Debit = result.Debit;
            Credit = result.Credit;
            Balance = result.Balance;
            Period = result.Period;
            Status = result.Status.TrimReduceTruncate(1);
            Statuses = result.Statuses?.Any() == true ? JsonConvert.SerializeObject(result.Statuses) : SqlString.Null;
            SupplierReference = result.SupplierReference?.TrimReduceTruncate(50) ?? SqlString.Null;
            Age = result.Age;
            OriginalValueABS = result.OriginalValueABS;
            DisputeMemo = result.DisputeMemo?.TrimReduceTruncate(50);
            Dispute = result.Dispute?.TrimReduceTruncate(1) ?? SqlString.Null;
            Adjust = result.Adjust?.TrimReduceTruncate(1) ?? SqlString.Null;
            DiscountPercentage = result.DiscountPercentage ?? SqlDecimal.Null;
            SelectedForCheque = result.SelectedForCheque?.TrimReduceTruncate(1) ?? SqlString.Null;
            DiscountAmount = result.DiscountAmount;
            Payment = result.Payment;
            Unallocated = result.Unallocated;
            SDTaken = result.SDTaken;
            DiscountTaken = result.DiscountTaken;
            PostDT = result.PostDT ?? SqlDateTime.Null;
            DiscountCutOffDT = result.DiscountCutOffDT ?? SqlDateTime.Null;
            CurrencyCode = result.CurrencyCode?.TrimReduceTruncate(4) ?? SqlString.Null;
            ExchangeRates = result.ExchangeRates.Any() ? JsonConvert.SerializeObject(result.ExchangeRates) : SqlString.Null;
            MostRecentExchangeRate = result.ExchangeRates.Any() ? result.ExchangeRates.First() : SqlDecimal.Null;
            ExchangeRateDifferences = result.ExchangeRateDifferences ?? SqlDecimal.Null;
            BankCharges = result.BankCharges ?? SqlDecimal.Null;
            PaidDT = result.PaidDT ?? SqlDateTime.Null;
            RoundingErrors = result.RoundingErrors ?? SqlDecimal.Null;
            Version = result.Version;
            NextKeepFlag = result.NextKeepFlag?.TrimReduceTruncate(1) ?? SqlString.Null;
            PurchaseOrderNumbers = result.PurchaseOrderNumbers.Any() ? JsonConvert.SerializeObject(result.PurchaseOrderNumbers) : SqlString.Null;
            Division = result.Division?.TrimReduceTruncate(2) ?? SqlString.Null;
            SupplierCallNoteReferences = result.SupplierCallNoteReferences.Any() ? JsonConvert.SerializeObject(result.SupplierCallNoteReferences) : SqlString.Null;
            PurchaseLedgerRegisterNumber = result.PurchaseLedgerRegisterNumber?.TrimReduceTruncate(50) ?? SqlString.Null;
            OutstandingAmount = result.OutstandingAmount ?? SqlDecimal.Null; // OutstandingAmount is named REFENQ.W9 which infers that it is a computed, working field - devised by some paragraph for some screen.
        }
    }
}
