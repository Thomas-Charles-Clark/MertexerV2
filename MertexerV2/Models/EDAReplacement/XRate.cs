using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.EDAReplacement
{
    public class XRate
    {
        public string MertexCurrencyCode { get; set; }
        public decimal SterlingExchangeRate { get; set; }

        public XRate(
            string mertexCurrencyCode,
            string sterlingExchangeRate)
        {
            MertexCurrencyCode = String.IsNullOrEmpty(mertexCurrencyCode) ? null : mertexCurrencyCode;

            if (Decimal.TryParse(sterlingExchangeRate, out decimal ser))
            {
                SterlingExchangeRate = ser;
            }
            else
            {
                SterlingExchangeRate = 1.0m;
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
