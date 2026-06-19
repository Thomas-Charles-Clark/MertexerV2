using MertexerV2.GeneralFunctions;
using MertexerV2.Models;
using MertexerV2.Models.Bank;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace MertexerV2.InvoiceFunctions
{
    public class BankCode
    {
        public string Code { get; set; }
    }

    public static partial class BankFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "BankSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string BankCodes)
        {
            if (String.IsNullOrEmpty(BankCodes))
            {
                foreach (var r in Selector<Bank>.SelectAllNonParallel(
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
                var results = Selector<Bank>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<BankCode>>(BankCodes).Select(p => p.Code),
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

        public static void BankSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name,
            out SqlString ChequesPrinted,
            out SqlString ChequesAvailable,
            out SqlString GeneralLedgerAccountNumbers_Bank,
            out SqlString NatWestAutoPrinted,
            out SqlString BankAccountNumber,
            out SqlString AutoPay,
            out SqlString GeneralLedgerChequesPrinted,
            out SqlString GeneralLedgerChequesAvailable,
            out SqlString Currency,
            out SqlDecimal ConversionCharge,
            out SqlInt32 LastSheetNumber,
            out SqlString GeneralLedgerAccountNumbers_ExchangeDifferences,
            out SqlString LastAudit,
            out SqlString PayRunInProgress,
            out SqlString SortCode,
            out SqlString BankAddressLine1,
            out SqlString BankAddressLine2,
            out SqlString DescriptionLines,
            out SqlString GeneralLedgerAccountNumbers_InterCompanyControl,
            out SqlDateTime DeletedDT)
        {
            var result = (Bank)obj;

            Code = result.Key;
            Name = result.Name ?? SqlString.Null;
            ChequesPrinted = result.ChequesPrinted ?? SqlString.Null;
            ChequesAvailable = result.ChequesAvailable ?? SqlString.Null;
            GeneralLedgerAccountNumbers_Bank = result.GeneralLedgerAccountNumbers_Bank ?? SqlString.Null;
            NatWestAutoPrinted = result.NatWestAutoPrinted ?? SqlString.Null;
            BankAccountNumber = result.BankAccountNumber ?? SqlString.Null;
            AutoPay = result.AutoPay ?? SqlString.Null;
            GeneralLedgerChequesPrinted = result.GeneralLedgerChequePrinted ?? SqlString.Null;
            GeneralLedgerChequesAvailable = result.GeneralLedgerChequesAvailable ?? SqlString.Null;
            Currency = result.Currency ?? SqlString.Null;
            ConversionCharge = result.ConversionCharge ?? SqlDecimal.Null;
            LastSheetNumber = result.LastSheetNumber;
            GeneralLedgerAccountNumbers_ExchangeDifferences = result.GeneralLedgerAccountNumbers_ExchangeDifferences ?? SqlString.Null;
            LastAudit = result.LastAudit ?? SqlString.Null;
            PayRunInProgress = result.PaymentRunInProgress ?? SqlString.Null;
            SortCode = result.SortCode ?? SqlString.Null;
            BankAddressLine1 = result.BankAddressLine1 ?? SqlString.Null;
            BankAddressLine2 = result.BankAddressLine2 ?? SqlString.Null;
            DescriptionLines = result.FreeFormatText?.Any() == true ? JsonConvert.SerializeObject(result.FreeFormatText) : SqlString.Null;
            GeneralLedgerAccountNumbers_InterCompanyControl = result.GeneralLedgerAccountNumbers_InterCompanyControl ?? SqlString.Null;
            DeletedDT = result.DeletedDT ?? SqlDateTime.Null;
        }
    }
}
