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
    public class ContractDiscountCode
    {
        public string Code { get; set; }
    }

    public static partial class ContractDiscountFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "ContractDiscountSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string TradeAccountCode, string ContractDiscountCodes)
        {
            var preResults = new List<ContractDiscount>();

            if (String.IsNullOrEmpty(ContractDiscountCodes))
            {
                if (String.IsNullOrEmpty(TradeAccountCode))
                {
                    foreach (var pr in Selector<ContractDiscount>.SelectAllNonParallel(
                        true,
                        false,
                        100,
                        UniVerseAccount))
                    {
                        if (pr.ContractDiscounts != null && pr.ContractDiscounts.Any())
                        {
                            var theseResults = new List<ContractDiscountExplosion>();

                            pr.ContractDiscounts.Select((cp, i) => new { contractDiscount = cp, index = i })
                            .ToList()
                            .ForEach((obj) =>
                            {
                                theseResults.Add(new ContractDiscountExplosion(
                                    pr,
                                    pr.StartDTs?.Any() == true && pr.StartDTs.Count() > obj.index ? pr.StartDTs.ElementAt(obj.index) : (DateTime?)null,
                                    pr.ExpiryDTs?.Any() == true && pr.ExpiryDTs.Count() > obj.index ? pr.ExpiryDTs.ElementAt(obj.index) : (DateTime?)null,
                                    obj.contractDiscount,
                                    pr.LastAmendedDTs?.Any() == true && pr.LastAmendedDTs.Count() > obj.index ? pr.LastAmendedDTs.ElementAt(obj.index) : (DateTime?)null,
                                    pr.QuantitiesRequired?.Any() == true && pr.QuantitiesRequired.Count() > obj.index ? pr.QuantitiesRequired.ElementAt(obj.index) : (decimal?)null
                                    ));
                            });

                            foreach (var r in theseResults)
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

                    preResults = Selector<ContractDiscount>.Filter(
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
                preResults = Selector<ContractDiscount>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<ContractDiscountCode>>(ContractDiscountCodes).Select(p => p.Code),
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

            var results = new List<ContractDiscountExplosion>();

            preResults.ForEach(pr =>
            {
                if (pr.ContractDiscounts != null && pr.ContractDiscounts.Any())
                {
                    pr.ContractDiscounts.Select((cp, i) => new { contractDiscount = cp, index = i })
                    .ToList()
                    .ForEach((obj) =>
                    {
                        results.Add(new ContractDiscountExplosion(
                            pr,
                            pr.StartDTs?.Any() == true && pr.StartDTs.Count() > obj.index ? pr.StartDTs.ElementAt(obj.index) : (DateTime?)null,
                            pr.ExpiryDTs?.Any() == true && pr.ExpiryDTs.Count() > obj.index ? pr.ExpiryDTs.ElementAt(obj.index) : (DateTime?)null,
                            obj.contractDiscount,
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

        public static void ContractDiscountSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString TradeAccountCode,
            out SqlString ProductCode,
            out SqlString SortType,
            out SqlString QualityCode,
            out SqlString PromotionCode,
            out SqlString BrandCode,
            out SqlString ProductGroupCode,
            out SqlString ItemTypeCode,
            out SqlDateTime StartDT,
            out SqlDateTime ExpiryDT,
            out SqlDecimal Discount,
            out SqlDateTime LastAmendedDT,
            out SqlDecimal QuantityRequired
            )
        {
            var result = (ContractDiscountExplosion)obj;

            Code = result.Code.TrimReduceTruncate(100);

            TradeAccountCode = result.TradeAccountCode?.TrimReduceTruncate(50) ?? SqlString.Null;

            ProductCode = result.ProductCode?.TrimReduceTruncate(50) ?? SqlString.Null;

            SortType = result.SortType?.TrimReduceTruncate(1) ?? SqlString.Null;

            QualityCode = result.QualityCode?.TrimReduceTruncate(50);

            PromotionCode = result.PromotionCode?.TrimReduceTruncate(50);

            BrandCode = result.BrandCode?.TrimReduceTruncate(5);

            ProductGroupCode = result.ProductGroupCode?.TrimReduceTruncate(10);

            ItemTypeCode = result.ItemTypeCode?.TrimReduceTruncate(10);

            StartDT = result.StartDT ?? SqlDateTime.Null;

            ExpiryDT = result.ExpiryDT ?? SqlDateTime.Null;

            Discount = result.ContractDiscount ?? SqlDecimal.Null;

            LastAmendedDT = result.LastAmendedDT ?? SqlDateTime.Null;

            QuantityRequired = result.QuantityRequired ?? SqlDecimal.Null;
        }
    }
}
