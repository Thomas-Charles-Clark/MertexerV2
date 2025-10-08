using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.PurchaseLedgerFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

namespace MertexerV2.Models.PurchaseLedger
{
    [UniVerseFileName("PLREGISTER.HIST")]
    public class PurchaseLedgerRegisterHistory : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public PurchaseLedgerRegisterHistory(string key) : base(key)
        {
            Initialise();
        }

        public PurchaseLedgerRegisterHistory() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(PurchaseLedgerRegisterHistory).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("REGNO")]
        public string RegisterNumber { get; set; }

        public DateTime? OnFileDT { get; set; }

        /// <summary>
        /// Date on File
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("SS.DATEONFILE.INT")]
        public string OnFileDate
        {
            get
            {
                if (OnFileDT.HasValue)
                {
                    return Framework.UniVerseDateString(OnFileDT.Value);
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
                        OnFileDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        OnFileDT = null;
                    }
                }
            }
        }

        [UniVerseIndex(3)]
        [UniVerseFieldName("SUPREF")]
        public string SupplierCode { get; set; }

        public DateTime? ConvertedDT { get; set; }

        /// <summary>
        /// Date on File
        /// </summary>
        [UniVerseIndex(4)]
        [UniVerseFieldName("SS.CONVDATE.INT")]
        public string ConvertedDate
        {
            get
            {
                if (ConvertedDT.HasValue)
                {
                    return Framework.UniVerseDateString(ConvertedDT.Value);
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
                        ConvertedDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        ConvertedDT = null;
                    }
                }
            }
        }

        [UniVerseIndex(5)]
        [UniVerseFieldName("TRANSACTION.NO")]
        public string DocumentReference { get; set; }

        public DateTime? DeletedDT { get; set; }

        /// <summary>
        /// Date on File
        /// </summary>
        [UniVerseIndex(6)]
        [UniVerseFieldName("SS.DELDATE.INT")]
        public string DeletedDate
        {
            get
            {
                if (DeletedDT.HasValue)
                {
                    return Framework.UniVerseDateString(DeletedDT.Value);
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
                        DeletedDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        DeletedDT = null;
                    }
                }
            }
        }

        [UniVerseIndex(7)]
        [UniVerseFieldName("USER.ID")]
        public string UserID { get; set; }

        [UniVerseIndex(8)]
        [UniVerseFieldName("TRANTYPE")]
        public string TransactionType { get; set; }

        [UniVerseIndex(9)]
        [UniVerseFieldName("DOCBAL")]
        [UniVerseConversion("MR22")]

        public decimal DocumentTotal { get; set; }
    }
}
