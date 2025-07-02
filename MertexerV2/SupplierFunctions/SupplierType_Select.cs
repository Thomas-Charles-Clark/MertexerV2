using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MertexerV2.GeneralFunctions;
using MertexerV2.Models.PurchaseLedger;
using MertexerV2.Models.Supplier;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

namespace MertexerV2.SupplierFunctions
{
    public class SupplierTypeCode
    {
        public string Code { get; set; }
    }

    public static partial class SupplierTypeFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "SupplierTypeSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string SupplierTypeCodes)
        {
            if (String.IsNullOrEmpty(SupplierTypeCodes))
            {
                foreach (var invoice in Selector<SupplierType>.SelectAllNonParallel(
                    true,
                    false,
                    100,
                    UniVerseAccount))
                {
                    yield return invoice;
                }
            }
            else
            {
                foreach (var invoice in Selector<PurchaseLedgerInvoiceRegister>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<SupplierTypeCode>>(SupplierTypeCodes).Select(p => p.Code),
                    true,
                    false,
                    100,
                    UniVerseAccount)
                    .ToList())
                {
                    yield return invoice;
                }
            }
        }

        public static void SupplierTypeSelect_FillRow(
            object obj,
            out SqlString SupplierTypeCode,
            out SqlString Name
            )
        {
            var result = (SupplierType)obj;

            SupplierTypeCode = result.Key?.TrimReduceTruncate(10);

            Name = result.Description?.TrimReduceTruncate(50);
        }
    }
}
