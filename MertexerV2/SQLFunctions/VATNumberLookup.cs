using MertexerV2.Models.DataSource;
using MertexerV2.Models.VAT;
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
    public static class VAT
    {
        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "VATNumberLookup_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Lookup(string TradeDataSourceJSON)
        {
            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(TradeDataSourceJSON);

            var dataSource = dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Account)
                                        .Single()
                                        .DataSources
                                        .Single();

            var result = new ArrayList();

            using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
            {
                var xDoc = XDocument.Parse(
                    qe.Run(
                        "SLCUST",
                        $"CUSREF=\"{dataSource.CustomerReference}\"",
                        "VATCOUNTRY INVVATDISPLAY",
                        "",
                        "XML"
                    )
                );

                result.Add(new VATNumberLookupResult() 
                { 
                    CountryCode = xDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value,
                    Number = xDoc.Descendants("record").First().XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value
                });
            }

            return result;
        }

        public static void VATNumberLookup_FillRow(
            object obj,
            out SqlString VATCountryCode,
            out SqlString VATNumber)
        {
            var result = (VATNumberLookupResult)obj;

            VATCountryCode = result.CountryCode;
            VATNumber = result.Number;
        }
    }
}
