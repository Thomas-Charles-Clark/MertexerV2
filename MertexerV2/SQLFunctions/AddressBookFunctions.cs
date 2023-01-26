using MertexerV2.Models;
using MertexerV2.Models.Address;
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

namespace MertexerV2
{
    public static class AddressBookFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(DataAccess = DataAccessKind.Read)]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static string Create(
            string TradeDataSourceJSON,
            string AddressLine1,
            string AddressLine2,
            string Town,
            string County,
            string Postcode,
            string CountryCode,
            string ContactName = null,
            string CompanyName = null,
            string PhoneNumber = null,
            string Email = null)
        {
            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(TradeDataSourceJSON);

            var dataSource = dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Address)
                                        .Single()
                                        .DataSources
                                        .Single();

            return AddDeliveryAddress(
                dataSource.UniVerseAccount,
                new DeliveryAddress(
                    tradeAccountCode: dataSource.CustomerReference,
                    addressLine1: AddressLine1,
                    addressLine2: AddressLine2,
                    town: Town,
                    county: County,
                    postcode: Postcode,
                    countryCode: CountryCode,
                    contactName: ContactName,
                    companyName: CompanyName,
                    phoneNumber: PhoneNumber,
                    email: Email));
        }

        [Microsoft.SqlServer.Server.SqlFunction(DataAccess = DataAccessKind.Read)]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static string Update(
            string TradeDataSourceJSON,
            string DeliveryReference,
            string AddressLine1,
            string AddressLine2,
            string Town,
            string County,
            string Postcode,
            string CountryCode,
            string ContactName = null,
            string CompanyName = null,
            string PhoneNumber = null,
            string Email = null)
        {
            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(TradeDataSourceJSON);

            var dataSource = dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Address)
                                        .Single()
                                        .DataSources
                                        .Single();

            return UpdateDeliveryAddress(
                dataSource.UniVerseAccount,
                new DeliveryAddress(
                    tradeAccountCode: dataSource.CustomerReference,
                    addressLine1: AddressLine1,
                    addressLine2: AddressLine2,
                    town: Town,
                    county: County,
                    postcode: Postcode,
                    countryCode: CountryCode,
                    contactName: ContactName,
                    companyName: CompanyName,
                    phoneNumber: PhoneNumber,
                    email: Email,
                    deliveryReference: DeliveryReference));
        }

        public static string AddDeliveryAddress(string uniVerseAccount, DeliveryAddress objAdd)
        {
            using (var qe = new QueryExecutor(uniVerseAccount))
            {
                var result = qe.Run("DELADD_MAINT",
                    "",
                    "",
                    objAdd.InsertWebData());

                return result;
            }
        }

        public static string UpdateDeliveryAddress(string uniVerseAccount, DeliveryAddress objAdd)
        {
            using (var qe = new QueryExecutor(uniVerseAccount))
            {
                var result = qe.Run("DELADD_MAINT",
                    "",
                    "",
                    objAdd.UpdateWebData());

                return result;
            }
        }

        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "FetchInvoiceAddress_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchInvoiceAddress(string TradeDataSourceJSON)
        {
            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(TradeDataSourceJSON);

            var dataSource = dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Address)
                                        .Single()
                                        .DataSources
                                        .Single();

            var results = new List<InvoiceAddress>();

            using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
            {
                var addressDoc = XDocument.Parse(Framework.RemoveInvalidXmlChars(qe.Run(
                    "SLCUST",
                    $"@ID = \"{dataSource.CustomerReference}\"",
                     "CUSADD1X CUSADD2X CUSADD3X CUSADD4X CUSPCODE COUNTRY")));

                if (addressDoc.Descendants("record").Count() != 1)
                {
                    throw new Exception($"ERROR Checking SLCUST in {dataSource.UniVerseAccount} for Invoice Address belonging to {dataSource.CustomerReference}");
                }
                else
                {
                    var da = addressDoc.Descendants("record").Single();

                    results.Add(new InvoiceAddress(
                        addressLine1: String.IsNullOrEmpty(da.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value) ? null :
                                                           da.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value,
                        addressLine2: String.IsNullOrEmpty(da.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value) ? null :
                                                           da.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value,
                        town: String.IsNullOrEmpty(da.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value) ? null :
                                                   da.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value,
                        county: String.IsNullOrEmpty(da.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value) ? null :
                                                     da.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value,
                        postcode: String.IsNullOrEmpty(da.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value) ? null :
                                                       da.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value,
                        countryCode: String.IsNullOrEmpty(da.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value) ? null :
                                                          da.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value
                    ));
                }
            }

            return new ArrayList(results);
        }

        public static void FetchInvoiceAddress_FillRow(
            object obj,
            out SqlString AddressLine1,
            out SqlString AddressLine2,
            out SqlString Town,
            out SqlString County,
            out SqlString Postcode,
            out SqlString CountryCode
            )
        {
            var invAddr = (InvoiceAddress)obj;

            AddressLine1 = invAddr.AddressLine1;
            AddressLine2 = invAddr.AddressLine2;
            Town = invAddr.Town;
            County = invAddr.County;
            Postcode = invAddr.Postcode;
            CountryCode = invAddr.CountryCode;
        }
    }
}
