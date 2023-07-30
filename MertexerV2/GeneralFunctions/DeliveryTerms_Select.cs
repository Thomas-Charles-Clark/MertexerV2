using MertexerV2.Models;
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
    public class DeliveryTermsCode
    {
        public string Code { get; set; }
    }

    public static partial class DeliveryTermsFunctions
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "DeliveryTermsSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string DeliveryTermsCodes)
        {
            if (String.IsNullOrEmpty(DeliveryTermsCodes))
            {
                foreach (var q in Selector<DeliveryTerms>.SelectAllNonParallel(
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
                var results = Selector<DeliveryTerms>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<DeliveryTermsCode>>(DeliveryTermsCodes).Select(p => p.Code),
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

        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "DeliveryTermsSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Search(string UniVerseAccount, string SearchTerm, int? SampleSize)
        {
            SearchTerm = SearchTerm?.ToLowerInvariant();

            var filters = new List<Tuple<string, string, string>>
            {
                new Tuple<string, string, string>($"(SS.DeliveryTerms.MCL LIKE \"...{SearchTerm}...\")", "", "")
            };

            var results = Selector<DeliveryTerms>.Filter(
                filters,
                true,
                false,
                50,
                UniVerseAccount,
                SampleSize)
                .ToList();

            foreach (var r in results)
            {
                yield return r;
            }
        }

        public static void DeliveryTermsSelect_FillRow(
            object obj,
            out SqlByte DeliveryTermID,
            out SqlString Code,
            out SqlByte SortOrder,
            out SqlString Description
            )
        {
            var p = (DeliveryTerms)obj;
            
            DeliveryTermID = Convert.ToByte(p.Key);
            Code = p.Code?.TrimReduceTruncate(3);
            SortOrder = p.SortOrder;
            Description = p.Description?.TrimReduceTruncate(30);
        }
    }
}
