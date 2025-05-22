using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.Utilities;

using Newtonsoft.Json;

namespace MertexerV2.Models.PurchaseOrder
{
    /*
        "DETAILKEY",
        "SORDNO",
        "PRODCODE",
        "CONV.QTY",
        "RCVDQTY",
        "SS.DATEREQUIRED.INT",
        "SS.DATEPROMISED.INT",
        "SS.EXPARRIVAL.INT",
        "DATE.CONFIRMED",
        "DELETED",
        "LNCOMPLETE",
        "LINENO",
        "ORDERPRICE",
        "ORDPRICE.CUR",
        "CONV.PURCONV",
        "FINISHERS.REF",
        "SUBMIT",
        "SS.POHEADKEY",
        "PROM.CH.REASON"
     */

    [UniVerseFileName("PODETAIL")]
    public class PurchaseOrderDetail : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public PurchaseOrderDetail(string key) : base(key)
        {
            Initialise();
        }

        public PurchaseOrderDetail() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(PurchaseOrderDetail).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public string PurchaseOrderHeadID 
        { 
            get
            {
                return Key.Split('*')[0];
            }
        }

        [UniVerseForeignKey("PurchaseOrderHeadID", "MertexerV2.Models.PurchaseOrder.PurchaseOrderHead")]
        public virtual PurchaseOrderHead PurchaseOrderHead { get; set; }
        
        public string OrderID
        {
            get
            {
                return PurchaseOrderHead?.OrderID;
            }
        }

        [UniVerseIndex(3)]
        [UniVerseFieldName("PRODCODE")]
        public string ProductCode { get; set; }

        [UniVerseIndex(19)]
        [UniVerseFieldName("CONV.QTY")]
        [UniVerseConversion("MR13")]
        public decimal? QuantityOrdered { get; set; }

        [UniVerseIndex(12)]
        [UniVerseFieldName("RCVDQTY")]
        [UniVerseConversion("MR13")]
        public decimal? QuantityReceived { get; set; }

        public decimal? QuantityOutstanding 
        { 
            get 
            {
                return QuantityOrdered - QuantityReceived;
            } 
        }

        public DateTime? RequiredDT { get; set; }

        [UniVerseIndex(14)]
        [UniVerseFieldName("SS.DATEREQUIRED.INT")]
        public string RequiredDate
        {
            get
            {
                if (RequiredDT.HasValue)
                {
                    return Framework.UniVerseDateString(RequiredDT.Value);
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
                        RequiredDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        RequiredDT = null;
                    }
                }
            }
        }

        public DateTime? PromisedDT { get; set; }

        [UniVerseIndex(21)]
        [UniVerseFieldName("SS.DATEPROMISED.INT")]
        public string DatePromised
        {
            get
            {
                if (PromisedDT.HasValue)
                {
                    return Framework.UniVerseDateString(PromisedDT.Value);
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
                        PromisedDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        PromisedDT = null;
                    }
                }
            }
        }

        public DateTime? ExpectedArrivalDT { get; set; }

        [UniVerseIndex(257)]
        [UniVerseFieldName("SS.EXPARRIVAL.INT")]
        public string ExpectedArrivalDate
        {
            get
            {
                if (ExpectedArrivalDT.HasValue)
                {
                    return Framework.UniVerseDateString(ExpectedArrivalDT.Value);
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
                        ExpectedArrivalDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        ExpectedArrivalDT = null;
                    }
                }
            }
        }

        [UniVerseIndex(283, 1)]
        [UniVerseFieldName("DATE.CONFIRMED")]
        public string DateConfirmed { get; set; }

        [UniVerseIndex(30)]
        [UniVerseFieldName("LNCOMPLETE")]
        [UniVerseConversion("")]
        public bool LineComplete { get; set; }

        public short? LineNumber 
        {
            get
            {
                return Convert.ToInt16(Key.Split('*')[1]);
            }
        }

        [UniVerseIndex(5)]
        [UniVerseFieldName("ORDERPRICE")]
        [UniVerseConversion("MR44")]
        public decimal? LineUnitPrice { get; set; }

        [UniVerseIndex(33)]
        [UniVerseFieldName("ORDPRICE.CUR")]
        [UniVerseConversion("MR44")]
        public decimal? LineUnitPriceCurrency { get; set; }

        [UniVerseIndex(17)]
        [UniVerseFieldName("CONV.PURCONV")]
        [UniVerseConversion("")]
        public string StockUnitCode { get; set; }

        [UniVerseIndex(138)]
        [UniVerseFieldName("FINISHERS.REF")]
        public string SupplierProductCode { get; set; }

        [UniVerseIndex(74)]
        [UniVerseFieldName("SUBMIT")]
        public bool SampleRequired { get; set; }

        [UniVerseIndex(151)]
        [UniVerseFieldName("DELETED")]
        public bool Deleted { get; set; }

        [UniVerseIndex(179,1)]
        [UniVerseFieldName("PROM.CH.REASON")]
        public string StatusDescription { get; set; }

        [UniVerseIndex(5)]
        [UniVerseFieldName("L.ORDERPRICE")]
        [UniVerseConversion("MR44")]
        public decimal? OrderPrice { get; set; }

    }
}
