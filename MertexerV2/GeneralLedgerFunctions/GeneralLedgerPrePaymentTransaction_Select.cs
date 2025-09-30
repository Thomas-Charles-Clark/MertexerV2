using MertexerV2.GeneralFunctions;
using MertexerV2.Models;
using MertexerV2.Models.Bank;
using MertexerV2.Models.GeneralLedger;
using MertexerV2.Models.PurchaseLedger;
using MertexerV2.SupplierFunctions;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace MertexerV2.GeneralLedgerFunctions
{
    public class GeneralLedgerPrePaymentTransactionCode
    {
        public string Code { get; set; }
    }

    public static partial class GeneralLedgerPrePaymentTransactionFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "GeneralLedgerPrePaymentTransactionSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string GeneralLedgerPrePaymentTransactionCodes, string PurchaseLedgerKey)
        {
            if (String.IsNullOrEmpty(GeneralLedgerPrePaymentTransactionCodes))
            {
                var filters = new List<Tuple<string, string, string>>();

                if (!String.IsNullOrEmpty(PurchaseLedgerKey))
                {
                    filters.Add(new Tuple<string, string, string>("PREPAY.TRANSNO", "eq", PurchaseLedgerKey));
                }


int f = 0;
                if (filters.Any())
                {
                    foreach (var r in Selector<GeneralLedgerPrePaymentTransaction>.FilterNonParallel(
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
                    foreach (var r in Selector<GeneralLedgerPrePaymentTransaction>.SelectAllNonParallel(
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
                var results = Selector<GeneralLedgerPrePaymentTransaction>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<GeneralLedgerPrePaymentTransactionCode>>(GeneralLedgerPrePaymentTransactionCodes).Select(p => p.Code),
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

        public static void GeneralLedgerPrePaymentTransactionSelect_FillRow(
            object obj,
            out SqlString GeneralLedgerPrepaymentTransactionID,
            out SqlString SupplierCode,
            out SqlString GeneralLedgerCodes,
            out SqlDecimal AmountTotal,
            out SqlString Memo,
            out SqlString DocumentNumber,
            out SqlDateTime DocumentDT,
            out SqlDateTime DueDT,
            out SqlString TransactionType,
            out SqlBoolean Apportioned,
            out SqlByte StartPeriod,
            out SqlByte NumberOfPeriods,
            out SqlBoolean SplitEqually,
            out SqlString SplitAmounts,
            out SqlString SplitMemos,
            out SqlString SplitDocumentDTs,
            out SqlString ExpenseGeneralLedgerCode,
            out SqlDecimal SplitTotal
            )
        {
            var result = (GeneralLedgerPrePaymentTransaction)obj;

            GeneralLedgerPrepaymentTransactionID = result.Key;

            SupplierCode = result.SupplierCode ?? SqlString.Null;

            GeneralLedgerCodes = result.GeneralLedgerCodes?.Any() == true ? JsonConvert.SerializeObject(result.GeneralLedgerCodes) : SqlString.Null;

            AmountTotal = result.AmountTotal;

            Memo = result.Memo ?? SqlString.Null;

            DocumentNumber = result.DocumentNumber ?? SqlString.Null;

            DocumentDT = result.DocumentDT ?? SqlDateTime.Null;

            DueDT = result.DueDT ?? SqlDateTime.Null;

            TransactionType = result.TransactionType ?? SqlString.Null;

            Apportioned = result.Apportioned;

            StartPeriod = result.StartPeriod;

            NumberOfPeriods = result.NumberOfPeriods;

            SplitEqually = result.SplitEqually;

            SplitAmounts = result.SplitAmounts?.Any() == true ? JsonConvert.SerializeObject(result.SplitAmounts) : SqlString.Null;

            SplitMemos = result.SplitMemos?.Any() == true ? JsonConvert.SerializeObject(result.SplitMemos) : SqlString.Null;

            SplitDocumentDTs = result.SplitDocumentDTs?.Any() == true ? JsonConvert.SerializeObject(result.SplitDocumentDTs) : SqlString.Null;

            ExpenseGeneralLedgerCode = result.ExpenseGeneralLedgerCode ?? SqlString.Null;

            SplitTotal = result.SplitTotal;
        }
    }
}
