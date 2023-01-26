using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.EDAReplacement
{
    public class EDAADDITIONAL
    {
        public string Code { get; set; }
        public string Value { get; set; }

        public EDAADDITIONAL(string code,
            string value)
        {
            Code = String.IsNullOrEmpty(code) ? null : code;

            Value = String.IsNullOrEmpty(value) ? null : value;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}