using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockChecker.Models
{
    public class B2CEnquiryDataFetch
    {
        public List<ProductCodeMinimumStockLevel> ProductCodes { get; set; }
        public List<string> StockLocations { get; set; }
        public List<ProductCodeMinimumStockLevel> SampleProductCodes { get; set; }
        public List<string> SampleStockLocations { get; set; }
        public List<ProductCodeMinimumStockLevel> PaintProductCodes { get; set; }
        public List<string> PaintStockLocations { get; set; }

        public B2CEnquiryDataFetch()
        {
            ProductCodes = new List<ProductCodeMinimumStockLevel>();
            StockLocations = new List<string>();
            SampleProductCodes = new List<ProductCodeMinimumStockLevel>();
            SampleStockLocations = new List<string>();
            PaintProductCodes = new List<ProductCodeMinimumStockLevel>();
            PaintStockLocations = new List<string>();
        }
    }
}
