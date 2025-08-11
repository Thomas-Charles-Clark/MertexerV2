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
    public class GeneralLedgerBatchCode
    {
        public string Code { get; set; }
    }

    public static partial class GeneralLedgerBatchFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "GeneralLedgerBatchSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string GeneralLedgerBatchCodes)
        {
            if (String.IsNullOrEmpty(GeneralLedgerBatchCodes))
            {
                foreach (var r in Selector<GeneralLedgerBatch>.SelectAllNonParallel(
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
                var results = Selector<GeneralLedgerBatch>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<GeneralLedgerBatchCode>>(GeneralLedgerBatchCodes).Select(p => p.Code),
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

        public static void GeneralLedgerBatchSelect_FillRow(
            object obj,
            out SqlString GeneralLedgerBatchID,
            out SqlString TransactionType,
            out SqlString Status,
            out SqlString BatchTotals,
            out SqlString AccumulatedTotals,
            out SqlString DataKeys,
            out SqlInt32 NumberOfRecords,
            out SqlDateTime BatchDT,
            out SqlString Operator
            )
        {
            var result = (GeneralLedgerBatch)obj;

            GeneralLedgerBatchID = result.Key;

            TransactionType = result.TransactionType ?? SqlString.Null;

            Status = result.Status ?? SqlString.Null;

            BatchTotals = result.BatchTotals?.Any() == true ? JsonConvert.SerializeObject(result.BatchTotals) : SqlString.Null;

            AccumulatedTotals = result.AccumulatedTotals?.Any() == true ? JsonConvert.SerializeObject(result.AccumulatedTotals) : SqlString.Null;

            DataKeys = result.DataKeys?.Any() == true ? JsonConvert.SerializeObject(result.DataKeys) : SqlString.Null;

            NumberOfRecords = result.NumberOfRecords;

            BatchDT = result.BatchDT ?? SqlDateTime.Null;

            Operator = result.Operator ?? SqlString.Null;
        }
    }
}
