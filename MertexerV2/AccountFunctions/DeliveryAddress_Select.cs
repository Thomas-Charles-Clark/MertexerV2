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
    public class DeliveryAddressReference
    {
        public string DeliveryReference { get; set; }
    }

    public static partial class DeliveryAddressFunctions    
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "DeliveryAddressSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string DeliveryAddressReferences, string TradeAccountCodes, int? SampleSize)
        {
            if (String.IsNullOrEmpty(DeliveryAddressReferences) && String.IsNullOrEmpty(TradeAccountCodes))
            {
                foreach (var q in Selector<DeliveryAddress>.SelectAllNonParallel(
                   true,
                   false,
                   50,
                   UniVerseAccount,
                   SampleSize))
                {
                    yield return q;
                }
            }
            else
            {
                if(!String.IsNullOrEmpty(DeliveryAddressReferences))
                {
                    var results = Selector<DeliveryAddress>.Select(
                        JsonConvert.DeserializeObject<IEnumerable<DeliveryAddressReference>>(DeliveryAddressReferences).Select(p => p.DeliveryReference),
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
                else
                {
                    if (!String.IsNullOrEmpty(TradeAccountCodes))
                    {
                        var batches = JsonConvert.DeserializeObject<List<TradeAccountCode>>(TradeAccountCodes).Select(k => k.Code);

                        var split = ListExtension.Split(batches, 50).ToList();

                        foreach (var batch in split)
                        {
                            var filters = new List<Tuple<string, string, string>>
                            {
                                new Tuple<string, string, string>("CUSREF", "=", String.Join(" ", batch))
                            };

                            var results = Selector<DeliveryAddress>.Filter(
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
                    }
                }
            }
        }

        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "DeliveryAddressSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Search(string UniVerseAccount, string TradeAccountCode, string SearchTerm, int? SampleSize)
        {
            SearchTerm = SearchTerm?.ToLowerInvariant();

            var filters = new List<Tuple<string, string, string>>
            {
                new Tuple<string, string, string>("CUSREF", "=", TradeAccountCode),
                new Tuple<string, string, string>($"(SS.DELADDR.MCL LIKE \"...{SearchTerm}...\")", "", "")
            };

            var results = Selector<DeliveryAddress>.Filter(
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

        public static void DeliveryAddressSelect_FillRow(
            object obj,
            out SqlString DeliveryReference,
            out SqlString TradeAccountCode,
            out SqlString ContactName,
            out SqlString CompanyName,
            out SqlString AddressLine1,
            out SqlString AddressLine2,
            out SqlString Town,
            out SqlString County,
            out SqlString Postcode,
            out SqlString CountryCode,
            out SqlString PhoneNumber,
            out SqlString Email,
            out SqlString DomesticOrTrade,
            out SqlString HaulierCode,
            out SqlString CarriageCode,
            out SqlString USTaxExemptionCertificate,
            out SqlString USStateCode,
            out SqlString USCountyCode
            )
        {
            var p = (DeliveryAddress)obj;

            DeliveryReference = p.Key.TrimReduceTruncate(20);
            TradeAccountCode = p.TradeAccountCode.TrimReduceTruncate(50);
            ContactName = p.ContactName?.TrimReduceTruncate(50);
            CompanyName = p.CompanyName?.TrimReduceTruncate(35);
            AddressLine1 = p.AddressLine1?.TrimReduceTruncate(60);
            AddressLine2 = p.AddressLine2?.TrimReduceTruncate(60);
            Town = p.Town?.TrimReduceTruncate(50);
            County = p.County?.TrimReduceTruncate(40);
            Postcode = p.Postcode?.TrimReduceTruncate(20);
            CountryCode = p.CountryCode?.TrimReduceTruncate(2);
            PhoneNumber = p.PhoneNumber?.TrimReduceTruncate(20);
            Email = p.Email?.TrimReduceTruncate(254);
            DomesticOrTrade = p.DomesticOrTrade?.TrimReduceTruncate(1); //
            HaulierCode = p.HaulierCode?.TrimReduceTruncate(10);
            CarriageCode = p.CarriageCode?.TrimReduceTruncate(20);
            USTaxExemptionCertificate = p.USTaxExemptionCertificate?.TrimReduceTruncate(100);
            USStateCode = p.USStateCode?.TrimReduceTruncate(2);
            USCountyCode = p.USCountyCode?.TrimReduceTruncate(5);
        }
    }
}