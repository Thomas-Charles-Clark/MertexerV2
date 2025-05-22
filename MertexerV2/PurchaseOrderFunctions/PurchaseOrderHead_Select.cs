using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MertexerV2.GeneralFunctions;
using MertexerV2.Models.PurchaseOrder;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

namespace MertexerV2.POFunctions
{
    public class PurchaseOrderHeadCode
    {
        public string PurchaseOrderHeadID { get; set; }
    }

    public static partial class PurchaseOrderHeadFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "PurchaseOrderHeadSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string PurchaseOrderHeadIDs, DateTime? FromDT, DateTime? ToDT)
        {
            var filters = new List<Tuple<string, string, string>>();

            if (FromDT != null)
            {
                filters.Add(new Tuple<string, string, string>("DATEONFILE.INT", ">=", $"{Framework.UniVerseDateString(FromDT.Value)}"));
            }

            if (ToDT != null)
            {
                filters.Add(new Tuple<string, string, string>("DATEONFILE.INT", "<=", $"{Framework.UniVerseDateString(ToDT.Value)}"));
            }

            if (String.IsNullOrEmpty(PurchaseOrderHeadIDs))
            {
                if (filters.Any())
                {
                    foreach (var r in Selector<PurchaseOrderHead>.FilterNonParallel(
                        filters,
                        true,
                        false,
                        50,
                        UniVerseAccount))
                    {
                        yield return r;
                    }
                }
                else
                {
                    foreach (var r in Selector<PurchaseOrderHead>.SelectAllNonParallel(
                        true,
                        false,
                        50,
                        UniVerseAccount))
                    {
                        yield return r;
                    }
                }
            }
            else
            {
                // FromDT and ToDT are ignored; we were given an explicit list of keys to return for.

                var results = Selector<PurchaseOrderHead>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<PurchaseOrderHeadCode>>(PurchaseOrderHeadIDs).Select(p => p.PurchaseOrderHeadID),
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

        public static void PurchaseOrderHeadSelect_FillRow(
            object obj,
            out SqlString PurchaseOrderHeadID,
            out SqlString OrderID,
            out SqlByte OrderStatus,
            out SqlString StockLocation,
            out SqlDateTime DateOnFile,
            out SqlString Supplier,
            out SqlString ProcessCode,
            out SqlString AuthoriserCode,
            out SqlBoolean CallOffIndicator,
            out SqlString DeliveryName,
            out SqlString DeliveryAddressLine1,
            out SqlString DeliveryAddressLine2,
            out SqlString DeliveryTown,
            out SqlString DeliveryCounty,
            out SqlString DeliveryPostcode,
            out SqlString DeliveryPhone
            )
        {
            var result = (PurchaseOrderHead)obj;

            PurchaseOrderHeadID = result.Key.TrimReduceTruncate(25);

            OrderID = String.IsNullOrEmpty(result.OrderID) ? SqlString.Null : result.OrderID.TrimReduceTruncate(25);

            OrderStatus = result.PurchaseOrderStatusID ?? SqlByte.Null;

            StockLocation = String.IsNullOrEmpty(result.StockLocationCode) ? SqlString.Null : result.StockLocationCode.TrimReduceTruncate(10);

            DateOnFile = result.DateOnFileDT ?? SqlDateTime.Null;

            Supplier = String.IsNullOrEmpty(result.SupplierCode) ? SqlString.Null : result.SupplierCode.TrimReduceTruncate(50);

            ProcessCode = String.IsNullOrEmpty(result.ProcessCode) ? SqlString.Null : result.ProcessCode.TrimReduceTruncate(10);

            AuthoriserCode = String.IsNullOrEmpty(result.AuthoriserCode) ? SqlString.Null : result.AuthoriserCode.TrimReduceTruncate(10);

            CallOffIndicator = result.CallOffIndicator;

            DeliveryName = String.IsNullOrEmpty(result.DeliveryName) ? SqlString.Null : result.DeliveryName.TrimReduceTruncate(50);

            DeliveryAddressLine1 = String.IsNullOrEmpty(result.DeliveryAddressLine1) ? SqlString.Null : result.DeliveryAddressLine1.TrimReduceTruncate(60);

            DeliveryAddressLine2 = String.IsNullOrEmpty(result.DeliveryAddressLine2) ? SqlString.Null : result.DeliveryAddressLine2.TrimReduceTruncate(60);

            DeliveryTown = String.IsNullOrEmpty(result.DeliveryTown) ? SqlString.Null : result.DeliveryTown.TrimReduceTruncate(50);

            DeliveryCounty = String.IsNullOrEmpty(result.DeliveryCounty) ? SqlString.Null : result.DeliveryCounty.TrimReduceTruncate(40);

            DeliveryPostcode = String.IsNullOrEmpty(result.DeliveryPostcode) ? SqlString.Null : result.DeliveryPostcode.TrimReduceTruncate(20);

            DeliveryPhone = String.IsNullOrEmpty(result.DeliveryPhone) ? SqlString.Null : result.DeliveryPhone.TrimReduceTruncate(20);
        }
    }
}
