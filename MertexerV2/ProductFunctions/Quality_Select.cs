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
    public static partial class QualityFunctions
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
            out SqlString Name,
            out SqlString CompositionCode,
            out SqlDecimal Width,
            out SqlDecimal WeightGramsPerSquareMetre,
            out SqlDecimal WeightGramsPerLinearMetre,
            out SqlDecimal FinishedWidth,
            out SqlDecimal RemeasureLength,
            out SqlBoolean SeparateMaterialLength,
            out SqlBoolean AlternativeRawMaterials,
            out SqlDateTime CreatedDT
            )
        {
            var p = (Quality)obj;

            Quality = p.Key?.TrimReduceTruncate(50);
            Name = p.Name?.TrimReduceTruncate(100);
            CompositionCode = p.CompositionCode?.TrimReduceTruncate(10);
            Width = p.Width ?? SqlDecimal.Null;
            WeightGramsPerSquareMetre = p.WeightGramsPerSquareMetre ?? SqlDecimal.Null;
            WeightGramsPerLinearMetre = p.WeightGramsPerLinearMetre ?? SqlDecimal.Null;
            FinishedWidth = p.FinishedWidth ?? SqlDecimal.Null;
            RemeasureLength = p.RemeasureLength ?? SqlDecimal.Null;
            SeparateMaterialLength = p.SeparateMaterialLength;
            AlternativeRawMaterials = p.AlternativeRawMaterials;
            CreatedDT = p.CreatedDT ?? SqlDateTime.Null;
        }
    }
}
