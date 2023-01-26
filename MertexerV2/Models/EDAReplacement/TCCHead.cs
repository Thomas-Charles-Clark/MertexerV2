using MertexerV2.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.EDAReplacement
{
    struct CCRHead
    {
        public string ClaimID { get; set; }
    }

    struct MarketCode
    {
        public string SalesMarketCode { get; set; }
    }

    public struct objCarrierReference
    {
        public string CarrierReference { get; set; }
    }

    public class TCCHead
    {
        public string ClaimID { get; set; }
        public string OrderID { get; set; }
        public string DocumentNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string TradeAccountCode { get; set; }
        public string SalesMarketCode { get; set; }
        public DateTime? DateOnFile { get; set; }
        public string ContactName { get; set; }
        public string CollectionAddress { get; set; }
        public string CollectionName { get; set; }
        public string CollectionAddressLine1 { get; set; }
        public string CollectionAddressLine2 { get; set; }
        public string CollectionTown { get; set; }
        public string CollectionCounty { get; set; }
        public string CollectionPostcode { get; set; }
        public string CollectionCountryCode { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryName { get; set; }
        public string DeliveryAddressLine1 { get; set; }
        public string DeliveryAddressLine2 { get; set; }
        public string DeliveryTown { get; set; }
        public string DeliveryCounty { get; set; }
        public string DeliveryPostcode { get; set; }
        public string DeliveryCountry { get; set; }
        public string ClaimTaker { get; set; }
        public string CurrentPerson { get; set; }
        public string CurrentDepartmentCode { get; set; }
        public List<objCarrierReference> CarrierReferences { get; set; }
        public string CarrierReferencesJSON
        {
            get
            {
                return JsonConvert.SerializeObject(CarrierReferences, Formatting.None);
            }
            set
            {

            }
        }
        public bool Completed { get; set; }
        public string Result { get; set; }

        public TCCHead(
            string claimId,
            string orderId,
            string documentNumber,
            string invoiceNumber,
            string dateOnFile,
            string timeOnFile,
            string tradeAccountCode,
            string salesMarketCode,
            string contactName,
            string collectionAddress,
            string collectionCountryCode,
            string deliveryAddress,
            string claimTaker,
            string currentPerson,
            string currentDepartment,
            string completed,
            string result,
            List<string> carrierReferences)
        {
            ClaimID = String.IsNullOrEmpty(claimId) ? null : claimId;

            OrderID = String.IsNullOrEmpty(orderId) ? null : orderId;

            DocumentNumber = String.IsNullOrEmpty(documentNumber) ? null : documentNumber;

            InvoiceNumber = String.IsNullOrEmpty(invoiceNumber) ? null : invoiceNumber;

            if (Int32.TryParse(dateOnFile, out int dof))
            {
                DateOnFile = Framework.DateTimeFromUniVerseOffset(dof);

                if (TimeSpan.TryParse(timeOnFile, out TimeSpan ts))
                {
                    DateOnFile += ts;
                }
            }

            TradeAccountCode = String.IsNullOrEmpty(tradeAccountCode) ? null : tradeAccountCode;

            SalesMarketCode = String.IsNullOrEmpty(salesMarketCode) ? null : salesMarketCode;

            ContactName = String.IsNullOrEmpty(contactName) ? null : contactName;

            CollectionAddress = String.IsNullOrEmpty(collectionAddress) ? null : collectionAddress;

            if (CollectionAddress != null)
            {
                var address = CollectionAddress.Split(Framework.ValueMarkDelimiter);

                if (address.Length < 6)
                {
                    var newAddress = address.ToList();

                    for (var i = 0; i < (6 - address.Length); i++)
                    {
                        newAddress.Add("");
                    }

                    address = newAddress.ToArray();
                }

                CollectionName = String.IsNullOrEmpty(address[0]) ? null : address[0];

                CollectionAddressLine1 = String.IsNullOrEmpty(address[1]) ? null : address[1];

                CollectionAddressLine2 = String.IsNullOrEmpty(address[2]) ? null : address[2];

                CollectionTown = String.IsNullOrEmpty(address[3]) ? null : address[3];

                CollectionCounty = String.IsNullOrEmpty(address[4]) ? null : address[4];

                CollectionPostcode = String.IsNullOrEmpty(address[5]) ? null : address[5];
            }

            CollectionCountryCode = String.IsNullOrEmpty(collectionCountryCode) ? null : collectionCountryCode;

            DeliveryAddress = String.IsNullOrEmpty(deliveryAddress) ? null : deliveryAddress;

            if (DeliveryAddress != null)
            {
                var address = DeliveryAddress.Split(Framework.ValueMarkDelimiter);

                if (address.Length < 6)
                {
                    var newAddress = address.ToList();

                    for (var i = 0; i < (6 - address.Length); i++)
                    {
                        newAddress.Add("");
                    }

                    address = newAddress.ToArray();
                }

                DeliveryName = String.IsNullOrEmpty(address[0]) ? null : address[0];

                DeliveryAddressLine1 = String.IsNullOrEmpty(address[1]) ? null : address[1];

                DeliveryAddressLine2 = String.IsNullOrEmpty(address[2]) ? null : address[2];

                DeliveryTown = String.IsNullOrEmpty(address[3]) ? null : address[3];

                DeliveryCounty = String.IsNullOrEmpty(address[4]) ? null : address[4];

                DeliveryPostcode = String.IsNullOrEmpty(address[5]) ? null : address[5];
            }

            ClaimTaker = String.IsNullOrEmpty(claimTaker) ? null : claimTaker;

            CurrentPerson = String.IsNullOrEmpty(currentPerson) ? null : currentPerson;

            CurrentDepartmentCode = String.IsNullOrEmpty(currentDepartment) ? null : currentDepartment;

            CarrierReferences = new List<objCarrierReference>();

            carrierReferences.ForEach(cr => 
            {
                CarrierReferences.Add(new objCarrierReference() { CarrierReference = cr });
            });

            Completed = String.IsNullOrEmpty(completed) ? false : completed == "Y" ? true : false;

            Result = String.IsNullOrEmpty(result) ? null : result;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}