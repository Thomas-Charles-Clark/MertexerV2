using MertexerV2.GeneralFunctions;
using MertexerV2.Models;
using MertexerV2.Models.General;
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

namespace MertexerV2.TradeAccountFunctions
{
    public class TradeAccountPartPaymentPlanCode
    {
        public string Code { get; set; }
    }

    public static partial class TradeAccountPartPaymentPlanFunctions
	{
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "TradeAccountPartPaymentPlanSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string TradeAccountPartPaymentPlanCodes)
        {
            if (String.IsNullOrEmpty(TradeAccountPartPaymentPlanCodes))
            {
                foreach (var q in Selector<TradeAccountPartPaymentPlan>.SelectAllNonParallel(
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
                var results = Selector<TradeAccountPartPaymentPlan>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<TradeAccountPartPaymentPlanCode>>(TradeAccountPartPaymentPlanCodes).Select(p => p.Code),
                    true,
                    false,
                    50,
                    UniVerseAccount);

                foreach (var r in results)
                {
                    yield return r;
                }
            }
        }

        public static void TradeAccountPartPaymentPlanSelect_FillRow(
            object obj,
            out SqlString Key,
            out SqlString TradeAccountCode,
            out SqlString PayReason,
            out SqlString ContractNumber,
			out SqlString ContractDescription,
			out SqlDecimal TotalAmountToBill,
			out SqlDecimal TotalBilledToDate,
			out SqlInt32 NumberOfPeriods,
            out SqlDecimal CurrentPeriodAmount,
            out SqlDateTime PaymentStartDT,
            out SqlDateTime LastInvoiceRaisedDT,
            out SqlString PaymentAmountsJSON,
            out SqlString InvoiceNumbersJSON,
            out SqlString InvoicePaymentDTsJSON,
            out SqlString InvoiceSequenceNumbersJSON,
            out SqlString InvoiceTextContentsJSON,
            out SqlBoolean ActivePlan,
            out SqlDateTime CreatedDT
			)
        {
            var p = (TradeAccountPartPaymentPlan)obj;

            Key = p.Key;
            TradeAccountCode = p.TradeAccountCode;
            PayReason = p.PayReason;
            ContractNumber = p.ContractNumber;
            ContractDescription = p.ContractDescription;
            TotalAmountToBill = p.TotalAmountToBill;
            TotalBilledToDate = p.TotalBilledToDate;
            NumberOfPeriods = p.NumberOfPeriods;
            CurrentPeriodAmount = p.CurrentPeriodAmount;
            PaymentStartDT = p.PaymentStartDT ?? SqlDateTime.Null;
            LastInvoiceRaisedDT = p.LastInvoiceRaisedDT ?? SqlDateTime.Null;
            PaymentAmountsJSON = p.PaymentAmounts?.Any() == true ? JsonConvert.SerializeObject(p.PaymentAmounts) : SqlString.Null;
            InvoiceNumbersJSON = p.InvoiceNumbers?.Any() == true ? JsonConvert.SerializeObject(p.InvoiceNumbers) : SqlString.Null;
            InvoicePaymentDTsJSON = p.InvoicePaymentDTs?.Any() == true ? JsonConvert.SerializeObject(p.InvoicePaymentDTs) : SqlString.Null;
            InvoiceSequenceNumbersJSON = p.InvoiceSequenceNumbers?.Any() == true ? JsonConvert.SerializeObject(p.InvoiceSequenceNumbers) : SqlString.Null;
            InvoiceTextContentsJSON = p.InvoiceTextContents?.Any() == true ? JsonConvert.SerializeObject(p.InvoiceTextContents) : SqlString.Null;
            ActivePlan = p.ActivePlan;
            CreatedDT = p.CreatedDT ?? SqlDateTime.Null;
        }
    }
}
