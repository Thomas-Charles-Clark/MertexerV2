using MertexerV2.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.EDAReplacement
{
    public class STProdMaster
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string GroupCode { get; set; }
        public string ItemType { get; set; }
        public string JLStockReference { get; set; }
        public string NewJLStockReference { get; set; }
        public string ManufacturersID { get; set; }
        public string EUExportCommodityCodeDescription { get; set; }
        public long? EUExportCommodityCode { get; set; }
        public long? USCommodityCode { get; set; }
        public string OriginCountry { get; set; }
        public decimal? LatestCostPrice { get; set; }
        public string SupplierReference { get; set; }
        public string QualityCode { get; set; }
        public string CollectionCode { get; set; }
        public string StockCode { get; set; }
        public string RugSampleProductCode { get; set; }
        public string UpholsteryGrade { get; set; }
        public string DeliveryBand { get; set; }
        public byte? MaxFoldLength { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public decimal? Height { get; set; }
        public decimal? Diameter { get; set; }
        public string Size { get; set; }
        public string FlamCode { get; set; }
        public int? Martindale { get; set; }
        public decimal? HorizontalPatternRepeat { get; set; }
        public decimal? VerticalPatternRepeat { get; set; }
        public string AdditionalInformation { get; set; }
        public short? PileHeightMM { get; set; }
        public string BrandCode { get; set; }
        public string ColourName { get; set; }
        public string DesignName { get; set; }
        public string EAN { get; set; }
        public DateTime? LaunchDate { get; set; }
        public DateTime? CarryForwardDate { get; set; }
        public string OrderGroupTitle { get; set; }
        public string PriceListDescription { get; set; }
        public List<string> AdditionalProducts { get; set; }
        public string PaintColourNumber { get; set; }
        public string PaintFinish { get; set; }
        public string CLRDiscDesc { get; set; }
        public string SSDesign { get; set; }
        public string ImportCommodityCode { get; set; }
        public string PackagingDescription { get; set; }

        public STProdMaster(
            string productCode,
            string productName,
            string status,
            string groupCode,
            string itemType,
            string jlStockReference,
            string newJlStockReference,
            string manufacturersId,
            string euExportCommodityCode,
            string euExportCommodityCodeDescription,
            string usCommodityCode,
            string originCountry,
            string latestCostPrice,
            string supplierReference,
            string qualityCode,
            string collectionCode,
            string stockCode,
            string rugSampleProductCode,
            string upholsteryGrade,
            string deliveryBand,
            string maxFoldLength,
            string width,
            string length,
            string height,
            string diameter,
            string size,
            string flamCode,
            string martindale,
            string horizontalPatternRepeat,
            string verticalPatternRepeat,
            string additionalInformation,
            string pileHeightMm,
            string brandCode,
            string colourName,
            string designName,
            string ean,
            string launchDate,
            string carryForwardDate,
            string orderGroupTitle,
            string priceListDescription,
            string additionalProducts,
            string paintColourNumber,
            string paintFinish,
            string clrDiscDesc,
            string ssDesign,
            string importCommodityCode,
            string packagingDescription
            )
        {
            Code = String.IsNullOrEmpty(productCode) ? null : productCode;

            Name = String.IsNullOrEmpty(productName) ? null : productName;

            Status = String.IsNullOrEmpty(status) ? null : status;

            GroupCode = String.IsNullOrEmpty(groupCode) ? null : groupCode;

            ItemType = String.IsNullOrEmpty(itemType) ? null : itemType;

            JLStockReference = String.IsNullOrEmpty(jlStockReference) ? null : jlStockReference;

            NewJLStockReference = String.IsNullOrEmpty(newJlStockReference) ? null : newJlStockReference;

            ManufacturersID = String.IsNullOrEmpty(manufacturersId) ? null : manufacturersId;

            EUExportCommodityCodeDescription = String.IsNullOrEmpty(euExportCommodityCodeDescription) ? null : euExportCommodityCodeDescription;


            if (Int64.TryParse(euExportCommodityCode, out long i))
            {
                EUExportCommodityCode = i;
            }

            if (Int64.TryParse(usCommodityCode, out i))
            {
                USCommodityCode = i;
            }

            OriginCountry = String.IsNullOrEmpty(originCountry) ? null : originCountry;

            if (Decimal.TryParse(latestCostPrice, out decimal lcp))
            {
                LatestCostPrice = lcp;
            }

            SupplierReference = String.IsNullOrEmpty(supplierReference) ? null : supplierReference;

            QualityCode = String.IsNullOrEmpty(qualityCode) ? null : qualityCode;

            CollectionCode = String.IsNullOrEmpty(collectionCode) ? null : collectionCode;

            StockCode = String.IsNullOrEmpty(stockCode) ? null : stockCode;

            RugSampleProductCode = String.IsNullOrEmpty(rugSampleProductCode) ? null : rugSampleProductCode;

            UpholsteryGrade = String.IsNullOrEmpty(upholsteryGrade) ? null : upholsteryGrade;

            DeliveryBand = String.IsNullOrEmpty(deliveryBand) ? null : deliveryBand;

            if (Byte.TryParse(maxFoldLength, out byte mfl))
            {
                MaxFoldLength = mfl;
            }

            if (Decimal.TryParse(width, out decimal w))
            {
                Width = w;
            }

            if (Decimal.TryParse(length, out decimal l))
            {
                Length = l;
            }

            if (Decimal.TryParse(height, out decimal h))
            {
                Height = h;
            }

            if (Decimal.TryParse(diameter, out decimal d))
            {
                Diameter = d;
            }

            Size = String.IsNullOrEmpty(size) ? null : size;

            FlamCode = String.IsNullOrEmpty(flamCode) ? null : flamCode;

            if (Int32.TryParse(martindale, out int m))
            {
                Martindale = m;
            }

            if (Decimal.TryParse(horizontalPatternRepeat, out decimal hpr))
            {
                HorizontalPatternRepeat = hpr;
            }

            if (Decimal.TryParse(verticalPatternRepeat, out decimal vpr))
            {
                VerticalPatternRepeat = vpr;
            }

            AdditionalInformation = String.IsNullOrEmpty(additionalInformation) ? null : additionalInformation;

            if (pileHeightMm != null)
                pileHeightMm = pileHeightMm.Replace("MM", "");

            if (Int16.TryParse(pileHeightMm, out short ph))
            {
                PileHeightMM = ph;
            }

            BrandCode = String.IsNullOrEmpty(brandCode) ? null : brandCode;

            ColourName = String.IsNullOrEmpty(colourName) ? null : colourName;

            DesignName = String.IsNullOrEmpty(designName) ? null : designName;

            EAN = String.IsNullOrEmpty(ean) ? null : ean;

            if (Int32.TryParse(launchDate, out int ld))
            {
                LaunchDate = Utilities.Framework.DateTimeFromUniVerseOffset(ld);
            }

            if (Int32.TryParse(carryForwardDate, out int cfd))
            {
                CarryForwardDate = Utilities.Framework.DateTimeFromUniVerseOffset(cfd);
            }

            OrderGroupTitle = String.IsNullOrEmpty(orderGroupTitle) ? null : orderGroupTitle;

            PriceListDescription = String.IsNullOrEmpty(priceListDescription) ? null : priceListDescription;

            AdditionalProducts = String.IsNullOrEmpty(additionalProducts) ? new List<string>() : additionalProducts.Split(Framework.ValueMarkDelimiter).ToList();

            PaintColourNumber = String.IsNullOrEmpty(paintColourNumber) ? null : paintColourNumber;
            
            PaintFinish = String.IsNullOrEmpty(paintFinish) ? null : paintFinish;

            CLRDiscDesc = String.IsNullOrEmpty(clrDiscDesc) ? null : clrDiscDesc;

            SSDesign = String.IsNullOrEmpty(ssDesign) ? null : ssDesign;

            ImportCommodityCode = String.IsNullOrEmpty(importCommodityCode) ? null : importCommodityCode;

            PackagingDescription = String.IsNullOrEmpty(packagingDescription) ? null : packagingDescription;
    }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
