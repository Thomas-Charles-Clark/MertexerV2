using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.GeneralFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

namespace MertexerV2.Models.General
{
    public class PeriodExplosion
    {
        public byte Period { get; set; }

        public DateTime? GeneralLedgerPeriodLastYearDT { get; set; }

        public DateTime? GeneralLedgerPeriodThisYearDT { get; set; }

        public DateTime? GeneralLedgerPeriodNextYearDT { get; set; }

        public bool? VATClose { get; set; }

        public bool? VATCloseNextYear { get; set; }

        public DateTime? SalesLedgerPeriodDatesThisYearDT { get; set; }

        public DateTime? SalesLedgerPeriodDatesNextYearDT { get; set; }

        public string SalesLedgerPeriodEndUser { get; set; }

        public DateTime? SalesLedgerPeriodEndDT { get; set; }

        public short? SalesLedgerPeriodEndStatus { get; set; }

        public string PurchaseLedgerPeriodEndUser { get; set; }

        public DateTime? PurchaseLedgerPeriodEndDT { get; set; }

        public short? PurchaseLedgerPeriodEndStatus { get; set; }

        public string CashBookPeriodEndUser { get; set; }

        public DateTime? CashBookPeriodEndDT { get; set; }

        public short? CashBookPeriodEndStatus { get; set; }

        public DateTime? GeneralLedgerPreviousYearPeriodEndDT { get; set; }

        public DateTime? SalesLedgerPreviousYearPeriodEndDT { get; set; }

        public short? CurrentFinancialYear { get; set; }
    }
}
