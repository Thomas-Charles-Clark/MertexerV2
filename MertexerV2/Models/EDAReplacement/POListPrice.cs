using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.EDAReplacement
{
    public class SupplierProductCode
    {
        public string Key { get; set; }
    }

    public class POListPrice
    {
        public string SupplierReference { get; set; }
        public string ProductCode { get; set; }
        public decimal? PurchasePrice { get; set; }
        public decimal? QuantityBreak { get; set; }
        public string MertexCurrencyCode { get; set; }

        public POListPrice(
            string supplierReference,
            string productCode,
            string purchasePrice,
            string quantityBreak,
            string mertexCurrencyCode)
        {
            SupplierReference = String.IsNullOrEmpty(supplierReference) ? null : supplierReference.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries)[0];

            ProductCode = String.IsNullOrEmpty(productCode) ? null : productCode;

            if (Decimal.TryParse(purchasePrice, out decimal pp))
            {
                PurchasePrice = pp;
            }

            if (Decimal.TryParse(quantityBreak, out decimal qb))
            {
                QuantityBreak = qb;
            }

            MertexCurrencyCode = String.IsNullOrEmpty(mertexCurrencyCode) ? null : mertexCurrencyCode;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
