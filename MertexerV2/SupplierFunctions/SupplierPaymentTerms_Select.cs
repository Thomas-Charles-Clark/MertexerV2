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
    public class SupplierPaymentTermsCode
    {
        public string Code { get; set; }
    }

    public static partial class SupplierPaymentTermsFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "SupplierPaymentTermsSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string SupplierTypeCodes)
        {
            if (String.IsNullOrEmpty(SupplierTypeCodes))
            {
                foreach (var invoice in Selector<SupplierPaymentTerms>.SelectAllNonParallel(
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
                foreach (var invoice in Selector<SupplierPaymentTerms>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<SupplierPaymentTermsCode>>(SupplierTypeCodes).Select(p => p.Code),
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

        public static void SupplierPaymentTermsSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlDecimal DiscountPercentage,
            out SqlBoolean Print,
            out SqlString Title,
            out SqlByte Days,
            out SqlByte Months,
            out SqlByte FixedDay,
            out SqlByte SettlementDays,
            out SqlByte SettlementMonths,
            out SqlByte SettlementFixedDay,
            out SqlString PaymentTitle
            )
        {
            var result = (SupplierPaymentTerms)obj;

            Code = result.Key?.TrimReduceTruncate(10);

            DiscountPercentage = result.DiscountPercentage ?? SqlDecimal.Null;

            SettlementDays = result.Days ?? SqlByte.Null;

            Print = result.Print;

            Title = result.Title?.TrimReduceTruncate(50);

            Days = result.DueDays ?? SqlByte.Null;

            Months = result.Months ?? SqlByte.Null;

            FixedDay = result.FixedDay ?? SqlByte.Null;

            SettlementMonths = result.SettlementMonths ?? SqlByte.Null;

            SettlementFixedDay = result.SettlementFixedDay ?? SqlByte.Null;

            PaymentTitle = result.PaymentTitle?.TrimReduceTruncate(50);
        }
    }
}
