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
    public class UsageCode
    {
        public string Code { get; set; }
    }

    public static partial class UsageFunctions    
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "UsageSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string UsageCodes)
        {
            if (String.IsNullOrEmpty(UsageCodes))
            {
                foreach (var q in Selector<Usage>.SelectAllNonParallel(
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
                var results = Selector<Usage>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<UsageCode>>(UsageCodes).Select(p => p.Code),
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

        public static void UsageSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Descriptions,
            out SqlString QuickDescription,
            out SqlString ImageName,
            out SqlByte ImageFont,
            out SqlDecimal ImageScalingFactor,
            out SqlDecimal OffsetX,
            out SqlDecimal OffsetY,
            out SqlString ImageCharacter
            )
        {
            var p = (Usage)obj;

            Code = p.Key?.TrimReduceTruncate(2);
            Descriptions = p.Descriptions.Any() ? JsonConvert.SerializeObject(p.Descriptions) : SqlString.Null;
            QuickDescription = p.QuickDescription?.TrimReduceTruncate(50);
            ImageName = p.ImageName?.TrimReduceTruncate(50);
            ImageFont = p.ImageFont ?? SqlByte.Null;
            ImageScalingFactor = p.ImageScalingFactor ?? SqlDecimal.Null;
            OffsetX = p.OffsetX ?? SqlDecimal.Null;
            OffsetY = p.OffsetY ?? SqlDecimal.Null;
            ImageCharacter = p.ImageCharacter?.TrimReduceTruncate(20);
        }
    }
}
