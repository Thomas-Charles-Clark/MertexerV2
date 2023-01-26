using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.EDAReplacement
{
    struct InvSund
    {
        public string SundryCode { get; set; }
    }

    public class EDAINVSUND
    {
        public string SundryCode { get; set; }
        public string Description { get; set; }
        public string ProductGroup { get; set; }
        public string ItemType { get; set; }
        public string CollectionCode { get; set; }
        public decimal? Amount { get; set; }
        public decimal? PriceEx { get; set; }
        public string VATCode { get; set; }

        public EDAINVSUND(
            string sundryCode,
            string description,
            string productGroup,
            string itemType,
            string collectionCode,
            string amount,
            string priceEx,
            string vatCode)
        {
            SundryCode = String.IsNullOrEmpty(sundryCode) ? null : sundryCode;

            Description = String.IsNullOrEmpty(description) ? null : description;

            ProductGroup = String.IsNullOrEmpty(productGroup) ? null : productGroup;

            ItemType = String.IsNullOrEmpty(itemType) ? null : itemType;

            CollectionCode = String.IsNullOrEmpty(collectionCode) ? null : collectionCode;

            if (Decimal.TryParse(amount, out decimal a))
            {
                Amount = a;
            }

            if (Decimal.TryParse(priceEx, out decimal p))
            {
                PriceEx = p;
            }

            VATCode = String.IsNullOrEmpty(vatCode) ? null : vatCode;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
