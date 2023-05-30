using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.EDAReplacement
{
    public class PRICECROSS
    {
        public string TradePriceListCode { get; set; }

        public string RRPPriceListCode { get; set; }

        public PRICECROSS(
            string tradePriceListCode,
            string rrpPriceListCode)
        {
            if (!String.IsNullOrEmpty(tradePriceListCode))
            {
                TradePriceListCode = tradePriceListCode;
            }

            if (!String.IsNullOrEmpty(rrpPriceListCode))
            {
                RRPPriceListCode = rrpPriceListCode;
            }
        }
    }
}
