using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.SQLFunctions
{
    public class Rack
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "Rack_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Check(string piece)
        {
            var results = new List<string>();

            using (var ce = new CommandExecutor())
            {
                results.Add(ce.Arbitrary($"LIST STRACKS WITH PIECES = {piece} COL.HDR.SUPP COUNT.SUP").Trim());
            }

            return new ArrayList(results);
        }

        public static void Rack_FillRow(
            object obj,
            out SqlString Rack
            )
        {
            var checkResult = (string)obj;

            Rack = checkResult;
        }
    }
}
