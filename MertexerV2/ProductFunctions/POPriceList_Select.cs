using MertexerV2.GeneralFunctions;
using MertexerV2.Models.EDAReplacement;
using MertexerV2.Models.Pricing;
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
    public class POPriceListCode
    {
        public string Code { get; set; }
    }

    public static partial class POPriceListFunctions    
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "POPriceListSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string POPriceListCodes)
        {
            IEnumerable<POPriceList> preResults = null;

            if (String.IsNullOrEmpty(POPriceListCodes))
            {
                preResults = Selector<POPriceList>.SelectAllNonParallel(
                   true,
                   false,
                   50,
                   UniVerseAccount);
            }
            else
            {
                preResults = Selector<POPriceList>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<POPriceListCode>>(POPriceListCodes).Select(p => p.Code),
                    true,
                    false,
                    50,
                    UniVerseAccount);
            }

            var results = new List<POPriceListExplosion>();

            preResults
                .ToList()
                .ForEach(pr =>
            {
                pr.Quantities.ToList().Select((q, i) => new { quantity = q, index = i })
                .ToList()
                .ForEach((obj) =>
                {
                    decimal? quantity = null;
                    decimal? price = null;
                    DateTime? lastAmendedDt = null;

                    if (Decimal.TryParse(obj.quantity, out decimal q))
                    {
                        quantity = q;
                    }

                    try
                    {
                        var priceString = pr.Prices.ElementAt(obj.index);

                        if (Decimal.TryParse(priceString, out decimal p))
                        {
                            price = p;
                        }
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        var lastAmendedString = pr.LastAmendedDates.ElementAt(obj.index);

                        if (Int32.TryParse(lastAmendedString, out int dt))
                        {
                            lastAmendedDt = Framework.DateTimeFromUniVerseOffset(dt);
                        }
                    }
                    catch (Exception)
                    {

                    }

                    results.Add(new POPriceListExplosion()
                    {
                        Code = pr.Key,
                        PurchaseUnit = pr.PurchaseUnit,
                        Quantity = quantity,
                        Price = price,
                        LastAmendedDT = lastAmendedDt,
                        OpenOrderPrice = pr.OpenOrderPrice,
                        FutureOrderPrice = pr.FutureOrderPrice,
                    });
                });
            });

            foreach (var r in results)
            {
                yield return r;
            }
        }

        public static void POPriceListSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString PurchaseUnit,
            out SqlDecimal Quantity,
            out SqlDecimal Prices,
            out SqlDateTime LastAmendedDT,
            out SqlDecimal OpenOrderPrice,
            out SqlDecimal FutureOrderPrice
            )
        {
            var p = (POPriceListExplosion)obj;

            Code = p.Code?.TrimReduceTruncate(10);
            PurchaseUnit = p.PurchaseUnit?.TrimReduceTruncate(10);
            Quantity = p.Quantity ?? SqlDecimal.Null;
            Prices = p.Price ?? SqlDecimal.Null;
            LastAmendedDT = p.LastAmendedDT ?? SqlDateTime.Null;
            OpenOrderPrice = p.OpenOrderPrice ?? SqlDecimal.Null;
            FutureOrderPrice = p.FutureOrderPrice ?? SqlDecimal.Null;
        }
    }
}
