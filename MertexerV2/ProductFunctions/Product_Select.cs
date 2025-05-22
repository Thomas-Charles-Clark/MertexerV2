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

namespace MertexerV2.ProductFunctions
{
    public static partial class ProductFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "ProductSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string ProductCodes)
        {
            if (String.IsNullOrEmpty(ProductCodes))
            {
                foreach (var r in Selector<Product>.SelectAllNonParallel(
                    true,
                    false,
                    50,
                    UniVerseAccount))
                {
                    yield return r;
                }
            }
            else
            {
                var results = Selector<Product>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<ProductCode>>(ProductCodes).Select(p => p.Code),
                    true,
                    false,
                    50,
                    UniVerseAccount)
                    .ToList();

                foreach( var r in results)
                {
                    yield return r;
                }
            }
        }

        public static void ProductSelect_FillRow(
            object obj,
            out SqlString ProductCode,
            out SqlString Description,
            out SqlString ProductGroup,
            out SqlString ItemType,
            out SqlString MertexSearchDescription,
            out SqlString UnitOfPurchase,
            out SqlString UnitOfStock,
            out SqlString VATCode,
            out SqlString DesignName,
            out SqlBoolean PieceLevelStock,
            out SqlString Rack,
            out SqlString SalesAnaylsisA,
            out SqlDecimal MinimumOrderQuantity,
            out SqlBoolean MultipleBatches,
            out SqlString LastCostPrices, // HERE
            out SqlDecimal LatestCostPrice,
            out SqlString Quality,
            out SqlString Composition,
            out SqlDecimal FinishedWidth,
            out SqlString SupplierCodes, // HERE
            out SqlString SupplierCodePrimary,
            out SqlString SupplierPartNumber,
            out SqlByte LeadTime,
            out SqlString CustomerServiceNotes, // HERE
            out SqlDecimal VerticalPatternRepeat,
            out SqlDecimal NetWeight,
            out SqlDecimal HorizontalPatternRepeat,
            out SqlString ExportCommodityCode,
            out SqlString CountryOfOrigin,
            out SqlString SupplierUnits,
            out SqlString ConversionFactor,
            out SqlString AftercareCodes, // HERE
            out SqlString ProductTypeTextFormula,
            out SqlString SampleLabelText,
            out SqlString SalesAnalysisD,
            out SqlString StockAnalysisD,
            out SqlString FullDescriptions, // HERE
            out SqlBoolean AllocateAtBatchLevel,
            out SqlString ValuationCostToUse,
            out SqlString PatternMatchCode,
            out SqlString UKFlameRetardancy,
            out SqlString ContractFlameRetardancy,
            out SqlString FormCode,
            out SqlString PatternBook,
            out SqlDateTime LaunchDT,
            out SqlString ManufacturersID,
            out SqlString StockCode,
            out SqlString UpholsteryGrade,
            out SqlString UsageCodes, // HERE
            out SqlDecimal MaximumRemnantLength,
            out SqlString Status,
            out SqlBoolean PieceAllocation,
            out SqlBoolean RepetitiveBatches,
            out SqlBoolean SampleProduct,
            out SqlDecimal DropPatternRepeat,
            out SqlString PieceBatchOnPickList,
            out SqlString PaintVOC,
            out SqlString EAN,
            out SqlString ImportCommodityCode,
            out SqlString USCommodityCode,
            out SqlString FlamCode,
            out SqlBoolean PieceOnlyOrders,
            out SqlString LinkedDocuments, // HERE
            out SqlString LinkedDocumentDescriptions, // HERE
            out SqlString TestCodes, // HERE
            out SqlString Type,
            out SqlString Supplier,
            out SqlString MylandsProductCode,
            out SqlString ProductClass2,
            out SqlString ProductClass3,
            out SqlDecimal CuttingLength,
            out SqlDecimal MinimumReplenishment,
            out SqlString BulkRack,
            out SqlString B2BCode,
            out SqlBoolean AutoAllocateMultipleTimes,
            out SqlString FlameRetardant,
            out SqlDecimal ClearanceDiscount,
            out SqlInt32 Martindale,
            out SqlByte ShippingLeadTimeWorkingDays,
            out SqlString OrderType,
            out SqlBoolean WebProduct,
            out SqlString LabelLogo,
            out SqlBoolean FullOrderOnly,
            out SqlString ChargeOn,
            out SqlString Brand,
            out SqlString Collection,
            out SqlString PickListGroup,
            out SqlString CutterInstructions, // HERE
            out SqlString MylandsBaseProductCode,
            out SqlString ProductDeveloperCode,
            out SqlString DesignCode,
            out SqlString SupplierDesignCodes, // HERE
            out SqlString SupplierColourNames, // HERE
            out SqlBoolean BespokeRug,
            out SqlString RugPileHeight,
            out SqlString RugPileWeight,
            out SqlBoolean IncludeInPatternBookAnalysis,
            out SqlString AdditionalProduct,
            out SqlDecimal CostWithComponents,
            out SqlString ProductBuyer,
            out SqlDateTime MostRecentCarryForwardDT,
            out SqlString PriceListDescription,
            out SqlString PriceListPackSize,
            out SqlString AdditionalText,
            out SqlBoolean LaunchProduct,
            out SqlString Market,
            out SqlString PartNumberRangeStart,
            out SqlString PartNumberRangeEnd,
            out SqlString RangeName,
            out SqlString LastRange,
            out SqlBoolean CreateEANCodes,
            out SqlString PurchaseOrderEmailAddress,
            out SqlString BasketName,
            out SqlInt16 ProductType,
            out SqlString ColourwayName,
            out SqlDecimal CostInCurrency,
            out SqlDecimal CostPriceTarget,
            out SqlString Currency,
            out SqlDecimal ExchangeRate,
            out SqlDecimal FinalCostPriceSterling,
            out SqlDecimal CostPriceExcludingAdditional,
            out SqlDecimal UKRRPIncVAT,
            out SqlDecimal UKRRPExVAT,
            out SqlDecimal EuroTradeExVAT,
            out SqlDecimal UKGrossMarginPercentage,
            out SqlString ProductDeveloperComments,
            out SqlInt32 MerchandiserCode,
            out SqlInt16 OrderFormGroup,
            out SqlString PackagingDescription,
            out SqlString DeliveryTerms,
            out SqlInt16 KnotsPerSquareMetre,
            out SqlDecimal WeightPerSquareMetre,
            out SqlString Transport,
            out SqlDecimal CostPerSquareMetreCurrency,
            out SqlDecimal CostPerSquareMetreSterling,
            out SqlDecimal SwatchCost,
            out SqlInt16 SalesQuantityForecast_Wholesale,
            out SqlInt16 SalesQuantityForecast_eCommerce,
            out SqlInt16 SalesQuantityForecast_Retail,
            out SqlInt16 SalesQuantityForecast_DGInc,
            out SqlDecimal LengthCM,
            out SqlDecimal WidthCM,
            out SqlDecimal HeightCM,
            out SqlDecimal LengthInches,
            out SqlDecimal WidthInches,
            out SqlDecimal HeightInches,
            out SqlString SizeDescription,
            out SqlDecimal LengthFeet,
            out SqlDecimal WidthFeet,
            out SqlDecimal HeightFeet,
            out SqlBoolean AttachLithography,
            out SqlString SampleCuttingSize,
            out SqlString ReturnableSize,
            out SqlBoolean DigitalDesign,
            out SqlBoolean WideWidth,
            out SqlBoolean SpecialColour,
            out SqlDecimal DiameterCM,
            out SqlBoolean OekoTexStandard,
            out SqlBoolean MasterApproved,
            out SqlString PhotoType,
            out SqlBoolean EngineeredPanel,
            out SqlDecimal VerticalPatternRepeatInches,
            out SqlDecimal HorizontalPatternRepeatInches,
            out SqlDecimal AdditionalProductCostSterling,
            out SqlString WebPattern,
            out SqlByte MaximumFoldSize,
            out SqlBoolean BespokeHanger,
            out SqlBoolean BespokeWaterfall,
            out SqlBoolean BespokeShowlength,
            out SqlBoolean PasteTheWall,
            out SqlDecimal RollLength,
            out SqlBoolean Leather,
            out SqlString ProductTypeDescription,
            out SqlString LinkedDesignDifferentWidth,
            out SqlBoolean ShowlengthInStore,
            out SqlDecimal RollCoverage,
            out SqlString StockHanger,
            out SqlString ShowlengthText,
            out SqlDecimal PaintCoverageSquareMetre,
            out SqlString PaintQualities,
            out SqlString OrderCutOffTime,
            out SqlByte PaintSheenPercentage,
            out SqlString PaintCoverageText,
            out SqlString PaintSamplePotCode,
            out SqlByte PaintMinimumCoats,
            out SqlString JLStockReference,
            out SqlBoolean Reversible,
            out SqlString WallpaperHangingInstructions,
            out SqlDateTime RelaunchedDT,
            out SqlBoolean CheckWeight,
            out SqlString PaintBase,
            out SqlString PaintUndercoatColour,
            out SqlString PaintColourNumber,
            out SqlString Paint1LitreUndercoatPartNumber,
            out SqlString Paint2PointFiveLitreUndercoatPartNumber,
            out SqlString CountryOfManufacture,
            out SqlString CountryWhereStockHeld,
            out SqlByte LeadTimeWorkingDays,
            out SqlByte LeadSW,
            out SqlString FurnitureSeatType,
            out SqlString FurnitureCategoryCode,
            out SqlString ImageType,
            out SqlString PaintFinish,
            out SqlString LinkedBespokeHangerPartNumber,
            out SqlDecimal BulkClearancePriceSterling,
            out SqlString LinkedBespokeRugPartNumber,
            out SqlString LinkedBespokeRugSamplePartNumber,
            out SqlString LinkedA6PaintCardPartNumber,
            out SqlDecimal VolumetricWeight,
            out SqlDecimal VolumetricLengthCM,
            out SqlDecimal VolumetricWidthCM,
            out SqlDecimal VolumetricHeightCM,
            out SqlString PaintType,
            out SqlString PoleSize,
            out SqlString CustomsCommodityDescription,
            out SqlString JLStockCodeMTM,
            out SqlString FRLPartNumber,
            out SqlString FoliaPartNumber,
            out SqlString PriceListDeliveryBand,
            out SqlString SpareField11,
            out SqlString SpareField12,
            out SqlString SpareField13,
            out SqlString SpareField14
            )
        {
            var p = (Product)obj;

            ProductCode = p.Key?.TrimReduceTruncate(50);
            Description = p.Description?.TrimReduceTruncate(200);
            ProductGroup = p.ProductGroup?.TrimReduceTruncate(2);
            ItemType = p.ItemType?.TrimReduceTruncate(3);
            MertexSearchDescription = p.MertexSearchDescription?.TrimReduceTruncate(200);
            UnitOfPurchase = p.UnitOfPurchase?.TrimReduceTruncate(10);
            UnitOfStock = p.UnitOfStock?.TrimReduceTruncate(10);
            VATCode = p.VATCode?.TrimReduceTruncate(1);
            DesignName = p.DesignName?.TrimReduceTruncate(100);
            PieceLevelStock = p.PieceLevelStock;
            Rack = p.Rack?.TrimReduceTruncate(20);
            SalesAnaylsisA = p.SalesAnaylsisA?.TrimReduceTruncate(20);
            MinimumOrderQuantity = p.MinimumOrderQuantity ?? SqlDecimal.Null;
            MultipleBatches = p.MultipleBatches;
            LastCostPrices = p.LastCostPrices == null ? SqlString.Null : JsonConvert.SerializeObject(p.LastCostPrices);
            LatestCostPrice = p.LatestCostPrice ?? SqlDecimal.Null;
            Quality = p.Quality?.TrimReduceTruncate(50);
            Composition = p.Composition?.TrimReduceTruncate(20);
            FinishedWidth = p.FinishedWidth ?? SqlDecimal.Null;
            SupplierCodes = p.SupplierCodes == null ? SqlString.Null : JsonConvert.SerializeObject(p.SupplierCodes);
            SupplierCodePrimary = p.SupplierCodePrimary?.TrimReduceTruncate(20);
            SupplierPartNumber = p.SupplierPartNumber?.TrimReduceTruncate(100);
            LeadTime = p.LeadTime ?? SqlByte.Null;
            CustomerServiceNotes = p.CustomerServiceNotes == null ? SqlString.Null : JsonConvert.SerializeObject(p.CustomerServiceNotes);
            VerticalPatternRepeat = p.VerticalPatternRepeat ?? SqlDecimal.Null;
            NetWeight = p.NetWeight ?? SqlDecimal.Null;
            HorizontalPatternRepeat = p.HorizontalPatternRepeat ?? SqlDecimal.Null;
            ExportCommodityCode = p.ExportCommodityCode?.TrimReduceTruncate(20);
            CountryOfOrigin = p.CountryOfOrigin?.TrimReduceTruncate(2);
            SupplierUnits = p.SupplierUnits?.TrimReduceTruncate(10);
            ConversionFactor = p.ConversionFactor?.TrimReduceTruncate(10);
            AftercareCodes = p.AftercareCodes == null ? SqlString.Null : JsonConvert.SerializeObject(p.AftercareCodes);
            ProductTypeTextFormula = p.ProductTypeTextFormula?.TrimReduceTruncate(5);
            SampleLabelText = p.SampleLabelText?.TrimReduceTruncate(300);
            SalesAnalysisD = p.SalesAnalysisD?.TrimReduceTruncate(20);
            StockAnalysisD = p.StockAnalysisD?.TrimReduceTruncate(20);
            FullDescriptions = p.FullDescriptions == null ? SqlString.Null :  JsonConvert.SerializeObject(p.FullDescriptions);
            AllocateAtBatchLevel = p.AllocateAtBatchLevel;
            ValuationCostToUse = p.ValuationCostToUse?.TrimReduceTruncate(1);
            PatternMatchCode = p.PatternMatchCode?.TrimReduceTruncate(50);
            UKFlameRetardancy = p.UKFlameRetardancy?.TrimReduceTruncate(5);
            ContractFlameRetardancy = p.ContractFlameRetardancy?.TrimReduceTruncate(5);
            FormCode = p.FormCode?.TrimReduceTruncate(5);
            PatternBook = p.PatternBook?.TrimReduceTruncate(20);
            LaunchDT = p.LaunchDT ?? SqlDateTime.Null;
            ManufacturersID = p.ManufacturersID?.TrimReduceTruncate(50);
            StockCode = p.StockCode?.TrimReduceTruncate(20);
            UpholsteryGrade = p.UpholsteryGrade?.TrimReduceTruncate(300);
            UsageCodes = p.UsageCodes == null ? SqlString.Null : JsonConvert.SerializeObject(p.UsageCodes);
            MaximumRemnantLength = p.MaximumRemnantLength ?? SqlDecimal.Null;
            Status = p.Status?.TrimReduceTruncate(1);
            PieceAllocation = p.PieceAllocation;
            RepetitiveBatches = p.RepetitiveBatches;
            SampleProduct = p.SampleProduct;
            DropPatternRepeat = p.DropPatternRepeat ?? SqlDecimal.Null;
            PieceBatchOnPickList = p.PieceBatchOnPickList?.TrimReduceTruncate(1);
            PaintVOC = p.PaintVOC?.TrimReduceTruncate(100);
            EAN = p.EAN?.TrimReduceTruncate(20);
            ImportCommodityCode = p.ImportCommodityCode?.TrimReduceTruncate(20);
            USCommodityCode = p.USCommodityCode?.TrimReduceTruncate(20);
            FlamCode = p.FlamCode?.TrimReduceTruncate(5);
            PieceOnlyOrders = p.PieceOnlyOrders;
            LinkedDocuments = p.LinkedDocuments == null ? SqlString.Null : JsonConvert.SerializeObject(p.LinkedDocuments);
            LinkedDocumentDescriptions = p.LinkedDocumentDescriptions == null ? SqlString.Null : JsonConvert.SerializeObject(p.LinkedDocumentDescriptions);
            TestCodes = p.TestCodes == null ? SqlString.Null : JsonConvert.SerializeObject(p.TestCodes);
            Type = p.Type?.TrimReduceTruncate(30);
            Supplier = p.Supplier?.TrimReduceTruncate(20);
            MylandsProductCode = p.MylandsProductCode?.TrimReduceTruncate(50);
            ProductClass2 = p.ProductClass2?.TrimReduceTruncate(20);
            ProductClass3 = p.ProductClass2?.TrimReduceTruncate(50);
            CuttingLength = p.CuttingLength ?? SqlDecimal.Null;
            MinimumReplenishment = p.MinimumReplenishment ?? SqlDecimal.Null;
            BulkRack = p.BulkRack?.TrimReduceTruncate(20);
            B2BCode = p.B2BCode?.TrimReduceTruncate(1);
            AutoAllocateMultipleTimes = p.AutoAllocateMultipleTimes;
            FlameRetardant = p.FlameRetardant?.TrimReduceTruncate(10);
            ClearanceDiscount = p.ClearanceDiscount ?? SqlDecimal.Null;
            Martindale = p.Martindale ?? SqlInt32.Null;
            ShippingLeadTimeWorkingDays = p.ShippingLeadTimeWorkingDays ?? SqlByte.Null;
            OrderType = p.OrderType?.TrimReduceTruncate(5);
            WebProduct = p.WebProduct;
            LabelLogo = p.LabelLogo?.TrimReduceTruncate(1);
            FullOrderOnly = p.FullOrderOnly;
            ChargeOn = p.ChargeOn?.TrimReduceTruncate(1);
            Brand = p.Brand?.TrimReduceTruncate(3);
            Collection = p.Collection?.TrimReduceTruncate(10);
            PickListGroup = p.PickListGroup?.TrimReduceTruncate(1);
            CutterInstructions = p.CutterInstructions == null ? SqlString.Null : JsonConvert.SerializeObject(p.CutterInstructions);
            MylandsBaseProductCode = p.MylandsBaseProductCode?.TrimReduceTruncate(20);
            ProductDeveloperCode = p.ProductDeveloperCode?.TrimReduceTruncate(20);
            DesignCode = p.DesignCode?.TrimReduceTruncate(100);
            SupplierDesignCodes = p.SupplierDesignCodes == null ? SqlString.Null : JsonConvert.SerializeObject(p.SupplierDesignCodes);
            SupplierColourNames = p.SupplierColourNames == null ? SqlString.Null : JsonConvert.SerializeObject(p.SupplierColourNames);
            BespokeRug = p.BespokeRug;
            RugPileHeight = p.RugPileHeight?.TrimReduceTruncate(10);
            RugPileWeight = p.RugPileWeight?.TrimReduceTruncate(10);
            IncludeInPatternBookAnalysis = p.IncludeInPatternBookAnalysis;
            AdditionalProduct = p.AdditionalProduct?.TrimReduceTruncate(50);
            CostWithComponents = p.CostWithComponents ?? SqlDecimal.Null;
            ProductBuyer = p.ProductBuyer?.TrimReduceTruncate(3);
            MostRecentCarryForwardDT = p.MostRecentCarryForwardDT ?? SqlDateTime.Null;
            PriceListDescription = p.PriceListDescription?.TrimReduceTruncate(250);
            PriceListPackSize = p.PriceListPackSize?.TrimReduceTruncate(5);
            AdditionalText = p.AdditionalText?.TrimReduceTruncate(250);
            LaunchProduct = p.LaunchProduct;
            Market = p.Market?.TrimReduceTruncate(3);
            PartNumberRangeStart = p.PartNumberRangeStart?.TrimReduceTruncate(10);
            PartNumberRangeEnd = p.PartNumberRangeEnd?.TrimReduceTruncate(10);
            RangeName = p.PartNumberRangeName?.TrimReduceTruncate(10);
            LastRange = p.PartNumberLastRange?.TrimReduceTruncate(10);
            CreateEANCodes = p.CreateEANCodes;
            PurchaseOrderEmailAddress = p.PurchaseOrderEmailAddress?.TrimReduceTruncate(254);
            BasketName = p.BasketName?.TrimReduceTruncate(200);
            ProductType = p.ProductType ?? SqlInt16.Null;
            ColourwayName = p.ColourwayName?.TrimReduceTruncate(100);
            CostInCurrency = p.CostInCurrency ?? SqlDecimal.Null;
            CostPriceTarget = p.CostPriceTarget ?? SqlDecimal.Null;
            Currency = p.Currency?.TrimReduceTruncate(20);
            ExchangeRate = p.ExchangeRate ?? SqlDecimal.Null;
            FinalCostPriceSterling = p.FinalCostPriceSterling ?? SqlDecimal.Null;
            CostPriceExcludingAdditional = p.CostPriceExcludingAdditional ?? SqlDecimal.Null;
            UKRRPIncVAT = p.UKRRPIncVAT ?? SqlDecimal.Null;
            UKRRPExVAT = p.UKRRPExVAT ?? SqlDecimal.Null;
            EuroTradeExVAT = p.EuroTradeExVAT ?? SqlDecimal.Null;
            UKGrossMarginPercentage = p.UKGrossMarginPercentage ?? SqlDecimal.Null;
            ProductDeveloperComments = p.ProductDeveloperComments?.TrimReduceTruncate(200);
            MerchandiserCode = p.MerchandiserCode ?? SqlInt32.Null;
            OrderFormGroup = p.OrderFormGroup ?? SqlInt16.Null;
            PackagingDescription = p.PackagingDescription?.TrimReduceTruncate(200);
            DeliveryTerms = p.DeliveryTerms?.TrimReduceTruncate(3);
            KnotsPerSquareMetre = p.KnotsPerSquareMetre ?? SqlInt16.Null;
            WeightPerSquareMetre = p.WeightPerSquareMetre ?? SqlDecimal.Null;
            Transport = p.Transport?.TrimReduceTruncate(20);
            CostPerSquareMetreCurrency = p.CostPerSquareMetreCurrency ?? SqlDecimal.Null;
            CostPerSquareMetreSterling = p.CostPerSquareMetreSterling ?? SqlDecimal.Null;
            SwatchCost = p.SwatchCost ?? SqlDecimal.Null;
            SalesQuantityForecast_Wholesale = p.SalesQuantityForecast_Wholesale ?? SqlInt16.Null;
            SalesQuantityForecast_eCommerce = p.SalesQuantityForecast_eCommerce ?? SqlInt16.Null;
            SalesQuantityForecast_Retail = p.SalesQuantityForecast_Retail ?? SqlInt16.Null;
            SalesQuantityForecast_DGInc = p.SalesQuantityForecast_DGInc ?? SqlInt16.Null;
            LengthCM = p.LengthCM ?? SqlDecimal.Null;
            WidthCM = p.WidthCM ?? SqlDecimal.Null;
            HeightCM = p.HeightCM ?? SqlDecimal.Null;
            LengthInches = p.LengthInches ?? SqlDecimal.Null;
            WidthInches = p.WidthInches ?? SqlDecimal.Null;
            HeightInches = p.HeightInches ?? SqlDecimal.Null;
            SizeDescription = p.SizeDescription?.TrimReduceTruncate(50);
            LengthFeet = p.LengthFeet ?? SqlDecimal.Null;
            WidthFeet = p.WidthFeet ?? SqlDecimal.Null;
            HeightFeet = p.HeightFeet ?? SqlDecimal.Null;
            AttachLithography = p.AttachLithography;
            SampleCuttingSize = p.SampleCuttingSize?.TrimReduceTruncate(20);
            ReturnableSize = p.ReturnableSize?.TrimReduceTruncate(20);
            DigitalDesign = p.DigitalDesign;
            WideWidth = p.WideWidth;
            SpecialColour = p.SpecialColour;
            DiameterCM = p.DiameterCM ?? SqlDecimal.Null;
            OekoTexStandard = p.OekoTexStandard;
            MasterApproved = p.MasterApproved;
            PhotoType = p.PhotoType?.TrimReduceTruncate(20);
            EngineeredPanel = p.EngineeredPanel;
            VerticalPatternRepeatInches = p.VerticalPatternRepeatInches ?? SqlDecimal.Null;
            HorizontalPatternRepeatInches = p.HorizontalPatternRepeatInches ?? SqlDecimal.Null;
            AdditionalProductCostSterling = p.AdditionalProductCostSterling ?? SqlDecimal.Null;
            WebPattern = p.WebPattern?.TrimReduceTruncate(1);
            MaximumFoldSize = p.MaximumFoldSize ?? SqlByte.Null;
            BespokeHanger = p.BespokeHanger;
            BespokeWaterfall = p.BespokeWaterfall;
            BespokeShowlength = p.BespokeShowlength;
            PasteTheWall = p.PasteTheWall;
            RollLength = p.RollLength ?? SqlDecimal.Null;
            Leather = p.Leather;
            ProductTypeDescription = p.ProductTypeDescription?.TrimReduceTruncate(100);
            LinkedDesignDifferentWidth = p.LinkedDesignDifferentWidth?.TrimReduceTruncate(12);
            ShowlengthInStore = p.ShowlengthInStore;
            RollCoverage = p.RollCoverage ?? SqlDecimal.Null;
            StockHanger = p.StockHanger?.TrimReduceTruncate(20);
            ShowlengthText = p.ShowlengthText?.TrimReduceTruncate(100);
            PaintCoverageSquareMetre = p.PaintCoverageSquareMetre ?? SqlDecimal.Null;
            PaintQualities = p.PaintQualities?.TrimReduceTruncate(200);
            OrderCutOffTime = p.OrderCutOffTime?.TrimReduceTruncate(100);
            PaintSheenPercentage = p.PaintSheenPercentage ?? SqlByte.Null;
            PaintCoverageText = p.PaintCoverageText?.TrimReduceTruncate(100);
            PaintSamplePotCode = p.PaintSamplePotCode?.TrimReduceTruncate(100);
            PaintMinimumCoats = p.PaintMinimumCoats ?? SqlByte.Null;
            JLStockReference = p.JLStockReference?.TrimReduceTruncate(50);
            Reversible = p.Reversible;
            WallpaperHangingInstructions = p.WallpaperHangingInstructions?.TrimReduceTruncate(100);
            RelaunchedDT = p.RelaunchedDT ?? SqlDateTime.Null;
            CheckWeight = p.CheckWeight;
            PaintBase = p.PaintBase?.TrimReduceTruncate(50);
            PaintUndercoatColour = p.PaintUndercoatColour?.TrimReduceTruncate(50);
            PaintColourNumber = p.PaintColourNumber?.TrimReduceTruncate(10);
            Paint1LitreUndercoatPartNumber = p.Paint1LitreUndercoatPartNumber?.TrimReduceTruncate(20);
            Paint2PointFiveLitreUndercoatPartNumber = p.Paint2PointFiveLitreUndercoatPartNumber?.TrimReduceTruncate(20);
            CountryOfManufacture = p.CountryOfManufacture?.TrimReduceTruncate(25);
            CountryWhereStockHeld = p.CountryWhereStockHeld?.TrimReduceTruncate(25);
            LeadTimeWorkingDays = p.LeadTimeWorkingDays ?? SqlByte.Null;
            LeadSW = p.LeadSW ?? SqlByte.Null;
            FurnitureSeatType = p.FurnitureSeatType?.TrimReduceTruncate(50);
            FurnitureCategoryCode = p.FurnitureCategoryCode?.TrimReduceTruncate(20);
            ImageType = p.ImageType?.TrimReduceTruncate(50);
            PaintFinish = p.PaintFinish?.TrimReduceTruncate(50);
            LinkedBespokeHangerPartNumber = p.LinkedBespokeHangerPartNumber?.TrimReduceTruncate(50);
            BulkClearancePriceSterling = p.BulkClearancePriceSterling ?? SqlDecimal.Null;
            LinkedBespokeRugPartNumber = p.LinkedBespokeRugPartNumber?.TrimReduceTruncate(50);
            LinkedBespokeRugSamplePartNumber = p.LinkedBespokeRugSamplePartNumber?.TrimReduceTruncate(50);
            LinkedA6PaintCardPartNumber = p.LinkedA6PaintCardPartNumber?.TrimReduceTruncate(50);
            VolumetricWeight = p.VolumetricWeight ?? SqlDecimal.Null;
            VolumetricLengthCM = p.VolumetricLengthCM ?? SqlDecimal.Null;
            VolumetricWidthCM = p.VolumetricWidthCM ?? SqlDecimal.Null;
            VolumetricHeightCM = p.VolumetricHeightCM ?? SqlDecimal.Null;
            PaintType = p.PaintType?.TrimReduceTruncate(50);
            PoleSize = p.PoleSize?.TrimReduceTruncate(1);
            CustomsCommodityDescription = p.CustomsCommodityDescription?.TrimReduceTruncate(100);
            JLStockCodeMTM = p.JLStockCodeMTM?.TrimReduceTruncate(50);
            FRLPartNumber = p.FRLPartNumber?.TrimReduceTruncate(50);
            FoliaPartNumber = p.FoliaPartNumber?.TrimReduceTruncate(50);
            PriceListDeliveryBand = p.PriceListDeliveryBand?.TrimReduceTruncate(10);
            SpareField11 = p.SpareField11?.TrimReduceTruncate(100);
            SpareField12 = p.SpareField12?.TrimReduceTruncate(100);
            SpareField13 = p.SpareField13?.TrimReduceTruncate(100);
            SpareField14 = p.SpareField14?.TrimReduceTruncate(100);
        }
    }
}
