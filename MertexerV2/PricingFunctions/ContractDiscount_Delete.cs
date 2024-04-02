using MertexerV2.GeneralFunctions;
using MertexerV2.Models.Pricing;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace MertexerV2.PricingFunctions
{
    public static partial class ContractDiscountFunctions
    {
        private static string fileName = typeof(ContractDiscount).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                           .Cast<UniVerseFileNameAttribute>()
                           .Single()
                           .FileName;

        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "ContractDiscountSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static int Delete(string UniVerseAccount, string TradeAccountCode, string ContractDiscountCodes)
        {
            // Well, this has to be done *really really carefully*

            var preResults = new List<ContractDiscount>();

            if (String.IsNullOrEmpty(ContractDiscountCodes))
            {
                if (String.IsNullOrEmpty(TradeAccountCode))
                {
                    throw new ArgumentNullException($"You must provide either TradeAccountCode or ContractDiscountCodes, or both");
                }
                else
                {
                    var filters = new List<Tuple<string, string, string>>
                        {
                            new Tuple<string, string, string>("CUSNO", "=", TradeAccountCode)
                        };

                    preResults = Selector<ContractDiscount>.Filter(
                        filters,
                        true,
                        false,
                        100,
                        UniVerseAccount)
                        .ToList();
                }
            }
            else
            {
                preResults = Selector<ContractDiscount>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<ContractDiscountCode>>(ContractDiscountCodes).Select(p => p.Code),
                    true,
                    false,
                    100,
                    UniVerseAccount)
                    .ToList();

                if (!String.IsNullOrEmpty(TradeAccountCode))
                {
                    preResults = preResults.Where(r => r.TradeAccountCode == TradeAccountCode).ToList();
                }
            }

            if (preResults.Any())
            {
                using (var ce = new CommandExecutor(UniVerseAccount))
                {
                    var uniFile = ce.Session.CreateUniFile(fileName);

                    preResults.ForEach(pr =>
                    {
                        uniFile.DeleteRecord(pr.Key);
                    });
                }
            }

            return 0;
        }
    }
}
