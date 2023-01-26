using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2SQL.Models.Accounts
{
    public class tradeAccountEmailsModel
    {
        public string OrderConfirmations { get; set; }
        public string InvoicesStatements { get; set; }
        public string ProductNewsEvents { get; set; }
        public string Pricelists { get; set; }
    }
}