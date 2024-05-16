using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Stock
{
    internal class B2CEnquiryDataFetch2
    {
        public IEnumerable<B2CEnquiryProductGroup> ProductGroups { get; set; }

        public B2CEnquiryDataFetch2()
        {
            ProductGroups = new List<B2CEnquiryProductGroup>();
        }
    }
}
