using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Stock
{
    public class B2CEnquiryResult
    {
        public string StockLocationCode { get; set; }
        public string ProductCode { get; set; }
        public DateTime? PurchaseOrderDate { get; set; }
        public string PurchaseOrderMessage { get; set; }
        public decimal TotalFreeStock { get; set; }
        public decimal NextPOQuantity { get; set; }
        public decimal TotalPOQuantity { get; set; }
        public bool? PurchaseOrderStatus { get; set; }

        public B2CEnquiryResult(
            string stockLocationCode,
            string productCode,
            decimal totalFreeStock,
            string nextPODueDate,
            decimal nextPOQuantity,
            decimal totalPOQuantity,
            bool? poStatus)
        {
            StockLocationCode = stockLocationCode;
            ProductCode = productCode;

            if (DateTime.TryParse(nextPODueDate, out DateTime poRealDate))
            {
                PurchaseOrderDate = poRealDate;
            }
            else
            {
                PurchaseOrderMessage = nextPODueDate;
            }

            TotalFreeStock = totalFreeStock;
            NextPOQuantity = nextPOQuantity;
            TotalPOQuantity = totalPOQuantity;
            PurchaseOrderStatus = poStatus;
        }

        public override string ToString()
        {
            return $@"StockLocation: {StockLocationCode}
ProductCode: {ProductCode}
TotalFreeStock: {TotalFreeStock}
NextPODueDate: {PurchaseOrderDate?.ToString() ?? PurchaseOrderMessage}
NextPOQuantity: {NextPOQuantity}
TotalPOQuantity: {TotalPOQuantity}
POStatus: {PurchaseOrderStatus}";
        }
    }
}
