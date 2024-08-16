using MertexerV2.GeneralFunctions;
using MertexerV2.Models.Bank;
using MertexerV2.Models.SalesAnalysis;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.InvoiceFunctions
{
    public class SalesAnalysisCode
    {
        public string SalesAnalysisID { get; set; }
    }

    public static partial class SalesAnalysisFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "SalesAnalysisSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string SalesAnalysisIDs, DateTime? FromDT, DateTime? ToDT)
        {
            if (String.IsNullOrEmpty(SalesAnalysisIDs))
            {
                var filters = new List<Tuple<string, string, string>>();

                if (FromDT != null)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DOCDATE.DES", ">=", Framework.UniVerseDateString(FromDT.Value)));
                }

                if (ToDT != null)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DOCDATE.DES", "<=", Framework.UniVerseDateString(ToDT.Value)));
                }

                // non parallel, 2k batch, 52k records, last month, about 6 minutes 9s
                // parallel, 2k batch, 52k records, last month, 

                if (filters.Any())
                {
#if DEBUG
                    var results = Selector<SalesAnalysis>.Filter(
                        filters,
                        true,
                        false,
                        1000,
                        UniVerseAccount);

                    foreach (var r in results)
                    {
                        yield return r;
                    }
#else
                    foreach (var r in Selector<SalesAnalysis>.FilterNonParallel(
                        filters,
                        true,
                        false,
                        1000,
                        UniVerseAccount))
                    {
                        yield return r;
                    }
#endif
                }
                else
                {
#if DEBUG
                    var results = Selector<SalesAnalysis>.SelectAll(
                        true,
                        false,
                        50,
                        UniVerseAccount);

                    foreach (var r in results)
                    {
                        yield return r;
                    }
#else
                    foreach (var r in Selector<SalesAnalysis>.SelectAllNonParallel(
                        true,
                        false,
                        50,
                        UniVerseAccount))
                    {
                        yield return r;
                    }
#endif
                }
            }
            else
            {
                var results = Selector<SalesAnalysis>.Select(
                   JsonConvert.DeserializeObject<IEnumerable<SalesAnalysisCode>>(SalesAnalysisIDs).Select(p => p.SalesAnalysisID),
                   true,
                   false,
                   50,
                   UniVerseAccount)
                   .ToList();

                foreach (var r in results)
                {
                    yield return r;
                }
            }
        }

        public static void SalesAnalysisSelect_FillRow(
            object obj,
            out SqlString SalesAnalysisID,
            out SqlString TradeAccountCode,
            out SqlString SalesRepresentativeCode,
            out SqlString SalesAgentCode,
            out SqlString SalesAreaCode,
            out SqlString MarketCode,
            out SqlString CountryCode,
            out SqlString RetailOrContractIndicator,
            out SqlString CustomerTypeCode,
            out SqlString CustomerClassCode,
            out SqlString TradeAccountGroupCode,
            out SqlString EndCustomerTradeAccountCode,
            out SqlString ProductGroupCode,
            out SqlString BrandCode,
            out SqlString ItemTypeCode,
            out SqlString CollectionCode,
            out SqlString PatternBookProductCode,
            out SqlString OrderID,
            out SqlString OrderTakerCode,
            out SqlDateTime OrderEntryDate,
            out SqlDateTime OrderDate,
            out SqlString HaulierCode,
            out SqlString ProductCode,
            out SqlString StockLocationCode,
            out SqlDecimal QuantityOrdered,
            out SqlString StockUnitCode,
            out SqlDecimal SalesValueCurrency,
            out SqlDecimal SalesValueSterling,
            out SqlDecimal CostValueCurrency,
            out SqlDecimal CostValueSterling,
            out SqlDecimal DiscountPercentage,
            out SqlDecimal DiscountValueCurrency,
            out SqlDecimal TransferValueCurrency,
            out SqlDecimal TransferValueSterling,
            out SqlDateTime DespatchDate,
            out SqlString CreditOrInvoiceIndicator,
            out SqlByte RecordType,
            out SqlDateTime InvoiceDate,
            out SqlDateTime PostDate,
            out SqlString InvoiceID,
            out SqlString GeneralLedgerSaleCode,
            out SqlString GeneralLedgerCostCode,
            out SqlString GeneralLedgerPromotionCode,
            out SqlString GeneralLedgerStockCode,
            out SqlString MertexCurrencyCode,
            out SqlDecimal SterlingExchangeRate,
            out SqlInt32 FinancialPeriod,
            out SqlDecimal MultiplicationFactor,
            out SqlString PromotionCode,
            out SqlString PromotionTypeCode,
            out SqlBoolean FreeOfChargeIndicator,
            out SqlByte OrderCategoryCode)
        {
            var result = (SalesAnalysis)obj;

            SalesAnalysisID = result.Key.TrimReduceTruncate(50);

            TradeAccountCode = result.TradeAccountCode?.TrimReduceTruncate(50);

            SalesRepresentativeCode = result.SalesRepresentativeCode?.TrimReduceTruncate(10);

            SalesAgentCode = result.SalesAgentCode?.TrimReduceTruncate(10);

            SalesAreaCode = result.SalesAreaCode?.TrimReduceTruncate(10);

            MarketCode = result.MarketCode?.TrimReduceTruncate(1);

            CountryCode = result.CountryCode?.TrimReduceTruncate(2);

            RetailOrContractIndicator = result.RetailOrContractIndicator.TrimReduceTruncate(1);

            CustomerTypeCode = result.CustomerTypeCode?.TrimReduceTruncate(5);

            CustomerClassCode = result.CustomerClassCode?.TrimReduceTruncate(5);

            TradeAccountGroupCode = result.TradeAccountGroupCode?.TrimReduceTruncate(10);

            EndCustomerTradeAccountCode = result.EndCustomerTradeAccountCode?.TrimReduceTruncate(50);

            ProductGroupCode = result.ProductGroupCode?.TrimReduceTruncate(10);

            BrandCode = result.BrandCode?.TrimReduceTruncate(5);

            ItemTypeCode = result.ItemTypeCode?.TrimReduceTruncate(10);

            CollectionCode = result.CollectionCode?.TrimReduceTruncate(20);

            PatternBookProductCode = result.PatternBookProductCode?.TrimReduceTruncate(50);

            OrderID = result.OrderID?.TrimReduceTruncate(25);

            OrderTakerCode = result.OrderTakerCode?.TrimReduceTruncate(10);

            OrderDate = result.OrderDT ?? SqlDateTime.Null;

            OrderEntryDate = result.OrderEntryDT ?? SqlDateTime.Null;

            HaulierCode = result.HaulierCode?.TrimReduceTruncate(10);

            ProductCode = result.ProductCode?.TrimReduceTruncate(50);

            StockLocationCode = result.StockLocationCode?.TrimReduceTruncate(10);

            QuantityOrdered = result.QuantityOrdered ?? SqlDecimal.Null;

            StockUnitCode = result.StockUnitCode?.TrimReduceTruncate(10);

            SalesValueCurrency = result.SalesValueCurrency ?? SqlDecimal.Null;

            SalesValueSterling = result.SalesValueSterling ?? SqlDecimal.Null;

            CostValueCurrency = result.CostValueCurrency ?? SqlDecimal.Null;   

            CostValueSterling = result.CostValueSterling ?? SqlDecimal.Null;

            DiscountPercentage = result.DiscountPercentage ?? SqlDecimal.Null;  

            DiscountValueCurrency = result.DiscountValue ?? SqlDecimal.Null;    

            TransferValueCurrency = result.TransferValueCurrency ?? SqlDecimal.Null;

            TransferValueSterling = result.TransferValueSterling ?? SqlDecimal.Null;

            DespatchDate = result.DespatchDT ?? SqlDateTime.Null;

            CreditOrInvoiceIndicator = result.CreditOrInvoiceIndicator?.TrimReduceTruncate(10);

            RecordType = result.RecordType;

            InvoiceDate = result.InvoiceDT ?? SqlDateTime.Null;

            PostDate = result.PostDT ?? SqlDateTime.Null;

            InvoiceID = result.InvoiceID?.TrimReduceTruncate(25);

            GeneralLedgerSaleCode = result.GeneralLedgerSaleCode?.TrimReduceTruncate(50);

            GeneralLedgerCostCode = result.GeneralLedgerCostCode?.TrimReduceTruncate(50);

            GeneralLedgerPromotionCode = result.GeneralLedgerPromotionCode?.TrimReduceTruncate(50);

            GeneralLedgerStockCode = result.GeneralLedgerStockCode?.TrimReduceTruncate(50);

            MertexCurrencyCode = result.CurrencyCode?.TrimReduceTruncate(3);

            SterlingExchangeRate = result.ExchangeRate ?? SqlDecimal.Null;

            FinancialPeriod = result.FinancialPeriod;

            MultiplicationFactor = result.MultiplicationFactor ?? SqlDecimal.Null;

            PromotionCode = result.PromotionCode?.TrimReduceTruncate(50);

            PromotionTypeCode = result.PromotionTypeCode?.TrimReduceTruncate(10);

            FreeOfChargeIndicator = result.FreeOfChargeIndicator;

            OrderCategoryCode = result.OrderCategoryCode;
        }
    }
}
