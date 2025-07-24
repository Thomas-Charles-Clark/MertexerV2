using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MertexerV2.GeneralFunctions;
using MertexerV2.Models.Pricing;
using MertexerV2.Models.SalesLedger;
using MertexerV2.PricingFunctions;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

namespace MertexerV2.SalesLedgerFunctions
{
    public class SalesLedgerTermsCode
    {
        public string Code { get; set; }
    }

    public static partial class SalesLedgerTermsFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "SalesLedgerTermsSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(
            string UniVerseAccount,
            string SalesLedgerTermsCodes)
        {
            if (String.IsNullOrEmpty(SalesLedgerTermsCodes))
            {
                foreach (var invoice in Selector<SalesLedgerTerms>.SelectAllNonParallel(
                    true,
                    false,
                    100,
                    UniVerseAccount)
                    .ToList())
                {
                    yield return invoice;
                }
            }
            else
            {
                foreach (var invoice in Selector<SalesLedgerTerms>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<SalesLedgerTermsCode>>(SalesLedgerTermsCodes).Select(p => p.Code),
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

        public static void SalesLedgerTermsSelect_FillRow(
            object obj,
            out SqlString Key,
            out SqlDecimal DiscountPercentage,
            out SqlInt32 Days,
            out SqlBoolean Print,
            out SqlString Title,
            out SqlByte DueDays,
            out SqlByte DueMonths,
            out SqlByte FixedDay,
            out SqlByte SettlementMonths,
            out SqlByte SettlementFixedDay,
            out SqlString PaymentTitle,
            out SqlDecimal CreditSurchargePercent,
            out SqlByte CreditSurchargeDays,
            out SqlByte CreditSurchargeMonths,
            out SqlByte CreditSurchargeFixedDay,
            out SqlString CreditSurchargeTitle,
            out SqlString InstallmentDueDays,
            out SqlByte NextInstallmentDueDay,
            out SqlByte SecondInstallmentDueDay,
            out SqlByte ThirdInstallmentDueDay,
            out SqlString InstallmentMonths,
            out SqlByte NextInstallmentDueMonth,
            out SqlByte SecondInstallmentDueMonth,
            out SqlByte ThirdInstallmentDueMonth,
            out SqlString InstallmentFixedDays,
            out SqlByte NextInstallmentDueFixedDay,
            out SqlByte SecondInstallmentDueFixedDay,
            out SqlByte ThirdInstallmentDueFixedDay,
            out SqlString InstallmentPercentages,
            out SqlDecimal NextInstallmentBalancePercentage,
            out SqlDecimal SecondInstallmentBalancePercentage,
            out SqlDecimal ThirdInstallmentBalancePercentage,
            out SqlBoolean DirectDebit,
            out SqlBoolean AutoDiscountTaking,
            out SqlBoolean DiscountOnTotalValue
            )
        {
            var result = (SalesLedgerTerms)obj;

            Key = result.Key?.TrimReduceTruncate(50);

            DiscountPercentage = result.DiscountPercentage ?? SqlDecimal.Null;
            Days = result.Days ?? SqlInt32.Null;
            Print = result.Print;
            Title = result.Title?.TrimReduceTruncate(100) ?? SqlString.Null;
            DueDays = result.DueDays ?? SqlByte.Null;
            DueMonths = result.DueMonths ?? SqlByte.Null;
            FixedDay = result.FixedDay ?? SqlByte.Null;
            SettlementMonths = result.SettlementMonths ?? SqlByte.Null;
            SettlementFixedDay = result.SettlementFixedDay ?? SqlByte.Null;
            PaymentTitle = result.PaymentTitle?.TrimReduceTruncate(100) ?? SqlString.Null;
            CreditSurchargePercent = result.CreditSurchargePercent ?? SqlDecimal.Null;
            CreditSurchargeDays = result.CreditSurchargeDays ?? SqlByte.Null;
            CreditSurchargeMonths = result.CreditSurchargeMonths ?? SqlByte.Null;
            CreditSurchargeFixedDay = result.CreditSurchargeFixedDay ?? SqlByte.Null;
            CreditSurchargeTitle = result.CreditSurchargeTitle?.TrimReduceTruncate(100) ?? SqlString.Null;
            InstallmentDueDays = result.InstallmentDueDays?.Any() == true ? JsonConvert.SerializeObject(result.InstallmentDueDays) : SqlString.Null;
            NextInstallmentDueDay = result.NextInstallmentDueDay ?? SqlByte.Null;
            SecondInstallmentDueDay = result.SecondInstallmentDueDay ?? SqlByte.Null;
            ThirdInstallmentDueDay = result.ThirdInstallmentDueDay ?? SqlByte.Null;
            InstallmentMonths = result.InstallmentMonths?.Any() == true ? JsonConvert.SerializeObject(result.InstallmentMonths) : SqlString.Null;
            NextInstallmentDueMonth = result.NextInstallmentDueMonth ?? SqlByte.Null;   
            SecondInstallmentDueMonth = result.SecondInstallmentDueMonth ?? SqlByte.Null;
            ThirdInstallmentDueMonth = result.ThirdInstallmentDueMonth ?? SqlByte.Null;
            InstallmentFixedDays = result.InstallmentFixedDays?.Any() == true ? JsonConvert.SerializeObject(result.InstallmentFixedDays) : SqlString.Null;
            NextInstallmentDueFixedDay = result.NextInstallmentDueFixedDay ?? SqlByte.Null;
            SecondInstallmentDueFixedDay = result.SecondInstallmentDueFixedDay ?? SqlByte.Null;
            ThirdInstallmentDueFixedDay = result.ThirdInstallmentDueFixedDay ?? SqlByte.Null;
            InstallmentPercentages = result.InstallmentPercentages?.Any() == true ? JsonConvert.SerializeObject(result.InstallmentPercentages) : SqlString.Null;
            NextInstallmentBalancePercentage = result.NextInstallmentBalancePercentage ?? SqlDecimal.Null;
            SecondInstallmentBalancePercentage = result.SecondInstallmentBalancePercentage ?? SqlDecimal.Null;
            ThirdInstallmentBalancePercentage = result.ThirdInstallmentBalancePercentage ?? SqlDecimal.Null;
            DirectDebit = result.DirectDebit;
            AutoDiscountTaking = result.AutoDiscountTaking;
            DiscountOnTotalValue = result.DiscountOnTotalValue;
        }
    }
}
