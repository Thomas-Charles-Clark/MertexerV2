using MertexerV2.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.EDAReplacement
{
    public class CLComp
    {
        public string Code { get; set; }
        public string Desc1 { get; set; }
        public string Desc2 { get; set; }
        public string Desc3 { get; set; }
        public string Desc4 { get; set; }
        public string Desc5 { get; set; }
        public string Description { get; set; }

        public CLComp(
            string code,
            string description1,
            string description2,
            string description3,
            string description4,
            string description5)
        {
            Code = String.IsNullOrEmpty(code) ? null : code;
            Desc1 = String.IsNullOrEmpty(description1) ? null : description1;
            Desc2 = String.IsNullOrEmpty(description2) ? null : description2;
            Desc3 = String.IsNullOrEmpty(description3) ? null : description3;
            Desc3 = String.IsNullOrEmpty(description4) ? null : description4;
            Desc3 = String.IsNullOrEmpty(description5) ? null : description5;

            if (!String.IsNullOrEmpty(Desc1) ||
                !String.IsNullOrEmpty(Desc2) ||
                !String.IsNullOrEmpty(Desc3) ||
                !String.IsNullOrEmpty(Desc4) ||
                !String.IsNullOrEmpty(Desc5))
            {
                Description = $"{Desc1} {Desc2} {Desc3} {Desc4} {Desc5}";
                //Description = Description.ConvertWhitespacesToSingleSpaces();
                //Description = Description.Replace("%", "% ");
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}