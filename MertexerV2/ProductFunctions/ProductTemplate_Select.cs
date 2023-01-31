using MertexerV2.GeneralFunctions;
using MertexerV2.Models.EDAReplacement;
using MertexerV2.Models.Product;
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

namespace MertexerV2.ProductFunctions
{
    public class ProductTemplateCode
    {
        public string Code { get; set; }
    }

    public static partial class ProductTemplateFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "ProductTemplateSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string ProductTemplateCodes)
        {
            if (String.IsNullOrEmpty(ProductTemplateCodes))
            {
                foreach (var q in Selector<ProductTemplate>.SelectAllNonParallel(
                   true,
                   false,
                   50,
                   UniVerseAccount))
                {
                    yield return q;
                }
            }
            else
            {
                var results = Selector<ProductTemplate>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<ProductTemplateCode>>(ProductTemplateCodes).Select(p => p.Code),
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

        public static void ProductTemplateSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name,
            out SqlString FirstUserScreen,
            out SqlString ProductGroupCode,
            out SqlString ItemTypeCode,
            out SqlString UnitOfPurchase,
            out SqlString UnitOfStock,
            out SqlString VATCode,
            out SqlBoolean PieceLevelStock,
            out SqlInt32 SalesAnalysisA,
            out SqlBoolean MultipleBatches,
            out SqlDecimal RemeasureLength,
            out SqlString ProductType,
            out SqlInt32 SalesAnalysisD,
            out SqlInt32 StockAnalysisD,
            out SqlBoolean AllocateAtBatchLevel,
            out SqlString ValuationCostToUse,
            out SqlString FormCode,
            out SqlDecimal MaximumRemnantLength,
            out SqlString CurrentStatus,
            out SqlBoolean PieceAllocation,
            out SqlBoolean RepetitiveBatches,
            out SqlBoolean SampleProduct,
            out SqlBoolean PieceBatchOnPickList,
            out SqlBoolean PieceOnlyOrders,
            out SqlString GeneralLedgerDiscountGroupCode,
            out SqlDecimal CuttingLength,
            out SqlDecimal MinimumReplenishment,
            out SqlString BulkRack,
            out SqlBoolean AutoAllocateMultipleTimes,
            out SqlString OrderType,
            out SqlString Logo,
            out SqlBoolean FullOrderOnly,
            out SqlString ChargeOn,
            out SqlString Brand,
            out SqlString PickingListGroupCode,
            out SqlBoolean BespokeRug,
            out SqlString PartNumberRangeStart,
            out SqlString PartNumberRangeEnd,
            out SqlString PartNumberPrefix,
            out SqlString PartNumberRangeLastUsed,
            out SqlBoolean CreateEANCode

            )
        {
            var p = (ProductTemplate)obj;

            Code = p.Key?.TrimReduceTruncate(50);
            Name = p.Name?.TrimReduceTruncate(50);
            FirstUserScreen = p.FirstUserScreen?.TrimReduceTruncate(20);
            ProductGroupCode = p.ProductGroupCode?.TrimReduceTruncate(5);
            ItemTypeCode = p.ItemTypeCode?.TrimReduceTruncate(5);
            UnitOfPurchase = p.UnitOfPurchase?.TrimReduceTruncate(10);
            UnitOfStock = p.UnitOfStock?.TrimReduceTruncate(10);
            VATCode = p.VATCode?.TrimReduceTruncate(10);
            PieceLevelStock = p.PieceLevelStock;
            SalesAnalysisA = p.SalesAnalysisA ?? SqlInt32.Null;
            MultipleBatches = p.MultipleBatches;
            RemeasureLength = p.RemeasureLength ?? SqlDecimal.Null;
            ProductType = p.ProductType?.TrimReduceTruncate(10);
            SalesAnalysisD = p.SalesAnalysisD ?? SqlInt32.Null;
            StockAnalysisD = p.StockAnalysisD ?? SqlInt32.Null;
            AllocateAtBatchLevel = p.AllocateAtBatchLevel;
            ValuationCostToUse = p.ValuationCostToUse?.TrimReduceTruncate(1);
            FormCode = p.FormCode?.TrimReduceTruncate(5);
            MaximumRemnantLength = p.MaximumRemnantLength ?? SqlDecimal.Null;
            CurrentStatus = p.CurrentStatus?.TrimReduceTruncate(1);
            PieceAllocation = p.PieceAllocation;
            RepetitiveBatches = p.RepetitiveBatches;
            SampleProduct = p.SampleProduct;
            PieceBatchOnPickList = p.PieceBatchOnPickList;
            PieceOnlyOrders = p.PieceOnlyOrders;
            GeneralLedgerDiscountGroupCode = p.GeneralLedgerDiscountGroupCode.TrimReduceTruncate(20);
            CuttingLength = p.CuttingLength ?? SqlDecimal.Null;
            MinimumReplenishment = p.MinimumReplenishment ?? SqlDecimal.Null;
            BulkRack = p.BulkRack?.TrimReduceTruncate(20);
            AutoAllocateMultipleTimes = p.AutoAllocateMultipleTimes;
            OrderType = p.OrderType?.TrimReduceTruncate(10);
            Logo = p.Logo?.TrimReduceTruncate(1);
            FullOrderOnly = p.FullOrderOnly;
            ChargeOn = p.ChargeOn?.TrimReduceTruncate(1);
            Brand = p.Brand?.TrimReduceTruncate(3);
            PickingListGroupCode = p.PickingListGroupCode?.TrimReduceTruncate(5);
            BespokeRug = p.BespokeRug;
            PartNumberRangeStart = p.PartNumberRangeStart?.TrimReduceTruncate(5);
            PartNumberRangeEnd = p.PartNumberRangeEnd?.TrimReduceTruncate(5);
            PartNumberPrefix = p.PartNumberPrefix?.TrimReduceTruncate(10);
            PartNumberRangeLastUsed = p.PartNumberRangeLastUsed?.TrimReduceTruncate(10);
            CreateEANCode = p.CreateEANCodes;
        }
    }
}