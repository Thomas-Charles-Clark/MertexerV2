using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Stock
{
    public class B2CEnquiryDataFetch
    {
        public List<string> ProductCodes { get; set; }
        public List<string> StockLocations { get; set; }
        public List<string> SampleProductCodes { get; set; }
        public List<string> SampleStockLocations { get; set; }
        public List<string> PaintProductCodes { get; set; }
        public List<string> PaintStockLocations { get; set; }

        public B2CEnquiryDataFetch()
        {
            ProductCodes = new List<string>();
            StockLocations = new List<string>();
            SampleProductCodes = new List<string>();
            SampleStockLocations = new List<string>();
            PaintProductCodes = new List<string>();
            PaintStockLocations = new List<string>();
        }
    }
}