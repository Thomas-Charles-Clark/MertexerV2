using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models
{
    public class DeliveryAddress
    {
        public string TradeAccountCode { get; set; }
        public string DeliveryReference { get; set; }
        public string ContactName { get; set; }
        public string CompanyName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string CountryCode { get; set; }
        public string DomesticOrTrade { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string HaulierCode { get; set; }
        public string CarriageCode { get; set; }
        public string USStateCode { get; set; }
        public string USCountyCode { get; set; }
        public string USTaxExemptionCertificateReference { get; set; }

        public DateTime? LastConfirmedDT { get; set; }

        public string NotUsed 
        { 
            get 
            { 
                return null; 
            } 
        }

        public string PipeDelimitedDeliveryString 
        { 
            get 
            {
                return $"{CompanyName}|{AddressLine1}|{AddressLine2}|{Town}|{County}|{Postcode}|{PhoneNumber}|{DomesticOrTrade}|{CountryCode}|Y|{USStateCode}|{USCountyCode}";
            } 
        }

        public DeliveryAddress (
            string tradeAccountCode,
            string addressLine1,
            string addressLine2,
            string town,
            string county,
            string postcode,
            string countryCode,
            string contactName = null,
            string companyName = null,
            string phoneNumber = null,
            string email = null,
            string deliveryReference = null,
            string domesticOrTrade = null,
            string haulierCode = null,
            string carriageCode = null,
            string usStateCode = null,
            string usCountyCode = null,
            string usTaxExemptionCertificateReference = null,
            string lastConfirmedDt = null)
        {
            TradeAccountCode = tradeAccountCode;
            AddressLine1 = addressLine1;
            AddressLine2 = addressLine2;
            Town = town;
            County = county;
            Postcode = postcode;
            CountryCode = countryCode;
            DomesticOrTrade = domesticOrTrade;
            ContactName = contactName;
            CompanyName = companyName;
            PhoneNumber = phoneNumber;
            Email = email;
            DeliveryReference = deliveryReference;
            HaulierCode = haulierCode;
            CarriageCode = carriageCode;
            USStateCode = usStateCode;
            USCountyCode = usCountyCode;
            USTaxExemptionCertificateReference = usTaxExemptionCertificateReference;

            if (!String.IsNullOrEmpty(lastConfirmedDt))
            {
                var ci = System.Threading.Thread.CurrentThread.CurrentCulture;

                if (DateTime.TryParseExact(lastConfirmedDt, "dd/MM/yyyy", ci, System.Globalization.DateTimeStyles.None, out DateTime dt))
                {
                    LastConfirmedDT = dt;
                }
            }
        }

        public string InsertWebData()
        {
            return $@"{TradeAccountCode}
{NotUsed}
{PipeDelimitedDeliveryString}
{Email}
{NotUsed}
{HaulierCode}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{CarriageCode}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{USTaxExemptionCertificateReference}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{ContactName}";
        }

        public string UpdateWebData()
        {
            return $@"{TradeAccountCode}
{DeliveryReference}
{PipeDelimitedDeliveryString}
{Email}
{NotUsed}
{HaulierCode}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{CarriageCode}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{USTaxExemptionCertificateReference}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{NotUsed}
{ContactName}";
        }

        public string DeleteWebData()
        {
            return $@"{TradeAccountCode}
{DeliveryReference}";
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public string MertexAddressLine
        {
            get
            {
                return PipeDelimitedDeliveryString.Replace('|', (char)253);
                //return $"{CompanyName}{(char)253}{AddressLine1}{(char)253}{AddressLine2}{(char)253}{Town}{(char)253}{County}{(char)253}{Postcode}";
            }
        }
    }
}
