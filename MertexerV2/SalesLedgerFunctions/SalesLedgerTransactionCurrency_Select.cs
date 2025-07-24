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
    public static partial class SalesLedgerTransactionCurrencyFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "SalesLedgerTransactionCurrencySelect_FillRow")]
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
                    foreach (var invoice in Selector<SalesLedgerTransactionCurrency>.FilterNonParallel(
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
                    foreach (var invoice in Selector<SalesLedgerTransactionCurrency>.SelectAllNonParallel(
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
                foreach (var invoice in Selector<SalesLedgerTransactionCurrency>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<SalesLedgerTransactionCode>>(SalesLedgerCodes).Select(p => p.Code),
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

        public static void SalesLedgerTransactionCurrencySelect_FillRow(
            object obj,
            out SqlString Key,
            out SqlString CurrencyCode,
            out SqlDecimal ExchangeRate,
            out SqlDecimal OriginalValue,
            out SqlDecimal VATDiscount,
            out SqlDecimal Debit,
            out SqlDecimal Credit,
            out SqlDecimal Balance,
            out SqlString TradeAccountCode,
            out SqlDecimal OriginalValueABS,
            out SqlDecimal Unallocated,
            out SqlDecimal OutstandingAmount,
            out SqlDecimal CSAmount,
            out SqlDecimal RoundingErrors,
            out SqlDecimal SettlementDiscount,
            out SqlDecimal SettlementDiscountTaken
            )
        {
            var result = (SalesLedgerTransactionCurrency)obj;
            
            Key = result.Key?.TrimReduceTruncate(50);
            CurrencyCode = result.CurrencyCode?.TrimReduceTruncate(10);
            ExchangeRate = result.ExchangeRate;
            OriginalValue = result.OriginalValue;
            VATDiscount = result.VATDiscount;
            Debit = result.Debit;
            Credit = result.Credit;
            Balance = result.Balance;
            TradeAccountCode = result.TradeAccountCode?.TrimReduceTruncate(50) ?? SqlString.Null;
            OriginalValueABS = result.OriginalValueABS;
            Unallocated = result.Unallocated;
            OutstandingAmount = result.OustandingAmount;
            CSAmount = result.CSAmount;
            RoundingErrors = result.RoundingErrors;
            SettlementDiscount = result.SettlementDiscount;
            SettlementDiscountTaken = result.SettlementDiscountTaken;
        }
    }
}
