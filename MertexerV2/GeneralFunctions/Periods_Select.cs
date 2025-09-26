using MertexerV2.Models;
using MertexerV2.Models.General;
using MertexerV2.Models.Product;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.GeneralFunctions
{
    public class CompanyCode
    {
        public string Code { get; set; }
    }

    public static partial class PeriodsFunctions
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "PeriodsSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string CompanyCodes)
        {
            var results = new List<PeriodExplosion>();

            if (String.IsNullOrEmpty(CompanyCodes))
            {
                foreach (var q in Selector<Periods>.SelectAllNonParallel(
                   true,
                   false,
                   50,
                   UniVerseAccount).ToList())
                {
                    results.AddRange(q.GeneralLedgerPeriodDatesThisYear.Select((glpty, i) =>
                    {
                        return new PeriodExplosion()
                        {
                            Period = Convert.ToByte(i + 1),
                            GeneralLedgerPeriodLastYearDT = q.GeneralLedgerPeriodLastYearDTs?.ElementAtOrDefault(i),
                            GeneralLedgerPeriodThisYearDT = q.GeneralLedgerPeriodDatesThisYearDTs?.ElementAtOrDefault(i),
                            GeneralLedgerPeriodNextYearDT = q.GeneralLedgerPeriodDatesNextYearDTs?.ElementAtOrDefault(i),
                            VATClose = q.VATClose?.ElementAtOrDefault(i),
                            VATCloseNextYear = q.VATCloseNextYear?.ElementAtOrDefault(i),
                            SalesLedgerPeriodDatesThisYearDT = q.SalesLedgerPeriodDatesThisYearDTs?.ElementAtOrDefault(i),
                            SalesLedgerPeriodDatesNextYearDT = q.SalesLedgerPeriodDatesNextYearDTs?.ElementAtOrDefault(i),
                            SalesLedgerPeriodEndUser = q.SalesLedgerPeriodEndUser?.ElementAtOrDefault(i),
                            SalesLedgerPeriodEndDT = q.SalesLedgerPeriodEndDTs?.ElementAtOrDefault(i),
                            SalesLedgerPeriodEndStatus = q.SalesLedgerPeriodEndStatuses?.ElementAtOrDefault(i),
                            PurchaseLedgerPeriodEndUser = q.PurchaseLedgerPeriodEndUser?.ElementAtOrDefault(i),
                            PurchaseLedgerPeriodEndDT = q.PurchaseLedgerPeriodEndDTs?.ElementAtOrDefault(i),
                            PurchaseLedgerPeriodEndStatus = q.PurchaseLedgerPeriodEndStatuses?.ElementAtOrDefault(i),
                            CashBookPeriodEndUser = q.CashBookPeriodEndUser?.ElementAtOrDefault(i),
                            CashBookPeriodEndDT = q.CashBookPeriodEndDTs?.ElementAtOrDefault(i),
                            CashBookPeriodEndStatus = q.CashBookPeriodEndStatuses?.ElementAtOrDefault(i),
                            
                            // Below two commented out because columns appear to contain both MV and SV values, which is just... too much for me to
                            // handle right now. Why would you record a period ending twice. What does that even mean.

                            //GeneralLedgerPreviousYearPeriodEndDT = q.GeneralLedgerPreviousYearPeriodEndDTs?.ElementAtOrDefault(i),
                            //SalesLedgerPreviousYearPeriodEndDT = q.SalesLedgerPreviousYearPeriodEndDTs?.ElementAtOrDefault(i)
                        };
                    }));
                }
            }
            else
            {
                foreach(var q in Selector<Periods>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<CompanyCode>>(CompanyCodes).Select(p => p.Code),
                    true,
                    false,
                    50,
                    UniVerseAccount).ToList())
                {
                    results.AddRange(q.GeneralLedgerPeriodDatesThisYear.Select((glpty, i) =>
                    {
                        return new PeriodExplosion()
                        {
                            Period = Convert.ToByte(i + 1),
                            GeneralLedgerPeriodLastYearDT = q.GeneralLedgerPeriodLastYearDTs?.ElementAtOrDefault(i),
                            GeneralLedgerPeriodThisYearDT = q.GeneralLedgerPeriodDatesThisYearDTs?.ElementAtOrDefault(i),
                            GeneralLedgerPeriodNextYearDT = q.GeneralLedgerPeriodDatesNextYearDTs?.ElementAtOrDefault(i),
                            VATClose = q.VATClose?.ElementAtOrDefault(i),
                            VATCloseNextYear = q.VATCloseNextYear?.ElementAtOrDefault(i),
                            SalesLedgerPeriodDatesThisYearDT = q.SalesLedgerPeriodDatesThisYearDTs?.ElementAtOrDefault(i),
                            SalesLedgerPeriodDatesNextYearDT = q.SalesLedgerPeriodDatesNextYearDTs?.ElementAtOrDefault(i),
                            SalesLedgerPeriodEndUser = q.SalesLedgerPeriodEndUser?.ElementAtOrDefault(i),
                            SalesLedgerPeriodEndDT = q.SalesLedgerPeriodEndDTs?.ElementAtOrDefault(i),
                            SalesLedgerPeriodEndStatus = q.SalesLedgerPeriodEndStatuses?.ElementAtOrDefault(i),
                            PurchaseLedgerPeriodEndUser = q.PurchaseLedgerPeriodEndUser?.ElementAtOrDefault(i),
                            PurchaseLedgerPeriodEndDT = q.PurchaseLedgerPeriodEndDTs?.ElementAtOrDefault(i),
                            PurchaseLedgerPeriodEndStatus = q.PurchaseLedgerPeriodEndStatuses?.ElementAtOrDefault(i),
                            CashBookPeriodEndUser = q.CashBookPeriodEndUser?.ElementAtOrDefault(i),
                            CashBookPeriodEndDT = q.CashBookPeriodEndDTs?.ElementAtOrDefault(i),
                            CashBookPeriodEndStatus = q.CashBookPeriodEndStatuses?.ElementAtOrDefault(i),
                            //GeneralLedgerPreviousYearPeriodEndDT = q.GeneralLedgerPreviousYearPeriodEndDTs?.ElementAtOrDefault(i),
                            //SalesLedgerPreviousYearPeriodEndDT = q.SalesLedgerPreviousYearPeriodEndDTs?.ElementAtOrDefault(i)
                        };
                    }));
                }
            }

            return new ArrayList(results);
        }

        public static void PeriodsSelect_FillRow(
            object obj,
            out SqlByte Period,
            out SqlDateTime GeneralLedgerPeriodLastYearDT,
            out SqlDateTime GeneralLedgerPeriodThisYearDT,
            out SqlDateTime GeneralLedgerPeriodNextYearDT,
            out SqlBoolean VATClose,
            out SqlBoolean VATCloseNextYear,
            out SqlDateTime SalesLedgerPeriodDatesThisYearDT,
            out SqlDateTime SalesLedgerPeriodDatesNextYearDT,
            out SqlString SalesLedgerPeriodEndUser,
            out SqlDateTime SalesLedgerPeriodEndDT,
            out SqlInt16 SalesLedgerPeriodEndStatus,
            out SqlString PurchaseLedgerPeriodEndUser,
            out SqlDateTime PurchaseLedgerPeriodEndDT,
            out SqlInt16 PurchaseLedgerPeriodEndStatus,
            out SqlString CashBookPeriodEndUser,
            out SqlDateTime CashBookPeriodEndDT,
            out SqlInt16 CashBookPeriodEndStatus,
            //out SqlDateTime GeneralLedgerPreviousYearPeriodEndDT,
            //out SqlDateTime SalesLedgerPreviousYearPeriodEndDT,
            out SqlInt16 CurrentFinancialYear)
        {
            var p = (PeriodExplosion)obj;

            Period = p.Period;
            GeneralLedgerPeriodLastYearDT = p.GeneralLedgerPeriodLastYearDT ?? SqlDateTime.Null;
            GeneralLedgerPeriodThisYearDT = p.GeneralLedgerPeriodThisYearDT ?? SqlDateTime.Null;
            GeneralLedgerPeriodNextYearDT = p.GeneralLedgerPeriodNextYearDT ?? SqlDateTime.Null;
            VATClose = p.VATClose ?? SqlBoolean.Null;
            VATCloseNextYear = p.VATCloseNextYear ?? SqlBoolean.Null;
            SalesLedgerPeriodDatesThisYearDT = p.SalesLedgerPeriodDatesThisYearDT ?? SqlDateTime.Null;
            SalesLedgerPeriodDatesNextYearDT = p.SalesLedgerPeriodDatesNextYearDT ?? SqlDateTime.Null;
            SalesLedgerPeriodEndUser = p.SalesLedgerPeriodEndUser;
            SalesLedgerPeriodEndDT = p.SalesLedgerPeriodEndDT ?? SqlDateTime.Null;
            SalesLedgerPeriodEndStatus = p.SalesLedgerPeriodEndStatus ?? SqlInt16.Null;
            PurchaseLedgerPeriodEndUser = p.PurchaseLedgerPeriodEndUser;
            PurchaseLedgerPeriodEndDT = p.PurchaseLedgerPeriodEndDT ?? SqlDateTime.Null;
            PurchaseLedgerPeriodEndStatus = p.PurchaseLedgerPeriodEndStatus ?? SqlInt16.Null;
            CashBookPeriodEndUser = p.CashBookPeriodEndUser;
            CashBookPeriodEndDT = p.CashBookPeriodEndDT ?? SqlDateTime.Null;
            CashBookPeriodEndStatus = p.CashBookPeriodEndStatus ?? SqlInt16.Null;
            //GeneralLedgerPreviousYearPeriodEndDT = p.GeneralLedgerPreviousYearPeriodEndDT ?? SqlDateTime.Null;
            //SalesLedgerPreviousYearPeriodEndDT = p.SalesLedgerPreviousYearPeriodEndDT ?? SqlDateTime.Null;
            CurrentFinancialYear = p.CurrentFinancialYear ?? SqlInt16.Null;
        }
    }
}
