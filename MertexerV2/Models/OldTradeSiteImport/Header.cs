using MertexerV2.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MertexerV2.Models.OldTradeSiteImport
{
    public class Header
    {
        public string BasketID { get; set; }
        public string Key { get; set; }
        public string TradeAccountCode { get; set; }
        public string DeliveryReference { get; set; }
        public string DeliveryName { get; set; }
        public string DeliveryAddressLine1 { get; set; }
        public string DeliveryAddressLine2 { get; set; }
        public string DeliveryAddressLine3 { get; set; }
        public string DeliveryAddressLine4 { get; set; }
        public string DeliveryPostcode { get; set; }
        public string DeliveryCountryCode { get; set; }
        public string DeliveryPhone { get; set; }
        public string DomesticOrTrade { get; set; }
        public string EmailAddresses { get; set; }
        public string CustomerOrderReference { get; set; }
        public string PriorityOrder { get; set; }
        public string USTaxState { get; set; }
        public string USTaxCounty { get; set; }
        public string USTaxExemptionCertificate { get; set; }
        public string USTaxDetails { get; set; }
        public string LastUpdated { get; set; }
        public string Database { get; set; }

        public List<Line> Lines { get; set; }

        [JsonIgnore]
        public string ToJSON
        {
            get
            {
                return JsonConvert.SerializeObject(this, Formatting.Indented);
            }
        }

        public Header(
            string basketId,
            string key,
            string tradeAccountCode,
            string deliveryReference,
            string deliveryName,
            string deliveryAddressLine1,
            string deliveryAddressLine2,
            string deliveryAddressLine3,
            string deliveryAddressLine4,
            string deliveryPostcode,
            string deliveryCountryCode,
            string deliveryPhone,
            string domesticOrTrade,
            string emailAddresses,
            string customerOrderReference,
            string priorityOrder,
            string usTaxState,
            string usTaxCounty,
            string usTaxExemptionCertificate,
            string usTaxDetails,
            string lastUpdated,
            List<Line> lines = null,
            string database = "MERTEX")
        {
            Database = database;

            BasketID = basketId;

            Key = key;

            TradeAccountCode = tradeAccountCode;
            DeliveryReference = String.IsNullOrEmpty(deliveryReference) ? null : deliveryReference;
            DeliveryName = String.IsNullOrEmpty(deliveryName) ? null : deliveryName;
            DeliveryAddressLine1 = String.IsNullOrEmpty(deliveryAddressLine1) ? null : deliveryAddressLine1;
            DeliveryAddressLine2 = String.IsNullOrEmpty(deliveryAddressLine2) ? null : deliveryAddressLine2;
            DeliveryAddressLine3 = String.IsNullOrEmpty(deliveryAddressLine3) ? null : deliveryAddressLine3;
            DeliveryAddressLine4 = String.IsNullOrEmpty(deliveryAddressLine4) ? null : deliveryAddressLine4;
            DeliveryPostcode = String.IsNullOrEmpty(deliveryPostcode) ? null : deliveryPostcode;
            DeliveryCountryCode = String.IsNullOrEmpty(deliveryCountryCode) ? null : deliveryCountryCode;
            DeliveryPhone = String.IsNullOrEmpty(deliveryPhone) ? null : deliveryPhone;
            DomesticOrTrade = domesticOrTrade;
            EmailAddresses = emailAddresses;
            CustomerOrderReference = customerOrderReference;
            PriorityOrder = priorityOrder;
            USTaxState = usTaxState;
            USTaxCounty = usTaxCounty;
            USTaxExemptionCertificate = usTaxExemptionCertificate;
            USTaxDetails = usTaxDetails;
            LastUpdated = lastUpdated;

            var endCustomer = false;

            using (var qe = new QueryExecutor(Database))
            {
                var ecDoc = XDocument.Parse(Framework.RemoveInvalidXmlChars(qe.Run(
                        "SLCUST",
                        $"CUSREF = \"{TradeAccountCode}\"",
                        "END.CUST.INV.DES"
                    )));

                var ec = ecDoc.Descendants("record").FirstOrDefault();

                if (ec != null)
                {
                    endCustomer = ec.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value == "Y";
                }
            }

            if (DeliveryReference != null && DeliveryReference != "C" || endCustomer)
            {
                if (endCustomer && DeliveryReference != null)
                {
                    FetchInvoiceAddress(DeliveryReference);
                    DeliveryReference = deliveryReference;
                }
                else
                {
                    FetchDeliveryAddress();
                }
            }
            else if (DeliveryReference == null &&
                     DeliveryAddressLine1 == null &&
                     DeliveryAddressLine2 == null &&
                     DeliveryAddressLine3 == null &&
                     DeliveryAddressLine4 == null && 
                     DeliveryPostcode == null &&
                     DeliveryCountryCode == null)
            {
                FetchInvoiceAddress(TradeAccountCode);
            }

            FetchLoginEmailAddress();

            Lines = lines.Where(p => p.Key == this.Key).ToList();
        }

        public void FetchLoginEmailAddress()
        {
            using (var qe = new QueryExecutor(Database))
            {
                EmailAddresses = $"{Framework.RemoveInvalidXmlChars(qe.Run("WTACCESS", $"PIN = \"{Key.Split(new char[] { '*' })[0]}\"", "EMAIL.1", "", "TEXT"))};{EmailAddresses}".TrimEnd(new char[] { ';' });
                EmailAddresses = String.Join(",", EmailAddresses?.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToArray());
            }
        }

        public void FetchDeliveryAddress()
        {
            using (var qe = new QueryExecutor(Database))
            {
                var addressDoc = XDocument.Parse(Framework.RemoveInvalidXmlChars(qe.Run(
                    "DELADD", 
                    $"DELREF = \"{DeliveryReference}\"", 
                    "DELNAME DELADDR1 DELADDR2 DELADDR3 DELADDR4 DELPCODE COUNTRY DELPHONE")));

                var da = addressDoc.Descendants("record").FirstOrDefault();

                if (da != null)
                {
                    DeliveryName = String.IsNullOrEmpty(da.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value) ? null :
                                                        da.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value;

                    DeliveryAddressLine1 = String.IsNullOrEmpty(da.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value) ? null :
                                                                da.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value;

                    DeliveryAddressLine2 = String.IsNullOrEmpty(da.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value) ? null :
                                                                da.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value;

                    DeliveryAddressLine3 = String.IsNullOrEmpty(da.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value) ? null :
                                                                da.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value;

                    DeliveryAddressLine4 = String.IsNullOrEmpty(da.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value) ? null :
                                                                da.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value;

                    DeliveryPostcode = String.IsNullOrEmpty(da.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value) ? null :
                                                            da.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value;

                    DeliveryCountryCode = String.IsNullOrEmpty(da.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value) ? null :
                                                                da.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value;

                    DeliveryPhone = String.IsNullOrEmpty(da.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value) ? null :
                                                            da.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value;
                }
            }
        }

        public void FetchInvoiceAddress(string customerReference)
        {
            DeliveryReference = "C";

            using (var qe = new QueryExecutor(Database))
            {
                var addressDoc = XDocument.Parse(Framework.RemoveInvalidXmlChars(qe.Run(
                    "SLCUST",
                    $"@ID = \"{customerReference}\"",
                     "CUSNAME CUSADD1X CUSADD2X CUSADD3X CUSADD4X CUSPCODE COUNTRY STD.TELEPHONE")));

                if (addressDoc.Descendants("record").Count() == 1)
                {
                    var da = addressDoc.Descendants("record").Single();

                    DeliveryName = String.IsNullOrEmpty(da.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value) ? null :
                                                        da.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value;

                    DeliveryAddressLine1 = String.IsNullOrEmpty(da.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value) ? null :
                                                                da.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value;

                    DeliveryAddressLine2 = String.IsNullOrEmpty(da.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value) ? null :
                                                                da.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value;

                    DeliveryAddressLine3 = String.IsNullOrEmpty(da.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value) ? null :
                                                                da.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value;

                    DeliveryAddressLine4 = String.IsNullOrEmpty(da.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value) ? null :
                                                                da.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value;

                    DeliveryPostcode = String.IsNullOrEmpty(da.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value) ? null :
                                                            da.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value;

                    DeliveryCountryCode = String.IsNullOrEmpty(da.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value) ? null :
                                                               da.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value;

                    DeliveryPhone = String.IsNullOrEmpty(da.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value) ? null :
                                                         da.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value;
                }
                else
                {
                    throw new Exception($"ERROR Processing Basket {Key} - More than one SLCUST record was found for {TradeAccountCode}");
                }
            }
        }

        public bool ClearBasket()
        {
#if DEBUG
            var result = "";
            var validResponses = new List<string>() { "Y", "N" };

            while (!validResponses.Contains(result.ToUpperInvariant()))
            {
                Console.WriteLine($"Clear Basket {BasketID}? Y/N");
                result = Console.ReadLine();
            }

            if (result.ToUpperInvariant() == "N")
            {
                return false;
            }
#endif

            using (var qe = new QueryExecutor(Database))
            {
                try
                {
                    var clearResult = qe.Run("B2B_CLEAR_BASKET", "", "", $"{BasketID}");
#if DEBUG
                    Console.WriteLine($"Clear Result: {clearResult}");
#endif
                }
                catch (Exception e)
                {
                    throw new Exception($"Something went wrong clearing Basket {BasketID} on {Database}", e);
                }

                return true;
            }
        }
    }
}
