using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.Utilities;

using Newtonsoft.Json;

namespace MertexerV2.Models.GeneralLedger
{
    [UniVerseFileName("GLBATCHDATA")]
    public class GeneralLedgerBatchData : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public GeneralLedgerBatchData(string key) : base(key)
        {
            Initialise();
        }

        public GeneralLedgerBatchData() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(GeneralLedgerBatchData).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        [UniVerseIndex(2)]
        [UniVerseFieldName("TYPE")]
        public string Type { get; set; }

        [UniVerseIndex(3)]
        [UniVerseFieldName("DOCREF")]
        public string DocumentNumber { get; set; }

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


        [JsonAlsoUpdates("PostDate")]
        public DateTime? PostDT { get; set; }

        [UniVerseIndex(5)]
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

        [UniVerseIndex(6)]
        [UniVerseFieldName("ITEMREF")]
        public string ItemReference { get; set; }

        [UniVerseIndex(7)]
        [UniVerseFieldName("SUPREF")]
        public string SupplierCode { get; set; }

        [UniVerseIndex(9)]
        [UniVerseFieldName("SS.AMOUNT")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> Amounts { get; set; }

        [UniVerseIndex(10)]
        [UniVerseFieldName("GLCODE")]
        [UniVerseMultiValue]
        public IEnumerable<string> GeneralLedgerCodes { get; set; }

        [UniVerseIndex(13)]
        [UniVerseFieldName("SS.VATTAX")]
        [UniVerseConversion("MR22")]
        public decimal VATTax { get; set; }

        [UniVerseIndex(14)]
        [UniVerseFieldName("SS.DOCTOT")]
        [UniVerseConversion("MR22")]
        public decimal DocumentTotal { get; set; }

        [UniVerseIndex(15)]
        [UniVerseFieldName("CUSNAME")]
        public string SupplierName { get; set; }

        [UniVerseIndex(19)]
        [UniVerseFieldName("MEMO")]
        public string Memo { get; set; }

        [UniVerseIndex(32)]
        [UniVerseFieldName("GLSUB")]
        [UniVerseMultiValue]
        public IEnumerable<string> GeneralLedgerSubCodes { get; set; }

        [UniVerseIndex(36)]
        [UniVerseFieldName("PREFIX")]
        public string Prefix { get; set; }

        [UniVerseIndex(37)]
        [UniVerseFieldName("TRANSTYPE")]
        public string TransactionType { get; set; }

        [UniVerseIndex(40)]
        [UniVerseFieldName("MEMO1")]
        [UniVerseMultiValue]
        public IEnumerable<string> OtherMemos { get; set; }
    }
}
