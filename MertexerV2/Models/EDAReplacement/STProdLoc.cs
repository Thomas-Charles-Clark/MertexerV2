using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.EDAReplacement
{
    public class ProductCode
    {
        public string Code { get; set; }
    }

    public class LocationCode
    {
        public string Code { get; set; }
    }

    public class STProdLoc
    {
        public string ProductCode { get; set; }
        public string LocationCode { get; set; }
        public decimal FreeStock { get; set; }

        public STProdLoc(string productCode,
            string locationCode,
            string freeStock)
        {
            ProductCode = String.IsNullOrEmpty(productCode) ? null : productCode;

            LocationCode = String.IsNullOrEmpty(locationCode) ? null : locationCode;


            if (Decimal.TryParse(freeStock, out decimal fs))
            {
                FreeStock = fs;
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
