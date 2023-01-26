using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MertexerV2.Models.Status
{
    public class LineTrackingResult
    {
        public string ToelineID { get; set; }
        public int OrderHeadID { get; set; }
        public long OrderLineID { get; set; }
        public short LineNumber { get; set; }
        public DateTime? DespatchedDT { get; set; } // DESP.DATE
        public string ParcelTrackingURL { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public string HaulierCode { get; set; }
        public string PickingListID { get; set; }

        public LineTrackingResult(
            string id,
            string orderHeadId,
            long orderLineId,
            string lineNumber,
            string estimatedDeliveryDate,
            string despatchedDate,
            string parcelTrackingUrl,
            string haulierCode,
            string pickingListId)
        {
            try
            {
                ToelineID = id;

                if (!Int32.TryParse(orderHeadId, out _))
                {
                    orderHeadId = id.Split(new char[] { '\'' })[0];
                }
                else
                {
                    OrderHeadID = Convert.ToInt32(orderHeadId);
                }

                OrderLineID = orderLineId;

                LineNumber = Convert.ToInt16(lineNumber);

                var enGB = new CultureInfo("en-GB");

                if (!String.IsNullOrWhiteSpace(estimatedDeliveryDate))
                {
                    DateTime conv = DateTime.MinValue;
                    if (DateTime.TryParseExact($"{estimatedDeliveryDate}", "dd/MM/yyyy", enGB, DateTimeStyles.None, out conv))
                    {
                        EstimatedDeliveryDate = conv;
                    }
                }

                if (!String.IsNullOrWhiteSpace(despatchedDate))
                {
                    DateTime conv = DateTime.MinValue;
                    if (DateTime.TryParseExact($"{despatchedDate}", "dd/MM/yyyy", enGB, DateTimeStyles.None, out conv))
                    {
                        DespatchedDT = conv;
                    }
                }

                ParcelTrackingURL = String.IsNullOrEmpty(parcelTrackingUrl) ? null : parcelTrackingUrl;

                HaulierCode = haulierCode;

                // 992440901-1*1 => 992440901-1
                PickingListID = Regex.Replace(pickingListId, "\\*.*", "");
                
                if (HaulierCode == "FL")
                {
                    ParcelTrackingURL = $"https://flightlg.couriernavigator-secure.com/online/DES1W?value={PickingListID}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Oh bugger, failed to create LineTrackingResult", ex);
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
