using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.EDAReplacement
{
    public class Market
    {
        public string Code { get; set; }
    }

    public class DelAdd
    {
        public string DeliveryReference { get; set; }
        public string TradeAccountCode { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string MertexCountryCode { get; set; }

        public DelAdd(
            string deliveryReference,
            string tradeAccountCode,
            string addressLine1,
            string addressLine2,
            string town,
            string county,
            string postcode,
            string mertexCountryCode)
        {
            DeliveryReference = String.IsNullOrEmpty(deliveryReference) ? null : deliveryReference;
            TradeAccountCode = String.IsNullOrEmpty(tradeAccountCode) ? null : tradeAccountCode;
            AddressLine1 = String.IsNullOrEmpty(addressLine1) ? null : addressLine1;
            AddressLine2 = String.IsNullOrEmpty(addressLine2) ? null : addressLine2;
            Town = String.IsNullOrEmpty(town) ? null : town;
            County = String.IsNullOrEmpty(county) ? null : county;
            Postcode = String.IsNullOrEmpty(postcode) ? null : postcode;
            MertexCountryCode = String.IsNullOrEmpty(mertexCountryCode) ? null : mertexCountryCode;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
