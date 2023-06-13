using MertexerV2.Models.General;
using MertexerV2.Models.Invoices;
using MertexerV2.Models.Pricing;
using MertexerV2.Models.Product;
using MertexerV2.TradeAccountFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models
{
    [UniVerseFileName("SLCUST")]
    public class TradeAccount : UniVerseSerializationBase<string>
    {
        [JsonRequired]
        public override string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }

        public string TradeAccountCode
        {
            get
            {
                return Key;
            }
            set
            {
                Key = value;
            }
        }

        [JsonConstructor]
        public TradeAccount(string key) : base(key)
        {
            Initialise();
        }

        public TradeAccount() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(TradeAccount).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            TradeAccountFunctions.TradeAccountFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Customer Name
        /// </summary>
        [UniVerseIndex(1, 1)]
        [UniVerseFieldName("CUSNAME")]
        public string Name { get; set; }

        /// <summary>
        /// Invoice Address Line 1
        /// </summary>
        [UniVerseIndex(1, 2)]
        [UniVerseFieldName("CUSADD1")]
        public string InvoiceAddressLine1 { get; set; }

        /// <summary>
        /// Invoice Address Line 2
        /// </summary>
        [UniVerseIndex(1, 3)]
        [UniVerseFieldName("CUSADD2")]
        public string InvoiceAddressLine2 { get; set; }

        /// <summary>
        /// Invoice Address Town
        /// </summary>
        [UniVerseIndex(1, 4)]
        [UniVerseFieldName("CUSADD3")]
        public string InvoiceAddressTown { get; set; }

        /// <summary>
        /// Invoice Address County
        /// </summary>
        [UniVerseIndex(1, 5)]
        [UniVerseFieldName("CUSADD4")]
        public string InvoiceAddressCounty { get; set; }

        /// <summary>
        /// Invoice Address Postcode
        /// </summary>
        [UniVerseIndex(1, 6)]
        [UniVerseFieldName("CUSPCODE")]
        public string InvoiceAddressPostcode { get; set; }

        /// <summary>
        /// Sort Name: no idea where this is used, nor indeed how useful or otherwise it is. (?)
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("SORTNAME.EXP")]
        public string SortName { get; set; }

        [JsonAlsoUpdates("CreatedDate")]
        public DateTime? CreatedDT { get; set; }

        /// <summary>
        /// Account Creation Date (UniVerse int offset from 31/12/1967 format)
        /// </summary>
        [UniVerseIndex(4, 1)]
        [UniVerseFieldName("ONFILE")]
        [JsonIgnore]
        public string CreatedDate
        {
            get
            {
                if (CreatedDT.HasValue)
                {
                    return Framework.UniVerseDateString(CreatedDT.Value);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (Int32.TryParse(value, out int valInt))
                    {
                        CreatedDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        CreatedDT = null;
                    }
                }
            }
        }

        [JsonAlsoUpdates("ModifiedDate")]
        public DateTime? ModifiedDT { get; set; }

        /// <summary>
        /// Date Last Amended
        /// </summary>
        [UniVerseIndex(4, 2)]
        [UniVerseFieldName("AMENDED")]
        [JsonIgnore]
        public string ModifiedDate
        {
            get
            {
                if (ModifiedDT.HasValue)
                {
                    return Framework.UniVerseDateString(ModifiedDT.Value);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (Int32.TryParse(value, out int valInt))
                    {
                        ModifiedDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        ModifiedDT = null;
                    }
                }
            }
        }

        /// <summary>
        /// Memorandum
        /// </summary>
        [UniVerseIndex(5)]
        [UniVerseMultiValue]
        [UniVerseFieldName("CONTACT")]
        public IEnumerable<string> Contacts { get; set; }

        /// <summary>
        /// Contact Type
        /// </summary>
        [UniVerseIndex(6)]
        [UniVerseMultiValue] // SLPOS
        [UniVerseFieldName("POSITION")]
        [JsonIgnore]
        public IEnumerable<string> ContactTypes { get; set; }

        /// <summary>
        /// Memorandum
        /// </summary>
        [UniVerseIndex(7)]
        [UniVerseMultiValue]
        [UniVerseFieldName("PHONEX")]
        [JsonIgnore]
        public IEnumerable<string> ContactPhones { get; set; }

        /// <summary>
        /// Memorandum
        /// </summary>
        [UniVerseIndex(8)]
        [UniVerseFieldName("MEMO")]
        public string Memo { get; set; }

        /// <summary>
        /// Customer Type: required (?), must exist on SLTYPE
        /// </summary>
        [UniVerseIndex(9)]
        [UniVerseFieldName("CUSTYPE")]
        public string TradeAccountType { get; set; }

        /// <summary>
        /// Analysis Code (D) - whatever that is (?)
        /// Optional; does not validate
        /// Defaults to B("DGLCODE,D.SALE") which is actually a call to a Default definition... which does some godawful mad thing.
        /// FORTUNATELY, this field is completely empty throughout the entirety of SLCUST.
        /// </summary>
        [UniVerseIndex(10)]
        [UniVerseFieldName("CUSALYS")]
        public string AnalysisCodeD { get; set; }

        /// <summary>
        /// Sales Representative Code: required, must exist on SLSEL1
        /// </summary>
        [UniVerseIndex(11)]
        [UniVerseFieldName("SEL1")]
        public string SalesRepresentativeCode { get; set; }

        /// <summary>
        /// Sales Agent Code: optional, must exist on SLSEL2 if present
        /// </summary>
        [UniVerseIndex(12)]
        [UniVerseFieldName("SEL2")]
        public string SalesAgentCode { get; set; }

        /// <summary>
        /// Sales Area: required (?), must exist on SLSEL3
        /// </summary>
        [UniVerseIndex(13)]
        [UniVerseFieldName("SEL3")]
        public string SalesArea { get; set; }

        /// <summary>
        /// Account State: 'O'pen or 'C'losed
        /// </summary>
        [UniVerseIndex(14)]
        [UniVerseFieldName("OPEN")]
        public string Status { get; set; }

        /// <summary>
        /// Credit Limit
        /// </summary>
        [UniVerseIndex(16)]
        [UniVerseFieldName("CRLIMIT")]
        public int? CreditLimit { get; set; }

        /// <summary>
        /// Credit Stopped? optional (?) defaults to 'N', should probably be either Y or N but is not validated
        /// </summary>
        [UniVerseIndex(17)]
        [UniVerseFieldName("CRSTOP")]
        public bool CreditStopped { get; set; }

        /// <summary>
        //  Payment Terms: optional; must exist on SLTERMS
        /// </summary>
        [UniVerseIndex(20)]
        [UniVerseFieldName("STTERMS")]
        public string PaymentTerms { get; set; }

        /// <summary>
        /// Statement Name (?)
        /// </summary>
        [UniVerseIndex(29, 1)]
        [UniVerseFieldName("STNAME")]
        public string StatementAccountName { get; set; }

        /// <summary>
        /// Statement Address Line 1 (?)
        /// </summary>
        [UniVerseIndex(29, 2)]
        [UniVerseFieldName("STADD1")]
        public string StatementAddressLine1 { get; set; }

        /// <summary>
        /// Statement Address Line 2 (?)
        /// </summary>
        [UniVerseIndex(29, 3)]
        [UniVerseFieldName("STADD2")]
        public string StatementAddressLine2 { get; set; }

        /// <summary>
        /// Statement Address Town (?)
        /// </summary>
        [UniVerseIndex(29, 4)]
        [UniVerseFieldName("STADD3")]
        public string StatementAddressTown { get; set; }

        /// <summary>
        /// Statement Address County (?)
        /// </summary>
        [UniVerseIndex(29, 5)]
        [UniVerseFieldName("STADD4")]
        public string StatementAddressCounty { get; set; }

        /// <summary>
        /// Statement Address Postcode (?)
        /// </summary>
        [UniVerseIndex(29, 6)]
        [UniVerseFieldName("STPCODE")]
        public string StatementAddressPostcode { get; set; }

        /// <summary>
        /// Analysis Code (S) - whatever that is (?)
        /// Optional; does not validate
        /// Refers to GLSUB 
        /// </summary>
        [UniVerseIndex(32)]
        [UniVerseFieldName("CUSSUB")]
        public string AnalysisCodeS { get; set; }

        /// <summary>
        /// Selling Price Reference: optional, must be a recognised Selling Price Number or Reference
        /// from... from WHERE?! OEDEFN... SELLPRICES.STD... horrible mess..
        /// This is PRICE LIST, but where am I supposed to fetch a valid list of Price List codes?
        /// 
        /// That... is doing something very funky. It's pulling columns out of OEDEFN from the record with key "SELLPRICES.STD".
        /// It's then projecting those together into a new table. I can see for example that column 003 contains a SV mark delimited list of PriceLists.
        /// 
        /// </summary>
        [UniVerseIndex(36, 1)]
        [UniVerseFieldName("SELLPRICE")]
        public string PriceList { get; set; }

        /// <summary>
        /// Currency: required, must exist on XRATE, cannot be modified if there are transactions posted 
        /// see [SL].[PG.VCURRENCY]
        /// see [SL].[PG.VAL.BANK]
        /// </summary>
        [UniVerseIndex(42)]
        [UniVerseFieldName("CURRENCY")]
        public string Currency { get; set; }

        /// <summary>
        /// Default Delivery Reference
        /// Optional; must exist on DELADD if given
        /// </summary>
        [UniVerseIndex(53)]
        [UniVerseFieldName("DEF.DELREF")]
        public string DefaultDeliveryReference { get; set; }

        /// <summary>
        /// "Normal VAT Rate": optional - if entered, must be a valid VAT Code from CFVAT
        /// </summary>
        [UniVerseIndex(54)]
        [UniVerseFieldName("VATNORM")]
        public string VATNormalRateCode { get; set; }

        [JsonAlsoUpdates("CreditLimitExpiryDate")]
        public DateTime? CreditLimitExpiryDT { get; set; }

        /// <summary>
        /// Credit Limit Expiry Date
        /// </summary>
        [UniVerseIndex(56)]
        [UniVerseFieldName("CRLIMIT.EXP")]
        [JsonIgnore]
        public string CreditLimitExpiryDate
        {
            get
            {
                if (CreditLimitExpiryDT.HasValue)
                    return Framework.UniVerseDateString(CreditLimitExpiryDT.Value);
                else
                    return null;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (Int32.TryParse(value, out int valInt))
                    {
                        CreditLimitExpiryDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        CreditLimitExpiryDT = null;
                    }
                }
            }
        }

        /// <summary>
        /// Min Order Val Warning
        /// Optional, does not validate
        /// Defaults: 500
        /// </summary>
        [UniVerseIndex(59)]
        [UniVerseConversion("MR22")]
        [UniVerseFieldName("MINORDVAL.SL")]
        public decimal? MinimumOrderValue { get; set; }

        /// <summary>
        /// Accept Sundries (?) what the hell does that mean?
        /// Can people refuse sundries? 
        /// Optional, if entered must be either 'Y' or 'N'
        /// </summary>
        [UniVerseIndex(68)]
        [UniVerseFieldName("SUNDRY.ACCPT")]
        public bool AcceptSundries { get; set; }

        /// <summary>
        /// Memorandum
        /// Optional; not validated. Multivalue.
        /// </summary>
        [UniVerseIndex(70)]
        [UniVerseFieldName("EXTENDEDMEMO.NV")]
        public IEnumerable<string> ExtendedMemos { get; set; }

        /// <summary>
        /// Bank Code (?) optional; see [SL].[PG.VAL.BANK]
        /// Must exist on BANK
        /// Like currency, this shouldn't be modified after creation (unless there happen to be no transactions or balances)
        /// Seems as though the currency can be automatically modified upon changing Bank Code to match the currency on the entered Bank record.
        /// SEE PG.VAL.BANK
        /// </summary>
        [UniVerseIndex(75)]
        [UniVerseFieldName("BANKNO")]
        public string BankCode { get; set; }

        /// <summary>
        /// Invoice Address Country
        /// </summary>
        [UniVerseIndex(77)]
        [UniVerseFieldName("COUNTRY")]
        public string InvoiceAddressCountryCode { get; set; }

        /// <summary>
        /// Invoice Copies
        /// Optional; validates using [SL].[PG.VAL.PRINTNO]
        /// Default: 1
        /// </summary>
        [UniVerseIndex(81)]
        [UniVerseFieldName("INV.COPIES")]
        public byte? InvoiceCopiesCount { get; set; }

        /// <summary>
        /// Selling Company (?)
        /// Optional; validates: must exist in SLSELLCO.TITLE
        /// </summary>
        [UniVerseIndex(82, 1)]
        [UniVerseFieldName("SELLCO")]
        public string SellingCompany { get; set; }

        /// <summary>
        /// Price Code
        /// No idea what this does... what about SELLPRICE?
        /// There is only ONE account with any value in here, and that's W1202 with an 'N'.
        /// </summary>
        [UniVerseIndex(87)]
        [UniVerseFieldName("PRICECODE")]
        public string PriceCode { get; set; }

        /// <summary>
        /// Analysis Code (A) - whatever that is (?)
        /// Optional; does not validate
        /// Defaults to B("DGLCODE,A.SALE") which is actually a call to a Default definition... which does some godawful mad thing.
        /// FORTUNATELY, this field is completely empty throughout the entirety of SLCUST.
        /// </summary>
        [UniVerseIndex(88)]
        [UniVerseFieldName("CUSACC1")]
        public string AnalysisCodeA { get; set; }

        /// <summary>
        /// Export (?)
        /// Optional; must be either 'Y' or 'N' if given. 
        /// Defaults to: see [SL].[PG.DEF.EXPORTFLAG.DES] - checks if <77> # the Company file <251>, then 'Y' else 'N'. 
        /// </summary>
        [UniVerseIndex(89)]
        [UniVerseFieldName("EXPORT.DES")]
        public bool Export { get; set; }

        /// <summary>
        /// Direct Delivery Account (?)
        /// Optional, 'Y', 'N' or '' if given.
        /// Defaults: ''
        /// </summary>
        [UniVerseIndex(90)]
        [UniVerseFieldName("DIR.DEL")]
        public bool DirectDelivery { get; set; }

        /// <summary>
        /// Overdue Invoice Period of Grace
        /// Optional; not validated. Presumably some number of days.
        /// </summary>
        [UniVerseIndex(95)]
        [UniVerseFieldName("INV.ODUE.POG")]
        public byte? OverdueInvoiceGracePeriod { get; set; }

        /// <summary>
        /// Value Added Tax Registration Number
        /// </summary>
        [UniVerseIndex(96)]
        [UniVerseFieldName("VATREG.DES")]
        public string VATNumber { get; set; }

        /// <summary>
        /// Statement Account (Refers to some other trade account e.g. Settes)
        /// </summary>
        [UniVerseIndex(98)]
        [UniVerseFieldName("STATEMENT.AC")]
        public string StatementTradeAccountCode { get; set; }

        /// <summary>
        /// Contract Pricing
        /// Optional; 'Y', 'N' or '' if given
        /// Defaults: 'Y'
        /// </summary>
        [UniVerseIndex(100)]
        [UniVerseFieldName("CONT.PRICING")]
        public bool ContractPricing { get; set; }

        /// <summary>
        /// Market: required, must be a value from CLMARKTYPE
        /// </summary>
        [UniVerseIndex(101, 1)]
        [UniVerseFieldName("MARKTYPE.DES")]
        public string Market { get; set; }

        /// <summary>
        /// Economic Operators Registration and Identification Number: optional
        /// </summary>
        [UniVerseIndex(106)]
        [UniVerseFieldName("EORI.NUMBER")]
        public string EORINumber { get; set; }

        /// <summary>
        /// Invoice Accounts (End Customers)
        /// </summary>
        [UniVerseIndex(107)]
        [UniVerseMultiValue]
        [UniVerseFieldName("INV.ACCOUNTS")]
        public IEnumerable<string> InvoiceAccounts { get; set; }

        /// <summary>
        /// Minimum Order Quantity (?)
        /// Optional; validates: if given, must be a positive number
        /// Defaults: IF(F("OEPARMS","1")<229> = "", ""[S],<118>)[M] -- whatever that means
        /// </summary>
        [UniVerseIndex(118)]
        [UniVerseFieldName("MINORDQTY.SL")]
        public byte? MinimumOrderQuantity { get; set; }

        /// <summary>
        /// Delivery Change Reconfirmation (?)
        /// Is this some sort of double-prompt for when a user changes the delivery address on an order belonging to this customer?
        /// Optional; 'Y' or 'N' if given.
        /// Defaults: 'N'
        /// </summary>
        [UniVerseIndex(133)]
        [UniVerseFieldName("DELCHANGE")]
        public bool DeliveryChangeReconfirmation { get; set; }

        /// <summary>
        /// Delivery Date Calculation (?)
        /// Optional, validates: [OE].[PG.VDELDATE.DEF]
        ///                      'W',   Default Week Commencing Date
        ///                      'T',   Default Today
        ///                      'X',   No Default
        ///                      'D',   Default Date from Previous Line
        ///                      'DW',  Default from Previous Line - Week Commencing on Line 1
        ///                      'DT'   Default from Previous Line - Today on Line 1
        /// </summary>
        [UniVerseIndex(140)]
        [UniVerseFieldName("DELDATE.SL")]
        public string DeliveryDateCalculation { get; set; }

        /// <summary>
        /// Cut Verify (?)
        /// No idea what this does
        /// Optional; '', 0, 1, 2, 3
        /// </summary>
        [UniVerseIndex(159)]
        [UniVerseFieldName("CUT.VERIFY")]
        public byte? CutVerify { get; set; }

        /// <summary>
        /// Standard Roll Length (?) 
        /// What the hell is this?! How can a customer have a standard roll length? What does this MEAN?!?
        /// Optional; validates non-negative
        /// </summary>
        [UniVerseIndex(160)]
        [UniVerseConversion("MR13")]
        [UniVerseFieldName("STD.ROLLEN")]
        public decimal? StandardRollLength { get; set; }

        /// <summary>
        /// Carriage Surcharge Exempt
        /// Optional; Y,N,E,D,F,1 (?)
        /// Default: 'F'
        /// </summary>
        [UniVerseIndex(161)]
        [UniVerseFieldName("CARR.EXEM.SL")]
        public string CarriageSurchargeExempt { get; set; }

        /// <summary>
        /// Order Confirmation Print (?)
        /// Optional; Validates: see [SL].[PG.AFTFAX] "After Fax Indicator".
        ///                      Y,N,F,E,B,C,A
        /// </summary>
        [UniVerseIndex(162)]
        [UniVerseFieldName("CONF.REQ.SL")]
        public string OrderConfirmationPrint { get; set; }

        /// <summary>
        /// Proforma Trading (?) Why this as well as STTERMS...?
        /// Optional; 'Y' or 'N' if given
        /// </summary>
        [UniVerseIndex(163)]
        [UniVerseFieldName("PF.TRADING")]
        public bool ProformaTrading { get; set; }

        /// <summary>
        /// Proforma Surcharge (?)
        /// Optional; does not validate
        /// </summary>
        [UniVerseIndex(164)]
        [UniVerseFieldName("PF.SURCHARGE")]
        public decimal? ProformaSurcharge { get; set; }

        /// <summary>
        /// Insured Credit Limit - optional (?)
        /// </summary>
        [UniVerseIndex(165)]
        [UniVerseFieldName("INS.CRLIM.SL.DES")]
        public int? InsuredCreditLimit { get; set; }

        [JsonAlsoUpdates("InsuredCreditLimitExpiryDate")]
        public DateTime? InsuredCreditLimitExpiryDT { get; set; }

        /// <summary>
        /// Insured Credit Limit Expiry Date
        /// </summary>
        [UniVerseIndex(166)]
        [UniVerseFieldName("INS.EXPDT.SL.DES")]
        [JsonIgnore]
        public string InsuredCreditLimitExpiryDate
        {
            get
            {
                if (InsuredCreditLimitExpiryDT.HasValue)
                    return Framework.UniVerseDateString(InsuredCreditLimitExpiryDT.Value);
                else
                    return null;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (Int32.TryParse(value, out int valInt))
                    {
                        InsuredCreditLimitExpiryDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        InsuredCreditLimitExpiryDT = null;
                    }
                }
            }
        }

        /// <summary>
        /// Invoice Detail Print (?)
        /// Optional; 'T' or 'P' if given.
        /// </summary>
        [UniVerseIndex(167)]
        [UniVerseFieldName("INV.DETAILS")]
        public string InvoiceDetailPrint { get; set; }

        /// <summary>
        /// Packing List (Copies) (?)
        /// Optional: validates, see [SL].[PG.VAL.PRINTNO] "Validation max no. of Printing"
        /// This appears to be a count: how many additional copies of the packing instructions should be printed?
        /// but, as with so many other things in Mertex, the question is: WHY DOES THIS EVEN NEED TO EXIST
        /// </summary>
        [UniVerseIndex(198)]
        [UniVerseFieldName("PACKLIST.SL")]
        public byte? PackingListCopiesCount { get; set; }

        /// <summary>
        /// Check for Duplicates
        /// Optional, 'Y' or '' if given
        /// </summary>
        [UniVerseIndex(206)]
        [UniVerseFieldName("CHKDUPLICATE")]
        public bool CheckForDuplicates { get; set; }

        /// <summary>
        /// Delivery Terms
        /// Optional; not validated
        /// Sarah tells me this links to STPRODCLASS8
        /// </summary>
        [UniVerseIndex(207)]
        [UniVerseFieldName("DELTERMS")]
        public string DeliveryTerms { get; set; }

        /// <summary>
        /// Redundant Town Field (Used for searching in Mertex)
        /// </summary>
        [UniVerseIndex(223)]
        [UniVerseFieldName("TOWN")]
        public string Town { get; set; }

        /// <summary>
        /// Redundant County field (Used for searching in Mertex)
        /// </summary>
        [UniVerseIndex(224)]
        [UniVerseFieldName("COUNTY")]
        public string County { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [UniVerseIndex(225)]
        [UniVerseFieldName("EMAIL")]
        public string Email { get; set; }

        /// <summary>
        /// Commercial Invoice (?)
        /// Optional; must be either 'Y' or 'N' if given
        /// </summary>
        [UniVerseIndex(227)]
        [UniVerseFieldName("COMM.INV.DES")]
        public bool CommercialInvoice { get; set; }

        /// <summary>
        /// [Send] Overdue Letters?
        /// Mandatory: must be either 'Y' or 'N'
        /// </summary>
        [UniVerseIndex(236)]
        [UniVerseFieldName("OVERDUE.LETTERS")]
        public bool SendOverdueLetters { get; set; }

        /// <summary>
        /// Overdue Letter 1 (?)
        /// Optional? Vaidates: must be less than the entry for Overdue Letter 2
        /// 
        /// There are 5 of these. I assume they're some integer count for "How long until we send the Nth overdue letter".
        /// 
        /// </summary>
        [UniVerseIndex(238, 1)]
        [UniVerseFieldName("OD.LETTER1")]
        public byte? OverdueLetter1Delay { get; set; }

        /// <summary>
        /// Overdue Letter 2
        /// Optional: must be > Overdue Letter 1 and < Overdue Letter 3
        /// </summary>
        [UniVerseIndex(238, 2)]
        [UniVerseFieldName("OD.LETTER2")]
        public byte? OverdueLetter2Delay { get; set; }

        /// <summary>
        /// Overdue Letter 3
        /// Optional: must be > Overdue Letter 2 and < Overdue Letter 4
        /// </summary>
        [UniVerseIndex(238, 3)]
        [UniVerseFieldName("OD.LETTER3")]
        public byte? OverdueLetter3Delay { get; set; }

        /// <summary>
        /// Overdue Letter 4
        /// Optional: must be > Overdue Letter 3 and < Overdue Letter 5
        /// </summary>
        [UniVerseIndex(238, 4)]
        [UniVerseFieldName("OD.LETTER4")]
        public byte? OverdueLetter4Delay { get; set; }

        /// <summary>
        /// Overdue Letter 5
        /// Optional: must be > Overdue Letter 4
        /// </summary>
        [UniVerseIndex(238, 5)]
        [UniVerseFieldName("OD.LETTER4")]
        public byte? OverdueLetter5Delay { get; set; }

        /// <summary>
        /// Stockist? (?)
        /// No idea what this does
        /// Optional; does not validate
        /// Defaults: IF (SEL1 = "",""[S],IF(REP.STOCKIST # "", REP.STOCKIST,"N"))[M]
        /// </summary>
        [UniVerseIndex(255)]
        [UniVerseFieldName("REP.STOCKIST")]
        public bool Stockist { get; set; }

        /// <summary>
        /// Partner Company
        /// Optional (?) reads COMPANY "1" using <153>... this is just 'What other UniVerse account... something something.'
        /// </summary>
        [UniVerseIndex(273)]
        [UniVerseFieldName("PARTNER.CO")]
        public string PartnerCompany { get; set; }

        /// <summary>
        /// Usual Haulier: defaults to SLCOUNTRY<3> using COUNTRY.
        /// Optional (?): must exist on OEHAULIER
        /// </summary>
        [UniVerseIndex(275)]
        [UniVerseFieldName("HAULIER")]
        public string HaulierCode { get; set; }

        /// <summary>
        /// Normal Service: defaults to SLCOUNTRY<4> using COUNTRY
        /// Optional (?): see [OE].[PG.VAL.SLCARRIAGE]
        /// </summary>
        [UniVerseIndex(276)]
        [UniVerseFieldName("SERVICE")]
        public string CarriageCode { get; set; }

        /// <summary>
        /// Contract / Display (?)
        /// Why does this live on SLCUST...?
        /// Optional; validates 'D'isplay, 'C'ontract, 'N'ormal
        /// </summary>
        [UniVerseIndex(287)]
        [UniVerseFieldName("CONTRACT.ORDER")]
        public string ContractOrDisplay { get; set; }

        /// <summary>
        /// Sample Stock Location
        /// Optional; must exist on STLOCATION if given
        /// </summary>
        [UniVerseIndex(302)]
        [UniVerseFieldName("SAMP.LOC")]
        public string SampleStockLocation { get; set; }

        /// <summary>
        /// Sample Haulier Code
        /// Optional (?): must exist on OEHAULIER
        /// </summary>
        [UniVerseIndex(303)]
        [UniVerseFieldName("SAMP.HAULIER")]
        public string SampleHaulierCode { get; set; }

        /// <summary>
        /// Sample Carriage Code
        /// Optional (?): must exist on OECARRIAGE
        /// </summary>
        [UniVerseIndex(304)]
        [UniVerseFieldName("SAMP.SERVICE")]
        public string SampleCarriageCode { get; set; }

        /// <summary>
        /// Sample Carriage Exempt
        /// </summary>
        [UniVerseIndex(305)]
        [UniVerseFieldName("SAMP.CARR.EX")]
        public string SampleCarriageExempt { get; set; }

        /// <summary>
        /// Normal Order Location (?)
        /// Optional; must exist on STLOCATION if given
        /// </summary>
        [UniVerseIndex(307)]
        [UniVerseFieldName("ORDER.LOC")]
        public string DefaultStockLocation { get; set; }

        /// <summary>
        /// Product Screen (?)
        /// The hell?
        /// </summary>
        [UniVerseIndex(317)]
        [UniVerseFieldName("PROD.SCREEN")]
        public bool ProductScreen { get; set; }

        /// <summary>
        /// Payment Details [Type...?] (?)
        /// Optional; validates - see [SL].[PG.VCCARD.DETAIL]
        ///                                 '0', '1', '2'
        ///                         uses <318>...
        /// Defaults: '0'
        /// </summary>
        [UniVerseIndex(318)]
        [UniVerseFieldName("CCARD.DETAILS")]
        public string CreditCardDetails { get; set; }

        /// <summary>
        /// Statement Print (?)
        /// Optional; 'Y' or 'N' if given
        /// Defaults: 'Y'
        /// </summary>
        [UniVerseIndex(319)]
        [UniVerseFieldName("STAT.PRINT")]
        public bool StatementPrint { get; set; }

        /// <summary>
        /// Default Order Priority
        /// Mandatory; validates {0:9}
        /// Defaults: 1
        /// </summary>
        [UniVerseIndex(320)]
        [UniVerseFieldName("DEF.ORDER.PRIORITY")]
        public byte? DefaultOrderPriority { get; set; }

        /// <summary>
        /// UPS Taxes / Duty Account Number
        /// optional; see [OE].[PG.VAL.DUTY.ACCOUNT] ?!
        /// That's a very weird validation: if non-null, and less than 6 characters,
        /// then display:  "WARNING: This field should contain the account number to be charged any duty / taxes for the delivery. You have entered: "{value}". 
        /// Is this correct? Y / N"
        /// </summary>
        [UniVerseIndex(322)]
        [UniVerseFieldName("DUTY.ACC.NO")]
        public string UPSTaxesDutyAccountNumber { get; set; }

        /// <summary>
        /// Out of Stock Letter (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(354)]
        [UniVerseFieldName("OOS.LETTER")]
        public bool OutOfStockLetter { get; set; }

        /// <summary>
        /// Minimum Carriage Value Surcharge (?)
        /// Optional; 'Y' or 'N' if given
        /// Defaults: 'N'
        /// </summary>
        [UniVerseIndex(376)]
        [UniVerseFieldName("MIN.VALUE.SUR")]
        public bool ApplyMinimumCarriageValueSurcharge { get; set; }

        /// <summary>
        /// Internal or External
        /// Optional; 'I' or 'E' if given [for Internal, External]
        /// Defaults: 'E'
        /// </summary>
        [UniVerseIndex(381)]
        [UniVerseFieldName("INT.EXT")]
        public string InternalOrExternal { get; set; }

        /// <summary>
        /// Memorandum
        /// </summary>
        [UniVerseIndex(392)]
        [UniVerseMultiValue]
        [UniVerseFieldName("CONTACT.EMAIL")]
        [JsonIgnore]
        public IEnumerable<string> ContactEmails { get; set; }

        /// <summary>
        /// Main Telephone Number
        /// </summary>
        [UniVerseIndex(393)]
        [UniVerseFieldName("STD.TELEPHONE")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Website Address
        /// </summary>
        [UniVerseIndex(394)]
        [UniVerseFieldName("WEB.SITE")]
        public string Website { get; set; }

        /// <summary>
        /// Company Registration Number: optional
        /// </summary>
        [UniVerseIndex(396)]
        [UniVerseFieldName("COMPANY.REGNO")]
        public string CompanyRegistrationNumber { get; set; }

        /// <summary>
        /// Statement Telephone Number (?)
        /// </summary>
        [UniVerseIndex(398)]
        [UniVerseFieldName("STPHONE")]
        public string StatementPhoneNumber { get; set; }

        /// <summary>
        /// Statement Contact Name (?)
        /// </summary>
        [UniVerseIndex(401)]
        [UniVerseFieldName("STCONTACT")]
        public string StatementContactName { get; set; }

        /// <summary>
        /// Classification Code: optional, from SLCUSCLASS. Seems to be regional groupings.
        /// </summary>
        [UniVerseIndex(410)]
        [UniVerseFieldName("CLASS.CODE")]
        public string TradeAccountClass { get; set; }

        /// <summary>
        /// File Tranfer Protocol (?)
        /// No idea what this does
        /// Optional; validates 'F','D','E','M'
        /// </summary>
        [UniVerseIndex(424)]
        [UniVerseFieldName("FILE.PROTOCOL")]
        public string FileProtocol { get; set; }

        /// <summary>
        /// Transfer Address (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(425)]
        [UniVerseFieldName("TRAN.ADDR")]
        public string TransferAddress { get; set; }

        /// <summary>
        /// User ID (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(426)]
        [UniVerseFieldName("TRAN.USER")]
        public string UserID { get; set; }

        /// <summary>
        /// Transfer Password (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(427)]
        [UniVerseFieldName("TRAN.PWD")]
        public string TransferPassword { get; set; }

        /// <summary>
        /// Electronic Data Interchange Invoice 
        /// Required; Defaults 'N'. Must be 'Y', 'N' or 'P'. There are no records where the value is 'P'.
        /// </summary>
        [UniVerseIndex(441)]
        [UniVerseFieldName("EDI.INVOICE")]
        public bool EDIInvoice { get; set; }

        /// <summary>
        /// Direct to Customer ANA (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(443)]
        [UniVerseFieldName("EDI.DIRECT.CUST")]
        public string DirectToCustomer { get; set; }

        /// <summary>
        /// Allow entry of Till Credit Notes (?)
        /// No idea what this does
        /// Optional; 'Y' or 'N' if given
        /// </summary>
        [UniVerseIndex(445)]
        [UniVerseFieldName("TILL.CREDITS")]
        public bool PermitTillCreditNotes { get; set; }

        /// <summary>
        /// Inform on Promised Date Changed
        /// Mandatory (?) validates: 'N'o, 'P'rint, 'F'ax, 'E'mail
        /// Defaults: 'E'
        /// </summary>
        [UniVerseIndex(516)]
        [UniVerseFieldName("INFORM.DATE.DES")]
        public string InformOnPromiseDateChanged { get; set; }

        /// <summary>
        /// Print Unit Price on Delivery Note?
        /// Mandatory; validates 'Y', 'N'
        /// Defaults: 'N'
        /// </summary>
        [UniVerseIndex(540)]
        [UniVerseFieldName("DELNOTE.PRICES")]
        public bool PrintUnitPriceOnDeliveryNote { get; set; }

        /// <summary>
        /// Code Group - This is a logical grouping of Trade Accounts. If provided, must be a value from OECODEGROUPS.
        /// </summary>
        [UniVerseIndex(541)]
        [UniVerseFieldName("CODE.GROUP")]
        public string TradeAccountGroup { get; set; }

        /// <summary>
        /// ID Code (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(542)]
        [UniVerseFieldName("TILLID.CODE")]
        public string TillIDCode { get; set; }

        /// <summary>
        /// Usual Order Unit (?)
        /// Optional, validates against STUNITS.CODE if given
        /// </summary>
        [UniVerseIndex(543)]
        [UniVerseFieldName("USUAL.ORD.UNIT")]
        public string UsualOrderUnit { get; set; }

        /// <summary>
        /// Send Invoice to Delivery Address
        /// Optional; validates [OE].[PG.INV.TO.DELADD.VAL]
        ///                     'Y', 'N'
        ///                     Shows warning if set to 'Y': INVOICES WILL BE SENT TO DELIVERY ADDRESS
        /// Defaults: 'N'
        /// </summary>
        [UniVerseIndex(544)]
        [UniVerseFieldName("INV.TO.DELIVERY")]
        public bool SendInvoiceToDeliveryAddress { get; set; }

        /// <summary>
        /// Full Order Only (?)
        /// Optional; 'Y' if given
        /// Defaults: ''
        /// </summary>
        [UniVerseIndex(555)]
        [UniVerseFieldName("SO.ALLOC.FULL")]
        public bool FullOrderOnly { get; set; }

        /// <summary>
        /// Distributor Reference (Code)
        /// Optional; validates: must exist on OEDISTRIBUTOR if given
        /// </summary>
        [UniVerseIndex(568)]
        [UniVerseFieldName("DISTREF")]
        public string DistributorReference { get; set; }

        /// <summary>
        /// Measure Pieces on Despatch
        /// Optional; 'Y', 'N' if given
        /// Defaults: IF(F("OEPARMS","1")<477> = "N", ""[S],<575>)[M]
        /// </summary>
        [UniVerseIndex(575)]
        [UniVerseFieldName("MEASURE.PIECE")]
        public bool MeasurePiece { get; set; }

        /// <summary>
        /// Division: required (?) this is groupage: G/N/T/W/Y - Value must exist on SLDIVISIONS
        /// </summary>
        [UniVerseIndex(576)]
        [UniVerseFieldName("DIVISION.CODE")]
        public string SalesDivision { get; set; }

        /// <summary>
        /// Transfer Account
        /// Optional; 'Y', 'N' or '' if given
        /// Default: ''
        /// </summary>
        [UniVerseIndex(587)]
        [UniVerseFieldName("TRANS.ACCNT.DES")]
        public bool TransferAccount { get; set; }

        /// <summary>
        /// Stock Only Account (Do Not Invoice) (?)
        /// The hell?
        /// Optional; 'Y' or 'N' if given
        /// </summary>
        [UniVerseIndex(588)]
        [UniVerseFieldName("STOCK.ONLY.DES")]
        public bool StockOnlyAccount { get; set; }

        /// <summary>
        /// Receiving Location (?)
        /// Optional; validates against STLOCATION.CODE
        /// </summary>
        [UniVerseIndex(589)]
        [UniVerseFieldName("RECEIVE.LOC.DES")]
        public string ReceivingLocation { get; set; }

        /// <summary>
        /// Store Name
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(594)]
        [UniVerseFieldName("STORE.NAME")]
        public string StoreName { get; set; }

        /// <summary>
        /// Interim Statement Actions (?)
        /// Optional, 'Y' or 'N' if given
        /// Defaults: IF(STAT.PRINT # "Y","N"[S],STMNT.CREDACT)[M] -- whatever that does
        /// </summary>
        [UniVerseIndex(608)]
        [UniVerseFieldName("STMNT.CREDACT")]
        public bool InterimStatementActions { get; set; }

        /// <summary>
        /// Credit Note (Copies) (?)
        /// Optional: validates, see [SL].[PG.VAL.PRINTNO] -- Again, a count of the number of duplicates of 
        /// a credit note which should be produced
        /// </summary>
        [UniVerseIndex(611)]
        [UniVerseFieldName("CRD.COPY")]
        public byte? CreditNoteCopiesCount { get; set; }

        /// <summary>
        /// Print Invoice Details
        /// Optional; 'Y', 'N' or '' if given
        /// Defaults: IF(OVERDUE.LETTERS = "N",""[S],IF(@VALUE = "","Y",@VALUE))[M] -- whatever that means
        /// </summary>
        [UniVerseIndex(612)]
        [UniVerseFieldName("OVERDUE.TOTALS")]
        public bool PrintInvoiceDetails { get; set; }

        /// <summary>
        /// Consolidate Indicator (?)
        /// Optional; must be 'Y' or 'N' if given.
        /// </summary>
        [UniVerseIndex(619)]
        [UniVerseFieldName("DEF.CONSOLIDATE")]
        public bool Consolidate { get; set; }

        /// <summary>
        /// Retail Till Customer
        /// Optional; 'Y' or 'N' if given
        /// Defaults: 'N'
        /// </summary>
        [UniVerseIndex(650)]
        [UniVerseFieldName("RETAIL.TILL.CUST")]
        public bool RetailTillCustomer { get; set; }

        /// <summary>
        /// Exclude From Invoice Print (?)
        /// Optional; if present must be 'Y' or 'N'
        /// </summary>
        [UniVerseIndex(659)]
        [UniVerseFieldName("EXCLUDE.INVOICE")]
        public bool ExcludeFromInvoicePrint { get; set; }

        /// <summary>
        /// Cancellation Letters (?)
        /// Optional; 'Y' or 'N' if given
        /// Defaults: 'Y'
        /// </summary>
        [UniVerseIndex(689)]
        [UniVerseFieldName("CANX.LETTS.DES")]
        public bool SendCancellationLetters { get; set; }

        /// <summary>
        /// Memorandum
        /// </summary>
        [UniVerseIndex(690)]
        [UniVerseMultiValue]
        [UniVerseFieldName("CONT.MOBNO.DES")]
        [JsonIgnore]
        public IEnumerable<string> ContactPhonesMobile { get; set; }

        /// <summary>
        /// Sample Letter Text / Titles In
        /// Think this one is the Language code 
        /// Optional; validates against REP.TITLES.DES
        /// </summary>
        [UniVerseIndex(694)]
        [UniVerseFieldName("TITLE.TRANS.DES")]
        public string SampleLetterTitleText { get; set; }

        /// <summary>
        /// Order Category
        /// Optional; validates against ORD.CATAGORY.DES if given
        /// </summary>
        [UniVerseIndex(698)]
        [UniVerseFieldName("ORDER.CATAGORY.DES")]
        public string OrderCategory { get; set; }

        /// <summary>
        /// Retail or Contract
        /// Optional; validates '', 'R'etail or 'C'ontract if given
        /// </summary>
        [UniVerseIndex(699)]
        [UniVerseFieldName("RET.CONT.DES")]
        public string RetailOrContract { get; set; }

        /// <summary>
        /// Stockist Category (?)
        /// No idea what this does
        /// Optional; validates '' or against SL.STOCKIST.CAT.CATEGORY if given
        /// </summary>
        [UniVerseIndex(706)]
        [UniVerseFieldName("SKIST.CAT")]
        public string StockistCategory { get; set; }

        /// <summary>
        /// Current Advertiser (?)
        /// No idea what this does
        /// Optional; validates 'Y' or 'N' if given
        /// </summary>
        [UniVerseIndex(727)]
        [UniVerseFieldName("ADVERT.DES")]
        public bool Advertiser { get; set; }

        /// <summary>
        /// Exempt from High Value Order Check?
        /// Optional; 'Y' or 'N'
        /// </summary>
        [UniVerseIndex(728)]
        [UniVerseFieldName("HIGH.VALUE.EXEMPT.DES")]
        public bool HighValueOrderCheckExempt { get; set; }

        /// <summary>
        /// Rf Scanner Memo (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(729)]
        [UniVerseMultiValue]
        [UniVerseFieldName("RF.SCANNER.MEMO.DES")]
        public IEnumerable<string> RFScannerMemos { get; set; }

        /// <summary>
        /// Confirmation Memo (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(730)]
        [UniVerseMultiValue]
        [UniVerseFieldName("CONFIRMATION.MEMO.DES")]
        public IEnumerable<string> ConfirmationMemos { get; set; }

        /// <summary>
        /// Export with no Packing Note
        /// Optional; must be 'Y' or 'N' if given.
        /// </summary>
        [UniVerseIndex(731)]
        [UniVerseFieldName("EXP.NO.PN.DES")]
        public bool ExportWithNoPackingNote { get; set; }

        /// <summary>
        /// End Customer Invoice?
        /// Is this account permitted to cause invoicing on End Customers
        /// Optional; 'Y' or 'N' if given
        /// Defaults: 'N'
        /// </summary>
        [UniVerseIndex(751)]
        [UniVerseFieldName("END.CUST.INV.DES")]
        public bool EndCustomerInvoicing { get; set; }

        /// <summary>
        /// Haulier for Consolidated Invoice
        /// Optional; validates against OEHAULIER if given
        /// </summary>
        [UniVerseIndex(752)]
        [UniVerseFieldName("HAULIER.CONSOLIDATE.DES")]
        public string HaulierCodeForConsolidatedInvoice { get; set; }

        /// <summary>
        /// TNT Account Number
        /// Optional; not validated
        /// </summary>
        [UniVerseIndex(754)]
        [UniVerseFieldName("TNT.BILLING.DES")]
        public string TNTAccountNumber { get; set; }

        /// <summary>
        /// ABC Pricing (?)
        /// Optional; 'Y' or 'N' if given
        /// Defaults: 'N'
        /// </summary>
        [UniVerseIndex(756)]
        [UniVerseFieldName("ABC.PRICING.DES")]
        public bool ABCPricing { get; set; }

        /// <summary>
        /// Credit Limit Check Against
        /// What the hell? (?) 
        /// Optional: if entered, must be either '1' or '2'...?
        /// </summary>
        [UniVerseIndex(757)]
        [UniVerseFieldName("CL.CHECK.AGAINST.DES")]
        public byte? CreditLimitCheckAgainst { get; set; }

        /// <summary>
        /// MTM Carriage Charged (?)
        /// Optional; 'Y' or 'N' if given
        /// Defaults: 'Y'
        /// </summary>
        [UniVerseIndex(758)]
        [UniVerseFieldName("MTM.CARR.CHARGED.DES")]
        public bool MTMCarriageCharged { get; set; }

        /// <summary>
        /// Cut Off Value
        /// Optional; validates {0:999999999} if given
        /// Defaults: IF(MTM.CARR.CHARGED.DES = "N",""[S],<759>)[M]
        /// </summary>
        [UniVerseIndex(759)]
        [UniVerseFieldName("CUT.OFF.VALUE.DES")]
        public int? CutOffValue { get; set; }

        /// <summary>
        /// New Picking List for Goods In / Problem Items
        /// Optional; 'Y' or 'N' if given
        /// </summary>
        [UniVerseIndex(762)]
        [UniVerseFieldName("NEWPL.FOR.GIPROB.DES")]
        public bool NewPickingListForGoodsInProblemItems { get; set; }

        /// <summary>
        /// Free of Charge Promotion Code
        /// If <748> FOC.ORDERS.DES != "", then mandatory.
        /// Validates: see [SL].[PG.FOC.PROMO.VAL.DES]
        ///                if entered, must exist on OEPROMOTIONS.DES
        /// </summary>
        [UniVerseIndex(764)]
        [UniVerseFieldName("FOC.CODE.DES")]
        public string FreeOfChargePromotionCode { get; set; }

        /// <summary>
        /// UPS Account Number
        /// optional; not validated
        /// </summary>
        [UniVerseIndex(771)]
        [UniVerseFieldName("UPS.ACCOUNT.NO.DES")]
        public string UPSInvoiceAccountNumber { get; set; }

        /// <summary>
        /// DHL Account Number
        /// Optional; not validated
        /// </summary>
        [UniVerseIndex(789)]
        [UniVerseFieldName("DHL.ACCOUNT.NO.DES")]
        public string DHLAccountNumber { get; set; }

        /// <summary>
        /// End Customer Required ?
        /// Validates: 'Y' or 'N' if given
        /// </summary>
        [UniVerseIndex(790)]
        [UniVerseFieldName("END.CUST.REQ.DES")]
        public bool EndCustomerRequired { get; set; }

        /// <summary>
        /// WebTex Exclude List
        /// Optional; validates against STPRODEXCLUDE if given
        /// That appears to be a list of products which are intentionally excluded from being available via WebTex
        /// </summary>
        [UniVerseIndex(791)]
        [UniVerseFieldName("WEBTEX.EXCLUDE.LIST")]
        public string ProductExclusionListCode { get; set; }

        /// <summary>
        /// UPS Returns Label (?)
        /// No idea what this does
        /// Optional; 'Y' or 'N' if given
        /// </summary>
        [UniVerseIndex(824)]
        [UniVerseFieldName("UPS.RET.LAB")]
        public bool UPSReturnsLabel { get; set; }

        /// <summary>
        /// "No Email" (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(849)]
        [UniVerseFieldName("CALLED")]
        public bool NoEmail { get; set; }

        /// <summary>
        /// Miscellaneous Account 
        /// Does not validate, or default
        /// </summary>
        [UniVerseIndex(851)]
        [UniVerseFieldName("MISC.ACCNT.DES")]
        public bool MiscellaneousAccount { get; set; }

        /// <summary>
        /// Allocate Stock for Proforma Orders
        /// Optional; 'Y' or 'N' if given
        /// </summary>
        [UniVerseIndex(898)]
        [UniVerseFieldName("ALLOC.PROFORMA.DES")]
        public bool AllocateStockForProformaOrders { get; set; }

        /// <summary>
        /// UPS Add On Percentage (?)
        /// Optional; validates {0:999999} if given "must be in range 0 to 999.9"
        /// THIS MEANS THAT MOST IF NOT ALL OF THEIR NUMBER VALUES ARE HELD TO THIS THOUSAND MULTIPLIER THING
        /// </summary>
        [UniVerseIndex(906)]
        [UniVerseFieldName("UPS.ADD.ON.DES")]
        public decimal? UPSAddOnPercentage { get; set; }

        /// <summary>
        /// Print Invoice at Confirmation
        /// Optional; 'N', 'T', 'A' if given
        /// </summary>
        [UniVerseIndex(907)]
        [UniVerseFieldName("PRINT.INV.AT.CONF.DES")]
        public bool PrintInvoiceAtConfirmation { get; set; }

        /// <summary>
        /// Days Forward for Reservations
        /// Optional; validates [OE].[PG.VAL.RESDAYS] if given
        ///                     must be either '' or >= 2
        /// </summary>
        [UniVerseIndex(908)]
        [UniVerseFieldName("RESERVE.DAYS.DES")]
        public byte? DaysForwardForReservations { get; set; }

        /// <summary>
        /// Print JLP Ean Labels (?)
        /// No idea what this does
        /// Optional; 'Y' or 'N' if given
        /// </summary>
        [UniVerseIndex(916)]
        [UniVerseFieldName("JLP.LABELS.DES")]
        public bool PrintJLPEanLabels { get; set; }

        /// <summary>
        /// Exit Code Email (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(948)]
        [UniVerseFieldName("EXIT.CODE.EMAIL")]
        public string ExitCodeEmail { get; set; }

        /// <summary>
        /// Credit Controller: optional; must be a code from SL.CONTROLLER - which is empty.
        /// </summary>
        [UniVerseIndex(986)]
        [UniVerseFieldName("CREDCONT.DES")]
        public string CreditController { get; set; }

        /// <summary>
        /// Email address for Marketing Correspondence (?)
        /// </summary>
        [UniVerseIndex(1002)]
        [UniVerseFieldName("SS.MARKET.EMAIL")]
        public string MarketingEmail { get; set; }

        /// <summary>
        /// The name of a contact...? Relatively recently added, obviously. (?)
        /// </summary>
        [UniVerseIndex(1003)]
        [UniVerseFieldName("SS.CONTACT.NAME")]
        public string ContactName { get; set; }

        /// <summary>
        /// Country of Registration for VAT Number
        /// </summary>
        [UniVerseIndex(1008)]
        [UniVerseFieldName("VATCOUNTRY")]
        public string VATCountry { get; set; }

        /// <summary>
        /// Invoice VAT Display (?)
        /// Optional; not validated. No idea what this is for.
        /// </summary>
        [UniVerseIndex(1009)]
        [UniVerseFieldName("INVVATDISPLAY")]
        public string VATInvoiceDisplay { get; set; }

        /// <summary>
        /// Valid VAT Number? optional
        /// </summary>
        [UniVerseIndex(1010)]
        [UniVerseFieldName("VALIDVATNO")]
        public string VATNumberValid { get; set; }

        [JsonAlsoUpdates("VATLastCheckedDate")]
        public DateTime? VATLastCheckedDT { get; set; }

        /// <summary>
        /// Date VAT Number was last checked for validity: optional
        /// </summary>
        [UniVerseIndex(1011)]
        [UniVerseFieldName("VATDATLASTCHK")]
        [JsonIgnore]
        public string VATLastCheckedDate
        {
            get
            {
                if (VATLastCheckedDT.HasValue)
                {
                    return Framework.UniVerseDateString(VATLastCheckedDT.Value);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (Int32.TryParse(value, out int valInt))
                    {
                        VATLastCheckedDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        VATLastCheckedDT = null;
                    }
                }
            }
        }

        /// <summary>
        /// Bankrupt (?) optional, not validated
        /// </summary>
        [UniVerseIndex(1012)]
        [UniVerseFieldName("BANKRUPT")]
        public string Bankrupt { get; set; }

        /// <summary>
        /// Purchase Order Req (?) No idea. Not validated.
        /// </summary>
        [UniVerseIndex(1013)]
        [UniVerseFieldName("POREQ")]
        public bool PurchaseOrderRequired { get; set; }

        /// <summary>
        /// Invalid... VAT... Number...? optional.
        /// </summary>
        [UniVerseIndex(1014)]
        [UniVerseFieldName("INVALIDVATNO")]
        public string VATNumberInvalid { get; set; }

        /// <summary>
        /// Tax Identifier: optional, no specifics about what sort of tax identifier...
        /// </summary>
        [UniVerseIndex(1015)]
        [UniVerseFieldName("TAXID")]
        public string TaxID { get; set; }

        /// <summary>
        /// Site-Specific Field 1 (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(1016)]
        [UniVerseFieldName("SS.FIELD1")]
        public bool eCommerceCustomer { get; set; }

        /// <summary>
        /// Site-Specific Field 2 (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(1017)]
        [UniVerseFieldName("SS.FIELD2")]
        public string SSField2 { get; set; }

        /// <summary>
        /// Wrapping (?)
        /// Optional; validates "W" ? See [SL].[PG.F3,W*Wrapping]
        ///             - this is a "global tool / process" and I can't seem to get to its definition
        /// </summary>
        [UniVerseIndex(1018)]
        [UniVerseFieldName("GROUPAGE.TYPE")]
        public string GroupageType { get; set; }

        /// <summary>
        /// Site-Specific Field 4 (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(1019)]
        [UniVerseFieldName("SS.FIELD4")]
        public string SSField4 { get; set; }

        /// <summary>
        /// Site-Specific Field 5 (?)
        /// This was, once-upon-a-time, where Sarah wanted me to store EORI numbers. In error.
        /// </summary>
        [UniVerseIndex(1020)]
        [UniVerseFieldName("SS.FIELD5")]
        public string SSField5 { get; set; }

        #region Linked Entities

        [JsonIgnore]
        [UniVerseForeignKey("InvoiceAccounts", "MertexerV2.Models.TradeAccount")]
        public virtual ICollection<TradeAccount> EndCustomers { get; set; }

        [JsonIgnore]
        [UniVerseForeignKey("TradeAccountType", "MertexerV2.Models.TradeAccountType")]
        public virtual TradeAccountType Type { get; set; }

        [JsonIgnore]
        [UniVerseForeignKey("TradeAccountClass", "MertexerV2.Models.TradeAccountClass")]
        public virtual TradeAccountClass Class { get; set; }

        [JsonIgnore]
        [UniVerseForeignKey("AnalysisCodeS", "MertexerV2.Models.Pricing.GeneralLedgerSubCode")]
        public virtual GeneralLedgerSubCode SubAnalysisCode { get; set; }

        [JsonIgnore]
        [UniVerseForeignKey("PaymentTerms", "MertexerV2.Models.Invoices.InvoiceTerms")]
        public virtual InvoiceTerms InvoiceTerms { get; set; }

        [JsonIgnore]
        [UniVerseForeignKey("DeliveryTerms", "MertexerV2.Models.General.DeliveryTerms")]
        public virtual DeliveryTerms DeliveryTerm { get; set; }

        [JsonProperty("TradeAccountContacts")]
        [JsonAlsoUpdates("Contacts", "ContactEmails", "ContactPhones", "ContactPhonesMobile", "ContactTypes")]
        public virtual IEnumerable<TradeAccountContact> TradeAccountContacts
        {
            get
            {
                var dummyList = new List<string>();

                for (var i = 0; i < Contacts?.Count(); i++)
                {
                    dummyList.Add(null);
                }

                return Contacts?.Select((c, i) =>
                {
                    var emails = (ContactEmails?.Count() >= dummyList.Count ? ContactEmails : dummyList).ToArray();
                    var phones = (ContactPhones?.Count() >= dummyList.Count ? ContactPhones : dummyList).ToArray();
                    var mobiles = (ContactPhonesMobile?.Count() >= dummyList.Count ? ContactPhonesMobile : dummyList).ToArray();
                    var types = (ContactTypes?.Count() >= dummyList.Count ? ContactTypes : dummyList).ToArray();
                    return new TradeAccountContact()
                    {
                        TradeAccountCode = TradeAccountCode,
                        Name = c,
                        Email = emails[i],
                        Phone = phones[i],
                        Mobile = mobiles[i],
                        Type = types[i]
                    };
                }).ToList();
            }
            set
            {
                var list = value.ToList();

                Contacts = list.Select(p => p.Name).ToList();
                ContactEmails = list.Select(p => p.Email).ToList();
                ContactPhones = list.Select(p => p.Phone).ToList();
                ContactPhonesMobile = list.Select(p => p.Mobile).ToList();
                ContactTypes = list.Select(p => p.Type).ToList();
            }
        }

        #endregion

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}