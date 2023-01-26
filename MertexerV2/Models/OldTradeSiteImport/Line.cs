using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MertexerV2.Models.OldTradeSiteImport
{
    public class Line
    {
        private static readonly char[] delimiter = { '|' };

        public string Key { get; set; }
        public string TradeAccountCode { get; set; }
        public short LineNumber { get; set; }
        public string ProductCode { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public string SampleIndicator { get; set; }
        public string ReserveIndicator { get; set; }
        public string BackOrderIndicator { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public string LineMemo { get; set; }
        public string CutCounts { get; set; }
        public string CutLengths { get; set; }
        public decimal UnitPriceExVAT { get; set; }
        public string CustomerOrderReference { get; set; }
        public string WithCutting { get; set; }
        public string SpecificBatch { get; set; }
        public string NonStockIndicator { get; set; }
        public string CutSingleBatchIndicators { get; set; }
        public decimal OrderLineDiscountValue { get; set; }
        public string LastUpdated { get; set; }

        public string Type
        {
            get
            {
                if (SampleIndicator == "C")
                {
                    return "SAMPLES CUTTING";
                }

                if (SampleIndicator == "A")
                {
                    return "RETURNABLE SAMPLE";
                }
                
                if (ReserveIndicator == "Y")
                {
                    if (WithCutting == "Y")
                    {
                        return "RESERVE WITH BATCH CUTTING";
                    }

                    return "RESERVE";
                }

                // Don't assume HANGER, SHOWLENGTH or WATERFALL if the part number doesn't seem to be Fabric or Paper
                if (ProductCode.StartsWith("F") || ProductCode.StartsWith("P"))
                {
                    if (ProductCode.EndsWith("/H"))
                    {
                        return "HANGER";
                    }

                    if (ProductCode.EndsWith("/S"))
                    {
                        return "SHOWLENGTH";
                    }

                    if (ProductCode.EndsWith("/W"))
                    {
                        return "WATERFALL ONLY";
                    }
                }

                return "PRODUCT";
            }
        }

        [JsonIgnore]
        public string ToJSON
        {
            get
            {
                return JsonConvert.SerializeObject(this);
            }
        }

        public Line (string key,
            string tradeAccountCode,
            string lineNumber,
            string productCode,
            string quantity,
            string unit,
            string sampleIndicator,
            string reserveIndicator,
            string backOrderIndicator,
            string discountPercentage,
            string lineMemo,
            string cutCounts,
            string cutLengths,
            string unitPriceExVat,
            string customerOrderReference,
            string withCutting,
            string specificBatch,
            string nonStockIndicator,
            string cutSingleBatchIndicators,
            string orderLineDiscountValue,
            string lastUpdated)
        {
            Key = key;

            TradeAccountCode = tradeAccountCode;

            if (Int16.TryParse(lineNumber, out _))
            {
                LineNumber = Int16.Parse(lineNumber);
            }

            ProductCode = productCode;

            if (Decimal.TryParse(quantity, out _))
            {
                Quantity = Decimal.Parse(quantity);
            }

            Unit = unit;

            SampleIndicator = sampleIndicator;

            ReserveIndicator = reserveIndicator;

            BackOrderIndicator = backOrderIndicator;

            if (Decimal.TryParse(discountPercentage, out _))
            {
                DiscountPercentage = Decimal.Parse(discountPercentage);
            }

            LineMemo = lineMemo;

            CutCounts = cutCounts;

            CutLengths = cutLengths;

            if (Decimal.TryParse(unitPriceExVat, out _))
            {
                UnitPriceExVAT = Decimal.Parse(unitPriceExVat);
            }

            CustomerOrderReference = customerOrderReference;

            WithCutting = withCutting;

            SpecificBatch = specificBatch;

            NonStockIndicator = nonStockIndicator;

            CutSingleBatchIndicators = cutSingleBatchIndicators;

            if (Decimal.TryParse(orderLineDiscountValue, out _))
            {
                OrderLineDiscountValue = Decimal.Parse(orderLineDiscountValue);
            }

            LastUpdated = lastUpdated;

            switch (Type)
            {
                // Not necessary in this use case - [portal].[fnc_OrderImportProductCode] performs the suffixing of SAMPLES CUTTING and RETURNABLE SAMPLE for us
                //case "SAMPLES CUTTING":
                //    ProductCode += "/C";
                //    break;
                //case "RETURNABLE SAMPLE":
                //    ProductCode += "/R";
                //    break;
                case "HANGER":
                    {
                        if (!productCode.EndsWith("/H"))
                        {
                            ProductCode = Regex.Replace(ProductCode, @"/.*", "/H");
                        }
                    }
                    break;
                case "SHOWLENGTH":
                    {
                        if (!productCode.EndsWith("/S"))
                        {
                            ProductCode = Regex.Replace(ProductCode, @"/.*", "/S");
                        }
                    }
                    break;
                case "WATERFALL ONLY":
                    {
                        if (!productCode.EndsWith("/W"))
                        {
                            ProductCode = Regex.Replace(ProductCode, @"/.*", "/W");
                        }
                    }
                    break;
                default:
                    break;
            }

            // Try and sort out the ItemCount, ItemLength and ItemSingleBatchIndicators into some sort of coherent memo addition

            if (!String.IsNullOrEmpty(CutCounts) &&
                !String.IsNullOrEmpty(CutLengths) &&
                !String.IsNullOrEmpty(CutSingleBatchIndicators) &&
                ProductCode.StartsWith("F"))
            {
                var counts = CutCounts.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                var cuts = CutLengths.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                var batchIndicators = CutSingleBatchIndicators.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                if ((counts.Length + cuts.Length + batchIndicators.Length) % 3 == 0)
                {
                    var cutPieces = "";

                    for (var i = 0; i< counts.Length; i++)
                    {
                        if (cuts[i] != "0")
                        {
                            cutPieces += $@"{counts[i]} x {cuts[i]}m piece(s) {(batchIndicators[i] == "Y" ? "all from the same batch" : "from any batch")}\n";
                        }
                    }

                    LineMemo = $@"\nCUT LENGTH BREAKDOWN REQUESTED:\n\n
{cutPieces}\n
~~~~ CUSTOMER MEMO BENEATH ~~~~\n
\n
{LineMemo}";
                }
//                else
//                {
//                    LineMemo = $@"\nAn attempt was made to interpret the old B2B site's cut piece breakdown. The attempt failed. Please report this incident to itsupport@designersguild.com\n\n
//\n
//~~~~ CUSTOMER MEMO BENEATH ~~~~\n
//\n
//{LineMemo}";
//                }
            }
        }
    }
}
