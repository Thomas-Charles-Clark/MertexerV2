using MertexerV2.GeneralFunctions;
using MertexerV2.Models.EDAReplacement;
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
    public class TestCode
    {
        public string Code { get; set; }
    }

    public static partial class TestFunctions    
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "TestSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string TestCodes)
        {
            if (String.IsNullOrEmpty(TestCodes))
            {
                foreach (var q in Selector<Test>.SelectAllNonParallel(
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
                var results = Selector<Test>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<TestCode>>(TestCodes).Select(p => p.Code),
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

        public static void TestSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Description,
            out SqlString Market
            )
        {
            var p = (Test)obj;

            Code = p.Key?.TrimReduceTruncate(50);
            Description = p.Description?.TrimReduceTruncate(50);
            Market = p.Market?.TrimReduceTruncate(50);
        }
    }
}
