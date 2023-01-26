using MertexerV2.Models.Accounts;
using MertexerV2.Models.DataSource;
using MertexerV2.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MertexerV2.Accounts
{
    public static partial class AccountFunctions
    {
        public static ListProformaInvoicesReturn ProformaInvoices(string tradeDataSourceJson, PassProformaInvoiceSearch objSearch = null)
        {
            var result = new ListProformaInvoicesReturn();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Invoice)
                                        .Single()
                                        .DataSources, 
            (dataSource) =>
            {
                var searchCriteria = "";

                if (objSearch != null)
                {
                    DateTime fromDt = DateTime.Now;
                    DateTime toDt = DateTime.Now;

                    switch (objSearch.dateRange)
                    {
                        case 12:
                            fromDt = DateTime.Now.AddYears(-1);
                            break;
                        case 6:
                            fromDt = DateTime.Now.AddMonths(-6);
                            break;
                        case 1:
                            fromDt = DateTime.Now.AddDays(-DateTime.Now.Day).AddDays(1);// DateTime.Now.AddMonths(-1);
                            break;
                        case -1:
                            fromDt = objSearch.fromDate ?? DateTime.Now.AddMonths(-1);
                            toDt = objSearch.toDate ?? DateTime.Now;
                            break;
                    }

                    var from = fromDt.ToString("dd/MM/yyyy");
                    var to = toDt.ToString("dd/MM/yyyy");

                    searchCriteria += $"AND DATEONFILE >= \"{from}\" AND DATEONFILE <= \"{to}\" ";

                    if (!String.IsNullOrEmpty(objSearch.orderStatus))
                    {
                        if (objSearch.orderStatus == "outstanding")
                            searchCriteria = $"{searchCriteria} AND ORDERTYPE = \"P\"";
                        if (objSearch.orderStatus == "released")
                            searchCriteria = $"{searchCriteria} AND ORDERTYPE # \"P\"";
                    }
                    else
                    {
                        searchCriteria = $"{searchCriteria} AND ORDERTYPE # ''";
                    }
                }

                using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
                {
                    var searchCriteriaFinal = $"CUSREF " +
                    $"= \"{dataSource.CustomerReference}\" {searchCriteria} BY CUSREF.PROF BY-DSND DATEONFILE";

                    var xDoc = XDocument.Parse(qe.Run("LIST_PROFORMAS",
                        searchCriteriaFinal,
                        "SORDNO DATEONFILE CUSORDNO T.GROSS.CUR ORDERTYPE"));

                    var invoices = new List<ProformaInvoiceResult>();

                    xDoc.Descendants("record").ToList().ForEach(c =>
                    {
                        System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-GB");

                        var docDate = DateTime.ParseExact(c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value, "dd/MM/yy", ci.DateTimeFormat);
                        
                        invoices.Add(new ProformaInvoiceResult()
                        {
                            OrderNumber = c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value,
                            DocumentKey = System.Web.HttpUtility.UrlEncode(Cryptography.Encrypt(c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value)),
                            DocumentDate = docDate,
                            CustomerOrderReference = c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value,
                            Value = Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value),
                            OrderType = c.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value
                        });
                    });

                    if (objSearch.searchTerm != null)
                    {
                        invoices = invoices.Where(p => p.CustomerOrderReference.ToLowerInvariant()
                                           .Contains(objSearch.searchTerm.ToLowerInvariant()))
                                           .ToList();
                    }

                    lock (locker)
                    {
                        result.Invoices.AddRange(invoices);
                    }
                }
            });

            result.OnAccountTotal = result.Invoices.Select(p => p.Value).Sum();

            return result;
        }
    }
}
