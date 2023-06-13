using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models
{
    // This doesn't have a concrete basis in Mertex, it's just comprised of MV fields.
    public class TradeAccountContact
    {
        public string TradeAccountCode { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Type { get; set; }
    }
}
