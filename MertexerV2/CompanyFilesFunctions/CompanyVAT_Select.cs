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
    public class CompanyVATCode
    {
        public string Code { get; set; }
    }

    public static partial class CompanyVATFunctions
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "CompanyVATSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string VATCodes)
        {
            if (String.IsNullOrEmpty(VATCodes))
            {
                foreach (var q in Selector<CompanyVAT>.SelectAllNonParallel(
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
                var results = Selector<CompanyVAT>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<CompanyVATCode>>(VATCodes).Select(p => p.Code),
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

        public static void CompanyVATSelect_FillRow(
            object obj,
            out SqlString VATCode,
            out SqlByte SortCode,
            out SqlString ReportIndicator,
            out SqlDecimal VATRate,
            out SqlString ECVAT,
            out SqlString Description,
            out SqlDecimal SalesInvoicesValue,
            out SqlDecimal SalesInvoicesTax,
            out SqlDecimal SalesCreditSalesValue,
            out SqlDecimal SalesCreditSalesTax,
            out SqlDecimal SalesCreditNotesValue,
            out SqlDecimal SalesCreditNotesTax,
            out SqlDecimal SalesCashSalesValue,
            out SqlDecimal SalesCashSalesTax,
            out SqlDecimal SalesQuarterValueTotal,
            out SqlDecimal SalesQuarterTaxTotal,
            out SqlDecimal SalesNextPeriodInvoicesValue,
            out SqlDecimal SalesNextPeriodInvoicesTax,
            out SqlDecimal SalesNextPeriodCreditSalesValue,
            out SqlDecimal SalesNextPeriodCreditSalesTax,
            out SqlDecimal SalesNextPeriodCreditNotesValue,
            out SqlDecimal SalesNextPeriodCreditNotesTax,
            out SqlDecimal SalesNextPeriodCashSalesValue,
            out SqlDecimal SalesNextPeriodCashSalesTax,
            out SqlDecimal PurchasesInvoicesValue,
            out SqlDecimal PurchasesInvoicesTax,
            out SqlDecimal PurchasesCreditNotesValue,
            out SqlDecimal PurchasesCreditNotesTax,
            out SqlDecimal PurchasesPettyCashValue,
            out SqlDecimal PurchasesPettyCashTax,
            out SqlDecimal PurchasesOtherPaymentsValue,
            out SqlDecimal PurchasesOtherPaymentsTax,
            out SqlDecimal PurchasesQuarterValueTotal,
            out SqlDecimal PurchasesQuarterTaxTotal,
            out SqlDecimal PurchasesNextPeriodInvoicesValue,
            out SqlDecimal PurchasesNextPeriodInvoicesTax,
            out SqlDecimal PurchasesNextPeriodCreditNotesValue,
            out SqlDecimal PurchasesNextPeriodCreditNotesTax,
            out SqlDecimal PurchasesNextPeriodPettyCashValue,
            out SqlDecimal PurchasesNextPeriodPettyCashTax,
            out SqlDecimal PurchasesNextPeriodOtherPaymentsValue,
            out SqlDecimal PurchasesNextPeriodOtherPaymentsTax

            )
        {
            var p = (CompanyVAT)obj;

            VATCode = p.Key;

            SortCode = p.SortCode;

            ReportIndicator = p.ReportIndicator ?? SqlString.Null;

            VATRate = p.VATRate;

            ECVAT = p.ECVAT ?? SqlString.Null;

            Description = p.Description ?? SqlString.Null;

            SalesInvoicesValue = p.SalesInvoicesValue;

            SalesInvoicesTax = p.SalesInvoicesTax;

            SalesCreditSalesValue = p.SalesCreditSalesValue;

            SalesCreditSalesTax = p.SalesCreditSalesTax;

            SalesCreditNotesValue = p.SalesCreditNotesValue;

            SalesCreditNotesTax = p.SalesCreditNotesTax;

            SalesCashSalesValue = p.SalesCashSalesValue;

            SalesCashSalesTax = p.SalesCashSalesTax;

            SalesQuarterValueTotal = p.SalesQuarterValueTotal;

            SalesQuarterTaxTotal = p.SalesQuarterTaxTotal;

            SalesNextPeriodInvoicesValue = p.SalesNextPeriodInvoicesValue;

            SalesNextPeriodInvoicesTax = p.SalesNextPeriodInvoicesTax;

            SalesNextPeriodCreditSalesValue = p.SalesNextPeriodCreditSalesValue;

            SalesNextPeriodCreditSalesTax = p.SalesNextPeriodCreditSalesTax;

            SalesNextPeriodCreditNotesValue = p.SalesNextPeriodCreditNotesValue;

            SalesNextPeriodCreditNotesTax = p.SalesNextPeriodCreditNotesTax;

            SalesNextPeriodCashSalesValue = p.SalesNextPeriodCashSalesValue;

            SalesNextPeriodCashSalesTax = p.SalesNextPeriodCashSalesTax;

            PurchasesInvoicesValue = p.PurchasesInvoicesValue;

            PurchasesInvoicesTax = p.PurchasesInvoicesTax;

            PurchasesCreditNotesValue = p.PurchasesCreditNotesValue;

            PurchasesCreditNotesTax = p.PurchasesCreditNotesTax;

            PurchasesPettyCashValue = p.PurchasesPettyCashValue;

            PurchasesPettyCashTax = p.PurchasesPettyCashTax;

            PurchasesOtherPaymentsValue = p.PurchasesOtherPaymentsValue;

            PurchasesOtherPaymentsTax = p.PurchasesOtherPaymentsTax;

            PurchasesQuarterValueTotal = p.PurchasesQuarterValueTotal;

            PurchasesQuarterTaxTotal = p.PurchasesQuarterTaxTotal;

            PurchasesNextPeriodInvoicesValue = p.PurchasesNextPeriodInvoicesValue;

            PurchasesNextPeriodInvoicesTax = p.PurchasesNextPeriodInvoicesTax;

            PurchasesNextPeriodCreditNotesValue = p.PurchasesNextPeriodCreditNotesValue;

            PurchasesNextPeriodCreditNotesTax = p.PurchasesNextPeriodCreditNotesTax;

            PurchasesNextPeriodPettyCashValue = p.PurchasesNextPeriodPettyCashValue;

            PurchasesNextPeriodPettyCashTax = p.PurchasesNextPeriodPettyCashTax;

            PurchasesNextPeriodOtherPaymentsValue = p.PurchasesNextPeriodOtherPaymentsValue;

            PurchasesNextPeriodOtherPaymentsTax = p.PurchasesNextPeriodOtherPaymentsTax;
        }
    }
}
