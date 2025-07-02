using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MertexerV2.GeneralFunctions;
using MertexerV2.Models.PurchaseLedger;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

namespace MertexerV2.PurchaseLedgerFunctions
{
    public class BudgetHolderCode
    {
        public string Code { get; set; }
    }

    public static partial class BudgetHolderFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "BudgetHolderSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string BudgetHolderCodes)
        {
            if (String.IsNullOrEmpty(BudgetHolderCodes))
            {
                foreach (var invoice in Selector<BudgetHolder>.SelectAllNonParallel(
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
                foreach (var invoice in Selector<BudgetHolder>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<BudgetHolderCode>>(BudgetHolderCodes).Select(p => p.Code),
                    true,
                    false,
                    100,
                    UniVerseAccount)
                    .ToList())
                {
                    yield return invoice;
                }
            }
        }

        public static void BudgetHolderSelect_FillRow(
            object obj,
            out SqlString BudgetHolderCode,
            out SqlString Name,
            out SqlString EmailAddress
            )
        {
            var result = (BudgetHolder)obj;

            BudgetHolderCode = result.Key?.TrimReduceTruncate(10);

            Name = result.Name?.TrimReduceTruncate(50);

            EmailAddress = result.PurchaseOrderEmailAddress?.TrimReduceTruncate(254);
        }
    }
}
