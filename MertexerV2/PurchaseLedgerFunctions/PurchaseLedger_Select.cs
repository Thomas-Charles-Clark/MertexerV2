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
    public class PurchaseLedgerCode
    {
        public string Code { get; set; }
    }

    public static partial class PurchaseLedgerFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "PurchaseLedgerSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(
            string UniVerseAccount,
            string PurchaseLedgerCodes,
            string SupplierCode,
            DateTime? OnFileDT_From,
            DateTime? OnFileDT_To,
            DateTime? DocumentDT_From,
            DateTime? DocumentDT_To,
            DateTime? DueDT_From,
            DateTime? DueDT_To,
            DateTime? PostDT_From,
            DateTime? PostDT_To,
            string PurchaseLedgerRegisterKey)
        {
            if (String.IsNullOrEmpty(PurchaseLedgerCodes))
            {
                int p = 0;

                var filters = new List<Tuple<string, string, string>>();

                if (!String.IsNullOrEmpty(SupplierCode))
                {
                    filters.Add(new Tuple<string, string, string>("PSUPREF", "eq", SupplierCode));
                }

                if (OnFileDT_From.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DATEONFILE.INT", ">=", Framework.UniVerseDateString(OnFileDT_From.Value)));
                }

                if (OnFileDT_To.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DATEONFILE.INT", "<=", Framework.UniVerseDateString(OnFileDT_To.Value)));
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

                if (!String.IsNullOrEmpty(PurchaseLedgerRegisterKey))
                {
                    filters.Add(new Tuple<string, string, string>("PLINVREG", "eq", PurchaseLedgerRegisterKey));
                }

                if (filters.Any())
                {
                    foreach (var invoice in Selector<PurchaseLedger>.FilterNonParallel(
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
                    foreach (var invoice in Selector<PurchaseLedger>.SelectAllNonParallel(
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
                foreach (var invoice in Selector<PurchaseLedger>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<PurchaseLedgerCode>>(PurchaseLedgerCodes).Select(p => p.Code),
                    true,
                    false,
                    100,
                    UniVerseAccount))
                {
                    yield return invoice;
                }
            }
        }

        public static void PurchaseLedgerSelect_FillRow(
            object obj,
            out SqlString Key,
            out SqlString SourceTransactionDefinition,
            out SqlDecimal DiscountPercentage,
            out SqlByte Days,
            out SqlByte DiscountNumber,
            out SqlByte DiscountDecimals,
            out SqlString SupplierCode,
            out SqlBoolean ThisPeriodZero,
            out SqlBoolean Period1Zero,
            out SqlBoolean Period2Zero,
            out SqlBoolean Period3Zero,
            out SqlBoolean Period4Zero,
            out SqlBoolean NoActivity,
            out SqlBoolean BalanceZero,
            out SqlBoolean DeletionValid,
            out SqlBoolean ConfirmOnDelete,
            out SqlBoolean NoOrderActivity,
            out SqlBoolean NoCurrentTransactionsThisYear,
            out SqlDateTime DocumentDT,
            out SqlString DocumentNumber,
            out SqlDateTime PostDT,
            out SqlDateTime OnFileDT,
            out SqlString SupplierInvoiceReference,
            out SqlDateTime InvoiceDueDT,
            out SqlInt32 BankNumber,
            out SqlInt32 BankAccountNumber,
            out SqlString Amounts,
            out SqlDecimal Amount,
            out SqlString GeneralLedgerCodes,
            out SqlString GeneralLedgerCode,
            out SqlString VATCodes,
            out SqlString VATCode,
            out SqlString VATTaxes,
            out SqlDecimal VATTax,
            out SqlString VATableValues,
            out SqlDecimal VATableValue,
            out SqlDecimal DocumentTotal,
            out SqlDecimal SterlingOriginalValue,
            out SqlDecimal ExchangeRateDifference,
            out SqlString DocumentReference,
            out SqlString SettlementTerms,
            out SqlString SettlementDueDTs,
            out SqlDateTime SettlementDueDT,
            out SqlString Memo,
            out SqlDecimal SettlementDiscount,
            out SqlString Key2,
            out SqlString Key3,
            out SqlDecimal VATBalance,
            out SqlDecimal DocumentBalance,
            out SqlDecimal AmountPaid,
            out SqlDecimal CurrentlyUnallocated,
            out SqlDecimal DiscountReceived,
            out SqlDecimal CashOnAccount,
            out SqlDecimal NotAllocated,
            out SqlDecimal UnallocatedValue,
            out SqlString DocumentNumbers,
            out SqlString DocumentReferences,
            out SqlString TransactionKeys,
            out SqlString SupplierReferences,
            out SqlString Indicator,
            out SqlString Allocated,
            out SqlBoolean OpenItem,
            out SqlString Key4,
            out SqlDecimal DiscountAmount,
            out SqlString Allocations,
            out SqlString OtherMemos,
            out SqlString OtherPrePaymentMemos,
            out SqlString DisputeDocumentNumber,
            out SqlString DisputeCustomerReference,
            out SqlString DisputeIndicator,
            out SqlString DisputeMemo,
            out SqlBoolean OkayToClear,
            out SqlString ReleasedBy,
            out SqlString Details,
            out SqlDecimal TransferAmount,
            out SqlDecimal GoodsValue,
            out SqlDecimal VATAmount,
            out SqlDecimal TotalValue,
            out SqlBoolean SetOpen,
            out SqlString OpeningDTs,
            out SqlString OpeningGoodsValue,
            out SqlString OpeningVATAmounts,
            out SqlString OpeningBalancesOutstanding,
            out SqlString OpeningItemReferences,
            out SqlString OpeningAges,
            out SqlBoolean IsUnallocatedValueZero,
            out SqlBoolean IsOnAccountZero,
            out SqlBoolean IsThisPeriodZero,
            out SqlBoolean TransferValid,
            out SqlBoolean ConfirmChange,
            out SqlBoolean AllocateCash,
            out SqlDecimal TotalAllocated,
            out SqlDateTime ReceivedDT,
            out SqlString PassedTo,
            out SqlDateTime PassedToDT,
            out SqlString TemporaryMessage,
            out SqlString GeneralLedgerCode9s,
            out SqlString GeneralLedgerCode9,
            out SqlString GeneralLedgerSubCodes,
            out SqlString GeneralLedgerSubCode,
            out SqlString RegisterReferences,
            out SqlString RegisterEntryTypes,
            out SqlDecimal TotalCashAllocated,
            out SqlDecimal ChequeValue,
            out SqlString ChequeAllocations,
            out SqlDecimal SterlingChequeValue,
            out SqlDecimal SettlementDiscountAmountInCurrency,
            out SqlDecimal DiscountPercentage_Again,
            out SqlString CurrencyCode,
            out SqlString ExchangeRates,
            out SqlDecimal ExchangeRateDifferences,
            out SqlBoolean Foreign,
            out SqlDecimal DiscountAmountInCurrency,
            out SqlString Authoriser,
            out SqlString DNMessages,
            out SqlString DNDescriptions,
            out SqlString DNQuantities,
            out SqlString DNPrices,
            out SqlString DNValues,
            out SqlString DNVATCodes,
            out SqlDecimal DNVATValue,
            out SqlString PurchaseLedgerRegisterKey,
            out SqlString OpeningGoodsValuesInCurrency,
            out SqlString OpeningVATAmountsInCurrency,
            out SqlString OpeningBalancesOutstandingInCurrency,
            out SqlDecimal ExchangeRateDifferencesInCurrency,
            out SqlDecimal DiscountAllowedInCurrency,
            out SqlString AmountsPaidInCurrency,
            out SqlString VATTaxesInCurrency,
            out SqlDecimal AmountInCurrency,
            out SqlDecimal UnallocatedCreditNoteAmountInCurrency,
            out SqlDecimal UnallocatedAmountInCurrency,
            out SqlString TransactionTypes,
            out SqlString SupplierInvoiceKeys,
            out SqlString Disputations,
            out SqlString SettlementDTs,
            out SqlString DiscountPercentages,
            out SqlString SettlementDiscountAmounts,
            out SqlString SettlementDiscountAmountsInCurrency,
            out SqlString PrepaymentStartPeriods,
            out SqlString PrepaymentNumberOfPeriods,
            out SqlString PrepaymentsSplitEqually,
            out SqlString PrepaymentSplitAmounts,
            out SqlString PrepaymentSplitMemos,
            out SqlString PrepaymentMemos,
            out SqlString PrepaymentGeneralLedgerExpenseCodes
            )
        {
            var p = (PurchaseLedger)obj;

            Key = p.Key;
            SourceTransactionDefinition = p.SourceTransactionDefinition;
            DiscountPercentage = p.DiscountPercentage ?? SqlDecimal.Null;
            Days = p.Days ?? SqlByte.Null;
            DiscountNumber = p.DiscountNumber ?? SqlByte.Null;
            DiscountDecimals = p.DiscountDecimals ?? SqlByte.Null;
            SupplierCode = p.SupplierCode;
            ThisPeriodZero = p.ThisPeriodZero ?? SqlBoolean.Null;
            Period1Zero = p.Period1Zero ?? SqlBoolean.Null;
            Period2Zero = p.Period2Zero ?? SqlBoolean.Null;
            Period3Zero = p.Period3Zero ?? SqlBoolean.Null;
            Period4Zero = p.Period4Zero ?? SqlBoolean.Null;
            NoActivity = p.NoActivity ?? SqlBoolean.Null;
            BalanceZero = p.BalanceZero ?? SqlBoolean.Null;
            DeletionValid = p.DeletionValid ?? SqlBoolean.Null;
            ConfirmOnDelete = p.ConfirmOnDelete ?? SqlBoolean.Null;
            NoOrderActivity = p.NoOrderActivity ?? SqlBoolean.Null;
            NoCurrentTransactionsThisYear = p.NoCurrentTransactionsThisYear ?? SqlBoolean.Null;
            DocumentDT = p.DocumentDT.HasValue ? p.DocumentDT.Value : SqlDateTime.Null;
            DocumentNumber = p.DocumentNumber;
            PostDT = p.PostDT.HasValue ? p.PostDT.Value : SqlDateTime.Null;
            OnFileDT = p.OnFileDT.HasValue ? p.OnFileDT.Value : SqlDateTime.Null;
            SupplierInvoiceReference = p.SupplierInvoiceReference;
            InvoiceDueDT = p.InvoiceDueDT.HasValue ? p.InvoiceDueDT.Value : SqlDateTime.Null;
            BankNumber = p.BankNumber ?? SqlInt32.Null;
            BankAccountNumber = p.BankAccountNumber ?? SqlInt32.Null;
            Amounts = p.Amounts?.Any() == true ? JsonConvert.SerializeObject(p.Amounts) : SqlString.Null;
            Amount = p.Amounts?.Any() == true ? Convert.ToDecimal(p.Amounts.First()) : SqlDecimal.Null;
            GeneralLedgerCodes = p.GeneralLedgerCodes?.Any() == true ? JsonConvert.SerializeObject(p.GeneralLedgerCodes) : SqlString.Null;
            GeneralLedgerCode = p.GeneralLedgerCodes?.Any() == true ? p.GeneralLedgerCodes.First() : SqlString.Null;
            VATCodes = p.VATCodes?.Any() == true ? JsonConvert.SerializeObject(p.VATCodes) : SqlString.Null;
            VATCode = p.VATCodes?.Any() == true ? p.VATCodes.First() : SqlString.Null;
            VATTaxes = p.VATTaxes?.Any() == true ? JsonConvert.SerializeObject(p.VATTaxes) : SqlString.Null;
            VATTax = p.VATTaxes?.Any() == true ? Convert.ToDecimal(p.VATTaxes.First()) : SqlDecimal.Null;
            VATableValues = p.VATableValues?.Any() == true ? JsonConvert.SerializeObject(p.VATableValues) : SqlString.Null;
            VATableValue = p.VATableValues?.Any() == true ? Convert.ToDecimal(p.VATableValues.First()) : SqlDecimal.Null;
            DocumentTotal = p.TotalDueInSterling;
            SterlingOriginalValue = p.SterlingOriginalValue;
            ExchangeRateDifference = p.ExchangeRateDifference;
            DocumentReference = p.DocumentReference;
            SettlementTerms = p.SettlementTerms;
            SettlementDueDTs = p.SettlementDueDTs?.Any() == true ? JsonConvert.SerializeObject(p.SettlementDueDTs) : SqlString.Null;
            SettlementDueDT = p.DueDT.HasValue ? p.DueDT.Value : SqlDateTime.Null;
            Memo = p.Memo;
            SettlementDiscount = p.SettlementDiscount;
            Key2 = p.Key2;
            Key3 = p.Key3;
            VATBalance = p.VATBalance;
            DocumentBalance = p.DocumentBalance;
            AmountPaid = p.AmountPaid ?? SqlDecimal.Null;
            CurrentlyUnallocated = p.CurrentlyUnallocated ?? SqlDecimal.Null;
            DiscountReceived = p.DiscountReceived ?? SqlDecimal.Null;
            CashOnAccount = p.CashOnAccount ?? SqlDecimal.Null;
            NotAllocated = p.NotAllocated ?? SqlDecimal.Null;
            UnallocatedValue = p.UnallocatedValue ?? SqlDecimal.Null;
            DocumentNumbers = p.DocumentNumbers?.Any() == true ? JsonConvert.SerializeObject(p.DocumentNumbers) : SqlString.Null;
            DocumentReferences = p.DocumentReferences?.Any() == true ? JsonConvert.SerializeObject(p.DocumentReferences) : SqlString.Null;
            TransactionKeys = p.TransactionKeys?.Any() == true ? JsonConvert.SerializeObject(p.TransactionKeys) : SqlString.Null;
            SupplierReferences = p.SupplierReferences?.Any() == true ? JsonConvert.SerializeObject(p.SupplierReferences) : SqlString.Null;
            Indicator = p.Indicator;
            Allocated = p.Allocated?.Any() == true ? JsonConvert.SerializeObject(p.Allocated) : SqlString.Null;
            OpenItem = p.OpenItem ?? SqlBoolean.Null;
            Key4 = p.Key4;
            DiscountAmount = p.DiscountAmount;
            Allocations = p.Allocations?.Any() == true ? JsonConvert.SerializeObject(p.Allocations) : SqlString.Null;
            OtherMemos = p.OtherMemos?.Any() == true ? JsonConvert.SerializeObject(p.OtherMemos) : SqlString.Null;
            OtherPrePaymentMemos = p.OtherPrepaymentMemos?.Any() == true ? JsonConvert.SerializeObject(p.OtherPrepaymentMemos) : SqlString.Null;
            DisputeDocumentNumber = p.DisputeDocumentNumber;
            DisputeCustomerReference = p.DisputeCustomerReference;
            DisputeIndicator = p.DisputeIndicator;
            DisputeMemo = p.DisputeMemo;
            OkayToClear = p.OkayToClear ?? SqlBoolean.Null;
            ReleasedBy = p.ReleasedBy;
            Details = p.Details;
            TransferAmount = p.TransferAmount ?? SqlDecimal.Null;
            GoodsValue = p.GoodsValue ?? SqlDecimal.Null;
            VATAmount = p.VATAmount ?? SqlDecimal.Null;
            TotalValue = p.TotalValue ?? SqlDecimal.Null;
            SetOpen = p.SetOpen ?? SqlBoolean.Null;
            OpeningDTs = p.OpeningDTs?.Any() == true ? JsonConvert.SerializeObject(p.OpeningDTs) : SqlString.Null;
            OpeningGoodsValue = p.OpeningGoodsValue?.Any() == true ? JsonConvert.SerializeObject(p.OpeningGoodsValue) : SqlString.Null;
            OpeningVATAmounts = p.OpeningVATAmounts?.Any() == true ? JsonConvert.SerializeObject(p.OpeningVATAmounts) : SqlString.Null;
            OpeningBalancesOutstanding = p.OpeningBalancesOutstanding?.Any() == true ? JsonConvert.SerializeObject(p.OpeningBalancesOutstanding) : SqlString.Null;
            OpeningItemReferences = p.OpeningItemReferences?.Any() == true ? JsonConvert.SerializeObject(p.OpeningItemReferences) : SqlString.Null;
            OpeningAges = p.OpeningAges?.Any() == true ? JsonConvert.SerializeObject(p.OpeningAges) : SqlString.Null;
            IsUnallocatedValueZero = p.IsUnallocatedValueZero ?? SqlBoolean.Null;
            IsOnAccountZero = p.IsOnAccountZero ?? SqlBoolean.Null;
            IsThisPeriodZero = p.IsThisPeriodZero ?? SqlBoolean.Null;
            TransferValid = p.TransferValid ?? SqlBoolean.Null;
            ConfirmChange = p.ConfirmChange ?? SqlBoolean.Null;
            AllocateCash = p.AllocateCash ?? SqlBoolean.Null;
            TotalAllocated = p.TotalAllocated ?? SqlDecimal.Null;
            ReceivedDT = p.ReceivedDT.HasValue ? p.ReceivedDT.Value : SqlDateTime.Null;
            PassedTo = p.PassedTo;
            PassedToDT = p.PassedToDT.HasValue ? p.PassedToDT.Value : SqlDateTime.Null;
            TemporaryMessage = p.TemporaryMessage;
            GeneralLedgerCode9s = p.GeneralLedgerCode9s?.Any() == true ? JsonConvert.SerializeObject(p.GeneralLedgerCode9s) : SqlString.Null;
            GeneralLedgerCode9 = p.GeneralLedgerCode9;
            GeneralLedgerSubCodes = p.GeneralLedgerSubCodes?.Any() == true ? JsonConvert.SerializeObject(p.GeneralLedgerSubCodes) : SqlString.Null;
            GeneralLedgerSubCode = p.GeneralLedgerSubCode;
            RegisterReferences = p.RegisterReferences?.Any() == true ? JsonConvert.SerializeObject(p.RegisterReferences) : SqlString.Null;
            RegisterEntryTypes = p.RegisterEntryTypes?.Any() == true ? JsonConvert.SerializeObject(p.RegisterEntryTypes) : SqlString.Null;
            TotalCashAllocated = p.TotalCashAllocated ?? SqlDecimal.Null;
            ChequeValue = p.ChequeValue ?? SqlDecimal.Null;
            ChequeAllocations = p.ChequeAllocations?.Any() == true ? JsonConvert.SerializeObject(p.ChequeAllocations) : SqlString.Null;
            SterlingChequeValue = p.SterlingChequeValue ?? SqlDecimal.Null;
            SettlementDiscountAmountInCurrency = p.SettlementDiscountAmountInCurrency ?? SqlDecimal.Null;
            DiscountPercentage_Again = p.DiscountPercentage_Again ?? SqlDecimal.Null;
            CurrencyCode = p.CurrencyCode;
            ExchangeRates = p.ExchangeRates?.Any() == true ? JsonConvert.SerializeObject(p.ExchangeRates) : SqlString.Null;
            ExchangeRateDifferences = p.ExchangeRateDifferences ?? SqlDecimal.Null;
            Foreign = p.Foreign;
            DiscountAmountInCurrency = p.DiscountAmountInCurrency ?? SqlDecimal.Null;
            Authoriser = p.Authoriser;
            DNMessages = p.DNMessages?.Any() == true ? JsonConvert.SerializeObject(p.DNMessages) : SqlString.Null;
            DNDescriptions = p.DNDescriptions?.Any() == true ? JsonConvert.SerializeObject(p.DNDescriptions) : SqlString.Null;
            DNQuantities = p.DNQuantities?.Any() == true ? JsonConvert.SerializeObject(p.DNQuantities) : SqlString.Null;
            DNPrices = p.DNPrices?.Any() == true ? JsonConvert.SerializeObject(p.DNPrices) : SqlString.Null;
            DNValues = p.DNValues?.Any() == true ? JsonConvert.SerializeObject(p.DNValues) : SqlString.Null;
            DNVATCodes = p.DNVATCodes?.Any() == true ? JsonConvert.SerializeObject(p.DNVATCodes) : SqlString.Null;
            DNVATValue = p.DNVATValue ?? SqlDecimal.Null;
            PurchaseLedgerRegisterKey = p.PurchaseLedgerRegisterKey;
            OpeningGoodsValuesInCurrency = p.OpeningGoodsValuesInCurrency?.Any() == true ? JsonConvert.SerializeObject(p.OpeningGoodsValuesInCurrency) : SqlString.Null;
            OpeningVATAmountsInCurrency = p.OpeningVATAmountsInCurrency?.Any() == true ? JsonConvert.SerializeObject(p.OpeningVATAmountsInCurrency) : SqlString.Null;
            OpeningBalancesOutstandingInCurrency = p.OpeningBalancesOutstandingInCurrency?.Any() == true ? JsonConvert.SerializeObject(p.OpeningVATAmountsInCurrency) : SqlString.Null;
            ExchangeRateDifferencesInCurrency = p.ExchangeRateDifferencesInCurrency;
            DiscountAllowedInCurrency = p.DiscountAllowedInCurrency ?? SqlDecimal.Null;
            AmountsPaidInCurrency = p.AmountsPaidInCurrency?.Any() == true ? JsonConvert.SerializeObject(p.AmountsPaidInCurrency) : SqlString.Null;
            VATTaxesInCurrency = p.VATTaxesInCurrency?.Any() == true ? JsonConvert.SerializeObject(p.VATTaxesInCurrency) : SqlString.Null;
            AmountInCurrency = p.AmountInCurrency;
            UnallocatedCreditNoteAmountInCurrency = p.UnallocatedCreditNoteAmountInCurrency ?? SqlDecimal.Null;
            UnallocatedAmountInCurrency = p.UnallocatedAmountInCurrency;
            TransactionTypes = p.TransactionTypes?.Any() == true ? JsonConvert.SerializeObject(p.TransactionTypes) : SqlString.Null;
            SupplierInvoiceKeys = p.SupplierInvoiceKeys?.Any() == true ? JsonConvert.SerializeObject(p.SupplierInvoiceKeys) : SqlString.Null;
            Disputations = p.Disputations?.Any() == true ? JsonConvert.SerializeObject(p.Disputations) : SqlString.Null;
            SettlementDTs = p.SettlementDTs?.Any() == true ? JsonConvert.SerializeObject(p.SettlementDTs) : SqlString.Null;
            DiscountPercentages = p.DiscountPercentages?.Any() == true ? JsonConvert.SerializeObject(p.DiscountPercentages) : SqlString.Null;
            SettlementDiscountAmounts = p.SettlementDiscountAmounts?.Any() == true ? JsonConvert.SerializeObject(p.SettlementDiscountAmounts) : SqlString.Null;
            SettlementDiscountAmountsInCurrency = p.SettlementDiscountAmountsInCurrency?.Any() == true ? JsonConvert.SerializeObject(p.SettlementDiscountAmountsInCurrency) : SqlString.Null;
            PrepaymentStartPeriods = p.PrepaymentStartPeriods?.Any() == true ? JsonConvert.SerializeObject(p.PrepaymentStartPeriods) : SqlString.Null;
            PrepaymentNumberOfPeriods = p.PrepaymentNumberOfPeriods?.Any() == true ? JsonConvert.SerializeObject(p.PrepaymentNumberOfPeriods) : SqlString.Null;
            PrepaymentsSplitEqually = p.PrepaymentsSplitEqually?.Any() == true ? JsonConvert.SerializeObject(p.PrepaymentsSplitEqually) : SqlString.Null;
            PrepaymentSplitAmounts = p.PrepaymentSplitAmounts?.Any() == true ? JsonConvert.SerializeObject(p.PrepaymentSplitAmounts) : SqlString.Null;
            PrepaymentSplitMemos = p.PrepaymentSplitMemos?.Any() == true ? JsonConvert.SerializeObject(p.PrepaymentSplitMemos) : SqlString.Null;
            PrepaymentMemos = p.PrepaymentMemos?.Any() == true ? JsonConvert.SerializeObject(p.PrepaymentMemos) : SqlString.Null;
            PrepaymentGeneralLedgerExpenseCodes = p.PrepaymentGeneralLedgerExpenseCodes?.Any() == true ? JsonConvert.SerializeObject(p.PrepaymentGeneralLedgerExpenseCodes) : SqlString.Null;
        }
    }
}
