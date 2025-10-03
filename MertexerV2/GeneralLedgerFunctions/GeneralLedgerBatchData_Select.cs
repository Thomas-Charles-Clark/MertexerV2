using MertexerV2.GeneralFunctions;
using MertexerV2.Models;
using MertexerV2.Models.Bank;
using MertexerV2.Models.GeneralLedger;
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
    public class GeneralLedgerBatchDataCode
    {
        public string Code { get; set; }
    }

    public static partial class GeneralLedgerBatchDataFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "GeneralLedgerBatchDataSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string GeneralLedgerBatchDataCodes)
        {
            if (String.IsNullOrEmpty(GeneralLedgerBatchDataCodes))
            {
                foreach (var r in Selector<GeneralLedgerBatchData>.SelectAllNonParallel(
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
                var results = Selector<GeneralLedgerBatchData>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<GeneralLedgerBatchDataCode>>(GeneralLedgerBatchDataCodes).Select(p => p.Code),
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

        public static void GeneralLedgerBatchDataSelect_FillRow(
            object obj,
            out SqlString GeneralLedgerBatchDataID,
            out SqlString Type,
            out SqlString DocumentNumber,
            out SqlDateTime DocumentDT,
            out SqlDateTime PostDT,
            out SqlString ItemReference,
            out SqlString SupplierCode,
            out SqlString Amounts,
            out SqlString GeneralLedgerCodes,
            out SqlDecimal VATTotal,
            out SqlDecimal DocumentTotal,
            out SqlString SupplierName,
            out SqlString Memo,
            out SqlString GeneralLedgerSubCodes,
            out SqlString Prefix,
            out SqlString TransactionType,
            out SqlString OtherMemos
            )
        {
            var result = (GeneralLedgerBatchData)obj;

            GeneralLedgerBatchDataID = result.Key;

            Type = result.Type ?? SqlString.Null;

            DocumentNumber = result.DocumentNumber ?? SqlString.Null;

            DocumentDT = result.DocumentDT ?? SqlDateTime.Null;

            PostDT = result.PostDT ?? SqlDateTime.Null;

            ItemReference = result.ItemReference ?? SqlString.Null;

            SupplierCode = result.SupplierCode ?? SqlString.Null;

            Amounts = result.Amounts?.Any() == true ? JsonConvert.SerializeObject(result.Amounts) : SqlString.Null;

            GeneralLedgerCodes = result.GeneralLedgerCodes?.Any() == true ? JsonConvert.SerializeObject(result.GeneralLedgerCodes) : SqlString.Null;

            VATTotal = result.VATTotal;

            DocumentTotal = result.DocumentTotal;

            SupplierName = result.SupplierName ?? SqlString.Null;

            Memo = result.Memo ?? SqlString.Null;

            GeneralLedgerSubCodes = result.GeneralLedgerSubCodes?.Any() == true ? JsonConvert.SerializeObject(result.GeneralLedgerSubCodes) : SqlString.Null;

            Prefix = result.Prefix ?? SqlString.Null;

            TransactionType = result.TransactionType ?? SqlString.Null;

            OtherMemos = result.OtherMemos?.Any() == true ? JsonConvert.SerializeObject(result.OtherMemos) : SqlString.Null;
        }
    }
}
