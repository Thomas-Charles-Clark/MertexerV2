using MertexerV2.GeneralFunctions;
using MertexerV2.Models.CustomerCare;
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

namespace MertexerV2.AccountFunctions
{
    public class CCRLineKey
    {
        public string Key { get; set; }
    }

    public static partial class CustomerCareRecordFunctions
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "CustomerCareRecordSelectLine_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable SelectLine(string UniVerseAccount, string CCRActionKeys)
        {
            if (String.IsNullOrEmpty(CCRActionKeys))
            {
                foreach (var q in Selector<CustomerCareRecordLine>.SelectAllNonParallel(
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
                var results = Selector<CustomerCareRecordLine>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<CCRActionKey>>(CCRActionKeys).Select(p => p.Key),
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

        public static void CustomerCareRecordSelectLine_FillRow(
            object obj,
            out SqlString Key,
            out SqlString ProductCode,
            out SqlString ProductDescription,
            out SqlDecimal QuantityClaimed,
            out SqlString FaultCodes,
            out SqlString FaultCode_First,
            out SqlString FaultCode_Second,
            out SqlString InvoiceNumber,
            out SqlDecimal QuantityAccepted,
            out SqlBoolean ReturnToStock,
            out SqlDecimal Price,
            out SqlDecimal PercentageDiscount,
            out SqlString MessageLine,
            out SqlString Text,
            out SqlDateTime InvoiceDT,
            out SqlString SupplierReference,
            out SqlString ContactName,
            out SqlString SupplierReturnNumber,
            out SqlDateTime ReturnDT,
            out SqlString Carrier,
            out SqlString Comments,
            out SqlDecimal CreditAmount,
            out SqlDecimal StockPrice,
            out SqlDecimal OrderPriceCurrency,
            out SqlString NSProductCode,
            out SqlString RecordType,
            out SqlString SundryUnit,
            out SqlDecimal NSCostPrice,
            out SqlDecimal NSWeight,
            out SqlString LineMemos,
            out SqlDecimal QuantityReturned,
            out SqlString OrderLineIDs,
            out SqlString OrderLineID,
            out SqlString CustomerReturnNumbers,
            out SqlString CustomerReturnQuantities,
            out SqlString SundryVATCode,
            out SqlString SundryTariffCode,
            out SqlDecimal SundryVATValue,
            out SqlBoolean VATInclusive,
            out SqlString CustomerProductCode,
            out SqlString InvoiceLineNumber,
            out SqlString ComplaintPieces,
            out SqlDecimal BespokeLength,
            out SqlDecimal BespokeWidth,
            out SqlDecimal BespokeSize
            )
        {
            var p = (CustomerCareRecordLine)obj;

            Key = p.Key?.TrimReduceTruncate(50);
            ProductCode = p.ProductCode?.TrimReduceTruncate(50);
            ProductDescription = p.ProductDescription?.TrimReduceTruncate(50);
            QuantityClaimed = p.QuantityClaimed ?? SqlDecimal.Null;
            FaultCodes = p.FaultCodes.Any() ? JsonConvert.SerializeObject(p.FaultCodes) : SqlString.Null;
            FaultCode_First = p.FaultCode_First?.TrimReduceTruncate(50);
            FaultCode_Second = p.FaultCode_Second?.TrimReduceTruncate(50);
            InvoiceNumber = p.InvoiceNumber?.TrimReduceTruncate(50);
            QuantityAccepted = p.QuantityAccepted ?? SqlDecimal.Null;
            ReturnToStock = p.ReturnToStock;
            Price = p.Price ?? SqlDecimal.Null;
            PercentageDiscount = p.PercentageDiscount ?? SqlDecimal.Null;
            MessageLine = p.MessageLine?.TrimReduceTruncate(50);
            Text = p.Text?.TrimReduceTruncate(50);
            InvoiceDT = p.InvoiceDT ?? SqlDateTime.Null;
            SupplierReference = p.SupplierReference?.TrimReduceTruncate(50);
            ContactName = p.ContactName?.TrimReduceTruncate(50);
            SupplierReturnNumber = p.SupplierReturnNumber?.TrimReduceTruncate(50);
            ReturnDT = p.ReturnDT ?? SqlDateTime.Null;
            Carrier = p.Carrier?.TrimReduceTruncate(50);
            Comments = p.Comments.Any() ? JsonConvert.SerializeObject(p.Comments) : SqlString.Null;
            CreditAmount = p.CreditAmount ?? SqlDecimal.Null;
            StockPrice = p.StockPrice ?? SqlDecimal.Null;
            OrderPriceCurrency = p.OrderPriceCurrency ?? SqlDecimal.Null;
            NSProductCode = p.NSProductCode?.TrimReduceTruncate(50);
            RecordType = p.RecordType?.TrimReduceTruncate(50);
            SundryUnit = p.SundryUnit?.TrimReduceTruncate(50);
            NSCostPrice = p.NSCostPrice ?? SqlDecimal.Null;
            NSWeight = p.NSWeight ?? SqlDecimal.Null;
            LineMemos = p.LineMemos.Any() ? JsonConvert.SerializeObject(p.LineMemos) : SqlString.Null;
            QuantityReturned = p.QuantityReturned ?? SqlDecimal.Null;
            OrderLineIDs = p.OrderLineIDs.Any() ? JsonConvert.SerializeObject(p.OrderLineIDs) : SqlString.Null;
            OrderLineID = p.OrderLineID?.TrimReduceTruncate(50);
            CustomerReturnNumbers = p.CustomerReturnNumbers.Any() ? JsonConvert.SerializeObject(p.CustomerReturnNumbers) : SqlString.Null;
            CustomerReturnQuantities = p.CustomerReturnQuantities.Any() ? JsonConvert.SerializeObject(p.CustomerReturnQuantities) : SqlString.Null;
            SundryVATCode = p.SundryVATCode?.TrimReduceTruncate(50);
            SundryTariffCode = p.SundryTariffCode?.TrimReduceTruncate(50);
            SundryVATValue = p.SundryVATValue ?? SqlDecimal.Null;
            VATInclusive = p.VATInclusive;
            CustomerProductCode = p.CustomerProductCode?.TrimReduceTruncate(50);
            InvoiceLineNumber = p.InvoiceLineNumber?.TrimReduceTruncate(50);
            ComplaintPieces = p.ComplaintPieces.Any() ? JsonConvert.SerializeObject(p.ComplaintPieces) : SqlString.Null;
            BespokeLength = p.BespokeLength ?? SqlDecimal.Null;
            BespokeWidth = p.BespokeWidth ?? SqlDecimal.Null;
            BespokeSize = p.BespokeSize ?? SqlDecimal.Null;
        }
    }
}
