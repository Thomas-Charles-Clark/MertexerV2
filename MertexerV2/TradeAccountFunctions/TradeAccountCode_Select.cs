using MertexerV2.GeneralFunctions;
using MertexerV2.Models;
using MertexerV2.Models.DataSource;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MertexerV2.TradeAccountFunctions
{
    public static partial class TradeAccountFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "TradeAccountCodeStubSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable CodeSelect(string UniVerseAccount)
        {
            var keys = Selector<TradeAccount>.SelectKeys(UniVerseAccount).ToList();

            return new ArrayList(keys);
		}

        public static void TradeAccountCodeStubSelect_FillRow(
            object obj,
            out SqlString TradeAccountCode)
        {
            TradeAccountCode = (string)obj;
        }
    }
}
