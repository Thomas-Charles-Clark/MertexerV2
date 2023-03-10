using MertexerV2.GeneralFunctions;
using MertexerV2.Models.Invoices;
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

namespace MertexerV2.InvoiceFunctions
{
    public static partial class InvoiceLineFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "InvoiceLineSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string InvoiceLineKeys, string InvoiceIDs)
        {
            if (String.IsNullOrEmpty(InvoiceLineKeys) && String.IsNullOrEmpty(InvoiceIDs))
            {
                foreach (var q in Selector<InvoiceLine>.SelectAllNonParallel(
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
                if (!String.IsNullOrEmpty(InvoiceLineKeys))
                {
                    var results = Selector<InvoiceLine>.Select(
                        JsonConvert.DeserializeObject<IEnumerable<InvoiceIDWrapper>>(InvoiceLineKeys).Select(p => p.InvoiceID),
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
                else
                {
                    if (!String.IsNullOrEmpty(InvoiceIDs))
                    {
                        var batches = JsonConvert.DeserializeObject<List<InvoiceIDWrapper>>(InvoiceIDs).Select(k => k.InvoiceID);

                        var split = ListExtension.Split(batches, 50).ToList();

                        foreach(var batch in split)
                        {
                            var filters = new List<Tuple<string, string, string>>();

                            filters.Add(new Tuple<string, string, string>("DOCNO", "=", String.Join(" ", batch)));

                            var results = Selector<InvoiceLine>.Filter(
                                filters,
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
                }
            }
        }

        public static void InvoiceLineSelect_FillRow(
            object obj,
            out SqlString InvoiceLineID,
            out SqlString InvoiceID,
            out SqlString ProductCode,
            out SqlDecimal QuantityOrdered,
            out SqlDecimal QuantityDespatched,
            out SqlDecimal QuantityInvoiced,
            out SqlDecimal UnitPriceExVAT,
            out SqlDecimal LineValueExVAT,
            out SqlDecimal DiscountPercentage,
            out SqlString VATCode,
            out SqlDecimal VATRate,
            out SqlDecimal DiscountValue,
            out SqlDecimal VATValue,
            out SqlString StockUnit,
            out SqlDecimal UnitCost,
            out SqlDecimal UnitCostCurrency,
            out SqlDecimal LineCost,
            out SqlDecimal LineCostCurrency,
            out SqlDecimal UnitPriceExVATCurrency,
            out SqlDecimal LineValueExVATCurrency,
            out SqlDecimal VATValueCurrency,
            out SqlString GeneralLedgerCode
            )
        {
            var p = (InvoiceLine)obj;

            InvoiceLineID = p.Key?.TrimReduceTruncate(25);
            InvoiceID = p.InvoiceNo?.TrimReduceTruncate(25);
            ProductCode = p.ProductCode?.TrimReduceTruncate(50);
            QuantityOrdered = p.QuantityOrdered ?? SqlDecimal.Null;
            QuantityDespatched = p.QuantityDespatched ?? SqlDecimal.Null;
            QuantityInvoiced = p.QuantityInvoiced ?? SqlDecimal.Null;
            UnitPriceExVAT = p.UnitPriceExVAT ?? SqlDecimal.Null;
            LineValueExVAT = p.LineValueExVAT ?? SqlDecimal.Null;
            DiscountPercentage = p.DiscountPercentage ?? SqlDecimal.Null;
            VATCode = p.VATCode?.TrimReduceTruncate(2);
            VATRate = p.VATRate ?? SqlDecimal.Null;
            DiscountValue = p.DiscountValue ?? SqlDecimal.Null;
            VATValue = p.VATValue ?? SqlDecimal.Null;
            StockUnit = p.StockUnit?.TrimReduceTruncate(10);
            UnitCost = p.UnitCost ?? SqlDecimal.Null;
            UnitCostCurrency = p.UnitCostCurrency ?? SqlDecimal.Null;
            LineCost = p.LineCost ?? SqlDecimal.Null;
            LineCostCurrency = p.LineCostCurrency ?? SqlDecimal.Null;
            UnitPriceExVATCurrency = p.UnitPriceExVATCurrency ?? SqlDecimal.Null;
            LineValueExVATCurrency = p.LineValueExVATCurrency ?? SqlDecimal.Null;
            VATValueCurrency = p.VATValueCurrency ?? SqlDecimal.Null;
            GeneralLedgerCode = p.GeneralLedgerCode?.TrimReduceTruncate(20);
        }
    }
}
