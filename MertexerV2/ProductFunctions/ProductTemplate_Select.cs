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
            out SqlString GroupCode,
            out SqlString ItemType,
            out SqlString UnitOfPurchase,
            out SqlString UnitOfStock,
            out SqlString VATCode,
            out SqlString PieceLevelStock,
            out SqlString SalesAnalysisA,
            out SqlBoolean MultipleBatches,
            out SqlDecimal RemeasureLength,
            out SqlString ProductType,
            out SqlString SalesAnalysisD,
            out SqlString StockAnalysisD,
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
            out SqlString ProductClass2,
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
            out SqlString RangeName,
            out SqlString LastRange,
            out SqlBoolean CreateEANCode

            )
        {
            var p = (ProductTemplate)obj;

            Code = p.Key?.TrimReduceTruncate(50);
            Name = p.Name?.TrimReduceTruncate(50);
            FirstUserScreen = p.FirstUserScreen?.TrimReduceTruncate(50);
            GroupCode = p.GroupCode?.TrimReduceTruncate(50);
            ItemType = p.ItemType?.TrimReduceTruncate(50);
            UnitOfPurchase = p.UnitOfPurchase?.TrimReduceTruncate(50);
            UnitOfStock = p.UnitOfStock?.TrimReduceTruncate(50);
            VATCode = p.VATCode?.TrimReduceTruncate(50);
            PieceLevelStock = p.PieceLevelStock?.TrimReduceTruncate(50);
            SalesAnalysisA = p.SalesAnalysisA?.TrimReduceTruncate(1);
            MultipleBatches = p.MultipleBatches;
            RemeasureLength = p.RemeasureLength ?? SqlDecimal.Null;
            ProductType = p.ProductType?.TrimReduceTruncate(10);
            SalesAnalysisD = p.SalesAnalysisD?.TrimReduceTruncate(50);
            StockAnalysisD = p.StockAnalysisD?.TrimReduceTruncate(50);
            AllocateAtBatchLevel = p.AllocateAtBatchLevel;
            ValuationCostToUse = p.ValuationCostToUse?.TrimReduceTruncate(50);
            FormCode = p.FormCode?.TrimReduceTruncate(10);
            MaximumRemnantLength = p.MaximumRemnantLength ?? SqlDecimal.Null;
            CurrentStatus = p.CurrentStatus?.TrimReduceTruncate(5);
            PieceAllocation = p.PieceAllocation;
            RepetitiveBatches = p.RepetitiveBatches;
            SampleProduct = p.SampleProduct;
            PieceBatchOnPickList = p.PieceBatchOnPickList;
            PieceOnlyOrders = p.PieceOnlyOrders;
            ProductClass2 = p.ProductClass2.TrimReduceTruncate(10);
            CuttingLength = p.CuttingLength ?? SqlDecimal.Null;
            MinimumReplenishment = p.MinimumReplenishment ?? SqlDecimal.Null;
            BulkRack = p.BulkRack?.TrimReduceTruncate(10);
            AutoAllocateMultipleTimes = p.AutoAllocateMultipleTimes;
            OrderType = p.OrderType?.TrimReduceTruncate(10);
            Logo = p.Logo?.TrimReduceTruncate(1);
            FullOrderOnly = p.FullOrderOnly;
            ChargeOn = p.ChargeOn?.TrimReduceTruncate(1);
            Brand = p.Brand?.TrimReduceTruncate(10);
            PickingListGroupCode = p.PickingListGroupCode?.TrimReduceTruncate(10);
            BespokeRug = p.BespokeRug;
            PartNumberRangeStart = p.PartNumberRangeStart?.TrimReduceTruncate(10);
            PartNumberRangeEnd = p.PartNumberRangeEnd?.TrimReduceTruncate(10);
            RangeName = p.RangeName?.TrimReduceTruncate(10);
            LastRange = p.LastRange?.TrimReduceTruncate(10);
            CreateEANCode = p.CreateEANCodes;
        }
    }
}