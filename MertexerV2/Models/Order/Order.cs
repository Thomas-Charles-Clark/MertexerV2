using MertexerV2.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MertexerV2.Models
{
    public class Order
    {
        /// <summary>
        /// (-1).
        /// A valid SLTERMS code.
        /// If set, this overrides the Invoice Terms on the Order.
        /// </summary>
        public string InvoiceTerms { get; set; }

        /// <summary>
        /// (0).
        /// The Order's BasketID in Mertex, takes the form CustomerReference*OrderNumber*GUID
        /// </summary>
        public string BasketID { get; set; }

        /// <summary>
        /// 1.
        /// The Mertex Customer Reference
        /// </summary>
        public string CustomerReference { get; set; }

        /// <summary>
        /// 2.
        /// The delivery reference to use.
        /// If using an existing Delivery Address from DELADD then set here.
        /// If using the Customer’s address or an adhoc address leave blank.
        /// </summary>
        public string DeliveryReference { get; set; }

        /// <summary>
        /// 3.
        /// If using an adhoc address then set to the required address, in the following format (pipe delimited):
        /// Name
        /// Address Line 1
        /// Address Line 2
        /// Address Line 3
        /// Address Line 4
        /// Post Code
        /// Phone
        /// (D)omestic or (T)rade
        /// Country Code
        /// Save Address? (Y or blank)
        /// US State Code
        /// US County Code
        /// </summary>
        public string DeliveryAddress { get; set; }

        /// <summary>
        /// 4.
        /// Notification Email Address
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// 5.
        /// Customer's Order Reference
        /// </summary>
        public string CustomerOrderReference { get; set; }

        /// <summary>
        /// 6.
        /// A valid Mertex Haulier Code
        /// </summary>
        public string HaulierCode { get; set; }

        /// <summary>
        /// 7.
        /// Y or blank
        /// </summary>
        public string PriorityOrder { get; set; }

        /// <summary>
        /// 8.
        /// Y, N or blank
        /// </summary>
        public string GiftWrap { get; set; }

        /// <summary>
        /// 9.
        /// Y, N or blank
        /// </summary>
        public string PartDeliveryAllowed { get; set; }

        /// <summary>
        /// 10.
        /// The value of the Order.
        /// Apparently, this should be inc. or ex. VAT depending on whether or not the
        /// order itself (or possibly the customer?) is inc. or ex. VAT.
        /// </summary>
        public string OrderValue { get; set; }

        /// <summary>
        /// 11.
        /// A valid Mertex Carriage Code
        /// </summary>
        public string CarriageCode { get; set; }

        /// <summary>
        /// 12.
        /// A valid Mertex VAT Code
        /// </summary>
        public string CarriageVATCode { get; set; }

        /// <summary>
        /// 13.
        /// The Carriage Value including VAT
        /// </summary>
        public string CarriageValue { get; set; }

        /// <summary>
        /// 14.
        /// The Carriage VAT Value
        /// </summary>
        public string CarriageVATValue { get; set; }

        /// <summary>
        /// 15.
        /// The order number to use.
        /// If blank, Mertex calculates.
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        /// 16.
        /// Tax Exemption Certificate reference for US Tax.
        /// </summary>
        public string USTaxExemptionCertificate { get; set; }

        /// <summary>
        /// 17.
        /// For orders loaded from a CSV file this contains the name of the file.
        /// </summary>
        public string FromCSVFile { get; set; }

        /// <summary>
        /// 18.
        /// Not used.
        /// </summary>
        public string MassSampleEntryBasketIDs { get; set; }

        /// <summary>
        /// 19.
        /// Y, N or blank
        /// </summary>
        public string HoldForSales { get; set; }

        /// <summary>
        /// 20.
        /// Order Header Promotion Code
        /// </summary>
        public string PromotionCode { get; set; }

        /// <summary>
        /// 21.
        /// (C)ontract, (D)isplay or (N)ot Applicable
        /// </summary>
        public string ContractOrDisplay { get; set; }

        /// <summary>
        /// 22.
        /// 'Y' or 'N'
        /// </summary>
        public string Urgent { get; set; }

        /// <summary>
        /// 23.
        /// [Disciption MISSING]
        /// </summary>
        public string BespokeSampleType { get; set; }

        /// <summary>
        /// 24.
        /// Customer Requires Full Despatch,
        /// 'Y' or blank
        /// </summary>
        public string CustomerFullDispatch { get; set; }

        /// <summary>
        /// 25.
        /// 'Y' or a number between 1 and 9
        /// </summary>
        public string AutoAllocationPriority { get; set; }

        /// <summary>
        /// 26.
        /// 'Y' or 'N'
        /// </summary>
        public string SendExpress { get; set; }

        /// <summary>
        /// 27.
        /// Is the Customer paying for the Order to be sent express?
        /// 'Y' or 'N'
        /// </summary>
        public string CustomerPaying { get; set; }

        /// <summary>
        /// 28.
        /// Customer’s Haulier Account No (if charging delivery to customer).
        /// </summary>
        public string HaulierAccountNumber { get; set; }

        /// <summary>
        /// 29.
        /// Charge taxes to the Invoice Account
        /// 'Y' or 'N'
        /// </summary>
        public string TaxesDutiesToInvoiceAccount { get; set; }

        /// <summary>
        /// 30.
        /// 'Y', 'N' or blank
        /// </summary>
        public string RetailOrder { get; set; }

        /// <summary>
        /// 31.
        /// [Disciption MISSING]
        /// </summary>
        public string ContactName { get; set; }

        /// <summary>
        /// 32.
        /// [Disciption MISSING]
        /// </summary>
        public string ContactPhone { get; set; }

        /// <summary>
        /// 33.
        /// [Disciption MISSING]
        /// </summary>
        public string DeliveryTerms { get; set; }

        /// <summary>
        /// 34.
        /// [Disciption MISSING]
        /// </summary>
        public string DeliveryType { get; set; }

        /// <summary>
        /// 35.
        /// A valid Mertex Order Taker
        /// </summary>
        public string OrderTaker { get; set; }

        /// <summary>
        /// 36.
        /// [Disciption MISSING]
        /// </summary>
        public string TradeAccount { get; set; }

        /// <summary>
        /// 37.
        /// [Disciption MISSING]
        /// </summary>
        public string LoyaltyCard { get; set; }

        /// <summary>
        /// 38.
        /// [Disciption MISSING]
        /// </summary>
        public string DistributorCode { get; set; }

        /// <summary>
        /// 39.
        /// [Disciption MISSING]
        /// </summary>
        public string TransferGLCode { get; set; }

        /// <summary>
        /// 40.
        /// 'Y' or 'N'
        /// </summary>
        public string ApplyMiscellaneousCharge { get; set; }

        /// <summary>
        /// 41.
        /// [Description MISSING]
        /// </summary>
        public string ChargeCode { get; set; }

        /// <summary>
        /// 42.
        /// [Description MISSING]
        /// </summary>
        public string MiscellaneousCharge { get; set; }

        /// <summary>
        /// 43.
        /// Blank or one of (E)mail, (F)ax, (P)ost or (N)ot at all.
        /// </summary>
        public string PreferredContactMethod { get; set; }

        /// <summary>
        /// 44.
        /// [Description MISSING]
        /// </summary>
        public string ContactEmailFax { get; set; }

        /// <summary>
        /// 45.
        /// 'Y', 'N' or blank
        /// </summary>
        public string FullDispatchOnly { get; set; }

        /// <summary>
        /// 46.
        /// 'Y', 'N' or blank
        /// </summary>
        public string HeldForDelivery { get; set; }

        /// <summary>
        /// 47.
        /// Pipe delimited
        /// </summary>
        public string ExpressReason { get; set; }

        /// <summary>
        /// 48.
        /// [Description MISSING]
        /// </summary>
        public string UPSReturnsLabel { get; set; }

        /// <summary>
        /// 49.
        /// 'Y', 'N' or blank
        /// </summary>
        public string CarriageExempt { get; set; }

        /// <summary>
        /// 50.
        /// 'Y', 'N' or blank
        /// </summary>
        public string HeldForExpress { get; set; }

        /// <summary>
        /// 51.
        /// 'Y', 'N' or blank
        /// </summary>
        public string CreditHeld { get; set; }

        /// <summary>
        /// 52.
        /// Not used
        /// </summary>
        public string FurnitureSystem { get; set; }

        /// <summary>
        /// 53.
        /// Not used
        /// </summary>
        public string EndCustomer { get; set; }

        /// <summary>
        /// 54.
        /// 'Y' or blank
        /// </summary>
        public string WebHeld { get; set; }

        /// <summary>
        /// 55.
        /// [Description MISSING]
        /// </summary>
        public string FOCIndicator { get; set; }

        /// <summary>
        /// 56.
        /// [Description MISSING]
        /// </summary>
        public string FOCBudgetManager { get; set; }

        /// <summary>
        /// 57.
        /// [Description MISSING]
        /// </summary>
        public string OrderPromotionCode { get; set; }

        /// <summary>
        /// 58.
        /// [Description MISSING]
        /// </summary>
        public string OrderPromotionDiscount { get; set; }

        /// <summary>
        /// 59.
        /// Pipe delimited list of Header References
        /// </summary>
        public string HeaderReferences { get; set; }

        /// <summary>
        /// 60.
        /// 'Y', 'N' or blank
        /// </summary>
        public string SendOrderConfirmationEmail { get; set; }

        /// <summary>
        /// 61.
        /// Pipe delimited, including:
        /// Name
        /// Address Line 1
        /// Address Line 2
        /// Address Line 3
        /// Address Line 4
        /// Post Code
        /// Contact Name
        /// Telephone Number
        /// Fax Number
        /// Email Address
        /// </summary>
        public string MiscellaneousAccountAddress { get; set; }

        /// <summary>
        /// 62.
        /// The related claim number.
        /// </summary>
        public string RaisedAsResultOfClaim { get; set; }

        /// <summary>
        /// 63.
        /// Pipe delimited, including:
        /// Exit Code
        /// Delivery
        /// Street
        /// Road
        /// Town
        /// County
        /// Post Code
        /// </summary>
        public string EAD { get; set; }

        /// <summary>
        /// 64.
        /// A pipe delimited list of Clause Codes to apply to the order.
        /// </summary>
        public string InvoiceClauseCodes { get; set; }

        /// <summary>
        /// 65.
        /// A pipe delimited list of Clause Text to apply to the order.
        /// </summary>
        public string InvoiceClauseText { get; set; }

        /// <summary>
        /// 66.
        /// A pipe delimited list comment string associated with the Invoice Clauses.
        /// </summary>
        public string InvoiceClauseComments { get; set; }

        /// <summary>
        /// 67.
        /// Till number for retail payments
        /// </summary>
        public string TillNumber { get; set; }

        /// <summary>
        /// 68.
        /// Session number for retail payments
        /// </summary>
        public string SessionNumber { get; set; }

        /// <summary>
        /// 69.
        /// A pipe delimited list of payment types
        /// </summary>
        public string PaymentType { get; set; }

        /// <summary>
        /// 70.
        /// A pipe delimited list of credit card types
        /// </summary>
        public string CreditCardType { get; set; }

        /// <summary>
        /// 71.
        /// A pipe delimited list of payment amounts
        /// </summary>
        public string PaymentAmount { get; set; }

        /// <summary>
        /// 72.
        /// A pipe delimited list of Invoice Comment Codes to apply to the order.
        /// </summary>
        public string InvoiceCommentCodes { get; set; }

        /// <summary>
        /// 73.
        /// A pipe delimited list of Invoice Comment Text to apply to the order.
        /// </summary>
        public string InvoiceCommentText { get; set; }

        /// <summary>
        /// 74.
        /// A pipe delimited list of Packing Note Comment Codes to apply to the order.
        /// </summary>
        public string PackingNoteCommentCodes { get; set; }

        /// <summary>
        /// 75.
        /// A pipe delimited list of Packing Note Comment Text to apply to the order.
        /// </summary>
        public string PackingNoteCommentText { get; set; }

        /// <summary>
        /// 76.
        /// A pipe delimited list of Delivery Note Comment Codes to apply to the order.
        /// </summary>
        public string DeliveryNoteCommentCodes { get; set; }

        /// <summary>
        /// 77.
        /// A pipe delimited list of Delivery Note Comment Text to apply to the order.
        /// </summary>
        public string DeliveryNoteCommentText { get; set; }

        /// <summary>
        /// 78.
        /// A pipe delimited list of Proforma Comment Codes to apply to the order.
        /// </summary>
        public string ProformaCommentCodes { get; set; }

        /// <summary>
        /// 79.
        /// A pipe delimited list of Proforma Comment Text to apply to the order.
        /// </summary>
        public string ProformaCommentText { get; set; }

        /// <summary>
        /// 80.
        /// A pipe delimited list of Confirmation Comment Codes to apply to the order.
        /// </summary>
        public string ConfirmationCommentCodes { get; set; }

        /// <summary>
        /// 81.
        /// A pipe delimited list of Confirmation Comment Text to apply to the order.
        /// </summary>
        public string ConfirmationCommentText { get; set; }

        /// <summary>
        /// 82.
        /// A pipe delimited list of Pick/Cut List Comment Codes to apply to the order.
        /// </summary>
        public string PickCutListCommentCodes { get; set; }

        /// <summary>
        /// 83.
        /// A pipe delimited list of Pick/Cut List Comment Text to apply to the order.
        /// </summary>
        public string PickCutListCommentText { get; set; }

        /// <summary>
        /// 84.
        /// Held Order Reason Code. Must be a valid code from OEHOLDREASON
        /// </summary>
        public string HeldOrderReasonCode { get; set; }

        /// <summary>
        /// 85. 
        /// Rep Code. Must be a valid Mertex rep code. Overrides the rep for the order.
        /// </summary>
        public string RepCode { get; set; }

        /// <summary>
        /// 86.
        /// Agent Code. Must be a valid Mertex agent code. Overrides the agent for the order.
        /// </summary>
        public string AgentCode { get; set; }

        /// <summary>
        /// 87. 
        /// Charge On. Charge carriage by "Parcel" or by "Value"? Leave blank to use the setting from 
        /// the first product on the order. If no setting is found on the first product then charge 
        /// by value is assumed.
        /// </summary>
        public string ChargeOn { get; set; }

        /// <summary>
        /// 88.
        /// Bill Duty Account Number. UPS Account Number to be charged duty and taxes.
        /// </summary>
        public string BillDutyAccountNumber { get; set; }

        /// <summary>
        /// 89.
        /// HoldForSalesReasonCode. The reason code to pair with the Held For Sales flag.
        /// </summary>
        public string HoldForSalesReasonCode { get; set; }

        /// <summary>
        /// 90.
        /// Order Memo. Self explanatory.
        /// </summary>
        public string OrderMemo { get; set; }

        /// <summary>
        /// 91.
        /// Call Type.
        /// </summary>
        public string CallType { get; set; }

        /// <summary>
        /// (92).
        /// A valid OrderType code.
        /// If set, this overrides the OrderType on the Order.
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// 93. 
        /// Sample Order Only.
        /// </summary>
        public string SampleOrderOnly { get; set; }

        /// <summary>
        /// 94.
        /// Use Invoice Address.
        /// Y if the Customer’s Invoice address should be used on the order. 
        /// If set this overrides the contents of Delivery Reference and Delivery Address fields 
        /// and also overrides logic that would otherwise apply the Customer’s default delivery 
        /// address if one is specified on the Customer in Mertex.
        /// </summary>
        public string UseInvoiceAddress { get; set; }

        /// <summary>
        /// 95.
        /// Retail Payment Comments
        /// Currently (and for reasons unfathomable) the Retail staff, when using SBClient to provide payment information
        /// about their Retail orders - specifically when taking payments using our Online Retail Payment System - place
        /// Braintree Payment References into this one "comments" field, which seems to be shared across all the payment information about an order.
        /// This will be filled by portal.Basket_Process to contain a semicolon separated single string of Payment References.
        /// </summary>
        public string RetailPaymentComments { get; set; }

        /// <summary>
        /// 96.
        /// Retail Payment References
        /// This is the per-payment reference field which is displayed when providing a Retail Payment via SBClient
        /// (you know, the one I described in field 94 ^ up there. The one they're not using.) 
        /// This will be filled with a pipe-delimited list of payment references.
        /// </summary>
        public string RetailPaymentReferences { get; set; }

        /// <summary>
        /// 97.
        /// B2B Ready
        /// If Y then the trade site is configured not to auto create orders for confirmed baskets 
        /// and this basket has been created on the trade site and the user has confirmed their order.
        /// </summary>
        public string B2BReadyOrCreateViaPortal { get; set; }

        /// <summary>
        /// 98.
        /// Atlas Delivery ANA Number
        /// JLP Atlas Delivery ANA Number
        /// </summary>
        public string AtlasDeliveryANANumber { get; set; }

        /// <summary>
        /// 99.
        /// Atlas Invoicee ANA Number
        /// JLP Atlas ANA Number
        /// </summary>
        public string AtlasInvoiceeANANumber { get; set; }

        /// <summary>
        /// 100.
        /// Atlas Direct to Customer ANA Number
        /// JLP Atlas Direct to Customer ANA Number
        /// </summary>
        public string AtlasDirectToCustomerANANumber { get; set; }

        /// <summary>
        /// 101.
        /// Atlas Store Name
        /// JLP Atlas Store Name
        /// </summary>
        public string AtlasStoreName { get; set; }

        /// <summary>
        /// Order Lines
        /// </summary>
        [JsonIgnore]
        public List<OrderLine> OrderLines { get; set; }

        /// <summary>
        /// Order Gift Boxes
        /// </summary>
        [JsonIgnore]
        public List<GiftBox> GiftBoxes { get; set; }

        public Order()
        {
            // New. For pulling order JSON in.
        }

        public Order(DataRow orderHead, DataTable orderLines, DataTable giftBoxes)
        {
            InvoiceTerms = orderHead["InvoiceTerms"]?.ToString();

            CustomerReference = orderHead["CustomerReference"].ToString();

            OrderNumber = orderHead["OrderNumber"]?.ToString();

            BasketID = $"{CustomerReference}~{OrderNumber}~{Guid.NewGuid()}";

            DeliveryReference = orderHead["DeliveryReference"]?.ToString();

            DeliveryAddress = orderHead["DeliveryAddress"]?.ToString();

            EmailAddress = orderHead["EmailAddress"]?.ToString();

            CustomerOrderReference = orderHead["CustomerOrderReference"]?.ToString();

            HaulierCode = orderHead["HaulierCode"]?.ToString();

            PriorityOrder = orderHead["PriorityOrder"]?.ToString();

            GiftWrap = orderHead["GiftWrap"]?.ToString();

            PartDeliveryAllowed = orderHead["PartDeliveryAllowed"]?.ToString();

            OrderValue = orderHead["OrderValue"]?.ToString();

            CarriageCode = orderHead["CarriageCode"]?.ToString();

            CarriageVATCode = orderHead["CarriageVATCode"]?.ToString();

            CarriageValue = orderHead["CarriageValue"]?.ToString();

            CarriageVATValue = orderHead["CarriageVATValue"]?.ToString();

            USTaxExemptionCertificate = orderHead["USTaxExemptionCertificate"]?.ToString();

            FromCSVFile = orderHead["FromCSVFile"]?.ToString();

            MassSampleEntryBasketIDs = orderHead["MassSampleEntryBasketIDs"]?.ToString();

            HoldForSales = orderHead["HoldForSales"]?.ToString();

            PromotionCode = orderHead["PromotionCode"]?.ToString();

            ContractOrDisplay = orderHead["ContractOrDisplay"]?.ToString();

            Urgent = orderHead["Urgent"]?.ToString();

            BespokeSampleType = orderHead["BespokeSampleType"]?.ToString();

            CustomerFullDispatch = orderHead["CustomerFullDispatch"]?.ToString();

            AutoAllocationPriority = orderHead["AutoAllocationPriority"]?.ToString();

            SendExpress = orderHead["SendExpress"]?.ToString();

            CustomerPaying = orderHead["CustomerPaying"]?.ToString();

            HaulierAccountNumber = orderHead["HaulierAccountNo"]?.ToString();

            TaxesDutiesToInvoiceAccount = orderHead["TaxesDutiesToInvoiceAccount"]?.ToString();

            RetailOrder = orderHead["RetailOrder"]?.ToString();

            ContactName = orderHead["ContactName"]?.ToString();

            ContactPhone = orderHead["ContactPhone"]?.ToString();

            DeliveryTerms = orderHead["DeliveryTerms"]?.ToString();

            DeliveryType = orderHead["DeliveryType"]?.ToString();

            OrderTaker = orderHead["OrderTaker"]?.ToString();

            TradeAccount = orderHead["TradeAccount"]?.ToString();

            LoyaltyCard = orderHead["LoyaltyCard"]?.ToString();

            DistributorCode = orderHead["DistributorCode"]?.ToString();

            TransferGLCode = orderHead["TransferGLCode"]?.ToString();

            ApplyMiscellaneousCharge = orderHead["ApplyMiscellaneousCharge"]?.ToString();

            ChargeCode = orderHead["ChargeCode"]?.ToString();

            MiscellaneousCharge = orderHead["MiscellaneousCharge"]?.ToString();

            PreferredContactMethod = orderHead["PreferredContactMethod"]?.ToString();

            ContactEmailFax = orderHead["ContactEmailFax"]?.ToString();

            FullDispatchOnly = orderHead["FullDispatchOnly"]?.ToString();

            HeldForDelivery = orderHead["HeldForDelivery"]?.ToString();

            ExpressReason = orderHead["ExpressReason"]?.ToString();

            UPSReturnsLabel = orderHead["UPSReturnsLabel"]?.ToString();

            CarriageExempt = orderHead["CarriageExempt"]?.ToString();

            HeldForExpress = orderHead["HeldForExpress"]?.ToString();

            CreditHeld = orderHead["CreditHeld"]?.ToString();

            FurnitureSystem = orderHead["FurnitureSystem"]?.ToString();

            EndCustomer = orderHead["EndCustomer"]?.ToString();

            WebHeld = orderHead["WebHeld"]?.ToString();

            FOCIndicator = orderHead["FOCIndicator"]?.ToString();

            FOCBudgetManager = orderHead["FOCBudgetManager"]?.ToString();

            OrderPromotionCode = orderHead["OrderPromotionCode"]?.ToString();

            OrderPromotionDiscount = orderHead["OrderPromotionDiscount"]?.ToString();

            HeaderReferences = orderHead["HeaderReferences"]?.ToString();

            SendOrderConfirmationEmail = orderHead["SendOrderConfirmationEmail"]?.ToString();

            MiscellaneousAccountAddress = orderHead["MiscellaneousAccountAddress"]?.ToString();

            RaisedAsResultOfClaim = orderHead["RaisedAsResultOfClaim"]?.ToString();

            EAD = orderHead["EAD"]?.ToString();

            InvoiceClauseCodes = orderHead["InvoiceClauseCodes"]?.ToString();

            InvoiceClauseText = orderHead["InvoiceClauseText"]?.ToString();

            InvoiceClauseComments = orderHead["InvoiceClauseComments"]?.ToString();

            TillNumber = orderHead["TillNumber"]?.ToString();

            SessionNumber = orderHead["SessionNumber"]?.ToString();

            PaymentType = orderHead["PaymentType"]?.ToString();

            CreditCardType = orderHead["CreditCardType"]?.ToString();

            PaymentAmount = orderHead["PaymentAmount"]?.ToString();

            InvoiceCommentCodes = orderHead["InvoiceCommentCodes"]?.ToString();

            InvoiceCommentText = orderHead["InvoiceCommentText"]?.ToString();

            PackingNoteCommentCodes = orderHead["PackingNoteCommentCodes"]?.ToString();

            PackingNoteCommentText = orderHead["PackingNoteCommentText"]?.ToString();

            DeliveryNoteCommentCodes = orderHead["DeliveryNoteCommentCodes"]?.ToString();

            DeliveryNoteCommentText = orderHead["DeliveryNoteCommentText"]?.ToString();

            ProformaCommentCodes = orderHead["ProformaCommentCodes"]?.ToString();

            ProformaCommentText = orderHead["ProformaCommentText"]?.ToString();

            ConfirmationCommentCodes = orderHead["ConfirmationCommentCodes"]?.ToString();

            ConfirmationCommentText = orderHead["ConfirmationCommentText"]?.ToString();

            PickCutListCommentCodes = orderHead["PickCutListCommentCodes"]?.ToString();

            PickCutListCommentText = orderHead["PickCutListCommentText"]?.ToString();

            HeldOrderReasonCode = orderHead["HeldOrderReasonCode"]?.ToString();

            RepCode = orderHead["SalesRepresentativeCode"]?.ToString();

            AgentCode = orderHead["SalesAgentCode"]?.ToString();

            ChargeOn = orderHead["ChargeOn"]?.ToString();

            BillDutyAccountNumber = orderHead["BillDutyAccountNumber"]?.ToString();

            HoldForSalesReasonCode = orderHead["HoldForSalesReasonCode"]?.ToString();

            OrderMemo = orderHead["OrderMemo"]?.ToString();

            CallType = orderHead["CallType"]?.ToString();

            OrderType = orderHead["OrderType"]?.ToString();

            SampleOrderOnly = orderHead["SampleOrderOnly"]?.ToString();

            UseInvoiceAddress = orderHead["UseInvoiceAddress"]?.ToString();

            RetailPaymentComments = orderHead["RetailPaymentComments"]?.ToString();

            RetailPaymentReferences = orderHead["RetailPaymentReferences"]?.ToString();

            B2BReadyOrCreateViaPortal = orderHead["AtlasDeliveryANANumber"]?.ToString();

            AtlasDeliveryANANumber = orderHead["AtlasDeliveryANANumber"]?.ToString();

            AtlasInvoiceeANANumber = orderHead["AtlasInvoiceeANANumber"]?.ToString();

            AtlasDirectToCustomerANANumber = orderHead["AtlasDirectToCustomerANANumber"]?.ToString();

            AtlasStoreName = orderHead["AtlasStoreName"]?.ToString();

            // Errant carriage returns cause mayhem.

            InvoiceTerms = InvoiceTerms.Replace(Environment.NewLine, "");

            CustomerReference = CustomerReference.Replace(Environment.NewLine, "");

            OrderNumber = OrderNumber.Replace(Environment.NewLine, "");

            BasketID = BasketID.Replace(Environment.NewLine, "");

            DeliveryReference = DeliveryReference.Replace(Environment.NewLine, "");

            DeliveryAddress = DeliveryAddress.Replace(Environment.NewLine, "");

            EmailAddress = EmailAddress.Replace(Environment.NewLine, "");

            CustomerOrderReference = CustomerOrderReference.Replace(Environment.NewLine, "");

            HaulierCode = HaulierCode.Replace(Environment.NewLine, "");

            PriorityOrder = PriorityOrder.Replace(Environment.NewLine, "");

            GiftWrap = GiftWrap.Replace(Environment.NewLine, "");

            PartDeliveryAllowed = PartDeliveryAllowed.Replace(Environment.NewLine, "");

            OrderValue = OrderValue.Replace(Environment.NewLine, "");

            CarriageCode = CarriageCode.Replace(Environment.NewLine, "");

            CarriageVATCode = CarriageVATCode.Replace(Environment.NewLine, "");

            CarriageValue = CarriageValue.Replace(Environment.NewLine, "");

            CarriageVATValue = CarriageVATValue.Replace(Environment.NewLine, "");

            USTaxExemptionCertificate = USTaxExemptionCertificate.Replace(Environment.NewLine, "");

            FromCSVFile = FromCSVFile.Replace(Environment.NewLine, "");

            MassSampleEntryBasketIDs = MassSampleEntryBasketIDs.Replace(Environment.NewLine, "");

            HoldForSales = HoldForSales.Replace(Environment.NewLine, "");

            PromotionCode = PromotionCode.Replace(Environment.NewLine, "");

            ContractOrDisplay = ContractOrDisplay.Replace(Environment.NewLine, "");

            Urgent = Urgent.Replace(Environment.NewLine, "");

            BespokeSampleType = BespokeSampleType.Replace(Environment.NewLine, "");

            CustomerFullDispatch = CustomerFullDispatch.Replace(Environment.NewLine, "");

            AutoAllocationPriority = AutoAllocationPriority.Replace(Environment.NewLine, "");

            SendExpress = SendExpress.Replace(Environment.NewLine, "");

            CustomerPaying = CustomerPaying.Replace(Environment.NewLine, "");

            HaulierAccountNumber = HaulierAccountNumber.Replace(Environment.NewLine, "");

            TaxesDutiesToInvoiceAccount = TaxesDutiesToInvoiceAccount.Replace(Environment.NewLine, "");

            RetailOrder = RetailOrder.Replace(Environment.NewLine, "");

            ContactName = ContactName.Replace(Environment.NewLine, "");

            ContactPhone = ContactPhone.Replace(Environment.NewLine, "");

            DeliveryTerms = DeliveryTerms.Replace(Environment.NewLine, "");

            DeliveryType = DeliveryType.Replace(Environment.NewLine, "");

            OrderTaker = OrderTaker.Replace(Environment.NewLine, "");

            TradeAccount = TradeAccount.Replace(Environment.NewLine, "");

            LoyaltyCard = LoyaltyCard.Replace(Environment.NewLine, "");

            DistributorCode = DistributorCode.Replace(Environment.NewLine, "");

            TransferGLCode = TransferGLCode.Replace(Environment.NewLine, "");

            ApplyMiscellaneousCharge = ApplyMiscellaneousCharge.Replace(Environment.NewLine, "");

            ChargeCode = ChargeCode.Replace(Environment.NewLine, "");

            MiscellaneousCharge = MiscellaneousCharge.Replace(Environment.NewLine, "");

            PreferredContactMethod = PreferredContactMethod.Replace(Environment.NewLine, "");

            ContactEmailFax = ContactEmailFax.Replace(Environment.NewLine, "");

            FullDispatchOnly = FullDispatchOnly.Replace(Environment.NewLine, "");

            HeldForDelivery = HeldForDelivery.Replace(Environment.NewLine, "");

            ExpressReason = ExpressReason.Replace(Environment.NewLine, "");

            UPSReturnsLabel = UPSReturnsLabel.Replace(Environment.NewLine, "");

            CarriageExempt = CarriageExempt.Replace(Environment.NewLine, "");

            HeldForExpress = HeldForExpress.Replace(Environment.NewLine, "");

            CreditHeld = CreditHeld.Replace(Environment.NewLine, "");

            FurnitureSystem = FurnitureSystem.Replace(Environment.NewLine, "");

            EndCustomer = EndCustomer.Replace(Environment.NewLine, "");

            WebHeld = WebHeld.Replace(Environment.NewLine, "");

            FOCIndicator = FOCIndicator.Replace(Environment.NewLine, "");

            FOCBudgetManager = FOCBudgetManager.Replace(Environment.NewLine, "");

            OrderPromotionCode = OrderPromotionCode.Replace(Environment.NewLine, "");

            OrderPromotionDiscount = OrderPromotionDiscount.Replace(Environment.NewLine, "");

            HeaderReferences = HeaderReferences.Replace(Environment.NewLine, "");

            SendOrderConfirmationEmail = SendOrderConfirmationEmail.Replace(Environment.NewLine, "");

            MiscellaneousAccountAddress = MiscellaneousAccountAddress.Replace(Environment.NewLine, "");

            RaisedAsResultOfClaim = RaisedAsResultOfClaim.Replace(Environment.NewLine, "");

            EAD = EAD.Replace(Environment.NewLine, "");

            InvoiceClauseCodes = InvoiceClauseCodes.Replace(Environment.NewLine, "");

            InvoiceClauseText = InvoiceClauseText.Replace(Environment.NewLine, "");

            InvoiceClauseComments = InvoiceClauseComments.Replace(Environment.NewLine, "");

            TillNumber = TillNumber.Replace(Environment.NewLine, "");

            SessionNumber = SessionNumber.Replace(Environment.NewLine, "");

            PaymentType = PaymentType.Replace(Environment.NewLine, "");

            CreditCardType = CreditCardType.Replace(Environment.NewLine, "");

            PaymentAmount = PaymentAmount.Replace(Environment.NewLine, "");

            InvoiceCommentCodes = InvoiceCommentCodes.Replace(Environment.NewLine, "");

            InvoiceCommentText = InvoiceCommentText.Replace(Environment.NewLine, "");

            PackingNoteCommentCodes = PackingNoteCommentCodes.Replace(Environment.NewLine, "");

            PackingNoteCommentText = PackingNoteCommentText.Replace(Environment.NewLine, "");

            DeliveryNoteCommentCodes = DeliveryNoteCommentCodes.Replace(Environment.NewLine, "");

            DeliveryNoteCommentText = DeliveryNoteCommentText.Replace(Environment.NewLine, "");

            ProformaCommentCodes = ProformaCommentCodes.Replace(Environment.NewLine, "");

            ProformaCommentText = ProformaCommentText.Replace(Environment.NewLine, "");

            ConfirmationCommentCodes = ConfirmationCommentCodes.Replace(Environment.NewLine, "");

            ConfirmationCommentText = ConfirmationCommentText.Replace(Environment.NewLine, "");

            PickCutListCommentCodes = PickCutListCommentCodes.Replace(Environment.NewLine, "");

            PickCutListCommentText = PickCutListCommentText.Replace(Environment.NewLine, "");

            HeldOrderReasonCode = HeldOrderReasonCode.Replace(Environment.NewLine, "");

            RepCode = RepCode.Replace(Environment.NewLine, "");

            AgentCode = AgentCode.Replace(Environment.NewLine, "");

            ChargeOn = ChargeOn.Replace(Environment.NewLine, "");

            BillDutyAccountNumber = BillDutyAccountNumber.Replace(Environment.NewLine, "");

            HoldForSalesReasonCode = HoldForSalesReasonCode.Replace(Environment.NewLine, "");

            OrderMemo = OrderMemo.Replace(Environment.NewLine, "");

            CallType = CallType.Replace(Environment.NewLine, "");

            OrderType = OrderType.Replace(Environment.NewLine, "");

            SampleOrderOnly = SampleOrderOnly.Replace(Environment.NewLine, "");

            UseInvoiceAddress = UseInvoiceAddress.Replace(Environment.NewLine, "");

            RetailPaymentComments = RetailPaymentComments.Replace(Environment.NewLine, "");

            RetailPaymentReferences = RetailPaymentReferences.Replace(Environment.NewLine, "");

            B2BReadyOrCreateViaPortal = B2BReadyOrCreateViaPortal.Replace(Environment.NewLine, "");

            AtlasDeliveryANANumber = AtlasDeliveryANANumber.Replace(Environment.NewLine, "");

            AtlasInvoiceeANANumber = AtlasInvoiceeANANumber.Replace(Environment.NewLine, "");

            AtlasDirectToCustomerANANumber = AtlasDirectToCustomerANANumber.Replace(Environment.NewLine, "");

            AtlasStoreName = AtlasStoreName.Replace(Environment.NewLine, "");

            // Urgh.

            OrderLines = new List<OrderLine>();

            GiftBoxes = new List<GiftBox>();

            foreach (DataRow row in orderLines.Rows)
            { 
                OrderLines.Add(new OrderLine(row));
            }
            
            for (var i = 0; i < giftBoxes.Rows.Count; i++)
            {
                GiftBoxes.Add(new GiftBox(i, giftBoxes.Rows[i]));
            }
        }

        public string DeliveryDetailsWebData()
        {
            return $@"{CustomerReference}
{DeliveryReference}
{DeliveryAddress}
{EmailAddress}
{CustomerOrderReference}
{HaulierCode}
{PriorityOrder}
{GiftWrap}
{PartDeliveryAllowed}
{OrderValue}
{CarriageCode}
{CarriageVATCode}
{CarriageValue}
{CarriageVATValue}
{OrderNumber}
{USTaxExemptionCertificate}
{FromCSVFile}
{MassSampleEntryBasketIDs}
{HoldForSales}
{PromotionCode}
{ContractOrDisplay}
{Urgent}
{BespokeSampleType}
{CustomerFullDispatch}
{AutoAllocationPriority}
{SendExpress}
{CustomerPaying}
{HaulierAccountNumber}
{TaxesDutiesToInvoiceAccount}
{RetailOrder}
{ContactName}
{ContactPhone}
{DeliveryTerms}
{DeliveryType}
{OrderTaker}
{TradeAccount}
{LoyaltyCard}
{DistributorCode}
{TransferGLCode}
{ApplyMiscellaneousCharge}
{ChargeCode}
{MiscellaneousCharge}
{PreferredContactMethod}
{ContactEmailFax}
{FullDispatchOnly}
{HeldForDelivery}
{ExpressReason}
{UPSReturnsLabel}
{CarriageExempt}
{HeldForExpress}
{CreditHeld}
{FurnitureSystem}
{EndCustomer}
{WebHeld}
{FOCIndicator}
{FOCBudgetManager}
{OrderPromotionCode}
{OrderPromotionDiscount}
{HeaderReferences}
{SendOrderConfirmationEmail}
{MiscellaneousAccountAddress}
{RaisedAsResultOfClaim}
{EAD}
{InvoiceClauseCodes}
{InvoiceClauseText}
{InvoiceClauseComments}
{TillNumber}
{SessionNumber}
{PaymentType}
{CreditCardType}
{PaymentAmount}
{InvoiceCommentCodes}
{InvoiceCommentText}
{PackingNoteCommentCodes}
{PackingNoteCommentText}
{DeliveryNoteCommentCodes}
{DeliveryNoteCommentText}
{ProformaCommentCodes}
{ProformaCommentText}
{ConfirmationCommentCodes}
{ConfirmationCommentText}
{PickCutListCommentCodes}
{PickCutListCommentText}
{HeldOrderReasonCode}
{RepCode}
{AgentCode}
{ChargeOn}
{BillDutyAccountNumber}
{HoldForSalesReasonCode}
{OrderMemo}
{CallType}
{OrderType}
{SampleOrderOnly}
{UseInvoiceAddress}
{RetailPaymentComments}
{RetailPaymentReferences}
{B2BReadyOrCreateViaPortal}
{AtlasDeliveryANANumber}
{AtlasInvoiceeANANumber}
{AtlasDirectToCustomerANANumber}
{AtlasStoreName}";
        }

        public string CreateOrderWebData()
        {
            return $@"{CustomerReference}
{InvoiceTerms}";
        }

        public override string ToString()
        {
            var result = $@"{Framework.WavyLineBreak}
Order Head
{Framework.WavyLineBreak}
{JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings() { ContractResolver = new JsonIgnoreAttributeIgnorerContractResolver() })}
{Framework.WavyLineBreak}";

            return result;
        }

        public string HeaderToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
