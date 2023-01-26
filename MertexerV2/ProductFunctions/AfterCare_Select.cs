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
    public class AfterCareCode
    {
        public string Code { get; set; }
    }

    public static partial class AfterCareFunctions    
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "AfterCareSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string AfterCareCodes)
        {
            if (String.IsNullOrEmpty(AfterCareCodes))
            {
                foreach (var q in Selector<AfterCare>.SelectAllNonParallel(
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
                var results = Selector<AfterCare>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<AfterCareCode>>(AfterCareCodes).Select(p => p.Code),
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

        public static void AfterCareSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Descriptions,
            out SqlString QuickDescription,
            out SqlString ImageName,
            out SqlByte ImageFont,
            out SqlDecimal ImageScalingFactor,
            out SqlDecimal OffsetX,
            out SqlDecimal OffsetY,
            out SqlString ImageCharacter,
            out SqlBoolean Used
            )
        {
            var p = (AfterCare)obj;

            Code = p.Key?.TrimReduceTruncate(5);
            Descriptions = p.Descriptions.Any() ? JsonConvert.SerializeObject(p.Descriptions) : SqlString.Null;
            QuickDescription = p.QuickDescription?.TrimReduceTruncate(50);
            ImageName = p.ImageName?.TrimReduceTruncate(50);
            ImageFont = p.ImageFont ?? SqlByte.Null;
            ImageScalingFactor = p.ImageScalingFactor ?? SqlDecimal.Null;
            OffsetX = p.OffsetX ?? SqlDecimal.Null;
            OffsetY = p.OffsetY ?? SqlDecimal.Null;
            ImageCharacter = p.ImageCharacter?.TrimReduceTruncate(1);
            Used = p.Used;
        }
    }
}
