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
            if (String.IsNullOrEmpty(POPriceListCodes))
            {
                foreach (var q in Selector<POPriceList>.SelectAllNonParallel(
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
                var results = Selector<POPriceList>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<POPriceListCode>>(POPriceListCodes).Select(p => p.Code),
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

        public static void POPriceListSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString PurchaseUnit,
            out SqlString Quantities,
            out SqlString Prices,
            out SqlString LastAmendedDates,
            out SqlDecimal OpenOrderPrice,
            out SqlDecimal FutureOrderPrice
            )
        {
            var p = (POPriceList)obj;

            Code = p.Key?.TrimReduceTruncate(10);
            PurchaseUnit = p.PurchaseUnit?.TrimReduceTruncate(10);
            Quantities = p.Quantities == null ? SqlString.Null : JsonConvert.SerializeObject(p.Quantities);
            Prices = p.Prices == null ? SqlString.Null : JsonConvert.SerializeObject(p.Prices);
            LastAmendedDates = p.LastAmendedDates == null ? SqlString.Null : JsonConvert.SerializeObject(p.LastAmendedDates);
            OpenOrderPrice = p.OpenOrderPrice ?? SqlDecimal.Null;
            FutureOrderPrice = p.FutureOrderPrice ?? SqlDecimal.Null;
        }
    }
}
