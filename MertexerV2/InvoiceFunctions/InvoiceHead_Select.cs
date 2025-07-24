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
            out SqlString TradeAccountCode,
            out SqlString OrderID,
            out SqlByte InvoiceOrCredit,
            out SqlDecimal CarriageValue,
            out SqlDecimal CarriageValueCurrency,
            out SqlString CurrencyCode,
            out SqlString Terms
            )
        {
            var p = (InvoiceHead)obj;

            InvoiceID = p.Key?.TrimReduceTruncate(25);
            InvoiceDT = p.InvoiceDT ?? SqlDateTime.Null;
            TradeAccountCode = p.TradeAccountCode?.TrimReduceTruncate(50);
            OrderID = p.OrderID?.TrimReduceTruncate(25);
            InvoiceOrCredit = p.InvoiceOrCredit;
            CarriageValue = p.CarriageValue ?? SqlDecimal.Null;
            CarriageValueCurrency = p.CarriageValueCurrency ?? SqlDecimal.Null;
            CurrencyCode = p.CurrencyCode ?? SqlString.Null;
            Terms = p.Terms?.TrimReduceTruncate(10) ?? SqlString.Null;
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

                    var keys = ce.Arbitrary($"LIST OEINVHEAD @ID FMT\"60L\" ID.SUPP COL.HDR.SUPP COUNT.SUP")
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

                var query = $"LIST OEINVHEAD @ID FMT\"60L\" ";

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
