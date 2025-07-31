using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.Utilities;
using Newtonsoft.Json;

namespace MertexerV2.Models.PurchaseLedger
{
    [UniVerseFileName("PLSUPINV")]
    public class PurchaseLedgerSupplierInvoice : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public PurchaseLedgerSupplierInvoice(string key) : base(key)
        {
            Initialise();
        }

        public PurchaseLedgerSupplierInvoice() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(PurchaseLedgerSupplierInvoice).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("SUPREF")]
        public string SupplierCode { get; set; }

        [UniVerseIndex(2)]
        [UniVerseFieldName("SUPPINVNO")]
        public string SupplierInvoiceReference { get; set; }

        [UniVerseIndex(3)]
        [UniVerseFieldName("TRANTYPE")]
        public string TransactionType { get; set; }

        [JsonAlsoUpdates("DocumentDate")]
        public DateTime? DocumentDT { get; set; }

        [UniVerseIndex(4)]
        [UniVerseFieldName("SS.DOCDATE.INT")]
        public string DocumentDate
        {
            get
            {
                if (DocumentDT.HasValue)
                {
                    return Framework.UniVerseDateString(DocumentDT.Value);
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
                        DocumentDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        DocumentDT = null;
                    }
                }
            }
        }

        [UniVerseIndex(5)]
        [UniVerseFieldName("DOCNO")]
        public string DocumentReference { get; set; }

        [JsonAlsoUpdates("PostDate")]
        public DateTime? PostDT { get; set; }

        [UniVerseIndex(6)]
        [UniVerseFieldName("SS.POSTDATE.INT")]
        public string PostDate
        {
            get
            {
                if (PostDT.HasValue)
                {
                    return Framework.UniVerseDateString(PostDT.Value);
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
                        PostDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        PostDT = null;
                    }
                }
            }
        }
    }
}
