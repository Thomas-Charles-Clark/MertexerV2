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
        public static ListInvoicesReturn ListInvoices(string tradeDataSourceJson, string criteria)
        {
            var result = new ListInvoicesReturn();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Invoice)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
                {
                    var xDoc = XDocument.Parse(qe.Run("LIST_INVOICES",
                        $"{criteria} BY DOCKEY",
                        "DOCKEY DOCDATE DUEDATE ORDERNO CUS.OWN.REF WI.ORIGVAL WI.OSAMOUNT WI.DISPUTED",
                        "",
                        "XML",
                        "",
                        false,
                        120));

                    var invoices = new List<InvoiceResult>();

                    xDoc.Descendants("record").ToList().ForEach(c =>
                    {
                        System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-GB");

                        var docDate = DateTime.ParseExact(c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value, "dd/MM/yy", ci.DateTimeFormat);

                        DateTime? dueDate = null;

                        if (!String.IsNullOrEmpty(c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value))
                        {
                            dueDate = DateTime.ParseExact(c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value, "dd/MM/yy", ci.DateTimeFormat);
                        }

                        invoices.Add(new InvoiceResult()
                        {
                            DocumentNumber = c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value,
                            DocumentDate = docDate,
                            DueDate = dueDate,
                            OrderNumber = c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value,
                            CustomerOrderReference = c.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value,
                            OriginalValue = Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value),
                            OutstandingAmount = Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value),
                            Disputed = c.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value
                        });
                    });

                    lock (locker)
                    {
                        result.Invoices.AddRange(invoices);
                    }
                }
            });

            result.OnAccountTotal = result.Invoices.Select(p => p.OutstandingAmount).Sum();

            return result;
        }

        public static ListInvoicesReturn OutstandingInvoices(string tradeDataSourceJson)
        {
            var result = new ListInvoicesReturn();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Invoice)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
                {
                    var xDoc = XDocument.Parse(qe.Run("LIST_INVOICES",
                        $"CUSREF.OS = \"{dataSource.CustomerReference}\" AND WI.OSAMOUNT # \"0\" BY DOCKEY",
                        "DOCKEY DOCDATE DUEDATE ORDERNO CUS.OWN.REF WI.ORIGVAL WI.OSAMOUNT WI.DISPUTED",
                        "",
                        "XML",
                        "",
                        false,
                        120));

                    xDoc.Descendants("record").ToList().ForEach(c =>
                    {
                        System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-GB");

                        var docDate = DateTime.ParseExact(c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value, "dd/MM/yy", ci.DateTimeFormat);

                        DateTime? dueDate = null;

                        if (!String.IsNullOrEmpty(c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value))
                        {
                            dueDate = DateTime.ParseExact(c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value, "dd/MM/yy", ci.DateTimeFormat);
                        }

                        result.Invoices.Add(new InvoiceResult()
                        {
                            DocumentNumber = c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value,
                            DocumentDate = docDate,
                            DueDate = dueDate,
                            OrderNumber = c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value,
                            CustomerOrderReference = c.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value,
                            OriginalValue = Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value),
                            OutstandingAmount = Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value),
                            Disputed = c.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value
                        });
                    });
                }

                var recentPaymentsResult = AccountSummaryRecentPayments(dataSource.UniVerseAccount, dataSource.CustomerReference, true);

                result.OnAccountTotal = recentPaymentsResult.PaymentsOnAccount.Select(p => p.OutstandingAmount).Sum();

            });

            return result;
        }

        public static InvoiceSearchReturn InvoiceSearch(string tradeDataSourceJson, PassInvoiceSearch objSearch)
        {
            var result = new InvoiceSearchReturn();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

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

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Invoice)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
                {
                    var selectionCriteria = $"CUSREF = \"{dataSource.CustomerReference}\" ";

                    switch (objSearch.dateRangeType)
                    {
                        case 1: // Shipment Date
                            selectionCriteria += $"AND WI.DESPDATE >= '{from}' AND WI.DESPDATE <= '{to}' ";
                            break;
                        case 2: // Invoice Date
                            selectionCriteria += $"AND DOCDATE >= '{from}' AND DOCDATE <= '{to}' ";
                            break;
                        case 3: // Due Date
                            selectionCriteria += $"AND DUEDATE >= '{from}' AND DUEDATE <= '{to}' ";
                            break;
                    }

                    if (!String.IsNullOrEmpty(objSearch.searchTerm))
                    {
                        selectionCriteria += $" AND ((DOCKEY = '{objSearch.searchTerm.ToUpperInvariant()}') OR (ORDERNO = '{objSearch.searchTerm.ToUpperInvariant()}') OR (PLIST = '{objSearch.searchTerm.ToUpperInvariant()}') OR (WI.SEARCH.REF = '[{objSearch.searchTerm.ToUpperInvariant()}]' OR CUS.OWN.REF = '[{objSearch.searchTerm.ToUpperInvariant()}]')) ";
                    }

                    selectionCriteria += " BY DOCKEY";

                    var xDoc = XDocument.Parse(qe.Run("LIST_INVOICES",
                        selectionCriteria,
                        "DOCKEY DOCDATE DUEDATE ORDERNO CUS.OWN.REF WI.ORIGVAL WI.OSAMOUNT WI.DISPUTED",
                        "",
                        "XML",
                        "",
                        false,
                        120));

                    var searchResults = new List<InvoiceResult>();

                    xDoc.Descendants("record").ToList().ForEach(c =>
                    {
                        System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-GB");

                        var docDate = DateTime.ParseExact(c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value, "dd/MM/yy", ci.DateTimeFormat);

                        DateTime? dueDate = null;

                        if (!String.IsNullOrEmpty(c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value))
                        {
                            dueDate = DateTime.ParseExact(c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value, "dd/MM/yy", ci.DateTimeFormat);
                        }

                        searchResults.Add(new InvoiceResult()
                        {
                            DocumentNumber = c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value,
                            DocumentDate = docDate,
                            DueDate = dueDate,
                            OrderNumber = c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value,
                            CustomerOrderReference = c.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value,
                            OriginalValue = Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value),
                            OutstandingAmount = Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value),
                            Disputed = c.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value
                        });
                    });

                    lock (locker)
                    {
                        result.SearchResults.AddRange(searchResults);
                    }
                }
            });

            return result;
        }
    }
}
