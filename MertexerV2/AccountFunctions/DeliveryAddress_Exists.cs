using MertexerV2.GeneralFunctions;
using MertexerV2.InvoiceFunctions;
using MertexerV2.Models;
using MertexerV2.Models.EDAReplacement;
using MertexerV2.Models.Invoices;
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

namespace MertexerV2.AccountFunctions
{
    public static partial class DeliveryAddressFunctions
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "DeliveryAddressExists_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Exists(
            string UniVerseAccount, 
            string TradeAccountCode,
            string CompanyName,
            string ContactName, 
            string AddressLine1,
            string Addressline2,
            string Town,
            string County,
            string Postcode,
            string CountryCode)
        {
            var searchTerm = $"\"{TradeAccountCode} {CompanyName} {ContactName} {AddressLine1} {Addressline2} {Town} {County} {Postcode} {CountryCode}\"";

            searchTerm = searchTerm?.ToLowerInvariant();

            var filters = new List<Tuple<string, string, string>>
            {
                new Tuple<string, string, string>($"SS.EXTANT.MCL", "=", searchTerm)
            };

            var results = Selector<DeliveryAddress>.Filter(
                filters,
                true,
                false,
                50,
                UniVerseAccount,
                100)
                .ToList();

            foreach (var r in results)
            {
                yield return r;
            }
        }

        public static void DeliveryAddressExists_FillRow(
            object obj,
            out SqlString DeliveryReference
            )
        {
            var p = (DeliveryAddress)obj;

            DeliveryReference = p.Key.TrimReduceTruncate(20);
        }
    }
}