using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.EDAReplacement
{
    public class STProdAftCare
    {
        public string ProductCode { get; set; }
        public string AftercareCode { get; set; }
        public byte? SortOrder { get; set; }

        public STProdAftCare(
            string productCode,
            string aftercareCode,
            string sortOrder)
        {
            ProductCode = String.IsNullOrEmpty(productCode) ? null : productCode;
            
            AftercareCode = String.IsNullOrEmpty(aftercareCode) ? null : aftercareCode;
            
            if (Byte.TryParse(sortOrder, out byte so))
            {
                SortOrder = so;
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}