using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.Utilities;

namespace MertexerV2.Models.GeneralLedger
{
    [UniVerseFileName("GLBATCH")]
    public class GeneralLedgerBatch : UniVerseSerializationBase<string>
    {
        public GeneralLedgerBatch(string key) : base(key)
        {

        }

        public GeneralLedgerBatch() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(GeneralLedgerBatch).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("TYPE")]
        public string TransactionType { get; set; }

        [UniVerseIndex(2)]
        [UniVerseFieldName("STATUS")]
        public string Status { get; set; }

        [UniVerseIndex(3)]
        [UniVerseFieldName("BATCH.TOTS")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> BatchTotals { get; set; }

        [UniVerseIndex(4)]
        [UniVerseFieldName("ACCUM.TOTS")]
        [UniVerseConversion("MR22")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> AccumulatedTotals { get; set; }

        [UniVerseIndex(5)]
        [UniVerseFieldName("DATAKEYS")]
        [UniVerseMultiValue]
        public IEnumerable<string> DataKeys { get; set; }

        [UniVerseIndex(6)]
        [UniVerseFieldName("NORECS")]
        public int NumberOfRecords { get; set; }

        [JsonAlsoUpdates("BDATE")]
        public DateTime? BatchDT { get; set; }

        [UniVerseIndex(7)]
        [UniVerseFieldName("SS.BDATE.INT")]
        public string BatchDate
        {
            get
            {
                if (BatchDT.HasValue)
                {
                    return Framework.UniVerseDateString(BatchDT.Value);
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
                        BatchDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        BatchDT = null;
                    }
                }
            }
        }

        [UniVerseIndex(8)]
        [UniVerseFieldName("OPER")]
        public string Operator { get; set; }
    }
}
