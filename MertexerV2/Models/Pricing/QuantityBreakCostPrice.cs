using MertexerV2.Models.Company;
using MertexerV2.Models.ExchangeRate;
using MertexerV2.Models.Pricing;
using MertexerV2.Models.Product;
using MertexerV2.Utilities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.GeneralFunctions.Pricing
{
    [UniVerseFileName("STPRODMASTER")]
    public class ProductStub : UniVerseSerializationBase<string>
    {
        public ProductStub(string key) : base(key)
        {

        }

        public ProductStub() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(ProductStub).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        /// <summary>
        /// Supplier Code (One)
        /// </summary>
        [UniVerseIndex(70,1)]
        [UniVerseFieldName("SUPPREF.1")]
        public string SupplierCode { get; set; }

        /// <summary>
        /// Quality
        /// </summary>
        [UniVerseIndex(57)]
        [UniVerseFieldName("QUALITY")]
        public string QualityCode { get; set; }

        /// <summary>
        /// Stock Code
        /// </summary>
        [UniVerseIndex(278)]
        [UniVerseFieldName("STOCKCODE")]
        public string StockCode { get; set; }

        /// <summary>
        /// Brand
        /// </summary>
        [UniVerseIndex(752)]
        [UniVerseFieldName("SALES.DIV.DES")]
        public string Brand { get; set; }
    }

    public class QuantityBreakCostPriceResult
    {
        public string ProductCode { get; set; }
        public string Brand { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal QuantityBreak { get; set; }
        public decimal PriceCurrency { get; set; }
        public decimal PriceSterling { get; set; }
        public decimal ImportCostsCurrency { get; set; }
        public decimal ImportCostsSterling { get; set; }
        public DateTime? LastAmendedDT { get; set; }
        public decimal CostPriceCurrency { get; set; }
        public decimal CostPriceSterling { get; set; }

        public QuantityBreakCostPriceResult(
            string productCode,
            string brand,
            string supplierCode,
            string supplierName,
            decimal exchangeRate,
            decimal quantityBreak,
            decimal priceCurrency,
            decimal priceSterling,
            decimal importCostsCurrency,
            decimal importCostsSterling,
            string amended,
            decimal importCostsTotalSterling,
            decimal importCostsTotalCurrency)
        {
            ProductCode = productCode;
            Brand = brand;
            SupplierCode = supplierCode;
            SupplierName = supplierName;
            ExchangeRate = exchangeRate;
            QuantityBreak = quantityBreak;
            PriceCurrency = priceCurrency;
            PriceSterling = priceSterling;
            ImportCostsCurrency = importCostsCurrency;
            ImportCostsSterling = importCostsSterling;
            LastAmendedDT = String.IsNullOrEmpty(amended) ? null : (DateTime?)Framework.DateTimeFromUniVerseOffset(Convert.ToInt32(amended));
            CostPriceSterling = importCostsTotalSterling;
            CostPriceCurrency = importCostsTotalCurrency;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Framework.IndentedFormatting);
        }
    }

    public static class QuantityBreakCostPrice
    {
        private static readonly object Locker = new object();

        public static IEnumerable<QuantityBreakCostPriceResult> Execute(IEnumerable<string> productCodes = null, string productGroupFilter = null, string brandFilter = null, string uniVerseAccount = "MERTEX")
        {
            var sw = new System.Diagnostics.Stopwatch();

            sw.Start();

            var results = new List<QuantityBreakCostPriceResult>();

            using (var ce = new CommandExecutor(uniVerseAccount))
            {
                var products = new List<ProductStub>();

                var filters = new List<Tuple<string, string, string>>()
                {
                    new Tuple<string, string, string>("CUR.STAT", "#", "\"D\"")
                };

                if (productCodes == null || productCodes.Any() == false)
                {
                    if (!String.IsNullOrEmpty(productGroupFilter) ||
                        !String.IsNullOrEmpty(brandFilter))
                    {
                        if (!String.IsNullOrEmpty(productGroupFilter))
                            filters.Add(new Tuple<string, string, string>("GROUPCODE", "EQUALS", $"\"{productGroupFilter}\""));

                        if (!String.IsNullOrEmpty(brandFilter))
                            filters.Add(new Tuple<string, string, string>("SALES.DIV.DES", "EQUALS", $"\"{brandFilter}\""));

                        products = Selector<ProductStub>.Filter(filters).ToList();
                    }
                    else
                    {
                        products = Selector<ProductStub>.SelectAll().ToList();
                    }
                }
                else
                {
                    products = Selector<ProductStub>.Select(productCodes).ToList();
                }

                var company = Selector<Company>.Select("1");

                var baseCurrency = company?.Currency;

                var supplierCodes = products.Select(t => t.SupplierCode).Distinct();

                supplierCodes = supplierCodes.Where(s => s != null).ToList();

                var suppliers = Selector<Supplier>.Select(supplierCodes, false, false);

                var exchangeRates = Selector<ExchangeRate>.SelectAll();

                foreach(var s in suppliers)
                {
                    s.ExchangeRate = exchangeRates.SingleOrDefault(er => er.Key == s.Currency);
                }

#if DEBUG
                sw.Stop();

                Console.WriteLine($"QuantityBreakCostPrice.Execute Stage One completed in {sw.Elapsed}");
                Console.WriteLine();
                Console.WriteLine($"QuantityBreakCostPrice.Execute Stage Two: Batch Processing");
                Console.WriteLine();

                sw.Reset();

                sw.Start();
#endif

//#if DEBUG
                var batches = ListExtension.Split(products, 1000);
//#else
//                var batches = ListExtension.Split(products, 200);
//#endif

                var poCostRecords = Selector<POCost>.SelectAll();

                var selectPriceListRecordsIndividually = false;

                var priceListRecords = new List<POPriceList>();

                if (productCodes != null && productCodes.Any() || products.Count < 1000)
                {
                    selectPriceListRecordsIndividually = true;
                }
                else
                {
                    priceListRecords = Selector<POPriceList>.SelectAll(false, false).ToList();
                }

                var batchCounter = 0;

#if DEBUG
                Console.WriteLine($"QuantityBreakCostPrice.Execute Stage Three saw {products.Count()} keys, splitting into {batches.Count} batches of size 1000");
                Console.WriteLine();
                Console.WriteLine($"QuantityBreakCostPrice.Execute Stage Three: Batch Processing");
                Console.WriteLine();
#endif

                Parallel.ForEach(batches, (batch) =>
                {
                    var thisBatchResults = new List<QuantityBreakCostPriceResult>();

                    foreach (var product in batch)
                    {
                        if (!String.IsNullOrEmpty(product.StockCode))
                        {
                            var stockProduct = Selector<ProductStub>.Select(product.StockCode);

                            if (stockProduct != null)
                            {
                                if (!String.IsNullOrEmpty(stockProduct.SupplierCode))
                                {
                                    product.SupplierCode = stockProduct.SupplierCode;
                                }

                                if (!String.IsNullOrEmpty(stockProduct.QualityCode))
                                {
                                    product.QualityCode = stockProduct.QualityCode;
                                }
                            }
                        }

                        var productCode = String.IsNullOrEmpty(product.StockCode) ? product.Key : product.StockCode;
                        var brand = product.Brand;
                        var supplierCode = product.SupplierCode;
                        var qualityCode = product.QualityCode;

                        var dutyPercentage = 0.0m;
                        var inlandChargesPercentage = 0.0m;
                        var packagingPercentage = 0.0m;
                        var freightCharge = 0.0m;
                        var inlandFreightCharge = 0.0m;

                        var priceCurrency = 0.0m;
                        var priceSterling = 0.0m;
                        var importCostsCurrency = 0.0m;
                        var importCostsSterling = 0.0m;
                        var amended = "";

                        var supplier = suppliers.SingleOrDefault(s => s.Key == supplierCode);

                        var supplierName = supplier?.Name;

                        if (supplier != null && supplier.ExchangeRate != null) // have found product records where the supplier is NULL...
                        {
                            // avoiding applying the conversions is necessary in this context because the calculations are done at the highest accuracy possible
                            var poCost = poCostRecords.SingleOrDefault(po => po.Key == $"{qualityCode}*\\{supplierCode}");

                            var priceKey = $"{supplierCode}*{productCode}";

                            var exchangeRate = supplier.ExchangeRate.Rate;

                            var poPriceListRec = selectPriceListRecordsIndividually 
                                ? Selector<POPriceList>.Select(priceKey, false, false) 
                                : priceListRecords.SingleOrDefault(plr => plr.Key == priceKey);

                            if (poPriceListRec == null) // try again but with the quality not the product code
                            {
                                priceKey = $"{supplierCode}*{qualityCode}";

                                poPriceListRec = selectPriceListRecordsIndividually
                                    ? Selector<POPriceList>.Select(priceKey, false, false)
                                    : priceListRecords.SingleOrDefault(plr => plr.Key == priceKey);
                            }

                            if (poPriceListRec != null)
                            {
                                if (poCost != null)
                                {
                                    if (poCost.DutyPercentage.HasValue)
                                    {
                                        dutyPercentage = poCost.DutyPercentage ?? 0 / 100; // What does that | "100" mean?
                                    }
                                    else
                                    {
                                        dutyPercentage = supplier.DutyPercentage ?? 0;
                                    }

                                    if (poCost.InlandCharges.HasValue)
                                    {
                                        inlandChargesPercentage = poCost.InlandCharges ?? 0 / 100;
                                    }
                                    else
                                    {
                                        inlandChargesPercentage = supplier.InlandChargesPercentage ?? 0;
                                    }

                                    if (poCost.PackagingCharges.HasValue)
                                    {
                                        packagingPercentage = poCost.PackagingCharges ?? 0 / 100;
                                    }
                                    else
                                    {
                                        packagingPercentage = supplier.MiscellaneousPercentage ?? 0;
                                    }

                                    if (poCost.FreightCharges.HasValue)
                                    {
                                        freightCharge = poCost.FreightCharges ?? 0 * 100; // this scaling is necessary, I hate Mertex...
                                    }
                                    else
                                    {
                                        freightCharge = supplier.MiscellaneousCharge ?? 0; // this scaling is necessary, I hate Mertex...
                                    }

                                    if (poCost.QuotaLevy.HasValue)
                                    {
                                        inlandFreightCharge = poCost.QuotaLevy ?? 0 * 100;
                                    }
                                    else
                                    {
                                        inlandFreightCharge = supplier.MiscellaneousCharge ?? 0; // the hell... why is this... 
                                    }
                                }
                                else
                                {
                                    dutyPercentage = supplier.DutyPercentage ?? 0;
                                    inlandChargesPercentage = supplier.InlandChargesPercentage ?? 0;
                                    packagingPercentage = supplier.MiscellaneousPercentage ?? 0;
                                    freightCharge = supplier.ImportChargePerUnit ?? 0;
                                    inlandFreightCharge = supplier.MiscellaneousCharge ?? 0;
                                }

                                // sanity check - you'd be surprised, honestly.
                                // we can live without the amended date and there are numerous instances where this is missing / not in step with the other MVs
                                if (poPriceListRec.Quantities != null &&
                                    poPriceListRec.Prices != null &&
                                    poPriceListRec.Quantities.Count() == poPriceListRec.Prices.Count())
                                {
                                    for (var i = 0; i < poPriceListRec.Quantities.Count(); i++)
                                    {
                                        var quantityBreak = Convert.ToDecimal(poPriceListRec.Quantities.ElementAt(i)) / 1000;

                                        priceCurrency = Convert.ToDecimal(poPriceListRec.Prices.ElementAt(i));

                                        priceSterling = priceCurrency / exchangeRate;

                                        var importCostsTotalSterling = 0.0m;
                                        var importCostsTotalCurrency = 0.0m;

                                        var a = Decimal.Round((((freightCharge + priceSterling) * 10000) * dutyPercentage) / 10000000, 0); // import cost 1 (duty%)
                                        var b = Decimal.Round(((priceSterling * 10000) * inlandChargesPercentage) / 10000000, 0);        // import cost 2 (inland charge%)
                                        var c = Decimal.Round(((priceSterling * 10000) * packagingPercentage) / 10000000, 0);            // import cost 3 (packaging%)
                                        var d = freightCharge;                                                         // import cost 4 (freight)
                                        var e = inlandFreightCharge;                                                   // import cost 5 (quota levy)

                                        // as above
                                        try
                                        {
                                            amended = poPriceListRec.LastAmendedDates?.ElementAt(i);
                                        }
                                        catch (ArgumentOutOfRangeException)
                                        {

                                        }


                                        importCostsSterling = a + b + c + d + e;

                                        importCostsCurrency = importCostsSterling * exchangeRate;

                                        importCostsTotalSterling = Decimal.Round(priceSterling + importCostsSterling, 0);

                                        importCostsTotalCurrency = Decimal.Round(importCostsTotalSterling * exchangeRate, 0);

                                        importCostsTotalSterling = Decimal.Round(importCostsTotalSterling / 10000, 2);

                                        importCostsTotalCurrency = Decimal.Round(importCostsTotalCurrency / 10000, 2);

                                        thisBatchResults.Add(new QuantityBreakCostPriceResult(
                                            productCode: productCode,
                                            brand: brand,
                                            supplierCode: supplierCode,
                                            supplierName: supplierName,
                                            exchangeRate: Math.Round(exchangeRate, 2),
                                            quantityBreak: quantityBreak,
                                            priceCurrency: Math.Round(priceCurrency / 10000, 2),
                                            priceSterling: Math.Round(priceSterling / 10000, 2),
                                            importCostsCurrency: Math.Round(importCostsCurrency / 10000, 2),
                                            importCostsSterling: Math.Round(importCostsSterling / 10000, 2),
                                            amended: amended,
                                            importCostsTotalSterling: importCostsTotalSterling,
                                            importCostsTotalCurrency: importCostsTotalCurrency));
                                    }
                                }
                            }
                        }
                    }

                    lock (Locker)
                    {
                        results.AddRange(thisBatchResults);
#if DEBUG
                        Console.SetCursorPosition(0, Console.CursorTop);
                        Console.Write($"Batch: {++batchCounter} - {sw.Elapsed}");
#endif
                    }
                });
            }

            Console.WriteLine();

            return results;
        }
    }
}
