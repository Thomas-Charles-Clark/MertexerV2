using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Tracking_Consignments
{
    public class ConsignmentLine
    {
        public string ConsignmentLineID { get; set; }

        public string OrderLineID { get; set; }

        public string OriginalParcelID { get; set; }
    }
}
