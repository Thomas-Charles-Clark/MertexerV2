using MertexerV2.Models.Accounts;
using MertexerV2.Models.DataSource;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.SQLFunctions
{
    public static partial class AccountFunctions
    {

        [Microsoft.SqlServer.Server.SqlFunction(DataAccess = DataAccessKind.Read)]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static string Upsert(string TradeDataSourceJSON, string TradeAccountJSON)
        {
            return "";// Accounts.AccountFunctions.Upsert(TradeDataSourceJSON, TradeAccountJSON);
        }
    }
}
