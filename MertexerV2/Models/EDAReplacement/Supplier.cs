using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.EDAReplacement
{
    public class Supplier
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string CountryCode { get; set; }
        public string MertexCurrencyCode { get; set; }


        public Supplier(
            string code,
            string name,
            string addressLine1,
            string addressLine2,
            string town,
            string county,
            string postcode,
            string countryCode,
            string mertexCurrencyCode)
        {
            Code = String.IsNullOrEmpty(code) ? null : code;

            Name = String.IsNullOrEmpty(name) ? null : name;

            AddressLine1 = String.IsNullOrEmpty(addressLine1) ? null : addressLine1;

            AddressLine2 = String.IsNullOrEmpty(addressLine2) ? null : addressLine2;

            Town = String.IsNullOrEmpty(town) ? null : town;

            County = String.IsNullOrEmpty(county) ? null : county;

            Postcode = String.IsNullOrEmpty(postcode) ? null : postcode;

            CountryCode = String.IsNullOrEmpty(countryCode) ? null : countryCode;

            MertexCurrencyCode = String.IsNullOrEmpty(mertexCurrencyCode) ? null : mertexCurrencyCode;
        }
    }
}
