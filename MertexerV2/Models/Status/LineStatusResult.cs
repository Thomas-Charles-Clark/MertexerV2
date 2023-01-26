using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Status
{
    public class LineStatusResult
    {
        public string ToelineID { get; set; }
        public int OrderHeadID { get; set; }
        public long OrderLineID { get; set; }
        public short LineNumber { get; set; }
        public string ProductCode { get; set; }
        public byte OrderStatusID { get; set; }
        public string StatusDescription { get; set; }
        public decimal? Quantity { get; set; } // QTYORD.ORD
        public decimal? CancelledQuantity { get; set; } // QTYCAN.ORD
        public DateTime? StatusChangedDT { get; set; }

        public LineStatusResult(
            string id,
            string orderHeadId,
            long orderLineId,
            string lineNumber,
            string prodCode,
            string status,
            string detailedStatusDescription,
            string quantity,
            string cancelledQuantity,
            string date,
            string time)
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

                if (Byte.TryParse(status, out _))
                {
                    OrderStatusID = Byte.Parse(status);
                }

                StatusDescription = detailedStatusDescription;

                ProductCode = prodCode;

                var enGB = new CultureInfo("en-GB");

                if (!String.IsNullOrWhiteSpace(date) &&
                    !String.IsNullOrWhiteSpace(time))
                {
                    DateTime conv = DateTime.MinValue;
                    if (DateTime.TryParseExact($"{date} {time}", "dd/MM/yyyy HH:mm:ss", enGB, DateTimeStyles.None, out conv))
                    {
                        StatusChangedDT = conv;
                    }
                }

                if (!String.IsNullOrWhiteSpace(quantity))
                {
                    Decimal conv = -1;
                    if (Decimal.TryParse(quantity, out conv))
                    {
                        Quantity = conv;
                    }
                }

                if (!String.IsNullOrWhiteSpace(cancelledQuantity))
                {
                    Decimal conv = -1;
                    if (Decimal.TryParse(cancelledQuantity, out conv))
                    {
                        CancelledQuantity = conv;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Oh bugger, failed to create LineResult", ex);
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
