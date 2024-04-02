using MertexerV2.GeneralFunctions;
using MertexerV2.Models.Pricing;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace MertexerV2.PricingFunctions
{
    public static partial class TradeAccountDiscountFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "ContractDiscountSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static int DeleteAll(string UniVerseAccount, string TradeAccountCode)
        {
            _ = ContractDiscountFunctions.Delete(UniVerseAccount, TradeAccountCode, null);

            _ = ContractPriceFunctions.Delete(UniVerseAccount, TradeAccountCode, null);

            return 0;
        }
    }
}
