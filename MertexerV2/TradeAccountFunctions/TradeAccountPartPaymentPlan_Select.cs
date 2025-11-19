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
                    UniVerseAccount)
                    .ToList();

                foreach (var r in results)
                {
                    yield return r;
                }
            }
        }

        public static void TradeAccountPartPaymentPlanSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name
            )
        {
            var p = (TradeAccountPartPaymentPlan)obj;

            Code = p.Key;
            Name = p.Name;
        }
    }
}
