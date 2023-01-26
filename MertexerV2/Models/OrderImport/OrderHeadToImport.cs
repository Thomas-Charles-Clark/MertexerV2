using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.OrderImport
{
    public class OrderHeadToImport
    {
        public List<OrderLineToImport> OrderLines { get; set; }

        public string OrderType { get; set; }

        public long TradeOrderImportFileID { get; set; }
        public string TradeAccountNumber { get; set; }
        public string OrderReference { get; set; }
        public string DeliveryName { get; set; }
        public string DeliveryAddressLine1 { get; set; }
        public string DeliveryAddressLine2 { get; set; }
        public string DeliveryTown { get; set; }
        public string DeliveryCounty { get; set; }
        public string DeliveryPostcode { get; set; }
        public string DeliveryCountryCode { get; set; }
        public string DeliveryPhoneNumber { get; set; }
        public string Email { get; set; }
        public string ShippingAlertEmailAddress { get; set; }
        public string ContactPhone { get; set; }
        public string HaulierCode { get; set; }
        public string CarriageCode { get; set; }
        public string HaulierAccountNumber { get; set; }
        public string TaxesDutiesToInvoiceAccount { get; set; }
        public string DeliveryTerms { get; set; }
        public string DeliveryType { get; set; }
        public string EndCustomer { get; set; }
        public bool HeldForSales { get; set; }

        public string DutchOrderGuid { get; set; }

        private string deliveryReference { get; set; }

        public string DeliveryReference
        {
            get
            {
                switch (OrderType)
                {
                    case "Standard":
                        return null;
                    case "Distributor":
                        return "E";
                    default:
                        return deliveryReference;
                }
            }
            set
            {
                deliveryReference = value;
            }
        }

        public OrderHeadToImport(
            long tradeOrderImportFileId,
            string orderType,
            string tradeAccountNumber,
            string orderReference,
            string deliveryName,
            string deliveryAddressLine1,
            string deliveryAddressLine2,
            string deliveryTown,
            string deliveryCounty,
            string deliveryPostcode,
            string deliveryCountryCode,
            string deliveryPhoneNumber,
            string email,
            string shippingAlertEmailAddress,
            string contactPhone,
            bool heldForSales
            )
        {

            OrderType = orderType;

            TradeOrderImportFileID = tradeOrderImportFileId;
            TradeAccountNumber = tradeAccountNumber;
            OrderReference = orderReference;
            DeliveryName = deliveryName;
            DeliveryAddressLine1 = deliveryAddressLine1;
            DeliveryAddressLine2 = deliveryAddressLine2;
            DeliveryTown = deliveryTown;
            DeliveryCounty = deliveryCounty;
            DeliveryPostcode = deliveryPostcode;
            DeliveryCountryCode = deliveryCountryCode;
            DeliveryPhoneNumber = deliveryPhoneNumber;
            Email = email;
            ShippingAlertEmailAddress = shippingAlertEmailAddress;
            ContactPhone = contactPhone;
            HaulierCode = null;
            CarriageCode = null;
            HaulierAccountNumber = null;
            TaxesDutiesToInvoiceAccount = null;
            DeliveryTerms = null;
            DeliveryType = null;
            EndCustomer = null;
            HeldForSales = heldForSales;

            OrderLines = new List<OrderLineToImport>();
        }

        public OrderHeadToImport(
            long tradeOrderImportFileId,
            string orderType,
            string tradeAccountNumber,
            string orderReference,
            string deliveryName,
            string deliveryAddressLine1,
            string deliveryAddressLine2,
            string deliveryTown,
            string deliveryCounty,
            string deliveryPostcode,
            string deliveryCountryCode,
            string deliveryPhoneNumber,
            string email,
            string shippingAlertEmailAddress,
            string contactPhone,
            string haulierCode,
            string carriageCode,
            string haulierAccountNumber,
            string taxesDutiesToInvoiceAccount,
            string deliveryTerms,
            string deliveryType,
            string endCustomer,
            bool heldForSales
            )
        {

            OrderType = orderType;

            TradeOrderImportFileID = tradeOrderImportFileId;
            TradeAccountNumber = tradeAccountNumber;
            OrderReference = orderReference;
            DeliveryName = deliveryName;
            DeliveryAddressLine1 = deliveryAddressLine1;
            DeliveryAddressLine2 = deliveryAddressLine2;
            DeliveryTown = deliveryTown;
            DeliveryCounty = deliveryCounty;
            DeliveryPostcode = deliveryPostcode;
            DeliveryCountryCode = deliveryCountryCode;
            DeliveryPhoneNumber = deliveryPhoneNumber;
            Email = email;
            ShippingAlertEmailAddress = shippingAlertEmailAddress;
            ContactPhone = contactPhone;
            HaulierCode = haulierCode;
            CarriageCode = carriageCode;
            HaulierAccountNumber = haulierAccountNumber;
            TaxesDutiesToInvoiceAccount = taxesDutiesToInvoiceAccount;
            DeliveryTerms = deliveryTerms;
            DeliveryType = deliveryType;
            EndCustomer = endCustomer;
            HeldForSales = heldForSales;

            OrderLines = new List<OrderLineToImport>();
        }


        public OrderHeadToImport(
            long tradeOrderImportFileId,
            string orderType,
            string tradeAccountNumber,
            string orderReference,
            string delRef,
            string deliveryName,
            string deliveryAddressLine1,
            string deliveryAddressLine2,
            string deliveryTown,
            string deliveryCounty,
            string deliveryPostcode,
            string deliveryCountryCode,
            string deliveryPhoneNumber,
            string email,
            string dutchOrderGuid,
            bool heldForSales,
            bool dutch
            )
        {

            OrderType = orderType;

            TradeOrderImportFileID = tradeOrderImportFileId;
            TradeAccountNumber = tradeAccountNumber;
            OrderReference = orderReference;
            DeliveryReference = delRef;
            DeliveryName = deliveryName;
            DeliveryAddressLine1 = deliveryAddressLine1;
            DeliveryAddressLine2 = deliveryAddressLine2;
            DeliveryTown = deliveryTown;
            DeliveryCounty = deliveryCounty;
            DeliveryPostcode = deliveryPostcode;
            DeliveryCountryCode = deliveryCountryCode;
            DeliveryPhoneNumber = deliveryPhoneNumber;
            Email = email;
            ShippingAlertEmailAddress = null;
            ContactPhone = null;
            HaulierCode = null;
            CarriageCode = null;
            HaulierAccountNumber = null;
            TaxesDutiesToInvoiceAccount = null;
            DeliveryTerms = null;
            DeliveryType = null;
            EndCustomer = null;
            HeldForSales = heldForSales;

            OrderLines = new List<OrderLineToImport>();

            DutchOrderGuid = dutchOrderGuid;
        }
    }
}