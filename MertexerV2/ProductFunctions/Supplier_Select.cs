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
    public class SupplierCode
    {
        public string Code { get; set; }
    }

    public static partial class SupplierFunctions    
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "SupplierSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string SupplierCodes)
        {
            if (String.IsNullOrEmpty(SupplierCodes))
            {
                foreach (var q in Selector<Models.Product.Supplier>.SelectAllNonParallel(
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
                var results = Selector<Models.Product.Supplier>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<SupplierCode>>(SupplierCodes).Select(p => p.Code),
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

        public static void SupplierSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name,
            out SqlString Currency,
            out SqlDecimal DutyPercentage,
            out SqlDecimal InlandChargesPercentage,
            out SqlDecimal MiscellaneousPercentage,
            out SqlDecimal ImportChargePerUnit,
            out SqlDecimal MiscellaneousCharge
            )
        {
            var p = (Models.Product.Supplier)obj;

            Code = p.Key?.TrimReduceTruncate(10);
            Name = p.Name?.TrimReduceTruncate(50);
            Currency = p.Currency?.TrimReduceTruncate(10);
            DutyPercentage = p.DutyPercentage ?? SqlDecimal.Null;
            InlandChargesPercentage = p.InlandChargesPercentage ?? SqlDecimal.Null;
            MiscellaneousPercentage = p.MiscellaneousPercentage ?? SqlDecimal.Null;
            ImportChargePerUnit = p.ImportChargePerUnit ?? SqlDecimal.Null;
            MiscellaneousCharge = p.MiscellaneousCharge ?? SqlDecimal.Null;
        }
    }
}
