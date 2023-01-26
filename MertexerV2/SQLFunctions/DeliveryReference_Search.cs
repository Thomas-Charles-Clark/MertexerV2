using MertexerV2.Models;
using MertexerV2.Models.DataSource;
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
using System.Xml.XPath;

namespace MertexerV2.SQLFunctions
{
    public static class DeliveryReference
    {

        [Microsoft.SqlServer.Server.SqlFunction(DataAccess = DataAccessKind.Read)]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static bool Check(string TradeDataSourceJSON, string DeliveryReference)
        {
            // This... potentially could be NULL because of the context of the stored procedure which calls this function
            if (String.IsNullOrEmpty(TradeDataSourceJSON))
            {
                return false;
            }

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(TradeDataSourceJSON);

            var dataSource = dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Address)
                                        .Single()
                                        .DataSources
                                        .Single();

            var found = false;

            using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
            {
                var xDoc = XDocument.Parse(
                    qe.Run(
                        "DELADD",
                        $"CUSREF = \"{dataSource.CustomerReference}\" AND DELREF = \"{DeliveryReference}\"",
                        "DELREF"));

                xDoc.Descendants("record").ToList().ForEach(c =>
                {
                    found = c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value == DeliveryReference;
                });
            }

            return found;
        }

        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "Search_FillRow",
            TableDefinition = "DeliveryReference NVARCHAR(20)")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Search(
            string TradeDataSourceJSON, 
            string AddressLine1, 
            string PostalCode, 
            string CountryCode)
        {
            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(TradeDataSourceJSON);

            var dataSource = dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Address)
                                        .Single()
                                        .DataSources
                                        .Single();

            var results = new ArrayList();

            var delrefs = "";

            using (var qe = new CommandExecutor(dataSource.UniVerseAccount))
            {
                qe.Arbitrary("CASE OFF");

                var command = $"SELECT DELREF FROM DELADD WHERE CUSREF = '{dataSource.CustomerReference}'" +
                    (!String.IsNullOrEmpty(AddressLine1) ? $" AND DELADDR1 LIKE '{AddressLine1}' " : "") + 
                    (!String.IsNullOrEmpty(PostalCode) ? $" AND DELPCODE LIKE '{PostalCode}' " : "") + 
                    $" AND COUNTRY = '{CountryCode}' ORDER BY DELREF NOPAGE SUPPRESS COLUMN HEADING COUNT.SUP;";

                delrefs = qe.Arbitrary(command);
                delrefs = delrefs.Trim();
            }

            var preResults = delrefs.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            var delim = new char[] { '-' };

            preResults = preResults.OrderBy<string, int>(p => {
                var n = p.Split(delim);
                if (n.Length == 2)
                {
                    if (Int32.TryParse(n[1], out int i))
                    {
                        return i;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }).ToArray();

            var addresses = new List<DeliveryAddress>();
            
            foreach(var delref in preResults)
            {
                using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
                {
                    var addressDoc = XDocument.Parse(qe.Run("DELADD",
                           $"DELREF = \"{delref}\"",
                           "DELADDR1 DELADDR2 DELADDR3 DELADDR4 DELPCODE COUNTRY CONTACT DELNAME PHONE E.MAIL DELREF SS.LASTCONF.DATE"));

                    if (addressDoc.Descendants("record").Any())
                    {
                        addresses.Add(new DeliveryAddress(
                            dataSource.CustomerReference,
                            addressDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value,
                            addressDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value,
                            addressDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value,
                            addressDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value,
                            addressDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value,
                            addressDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value,
                            addressDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value,
                            addressDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value,
                            addressDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C9']").Attribute("value").Value,
                            addressDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C10']").Attribute("value").Value,
                            deliveryReference: addressDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value,
                            domesticOrTrade: null,
                            haulierCode: null,
                            carriageCode: null,
                            usStateCode: null,
                            usCountyCode: null,
                            usTaxExemptionCertificateReference: null,
                            addressDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C12']").Attribute("value").Value
                        ));
                    }
                }
            }

            results.AddRange(addresses);

            return results;
        }

        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "Search_FillRow",
            TableDefinition = "DeliveryReference NVARCHAR(20)")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(
            string TradeAccountCode,
            string DeliveryReference)
        {
            var results = new ArrayList();

            using (var qe = new QueryExecutor())
            {
                var addressDoc = XDocument.Parse(qe.Run("DELADD",
                        String.IsNullOrEmpty(DeliveryReference) ? $"CUSREF = {TradeAccountCode}" : 
                        $"DELREF = \"{DeliveryReference}\" AND CUSREF = {TradeAccountCode}",
                         "DELADDR1 DELADDR2 DELADDR3 DELADDR4 DELPCODE COUNTRY CONTACT DELNAME PHONE E.MAIL DELREF SS.LASTCONF.DATE"));

                if (addressDoc.Descendants("record").Any())
                {
                    if (!String.IsNullOrEmpty(DeliveryReference))
                    {
                        results.Add(new DeliveryAddress(
                            tradeAccountCode: TradeAccountCode,
                            addressLine1: addressDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value,
                            addressLine2: addressDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value,
                            town: addressDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value,
                            county: addressDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value,
                            postcode: addressDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value,
                            countryCode: addressDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value,
                            contactName: addressDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value,
                            companyName: addressDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value,
                            phoneNumber: addressDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C9']").Attribute("value").Value,
                            email: addressDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C10']").Attribute("value").Value,
                            deliveryReference: addressDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value,
                            domesticOrTrade: null,
                            haulierCode: null,
                            carriageCode: null,
                            usStateCode: null,
                            usCountyCode: null,
                            usTaxExemptionCertificateReference: null,
                            addressDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C12']").Attribute("value").Value
                        ));
                    }
                    else
                    {
                        addressDoc.Descendants("record").ToList().ForEach(delrec =>
                        {
                            results.Add(new DeliveryAddress(
                                tradeAccountCode: TradeAccountCode,
                                addressLine1: delrec.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value,
                                addressLine2: delrec.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value,
                                town: delrec.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value,
                                county: delrec.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value,
                                postcode: delrec.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value,
                                countryCode: delrec.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value,
                                contactName: delrec.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value,
                                companyName: delrec.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value,
                                phoneNumber: delrec.XPathSelectElement("descendant::fd[@id='C9']").Attribute("value").Value,
                                email: delrec.XPathSelectElement("descendant::fd[@id='C10']").Attribute("value").Value,
                                deliveryReference: delrec.XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value,
                                domesticOrTrade: null,
                                haulierCode: null,
                                carriageCode: null,
                                usStateCode: null,
                                usCountyCode: null,
                                usTaxExemptionCertificateReference: null,
                                delrec.XPathSelectElement("descendant::fd[@id='C12']").Attribute("value").Value
                            ));
                        });
                    }
                }
            }

            return results;
        }

        public static void Search_FillRow(
            object obj,
            out SqlString DeliveryReference,
            out SqlString CompanyName,
            out SqlString ContactName,
            out SqlString AddressLine1,
            out SqlString AddressLine2,
            out SqlString Town,
            out SqlString County,
            out SqlString Postcode,
            out SqlString CountryCode,
            out SqlString PhoneNumber,
            out SqlString Email,
            out SqlDateTime LastConfirmedDT
            )
        {
            var Address = (DeliveryAddress)obj;

            DeliveryReference = Address.DeliveryReference;
            CompanyName = String.IsNullOrEmpty(Address.CompanyName) ? SqlString.Null : Address.CompanyName.TrimReduceTruncate(35);
            ContactName = String.IsNullOrEmpty(Address.ContactName) ? SqlString.Null : Address.ContactName.TrimReduceTruncate(50);
            AddressLine1 = String.IsNullOrEmpty(Address.AddressLine1) ? SqlString.Null : Address.AddressLine1.TrimReduceTruncate(60);
            AddressLine2 = String.IsNullOrEmpty(Address.AddressLine2) ? SqlString.Null : Address.AddressLine2.TrimReduceTruncate(60);
            Town = String.IsNullOrEmpty(Address.Town) ? SqlString.Null : Address.Town.TrimReduceTruncate(50);
            County = String.IsNullOrEmpty(Address.County) ? SqlString.Null : Address.County.TrimReduceTruncate(40);
            Postcode = String.IsNullOrEmpty(Address.Postcode) ? SqlString.Null : Address.Postcode.TrimReduceTruncate(20);
            CountryCode = String.IsNullOrEmpty(Address.CountryCode) ? SqlString.Null : Address.CountryCode.TrimReduceTruncate(2);
            PhoneNumber = String.IsNullOrEmpty(Address.PhoneNumber) ? SqlString.Null : Address.PhoneNumber.TrimReduceTruncate(20);
            Email = String.IsNullOrEmpty(Address.Email) ? SqlString.Null : Address.Email; // Not reduced - NVARCHAR(MAX).
            LastConfirmedDT = Address.LastConfirmedDT ?? SqlDateTime.Null;
        }
    }
}
