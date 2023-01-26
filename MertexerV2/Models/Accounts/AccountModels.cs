using MertexerV2.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Accounts
{
    public class SSLPostDocument
    {
        //[UniVerseIndex(0)]
        //[UniVerseFieldName("CONTACTS.KEY")]
        public string W_SSLPOST_DOCTYPE { get; set; }

        [UniVerseIndex(1)]
        [UniVerseFieldName("CONTACT.EMAIL")]
        public string W_SSLPOST_EMAIL { get; set; }
    }

    public class SSLPostDocumentRead
    {
        public string W_SSLPOST_DOCTYPE { get; set; }

        public string W_SSLPOST_EMAIL { get; set; }

        public string CUSREF { get; set; }
    }

    public class PassPasswordSave
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class PassEmailSave
    {
        public string Email { get; set; }
    }

    public class PassNameSave
    {
        public string Name { get; set; }
    }

    public class PassTradePricingSave
    {
        public bool DisplayTradePrice { get; set; }
    }

    public class PassTradeUserManagementSave
    {
        public int? MemberID { get; set; }
        public string Email { get; set; }
        public bool ManageAccount { get; set; }
        public bool PlaceOrders { get; set; }
        public bool PlaceSamples { get; set; }
        public bool ViewAccounts { get; set; }
        public bool DownloadPricelistsBrochures { get; set; }
        public bool DownloadProductFeeds { get; set; }
        public short CountryID { get; set; }
        public byte LanguageID { get; set; }
    }

    public class PassProformaInvoiceSearch
    {
        public string orderStatus { get; set; }
        public short dateRange { get; set; }
        public string searchTerm { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
    }

    public class PassInvoiceSearch
    {
        public short dateRange { get; set; }
        public byte dateRangeType { get; set; }
        public string searchTerm { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
    }

    public class ReserveModifyResult
    {
        public TradeAccountInvoiceAddress invoiceAddress { get; set; }
        public TradeAccountDeliveryAddress deliveryAddress { get; set; }
    }

    public class TradeAccountInvoiceAddress
    {
        public string CustomerName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
    }

    public class TradeAccountDeliveryAddress
    {
        public string DeliveryReference { get; set; }
        public string DeliveryName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string HaulierDescription { get; set; }
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

        public string MD5Hash
        {
            get
            {
                string result = null;

                using (var md5 = System.Security.Cryptography.MD5.Create())
                {
                    addString(md5, String.IsNullOrEmpty(TradeAccountCode) ? "|" : $"{TradeAccountCode}|");
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

        public short? CountryID { get; set; }

        void addString(System.Security.Cryptography.ICryptoTransform transform, string inputString)
        {
            var buff = System.Text.Encoding.UTF8.GetBytes(inputString);
            transform.TransformBlock(buff, 0, buff.Length, buff, 0);
        }
    }

    public class MertexAddress
    {
        public string Phone { get; set; }
        public string Email { get; set; }
        public string PostalCode { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public string Line2 { get; set; }
        public string Line1 { get; set; }
        public string Company { get; set; }
        public string Name { get; set; }
        public string Label { get; }
        public string DeliveryReference { get; set; }
        public string Country { get; set; }
        public int DeliveryReferenceID { get; set; }
        public object TradeAccountCode { get; set; }
    }

    public class MertexInvoiceAddress : MertexAddress
    {
        public string DefaultDeliveryReference { get; set; }
        public short? CountryID { get; set; }
    }

    public class InvoiceSearchReturn
    {
        public List<InvoiceResult> SearchResults { get; set; }

        public InvoiceSearchReturn()
        {
            SearchResults = new List<InvoiceResult>();
        }
    }
    public class ListProformaInvoicesReturn
    {
        public decimal OnAccountTotal { get; set; }
        public List<ProformaInvoiceResult> Invoices { get; set; }

        public ListProformaInvoicesReturn()
        {
            Invoices = new List<ProformaInvoiceResult>();
        }
    }

    public class ProformaInvoiceResult
    {
        public string OrderNumber { get; set; }
        public string DocumentKey { get; set; }
        public DateTime DocumentDate { get; set; }
        public string CustomerOrderReference { get; set; }
        public decimal Value { get; set; }
        public string OrderType { get; set; }
    }

    public class ListInvoicesReturn
    {
        public decimal OnAccountTotal { get; set; }
        public List<InvoiceResult> Invoices { get; set; }

        public ListInvoicesReturn()
        {
            Invoices = new List<InvoiceResult>();
        }
    }

    public class InvoiceResult
    {
        public string DocumentNumber { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerOrderReference { get; set; }
        public decimal OriginalValue { get; set; }
        public decimal OutstandingAmount { get; set; }
        public string Disputed { get; set; }

        public string DocumentKey
        {
            get
            {
                return System.Web.HttpUtility.UrlEncode(Cryptography.Encrypt(DocumentNumber));
            }
        }
    }

    public class AccountSummaryRecentPaymentsReturn
    {
        public List<OnAccountResult> PaymentsOnAccount { get; set; }

        public AccountSummaryRecentPaymentsReturn()
        {
            PaymentsOnAccount = new List<OnAccountResult>();
        }
    }

    public class OnAccountResult
    {
        public string DocumentReference { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal OriginalValue { get; set; }
        public decimal OutstandingAmount { get; set; }
        public string DocumentKey
        {
            get
            {
                return System.Web.HttpUtility.UrlEncode(Cryptography.Encrypt(DocumentReference));
            }
        }
    }

    public class AccountSummaryReturn
    {
        public List<string> ColumnHeadings { get; set; } // C1 sterling / currency...? single or mv?
        public List<string> MonthsDocumentDate { get; set; } // C2 mv, pipe-separated pairs: month|year month|year month|year, unhelpful
        public List<string> MonthsDocDateSelectionCriteria { get; set; } // C3 the selection criteria for the following...?
        public List<decimal> AmountBaseDocDate { get; set; } // C4 pipe-separated values, match with pairs above: IF CURR != STERL
        public List<decimal> AmountCurrDocDate { get; set; } // C5 pipe-separated values, match with pairs above: IF CURR != STERL

        public List<string> MonthsDueDate { get; set; } // C6 mv, pipe-separated pairs: month|year month|year month|year, unhelpful
        public List<string> MonthsDueDateSelectionCriteria { get; set; } // C7 the selection criteria for the following...?
        public List<decimal> AmountBaseDueDate { get; set; } // C8 pipe-separated values, match with pairs above
        public List<decimal> AmountCurrDueDate { get; set; } // C9 pipe-separated values, match with pairs above

        public decimal TotalBaseDoc { get; set; } // C10 Total Base [by Document Date]
        public decimal TotalCurrDoc { get; set; } // C11 Total Curr [by Document Date]

        public decimal TotalBaseDue { get; set; } // C12 Total Base [by Due Date]
        public decimal TotalCurrDue { get; set; } // C13 Total Currency [by Due Date]

        public AccountSummaryReturn ()
        {
            ColumnHeadings = new List<string>();
            MonthsDocumentDate = new List<string>();
            MonthsDocDateSelectionCriteria = new List<string>();
            AmountBaseDocDate = new List<decimal>();
            AmountCurrDocDate = new List<decimal>();
            MonthsDueDate = new List<string>();
            MonthsDueDateSelectionCriteria = new List<string>();
            AmountBaseDueDate = new List<decimal>();
            AmountCurrDueDate = new List<decimal>();
            TotalBaseDoc = 0m;
            TotalCurrDoc = 0m;
            TotalBaseDue = 0m;
            TotalCurrDue = 0m;
        }
    }

    public class OutstandingOrdersReturn
    {
        public List<OutstandingOrderResult> Results { get; set; }

        public OutstandingOrdersReturn()
        {
            Results = new List<OutstandingOrderResult>();
        }
    }

    public class OutstandingOrderResult
    {
        public string UniVerseAccount { get; set; }
        public int LineNumber { get; set; }
        public int OrderNumber { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal OrderQuantity { get; set; }
        public string Product { get; set; }
        public string Description { get; set; }
        public string CustomerOrderReference { get; set; }
        public string Status { get; set; }
        public string Tracking { get; set; }
        public string BaseProduct { get; set; }
        public DateTime? DespatchStatusDate { get; set; }
        public string DespatchStatusDateMessage { get; set; }
        public DateTime? ReservedUntil { get; set; }
        public string ReservedUntilMessage { get; set; }
        public string ReserveAction { get; set; }
        public string DeliveryReference { get; set; }
        public string Batch { get; set; }
        public string CustomerOrderNumber { get; set; }

        public OutstandingOrderResult(string uva)
        {
            this.UniVerseAccount = uva;
        }
    }

    public class PassOutstandingOrdersSearch
    {
        public string orderStatus { get; set; }
        public short dateRange { get; set; }
        public string searchTerm { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public bool sampleOrders { get; set; }
    }

    public class PassParcelCount
    {
        public string tradeAccountNumber { get; set; }

        public string documentNumber { get; set; }

        public string DocumentKey
        {
            get
            {
                return System.Web.HttpUtility.UrlEncode(Cryptography.Encrypt(documentNumber));
            }
        }
    }

    public class ParcelCountReturn
    {
        public List<ParcelTrackingDetail> Results { get; set; }

        public ParcelCountReturn()
        {
            Results = new List<ParcelTrackingDetail>();
        }
    }

    public class ParcelTrackingDetail
    {
        public string Parcel { get; set; }
        public string ProductCodes { get; set; }
        public string Descriptions { get; set; }
        public string TrackingURL { get; set; }
    }

    public class OrderDeliveryInfoReturn
    {
        public OrderDeliveryInfo DeliveryInfo { get; set; }
        public List<OrderLineDetail> OrderLines { get; set; }

        public OrderDeliveryInfoReturn()
        {
            DeliveryInfo = null;
            OrderLines = new List<OrderLineDetail>();
        }
    }

    public class OrderDeliveryInfo
    {
        public string DeliveryReference { get; set; }
        public string DeliveryName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string HaulierDescription { get; set; }
    }

    public class OrderLineDetail
    {
        public string UniVerseAccount { get; set; }

        public string LineNumber { get; set; }
        public string OrderNumber { get; set; }
        public string Product { get; set; }
        public string Reserved { get; set; }
        public string Memo { get; set; }
        public string Description { get; set; }
        public decimal Quantity { get; set; }
        public string BatchNumber { get; set; }
        public string CustomerOrderReference { get; set; }
        public string OrderStatus { get; set; }
        public string DespatchStatus { get; set; }
        public string DeliveryReference { get; set; }
        public string Tracking { get; set; }
        public string WebProduct { get; set; }
        public int? ProductID { get; set; }
        public long? ImageID { get; set; }
        public string ReserveFlag { get; set; }

        public OrderLineDetail(string universeAccount)
        {
            this.UniVerseAccount = universeAccount;
        }

        public string name
        {
            set { }
            get
            {
                return Description;
            }
        }

        public string code
        {
            set { }
            get
            {
                return Product;
            }
        }
    }

    public class ModifyReserveLineResult
    {
        public string errorMessage { get; set; }
    }

    public class PassReserveOrderCountryChange
    {
        public byte languageId { get; set; }
        public string deliveryReference { get; set; }
        public string orderNumber { get; set; }
        public PassBasketAddress desiredDestination { get; set; }
    }

    public class PassQuickSearch
    {
        public string searchTerm { get; set; }
        public short countryId { get; set; }
        public byte languageId { get; set; }
        public byte? productTypeId { get; set; }
    }

    public class PassInvoiceAddressChangeRequest
    {
        public byte LanguageID { get; set; }
        public string CurrentJSON { get; set; }
        public string CurrentDisplayBlock { get; set; }
        public string RequestedJSON { get; set; }
        public string RequestedDisplayBlock { get; set; }
    }

    public class PassOrderNumber
    {
        public string orderNumber { get; set; }
    }

    public class PassReserveFetch
    {
        public string universeAccount { get; set; }
        public string orderNumber { get; set; }
        public string tradeAccountCode { get; set; }
        public string deliveryReference { get; set; }
    }

    public class PassReserveModify : PassReserveFetch
    {
        public string action { get; set; }
    }

    public class PassReserveLine
    {
        public string universeAccount { get; set; }
        public byte languageId { get; set; }
        public string lineKey { get; set; } //LINEKEYT
        public string action { get; set; } // UPDTYPE
        public decimal? quantity { get; set; } //QUANTITY
        public string lineReference { get; set; } //CUSORDREF
        public string lineMemo { get; set; } // LINE.COMM (lines breaks delimited by pipe)
        public string batch { get; set; }
        public string code { get; set; }
        public bool isLineQuantityIncrease { get; set; }
        public decimal? requestedIncreasedQuantity { get; set; }
        public bool isDateExtension { get; set; }
        public DateTime? requestedExtension { get; set; }
    }

    public class PassPCAPredictFind
    {
        public string mode { get; set; }
        public string id { get; set; }
        public string text { get; set; }
        public string container { get; set; }
        public string origin { get; set; }
        public string countries { get; set; }
        public string limit { get; set; }
        public string language { get; set; }
        public string tradeAccountCode { get; set; }
    }

    public class PassAccountSummaryDrilldown
    {
        public string Criteria { get; set; }
    }

    public class PassCountryLanguage
    {
        public short countryId { get; set; }
        public byte languageId { get; set; }
    }

    public class PassBasketAddress
    {
        public string tradeAccountCode { get; set; }
        public long basketId { get; set; }
        public string deliveryReference { get; set; }
        public string deliveryName { get; set; }
        public string deliveryCompanyName { get; set; }
        public string deliveryAddressLine1 { get; set; }
        public string deliveryAddressLine2 { get; set; }
        public string deliveryTown { get; set; }
        public string deliveryCounty { get; set; }
        public string deliveryPostcode { get; set; }
        public short? deliveryCountryId { get; set; }
        public string deliveryCountryCode { get; set; }
        public string deliveryPhoneNumber { get; set; }
        public string email { get; set; }
        public string endCustomerCode { get; set; }
        public string endCustomerName { get; set; }
    }

    public class PassBasketAddressCleanse : PassBasketAddress
    {
        public bool isLoqateAddress { get; set; }
        public bool forceCleanse { get; set; }
        public string status { get; set; }

        public string md5Hash
        {
            get
            {
                string result = null;

                using (var md5 = System.Security.Cryptography.MD5.Create())
                {

                    addString(md5, String.IsNullOrEmpty(tradeAccountCode) ? "|" : $"{tradeAccountCode}|");
                    addString(md5, String.IsNullOrEmpty(deliveryAddressLine1) ? "|" : $"{deliveryAddressLine1}|");
                    addString(md5, String.IsNullOrEmpty(deliveryAddressLine2) ? "|" : $"{deliveryAddressLine2}|");
                    addString(md5, String.IsNullOrEmpty(deliveryTown) ? "|" : $"{deliveryTown}|");
                    addString(md5, String.IsNullOrEmpty(deliveryCounty) ? "|" : $"{deliveryCounty}|");
                    addString(md5, String.IsNullOrEmpty(deliveryPostcode) ? "|" : $"{deliveryPostcode}|");
                    addString(md5, $"{deliveryCountryCode}");

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
        public string label
        {
            get
            {
                var lines = new List<string>()
                {
                    deliveryName,
                    deliveryCompanyName,
                    deliveryAddressLine1,
                    deliveryAddressLine2,
                    deliveryTown,
                    deliveryCounty,
                    deliveryPostcode
                }.ToList();

                lines.RemoveAll(c => String.IsNullOrEmpty(c));

                var displayLine = String.Join(", ", lines);

                return $"{deliveryReference} - {displayLine}";
            }
        }

        void addString(System.Security.Cryptography.ICryptoTransform transform, string inputString)
        {
            var buff = System.Text.Encoding.UTF8.GetBytes(inputString);
            transform.TransformBlock(buff, 0, buff.Length, buff, 0);
        }

        public bool manual { get; set; }

        public bool skipDedupe { get; set; }
    }

    public class PassDeliveryReference
    {
        public string deliveryReference { get; set; }
    }
}
