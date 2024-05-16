using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Stock
{
    public class B2CEnquiryDataFetch
    {
        public List<ProductCodeStockCheckMinimum> ProductCodes { get; set; }
        public List<string> StockLocations { get; set; }
        public List<ProductCodeStockCheckMinimum> SampleProductCodes { get; set; }
        public List<string> SampleStockLocations { get; set; }
        public List<ProductCodeStockCheckMinimum> PaintProductCodes { get; set; }
        public List<string> PaintStockLocations { get; set; }

        public B2CEnquiryDataFetch()
        {
            ProductCodes = new List<ProductCodeStockCheckMinimum>();
            StockLocations = new List<string>();
            SampleProductCodes = new List<ProductCodeStockCheckMinimum>();
            SampleStockLocations = new List<string>();
            PaintProductCodes = new List<ProductCodeStockCheckMinimum>();
            PaintStockLocations = new List<string>();
        }
    }
}