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
using System.Xml.Linq;

namespace MertexerV2.ProductFunctions
{
    public class MakeDetailCode
    {
        public string Code { get; set; }
    }

    public static partial class MakeDetailFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "MakeDetailSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string MakeDetailCodes)
        {
            if (String.IsNullOrEmpty(MakeDetailCodes))
            {
                foreach (var q in Selector<MakeDetail>.SelectAllNonParallel(
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
                var results = Selector<MakeDetail>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<MakeDetailCode>>(MakeDetailCodes).Select(p => p.Code),
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

        public static void MakeDetailSelect_FillRow(
            object obj,
            out SqlString ProductProcessID,
            out SqlString ProductCode,
            out SqlString ProcessCode,
            out SqlString ProductCodes,
            out SqlString Quantities,
            out SqlString Units,
            out SqlString Comments,
            out SqlString HiddenReadField,
            out SqlString AgreedPercentageLosses,
            out SqlString ExpectedPercentageLosses
            )
        {
            var p = (MakeDetail)obj;

            ProductProcessID = p.Key?.TrimReduceTruncate(50);
            ProductCode = p.ProductCode?.TrimReduceTruncate(20);
            ProcessCode = p.ProcessCode?.TrimReduceTruncate(20);
            ProductCodes = p.ProductCodes.Any() ? JsonConvert.SerializeObject(p.ProductCodes) : SqlString.Null;
            Quantities = p.Quantities.Any() ? JsonConvert.SerializeObject(p.Quantities) : SqlString.Null;
            Units = p.Units.Any() ? JsonConvert.SerializeObject(p.Units) : SqlString.Null;
            Comments = p.Comments.Any() ? JsonConvert.SerializeObject(p.Comments) : SqlString.Null;
            HiddenReadField = p.HiddenReadField?.TrimReduceTruncate(1);
            AgreedPercentageLosses = p.AgreedPercentageLosses.Any() ? JsonConvert.SerializeObject(p.AgreedPercentageLosses) : SqlString.Null;
            ExpectedPercentageLosses = p.ExpectedPercentageLosses.Any() ? JsonConvert.SerializeObject(p.ExpectedPercentageLosses) : SqlString.Null;
        }
    }
}