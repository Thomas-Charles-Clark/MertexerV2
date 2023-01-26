using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.EDAReplacement
{
    public class CLQualDes
    {
        public string QualityCode { get; set; }
        public decimal? Width { get; set; }
        public string CompositionCode { get; set; }
        public string Name { get; set; }
        public decimal? GSM { get; set; }
        public decimal? GLM { get; set; }

        public CLQualDes(
            string qualityCode,
            string width,
            string compositionCode,
            string name,
            string gsm,
            string glm)
        {
            QualityCode = String.IsNullOrEmpty(qualityCode) ? null : qualityCode;


            if (Decimal.TryParse(width, out decimal w))
            {
                Width = w;
            }

            CompositionCode = String.IsNullOrEmpty(compositionCode) ? null : compositionCode;

            Name = String.IsNullOrEmpty(name) ? null : name;

            if (Decimal.TryParse(gsm, out decimal g))
            {
                GSM = g;
            }

            if (Decimal.TryParse(glm, out decimal gl))
            {
                GLM = gl;
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
