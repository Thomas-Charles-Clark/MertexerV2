using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MertexerV2.GeneralFunctions;
using MertexerV2.Models.EDAReplacement;
using MertexerV2.Models.PurchaseOrder;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

namespace MertexerV2.POFunctions
{
    public static partial class PurchaseOrderDetailFunctions
    {
        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "FetchPurchaseOrderDetail_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string PurchaseOrderHeadIDs)
        {
            if (String.IsNullOrEmpty(PurchaseOrderHeadIDs))
            {
                foreach (var r in Selector<PurchaseOrderDetail>.SelectAllNonParallel(
                    true,
                    true,
                    50,
                    UniVerseAccount))
                {
                    yield return r;
                }
            }
            else
            {
                var poHeadIds = JsonConvert.DeserializeObject<List<PurchaseOrderHeadCode>>(PurchaseOrderHeadIDs).Select(p => p.PurchaseOrderHeadID).ToList();

                var poHeadIdsChunks = ListExtension.Split(poHeadIds, 100);

                var poDetails = new List<PurchaseOrderDetail>();

                poHeadIdsChunks.ForEach((poheadChunk) =>
                {
                    var filters = new List<Tuple<string, string, string>>();

                    filters.Add(new Tuple<string, string, string>("SS.POHEADKEY", "=", $"{String.Join(" ", poheadChunk)}"));

                    var theseResults = Selector<PurchaseOrderDetail>.Filter(
                        filters,
                        true,
                        true,
                        50,
                        UniVerseAccount)
                        .ToList();

                    poDetails.AddRange(theseResults);
                });


                foreach (var r in poDetails)
                {
                    yield return r;
                }
            }
        }

        public static void FetchPurchaseOrderDetail_FillRow(
            object obj,
            out SqlString PurchaseOrderLineID,
            out SqlString PurchaseOrderNumber,
            out SqlString OrderID,
            out SqlString ProductCode,
            out SqlDecimal OrderedQuantity,
            out SqlDecimal ReceivedQuantity,
            out SqlDecimal OutstandingQuantity,
            out SqlDateTime PromisedDT,
            out SqlDateTime ExpectedDT,
            out SqlBoolean IsDateConfirmed,
            out SqlBoolean IsLineComplete,
            out SqlInt16 PurchaseOrderLineNumber,
            out SqlDecimal LineUnitPrice,
            out SqlDecimal LineUnitPriceCurrency,
            out SqlString StockUnitCode,
            out SqlString SupplierProductCode,
            out SqlBoolean SampleRequired,
            out SqlString PurchaseOrderStatusDescription,
            out SqlString PurchaseOrderStatusInterpreted,
            out SqlBoolean Deleted,
            out SqlDecimal OrderPrice,
            out SqlBoolean CallOffIndicator
            )
        {
            var result = (PurchaseOrderDetail)obj;

            PurchaseOrderLineID =  result.Key.TrimReduceTruncate(30);
            PurchaseOrderNumber = String.IsNullOrEmpty(result.PurchaseOrderHeadID) ? SqlString.Null : result.PurchaseOrderHeadID.TrimReduceTruncate(25);
            OrderID = String.IsNullOrEmpty(result.OrderID) ? SqlString.Null : result.OrderID.TrimReduceTruncate(25);
            ProductCode = String.IsNullOrEmpty(result.ProductCode) ? SqlString.Null : result.ProductCode.TrimReduceTruncate(50);
            OrderedQuantity = result.QuantityOrdered ?? SqlDecimal.Null;
            ReceivedQuantity = result.QuantityReceived ?? SqlDecimal.Null;
            OutstandingQuantity = result.QuantityOutstanding ?? SqlDecimal.Null;
            PromisedDT = result.PromisedDT ?? SqlDateTime.Null;
            ExpectedDT = result.ExpectedArrivalDT ?? SqlDateTime.Null;
            IsDateConfirmed = result.DateConfirmed == "Y";
            Deleted = result.Deleted;
            IsLineComplete = result.LineComplete;
            PurchaseOrderLineNumber = result.LineNumber ?? SqlInt16.Null;
            LineUnitPrice = result.LineUnitPrice ?? SqlDecimal.Null;
            LineUnitPriceCurrency = result.LineUnitPriceCurrency ?? SqlDecimal.Null;
            StockUnitCode = String.IsNullOrEmpty(result.StockUnitCode) ? SqlString.Null : result.StockUnitCode.TrimReduceTruncate(10);
            SupplierProductCode = String.IsNullOrEmpty(result.SupplierProductCode) ? SqlString.Null : result.SupplierProductCode.TrimReduceTruncate(50);
            SampleRequired = result.SampleRequired;
            PurchaseOrderStatusDescription = String.IsNullOrEmpty(result.StatusDescription) ? SqlString.Null : result.StatusDescription.TrimReduceTruncate(255);

            PurchaseOrderStatusInterpreted = "";

            if (result.LineComplete)
            {
                switch (result.DateConfirmed)
                {
                    case "Y":
                        PurchaseOrderStatusInterpreted = "PO Conf";
                        break;
                    case "N":
                        PurchaseOrderStatusInterpreted = "PO Unconf";
                        break;
                    case "C":
                        PurchaseOrderStatusInterpreted = "PO Check";
                        break;
                    default:
                        break;
                }
            }

            Deleted = result.Deleted;

            OrderPrice = result.OrderPrice ?? SqlDecimal.Null;

            CallOffIndicator = result.CallOffIndicator;
        }
    }
}
