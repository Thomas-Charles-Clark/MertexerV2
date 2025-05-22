using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.EDAReplacement
{
    public class PODetail
    {
        public string PurchaseOrderLineID { get; set; }
        public string PurchaseOrderHeadID { get; set; }
        public string OrderID { get; set; }
        public string ProductCode { get; set; }
        public decimal? OrderQuantity { get; set; }
        public decimal? ReceivedQuantity { get; set; }
        public DateTime? DateRequired { get; set; }
        public DateTime? DatePromised { get; set; }
        public DateTime? DateExpected { get; set; }
        public bool IsDateConfirmed { get; set; }
        public bool Deleted { get; set; }
        public bool LineComplete { get; set; }
        public short? PurchaseOrderLineNumber { get; set; }
        public decimal? LineUnitPriceSTG { get; set; }
        public decimal? LineUnitPriceCURR { get; set; }
        public string StockUnitCode { get; set; }
        public string SupplierProductCode { get; set; }
        public bool SampleRequired { get; set; }
        public string PurchaseOrderStatusDescription { get; set; }
        public string PurchaseOrderStatusInterpreted { get; set; }

        public PODetail(
            string purchaseOrderLineId,
            string orderId,
            string productCode,
            string orderedQuantity,
            string receivedQuantity,
            string dateRequired,
            string datePromised,
            string dateExpected,
            string isDateConfirmed,
            string deleted,
            string lineComplete,
            string purchaseOrderLineNumber,
            string orderPrice,
            string orderPriceCurr,
            string stockUnitCode,
            string supplierProductCode,
            string sampleRequired,
            string purchaseOrderHead,
            string statusDescription)
        {
            PurchaseOrderLineID = String.IsNullOrEmpty(purchaseOrderLineId) ? null : purchaseOrderLineId;

            OrderID = String.IsNullOrEmpty(orderId) ? null : OrderID;

            ProductCode = String.IsNullOrEmpty(productCode) ? null : productCode;

            if (Decimal.TryParse(orderedQuantity, out decimal oq))
            {
                OrderQuantity = oq;
            }

            if (Decimal.TryParse(receivedQuantity, out decimal rq))
            {
                ReceivedQuantity = rq;
            }

            if (Int32.TryParse(dateRequired, out int dr))
            {
                DateRequired = Utilities.Framework.DateTimeFromUniVerseOffset(dr);
            }

            if (Int32.TryParse(datePromised, out int dp))
            {
                DatePromised = Utilities.Framework.DateTimeFromUniVerseOffset(dp);
            }

            if (Int32.TryParse(dateExpected, out int de))
            {
                DateExpected = Utilities.Framework.DateTimeFromUniVerseOffset(de);
            }

            if (!String.IsNullOrEmpty(isDateConfirmed))
            {
                IsDateConfirmed = isDateConfirmed == "Y" ? true : false;
            }

            if (!String.IsNullOrEmpty(deleted))
            {
                Deleted = deleted == "Y" ? true : false;
            }

            if (!String.IsNullOrEmpty(lineComplete))
            {
                LineComplete = lineComplete == "Y" ? true : false;
            }

            if (Int16.TryParse(purchaseOrderLineNumber, out short ln))
            {
                PurchaseOrderLineNumber = ln;
            }
            
            if (Decimal.TryParse(orderPrice, out decimal op))
            {
                LineUnitPriceSTG = Decimal.Round(Convert.ToDecimal(op), 2);
            }

            if (Decimal.TryParse(orderPriceCurr, out decimal opc))
            {
                LineUnitPriceCURR = Convert.ToDecimal(opc);
            }

            StockUnitCode = String.IsNullOrEmpty(stockUnitCode) ? null : stockUnitCode;

            SupplierProductCode = String.IsNullOrEmpty(supplierProductCode) ? null : supplierProductCode;

            SampleRequired = sampleRequired == "Y" ? true : false;

            PurchaseOrderHeadID = String.IsNullOrEmpty(purchaseOrderHead) ? null : purchaseOrderHead;

            PurchaseOrderStatusDescription = String.IsNullOrEmpty(statusDescription) ? null : statusDescription;

            PurchaseOrderStatusInterpreted = "";

            if (lineComplete != "Y")
            {
                switch (isDateConfirmed)
                {
                    case "Y":
                        PurchaseOrderStatusInterpreted = "PO Conf";
                        break;
                    case "N":
                        PurchaseOrderStatusInterpreted = "PO Unconf";
                        break;
                    case "C":
                        PurchaseOrderStatusInterpreted = "PO Check";
                        break;
                    default: 
                        break;
                }
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}