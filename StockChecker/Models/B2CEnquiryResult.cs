using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace StockChecker.Models
{
    internal class B2CEnquiryResult
    {
        public string StockLocationCode { get; set; }
        public string ProductCode { get; set; }
        public Decimal StockCheckMinimum { get; set; }
        public DateTime? PurchaseOrderDate { get; set; }
        public string PurchaseOrderMessage { get; set; }
        public decimal TotalFreeStock { get; set; }
        public decimal NextPOQuantity { get; set; }
        public decimal TotalPOQuantity { get; set; }
        public bool? PurchaseOrderStatus { get; set; }

        public B2CEnquiryResult(
            string stockLocationCode,
            string productCode,
            decimal stockCheckMinimum,
            decimal totalFreeStock,
            string nextPODueDate,
            decimal nextPOQuantity,
            decimal totalPOQuantity,
            bool? poStatus,
            string poMessage)
        {
            StockLocationCode = stockLocationCode;
            ProductCode = productCode;
            StockCheckMinimum = stockCheckMinimum;
            PurchaseOrderMessage = poMessage;

            if (DateTime.TryParse(nextPODueDate, out DateTime poRealDate))
            {
                PurchaseOrderDate = poRealDate;
            }

            TotalFreeStock = totalFreeStock;
            NextPOQuantity = nextPOQuantity;
            TotalPOQuantity = totalPOQuantity;
            PurchaseOrderStatus = poStatus;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
        }
    }
}
