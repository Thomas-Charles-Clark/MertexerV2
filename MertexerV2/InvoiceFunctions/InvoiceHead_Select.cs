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
    public class InvoiceIDWrapper
    {
        public string InvoiceID { get; set; }
    }

    public static partial class InvoiceHeadFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "InvoiceHeadSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string InvoiceIDs)
        {
            // If we have nothing to filter on
            if (String.IsNullOrEmpty(InvoiceIDs))
            {
                // Return everything
                foreach (var q in Selector<InvoiceHead>.SelectAllNonParallel(
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
                // If we have specific invoice numbers
                if (!String.IsNullOrEmpty(InvoiceIDs))
                {
                    // Return those
                    var results = Selector<InvoiceHead>.Select(
                        JsonConvert.DeserializeObject<IEnumerable<InvoiceIDWrapper>>(InvoiceIDs).Select(p => p.InvoiceID),
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

        public static void InvoiceHeadSelect_FillRow(
            object obj,
            out SqlString InvoiceID,
            out SqlDateTime InvoiceDT,
            out SqlDateTime DueDT,
            out SqlDateTime DespatchedDT,
			out SqlString TradeAccountCode,
            out SqlString OrderID,
            out SqlByte InvoiceOrCredit,
            out SqlDecimal CarriageValue,
            out SqlDecimal CarriageValueCurrency,
            out SqlString CurrencyCode,
            out SqlString Terms,
            out SqlDecimal TotalValue,
            out SqlDecimal TotalVAT,
            out SqlString VATCodes,
            out SqlString VATCode,
            out SqlString VATRates,
            out SqlDecimal VATRate,
            out SqlString VATableValues,
            out SqlDecimal VATableValue,
            out SqlString VATTaxes,
            out SqlDecimal VATTax,
            out SqlString VATableValuesCurrency,
            out SqlDecimal VATableValueCurrency,
            out SqlString VATTaxesCurrency,
            out SqlDecimal VATTaxCurrency,
            out SqlDecimal TotalValueCurrency,
            out SqlDecimal TotalVATCurrency,
            out SqlString MiscellaneousChargeCode,
            out SqlDecimal MiscellaneousChargeValue,
            out SqlDecimal MiscellaneousChargeValueCurrency,
            out SqlDecimal MiscellaneousChargeVAT,
            out SqlDecimal MiscellaneousChargeVATCurrency,
            out SqlString CarriageVATCode,
            out SqlDecimal CarriageVAT,
            out SqlDecimal CarriageVATCurrency,
            out SqlDecimal CalculatedTotalValue,
            out SqlDecimal CalculatedTotalValueCurrency,
            out SqlDecimal CalculatedTotalVAT,
            out SqlDecimal CalculatedTotalVATCurrency
            )
        {
            var p = (InvoiceHead)obj;

            InvoiceID = p.Key?.TrimReduceTruncate(25);
            InvoiceDT = p.InvoiceDT ?? SqlDateTime.Null;
            DueDT = p.DueDT ?? SqlDateTime.Null;
            DespatchedDT = p.DespatchedDT ?? SqlDateTime.Null;
			TradeAccountCode = p.TradeAccountCode?.TrimReduceTruncate(50);
            OrderID = p.OrderID?.TrimReduceTruncate(25);
            InvoiceOrCredit = p.InvoiceOrCredit;
            CarriageValue = p.CarriageValue ?? SqlDecimal.Null;
            CarriageValueCurrency = p.CarriageValueCurrency ?? SqlDecimal.Null;
            CurrencyCode = p.CurrencyCode ?? SqlString.Null;
            Terms = p.Terms?.TrimReduceTruncate(10) ?? SqlString.Null;

            TotalValue = p.TotalValue;
            TotalVAT = p.TotalVAT;
            VATCodes = p.VATCodes?.Any() == true ? JsonConvert.SerializeObject(p.VATCodes) : SqlString.Null;
            VATCode = p.VATCode;
            VATRates = p.VATRates?.Any() == true ? JsonConvert.SerializeObject(p.VATRates) : SqlString.Null;
            VATRate = p.VATRate;
            VATableValues = p.VATableValues?.Any() == true ? JsonConvert.SerializeObject(p.VATableValues) : SqlString.Null;
            VATableValue = p.VATableValue;
            VATTaxes = p.VATTaxes?.Any() == true ? JsonConvert.SerializeObject(p.VATTaxes) : SqlString.Null;
            VATTax = p.VATTax;
            VATableValuesCurrency = p.VATableValuesCurrency?.Any() == true ? JsonConvert.SerializeObject(p.VATableValuesCurrency) : SqlString.Null;
            VATableValueCurrency = p.VATableValueCurrency;
            VATTaxesCurrency = p.VATTaxesCurrency?.Any() == true ? JsonConvert.SerializeObject(p.VATTaxesCurrency) : SqlString.Null;
            VATTaxCurrency = p.VATTaxCurrency;
            TotalValueCurrency = p.TotalValueCurrency;
            TotalVATCurrency = p.TotalVATCurrency;
            MiscellaneousChargeCode = p.MiscellaneousChargeCode;
            MiscellaneousChargeValue = p.MiscellaneousChargeValue;
            MiscellaneousChargeValueCurrency = p.MiscellaneousChargeValueCurrency;
            MiscellaneousChargeVAT = p.MiscellaneousChargeVAT;
            MiscellaneousChargeVATCurrency = p.MiscellaneousChargeVATCurrency;
            CarriageVATCode = p.CarriageVATCode;
            CarriageVAT = p.CarriageVAT;
            CarriageVATCurrency = p.CarriageVATCurrency;
            CalculatedTotalValue = p.CalculatedTotalValue;
            CalculatedTotalValueCurrency = p.CalculatedTotalValueCurrency;
            CalculatedTotalVAT = p.CalculatedTotalVAT;
            CalculatedTotalVATCurrency = p.CalculatedTotalValueCurrency;
        }

        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "InvoiceHeadInitialiseSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable InitialiseSelect(string UniVerseAccount, DateTime? FromDT, DateTime? ToDT)
        {
            // If we have nothing to filter on
            if (FromDT == null && ToDT == null)
            {
                using (var ce = new CommandExecutor(UniVerseAccount))
                {
                    var uniFile = ce.Session.CreateUniFile("OEINVHEAD");

                    var keys = ce.Arbitrary($"LIST OEINVHEAD @ID FMT\"80L\" ID.SUPP COL.HDR.SUPP COUNT.SUP")
                    .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .Distinct()
                    .ToList();

                    foreach (var key in keys) 
                    { 
                        yield return key;
                    }
                }
            }
            else
            {
                var filters = new List<Tuple<string, string, string>>();

                if (FromDT != null)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DOCDATE.INT", ">=", Framework.UniVerseDateString(FromDT.Value)));
                }

                if (ToDT != null)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DOCDATE.INT", "<=", Framework.UniVerseDateString(ToDT.Value)));
                }

                var query = $"LIST OEINVHEAD @ID FMT\"80L\" ";

                query += $"WITH {String.Join(" AND ", filters.Select(f => $"{f.Item1} {f.Item2} {f.Item3}"))} ";

                query += "ID.SUPP COL.HDR.SUPP COUNT.SUP";

                using (var ce = new CommandExecutor(UniVerseAccount))
                {
                    var uniFile = ce.Session.CreateUniFile("OEINVHEAD");

                    var keys = ce.Arbitrary(query)
                    .Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .Distinct()
                    .ToList();

                    foreach (var key in keys)
                    {
                        yield return key;
                    }
                }
            }
        }

        public static void InvoiceHeadInitialiseSelect_FillRow(
            object obj,
            out SqlString InvoiceID
            )
        {
            var key = (string)obj;

            InvoiceID = key?.TrimReduceTruncate(25);
        }
    }
}
