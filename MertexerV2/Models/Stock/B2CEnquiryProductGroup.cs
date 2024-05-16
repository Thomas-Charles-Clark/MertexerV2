using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Stock
{
    internal class B2CEnquiryProductGroup
    {
        public string Name { get; set; }
        public List<ProductCodeStockCheckMinimum> ProductCodes { get; set; }
        public List<B2CEnquiryStockLocation> StockLocations { get; set; }
    }
}
