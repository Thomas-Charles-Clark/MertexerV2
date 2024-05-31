using MertexerV2.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.EDAReplacement
{
    public class OrderHead
    { 
        public string OrderID { get; set; }
    }

    public class TOEHead
    {
        public string TradeAccountCode { get; set; }
        public string TradeAccountName { get; set; }
        public string OrderID { get; set; }
        public string CustomerOrderReference { get; set; }
        public int? DateOnFile { get; set; }
        public string TimeOnFile { get; set; }
        public string OrderTaker { get; set; }
        public byte? OrderPriority { get; set; }
        public byte? AllocationPriority { get; set; }
        public bool HoldOrder { get; set; }
        public bool HoldDelivery { get; set; }
        public string DeliveryName { get; set; }
        public string DeliveryAddressLine1 { get; set; }
        public string DeliveryAddressLine2 { get; set; }
        public string DeliveryTown { get; set; }
        public string DeliveryCounty { get; set; }
        public string DeliveryPostcode { get; set; }
        public string DeliveryCountry { get; set; }
        public string HaulierCode { get; set; }
        public string CarriageCode { get; set; }
        public decimal? TotalPaintWeight { get; set; }
        public string PaintMessage { get; set; }


        public string DeliveryAlertLandlinePhoneNumber { get; set; }
        public string DeliveryAlertMobilePhoneNumber { get; set; }
        public string DeliveryAlertEmail { get; set; }

        public string InvoiceAddressName { get; set; }
        public string InvoiceAddressLine1 { get; set; }
        public string InvoiceAddressLine2 { get; set; }
        public string InvoiceAddressTown { get; set; }
        public string InvoiceAddressCounty { get; set; }
        public string InvoiceAddressPostcode { get; set; }

        public DateTime? OrderDT 
        { 
            get 
            {
                if (DateOnFile != null)
                {
                    var date = Framework.DateTimeFromUniVerseOffset(DateOnFile.Value);

                    if (!String.IsNullOrEmpty(TimeOnFile))
                    {
                        date = date.Add(TimeSpan.Parse(TimeOnFile));
                    }

                    return date;
                }

                return null;
            }
            set { }
        }

        public string ChargeOn { get; set; }

        public decimal? DeliveryChargeSterling { get; set; }

        public decimal? DeliveryChargeCurrency { get; set; }

        public string HaulierAccountNumberForShippingCharge { get; set; }

        public string HaulierAccountNumberForDuty { get; set; }

        public string MertexCurrencyCode { get; set; }

        public Decimal SterlingExchangeRate { get; set; }

        public string DeliveryGroup { get; set; }

        public string EORINumber { get; set; }

        public string DeliveryPhone { get; set; }

        public string OrderType { get; set; } 

        public string ContactEmailFax { get; set; }

        public string EmailTill { get; set; }

        public string EcommEmail { get; set; }

        public string MiscEmail { get; set; }

        public string EndCustomer { get; set; }

        public string DeliveryReference { get; set; }

        public string DeliveryType { get; set; }

        public string PaymentTerms { get; set; }

        public string CarriageExempt { get; set; }

        public bool CustomerHeldForFullDespatch { get; set; }

        public TOEHead(
            string orderId,
            string tradeAccountCode,
            string tradeAccountName,
            string customerOrderReference,
            string dateOnFile,
            string timeOnFile,
            string orderTaker,
            string orderPriority,
            string allocationPriority,
            string holdOrder,
            string holdDelivery,
            string deliveryName,
            string deliveryAddressLine1,
            string deliveryAddressLine2,
            string deliveryTown,
            string deliveryCounty,
            string deliveryPostcode,
            string deliveryCountry,
            string haulierCode,
            string carriageCode,
            string totalPaintWeight,
            string paintMessage,
            string deliveryAlertLandlinePhoneNumber,
            string deliveryAlertMobilePhoneNumber,
            string deliveryAlertEmail,
            string chargeOn,
            string deliveryCharge,
            string deliveryChargeCurrency,
            string haulierAccountNumberForShippingCharge,
            string haulierAccountNumberForDuty,
            string mertexCurrencyCode,
            string sterlingExchangeRate,
            string deliveryGroup,
            string eoriNumber,
            string deliveryPhone,
            string invoiceAddressName,
            string invoiceAddressLine1,
            string invoiceAddressLine2,
            string invoiceAddressTown,
            string invoiceAddressCounty,
            string invoiceAddressPostcode,
            string orderType,
            string contactEmailFax,
            string emailTill,
            string ecommEmail,
            string miscEmail,
            string endCustomer,
            string deliveryReference,
            string deliveryType,
            string paymentTerms,
            string carriageExempt,
            string customerHeldForFullDespatch)
        {
            TradeAccountCode = String.IsNullOrEmpty(tradeAccountCode) ? null : tradeAccountCode;

            TradeAccountName = String.IsNullOrEmpty(tradeAccountName) ? null : tradeAccountName;

            OrderID = orderId;

            CustomerOrderReference = String.IsNullOrEmpty(customerOrderReference) ? null : customerOrderReference.Replace(Framework.ValueMark, '|');


            if (Int32.TryParse(dateOnFile, out int dof))
            {
                DateOnFile = dof;
            }

            TimeOnFile = String.IsNullOrEmpty(timeOnFile) ? null : timeOnFile;
            OrderTaker = String.IsNullOrEmpty(orderTaker) ? null : orderTaker;


            if (Byte.TryParse(orderPriority, out byte op))
            {
                OrderPriority = op;
            }


            if (Byte.TryParse(allocationPriority, out byte ap))
            {
                AllocationPriority = ap;
            }

            HoldOrder = holdOrder == "Y" ? true : false;
            HoldDelivery = holdDelivery == "Y" ? true : false;

            DeliveryName = String.IsNullOrEmpty(deliveryName) ? null : deliveryName;
            DeliveryAddressLine1 = String.IsNullOrEmpty(deliveryAddressLine1) ? null : deliveryAddressLine1;
            DeliveryAddressLine2 = String.IsNullOrEmpty(deliveryAddressLine2) ? null : deliveryAddressLine2;
            DeliveryTown = String.IsNullOrEmpty(deliveryTown) ? null : deliveryTown;
            DeliveryCounty = String.IsNullOrEmpty(deliveryCounty) ? null : deliveryCounty;
            DeliveryPostcode = String.IsNullOrEmpty(deliveryPostcode) ? null : deliveryPostcode;
            DeliveryCountry = String.IsNullOrEmpty(deliveryCountry) ? null : deliveryCountry;

            HaulierCode = String.IsNullOrEmpty(haulierCode) ? null : haulierCode;
            CarriageCode = String.IsNullOrEmpty(carriageCode) ? null : carriageCode;


            if (Decimal.TryParse(totalPaintWeight, out decimal tpw))
            {
                TotalPaintWeight = tpw;
            }

            PaintMessage = String.IsNullOrEmpty(paintMessage) ? null : paintMessage;

            if (deliveryAlertLandlinePhoneNumber != null)
            {
                if (deliveryAlertLandlinePhoneNumber.Contains(nameof(DeliveryAlertLandlinePhoneNumber)))
                {
                    DeliveryAlertLandlinePhoneNumber = deliveryAlertLandlinePhoneNumber.Replace($"{nameof(DeliveryAlertLandlinePhoneNumber)}:", "");
                }
            }

            if (deliveryAlertMobilePhoneNumber != null)
            {
                if (deliveryAlertMobilePhoneNumber.Contains(nameof(DeliveryAlertMobilePhoneNumber)))
                {
                    DeliveryAlertMobilePhoneNumber = deliveryAlertMobilePhoneNumber.Replace($"{nameof(DeliveryAlertMobilePhoneNumber)}:", "");
                }
            }

            if (deliveryAlertEmail != null)
            {
                if (deliveryAlertEmail.Contains(nameof(DeliveryAlertEmail)))
                {
                    DeliveryAlertEmail = deliveryAlertEmail.Replace($"{nameof(DeliveryAlertEmail)}:", "");
                }
            }

            ChargeOn = String.IsNullOrEmpty(chargeOn) ? null : chargeOn;

            if (Decimal.TryParse(deliveryCharge, out decimal dcs))
            {
                DeliveryChargeSterling = dcs;
            }

            if (Decimal.TryParse(deliveryChargeCurrency, out decimal dcc))
            {
                DeliveryChargeCurrency = dcc;
            }

            HaulierAccountNumberForShippingCharge = String.IsNullOrEmpty(haulierAccountNumberForShippingCharge) ? null : haulierAccountNumberForShippingCharge;

            HaulierAccountNumberForDuty = String.IsNullOrEmpty(haulierAccountNumberForDuty) ? null : haulierAccountNumberForDuty;

            MertexCurrencyCode = String.IsNullOrEmpty(mertexCurrencyCode) ? null : mertexCurrencyCode;

            if (!String.IsNullOrEmpty(sterlingExchangeRate))
            {
                sterlingExchangeRate = sterlingExchangeRate.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries)[0];
            }
            else
            {
                SterlingExchangeRate = 1.0m;
            }

            if (Decimal.TryParse(sterlingExchangeRate, out decimal ser))
            {
                SterlingExchangeRate = ser;
            }
            else
            {
                SterlingExchangeRate = 1.0m;
            }

            DeliveryGroup = String.IsNullOrEmpty(deliveryGroup) ? null : deliveryGroup;

            if (eoriNumber != null)
                eoriNumber = eoriNumber.Replace("EORI:", "");

            EORINumber = String.IsNullOrEmpty(eoriNumber) ? null : eoriNumber;

            DeliveryPhone = String.IsNullOrEmpty(deliveryPhone) ? null : deliveryPhone;

            InvoiceAddressName = invoiceAddressName;

            InvoiceAddressLine1 = invoiceAddressLine1;

            InvoiceAddressLine2 = invoiceAddressLine2;

            InvoiceAddressTown = invoiceAddressTown;

            InvoiceAddressCounty = invoiceAddressCounty;

            InvoiceAddressPostcode = invoiceAddressPostcode;

            OrderType = orderType;

            ContactEmailFax = contactEmailFax;

            EmailTill = emailTill;

            EcommEmail = ecommEmail;

            MiscEmail = miscEmail;

            EndCustomer = endCustomer;

            DeliveryReference = String.IsNullOrEmpty(deliveryReference) ? null : deliveryReference;

            DeliveryType = String.IsNullOrEmpty(deliveryType) ? null : deliveryType;

            PaymentTerms = String.IsNullOrEmpty(paymentTerms) ? null : paymentTerms;

            CarriageExempt = String.IsNullOrEmpty(carriageExempt) ? null : carriageExempt;

            CustomerHeldForFullDespatch = customerHeldForFullDespatch == "Y" ? true : false;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}