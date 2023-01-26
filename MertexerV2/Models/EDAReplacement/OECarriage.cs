using MertexerV2.Models.General;
using MertexerV2.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.EDAReplacement
{
    public class OECarriage : CodeDescription
    {
        public string CarriageCode
        {
            get
            {
                return Code.Split(Framework.AsteriskDelimiter, StringSplitOptions.RemoveEmptyEntries)[0];
            }
        }

        public string HaulierCode
        {
            get
            {
                return Code.Split(Framework.AsteriskDelimiter, StringSplitOptions.RemoveEmptyEntries)[1];
            }
        }

        public bool PriorityOrder { get; set; }

        public OECarriage(string code, string description, string priorityOrder) : base(code, description) 
        {
            if (priorityOrder == "Y")
            {
                PriorityOrder = true;
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
