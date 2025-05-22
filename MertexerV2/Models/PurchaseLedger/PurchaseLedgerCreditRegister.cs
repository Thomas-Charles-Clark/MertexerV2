using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.Utilities;
using Newtonsoft.Json;

namespace MertexerV2.Models.PurchaseLedger
{
    [UniVerseFileName("PLCRNREG")]
    public class PurchaseLedgerCreditRegister : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public PurchaseLedgerCreditRegister(string key) : base(key)
        {
            Initialise();
        }

        public PurchaseLedgerCreditRegister() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(PurchaseLedgerCreditRegister).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public DateTime? DocumentDT { get; set; }

        /// <summary>
        /// Document Date
        /// </summary>
        [UniVerseIndex(33)]
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

        public DateTime? DueDT { get; set; }

        /// <summary>
        /// Due Date
        /// </summary>
        [UniVerseIndex(38)]
        [UniVerseFieldName("SS.DUEDATE.INT")]
        public string DueDate
        {
            get
            {
                if (DueDT.HasValue)
                {
                    return Framework.UniVerseDateString(DueDT.Value);
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
                        DueDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        DueDT = null;
                    }
                }
            }
        }

        [UniVerseIndex(37)]
        [UniVerseFieldName("ITEMREF")]
        public string ItemReference { get; set; }

        [UniVerseIndex(39)]
        [UniVerseFieldName("PSUPREF")]
        public string SupplierReference { get; set; }

        [UniVerseIndex(50)]
        [UniVerseFieldName("STGAMT")]
        [UniVerseConversion("MR22")]
        public decimal AmountSterling { get; set; }

        [UniVerseIndex(115)]
        [UniVerseFieldName("PTO")]
        public string PassedTo { get; set; }

        [UniVerseIndex(250)]
        [UniVerseFieldName("CURAMT")]
        [UniVerseConversion("MR22")]
        public decimal AmountCurrency { get; set; }
    }
}
