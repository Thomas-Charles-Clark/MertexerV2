using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockChecker.Models
{
    public class ProductCodeMinimumStockLevel
    {
        public string ProductCode { get; set; }
        public decimal MinimumStockLevel { get; set; }

        public ProductCodeMinimumStockLevel(string productCode, decimal minimumStockLevel)
        {
            ProductCode = productCode;
            MinimumStockLevel = minimumStockLevel;
        }
    }
}
