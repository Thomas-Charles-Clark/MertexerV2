using MertexerV2.Models.Accounts;
using MertexerV2.Models.DataSource;
using MertexerV2.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MertexerV2.Accounts
{
    public static partial class AccountFunctions
    {
        public static TradeAccountInvoiceAddress FetchCustomerInvoiceAddress(string tradeDataSourceJson)
        {
            var result = new TradeAccountInvoiceAddress();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            dataSources.EnquiryTypes
                       .Where(p => p.EnquiryType == EnquiryType.Address)
                       .Single()
                       .DataSources.ForEach(
            (dataSource) =>
            {
                using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
                {
                    var uvFileName = "CUSTOMER_ADDRESS";

                    var uvSelectStatement = $"{dataSource.CustomerReference}";

                    var queryResult = qe.Run(uvFileName,
                        uvSelectStatement,
                        "CUSNAME CUSADD1 CUSADD2 CUSADD3 CUSADD4 CUSPCODE COUNTRY",
                        "",
                        "XML",
                        null,
                        false,
                        60);


                    var xDoc = XDocument.Parse(queryResult);

                    var rec = xDoc.Descendants("record").FirstOrDefault();

                    if (rec != null)
                    {
                        result.CustomerName = rec.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value;
                        result.AddressLine1 = rec.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value;
                        result.AddressLine2 = rec.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value;
                        result.Town = rec.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value;
                        result.County = rec.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value;
                        result.Postcode = rec.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value;
                        result.Country = rec.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value;
                    }
                }
            });

            return result;
        }

        public static TradeAccountDeliveryAddress FetchDeliveryAddress(string tradeDataSourceJson, string orderNumber)
        {
            var result = new TradeAccountDeliveryAddress();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            dataSources.EnquiryTypes
                       .Where(p => p.EnquiryType == EnquiryType.Address)
                       .Single()
                       .DataSources.ForEach(
            (dataSource) =>
            {
                using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
                {
                    var uvFileName = "TOEHEAD";

                    var uvSelectStatement = $"SORDNO = '{orderNumber}'";

                    var queryResult = qe.Run(uvFileName,
                        uvSelectStatement,
                        "DELREF DELNAME DELADDR1 DELADDR2 DELADDR3 DELADDR4 DELPCODE DELPHONE TO.COUNTRY HAULIERDESC",
                        "",
                        "XML",
                        null,
                        false,
                        60);

                    var xDoc = XDocument.Parse(queryResult);

                    var rec = xDoc.Descendants("record").FirstOrDefault();

                    if (rec != null)
                    {
                        result.DeliveryReference = rec.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value;
                        result.DeliveryName = rec.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value;
                        result.AddressLine1 = rec.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value;
                        result.AddressLine2 = rec.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value;
                        result.Town = rec.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value;
                        result.County = rec.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value;
                        result.Postcode = rec.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value;
                        result.Phone = rec.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value;
                        result.Country = rec.XPathSelectElement("descendant::fd[@id='C9']").Attribute("value").Value;
                        result.HaulierDescription = rec.XPathSelectElement("descendant::fd[@id='C10']").Attribute("value").Value;
                    }
                }
            });

            return result;
        }

    }
}
