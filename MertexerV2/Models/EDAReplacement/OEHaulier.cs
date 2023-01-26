using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.EDAReplacement
{
    public class OEHaulier
    {
        public string HaulierCode { get; set; }
        public string Description { get; set; }
        public TimeSpan? CutOffTime { get; set; }
        public bool ExpectParcelData { get; set; }

        public OEHaulier(
            string haulierCode,
            string description,
            string cutOffTime,
            string parcelSystem)
        {
            HaulierCode = String.IsNullOrEmpty(haulierCode) ? null : haulierCode;

            Description = String.IsNullOrEmpty(description) ? null : description;

            if (!String.IsNullOrEmpty(cutOffTime))
            {
                TimeSpan ts = TimeSpan.Zero;

                if (TimeSpan.TryParse(cutOffTime, out ts))
                {
                    CutOffTime = ts;
                }
            }

            ExpectParcelData = String.IsNullOrEmpty(parcelSystem) ? false : true;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
