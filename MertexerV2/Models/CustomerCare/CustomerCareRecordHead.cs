using MertexerV2.AccountFunctions;
using MertexerV2.ProductFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.CustomerCare
{
    [UniVerseFileName("TCCHEAD")]
    public class CustomerCareRecordHead : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public CustomerCareRecordHead(string key) : base(key) 
        {
            Initialise();
        }

        public CustomerCareRecordHead() : base() 
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(CustomerCareRecordHead).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            CustomerCareRecordFunctions.UpsertHead(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("COLREF")]
        public string CollectionReference { get; set; }

        [UniVerseIndex(2)]
        [UniVerseFieldName("COLNAME")]
        public string CollectionName { get; set; }

        [UniVerseIndex(3)]
        [UniVerseMultiValue]
        [UniVerseFieldName("COLADDR")]
        public IEnumerable<string> CollectionAddress { get; set; }

        [UniVerseIndex(3, 1)]
        [UniVerseFieldName("COLADDR1")]
        public string CollectionAddressLine1 { get; set; }

        [UniVerseIndex(3, 2)]
        [UniVerseFieldName("COLADDR2")]
        public string CollectionAddressLine2 { get; set; }

        [UniVerseIndex(3, 3)]
        [UniVerseFieldName("COLADDR3")]
        public string CollectionAddressTown { get; set; }

        [UniVerseIndex(3, 4)]
        [UniVerseFieldName("COLADDR4")]
        public string CollectionAddressCounty { get; set; }

        [UniVerseIndex(3, 5)]
        [UniVerseFieldName("COLPCODE")]
        public string CollectionAddressPostcode { get; set; }

        [UniVerseIndex(4)]
        [UniVerseFieldName("CONTACT1")]
        public string ContactName { get; set; }

        public DateTime? CreatedDT { get; set; }

        /// <summary>
        /// Clear Date
        /// </summary>
        [UniVerseIndex(5)]
        [UniVerseFieldName("CLEARDATE")]
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

        [UniVerseIndex(6)]
        [UniVerseFieldName("CLAIMTAKER")]
        public string ClaimTaker { get; set; }

        [UniVerseIndex(7)]
        [UniVerseFieldName("HAULIERCODE")]
        public string HaulierCode { get; set; }

        [UniVerseIndex(8)]
        [UniVerseMultiValue]
        [UniVerseFieldName("CLINKS")]
        public IEnumerable<string> ClaimLineKeys { get; set; }

        [UniVerseIndex(8, 1)]
        [UniVerseFieldName("CLINK1")]
        public string FirstClaimLine { get; set; }

        [UniVerseIndex(9)]
        [UniVerseFieldName("CUSREF")]
        public string TradeAccountCode { get; set; }

        [UniVerseIndex(10)]
        [UniVerseFieldName("PRINTED")]
        public bool Printed { get; set; }

        [UniVerseIndex(11)]
        [UniVerseFieldName("SPRICE.NO")]
        public decimal? SellingPrice { get; set; } 
        
        [UniVerseIndex(12)]
        [UniVerseFieldName("LOCCODE")]
        public string StockLocationCode { get; set; }

        [UniVerseIndex(13)]
        [UniVerseFieldName("TERMS")]
        public string Terms { get; set; }

        [UniVerseIndex(14)]
        [UniVerseFieldName("RESULT")]
        public byte? ResultCode { get; set; }

        [UniVerseIndex(15)]
        [UniVerseFieldName("COMPLETED")]
        public bool Completed { get; set; }

        [UniVerseIndex(16)]
        [UniVerseFieldName("CREDIT.NOTE")]
        public string CreditNote { get; set; }

        [UniVerseIndex(17)]
        [UniVerseFieldName("ONTIME")]
        public string EntryTime { get; set; }

        [UniVerseIndex(18)]
        [UniVerseFieldName("DETAILS")]
        public string Details { get; set; }

        [UniVerseIndex(19)]
        [UniVerseFieldName("UPLIFTREQ")]
        public bool UpliftRequired { get; set; }

        [UniVerseIndex(20)]
        [UniVerseFieldName("CURRENTPERSON")]
        public string AssignedToPerson { get; set; }

        [UniVerseIndex(21)]
        [UniVerseFieldName("CURRENTSECTION")]
        public string AssignedToDepartment { get; set; }

        public DateTime? AssignedDT { get; set; }

        [UniVerseIndex(22)]
        [UniVerseFieldName("CURALDATE")]
        public string AssignedDate
        {
            get
            {
                if (AssignedDT.HasValue)
                {
                    return Framework.UniVerseDateString(AssignedDT.Value);
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
                        AssignedDT = Framework.DateTimeFromUniVerseOffset(valInt);

                        if (TimeSpan.TryParse(AssignedTime, out TimeSpan ts))
                        {
                            AssignedDT += ts;
                        }
                    }
                    else
                    {
                        AssignedDT = null;
                    }
                }
            }
        }
        
        [UniVerseIndex(23)]
        [UniVerseFieldName("CURALTIME")]
        public string AssignedTime
        {
            get
            {
                if (AssignedDT.HasValue)
                {
                    return AssignedDT.Value.TimeOfDay.ToString();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!String.IsNullOrEmpty(value) && AssignedDT.HasValue)
                {
                    if (TimeSpan.TryParse(AssignedTime, out TimeSpan ts))
                    {
                        AssignedDT += ts;
                    }
                }
            }
        }

        [UniVerseIndex(24)]
        [UniVerseMultiValue]
        [UniVerseFieldName("ALINKS")]
        public IEnumerable<string> ActionKeys { get; set; }

        [UniVerseIndex(25)]
        [UniVerseMultiValue]
        [UniVerseFieldName("INSPECTION")]
        public IEnumerable<string> InspectionReportLines { get; set; }

        [UniVerseIndex(26)]
        [UniVerseFieldName("CREDITREQ")]
        public bool CreditRequired { get; set; }

        [UniVerseIndex(27)]
        [UniVerseFieldName("SIGNEDOFF")]
        public bool SignedOff { get; set; }

        [UniVerseIndex(28)]
        [UniVerseMultiValue]
        [UniVerseFieldName("DEL.ADDRESS")]
        public IEnumerable<string> DeliveryAddress { get; set; }

        [UniVerseIndex(29)]
        [UniVerseFieldName("RET.METHOD")]
        public string ReturnMethod { get; set; }

        [UniVerseIndex(30)]
        [UniVerseFieldName("DOC.NO")]
        public string DocumentNumber { get; set; }

        [UniVerseIndex(31)]
        [UniVerseFieldName("SUPREF")]
        public string SupplierReference { get; set; }

        [UniVerseIndex(32)]
        [UniVerseFieldName("DEBIT.REQ")]
        public bool DebitRequired { get; set; }

        [UniVerseIndex(33)]
        [UniVerseFieldName("DEBIT.NOTE")]
        public string DebitNote { get; set; }

        [UniVerseIndex(34)]
        [UniVerseMultiValue]
        [UniVerseFieldName("CARRIER.REF")]
        public IEnumerable<string> CarrierReferences { get; set; }

        [UniVerseIndex(34, 1)]
        [UniVerseFieldName("CARRIER.REF1")]
        public string CarrierReference_First { get; set; }

        [UniVerseIndex(35)]
        [UniVerseFieldName("CUS.OWN.REF")]
        public string CustomersReference { get; set; }

        [UniVerseIndex(36)]
        [UniVerseMultiValue]
        [UniVerseFieldName("PRINT.FLD")]
        public IEnumerable<string> PrintFieldLines { get; set; }

        [UniVerseIndex(37)]
        [UniVerseFieldName("SELLCO")]
        public string SellingCompany { get; set; }

        [UniVerseIndex(38)]
        [UniVerseFieldName("COMP.PRINTED")]
        public bool CompPrinted { get; set; }

        [UniVerseIndex(62)]
        [UniVerseFieldName("FROM.WEB.DES")]
        public bool FromWeb { get; set; }

        [UniVerseIndex(63)]
        [UniVerseFieldName("CONTACT.PHONE.DES")]
        public string ContactPhone { get; set; }

        [UniVerseIndex(64)]
        [UniVerseFieldName("CONTACT.EMAIL.DES")]
        public string ContactEmail { get; set; }

        [UniVerseIndex(209)]
        [UniVerseMultiValue]
        [UniVerseFieldName("CUSADDR")]
        public IEnumerable<string> CustomerAddressLines { get; set; }

        [UniVerseIndex(209, 1)]
        [UniVerseFieldName("CUSNAME.FF")]
        public string CustomerName { get; set; }

        [UniVerseIndex(209, 2)]
        [UniVerseFieldName("CUSADDR1.FF")]
        public string CustomerAddressLine1 { get; set; }

        [UniVerseIndex(209, 3)]
        [UniVerseFieldName("CUSADDR2.FF")]
        public string CustomerAddressLine2 { get; set; }

        [UniVerseIndex(209, 4)]
        [UniVerseFieldName("CUSADDR3.FF")]
        public string CustomerAddressTown { get; set; }

        [UniVerseIndex(209, 5)]
        [UniVerseFieldName("CUSADDR4.FF")]
        public string CustomerAddressCounty { get; set; }

        [UniVerseIndex(209, 6)]
        [UniVerseFieldName("CUSPCODE.FF")]
        public string CustomerAddressPostcode { get; set; }

        [UniVerseIndex(210)]
        [UniVerseMultiValue]
        [UniVerseFieldName("LINK.DOCS")]
        public IEnumerable<string> LinkedDocuments { get; set; }

        [UniVerseIndex(211)]
        [UniVerseMultiValue]
        [UniVerseFieldName("LINK.DOCS.DESC")]
        public IEnumerable<string> LinkedDocumentNames { get; set; }

        [UniVerseIndex(213)]
        [UniVerseMultiValue]
        [UniVerseFieldName("CCALLNOS")]
        public IEnumerable<string> CallNumbers { get; set; }

        [UniVerseIndex(242)]
        [UniVerseFieldName("ENDCUST")]
        public string EndCustomer { get; set; }

        [UniVerseIndex(254)]
        [UniVerseFieldName("SORDERNO")]
        public string OrderID { get; set; }

        [UniVerseIndex(257)]
        [UniVerseFieldName("INCCARRIAGE")]
        public bool IncludeCarriage { get; set; }

        [UniVerseIndex(258)]
        [UniVerseFieldName("CARRIAGE")]
        [UniVerseConversion("MR22")]
        public decimal? CarriageValue { get; set; }

        [UniVerseIndex(259)]
        [UniVerseFieldName("RECOUPHAND")]
        public bool RecoupHandling { get; set; }

        [UniVerseIndex(260)]
        [UniVerseFieldName("RECOUPPCNT")]
        [UniVerseConversion("MR12")]
        public decimal? RecoupPercentage { get; set; }

        [UniVerseIndex(261)]
        [UniVerseMultiValue]
        [UniVerseFieldName("RETURNNOS")]
        public IEnumerable<string> ReturnNumbers { get; set; }

        [UniVerseIndex(263)]
        [UniVerseFieldName("SERV.CODE")]
        public string ServiceCode { get; set; }

        [UniVerseIndex(264)]
        [UniVerseMultiValue]
        [UniVerseFieldName("RETN.PCL.NOS")]
        public IEnumerable<string> ReturnParcelNumbers { get; set; }

        [UniVerseIndex(265)]
        [UniVerseMultiValue]
        [UniVerseFieldName("ORDERS.AAR.DES")]
        public IEnumerable<string> ResultantOrderIDs { get; set; }

        [UniVerseIndex(265, 1)]
        [UniVerseFieldName("ORDERS.AAR1.DES")]
        public string ResultantOrderID_First { get; set; }

        [UniVerseIndex(266)]
        [UniVerseMultiValue]
        [UniVerseFieldName("INVOICES.AAR.DES")]
        public IEnumerable<string> ResultantInvoiceNumbers { get; set; }

        [UniVerseIndex(266, 1)]
        [UniVerseFieldName("INVOICES.AAR1.DES")]
        public string ResultantInvoiceNumber_First { get; set; }

        [UniVerseIndex(269)]
        [UniVerseFieldName("TO.COUNTRY")]
        public string Country { get; set; }

        [UniVerseIndex(270)]
        [UniVerseFieldName("RTN.PRINTED")]
        public bool ReturnPrinted { get; set; }

        [UniVerseIndex(270)]
        [UniVerseFieldName("FOC.REQUESTED.DES")]
        public bool FreeOfChargeRequested { get; set; }

        [UniVerseIndex(300)]
        [UniVerseMultiValue]
        [UniVerseFieldName("RD.CC")]
        public IEnumerable<string> RelatedCustomerCareLineKeys { get; set; }
    }
}
