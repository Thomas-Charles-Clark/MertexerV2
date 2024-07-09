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

namespace MertexerV2.GeneralFunctions
{
    public class OperatorCode
    {
        public string Code { get; set; }
    }

    public static partial class OperatorFunctions    
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "OperatorSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string OperatorCodes)
        {
            if (String.IsNullOrEmpty(OperatorCodes))
            {
                foreach (var q in Selector<Operator>.SelectAllNonParallel(
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
                var results = Selector<Operator>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<OperatorCode>>(OperatorCodes).Select(p => p.Code),
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

        public static void OperatorSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name,
            out SqlString EmailAddress
            )
        {
            var p = (Operator)obj;

            Code = p.Key?.TrimReduceTruncate(10);
            Name = p.Name?.TrimReduceTruncate(50);
            EmailAddress = p.EmailAddress?.TrimReduceTruncate(254);
        }
    }
}
