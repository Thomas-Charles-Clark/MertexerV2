using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.EDAReplacement
{
    public class POHead
    {
        public string PurchaseOrderHeadID { get; set; }
        public string OrderID { get; set; }
        public byte? PurchaseOrderStatusID { get; set; }
        public string StockLocationCode { get; set; }
        public DateTime? DateOnFile { get; set; }
        public string SupplierCode { get; set; }
        public string ProcessCode { get; set; }
        public string AuthoriserCode { get; set; }
        public bool CallOffIndicator { get; set; }
        public string DeliveryName { get; set; }
        public string DeliveryAddressLine1 { get; set; }
        public string DeliveryAddressLine2 { get; set; }
        public string DeliveryTown { get; set; }
        public string DeliveryCounty { get; set; }
        public string DeliveryPostcode { get; set; }
        public string DeliveryPhone { get; set; }

        public POHead(string poNumber,
            string orderId,
            string orderStatus,
            string stockLocation,
            string dateOnFile,
            string supplier,
            string processCode,
            string authoriserCode,
            string callOffIndicator,
            string deliveryName,
            string deliveryAddressLine1,
            string deliveryAddressLine2,
            string deliveryTown,
            string deliveryCounty,
            string deliveryPostcode,
            string deliveryPhone)
        {
            PurchaseOrderHeadID = String.IsNullOrEmpty(poNumber) ? null : poNumber;

            OrderID = String.IsNullOrEmpty(orderId) ? null : orderId;

            if (Byte.TryParse(orderStatus, out byte os))
            {
                PurchaseOrderStatusID = os;
            }

            StockLocationCode = String.IsNullOrEmpty(stockLocation) ? null : stockLocation;

            if (Int32.TryParse(dateOnFile, out int dof))
            {
                DateOnFile = Utilities.Framework.DateTimeFromUniVerseOffset(dof);
            }

            SupplierCode = String.IsNullOrEmpty(supplier) ? null : supplier;

            ProcessCode = String.IsNullOrEmpty(processCode) ? null : processCode;

            AuthoriserCode = String.IsNullOrEmpty(authoriserCode) ? null : authoriserCode;

            CallOffIndicator = callOffIndicator == "Y" ? true : false;

            DeliveryName = String.IsNullOrEmpty(deliveryName) ? null : deliveryName;

            DeliveryAddressLine1 = String.IsNullOrEmpty(deliveryAddressLine1) ? null : deliveryAddressLine1;

            DeliveryAddressLine2 = String.IsNullOrEmpty(deliveryAddressLine2) ? null : deliveryAddressLine2;

            DeliveryTown = String.IsNullOrEmpty(deliveryTown) ? null : deliveryTown;

            DeliveryCounty = String.IsNullOrEmpty(deliveryCounty) ? null : deliveryCounty;

            DeliveryPostcode = String.IsNullOrEmpty(deliveryPostcode) ? null : deliveryPostcode;

            DeliveryPhone = String.IsNullOrEmpty(deliveryPhone) ? null : deliveryPhone;
        }
    }
}
