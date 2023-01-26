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
        public static AccountSummaryReturn AccountSummary(string tradeDataSourceJson)
        {
            var result = new AccountSummaryReturn();

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
                    var xDoc = XDocument.Parse(qe.Run(
                        "ACCOUNT_SUMMARY",
                        $"CUSREF = \"{dataSource.CustomerReference}\"",
                        ""));

                    var c = xDoc.Descendants("record").FirstOrDefault();

                    if (c != null)
                    {
                        lock (locker)
                        {
                            result.ColumnHeadings.AddRange(c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value.Split(delimiter).ToList());

                            var monthsDocumentDates = c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value.Split(delimiter).ToList();

                            var actualMonthsYearsDocDate = new List<string>();

                            for (var i = 0; i < monthsDocumentDates.Count; i += 2)
                            {
                                if (i + 1 < monthsDocumentDates.Count)
                                    actualMonthsYearsDocDate.Add($"{monthsDocumentDates[i]} {monthsDocumentDates[i + 1]}");
                                else
                                    actualMonthsYearsDocDate.Add(monthsDocumentDates[i]);
                            }

                            result.MonthsDocumentDate = actualMonthsYearsDocDate;

                            result.MonthsDocDateSelectionCriteria.AddRange(c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value.Split(delimiter).ToList());

                            result.MonthsDocDateSelectionCriteria[result.MonthsDocDateSelectionCriteria.Count - 1] = "OnAccount";

                            result.AmountBaseDocDate.AddRange(c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value.Split(delimiter).ToList().Select(p =>
                                Convert.ToDecimal(p)).ToList());

                            result.AmountCurrDocDate.AddRange(c.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).ToList().Select(p =>
                               Convert.ToDecimal(p)).ToList());

                            var monthsDueDates = c.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value.Split(delimiter).ToList();

                            var actualMonthsYearsDueDate = new List<string>();

                            for (var i = 0; i < monthsDueDates.Count; i += 2)
                            {
                                if (i + 1 < monthsDueDates.Count)
                                    actualMonthsYearsDueDate.Add($"{monthsDueDates[i]} {monthsDueDates[i + 1]}");
                                else
                                    actualMonthsYearsDueDate.Add(monthsDueDates[i]);
                            }

                            result.MonthsDueDate.AddRange(actualMonthsYearsDueDate);

                            result.MonthsDueDateSelectionCriteria.AddRange(c.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value.Split(delimiter).ToList());

                            result.MonthsDueDateSelectionCriteria[result.MonthsDueDateSelectionCriteria.Count - 1] = "OnAccount";

                            result.AmountBaseDueDate.AddRange(c.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value.Split(delimiter).ToList().Select(p =>
                               Convert.ToDecimal(p)).ToList());

                            result.AmountCurrDueDate.AddRange(c.XPathSelectElement("descendant::fd[@id='C9']").Attribute("value").Value.Split(delimiter, StringSplitOptions.RemoveEmptyEntries).ToList().Select(p =>
                               Convert.ToDecimal(p)).ToList());

                            result.TotalBaseDoc += Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C10']").Attribute("value").Value);

                            result.TotalCurrDoc += Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value);

                            result.TotalBaseDue += Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C12']").Attribute("value").Value);

                            result.TotalCurrDue += Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C13']").Attribute("value").Value);

                        }
                    }
                }
            });

            return result;
        }
    }
}
