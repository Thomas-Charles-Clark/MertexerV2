using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MertexerV2.GeneralFunctions;
using MertexerV2.Models.Pricing;
using MertexerV2.Models.PurchaseLedger;
using MertexerV2.PricingFunctions;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

namespace MertexerV2.PurchaseLedgerFunctions
{
    public class PurchaseLedgerRegisterHistoryCode
    {
        public string Code { get; set; }
    }

    public static partial class PurchaseLedgerRegisterHistoryFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "PurchaseLedgerRegisterHistorySelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(
            string UniVerseAccount,
            string PurchaseLedgerRegisterHistoryCodes,
            string SupplierCode,
            DateTime? OnFileDT_From,
            DateTime? OnFileDT_To,
            DateTime? DocumentDT_From,
            DateTime? DocumentDT_To,
            DateTime? DueDT_From,
            DateTime? DueDT_To,
            DateTime? ConvertedDT_From,
            DateTime? ConvertedDT_To)
        {
            if (String.IsNullOrEmpty(PurchaseLedgerRegisterHistoryCodes))
            {
                int p = 0;

                var filters = new List<Tuple<string, string, string>>();

                if (!String.IsNullOrEmpty(SupplierCode))
                {
                    filters.Add(new Tuple<string, string, string>("SUPREF", "eq", SupplierCode));
                }

                if (OnFileDT_From.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DATEONFILE.INT", ">=", Framework.UniVerseDateString(OnFileDT_From.Value)));
                }

                if (OnFileDT_To.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DATEONFILE.INT", "<=", Framework.UniVerseDateString(OnFileDT_To.Value)));
                }

                if (DocumentDT_From.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DOCDATE.INT", ">=", Framework.UniVerseDateString(DocumentDT_From.Value)));
                }

                if (DocumentDT_To.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DOCDATE.INT", "<=", Framework.UniVerseDateString(DocumentDT_To.Value)));
                }

                if (DueDT_From.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DUEDATE.INT", ">=", Framework.UniVerseDateString(DueDT_From.Value)));
                }

                if (DueDT_To.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DUEDATE.INT", "<=", Framework.UniVerseDateString(DueDT_To.Value)));
                }

                if (ConvertedDT_From.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.CONVDATE.INT", ">=", Framework.UniVerseDateString(ConvertedDT_From.Value)));
                }

                if (ConvertedDT_To.HasValue)
                {
                    filters.Add(new Tuple<string, string, string>("SS.CONVDATE.INT", "<=", Framework.UniVerseDateString(ConvertedDT_To.Value)));
                }

                if (filters.Any())
                {
                    foreach (var invoice in Selector<PurchaseLedgerRegisterHistory>.FilterNonParallel(
                        filters,
                        true,
                        false,
                        100,
                        UniVerseAccount))
                    {
                        yield return invoice;
                    }
                }
                else
                {
                    foreach (var invoice in Selector<PurchaseLedgerRegisterHistory>.SelectAllNonParallel(
                        true,
                        false,
                        100,
                        UniVerseAccount))
                    {
                        yield return invoice;
                    }
                }
            }
            else
            {
                foreach (var invoice in Selector<PurchaseLedgerRegisterHistory>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<PurchaseLedgerRegisterHistoryCode>>(PurchaseLedgerRegisterHistoryCodes).Select(p => p.Code),
                    true,
                    false,
                    100,
                    UniVerseAccount))
                {
                    yield return invoice;
                }
            }
        }

        public static void PurchaseLedgerRegisterHistorySelect_FillRow(
            object obj,
            out SqlString PurchaseLedgerRegisterHistoryID,
            out SqlString RegisterNumber,
            out SqlDateTime OnFileDT,
            out SqlString SupplierCode,
            out SqlDateTime ConvertedDT,
            out SqlString CreditNumber,
            out SqlString InvoiceNumber,
            out SqlString TransactionNumber,
            out SqlDateTime DeletedDT,
            out SqlString UserID,
            out SqlString TransactionType,
            out SqlDecimal DocumentTotal
            )
        {
            var result = (PurchaseLedgerRegisterHistory)obj;

            PurchaseLedgerRegisterHistoryID = result.Key;

            RegisterNumber = result.RegisterNumber ?? SqlString.Null;

            OnFileDT = result.OnFileDT ?? SqlDateTime.Null;

            SupplierCode = result.SupplierCode ?? SqlString.Null;

            ConvertedDT = result.ConvertedDT ?? SqlDateTime.Null;

            CreditNumber = result.CreditNumber ?? SqlString.Null;

            InvoiceNumber = result.InvoiceNumber ?? SqlString.Null;

            TransactionNumber = result.TransactionNumber ?? SqlString.Null;

            DeletedDT = result.DeletedDT ?? SqlDateTime.Null;

            UserID = result.UserID ?? SqlString.Null;

            TransactionType = result.TransactionType ?? SqlString.Null;

            DocumentTotal = result.DocumentTotal;
        }
    }
}
