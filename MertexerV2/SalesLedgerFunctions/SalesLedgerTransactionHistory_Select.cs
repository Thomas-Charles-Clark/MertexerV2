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
    public static partial class SalesLedgerTransactionHistoryFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "SalesLedgerTransactionHistorySelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(
            string UniVerseAccount,
            string SalesLedgerCodes,
            string TradeAccountCode,
            DateTime? DocumentDT_From,
            DateTime? DocumentDT_To,
            DateTime? DueDT_From,
            DateTime? DueDT_To,
            DateTime? PostDT_From,
            DateTime? PostDT_To)
        {
            if (String.IsNullOrEmpty(SalesLedgerCodes))
            {
                int p = 0;

                var filters = new List<Tuple<string, string, string>>();

                if (!String.IsNullOrEmpty(TradeAccountCode))
                {
                    filters.Add(new Tuple<string, string, string>("CUSTREF", "eq", TradeAccountCode));
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

                if (filters.Any())
                {
                    foreach (var invoice in Selector<SalesLedgerTransactionHistory>.FilterNonParallel(
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
                    foreach (var invoice in Selector<SalesLedgerTransactionHistory>.SelectAllNonParallel(
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
                foreach (var invoice in Selector<SalesLedgerTransactionHistory>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<SalesLedgerTransactionCode>>(SalesLedgerCodes).Select(p => p.Code),
                    true,
                    false,
                    500,
                    UniVerseAccount))
                {
                    yield return invoice;
                }
            }
        }

        public static void SalesLedgerTransactionHistorySelect_FillRow(
            object obj,
            out SqlString Key,
            out SqlDateTime DocumentDT,
            out SqlDateTime DueDT,
            out SqlString TransactionType,
            out SqlString ItemReference,
            out SqlDecimal OriginalValue,
            out SqlString DocumentReference,
            out SqlDecimal VATDiscount,
            out SqlDecimal Debit,
            out SqlDecimal Credit,
            out SqlDecimal Balance,
            out SqlByte Period,
            out SqlString Status,
            out SqlString TradeAccountCode,
            out SqlInt32 Age,
            out SqlDecimal OriginalValueABS,
            out SqlString DisputeMemo,
            out SqlString Dispute,
            out SqlString Adjust,
            out SqlDecimal Unallocated,
            out SqlDecimal OutstandingAmount,
            out SqlDateTime PostDT,
            out SqlDateTime CSDT,
            out SqlDecimal CSAmount,
            out SqlString CSAction,
            out SqlString AllocatedFromDocuments,
            out SqlString AllocatedToDocuments,
            out SqlString InvoiceAccount,
            out SqlString CustomerOrderNumber,
            out SqlDecimal ExchangeRateDifferences,
            out SqlDateTime PaidDT,
            out SqlDecimal RoundingErrors,
            out SqlString SourceDocument,
            out SqlInt32 Version,
            out SqlDecimal SettlementDiscount,
            out SqlDateTime SettlementDT,
            out SqlDecimal SettlementDiscountTaken,
            out SqlString CreditClaimNumber,
            out SqlString SellingCompany,
            out SqlInt32 PostTime
            )
        {
            var result = (SalesLedgerTransactionHistory)obj;

            Key = result.Key?.TrimReduceTruncate(50);
            DocumentDT = result.DocumentDT ?? SqlDateTime.Null;
            DueDT = result.DueDT ?? SqlDateTime.Null;
            TransactionType = result.TransactionType?.TrimReduceTruncate(10) ?? SqlString.Null;
            ItemReference = result.ItemReference?.TrimReduceTruncate(50) ?? SqlString.Null;
            OriginalValue = result.OriginalValue;
            DocumentReference = result.DocumentReference?.TrimReduceTruncate(50) ?? SqlString.Null;
            VATDiscount = result.VATDiscount;
            Debit = result.Debit;
            Credit = result.Credit;
            Balance = result.Balance;
            Period = result.Period;
            Status = result.Status;
            TradeAccountCode = result.TradeAccountCode?.TrimReduceTruncate(50) ?? SqlString.Null;
            Age = result.Age;
            OriginalValueABS = result.OriginalValueABS;
            DisputeMemo = result.Reason?.TrimReduceTruncate(200) ?? SqlString.Null;
            Dispute = result.Dispute?.TrimReduceTruncate(10) ?? SqlString.Null;
            Adjust = result.Adjust?.TrimReduceTruncate(10) ?? SqlString.Null;
            Unallocated = result.Unallocated;
            OutstandingAmount = result.OustandingAmount;
            PostDT = result.PostDT ?? SqlDateTime.Null;
            CSDT = result.CSDT ?? SqlDateTime.Null;
            CSAmount = result.CSAmount;
            CSAction = result.CSAction?.TrimReduceTruncate(50) ?? SqlString.Null;
            AllocatedFromDocuments = result.AllocatedFromDocuments.Any() ? JsonConvert.SerializeObject(result.AllocatedFromDocuments) : SqlString.Null;
            AllocatedToDocuments = result.AllocatedToDocuments.Any() ? JsonConvert.SerializeObject(result.AllocatedToDocuments) : SqlString.Null;
            InvoiceAccount = result.InvoiceAccount?.TrimReduceTruncate(50) ?? SqlString.Null;
            CustomerOrderNumber = result.CustomerOrderNumber?.TrimReduceTruncate(50) ?? SqlString.Null;
            ExchangeRateDifferences = result.ExchangeRateDifferences;
            PaidDT = result.PaidDT ?? SqlDateTime.Null;
            RoundingErrors = result.RoundingErrors;
            SourceDocument = result.SourceDocument?.TrimReduceTruncate(260) ?? SqlString.Null;
            Version = result.Version;
            SettlementDiscount = result.SettlementDiscount;
            SettlementDT = result.SettlementDT ?? SqlDateTime.Null;
            SettlementDiscountTaken = result.SettlementDiscountTaken;
            CreditClaimNumber = result.CreditClaimNumber?.TrimReduceTruncate(50) ?? SqlString.Null;
            SellingCompany = result.SellingCompany?.TrimReduceTruncate(50) ?? SqlString.Null;
            PostTime = result.PostTime;
        }
    }
}
