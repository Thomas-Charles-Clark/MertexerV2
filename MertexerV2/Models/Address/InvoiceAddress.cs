using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Address
{
    public class InvoiceAddress
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string CountryCode { get; set; }

        public InvoiceAddress(string addressLine1,
            string addressLine2,
            string town,
            string county,
            string postcode,
            string countryCode)
        {
            AddressLine1 = addressLine1;
            AddressLine2 = addressLine2;
            Town = town;
            County = county;
            Postcode = postcode;
            CountryCode = countryCode;
        }
    }
}
