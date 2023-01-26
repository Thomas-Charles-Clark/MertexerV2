using MertexerV2.GeneralFunctions;
using MertexerV2.GeneralFunctions.Pricing;
using MertexerV2.Models.Company;
using MertexerV2.Models.ExchangeRate;
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

namespace MertexerV2.SQLFunctions
{
    public class QuantityBreakCostPrice
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "CostPrice_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string uniVerseAccount, string productCodesJson, string productGroupFilter = null, string brandFilter = null)
        {
            var productCodes = new List<string>();

            if (!String.IsNullOrEmpty(productCodesJson))
            {
                productCodes = JsonConvert.DeserializeObject<List<Models.EDAReplacement.ProductCode>>(productCodesJson).Select(p => p.Code).ToList();
            }

            var results = GeneralFunctions.Pricing.QuantityBreakCostPrice.Execute(productCodes, productGroupFilter, brandFilter, uniVerseAccount).ToList();

            foreach (var r in results)
            {
                yield return r;
            }

            //            var results = new List<QuantityBreakCostPriceResult>();

            //            using (var ce = new CommandExecutor(uniVerseAccount))
            //            {
            //                var products = new List<ProductStub>();

            //                if (productCodes == null || productCodes.Any() == false)
            //                {
            //                    if (!String.IsNullOrEmpty(productGroupFilter) ||
            //                        !String.IsNullOrEmpty(brandFilter))
            //                    {
            //                        var filters = new List<Tuple<string, string>>();

            //                        if (!String.IsNullOrEmpty(productGroupFilter))
            //                            filters.Add(new Tuple<string, string>("GROUPCODE", productGroupFilter));

            //                        if (!String.IsNullOrEmpty(brandFilter))
            //                            filters.Add(new Tuple<string, string>("SALES.DIV.DES", brandFilter));

            //                        products = Selector<ProductStub>.Filter(filters).ToList();
            //                    }
            //                    else
            //                    {
            //                        products = Selector<ProductStub>.SelectAll().ToList();
            //                    }
            //                }
            //                else
            //                {
            //                    products = Selector<ProductStub>.Select(productCodes).ToList();
            //                }

            //                var company = Selector<Company>.Select("1");

            //                var baseCurrency = company.Currency;

            //                var supplierCodes = products.Select(t => t.SupplierCode).Distinct();

            //                supplierCodes = supplierCodes.Where(s => s != null).ToList();

            //                var suppliers = Selector<Supplier>.Select(supplierCodes, false, false);

            //                var exchangeRates = Selector<ExchangeRate>.SelectAll();

            //                foreach (var s in suppliers)
            //                {
            //                    s.ExchangeRate = exchangeRates.Single(er => er.Key == s.Currency);
            //                }

            //#if DEBUG
            //                var batches = ListExtension.Split(products, 1000);
            //#else
            //                var batches = ListExtension.Split(products, 100);
            //#endif

            //                var poCostRecords = Selector<POCost>.SelectAll();

            //                var selectPriceListRecordsIndividually = false;

            //                var priceListRecords = new List<POPriceList>();

            //                if (productCodes != null && productCodes.Any() || products.Count < 1000)
            //                {
            //                    selectPriceListRecordsIndividually = true;
            //                }
            //                else
            //                {
            //                    priceListRecords = Selector<POPriceList>.SelectAll(false, false).ToList();
            //                }

            //                foreach(var batch in batches)
            //                {
            //                    //var thisBatchResults = new List<QuantityBreakCostPriceResult>();

            //                    foreach (var product in batch)
            //                    {
            //                        var productCode = String.IsNullOrEmpty(product.StockCode) ? product.Key : product.StockCode;
            //                        var brand = product.Brand;
            //                        var supplierCode = product.SupplierCode;
            //                        var qualityCode = product.QualityCode;

            //                        var dutyPercentage = 0.0m;
            //                        var inlandChargesPercentage = 0.0m;
            //                        var packagingPercentage = 0.0m;
            //                        var freightCharge = 0.0m;
            //                        var inlandFreightCharge = 0.0m;

            //                        var priceCurrency = 0.0m;
            //                        var priceSterling = 0.0m;
            //                        var importCostsCurrency = 0.0m;
            //                        var importCostsSterling = 0.0m;
            //                        var amended = "";

            //                        var supplier = suppliers.SingleOrDefault(s => s.Key == supplierCode);

            //                        if (supplier != null) // have found product records where the supplier is NULL...
            //                        {
            //                            // avoiding applying the conversions is necessary in this context because the calculations are done at the highest accuracy possible
            //                            var poCost = poCostRecords.SingleOrDefault(po => po.Key == $"{qualityCode}*\\{supplierCode}");


            //                            var priceKey = $"{supplierCode}*{productCode}";

            //                            var exchangeRate = Decimal.Parse(supplier.ExchangeRate.Rate);

            //                            var poPriceListRec = selectPriceListRecordsIndividually
            //                                ? Selector<POPriceList>.Select(priceKey, false, false)
            //                                : priceListRecords.SingleOrDefault(plr => plr.Key == priceKey);

            //                            if (poPriceListRec == null) // try again but with the quality not the product code
            //                            {
            //                                priceKey = $"{supplierCode}*{qualityCode}";

            //                                poPriceListRec = selectPriceListRecordsIndividually
            //                                    ? Selector<POPriceList>.Select(priceKey, false, false)
            //                                    : priceListRecords.SingleOrDefault(plr => plr.Key == priceKey);
            //                            }

            //                            if (poPriceListRec != null)
            //                            {
            //                                if (poCost != null)
            //                                {
            //                                    if (!String.IsNullOrEmpty(poCost.DutyPercentage))
            //                                    {
            //                                        dutyPercentage = Decimal.Parse(poCost.DutyPercentage) / 100; // What does that | "100" mean?
            //                                    }
            //                                    else
            //                                    {
            //                                        dutyPercentage = Decimal.Parse(supplier.DutyPercentage);
            //                                    }

            //                                    if (!String.IsNullOrEmpty(poCost.InlandCharges))
            //                                    {
            //                                        inlandChargesPercentage = Decimal.Parse(poCost.InlandCharges) / 100;
            //                                    }
            //                                    else
            //                                    {
            //                                        inlandChargesPercentage = Decimal.Parse(supplier.AntiDumpingPercentage);
            //                                    }

            //                                    if (!String.IsNullOrEmpty(poCost.PackagingCharges))
            //                                    {
            //                                        packagingPercentage = Decimal.Parse(poCost.PackagingCharges) / 100;
            //                                    }
            //                                    else
            //                                    {
            //                                        packagingPercentage = Decimal.Parse(supplier.MiscellaneousPercentage);
            //                                    }

            //                                    if (!String.IsNullOrEmpty(poCost.FreightCharges))
            //                                    {
            //                                        freightCharge = Decimal.Parse(poCost.FreightCharges) * 100; // this scaling is necessary, I hate Mertex...
            //                                    }
            //                                    else
            //                                    {
            //                                        freightCharge = Decimal.Parse(supplier.MiscellaneousCharge); // this scaling is necessary, I hate Mertex...
            //                                    }

            //                                    if (!String.IsNullOrEmpty(poCost.QuotaLevy))
            //                                    {
            //                                        inlandFreightCharge = Decimal.Parse(poCost.QuotaLevy) * 100;
            //                                    }
            //                                    else
            //                                    {
            //                                        inlandFreightCharge = Decimal.Parse(supplier.MiscellaneousCharge);
            //                                    }
            //                                }
            //                                else
            //                                {
            //                                    dutyPercentage = Decimal.Parse(supplier.DutyPercentage ?? "0");
            //                                    inlandChargesPercentage = Decimal.Parse(supplier.AntiDumpingPercentage ?? "0");
            //                                    packagingPercentage = Decimal.Parse(supplier.MiscellaneousPercentage ?? "0");
            //                                    freightCharge = Decimal.Parse(supplier.ImportChargePerUnit ?? "0");
            //                                    inlandFreightCharge = Decimal.Parse(supplier.MiscellaneousCharge ?? "0");
            //                                }

            //                                // sanity check - you'd be surprised, honestly.
            //                                // we can live without the amended date and there are numerous instances where this is missing / not in step with the other MVs
            //                                if (poPriceListRec.Quantity != null &&
            //                                    poPriceListRec.Price != null &&
            //                                    poPriceListRec.Quantity.Count() == poPriceListRec.Price.Count())
            //                                {
            //                                    for (var i = 0; i < poPriceListRec.Quantity.Count(); i++)
            //                                    {
            //                                        var quantityBreak = Convert.ToDecimal(poPriceListRec.Quantity.ElementAt(i)) / 1000;

            //                                        priceCurrency = Convert.ToDecimal(poPriceListRec.Price.ElementAt(i));

            //                                        priceSterling = priceCurrency / exchangeRate;

            //                                        var importCostsTotalSterling = 0.0m;
            //                                        var importCostsTotalCurrency = 0.0m;

            //                                        var a = Decimal.Round((((freightCharge + priceSterling) * 10000) * dutyPercentage) / 10000000, 0); // import cost 1 (duty%)
            //                                        var b = Decimal.Round(((priceSterling * 10000) * inlandChargesPercentage) / 10000000, 0);        // import cost 2 (inland charge%)
            //                                        var c = Decimal.Round(((priceSterling * 10000) * packagingPercentage) / 10000000, 0);            // import cost 3 (packaging%)
            //                                        var d = freightCharge;                                                         // import cost 4 (freight)
            //                                        var e = inlandFreightCharge;                                                   // import cost 5 (quota levy)

            //                                        // as above
            //                                        try
            //                                        {
            //                                            amended = poPriceListRec.LastAmended?.ElementAt(i);
            //                                        }
            //                                        catch (ArgumentOutOfRangeException)
            //                                        {

            //                                        }


            //                                        importCostsSterling = a + b + c + d + e;

            //                                        importCostsCurrency = importCostsSterling * exchangeRate;

            //                                        importCostsTotalSterling = Decimal.Round(priceSterling + importCostsSterling, 0);

            //                                        importCostsTotalCurrency = Decimal.Round(importCostsTotalSterling * exchangeRate, 0);

            //                                        importCostsTotalSterling = Decimal.Round(importCostsTotalSterling / 10000, 2);

            //                                        importCostsTotalCurrency = Decimal.Round(importCostsTotalCurrency / 10000, 2);

            //                                        yield return new QuantityBreakCostPriceResult(
            //                                            productCode: productCode,
            //                                            brand: brand,
            //                                            supplierCode: supplierCode,
            //                                            quantityBreak: quantityBreak,
            //                                            priceCurrency: Math.Round(priceCurrency / 10000, 2),
            //                                            priceSterling: Math.Round(priceSterling / 10000, 2),
            //                                            importCostsCurrency: Math.Round(importCostsCurrency / 10000, 2),
            //                                            importCostsSterling: Math.Round(importCostsSterling / 10000, 2),
            //                                            amended: amended,
            //                                            importCostsTotalSterling: importCostsTotalSterling,
            //                                            importCostsTotalCurrency: importCostsTotalCurrency);
            //                                    }
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }
        }

        public static void CostPrice_FillRow(
            object obj,
            out SqlString ProductCode,
            out SqlString Brand,
            out SqlString SupplierCode,
            out SqlString SupplierName,
            out SqlDecimal ExchangeRate,
            out SqlDecimal QuantityBreak,
            out SqlDecimal PriceCurrency,
            out SqlDecimal PriceSterling,
            out SqlDecimal ImportCostsCurrency,
            out SqlDecimal ImportCostsSterling,
            out SqlDecimal CostPriceCurrency,
            out SqlDecimal CostPriceSterling,
            out SqlDateTime LastAmendedDT
            )
        {
            var result = (QuantityBreakCostPriceResult)obj;

            ProductCode = result.ProductCode.TrimReduceTruncate(50);
            Brand = result.Brand.TrimReduceTruncate(5);
            SupplierCode = result.SupplierCode.TrimReduceTruncate(50);
            SupplierName = result.SupplierName.TrimReduceTruncate(200);
            ExchangeRate = result.ExchangeRate;
            QuantityBreak = result.QuantityBreak;
            PriceCurrency = result.PriceCurrency;
            PriceSterling = result.PriceSterling;
            ImportCostsCurrency = result.ImportCostsCurrency;
            ImportCostsSterling = result.ImportCostsSterling;
            CostPriceCurrency = result.CostPriceCurrency;
            CostPriceSterling = result.CostPriceSterling;
            LastAmendedDT = result.LastAmendedDT.HasValue ? result.LastAmendedDT.Value : SqlDateTime.Null;
        }
    }
}
