using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.Utilities;
using Newtonsoft.Json;

namespace MertexerV2.Models.PurchaseOrder
{
    [UniVerseFileName("POHEAD")]
    public class PurchaseOrderHead : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public PurchaseOrderHead(string key) : base(key)
        {
            Initialise();
        }

        public PurchaseOrderHead() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(PurchaseOrderHead).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("LOCATION")]
        [UniVerseConversion("")]
        public string StockLocationCode { get; set; }

        public DateTime? DateOnFileDT { get; set; }

        [UniVerseIndex(2)]
        [UniVerseFieldName("SS.F2")]
        [UniVerseConversion("")]
        public string DateOnFile
        {
            get
            {
                if (DateOnFileDT.HasValue)
                {
                    return Framework.UniVerseDateString(DateOnFileDT.Value);
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
                        DateOnFileDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        DateOnFileDT = null;
                    }
                }
            }
        }

        [UniVerseIndex(3)]
        [UniVerseFieldName("SUPPLIER")]
        [UniVerseConversion("")]
        public string SupplierCode { get; set; }

        [UniVerseIndex(7)]
        [UniVerseFieldName("ORDERSTATUS")]
        [UniVerseConversion("")]
        public byte? PurchaseOrderStatusID { get; set; }

        [UniVerseIndex(8, 1)]
        [UniVerseFieldName("DEL.NAME")]
        [UniVerseConversion("")]
        public string DeliveryName { get; set; }

        [UniVerseIndex(8, 2)]
        [UniVerseFieldName("DEL.ADDR1")]
        [UniVerseConversion("")]
        public string DeliveryAddressLine1 { get; set; }

        [UniVerseIndex(8, 3)]
        [UniVerseFieldName("DEL.ADDR2")]
        [UniVerseConversion("")]
        public string DeliveryAddressLine2 { get; set; }

        [UniVerseIndex(8, 4)]
        [UniVerseFieldName("DEL.ADDR3")]
        [UniVerseConversion("")]
        public string DeliveryTown { get; set; }

        [UniVerseIndex(8, 5)]
        [UniVerseFieldName("DEL.ADDR4")]
        [UniVerseConversion("")]
        public string DeliveryCounty { get; set; }

        [UniVerseIndex(8, 6)]
        [UniVerseFieldName("DEL.PC")]
        [UniVerseConversion("")]
        public string DeliveryPostcode { get; set; }

        [UniVerseIndex(8, 7)]
        [UniVerseFieldName("DEL.PHONE")]
        [UniVerseConversion("")]
        public string DeliveryPhone { get; set; }

        [UniVerseIndex(12)]
        [UniVerseFieldName("AUTHOR.CODE")]
        [UniVerseConversion("")]
        public string AuthoriserCode { get; set; }

        [UniVerseIndex(31)]
        [UniVerseFieldName("SORDNO")]
        [UniVerseConversion("")]
        public string OrderID { get; set; }

        [UniVerseIndex(59)]
        [UniVerseFieldName("ORDERPROCESS")]
        [UniVerseConversion("")]
        public string ProcessCode { get; set; }

        [UniVerseIndex(222)]
        [UniVerseFieldName("CALLOFF.DES")]
        [UniVerseConversion("")]
        public bool CallOffIndicator { get; set; }
    }
}
