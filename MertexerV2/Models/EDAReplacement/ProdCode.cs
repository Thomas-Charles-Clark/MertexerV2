using MertexerV2.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.EDAReplacement
{
    public class ProdCode
    {
        public string Code { get; set; }

        public ProdCode(
            string productCode
            )
        {
            Code = String.IsNullOrEmpty(productCode) ? null : productCode;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
