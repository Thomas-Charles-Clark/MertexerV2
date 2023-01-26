using MertexerV2.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Accounts
{
    public class TradeAccountCode
    {
        public string Code { get; set; }
    }

    public class TradeAccount
    {
        // This is the key: do not try to submit a value to field 0 as it might not work and it isn't where you
        // are supposed to be providing the key anyway.
        //[UniVerseIndex(0)]
        //[UniVerseFieldName("CUSREF")]
        /// <summary>
        /// This is the Customer Reference - the key on SLCUST.
        /// </summary>
        public string CUSREF { get; set; }

        /// <summary>
        /// Customer Name
        /// </summary>
        [UniVerseIndex(1, 1)]
        [UniVerseFieldName("CUSNAME")]
        public string CUSNAME { get; set; }

        /// <summary>
        /// Account State: 'O'pen or 'C'losed
        /// </summary>
        [UniVerseIndex(14)]
        [UniVerseFieldName("OPEN")]
        public string OPEN { get; set; }

        /// <summary>
        /// Sort Name: no idea where this is used, nor indeed how useful or otherwise it is. (?)
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("SORTNAME.EXP")]
        public string SORTNAME_EXP { get; set; }

        public DateTime? ONFILE_DT { get; set; }

        /// <summary>
        /// Account Creation Date (UniVerse int offset from 31/12/1967 format)
        /// </summary>
        [UniVerseIndex(4, 1)]
        [UniVerseFieldName("ONFILE")]
        public string ONFILE
        {
            get
            {
                if (ONFILE_DT.HasValue)
                {
                    return Framework.UniVerseDateString(ONFILE_DT.Value);
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
                        ONFILE_DT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        ONFILE_DT = null;
                    }
                }
            }
        }

        public DateTime? AMENDED_DT { get; set; }

        /// <summary>
        /// Date Last Amended
        /// </summary>
        [UniVerseIndex(4, 2)]
        [UniVerseFieldName("AMENDED")]
        public string AMENDED
        {
            get
            {
                if (AMENDED_DT.HasValue)
                {
                    return Framework.UniVerseDateString(AMENDED_DT.Value);
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
                        AMENDED_DT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        AMENDED_DT = null;
                    }
                }
            }
        }

        /// <summary>
        /// Code Group - This is a logical grouping of Trade Accounts. If provided, must be a value from OECODEGROUPS.
        /// </summary>
        [UniVerseIndex(541)]
        [UniVerseFieldName("CODE.GROUP")]
        public string CODE_GROUP { get; set; }

        /// <summary>
        /// The name of a contact...? Relatively recently added, obviously. (?)
        /// </summary>
        [UniVerseIndex(1003)]
        [UniVerseFieldName("SS.CONTACT.NAME")]
        public string SS_CONTACT_NAME { get; set; }

        //[UniVerseIndex(1, 1)]
        //[UniVerseFieldName("CUSNAMEX")]
        //public string CUSNAMEX { get; set; }

        /// <summary>
        /// Invoice Address Line 1
        /// </summary>
        [UniVerseIndex(1, 2)]
        [UniVerseFieldName("CUSADD1X")]
        public string CUSADD1X { get; set; }

        /// <summary>
        /// Invoice Address Line 2
        /// </summary>
        [UniVerseIndex(1, 3)]
        [UniVerseFieldName("CUSADD2X")]
        public string CUSADD2X { get; set; }

        /// <summary>
        /// Invoice Address Town
        /// </summary>
        [UniVerseIndex(1, 4)]
        [UniVerseFieldName("CUSADD3X")]
        public string CUSADD3X { get; set; }

        /// <summary>
        /// Invoice Address County
        /// </summary>
        [UniVerseIndex(1, 5)]
        [UniVerseFieldName("CUSADD4X")]
        public string CUSADD4X { get; set; }

        /// <summary>
        /// Invoice Address Postcode
        /// </summary>
        [UniVerseIndex(1, 6)]
        [UniVerseFieldName("CUSPCODE")]
        public string CUSPCODE { get; set; }

        /// <summary>
        /// Invoice Address Country
        /// </summary>
        [UniVerseIndex(77)]
        [UniVerseFieldName("COUNTRY")]
        public string COUNTRY { get; set; }

        /// <summary>
        /// Redundant Town Field (?)
        /// </summary>
        [UniVerseIndex(223)]
        [UniVerseFieldName("TOWN")]
        public string TOWN { get; set; }

        /// <summary>
        /// Redundant County field (?)
        /// </summary>
        [UniVerseIndex(224)]
        [UniVerseFieldName("COUNTY")]
        public string COUNTY { get; set; }

        /// <summary>
        /// Statement Account (?)
        /// </summary>
        [UniVerseIndex(98)]
        [UniVerseFieldName("STATEMENT.AC")]
        public string STATEMENT_AC { get; set; }

        /// <summary>
        /// Statement Name (?)
        /// </summary>
        [UniVerseIndex(29, 1)]
        [UniVerseFieldName("STNAME")]
        public string STNAME { get; set; }

        /// <summary>
        /// Statement Address Line 1 (?)
        /// </summary>
        [UniVerseIndex(29, 2)]
        [UniVerseFieldName("STADD1")]
        public string STADD1 { get; set; }



        /// <summary>
        /// Statement Address Line 2 (?)
        /// </summary>
        [UniVerseIndex(29, 3)]
        [UniVerseFieldName("STADD2")]
        public string STADD2 { get; set; }

        /// <summary>
        /// Statement Address Town (?)
        /// </summary>
        [UniVerseIndex(29, 4)]
        [UniVerseFieldName("STADD3")]
        public string STADD3 { get; set; }

        /// <summary>
        /// Statement Address County (?)
        /// </summary>
        [UniVerseIndex(29, 5)]
        [UniVerseFieldName("STADD4")]
        public string STADD4 { get; set; }

        /// <summary>
        /// Statement Address Postcode (?)
        /// </summary>
        [UniVerseIndex(29, 6)]
        [UniVerseFieldName("STPCODE")]
        public string STPCODE { get; set; }

        /// <summary>
        /// Statement Contact Name (?)
        /// </summary>
        [UniVerseIndex(401)]
        [UniVerseFieldName("STCONTACT")]
        public string STCONTACT { get; set; }

        /// <summary>
        /// Statement Telephone Number (?)
        /// </summary>
        [UniVerseIndex(398)]
        [UniVerseFieldName("STPHONE")]
        public string STPHONE { get; set; }

        /// <summary>
        /// Main Telephone Number
        /// </summary>
        [UniVerseIndex(393)]
        [UniVerseFieldName("STD.TELEPHONE")]
        public string STD_TELEPHONE { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [UniVerseIndex(225)]
        [UniVerseFieldName("EMAIL")]
        public string EMAIL { get; set; }

        /// <summary>
        /// Email address for Marketing Correspondence (?)
        /// </summary>
        [UniVerseIndex(1002)]
        [UniVerseFieldName("SS.MARKET.EMAIL")]
        public string SS_MARKET_EMAIL { get; set; }

        /// <summary>
        /// Website Address
        /// </summary>
        [UniVerseIndex(394)]
        [UniVerseFieldName("WEB.SITE")]
        public string WEB_SITE { get; set; }


        /// <summary>
        /// I'm not sure how this should go down...
        /// </summary>
        //public string SSLPOSTJSON { get; set; }

        //[UniVerseIndex(5)]
        //[UniVerseFieldName(null)]
        public List<SSLPostDocument> SSLPOSTJSON { get; set; }
        //{ 
        //    get 
        //    { 
        //        return JsonConvert.DeserializeObject<List<SSLPostDocument>>(SSLPOSTJSON ?? "[]"); 
        //    }
        //    set 
        //    { 

        //    } 
        //}



        /// <summary>
        /// Market: required, must be a value from CLMARKTYPE
        /// </summary>
        [UniVerseIndex(101, 1)]
        [UniVerseFieldName("MARKTYPE.DES")]
        public string MARKTYPE_DES { get; set; }

        /// <summary>
        /// Customer Type: required (?), must exist on SLTYPE
        /// </summary>
        [UniVerseIndex(9)]
        [UniVerseFieldName("CUSTYPE")]
        public string CUSTYPE { get; set; }

        /// <summary>
        /// Classification Code: optional, from SLCUSCLASS. Seems to be regional groupings.
        /// </summary>
        [UniVerseIndex(410)]
        [UniVerseFieldName("CLASS.CODE")]
        public string CLASS_CODE { get; set; }

        /// <summary>
        /// Sales Representative Code: required, must exist on SLSEL1
        /// </summary>
        [UniVerseIndex(11)]
        [UniVerseFieldName("SEL1")]
        public string SEL1 { get; set; }

        /// <summary>
        /// Sales Agent Code: optional, must exist on SLSEL2 if present
        /// </summary>
        [UniVerseIndex(12)]
        [UniVerseFieldName("SEL2")]
        public string SEL2 { get; set; }

        /// <summary>
        /// Sales Area: required (?), must exist on SLSEL3
        /// </summary>
        [UniVerseIndex(13)]
        [UniVerseFieldName("SEL3")]
        public string SEL3 { get; set; }

        /// <summary>
        /// Division: required (?) this is groupage: G/N/T/W/Y - Value must exist on SLDIVISIONS
        /// </summary>
        [UniVerseIndex(576)]
        [UniVerseFieldName("DIVISION.CODE")]
        public string DIVISION_CODE { get; set; }

        /// <summary>
        /// Company Registration Number: optional
        /// </summary>
        [UniVerseIndex(396)]
        [UniVerseFieldName("COMPANY.REGNO")]
        public string COMPANY_REGNO { get; set; }

        /// <summary>
        /// Economic Operators Registration and Identification Number: optional
        /// </summary>
        [UniVerseIndex(106)]
        [UniVerseFieldName("EORI.NUMBER")]
        public string EORI_NUMBER { get; set; }

        /// <summary>
        /// Tax Identifier: optional, no specifics about what sort of tax identifier...
        /// </summary>
        [UniVerseIndex(1015)]
        [UniVerseFieldName("TAXID")]
        public string TAXID { get; set; }

        /// <summary>
        /// Value Added Tax Registration Number
        /// </summary>
        [UniVerseIndex(96)]
        [UniVerseFieldName("VATREG.DES")]
        public string VATREG_DES { get; set; }

        /// <summary>
        /// Country of Registration for VAT Number
        /// </summary>
        [UniVerseIndex(1008)]
        [UniVerseFieldName("VATCOUNTRY")]
        public string VATCOUNTRY { get; set; }

        /// <summary>
        /// "Normal VAT Rate": optional - if entered, must be a valid VAT Code from CFVAT
        /// </summary>
        [UniVerseIndex(54)]
        [UniVerseFieldName("VATNORM")]
        public string VATNORM { get; set; }

        /// <summary>
        /// Date VAT Number was last checked for validity: optional
        /// </summary>
        [UniVerseIndex(1011)]
        [UniVerseFieldName("VATDATLASTCHK")]
        public string VATDATLASTCHK { get; set; }

        /// <summary>
        /// Valid VAT Number? optional
        /// </summary>
        [UniVerseIndex(1010)]
        [UniVerseFieldName("VALIDVATNO")]
        public string VALIDVATNO { get; set; }

        /// <summary>
        /// Invalid... VAT... Number...? optional.
        /// </summary>
        [UniVerseIndex(1014)]
        [UniVerseFieldName("INVALIDVATNO")]
        public string INVALIDVATNO { get; set; }

        /// <summary>
        /// Currency: required, must exist on XRATE, cannot be modified if there are transactions posted 
        /// see [SL].[PG.VCURRENCY]
        /// see [SL].[PG.VAL.BANK]
        /// </summary>
        [UniVerseIndex(42)]
        [UniVerseFieldName("CURRENCY")]
        public string CURRENCY { get; set; }

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
        public string SELLPRICE { get; set; }

        /// <summary>
        /// Bank Code (?) optional; see [SL].[PG.VAL.BANK]
        /// Must exist on BANK
        /// Like currency, this shouldn't be modified after creation (unless there happen to be no transactions or balances)
        /// Seems as though the currency can be automatically modified upon changing Bank Code to match the currency on the entered Bank record.
        /// </summary>
        [UniVerseIndex(75)]
        [UniVerseFieldName("BANKNO")]
        public string BANKNO { get; set; }

        /// <summary>
        /// Credit Controller: optional; must be a code from SL.CONTROLLER - which is empty.
        /// </summary>
        [UniVerseIndex(986)]
        [UniVerseFieldName("CREDCONT.DES")]
        public string CREDCONT_DES { get; set; }

        /// <summary>
        /// Credit Limit
        /// </summary>
        [UniVerseIndex(16)]
        [UniVerseFieldName("CRLIMIT")]
        public string CRLIMIT { get; set; }

        public DateTime? CRLIMIT_EXPIRY_DT { get; set; }

        /// <summary>
        /// Credit Limit Expiry Date
        /// </summary>
        [UniVerseIndex(56)]
        [UniVerseFieldName("CRLIMIT.EXP")]
        public string CRLIMIT_EXP
        {
            get
            {
                if (CRLIMIT_EXPIRY_DT.HasValue)
                    return Framework.UniVerseDateString(CRLIMIT_EXPIRY_DT.Value);
                else
                    return null;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (Int32.TryParse(value, out int valInt))
                    {
                        CRLIMIT_EXPIRY_DT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        CRLIMIT_EXPIRY_DT = null;
                    }
                }
            }
        }

        /// <summary>
        /// Exempt from High Value Order Check?
        /// Optional; 'Y' or 'N'
        /// </summary>
        [UniVerseIndex(728)]
        [UniVerseFieldName("HIGH.VALUE.EXEMPT.DES")]
        public string HIGH_VALUE_EXEMPT_DES { get; set; }

        /// <summary>
        /// Insured Credit Limit - optional (?)
        /// </summary>
        [UniVerseIndex(165)]
        [UniVerseFieldName("INS.CRLIM.SL.DES")]
        public string INS_CRLIM_SL_DES { get; set; }

        /// <summary>
        /// Credit Stopped? optional (?) defaults to 'N', should probably be either Y or N but is not validated
        /// </summary>
        [UniVerseIndex(17)]
        [UniVerseFieldName("CRSTOP")]
        public string CRSTOP { get; set; }

        /// <summary>
        /// Credit Limit Check Against
        /// What the hell? (?) 
        /// Optional: if entered, must be either '1' or '2'...?
        /// </summary>
        [UniVerseIndex(757)]
        [UniVerseFieldName("CL.CHECK.AGAINST.DES")]
        public string CL_CHECK_AGAINST_DES { get; set; }


        public DateTime? INS_EXPDT_SL_DES_DT { get; set; }

        /// <summary>
        /// Insured Credit Limit Expiry Date
        /// </summary>
        [UniVerseIndex(166)]
        [UniVerseFieldName("INS.EXPDT.SL.DES")]
        public string INS_EXPDT_SL_DES
        {
            get
            {
                if (INS_EXPDT_SL_DES_DT.HasValue)
                    return Framework.UniVerseDateString(INS_EXPDT_SL_DES_DT.Value);
                else
                    return null;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (Int32.TryParse(value, out int valInt))
                    {
                        INS_EXPDT_SL_DES_DT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        INS_EXPDT_SL_DES_DT = null;
                    }
                }
            }
        }

        //[UniVerseIndex(0)]
        //[UniVerseFieldName("STOP.REASON")]
        //public string STOP_REASON { get; set; }

        /// <summary>
        /// Bankrupt (?) optional, not validated
        /// </summary>
        [UniVerseIndex(1012)]
        [UniVerseFieldName("BANKRUPT")]
        public string BANKRUPT { get; set; }

        /// <summary>
        //  Payment Terms: optional; must exist on SLTERMS
        /// </summary>
        [UniVerseIndex(20)]
        [UniVerseFieldName("STTERMS")]
        public string STTERMS { get; set; }

        /// <summary>
        /// Purchase Order Req (?) No idea. Not validated.
        /// </summary>
        [UniVerseIndex(1013)]
        [UniVerseFieldName("POREQ")]
        public string POREQ { get; set; }

        /// <summary>
        /// Usual Haulier: defaults to SLCOUNTRY<3> using COUNTRY.
        /// Optional (?): must exist on OEHAULIER
        /// </summary>
        [UniVerseIndex(275)]
        [UniVerseFieldName("HAULIER")]
        public string HAULIER { get; set; }

        /// <summary>
        /// Normal Service: defaults to SLCOUNTRY<4> using COUNTRY
        /// Optional (?): see [OE].[PG.VAL.SLCARRIAGE]
        /// </summary>
        [UniVerseIndex(276)]
        [UniVerseFieldName("SERVICE")]
        public string SERVICE { get; set; }

        /// <summary>
        /// UPS Account Number
        /// optional; not validated
        /// </summary>
        [UniVerseIndex(771)]
        [UniVerseFieldName("UPS.ACCOUNT.NO.DES")]
        public string UPS_ACCOUNT_NO_DES { get; set; }

        /// <summary>
        /// UPS Taxes / Duty Account Number
        /// optional; see [OE].[PG.VAL.DUTY.ACCOUNT] ?!
        /// That's a very weird validation: if non-null, and less than 6 characters,
        /// then display:  "WARNING: This field should contain the account number to be charged any duty / taxes for the delivery. You have entered: "{value}". 
        /// Is this correct? Y / N"
        /// </summary>
        [UniVerseIndex(322)]
        [UniVerseFieldName("DUTY.ACC.NO")]
        public string DUTY_ACC_NO { get; set; }

        /// <summary>
        /// DHL Account Number
        /// Optional; not validated
        /// </summary>
        [UniVerseIndex(789)]
        [UniVerseFieldName("DHL.ACCOUNT.NO.DES")]
        public string DHL_ACCOUNT_NO_DES { get; set; }

        /// <summary>
        /// TNT Account Number
        /// Optional; not validated
        /// </summary>
        [UniVerseIndex(754)]
        [UniVerseFieldName("TNT.BILLING.DES")]
        public string TNT_BILLING_DES { get; set; }

        /// <summary>
        /// Delivery Terms
        /// Optional; not validated
        /// </summary>
        [UniVerseIndex(207)]
        [UniVerseFieldName("DELTERMS")]
        public string DELTERMS { get; set; }

        /// <summary>
        /// Exclude From Invoice Print (?)
        /// Optional; if present must be 'Y' or 'N'
        /// </summary>
        [UniVerseIndex(659)]
        [UniVerseFieldName("EXCLUDE.INVOICE")]
        public string EXCLUDE_INVOICE { get; set; }

        /// <summary>
        /// Invoice VAT Display (?)
        /// Optional; not validated. No idea what this is for.
        /// </summary>
        [UniVerseIndex(1009)]
        [UniVerseFieldName("INVVATDISPLAY")]
        public string INVVATDISPLAY { get; set; }

        /// <summary>
        /// Overdue Invoice Period of Grace
        /// Optional; not validated. Presumably some number of days.
        /// </summary>
        [UniVerseIndex(95)]
        [UniVerseFieldName("INV.ODUE.POG")]
        public string INV_ODUE_POG { get; set; }

        /// <summary>
        /// Electronic Data Interchange Invoice 
        /// Required; Defaults 'N'. Must be 'Y', 'N' or 'P'.
        /// </summary>
        [UniVerseIndex(441)]
        [UniVerseFieldName("EDI.INVOICE")]
        public string EDI_INVOICE { get; set; }

        /// <summary>
        /// Memorandum
        /// Optional; not validated. Multivalue.
        /// </summary>
        [UniVerseIndex(70)]
        [UniVerseFieldName("EXTENDEDMEMO.NV")]
        public string EXTENDEDMEMO_NV { get; set; }

        /// <summary>
        /// Partner Company
        /// Optional (?) reads COMPANY "1" using <153>... this is just 'What other UniVerse account... something something.'
        /// </summary>
        [UniVerseIndex(273)]
        [UniVerseFieldName("PARTNER.CO")]
        public string PARTNER_CO { get; set; }

        /// <summary>
        /// Analysis Code (A) - whatever that is (?)
        /// Optional; does not validate
        /// Defaults to B("DGLCODE,A.SALE") which is actually a call to a Default definition... which does some godawful mad thing.
        /// FORTUNATELY, this field is completely empty throughout the entirety of SLCUST.
        /// </summary>
        [UniVerseIndex(88)]
        [UniVerseFieldName("CUSACC1")]
        public string CUSACC1 { get; set; }

        /// <summary>
        /// Analysis Code (D) - whatever that is (?)
        /// Optional; does not validate
        /// Defaults to B("DGLCODE,D.SALE") which is actually a call to a Default definition... which does some godawful mad thing.
        /// FORTUNATELY, this field is completely empty throughout the entirety of SLCUST.
        /// </summary>
        [UniVerseIndex(10)]
        [UniVerseFieldName("CUSALYS")]
        public string CUSALYS { get; set; }

        /// <summary>
        /// Analysis Code (S) - whatever that is (?)
        /// Optional; does not validate
        /// Defaults to B("DGLCODE,S") which is actually a call to a Default definition... which does some godawful thing:
        /// IF(<42,-1> < 2000,<42,-1>*1000+<38>[S],<42,-1>*1000+<38>)
        /// This seems to result in 01 or 02, lord help me I can't see why it should
        /// </summary>
        [UniVerseIndex(32)]
        [UniVerseFieldName("CUSSUB")]
        public string CUSSUB { get; set; }

        /// <summary>
        /// Accept Sundries (?) what the hell does that mean?
        /// Can people refuse sundries? 
        /// Optional, if entered must be either 'Y' or 'N'
        /// </summary>
        [UniVerseIndex(68)]
        [UniVerseFieldName("SUNDRY.ACCPT")]
        public string SUNDRY_ACCPT { get; set; }

        /// <summary>
        /// Overdue Letter 1 (?)
        /// Optional? Vaidates: must be less than the entry for Overdue Letter 2
        /// 
        /// There are 5 of these. I assume they're some integer count for "How long until we send the Nth overdue letter".
        /// 
        /// </summary>
        [UniVerseIndex(238, 1)]
        [UniVerseFieldName("OD.LETTER1")]
        public string OD_LETTER1 { get; set; }

        /// <summary>
        /// Overdue Letter 2
        /// Optional: must be > Overdue Letter 1 and < Overdue Letter 3
        /// </summary>
        [UniVerseIndex(238, 2)]
        [UniVerseFieldName("OD.LETTER2")]
        public string OD_LETTER2 { get; set; }

        /// <summary>
        /// Overdue Letter 3
        /// Optional: must be > Overdue Letter 2 and < Overdue Letter 4
        /// </summary>
        [UniVerseIndex(238, 3)]
        [UniVerseFieldName("OD.LETTER3")]
        public string OD_LETTER3 { get; set; }

        /// <summary>
        /// Overdue Letter 4
        /// Optional: must be > Overdue Letter 3 and < Overdue Letter 5
        /// </summary>
        [UniVerseIndex(238, 4)]
        [UniVerseFieldName("OD.LETTER4")]
        public string OD_LETTER4 { get; set; }

        /// <summary>
        /// Overdue Letter 5
        /// Optional: must be > Overdue Letter 4
        /// </summary>
        [UniVerseIndex(238, 5)]
        [UniVerseFieldName("OD.LETTER4")]
        public string OD_LETTER5 { get; set; }

        /// <summary>
        /// Default Delivery Reference
        /// Optional; must exist on DELADD if given
        /// </summary>
        [UniVerseIndex(53)]
        [UniVerseFieldName("DEF.DELREF")]
        public string DEF_DELREF { get; set; }

        /// <summary>
        /// Commercial Invoice (?)
        /// Optional; must be either 'Y' or 'N' if given
        /// </summary>
        [UniVerseIndex(227)]
        [UniVerseFieldName("COMM.INV.DES")]
        public string COMM_INV_DES { get; set; }

        /// <summary>
        /// Normal Order Location (?)
        /// Optional; must exist on STLOCATION if given
        /// </summary>
        [UniVerseIndex(307)]
        [UniVerseFieldName("ORDER.LOC")]
        public string ORDER_LOC { get; set; }

        /// <summary>
        /// Export (?)
        /// Optional; must be either 'Y' or 'N' if given. 
        /// Defaults to: see [SL].[PG.DEF.EXPORTFLAG.DES] - checks if <77> # the Company file <251>, then 'Y' else 'N'. 
        /// </summary>
        [UniVerseIndex(89)]
        [UniVerseFieldName("EXPORT.DES")]
        public string EXPORT_DES { get; set; }

        /// <summary>
        /// Export with no Packing Note
        /// Optional; must be 'Y' or 'N' if given.
        /// </summary>
        [UniVerseIndex(731)]
        [UniVerseFieldName("EXP.NO.PN.DES")]
        public string EXP_NO_PN_DES { get; set; }

        /// <summary>
        /// Consolidate Indicator (?)
        /// Optional; must be 'Y' or 'N' if given.
        /// </summary>
        [UniVerseIndex(619)]
        [UniVerseFieldName("DEF.CONSOLIDATE")]
        public string DEF_CONSOLIDATE { get; set; }

        /// <summary>
        /// Carriage Surcharge Exempt
        /// Optional; Y,N,E,D,F,1 (?)
        /// Default: 'F'
        /// </summary>
        [UniVerseIndex(161)]
        [UniVerseFieldName("CARR.EXEM.SL")]
        public string CARR_EXEM_SL { get; set; }

        /// <summary>
        /// Order Confirmation Print (?)
        /// Optional; Validates: see [SL].[PG.AFTFAX] "After Fax Indicator".
        ///                      Y,N,F,E,B,C,A
        /// </summary>
        [UniVerseIndex(162)]
        [UniVerseFieldName("CONF.REQ.SL")]
        public string CONF_REQ_SL { get; set; }

        /// <summary>
        /// Proforma Trading (?) Why this as well as STTERMS...?
        /// Optional; 'Y' or 'N' if given
        /// </summary>
        [UniVerseIndex(163)]
        [UniVerseFieldName("PF.TRADING")]
        public string PF_TRADING { get; set; }

        /// <summary>
        /// Free of Charge Promotion Code
        /// If <748> FOC.ORDERS.DES != "", then mandatory.
        /// Validates: see [SL].[PG.FOC.PROMO.VAL.DES]
        ///                if entered, must exist on OEPROMOTIONS.DES
        /// </summary>
        [UniVerseIndex(764)]
        [UniVerseFieldName("FOC.CODE.DES")]
        public string FOC_CODE_DES { get; set; }

        /// <summary>
        /// Invoice Detail Print (?)
        /// Optional; 'T' or 'P' if given.
        /// </summary>
        [UniVerseIndex(167)]
        [UniVerseFieldName("INV.DETAILS")]
        public string INV_DETAILS { get; set; }

        /// <summary>
        /// Selling Company (?)
        /// Optional; validates: must exist in SLSELLCO.TITLE
        /// </summary>
        [UniVerseIndex(82, 1)]
        [UniVerseFieldName("SELLCO")]
        public string SELLCO { get; set; }

        /// <summary>
        /// Wrapping (?)
        /// Optional; validates "W" ? See [SL].[PG.F3,W*Wrapping]
        ///             - this is a "global tool / process" and I can't seem to get to its definition
        /// </summary>
        [UniVerseIndex(1018)]
        [UniVerseFieldName("GROUPAGE.TYPE")]
        public string GROUPAGE_TYPE { get; set; }

        /// <summary>
        /// Direct Delivery Account (?)
        /// Optional, 'Y', 'N' or '' if given.
        /// Defaults: ''
        /// </summary>
        [UniVerseIndex(90)]
        [UniVerseFieldName("DIR.DEL")]
        public string DIR_DEL { get; set; }

        /// <summary>
        /// Packing List (Copies) (?)
        /// Optional: validates, see [SL].[PG.VAL.PRINTNO] "Validation max no. of Printing"
        /// This appears to be a count: how many additional copies of the packing instructions should be printed?
        /// but, as with so many other things in Mertex, the question is: WHY DOES THIS EVEN NEED TO EXIST
        /// </summary>
        [UniVerseIndex(198)]
        [UniVerseFieldName("PACKLIST.SL")]
        public string PACKLIST_SL { get; set; }

        /// <summary>
        /// Credit Note (Copies) (?)
        /// Optional: validates, see [SL].[PG.VAL.PRINTNO] -- Again, a count of the number of duplicates of 
        /// a credit note which should be produced
        /// </summary>
        [UniVerseIndex(611)]
        [UniVerseFieldName("CRD.COPY")]
        public string CRD_COPY { get; set; }

        /// <summary>
        /// ABC Pricing (?)
        /// Optional; 'Y' or 'N' if given
        /// Defaults: 'N'
        /// </summary>
        [UniVerseIndex(756)]
        [UniVerseFieldName("ABC.PRICING.DES")]
        public string ABC_PRICING_DES { get; set; }

        /// <summary>
        /// Standard Roll Length (?) 
        /// What the hell is this?! How can a customer have a standard roll length? What does this MEAN?!?
        /// Optional; validates non-negative
        /// </summary>
        [UniVerseIndex(160)]
        [UniVerseFieldName("STD.ROLLEN")]
        public string STD_ROLLEN { get; set; }

        /// <summary>
        /// Minimum Carriage Value Surcharge (?)
        /// Optional; 'Y' or 'N' if given
        /// Defaults: 'N'
        /// </summary>
        [UniVerseIndex(376)]
        [UniVerseFieldName("MIN.VALUE.SUR")]
        public string MIN_VALUE_SUR { get; set; }

        /// <summary>
        /// Delivery Change Reconfirmation (?)
        /// Is this some sort of double-prompt for when a user changes the delivery address on an order belonging to this customer?
        /// Optional; 'Y' or 'N' if given.
        /// Defaults: 'N'
        /// </summary>
        [UniVerseIndex(133)]
        [UniVerseFieldName("DELCHANGE")]
        public string DELCHANGE { get; set; }

        /// <summary>
        /// Proforma Surcharge (?)
        /// Optional; does not validate
        /// </summary>
        [UniVerseIndex(164)]
        [UniVerseFieldName("PF.SURCHARGE")]
        public string PF_SURCHARGE { get; set; }

        ///// <summary>
        ///// Free of Charge Orders Only?
        ///// Optional; 'Y' or 'N' if given
        ///// Defaults: 'N'
        ///// </summary>
        ///// DEFUNCT 10/09/2021
        //[UniVerseIndex(748)]
        //[UniVerseFieldName("FOC.ORDERS.DES")]
        //public string FOC_ORDERS_DES { get; set; }

        /// <summary>
        /// Invoice Copies
        /// Optional; validates using [SL].[PG.VAL.PRINTNO]
        /// Default: 1
        /// </summary>
        [UniVerseIndex(81)]
        [UniVerseFieldName("INV.COPIES")]
        public string INV_COPIES { get; set; }

        /// <summary>
        /// Distributor Reference (Code)
        /// Optional; validates: must exist on OEDISTRIBUTOR if given
        /// </summary>
        [UniVerseIndex(568)]
        [UniVerseFieldName("DISTREF")]
        public string DISTREF { get; set; }

        /// <summary>
        /// Transfer Account
        /// Optional; 'Y', 'N' or '' if given
        /// Default: ''
        /// </summary>
        [UniVerseIndex(587)]
        [UniVerseFieldName("TRANS.ACCNT.DES")]
        public string TRANS_ACCNT_DES { get; set; }

        /// <summary>
        /// Stock Only Account (Do Not Invoice) (?)
        /// The hell?
        /// Optional; 'Y' or 'N' if given
        /// </summary>
        [UniVerseIndex(588)]
        [UniVerseFieldName("STOCK.ONLY.DES")]
        public string STOCK_ONLY_DES { get; set; }

        /// <summary>
        /// Receiving Location (?)
        /// Optional; validates against STLOCATION.CODE
        /// </summary>
        [UniVerseIndex(589)]
        [UniVerseFieldName("RECEIVE.LOC.DES")]
        public string RECEIVE_LOC_DES { get; set; }

        /// <summary>
        /// Contract Pricing
        /// Optional; 'Y', 'N' or '' if given
        /// Defaults: 'Y'
        /// </summary>
        [UniVerseIndex(100)]
        [UniVerseFieldName("CONT.PRICING")]
        public string CONT_PRICING { get; set; }

        /// <summary>
        /// Product Screen (?)
        /// The hell?
        /// </summary>
        [UniVerseIndex(317)]
        [UniVerseFieldName("PROD.SCREEN")]
        public string PROD_SCREEN { get; set; }

        /// <summary>
        /// Minimum Order Quantity (?)
        /// Optional; validates: if given, must be a positive number
        /// Defaults: IF(F("OEPARMS","1")<229> = "", ""[S],<118>)[M] -- whatever that means
        /// </summary>
        [UniVerseIndex(118)]
        [UniVerseFieldName("MINORDQTY.SL")]
        public string MINORDQTY_SL { get; set; }

        /// <summary>
        /// [Send] Overdue Letters?
        /// Mandatory: must be either 'Y' or 'N'
        /// </summary>
        [UniVerseIndex(236)]
        [UniVerseFieldName("OVERDUE.LETTERS")]
        public string OVERDUE_LETTERS { get; set; }

        /// <summary>
        /// Print Invoice Details
        /// Optional; 'Y', 'N' or '' if given
        /// Defaults: IF(OVERDUE.LETTERS = "N",""[S],IF(@VALUE = "","Y",@VALUE))[M] -- whatever that means
        /// </summary>
        [UniVerseIndex(612)]
        [UniVerseFieldName("OVERDUE.TOTALS")]
        public string OVERDUE_TOTALS { get; set; }

        /// <summary>
        /// Cut Verify (?)
        /// No idea what this does
        /// Optional; '', 0, 1, 2, 3
        /// </summary>
        [UniVerseIndex(159)]
        [UniVerseFieldName("CUT.VERIFY")]
        public string CUT_VERIFY { get; set; }

        /// <summary>
        /// Payment Details [Type...?] (?)
        /// Optional; validates - see [SL].[PG.VCCARD.DETAIL]
        ///                                 '0', '1', '2'
        ///                         uses <318>...
        /// Defaults: '0'
        /// </summary>
        [UniVerseIndex(318)]
        [UniVerseFieldName("CCARD.DETAILS")]
        public string CCARD_DETAILS { get; set; }

        /// <summary>
        /// Out of Stock Letter (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(354)]
        [UniVerseFieldName("OOS.LETTER")]
        public string OOS_LETTER { get; set; }

        /// <summary>
        /// Statement Print (?)
        /// Optional; 'Y' or 'N' if given
        /// Defaults: 'Y'
        /// </summary>
        [UniVerseIndex(319)]
        [UniVerseFieldName("STAT.PRINT")]
        public string STAT_PRINT { get; set; }

        /// <summary>
        /// Internal or External
        /// Optional; 'I' or 'E' if given [for Internal, External]
        /// Defaults: 'E'
        /// </summary>
        [UniVerseIndex(318)]
        [UniVerseFieldName("INT.EXT")]
        public string INT_EXT { get; set; }

        /// <summary>
        /// Direct to Customer ANA (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(443)]
        [UniVerseFieldName("EDI.DIRECT.CUST")]
        public string EDI_DIRECT_CUST { get; set; }

        /// <summary>
        /// File Tranfer Protocol (?)
        /// No idea what this does
        /// Optional; validates 'F','D','E','M'
        /// </summary>
        [UniVerseIndex(424)]
        [UniVerseFieldName("FILE.PROTOCOL")]
        public string FILE_PROTOCOL { get; set; }

        /// <summary>
        /// User ID (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(426)]
        [UniVerseFieldName("TRAN.USER")]
        public string TRAN_USER { get; set; }

        /// <summary>
        /// Allow entry of Till Credit Notes (?)
        /// No idea what this does
        /// Optional; 'Y' or 'N' if given
        /// </summary>
        [UniVerseIndex(445)]
        [UniVerseFieldName("TILL.CREDITS")]
        public string TILL_CREDITS { get; set; }

        /// <summary>
        /// Sample Letter Text / Titles In
        /// Think this one is the Language code 
        /// Optional; validates against REP.TITLES.DES
        /// </summary>
        [UniVerseIndex(694)]
        [UniVerseFieldName("TITLE.TRANS.DES")]
        public string TITLE_TRANS_DES { get; set; }

        /// <summary>
        /// End Customer Invoice?
        /// Is this account permitted to cause invoicing on End Customers
        /// Optional; 'Y' or 'N' if given
        /// Defaults: 'N'
        /// </summary>
        [UniVerseIndex(751)]
        [UniVerseFieldName("END.CUST.INV.DES")]
        public string END_CUST_INV_DES { get; set; }

        /// <summary>
        /// WebTex Exclude List
        /// Optional; validates against STPRODEXCLUDE if given
        /// That appears to be a list of products which are intentionally excluded from being available via WebTex
        /// </summary>
        [UniVerseIndex(791)]
        [UniVerseFieldName("WEBTEX.EXCLUDE.LIST")]
        public string WEBTEX_EXCLUDE_LIST { get; set; }

        /// <summary>
        /// Miscellaneous Account 
        /// Does not validate, or default
        /// </summary>
        [UniVerseIndex(851)]
        [UniVerseFieldName("MISC.ACCNT.DES")]
        public string MISC_ACCNT_DES { get; set; }

        /// <summary>
        /// Price Code
        /// No idea what this does... what about SELLPRICE?
        /// There is only ONE account with any value in here, and that's W1202 with an 'N'.
        /// </summary>
        [UniVerseIndex(87)]
        [UniVerseFieldName("PRICECODE")]
        public string PRICECODE { get; set; }

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
        public string DELDATE_SL { get; set; }

        /// <summary>
        /// Min Order Val Warning
        /// Optional, does not validate
        /// Defaults: 500
        /// </summary>
        [UniVerseIndex(59)]
        [UniVerseFieldName("MINORDVAL.SL")]
        public string MINORDVAL_SL { get; set; }

        /// <summary>
        /// Check for Duplicates
        /// Optional, 'Y' or '' if given
        /// </summary>
        [UniVerseIndex(206)]
        [UniVerseFieldName("CHKDUPLICATE")]
        public string CHKDUPLICATE { get; set; }

        /// <summary>
        /// ID Code (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(542)]
        [UniVerseFieldName("TILLID.CODE")]
        public string TILLID_CODE { get; set; }

        /// <summary>
        /// Usual Order Unit (?)
        /// Optional, validates against STUNITS.CODE if given
        /// </summary>
        [UniVerseIndex(543)]
        [UniVerseFieldName("USUAL.ORD.UNIT")]
        public string USUAL_ORD_UNIT { get; set; }

        /// <summary>
        /// Cancellation Letters (?)
        /// Optional; 'Y' or 'N' if given
        /// Defaults: 'Y'
        /// </summary>
        [UniVerseIndex(689)]
        [UniVerseFieldName("CANX.LETTS.DES")]
        public string CANX_LETTS_DES { get; set; }

        /// <summary>
        /// Interim Statement Actions (?)
        /// Optional, 'Y' or 'N' if given
        /// Defaults: IF(STAT.PRINT # "Y","N"[S],STMNT.CREDACT)[M] -- whatever that does
        /// </summary>
        [UniVerseIndex(608)]
        [UniVerseFieldName("STMNT.CREDACT")]
        public string STMNT_CREDACT { get; set; }

        /// <summary>
        /// Inform on Promised Date Changed
        /// Mandatory (?) validates: 'N'o, 'P'rint, 'F'ax, 'E'mail
        /// Defaults: 'E'
        /// </summary>
        [UniVerseIndex(516)]
        [UniVerseFieldName("INFORM.DATE.DES")]
        public string INFORM_DATE_DES { get; set; }

        /// <summary>
        /// Store Name
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(594)]
        [UniVerseFieldName("STORE.NAME")]
        public string STORE_NAME { get; set; }

        /// <summary>
        /// Measure Pieces on Despatch
        /// Optional; 'Y', 'N' if given
        /// Defaults: IF(F("OEPARMS","1")<477> = "N", ""[S],<575>)[M]
        /// </summary>
        [UniVerseIndex(575)]
        [UniVerseFieldName("MEASURE.PIECE")]
        public string MEASURE_PIECE { get; set; }

        /// <summary>
        /// Transfer Address (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(425)]
        [UniVerseFieldName("TRAN.ADDR")]
        public string TRAN_ADDR { get; set; }

        /// <summary>
        /// Transfer Password (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(427)]
        [UniVerseFieldName("TRAN.PWD")]
        public string TRAN_PWD { get; set; }

        /// <summary>
        /// Retail Till Customer
        /// Optional; 'Y' or 'N' if given
        /// Defaults: 'N'
        /// </summary>
        [UniVerseIndex(650)]
        [UniVerseFieldName("RETAIL.TILL.CUST")]
        public string RETAIL_TILL_CUST { get; set; }

        /// <summary>
        /// End Customer Required ?
        /// Validates: 'Y' or 'N' if given
        /// </summary>
        [UniVerseIndex(790)]
        [UniVerseFieldName("END.CUST.REQ.DES")]
        public string END_CUST_REQ_DES { get; set; }

        /// <summary>
        /// Default Order Priority
        /// Mandatory; validates {0:9}
        /// Defaults: 1
        /// </summary>
        [UniVerseIndex(320)]
        [UniVerseFieldName("DEF.ORDER.PRIORITY")]
        public string DEF_ORDER_PRIORITY { get; set; }

        /// <summary>
        /// Full Order Only (?)
        /// Optional; 'Y' if given
        /// Defaults: ''
        /// </summary>
        [UniVerseIndex(555)]
        [UniVerseFieldName("SO.ALLOC.FULL")]
        public string SO_ALLOC_FULL { get; set; }

        /// <summary>
        /// Send Invoice to Delivery Address
        /// Optional; validates [OE].[PG.INV.TO.DELADD.VAL]
        ///                     'Y', 'N'
        ///                     Shows warning if set to 'Y': INVOICES WILL BE SENT TO DELIVERY ADDRESS
        /// Defaults: 'N'
        /// </summary>
        [UniVerseIndex(544)]
        [UniVerseFieldName("INV.TO.DELIVERY")]
        public string INV_TO_DELIVERY { get; set; }

        /// <summary>
        /// Print Unit Price on Delivery Note?
        /// Mandatory; validates 'Y', 'N'
        /// Defaults: 'N'
        /// </summary>
        [UniVerseIndex(540)]
        [UniVerseFieldName("DELNOTE.PRICES")]
        public string DELNOTE_PRICES { get; set; }

        /// <summary>
        /// Order Category
        /// Optional; validates against ORD.CATAGORY.DES if given
        /// </summary>
        [UniVerseIndex(698)]
        [UniVerseFieldName("ORDER.CATAGORY.DES")]
        public string ORDER_CATAGORY_DES { get; set; }

        /// <summary>
        /// Retail or Contract
        /// Optional; validates '', 'R'etail or 'C'ontract if given
        /// </summary>
        [UniVerseIndex(699)]
        [UniVerseFieldName("RET.CONT.DES")]
        public string RET_CONT_DES { get; set; }

        /// <summary>
        /// Haulier for Consolidated Invoice
        /// Optional; validates against OEHAULIER if given
        /// </summary>
        [UniVerseIndex(752)]
        [UniVerseFieldName("HAULIER.CONSOLIDATE.DES")]
        public string HAULIER_CONSOLIDATE_DES { get; set; }

        /// <summary>
        /// New Picking List for Goods In / Problem Items
        /// Optional; 'Y' or 'N' if given
        /// </summary>
        [UniVerseIndex(762)]
        [UniVerseFieldName("NEWPL.FOR.GIPROB.DES")]
        public string NEWPL_FOR_GIPROB_DES { get; set; }

        /// <summary>
        /// MTM Carriage Charged (?)
        /// Optional; 'Y' or 'N' if given
        /// Defaults: 'Y'
        /// </summary>
        [UniVerseIndex(758)]
        [UniVerseFieldName("MTM.CARR.CHARGED.DES")]
        public string MTM_CARR_CHARGED_DES { get; set; }

        /// <summary>
        /// Cut Off Value
        /// Optional; validates {0:999999999} if given
        /// Defaults: IF(MTM.CARR.CHARGED.DES = "N",""[S],<759>)[M]
        /// </summary>
        [UniVerseIndex(759)]
        [UniVerseFieldName("CUT.OFF.VALUE.DES")]
        public string CUT_OFF_VALUE_DES { get; set; }

        /// <summary>
        /// UPS Add On Percentage (?)
        /// Optional; validates {0:999999} if given "must be in range 0 to 999.9"
        /// THIS MEANS THAT MOST IF NOT ALL OF THEIR NUMBER VALUES ARE HELD TO THIS THOUSAND MULTIPLIER THING
        /// </summary>
        [UniVerseIndex(906)]
        [UniVerseFieldName("UPS.ADD.ON.DES")]
        public string UPS_ADD_ON_DES { get; set; }

        /// <summary>
        /// Allocate Stock for Proforma Orders
        /// Optional; 'Y' or 'N' if given
        /// </summary>
        [UniVerseIndex(898)]
        [UniVerseFieldName("ALLOC.PROFORMA.DES")]
        public string ALLOC_PROFORMA_DES { get; set; }

        /// <summary>
        /// Print Invoice at Confirmation
        /// Optional; 'N', 'T', 'A' if given
        /// </summary>
        [UniVerseIndex(907)]
        [UniVerseFieldName("PRINT.INV.AT.CONF.DES")]
        public string PRINT_INV_AT_CONF_DES { get; set; }

        /// <summary>
        /// Days Forward for Reservations
        /// Optional; validates [OE].[PG.VAL.RESDAYS] if given
        ///                     must be either '' or >= 2
        /// </summary>
        [UniVerseIndex(908)]
        [UniVerseFieldName("RESERVE.DAYS.DES")]
        public string RESERVE_DAYS_DES { get; set; }

        /// <summary>
        /// Print JLP Ean Labels (?)
        /// No idea what this does
        /// Optional; 'Y' or 'N' if given
        /// </summary>
        [UniVerseIndex(916)]
        [UniVerseFieldName("JLP.LABELS.DES")]
        public string JLP_LABELS_DES { get; set; }

        /// <summary>
        /// Rf Scanner Memo (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(729)]
        [UniVerseFieldName("RF.SCANNER.MEMO.DES")]
        public string RF_SCANNER_MEMO_DES { get; set; }

        /// <summary>
        /// Confirmation Memo (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(730)]
        [UniVerseFieldName("CONFIRMATION.MEMO.DES")]
        public string CONFIRMATION_MEMO_DES { get; set; }

        /// <summary>
        /// Site-Specific Field 1 (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(1016)]
        [UniVerseFieldName("SS.FIELD1")]
        public string SS_FIELD1 { get; set; }

        /// <summary>
        /// Site-Specific Field 2 (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(1017)]
        [UniVerseFieldName("SS.FIELD2")]
        public string SS_FIELD2 { get; set; }

        /// <summary>
        /// Site-Specific Field 3 (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(1018)]
        [UniVerseFieldName("SS.FIELD3")]
        public string SS_FIELD3 { get; set; }

        /// <summary>
        /// Site-Specific Field 4 (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(1019)]
        [UniVerseFieldName("SS.FIELD4")]
        public string SS_FIELD4 { get; set; }

        /// <summary>
        /// Site-Specific Field 5 (?)
        /// This was, once-upon-a-time, where Sarah wanted me to store EORI numbers. In error.
        /// </summary>
        [UniVerseIndex(1020)]
        [UniVerseFieldName("SS.FIELD5")]
        public string SS_FIELD5 { get; set; }

        /// <summary>
        /// Contract / Display (?)
        /// Why does this live on SLCUST...?
        /// Optional; validates 'D'isplay, 'C'ontract, 'N'ormal
        /// </summary>
        [UniVerseIndex(287)]
        [UniVerseFieldName("CONTRACT.ORDER")]
        public string CONTRACT_ORDER { get; set; }

        /// <summary>
        /// Current Advertiser (?)
        /// No idea what this does
        /// Optional; validates 'Y' or 'N' if given
        /// </summary>
        [UniVerseIndex(727)]
        [UniVerseFieldName("ADVERT.DES")]
        public string ADVERT_DES { get; set; }

        /// <summary>
        /// Stockist? (?)
        /// No idea what this does
        /// Optional; does not validate
        /// Defaults: IF (SEL1 = "",""[S],IF(REP.STOCKIST # "", REP.STOCKIST,"N"))[M]
        /// </summary>
        [UniVerseIndex(255)]
        [UniVerseFieldName("REP.STOCKIST")]
        public string REP_STOCKIST { get; set; }

        /// <summary>
        /// Stockist Category (?)
        /// No idea what this does
        /// Optional; validates '' or against SL.STOCKIST.CAT.CATEGORY if given
        /// </summary>
        [UniVerseIndex(706)]
        [UniVerseFieldName("SKIST.CAT")]
        public string SKIST_CAT { get; set; }

        /// <summary>
        /// UPS Returns Label (?)
        /// No idea what this does
        /// Optional; 'Y' or 'N' if given
        /// </summary>
        [UniVerseIndex(824)]
        [UniVerseFieldName("UPS.RET.LAB")]
        public string UPS_RET_LAB { get; set; }

        /// <summary>
        /// Exit Code Email (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(948)]
        [UniVerseFieldName("EXIT.CODE.EMAIL")]
        public string EXIT_CODE_EMAIL { get; set; }

        /// <summary>
        /// "No Email" (?)
        /// No idea what this does
        /// </summary>
        [UniVerseIndex(849)]
        [UniVerseFieldName("CALLED")]
        public string CALLED { get; set; }

        /// <summary>
        /// Memorandum
        /// </summary>
        [UniVerseIndex(8)]
        [UniVerseFieldName("MEMO")]
        public string MEMO { get; set; }

        public TradeAccount ()
        {
            SSLPOSTJSON = new List<SSLPostDocument>();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
