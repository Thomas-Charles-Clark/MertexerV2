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
            out SqlString Code,
            out SqlByte SortCode,
            out SqlString ReportIndicator,
            out SqlDecimal VATRate,
            out SqlString ECVAT,
            out SqlString Description,
            out SqlDecimal PurchaseInvoiceTax,
            out SqlDecimal PurchaseInvoiceVAT,
            out SqlDecimal PurchaseQuarterTaxTotal,
            out SqlDecimal PurchaseQuarterVATTotal,
            out SqlDecimal PurchaseNextPeriodInvoiceTax,
            out SqlDecimal PurchaseNextPeriodInvoiceVAT
            )
        {
            var p = (CompanyVAT)obj;

            Code = p.Key?.TrimReduceTruncate(2);
            SortCode = p.SortCode;
            ReportIndicator = p.ReportIndicator?.TrimReduceTruncate(1);
            VATRate = p.VATRate;
            ECVAT = p.ECVAT;
            Description = p.Description?.TrimReduceTruncate(100);
            PurchaseInvoiceTax = p.PurchasesInvoicesTax;
            PurchaseInvoiceVAT = p.PurchasesInvoicesValue;
            PurchaseQuarterTaxTotal = p.PurchasesQuarterTaxTotal;
            PurchaseQuarterVATTotal = p.PurchasesQuarterValueTotal;
            PurchaseNextPeriodInvoiceTax = p.PurchasesNextPeriodInvoicesTax;
            PurchaseNextPeriodInvoiceVAT = p.PurchasesNextPeriodInvoicesValue;
        }
    }
}
