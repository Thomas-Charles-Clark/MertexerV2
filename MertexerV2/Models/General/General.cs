using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.General
{
    public class CodeDescription
    {
        public string Code { get; set; }
        public string Description { get; set; }
        
        public Dictionary<string, string> Errata { get; set; }

        public CodeDescription(string c, string d)
        {
            Code = c;
            Description = d;
            Errata = null;
        }

        public CodeDescription(string c, string d, Dictionary<string, string> e = null)
        {
            Code = c;
            Description = d;
            Errata = e;
        }
    }
}
