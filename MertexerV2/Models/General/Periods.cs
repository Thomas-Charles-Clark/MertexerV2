using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.GeneralFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

namespace MertexerV2.Models.General
{
    [UniVerseFileName("PERIODS")]
    public class Periods : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public Periods(string key) : base(key)
        {
            Initialise();
        }

        public Periods() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(Periods).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// General Ledger Periods Last Year
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("SS.LASTYEAR.INT")]
        [UniVerseMultiValue]
        public IEnumerable<string> GeneralLedgerPeriodDatesLastYear
        {
            get
            {
                if (GeneralLedgerPeriodLastYearDTs.Any())
                {
                    return GeneralLedgerPeriodLastYearDTs.Select(dt => dt.HasValue ? Framework.UniVerseDateString(dt.Value) : null);
                }
                else
                {
                    return new List<string>();
                }
            }
            set
            {
                if (value != null)
                {
                    GeneralLedgerPeriodLastYearDTs = value.Select(v => (DateTime?)Framework.DateTimeFromUniVerseOffset(Convert.ToInt32(v)));
                }
                else
                {
                    GeneralLedgerPeriodLastYearDTs = new List<DateTime?>();
                }
            }
        }

        public IEnumerable<DateTime?> GeneralLedgerPeriodLastYearDTs { get; set; }

        /// <summary>
        /// General Ledger Periods This Year
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("SS.THISYEAR.INT")]
        [UniVerseMultiValue]
        public IEnumerable<string> GeneralLedgerPeriodDatesThisYear
        {
            get
            {
                if (GeneralLedgerPeriodDatesThisYearDTs.Any())
                {
                    return GeneralLedgerPeriodDatesThisYearDTs.Select(dt => Framework.UniVerseDateString(dt));
                }
                else
                {
                    return new List<string>();
                }
            }
            set
            {
                if (value != null)
                {
                    GeneralLedgerPeriodDatesThisYearDTs = value.Select(v => Framework.DateTimeFromUniVerseOffset(Convert.ToInt32(v)));
                }
                else
                {
                    GeneralLedgerPeriodDatesThisYearDTs = new List<DateTime>();
                }
            }
        }

        public IEnumerable<DateTime> GeneralLedgerPeriodDatesThisYearDTs { get; set; }

        /// <summary>
        /// General Ledger Periods This Year
        /// </summary>
        [UniVerseIndex(3)]
        [UniVerseFieldName("SS.NEXTYEAR.INT")]
        [UniVerseMultiValue]
        public IEnumerable<string> GeneralLedgerPeriodDatesNextYear
        {
            get
            {
                if (GeneralLedgerPeriodDatesNextYearDTs.Any())
                {
                    return GeneralLedgerPeriodDatesNextYearDTs.Select(dt => Framework.UniVerseDateString(dt));
                }
                else
                {
                    return new List<string>();
                }
            }
            set
            {
                if (value != null)
                {
                    GeneralLedgerPeriodDatesNextYearDTs = value.Select(v => Framework.DateTimeFromUniVerseOffset(Convert.ToInt32(v)));
                }
                else
                {
                    GeneralLedgerPeriodDatesNextYearDTs = new List<DateTime>();
                }
            }
        }


        public IEnumerable<DateTime> GeneralLedgerPeriodDatesNextYearDTs { get; set; }

        /// <summary>
        /// VAT Close?
        /// </summary>
        [UniVerseIndex(4)]
        [UniVerseFieldName("VATCLOSE")]
        [UniVerseMultiValue]
        public IEnumerable<bool> VATClose { get; set; }

        /// <summary>
        /// VAT Close Next Year?
        /// </summary>
        [UniVerseIndex(5)]
        [UniVerseFieldName("VATCLOSENEXT")]
        [UniVerseMultiValue]
        public IEnumerable<bool> VATCloseNextYear { get; set; }

        /// <summary>
        /// Sales Ledger Periods This Year
        /// </summary>
        [UniVerseIndex(6)]
        [UniVerseFieldName("SS.SL.THISYEAR.INT")]
        [UniVerseMultiValue]
        public IEnumerable<string> SalesLedgerPeriodDatesThisYear
        {
            get
            {
                if (SalesLedgerPeriodDatesThisYearDTs.Any())
                {
                    return SalesLedgerPeriodDatesThisYearDTs.Select(dt => Framework.UniVerseDateString(dt));
                }
                else
                {
                    return new List<string>();
                }
            }
            set
            {
                if (value != null)
                {
                    SalesLedgerPeriodDatesThisYearDTs = value.Select(v => Framework.DateTimeFromUniVerseOffset(Convert.ToInt32(v)));
                }
                else
                {
                    SalesLedgerPeriodDatesThisYearDTs = new List<DateTime>();
                }
            }
        }

        public IEnumerable<DateTime> SalesLedgerPeriodDatesThisYearDTs { get; set; }

        /// <summary>
        /// Sales Ledger Periods Next Year
        /// </summary>
        [UniVerseIndex(7)]
        [UniVerseFieldName("SS.SL.NEXTYEAR.INT")]
        [UniVerseMultiValue]
        public IEnumerable<string> SalesLedgerPeriodDatesNextYear
        {
            get
            {
                if (SalesLedgerPeriodDatesNextYearDTs.Any())
                {
                    return SalesLedgerPeriodDatesNextYearDTs.Select(dt => Framework.UniVerseDateString(dt));
                }
                else
                {
                    return new List<string>();
                }
            }
            set
            {
                if (value != null)
                {
                    SalesLedgerPeriodDatesNextYearDTs = value.Select(v => Framework.DateTimeFromUniVerseOffset(Convert.ToInt32(v)));
                }
                else
                {
                    SalesLedgerPeriodDatesNextYearDTs = new List<DateTime>();
                }
            }
        }

        public IEnumerable<DateTime> SalesLedgerPeriodDatesNextYearDTs { get; set; }

        /// <summary>
        /// Sales Ledger End of Period User
        /// </summary>
        [UniVerseIndex(8)]
        [UniVerseFieldName("SLPE.USER")]
        [UniVerseMultiValue]
        public IEnumerable<string> SalesLedgerPeriodEndUser { get; set; }

        /// <summary>
        /// Sales Ledger End of Period Times
        /// </summary>
        [UniVerseIndex(10)]
        [UniVerseFieldName("SS.SLPE.TIME.INT")]
        [UniVerseMultiValue]
        public IEnumerable<string> SalesLedgerPeriodEndTimes
        {
            get
            {
                if (SalesLedgerPeriodEndTimeSpans.Any())
                {
                    return SalesLedgerPeriodEndTimeSpans.Select(dt => dt.Seconds.ToString());
                }
                else
                {
                    return new List<string>();
                }
            }
            set
            {
                if (value != null)
                {
                    SalesLedgerPeriodEndTimeSpans = value.Select(v => TimeSpan.FromSeconds(Convert.ToInt32(v)));
                }
                else
                {
                    SalesLedgerPeriodEndTimeSpans = new List<TimeSpan>();
                }
            }
        }

        public IEnumerable<TimeSpan> SalesLedgerPeriodEndTimeSpans { get; set; }

        /// <summary>
        /// Sales Ledger End of Period DateTimes
        /// </summary>
        [UniVerseIndex(9)]
        [UniVerseFieldName("SS.SLPE.DATE.INT")]
        [UniVerseMultiValue]
        public IEnumerable<string> SalesLedgerPeriodEndDates
        {
            get
            {
                if (SalesLedgerPeriodEndDTs.Any())
                {
                    return SalesLedgerPeriodEndDTs.Select(dt => Framework.UniVerseDateString(dt.Date));
                }
                else
                {
                    return new List<string>();
                }
            }
            set
            {
                if (value != null)
                {
                    if (SalesLedgerPeriodEndTimeSpans.Any())
                    {
                        SalesLedgerPeriodEndDTs = value.Select((v, i) => Framework.DateTimeFromUniVerseOffset(Convert.ToInt32(v)) + (SalesLedgerPeriodEndTimeSpans.ElementAtOrDefault(i)));
                    }
                    else
                    {
                        SalesLedgerPeriodEndDTs = value.Select(v => Framework.DateTimeFromUniVerseOffset(Convert.ToInt32(v)));
                    }
                }
                else
                {
                    SalesLedgerPeriodEndDTs = new List<DateTime>();
                }
            }
        }

        public IEnumerable<DateTime> SalesLedgerPeriodEndDTs { get; set; }

        /// <summary>
        /// Sales Ledger End of Period Statuses
        /// </summary>
        [UniVerseIndex(11)]
        [UniVerseFieldName("SLPE.STATUS")]
        [UniVerseMultiValue]
        public IEnumerable<short> SalesLedgerPeriodEndStatuses { get; set; }

        /// <summary>
        /// Purchase Ledger End of Period User
        /// </summary>
        [UniVerseIndex(12)]
        [UniVerseFieldName("PLPE.USER")]
        [UniVerseMultiValue]
        public IEnumerable<string> PurchaseLedgerPeriodEndUser { get; set; }

        /// <summary>
        /// Purchase Ledger End of Period Times
        /// </summary>
        [UniVerseIndex(14)]
        [UniVerseFieldName("SS.PLPE.TIME.INT")]
        [UniVerseMultiValue]
        public IEnumerable<string> PurchaseLedgerPeriodEndTimes
        {
            get
            {
                if (PurchaseLedgerPeriodEndTimeSpans.Any())
                {
                    return PurchaseLedgerPeriodEndTimeSpans.Select(dt => dt.Seconds.ToString());
                }
                else
                {
                    return new List<string>();
                }
            }
            set
            {
                if (value != null)
                {
                    PurchaseLedgerPeriodEndTimeSpans = value.Select(v => TimeSpan.FromSeconds(Convert.ToInt32(v)));
                }
                else
                {
                    PurchaseLedgerPeriodEndTimeSpans = new List<TimeSpan>();
                }
            }
        }

        public IEnumerable<TimeSpan> PurchaseLedgerPeriodEndTimeSpans { get; set; }

        /// <summary>
        /// Purchase Ledger End of Period DateTimes
        /// </summary>
        [UniVerseIndex(13)]
        [UniVerseFieldName("SS.PLPE.DATE.INT")]
        [UniVerseMultiValue]
        public IEnumerable<string> PurchaseLedgerPeriodEndDates
        {
            get
            {
                if (PurchaseLedgerPeriodEndDTs.Any())
                {
                    return PurchaseLedgerPeriodEndDTs.Select(dt => Framework.UniVerseDateString(dt.Date));
                }
                else
                {
                    return new List<string>();
                }
            }
            set
            {
                if (value != null)
                {
                    if (PurchaseLedgerPeriodEndTimeSpans.Any())
                    {
                        PurchaseLedgerPeriodEndDTs = value.Select((v, i) => Framework.DateTimeFromUniVerseOffset(Convert.ToInt32(v)) + (PurchaseLedgerPeriodEndTimeSpans.ElementAtOrDefault(i)));
                    }
                    else
                    {
                        PurchaseLedgerPeriodEndDTs = value.Select(v => Framework.DateTimeFromUniVerseOffset(Convert.ToInt32(v)));
                    }
                }
                else
                {
                    PurchaseLedgerPeriodEndDTs = new List<DateTime>();
                }
            }
        }

        public IEnumerable<DateTime> PurchaseLedgerPeriodEndDTs { get; set; }

        /// <summary>
        /// Purchase Ledger End of Period Statuses
        /// </summary>
        [UniVerseIndex(15)]
        [UniVerseFieldName("PLPE.STATUS")]
        [UniVerseMultiValue]
        public IEnumerable<short> PurchaseLedgerPeriodEndStatuses { get; set; }

        /// <summary>
        /// Cash Book End of Period User
        /// </summary>
        [UniVerseIndex(16)]
        [UniVerseFieldName("CBPE.USER")]
        [UniVerseMultiValue]
        public IEnumerable<string> CashBookPeriodEndUser { get; set; }

        /// <summary>
        /// Cash Book End of Period Times
        /// </summary>
        [UniVerseIndex(14)]
        [UniVerseFieldName("SS.CBPE.TIME.INT")]
        [UniVerseMultiValue]
        public IEnumerable<string> CashBookPeriodEndTimes
        {
            get
            {
                if (CashBookPeriodEndTimeSpans.Any())
                {
                    return CashBookPeriodEndTimeSpans.Select(dt => dt.Seconds.ToString());
                }
                else
                {
                    return new List<string>();
                }
            }
            set
            {
                if (value != null)
                {
                    CashBookPeriodEndTimeSpans = value.Select(v => TimeSpan.FromSeconds(Convert.ToInt32(v)));
                }
                else
                {
                    CashBookPeriodEndTimeSpans = new List<TimeSpan>();
                }
            }
        }

        public IEnumerable<TimeSpan> CashBookPeriodEndTimeSpans { get; set; }

        /// <summary>
        /// Cash Book End of Period DateTimes
        /// </summary>
        [UniVerseIndex(17)]
        [UniVerseFieldName("SS.CBPE.DATE.INT")]
        [UniVerseMultiValue]
        public IEnumerable<string> CashBookPeriodEndDates
        {
            get
            {
                if (CashBookPeriodEndDTs.Any())
                {
                    return CashBookPeriodEndDTs.Select(dt => Framework.UniVerseDateString(dt.Date));
                }
                else
                {
                    return new List<string>();
                }
            }
            set
            {
                if (value != null)
                {
                    if (CashBookPeriodEndTimeSpans.Any())
                    {
                        CashBookPeriodEndDTs = value.Select((v, i) => Framework.DateTimeFromUniVerseOffset(Convert.ToInt32(v)) + (CashBookPeriodEndTimeSpans.ElementAtOrDefault(i)));
                    }
                    else
                    {
                        CashBookPeriodEndDTs = value.Select(v => Framework.DateTimeFromUniVerseOffset(Convert.ToInt32(v)));
                    }
                }
                else
                {
                    CashBookPeriodEndDTs = new List<DateTime>();
                }
            }
        }

        public IEnumerable<DateTime> CashBookPeriodEndDTs { get; set; }

        /// <summary>
        /// Cash Book End of Period Statuses
        /// </summary>
        [UniVerseIndex(19)]
        [UniVerseFieldName("CBPE.STATUS")]
        [UniVerseMultiValue]
        public IEnumerable<short> CashBookPeriodEndStatuses { get; set; }

        ///// <summary>
        ///// General Ledger Previous Year Period End Dates
        ///// </summary>
        //[UniVerseIndex(20)]
        //[UniVerseFieldName("SS.GL.PREVYR.INT")]
        //[UniVerseMultiValue]
        //public IEnumerable<string> GeneralLedgerPreviousYearPeriodEndDates
        //{
        //    get
        //    {
        //        if (GeneralLedgerPreviousYearPeriodEndDTs.Any())
        //        {
        //            return GeneralLedgerPreviousYearPeriodEndDTs.Select(dt => Framework.UniVerseDateString(dt.Date));
        //        }
        //        else
        //        {
        //            return new List<string>();
        //        }
        //    }
        //    set
        //    {
        //        if (value != null)
        //        {
        //            GeneralLedgerPreviousYearPeriodEndDTs = value.Select(v => Framework.DateTimeFromUniVerseOffset(Convert.ToInt32(v)));
        //        }
        //        else
        //        {
        //            GeneralLedgerPreviousYearPeriodEndDTs = new List<DateTime>();
        //        }
        //    }
        //}

        //public IEnumerable<DateTime> GeneralLedgerPreviousYearPeriodEndDTs { get; set; }

        ///// <summary>
        ///// Sales Ledger Previous Year Period End Dates
        ///// </summary>
        //[UniVerseIndex(21)]
        //[UniVerseFieldName("SS.SL.PREVYR.INT")]
        //[UniVerseMultiValue]
        //public IEnumerable<string> SalesLedgerPreviousYearPeriodEndDates
        //{
        //    get
        //    {
        //        if (SalesLedgerPreviousYearPeriodEndDTs.Any())
        //        {
        //            return SalesLedgerPreviousYearPeriodEndDTs.Select(dt => Framework.UniVerseDateString(dt.Date));
        //        }
        //        else
        //        {
        //            return new List<string>();
        //        }
        //    }
        //    set
        //    {
        //        if (value != null)
        //        {
        //            SalesLedgerPreviousYearPeriodEndDTs = value.Select(v => Framework.DateTimeFromUniVerseOffset(Convert.ToInt32(v)));
        //        }
        //        else
        //        {
        //            SalesLedgerPreviousYearPeriodEndDTs = new List<DateTime>();
        //        }
        //    }
        //}

        //public IEnumerable<DateTime> SalesLedgerPreviousYearPeriodEndDTs { get; set; }

        [UniVerseIndex(22)]
        [UniVerseFieldName("CURRENT.FIN.YEAR")]
        public short? CurrentFinancialYear { get; set; }
    }
}
