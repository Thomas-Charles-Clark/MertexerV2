using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.EDAReplacement
{
    public class SLCustStub
    {
        public string TradeAccountCode { get; set; }
        public bool AccountOpen { get; set; }

        public SLCustStub(
            string tradeAccountCode,
            string accountOpenStatus
        )
        {
            TradeAccountCode = tradeAccountCode;
            AccountOpen = accountOpenStatus == "O";
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
