using MertexerV2.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.EDAReplacement
{
    public class DespatchLine
    {
        public string DespatchLineID { get; set; }
        public string HaulierCode { get; set; }

        public override bool Equals(object obj)
        {
            var dlhc = (DespatchLine)obj;

            return dlhc.DespatchLineID == DespatchLineID &&
                   dlhc.HaulierCode == HaulierCode;
        }

        public override int GetHashCode()
        {
            return $"{DespatchLineID}~#~{HaulierCode}".GetHashCode();
        }
    }

    public class OEDNLine
    {
        public string Source { get; set; }
        public string DespatchHeadID { get; set; }
        public string DespatchLineID { get; set; }
        public string ProductCode { get; set; }
        public List<Parcel> Parcels { get; set; }

        public string ParcelJSON
        {
            get
            {
                return JsonConvert.SerializeObject(Parcels, Formatting.None);
            }
            set
            {

            }
        }

        public OEDNLine(
            string source,
            string despatchLineId,
            string despatchHeadId,
            string productCode,
            List<Parcel> parcels)
        {
            Source = String.IsNullOrEmpty(source) ? null : source;
            
            DespatchHeadID = String.IsNullOrEmpty(despatchHeadId) ? null : despatchHeadId;

            DespatchLineID = String.IsNullOrEmpty(despatchLineId) ? null : despatchLineId;

            ProductCode = String.IsNullOrEmpty(productCode) ? null : productCode;

            Parcels = parcels;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}

/*
 
            int[] fields = new int[] {
                2,  //"OELPHEADP"
                4,  //"PRODCODE",
                34, //"PARCEL"
                97, //"PARCEL.FULL"
                30, //"PIECE"
                31, //"BATCH"
                42, //"DESPQTY.STK"
                55  //"INVQTY.ORD"
            };

 */