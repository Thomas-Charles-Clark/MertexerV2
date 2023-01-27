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

namespace MertexerV2.ProductFunctions
{
    public static partial class QualitySelect
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "QualitySelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string QualityCodes)
        {
            if (String.IsNullOrEmpty(QualityCodes))
            {
                foreach (var q in Selector<Quality>.SelectAllNonParallel(
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
                var results = Selector<Quality>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<ProductCode>>(QualityCodes).Select(p => p.Code),
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


        public static void QualitySelect_FillRow(
            object obj,
            out SqlString Quality,
            out SqlString Design,
            out SqlString Composition,
            out SqlString Description,
            out SqlDecimal Width,
            out SqlDecimal WeightGSM,
            out SqlDecimal GramsPerLinearMetre,
            out SqlDecimal FinishedWidth,
            out SqlDecimal RemeasureLength,
            out SqlBoolean SeparateMaterialLength,
            out SqlBoolean AlternativeRawMaterials,
            out SqlDateTime CreatedDT
            )
        {
            var p = (Quality)obj;

            Quality = p.Key?.TrimReduceTruncate(50);
            Design = p.Design?.TrimReduceTruncate(100);
            Composition = p.Design?.TrimReduceTruncate(10);
            Description = p.Description?.TrimReduceTruncate(100);
            Width = p.Width ?? SqlDecimal.Null;
            WeightGSM = p.WeightGSM ?? SqlDecimal.Null;
            GramsPerLinearMetre = p.GramsPerLinearMetre ?? SqlDecimal.Null;
            FinishedWidth = p.FinishedWidth ?? SqlDecimal.Null;
            RemeasureLength = p.RemeasureLength ?? SqlDecimal.Null;
            SeparateMaterialLength = p.SeparateMaterialLength;
            AlternativeRawMaterials = p.AlternativeRawMaterials;
            CreatedDT = p.CreatedDT ?? SqlDateTime.Null;
        }
    }
}
