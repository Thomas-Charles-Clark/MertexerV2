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
    public class CCRHeadKey
    {
        public string Key { get; set; }
    }

    public static partial class CustomerCareRecordFunctions
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "CustomerCareRecordSelectHead_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable SelectHead(string UniVerseAccount, string CCRHeadKeys)
        {
            if (String.IsNullOrEmpty(CCRHeadKeys))
            {
                foreach (var q in Selector<CustomerCareRecordHead>.SelectAllNonParallel(
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
                var results = Selector<CustomerCareRecordHead>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<CCRHeadKey>>(CCRHeadKeys).Select(p => p.Key),
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

        public static void CustomerCareRecordSelectHead_FillRow(
            object obj,
            out SqlString Key,
            out SqlString CollectionReference,
            out SqlString CollectionName,
            out SqlString CollectionAddress,
            out SqlString CollectionAddressLine1,
            out SqlString CollectionAddressLine2,
            out SqlString CollectionAddressTown,
            out SqlString CollectionAddressCounty,
            out SqlString CollectionAddressPostcode,
            out SqlString ContactName,
            out SqlDateTime CreatedDT,
            out SqlString ClaimTaker,
            out SqlString HaulierCode,
            out SqlString ClaimLineKeys,
            out SqlString FirstClaimLine,
            out SqlString TradeAccountCode,
            out SqlBoolean Printed,
            out SqlDecimal SellingPrice,
            out SqlString StockLocationCode,
            out SqlString Terms,
            out SqlByte ResultCode,
            out SqlBoolean Completed,
            out SqlString CreditNote,
            out SqlString EntryTime,
            out SqlString Details,
            out SqlBoolean UpliftRequired,
            out SqlString AssignedToPerson,
            out SqlString AssignedToDepartment,
            out SqlDateTime AssignedDT,
            out SqlString ActionKeys,
            out SqlString InspectionReportLines,
            out SqlBoolean CreditRequired,
            out SqlBoolean SignedOff,
            out SqlString DeliveryAddress,
            out SqlString ReturnMethod,
            out SqlString DocumentNumber,
            out SqlString SupplierReference,
            out SqlBoolean DebitRequired,
            out SqlString DebitNote,
            out SqlString CarrierReferences,
            out SqlString CarrierReference_First,
            out SqlString CustomersReference,
            out SqlString PrintFieldLines,
            out SqlString SellingCompany,
            out SqlBoolean CompPrinted,
            out SqlBoolean FromWeb,
            out SqlString ContactPhone,
            out SqlString ContactEmail,
            out SqlString CustomerAddressLines,
            out SqlString CustomerName,
            out SqlString CustomerAddressLine1,
            out SqlString CustomerAddressLine2,
            out SqlString CustomerAddressTown,
            out SqlString CustomerAddressCounty,
            out SqlString CustomerAddressPostcode,
            out SqlString LinkedDocuments,
            out SqlString LinkedDocumentNames,
            out SqlString CallNumbers,
            out SqlString EndCustomer,
            out SqlString OrderID,
            out SqlBoolean IncludeCarriage,
            out SqlDecimal CarriageValue,
            out SqlBoolean RecoupHandling,
            out SqlDecimal RecoupPercentage,
            out SqlString ReturnNumbers,
            out SqlString ServiceCode,
            out SqlString ReturnParcelNumbers,
            out SqlString ResultantOrderIDs,
            out SqlString ResultantOrderID_First,
            out SqlString ResultantInvoiceNumbers,
            out SqlString ResultantInvoiceNumber_First,
            out SqlString Country,
            out SqlBoolean ReturnPrinted,
            out SqlBoolean FreeOfChargeRequested,
            out SqlString RelatedCustomerCareLineKeys
            )
        {
            var p = (CustomerCareRecordHead)obj;

            Key = p.Key?.TrimReduceTruncate(20);

            CollectionReference = p.CollectionReference?.TrimReduceTruncate(50);
            CollectionName = p.CollectionName?.TrimReduceTruncate(100);
            CollectionAddress = p.CollectionAddress.Any() ? JsonConvert.SerializeObject(p.CollectionAddress) : SqlString.Null;
            CollectionAddressLine1 = p.CollectionAddressLine1?.TrimReduceTruncate(100);
            CollectionAddressLine2 = p.CollectionAddressLine2?.TrimReduceTruncate(100);
            CollectionAddressTown = p.CollectionAddressTown?.TrimReduceTruncate(100);
            CollectionAddressCounty = p.CollectionAddressCounty?.TrimReduceTruncate(100);
            CollectionAddressPostcode = p.CollectionAddressPostcode?.TrimReduceTruncate(50);
            ContactName = p.ContactName?.TrimReduceTruncate(100);
            CreatedDT = p.CreatedDT ?? SqlDateTime.Null;
            ClaimTaker = p.ClaimTaker?.TrimReduceTruncate(10);
            HaulierCode = p.HaulierCode?.TrimReduceTruncate(10);
            ClaimLineKeys = p.ClaimLineKeys.Any() ? JsonConvert.SerializeObject(p.ClaimLineKeys) : SqlString.Null;
            FirstClaimLine = p.FirstClaimLine?.TrimReduceTruncate(25);
            TradeAccountCode = p.TradeAccountCode?.TrimReduceTruncate(50);
            Printed = p.Printed;
            SellingPrice = p.SellingPrice ?? SqlDecimal.Null;
            StockLocationCode = p.StockLocationCode?.TrimReduceTruncate(10);
            Terms = p.Terms?.TrimReduceTruncate(10);
            ResultCode = p.ResultCode ?? SqlByte.Null;
            Completed = p.Completed;
            CreditNote = p.CreditNote?.TrimReduceTruncate(10);
            EntryTime = p.EntryTime?.TrimReduceTruncate(10);
            Details = p.Details?.TrimReduceTruncate(2000);
            UpliftRequired = p.UpliftRequired;
            AssignedToPerson = p.AssignedToPerson?.TrimReduceTruncate(50);
            AssignedToDepartment = p.AssignedToDepartment?.TrimReduceTruncate(50);
            AssignedDT = p.AssignedDT ?? SqlDateTime.Null;
            ActionKeys = p.ActionKeys.Any() ? JsonConvert.SerializeObject(p.ActionKeys) : SqlString.Null;
            InspectionReportLines = p.InspectionReportLines.Any() ? JsonConvert.SerializeObject(p.InspectionReportLines) : SqlString.Null;
            CreditRequired = p.CreditRequired;
            SignedOff = p.SignedOff;
            DeliveryAddress = p.DeliveryAddress.Any() ? JsonConvert.SerializeObject(p.DeliveryAddress) : SqlString.Null;
            ReturnMethod = p.ReturnMethod?.TrimReduceTruncate(1);
            DocumentNumber = p.DocumentNumber?.TrimReduceTruncate(25);
            SupplierReference = p.SupplierReference?.TrimReduceTruncate(200);
            DebitRequired = p.DebitRequired;
            DebitNote = p.DebitNote?.TrimReduceTruncate(2000);
            CarrierReferences = p.CarrierReferences.Any() ? JsonConvert.SerializeObject(p.CarrierReferences) : SqlString.Null;
            CarrierReference_First = p.CarrierReference_First?.TrimReduceTruncate(100);
            CustomersReference = p.CustomersReference?.TrimReduceTruncate(100);
            PrintFieldLines = p.PrintFieldLines.Any() ? JsonConvert.SerializeObject(p.PrintFieldLines) : SqlString.Null;
            SellingCompany = p.SellingCompany?.TrimReduceTruncate(10);
            CompPrinted = p.CompPrinted;
            FromWeb = p.FromWeb;
            ContactPhone = p.ContactPhone?.TrimReduceTruncate(20);
            ContactEmail = p.ContactEmail?.TrimReduceTruncate(254);
            CustomerAddressLines = p.CustomerAddressLines.Any() ? JsonConvert.SerializeObject(p.CustomerAddressLines) : SqlString.Null;
            CustomerName = p.CustomerName?.TrimReduceTruncate(100);
            CustomerAddressLine1 = p.CustomerAddressLine1?.TrimReduceTruncate(100);
            CustomerAddressLine2 = p.CustomerAddressLine2?.TrimReduceTruncate(100);
            CustomerAddressTown = p.CustomerAddressTown?.TrimReduceTruncate(100);
            CustomerAddressCounty = p.CustomerAddressCounty?.TrimReduceTruncate(100);
            CustomerAddressPostcode = p.CustomerAddressPostcode?.TrimReduceTruncate(50);
            LinkedDocuments = p.LinkedDocuments.Any() ? JsonConvert.SerializeObject(p.LinkedDocuments) : SqlString.Null;
            LinkedDocumentNames = p.LinkedDocumentNames.Any() ? JsonConvert.SerializeObject(p.LinkedDocumentNames) : SqlString.Null;
            CallNumbers = p.CallNumbers.Any() ? JsonConvert.SerializeObject(p.CallNumbers) : SqlString.Null;
            EndCustomer = p.EndCustomer?.TrimReduceTruncate(50);
            OrderID = p.OrderID?.TrimReduceTruncate(50);
            IncludeCarriage = p.IncludeCarriage;
            CarriageValue = p.CarriageValue ?? SqlDecimal.Null;
            RecoupHandling = p.RecoupHandling;
            RecoupPercentage = p.RecoupPercentage ?? SqlDecimal.Null;
            ReturnNumbers = p.ReturnNumbers.Any() ? JsonConvert.SerializeObject(p.ReturnNumbers) : SqlString.Null;
            ServiceCode = p.ServiceCode?.TrimReduceTruncate(10);
            ReturnParcelNumbers = p.ReturnParcelNumbers.Any() ? JsonConvert.SerializeObject(p.ReturnParcelNumbers) : SqlString.Null;
            ResultantOrderIDs = p.ResultantOrderIDs.Any() ? JsonConvert.SerializeObject(p.ResultantOrderIDs) : SqlString.Null;
            ResultantOrderID_First = p.ResultantOrderID_First?.TrimReduceTruncate(25);
            ResultantInvoiceNumbers = p.ResultantInvoiceNumbers.Any() ? JsonConvert.SerializeObject(p.ResultantInvoiceNumbers) : SqlString.Null;
            ResultantInvoiceNumber_First = p.ResultantInvoiceNumber_First?.TrimReduceTruncate(25);
            Country = p.Country?.TrimReduceTruncate(3);
            ReturnPrinted = p.ReturnPrinted;
            FreeOfChargeRequested = p.FreeOfChargeRequested;
            RelatedCustomerCareLineKeys = p.RelatedCustomerCareLineKeys.Any() ? JsonConvert.SerializeObject(p.RelatedCustomerCareLineKeys) : SqlString.Null;
        }
    }
}
