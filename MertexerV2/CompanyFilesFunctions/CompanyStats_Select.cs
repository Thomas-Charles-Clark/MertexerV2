using MertexerV2.GeneralFunctions;
using MertexerV2.Models.Company;
using MertexerV2.Models.EDAReplacement;
using MertexerV2.Models.Product;
using MertexerV2.Models.PurchaseLedger;
using MertexerV2.Models.SalesAnalysis;
using MertexerV2.Models.Supplier;
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

using Supplier = MertexerV2.Models.Supplier.Supplier;

namespace MertexerV2.CompanyFileFunctions
{
    public class CompanyStatsCode
    {
        public string Code { get; set; }
    }

    public static partial class CompanyStatsFunctions
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "CompanyStatsSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string StatsCodes)
        {
            if (String.IsNullOrEmpty(StatsCodes))
            {
                foreach (var q in Selector<CompanyStats>.SelectAllNonParallel(
                   true,
                   false,
                   50,
                   UniVerseAccount))
                {
                    yield return q;
                }
            }
            else
            {
                var results = Selector<CompanyStats>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<CompanyStatsCode>>(StatsCodes).Select(p => p.Code),
                    true,
                    false,
                    50,
                    UniVerseAccount)
                    .ToList();

                foreach (var r in results)
                {
                    yield return r;
                }
            }
        }

        public static void CompanyStatsSelect_FillRow(
            object obj,
            out SqlString CompanyID,
            out SqlDecimal SalesLedgerBroughtForward,
	        out SqlDecimal SalesLedgerCreditSales,
	        out SqlDecimal SalesLedgerCreditNotes,
	        out SqlDecimal SalesLedgerCashReceipts,
	        out SqlDecimal SalesLedgerAdjustments,
	        out SqlDecimal SalesLedgerTotalCashPayments,
	        out SqlDecimal PurchaseLedgerBroughtForward,
	        out SqlDecimal PurchaseLedgerInvoices,
	        out SqlDecimal PurchaseLedgerCreditNotes,
	        out SqlDecimal PurchaseLedgerPayments,
	        out SqlDecimal PurchaseLedgerAdjustments,
	        out SqlDecimal PurchaseLedgerPettyCash,
	        out SqlDecimal PurchaseLedgerOtherPayments,
	        out SqlDecimal NextPurchaseLedgerInvoices,
	        out SqlDecimal NextPurchaseLedgerCreditNotes,
	        out SqlDecimal NextPurchaseLedgerPayments,
	        out SqlDecimal NextPurchaseLedgerAdjustments,
	        out SqlDecimal NextPurchaseLedgerPettyCash,
	        out SqlDecimal NextPurchaseLedgerOtherPayments,
	        out SqlDecimal SalesLedgerCarriedForward,
	        out SqlDecimal PurchaseLedgerCarriedForward,
	        out SqlDecimal PurchaseLedgerNextPeriod
            )
        {
            var p = (CompanyStats)obj;

            // ADD ADDRESS DETAILS AND REMOVE EXTANT EDAREPLACEMENT VERSION

            CompanyID = p.Key;

            SalesLedgerBroughtForward = p.SalesLedgerBroughtForward;

	        SalesLedgerCreditSales = p.SalesLedgerCreditSales;

	        SalesLedgerCreditNotes = p.SalesLedgerCreditNotes;

	        SalesLedgerCashReceipts = p.SalesLedgerCashReceipts;

	        SalesLedgerAdjustments = p.SalesLedgerAdjustments;

	        SalesLedgerTotalCashPayments = p.SalesLedgerTotalCashPayments;

	        PurchaseLedgerBroughtForward = p.PurchaseLedgerBroughtForward;

	        PurchaseLedgerInvoices = p.PurchaseLedgerInvoices;

	        PurchaseLedgerCreditNotes = p.PurchaseLedgerCreditNotes;

	        PurchaseLedgerPayments = p.PurchaseLedgerPayments;

	        PurchaseLedgerAdjustments = p.PurchaseLedgerAdjustments;

	        PurchaseLedgerPettyCash = p.PurchaseLedgerPettyCash;

	        PurchaseLedgerOtherPayments = p.PurchaseLedgerOtherPayments;

	        NextPurchaseLedgerInvoices = p.NextPurchaseLedgerInvoices;

	        NextPurchaseLedgerCreditNotes = p.NextPurchaseLedgerCreditNotes;

	        NextPurchaseLedgerPayments = p.NextPurchaseLedgerPayments;

	        NextPurchaseLedgerAdjustments = p.NextPurchaseLedgerAdjustments;

	        NextPurchaseLedgerPettyCash = p.NextPurchaseLedgerPettyCash;

	        NextPurchaseLedgerOtherPayments = p.NextPurchaseLedgerOtherPayments;

	        SalesLedgerCarriedForward = p.SalesLedgerCarriedForward;

	        PurchaseLedgerCarriedForward = p.PurchaseLedgerCarriedForward;

	        PurchaseLedgerNextPeriod = p.PurchaseLedgerNextPeriod;
        }
    }
}
