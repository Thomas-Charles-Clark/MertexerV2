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
    public class MadeToMeasureTypeCode
    {
        public string Code { get; set; }
    }

    public static partial class MadeToMeasureTypeFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "MadeToMeasureTypeSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string MadeToMeasureTypeCodes)
        {
            if (String.IsNullOrEmpty(MadeToMeasureTypeCodes))
            {
                foreach (var q in Selector<MadeToMeasureType>.SelectAllNonParallel(
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
                var results = Selector<MadeToMeasureType>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<MadeToMeasureTypeCode>>(MadeToMeasureTypeCodes).Select(p => p.Code),
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

        public static void MadeToMeasureTypeSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name
            )
        {
            var p = (MadeToMeasureType)obj;

            Code = p.Key?.TrimReduceTruncate(1);
            Name = p.Name?.TrimReduceTruncate(50);
        }
    }
}