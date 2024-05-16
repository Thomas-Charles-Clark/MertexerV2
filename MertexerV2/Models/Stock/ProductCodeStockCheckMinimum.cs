using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Stock
{
    public class ProductCodeStockCheckMinimum
    {
        public string ProductCode { get; set; }
        public decimal StockCheckMinimum { get; set; }

        public ProductCodeStockCheckMinimum(string productCode, decimal stockCheckMinimum)
        {
            ProductCode = productCode;
            StockCheckMinimum = stockCheckMinimum;
        }
    }
}
