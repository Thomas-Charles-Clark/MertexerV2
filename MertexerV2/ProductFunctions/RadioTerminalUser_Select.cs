using MertexerV2.GeneralFunctions;
using MertexerV2.Models.EDAReplacement;
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

namespace MertexerV2.ProductFunctions
{
    public class RadioTerminalUserCode
    {
        public string Code { get; set; }
    }

    public static partial class RadioTerminalUserFunctions    
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "RadioTerminalUserSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string RadioTerminalUserCodes)
        {
            if (String.IsNullOrEmpty(RadioTerminalUserCodes))
            {
                foreach (var q in Selector<RadioTerminalUser>.SelectAllNonParallel(
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
                var results = Selector<RadioTerminalUser>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<RadioTerminalUserCode>>(RadioTerminalUserCodes).Select(p => p.Code),
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

        public static void RadioTerminalUserSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name
            )
        {
            var p = (RadioTerminalUser)obj;

            Code = p.Key?.TrimReduceTruncate(10);
            Name = p.Name?.TrimReduceTruncate(50);
        }
    }
}
