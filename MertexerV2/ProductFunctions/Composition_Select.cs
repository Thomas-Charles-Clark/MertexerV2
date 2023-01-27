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
    public class CompositionCode
    {
        public string Code { get; set; }
    }

    public static partial class CompositionFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "CompositionSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string CompositionCodes)
        {
            if (String.IsNullOrEmpty(CompositionCodes))
            {
                foreach (var q in Selector<Composition>.SelectAllNonParallel(
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
                var results = Selector<Composition>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<CompositionCode>>(CompositionCodes).Select(p => p.Code),
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

        public static void CompositionSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Description1,
            out SqlString Description2,
            out SqlString Description3,
            out SqlString Description4,
            out SqlString Description5,
            out SqlString Description
            )
        {
            var p = (Composition)obj;

            Code = p.Key?.TrimReduceTruncate(20);
            Description1 = p.DescriptionOne?.TrimReduceTruncate(50);
            Description2 = p.DescriptionTwo?.TrimReduceTruncate(50);
            Description3 = p.DescriptionThree?.TrimReduceTruncate(50);
            Description4 = p.DescriptionFour?.TrimReduceTruncate(50);
            Description5 = p.DescriptionFive?.TrimReduceTruncate(50);
            Description = p.Description?.TrimReduceTruncate(250);
        }
    }
}