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
        public static AccountSummaryRecentPaymentsReturn AccountSummaryRecentPayments(string tradeDataSourceJson, bool forOutstandingInvoices = false)
        {
            var result = new AccountSummaryRecentPaymentsReturn();

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
                    var outstandingInvoicesInclusion = "";

                    if (forOutstandingInvoices)
                        outstandingInvoicesInclusion = " AND TYPE # 'CRN' ";

                    var xDoc = XDocument.Parse(qe.Run("RECENT_PAYMENTS",
                        $"CUSTREF = \"{dataSource.CustomerReference}\" AND UNALLOCATED # '' AND UNALLOCATED # '0' {outstandingInvoicesInclusion} BY DOC.REF.WI",
                        "DOC.REF.WI DOCDATE ORIGVAL OSAMOUNT"));

                    var payments = new List<OnAccountResult>();

                    xDoc.Descendants("record").ToList().ForEach(c =>
                    {
                        System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-GB");

                        DateTime? paymentDate = null;

                        if (!String.IsNullOrEmpty(c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value))
                        {
                            paymentDate = DateTime.ParseExact(c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value, "dd/MM/yy", ci.DateTimeFormat);
                        }

                        payments.Add(new OnAccountResult()
                        {
                            DocumentReference = c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value,
                            PaymentDate = paymentDate,
                            OriginalValue = Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value),
                            OutstandingAmount = Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value)
                        });
                    });

                    lock (locker)
                    {
                        result.PaymentsOnAccount.AddRange(payments);
                    }
                }
            });

            return result;
        }

        public static AccountSummaryRecentPaymentsReturn AccountSummaryRecentPayments(string dataSource, string customerReference, bool forOutstandingInvoices = false)
        {
            var result = new AccountSummaryRecentPaymentsReturn();

            using (var qe = new QueryExecutor(dataSource))
            {
                var outstandingInvoicesInclusion = "";

                if (forOutstandingInvoices)
                    outstandingInvoicesInclusion = " AND TYPE # 'CRN' ";

                var xDoc = XDocument.Parse(qe.Run("RECENT_PAYMENTS",
                    $"CUSTREF = \"{customerReference}\" AND UNALLOCATED # '' AND UNALLOCATED # '0' {outstandingInvoicesInclusion} BY DOC.REF.WI",
                    "DOC.REF.WI DOCDATE ORIGVAL OSAMOUNT"));

                xDoc.Descendants("record").ToList().ForEach(c =>
                {
                    System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-GB");

                    DateTime? paymentDate = null;

                    if (!String.IsNullOrEmpty(c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value))
                    {
                        paymentDate = DateTime.ParseExact(c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value, "dd/MM/yy", ci.DateTimeFormat);
                    }

                    result.PaymentsOnAccount.Add(new OnAccountResult()
                    {
                        DocumentReference = c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value,
                        PaymentDate = paymentDate,
                        OriginalValue = Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value),
                        OutstandingAmount = Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value)
                    });
                });
            }

            return result;
        }

        public static AccountSummaryRecentPaymentsReturn RecentPayments(string tradeDataSourceJson)
        {
            var result = new AccountSummaryRecentPaymentsReturn();

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
                    var xDoc = XDocument.Parse(qe.Run("RECENT_PAYMENTS",
                        $"CUSTREF = \"{dataSource.CustomerReference}\" AND PAYMENT.TYPE = 'Y' BY DOC.REF.WI",
                        "DOC.REF.WI DOCDATE ORIGVAL"));

                    var payments = new List<OnAccountResult>();

                    xDoc.Descendants("record").ToList().ForEach(c =>
                    {
                        System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-GB");

                        DateTime? paymentDate = null;

                        if (!String.IsNullOrEmpty(c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value))
                        {
                            paymentDate = DateTime.ParseExact(c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value, "dd/MM/yy", ci.DateTimeFormat);
                        }

                        payments.Add(new OnAccountResult()
                        {
                            DocumentReference = c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value,
                            PaymentDate = paymentDate,
                            OriginalValue = Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value),
                            OutstandingAmount = 0
                        });
                    });

                    lock (locker)
                    {
                        result.PaymentsOnAccount.AddRange(payments);
                    }
                }
            });

            return result;
        }
    }
}
