using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2SQL.Models.Misc
{
    public class PassProductCode
    {
        public string code { get; set; }
    }

    public class MiscellaneousInvoiceAddress
    {
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string PhoneNumber { get; set; }
        public short? CountryID { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string PotentialCountryCode { get; set; }
        public string Email { get; set; }
    }

    public class PassPCAPredictFind
    {
        public string id { get; set; }
        public string text { get; set; }
        public string container { get; set; }
        public string origin { get; set; }
        public string countries { get; set; }
        public string limit { get; set; }
        public string language { get; set; }
        public string tradeAccountCode { get; set; }
        public string endCustomerTradeAccountCode { get; set; }
    }

    public class MertexPCALookupResult
    {
        public readonly string Type = "Mertex";

        public int DeliveryReferenceID { get; set; }

        public string DeliveryReference { get; set; }

        public string TradeAccountCode { get; set; }

        public string Label
        {
            get
            {
                var lines = new List<string>()
                {
                    Name,
                    Company,
                    Line1,
                    Line2,
                    City,
                    Province,
                    PostalCode
                }.ToList();

                lines.RemoveAll(c => String.IsNullOrEmpty(c));

                var displayLine = String.Join(", ", lines);

                return $"{DeliveryReference} - {displayLine}";
            }
        }

        public string Name { get; set; }
        public string Company { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string Province { get; set; } // County
        public string PostalCode { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public short? CountryID { get; set; }

        public string MD5Hash
        {
            get
            {
                string result = null;

                using (var md5 = System.Security.Cryptography.MD5.Create())
                {
                    void addString(System.Security.Cryptography.ICryptoTransform transform, string inputString)
                    {
                        var buff = System.Text.Encoding.UTF8.GetBytes(inputString);
                        transform.TransformBlock(buff, 0, buff.Length, buff, 0);
                    }

                    addString(md5, String.IsNullOrEmpty(TradeAccountCode) ? "|" : $"{TradeAccountCode}|");
                    addString(md5, String.IsNullOrEmpty(Name) ? "|" : $"{Name}|");
                    addString(md5, String.IsNullOrEmpty(Company) ? "|" : $"{Company}|");
                    addString(md5, String.IsNullOrEmpty(Line1) ? "|" : $"{Line1}|");
                    addString(md5, String.IsNullOrEmpty(Line2) ? "|" : $"{Line2}|");
                    addString(md5, String.IsNullOrEmpty(City) ? "|" : $"{City}|");
                    addString(md5, String.IsNullOrEmpty(Province) ? "|" : $"{Province}|");
                    addString(md5, String.IsNullOrEmpty(PostalCode) ? "|" : $"{PostalCode}|");
                    addString(md5, $"{Country}");

                    md5.TransformFinalBlock(new byte[0], 0, 0);

                    var sb = new System.Text.StringBuilder();

                    foreach (var b in md5.Hash)
                    {
                        sb.Append(b.ToString("x2"));
                    }

                    result = sb.ToString();
                }

                return result;
            }
        }
    }

    public class PassAutocompleteTerm
    {
        [StringLength(50)]
        public string term { get; set; }
        public string searchTerm { get; set; }
        public int? tradeAccountId { get; set; }
        public string tradeAccountCode { get; set; }
        public short? marketId { get; set; }
        public short? tradePriceListId { get; set; }
        public byte? currencyId { get; set; }
        public bool includePreLaunch { get; set; }
    }

    public class PassDuplicateLineCheck
    {
        public long basketId { get; set; }
        public string tradeAccountCode { get; set; }
        public string lineReference { get; set; }
        public int productId { get; set; }
        public string productCode { get; set; }
        public decimal quantity { get; set; }
    }

    public class DuplicateLineCheckResult
    {
        public bool isDuplicate { get; set; }
        public bool isCurrentBasketDuplicate { get; set; }
        public List<string> orderNumbers { get; set; }

        public DuplicateLineCheckResult()
        {
            isDuplicate = false;
            isCurrentBasketDuplicate = false;
            orderNumbers = new List<string>();
        }
    }

    public class PassOrderReference
    {
        public string tradeAccountCode { get; set; }
        public string OrderReference { get; set; }
    }
    public class TradeAccountVATNumber
    {
        public string CountryCode { get; set; }
        public string VATNumber { get; set; }
        public bool Errored { get; set; }
    }

    public class TradeAccountInvoiceTerms
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class PassVATNumber
    {
        public string vatNumber { get; set; }
        public string countryCode { get; set; }

        public string requesterVATNumber { get; set; }
        public string requesterCountryCode { get; set; }
    }

    public class PassEORINumber
    {
        public string eoriNumber { get; set; }
    }

    public class TradeAccountAddressReturn
    {
        public TradeAccountInvoiceAddress InvoiceAddress { get; set; }
        public TradeAccountPrimaryAddress PrimaryAddress { get; set; }
        public List<TradeAccountPrimaryAddress> PrimaryAddresses { get; set; } // this retained because the sample ordering app uses it...
        public List<TradeAccountSecondaryAddress> SecondaryAddresses { get; set; }

        public TradeAccountAddressReturn()
        {
            InvoiceAddress = new TradeAccountInvoiceAddress();
            PrimaryAddress = new TradeAccountPrimaryAddress();
            PrimaryAddresses = new List<TradeAccountPrimaryAddress>();
        }
    }

    public class TradeAccountPrimaryAddress
    {
        public string DeliveryReference { get; set; }
        public string CompanyName { get; set; }
        public string Name { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public short? CountryID { get; set; }
        public bool? EU { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public class TradeAccountSecondaryAddress : TradeAccountPrimaryAddress
    {
        public bool Default { get; set; }
    }

    public class TradeAccountInvoiceAddress : TradeAccountPrimaryAddress
    {

    }

    public class EndCustomerAddress : TradeAccountPrimaryAddress
    {
        public int TradeAccountID { get; set; }
        public string TradeAccountCode { get; set; }
        public string TradeAccountName { get; set; }
    }

    public class PassTradeAccountNumber
    {
        public string accountNumber { get; set; }
    }
    public class FetchLinkedDocumentsResult
    {
        public List<LinkedDocument> LinkedDocuments { get; set; }

        public FetchLinkedDocumentsResult()
        {
            LinkedDocuments = new List<LinkedDocument>();
        }
    }

    public class LinkedDocument
    {
        public string Path { get; set; }
        public string Description { get; set; }

        [NotMapped]
        public string FileName
        {
            get
            {
                return Path.Substring(Path.LastIndexOf("\\") + 1);
            }
            set { }
        }
    }

    public class MertexRetailTill
    {
        public string TillNumber { get; set; }
        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            return ((MertexRetailTill)obj).TillNumber == TillNumber;
        }

        public override int GetHashCode()
        {
            return TillNumber.GetHashCode();
        }
    }

    public class MertexRetailTillSession
    {
        public string SessionNumber { get; set; }
        public DateTime OpenedOn { get; set; }
        public TimeSpan OpenedAt { get; set; }
        public string OpenedBy { get; set; }
        public string Label
        {
            get
            {
                return $"Session {SessionNumber} Opened: {OpenedOn.ToString("dd/MM/yyyy HH:mm")} by: {OpenedBy}";
            }
        }
    }

    public class PassRetailTillNumber
    {
        public string retailTillNumber { get; set; }
    }
}
