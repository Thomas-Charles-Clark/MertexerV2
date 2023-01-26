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
    public class ProductTypeCode
    {
        public string Code { get; set; }
    }

    public static partial class ProductTypeSelect
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "ProductTypeSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string ProductTypeCodes)
        {
            if (String.IsNullOrEmpty(ProductTypeCodes))
            {
                foreach (var q in Selector<ProductType>.SelectAllNonParallel(
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
                var results = Selector<ProductType>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<ProductTypeCode>>(ProductTypeCodes).Select(p => p.Code),
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


        public static void ProductTypeSelect_FillRow(
            object obj,
            out SqlInt16 ProductTypeID,
            out SqlString Name,
            out SqlString MertexDescription,
            out SqlString PriceListDescription,
            out SqlString OnlineProductDescription,
            out SqlString DisplayName,
            out SqlString DisplayNamePriceList,
            out SqlString DisplayNameBasket,
            out SqlString MertexSearchDescription,
            out SqlString DisplayNameSort,
            out SqlString Filename,
            out SqlString PriceListDescription2
            )
        {
            var p = (ProductType)obj;

            ProductTypeID = Convert.ToInt16(p.Key);
            Name = p.Name?.TrimReduceTruncate(50);
            MertexDescription = p.MertexDescription == null ? SqlString.Null : JsonConvert.SerializeObject(p.MertexDescription);
            PriceListDescription = p.PriceListDescription == null ? SqlString.Null : JsonConvert.SerializeObject(p.PriceListDescription);
            OnlineProductDescription = p.OnlineProductDescription == null ? SqlString.Null : JsonConvert.SerializeObject(p.OnlineProductDescription);
            DisplayName = p.DisplayName == null ? SqlString.Null : JsonConvert.SerializeObject(p.DisplayName);
            DisplayNamePriceList = p.DisplayNamePriceList == null ? SqlString.Null : JsonConvert.SerializeObject(p.DisplayNamePriceList);
            DisplayNameBasket = p.DisplayNameBasket == null ? SqlString.Null : JsonConvert.SerializeObject(p.DisplayNameBasket);
            MertexSearchDescription = p.MertexSearchDescription == null ? SqlString.Null : JsonConvert.SerializeObject(p.MertexSearchDescription);
            DisplayNameSort = p.DisplayNameSort == null ? SqlString.Null : JsonConvert.SerializeObject(p.DisplayNameSort);
            Filename = p.Filename?.TrimReduceTruncate(50);
            PriceListDescription2 = p.PriceListDescription2 == null ? SqlString.Null : JsonConvert.SerializeObject(p.PriceListDescription2);
        }
    }
}
