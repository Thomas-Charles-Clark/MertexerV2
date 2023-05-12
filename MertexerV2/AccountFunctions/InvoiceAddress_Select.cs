using MertexerV2.GeneralFunctions;
using MertexerV2.Models;
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

namespace MertexerV2.AccountFunctions
{
    public class InvoiceAddressReference
    {
        public string InvoiceReference { get; set; }
    }

    public static partial class InvoiceAddressFunctions    
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "InvoiceAddressSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string InvoiceAddressReferences)
        {
            if (String.IsNullOrEmpty(InvoiceAddressReferences))
            {
                foreach (var q in Selector<InvoiceAddress>.SelectAllNonParallel(
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
                var results = Selector<InvoiceAddress>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<InvoiceAddressReference>>(InvoiceAddressReferences).Select(p => p.InvoiceReference),
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

        public static void InvoiceAddressSelect_FillRow(
            object obj,
            out SqlString AddressLine1,
            out SqlString AddressLine2,
            out SqlString Town,
            out SqlString County,
            out SqlString Postcode,
            out SqlString CountryCode
            )
        {
            var p = (InvoiceAddress)obj;

            AddressLine1 = p.AddressLine1?.TrimReduceTruncate(60);
            AddressLine2 = p.AddressLine2?.TrimReduceTruncate(60);
            Town = p.Town?.TrimReduceTruncate(50);
            County = p.County?.TrimReduceTruncate(40);
            Postcode = p.Postcode?.TrimReduceTruncate(20);
            CountryCode = p.CountryCode?.TrimReduceTruncate(2);
        }
    }
}