using MertexerV2.GeneralFunctions;
using MertexerV2.Models.Pricing;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace MertexerV2.PricingFunctions
{
    public class ContractPriceCode
    {
        public string Code { get; set; }
    }

    public static partial class ContractPriceFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "ContractPriceSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string TradeAccountCode, string ContractPriceCodes)
        {
            var preResults = new List<ContractPrice>();

            if (String.IsNullOrEmpty(ContractPriceCodes))
            {
                if (String.IsNullOrEmpty(TradeAccountCode))
                {
                    foreach(var pr in Selector<ContractPrice>.SelectAllNonParallel(
                        true,
                        false,
                        100,
                        UniVerseAccount))
                    {
                        if (pr.ContractPrices != null && pr.ContractPrices.Any())
                        {
                            var theseResults = new List<ContractPriceExplosion>();

                            pr.ContractPrices.Select((cp, i) => new { contractPrice = cp, index = i })
                            .ToList()
                            .ForEach((obj) =>
                            {
                                theseResults.Add(new ContractPriceExplosion(
                                    pr,
                                    pr.StartDTs?.Any() == true && pr.StartDTs.Count() > obj.index ? pr.StartDTs.ElementAt(obj.index) : (DateTime?)null,
                                    pr.ExpiryDTs?.Any() == true && pr.ExpiryDTs.Count() > obj.index ? pr.ExpiryDTs.ElementAt(obj.index) : (DateTime?)null,
                                    obj.contractPrice,
                                    pr.LastAmendedDTs?.Any() == true && pr.LastAmendedDTs.Count() > obj.index ? pr.LastAmendedDTs.ElementAt(obj.index) : (DateTime?)null,
                                    pr.QuantitiesRequired?.Any() == true && pr.QuantitiesRequired.Count() > obj.index ? pr.QuantitiesRequired.ElementAt(obj.index) : (decimal?)null
                                    ));
                            });

                            foreach(var r in theseResults)
                            {
                                yield return r;
                            }
                        }
                    }
                }
                else
                {
                    var filters = new List<Tuple<string, string, string>>
                        {
                            new Tuple<string, string, string>("CUSNO", "=", TradeAccountCode)
                        };

                    preResults = Selector<ContractPrice>.Filter(
                        filters,
                        true,
                        false,
                        100,
                        UniVerseAccount)
                        .ToList();
                }
            }
            else
            {
                preResults = Selector<ContractPrice>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<ContractPriceCode>>(ContractPriceCodes).Select(p => p.Code),
                    true,
                    false,
                    100,
                    UniVerseAccount)
                    .ToList();

                if (!String.IsNullOrEmpty(TradeAccountCode))
                {
                    preResults = preResults.Where(r => r.TradeAccountCode == TradeAccountCode).ToList();
                }
            }

            var results = new List<ContractPriceExplosion>();

            preResults.ForEach(pr => 
            {
                if (pr.ContractPrices != null && pr.ContractPrices.Any())
                {
                    pr.ContractPrices.Select((cp, i) => new { contractPrice = cp, index = i })
                    .ToList()
                    .ForEach((obj) =>
                    {
                        results.Add(new ContractPriceExplosion(
                            pr,
                            pr.StartDTs?.Any() == true && pr.StartDTs.Count() > obj.index ? pr.StartDTs.ElementAt(obj.index) : (DateTime?)null,
                            pr.ExpiryDTs?.Any() == true && pr.ExpiryDTs.Count() > obj.index ? pr.ExpiryDTs.ElementAt(obj.index) : (DateTime?)null,
                            obj.contractPrice,
                            pr.LastAmendedDTs?.Any() == true && pr.LastAmendedDTs.Count() > obj.index ? pr.LastAmendedDTs.ElementAt(obj.index) : (DateTime?)null,
                            pr.QuantitiesRequired?.Any() == true && pr.QuantitiesRequired.Count() > obj.index ? pr.QuantitiesRequired.ElementAt(obj.index) : (decimal?)null
                            ));
                    });
                }
            });

            foreach (var r in results)
            {
                yield return r;
            }
        }

        public static void ContractPriceSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString TradeAccountCode,
            out SqlString ProductCode,
            out SqlString SortType,
            out SqlString QualityCode,
            out SqlBoolean VATInclusive,
            out SqlString BrandCode,
            out SqlString ProductGroupCode,
            out SqlString ItemTypeCode,
            out SqlDateTime StartDT,
            out SqlDateTime ExpiryDT,
            out SqlDecimal Price,
            out SqlDateTime LastAmendedDT,
            out SqlDecimal QuantityRequired
            )
        {
            var result = (ContractPriceExplosion)obj;

            Code = result.Code.TrimReduceTruncate(100);

            TradeAccountCode = result.TradeAccountCode?.TrimReduceTruncate(50) ?? SqlString.Null;

            ProductCode = result.ProductCode?.TrimReduceTruncate(50) ?? SqlString.Null;

            SortType = result.SortType?.TrimReduceTruncate(1) ?? SqlString.Null;

            QualityCode = result.QualityCode?.TrimReduceTruncate(50);

            VATInclusive = result.VATInclusive;

            BrandCode = result.BrandCode?.TrimReduceTruncate(5);

            ProductGroupCode = result.ProductGroupCode?.TrimReduceTruncate(10);

            ItemTypeCode = result.ItemTypeCode?.TrimReduceTruncate(10);

            StartDT = result.StartDT ?? SqlDateTime.Null;

            ExpiryDT = result.ExpiryDT ?? SqlDateTime.Null;

            Price = result.ContractPrice ?? SqlDecimal.Null;

            LastAmendedDT = result.LastAmendedDT ?? SqlDateTime.Null;

            QuantityRequired = result.QuantityRequired ?? SqlDecimal.Null;
        }
    }
}
