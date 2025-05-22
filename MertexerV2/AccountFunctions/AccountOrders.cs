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
        public static OutstandingOrdersReturn OutstandingOrders(string tradeDataSourceJson, PassOutstandingOrdersSearch objSearch)
        {
            var result = new OutstandingOrdersReturn();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.CheckOrders)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
                {
                    var uvFileName = "LIST_ORDER_LINES";

                    var cusrefType = "";

                    var sampleOrderInclusion = "";

                    var dateInclusion = "";

                    var selectionCriteria = "";

                    var ordering = "";

                    var searchTermInclusion = "";

                    if (!String.IsNullOrEmpty(objSearch.searchTerm))
                    {
                        searchTermInclusion = $" AND ((ORDNO = '{objSearch.searchTerm.ToUpperInvariant()}') OR (WI.CUSORDNO = '[{objSearch.searchTerm.ToUpperInvariant()}]') OR (WI.STDPROD = '[{objSearch.searchTerm.ToUpperInvariant()}]') OR (SS.CUS.ORD.REF.LWR = '[{objSearch.searchTerm.ToLowerInvariant()}]')) ";
                    }

                    switch (objSearch.orderStatus)
                    {
                        case "outstanding":
                            cusrefType = "CUSREF";
                            ordering = " BY ORDNO ";
                            break;
                        case "backOrders":
                            cusrefType = "CUSREF";
                            selectionCriteria = " AND BO.IND = 'Y' AND RESERVE.FLAG = '' AND MBATCH.NO1 = '' AND WI.HOLDORDER # 'Y' ";
                            ordering = " BY DATEONFILE BY ORDNO ";
                            break;
                        case "reserveOrders":
                            cusrefType = "CUSREF";
                            selectionCriteria = " AND BO.IND = 'Y' AND RESERVE.FLAG # '' ";
                            ordering = " BY DATEONFILE BY ORDNO BY WI.RESERVED.UNTIL BY ORDNO ";
                            break;
                        case "despatchedOrders":
                            cusrefType = "CUSREF.RECDESP";
                            selectionCriteria = "  ";
                            ordering = " BY WI.DESP.STATUS.DATE BY ORDNO ";
                            break;
                        case "ordersInProgress":
                            cusrefType = "CUSREF";
                            selectionCriteria = " AND STATUS = '3' AND WI.HOLDORDER # 'Y' ";
                            ordering = " BY DATEONFILE BY ORDNO ";
                            break;
                        case "held":
                            cusrefType = "CUSREF";
                            selectionCriteria = " AND WI.HOLDORDER = 'Y' ";
                            ordering = " BY ORDNO ";
                            break;
                        default:
                            break;
                    }

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

                    dateInclusion = $" AND DATEONFILE >= '{fromDt:dd/MM/yyyy}' AND DATEONFILE <= '{toDt:dd/MM/yyyy}' ";

                    switch (objSearch.sampleOrders)
                    {
                        case true:
                            sampleOrderInclusion = " AND SAMP.PROD = 'Y' ";
                            break;
                        case false:
                            sampleOrderInclusion = " AND SAMP.PROD # 'Y' ";
                            break;
                        default:
                            break;
                    }

                    var uvSelectStatement = $"{cusrefType} = '{dataSource.CustomerReference}' {sampleOrderInclusion} {selectionCriteria} {searchTermInclusion} {dateInclusion} {ordering}";

                    var queryResult = qe.Run(uvFileName,
                        uvSelectStatement,
                        //"ORDNO LINE DATEONFILE QTYORD.ORD WI.STDPROD TEXT.DESC CUS.ORD.REF WI.STATUS WI.PARCEL.TRACK.URL WT.BASEPROD.STD WI.DESP.STATUS.DATE WI.RESERVED.UNTIL WI.RESERVED MBATCH.NO.WI",
                        "ORDNO LINE DATEONFILE QTYORD.ORD WI.STDPROD TEXT.DESC CUS.ORD.REF WI.STATUS WI.PARCEL.LINK WT.BASEPROD.STD WI.DESP.STATUS.DATE WI.RESERVED.UNTIL WI.RESERVED MBATCH.NO.WI CUSORDNO",
                        "",
                        "XML",
                        null,
                        false,
                        120);

                    //queryResult = queryResult.Replace("&", " and ");//Regex.Replace(queryResult, "&*", "");

                    //queryResult = queryResult.Replace(@"#38;", "");

                    //queryResult = queryResult.Replace(@"prefix_txt", "p");

                    var xDoc = XDocument.Parse(queryResult);

                    var orderCount = xDoc.Descendants("record").Count();

                    var orders = new List<OutstandingOrderResult>();

                    xDoc.Descendants("record").ToList().ForEach(c =>
                    {
                        try
                        {
                            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-GB");

                            DateTime holdingTime = DateTime.Now;

                            DateTime? orderDate = null;

                            if (!String.IsNullOrEmpty(c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value))
                            {
                                orderDate = DateTime.ParseExact(c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value, "dd/MM/yy", ci.DateTimeFormat);
                            }

                            DateTime? despatchDueDate = null;
                            string despatchDueDateMessage = null;

                            if (!String.IsNullOrEmpty(c.XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value))
                            {
                                if (DateTime.TryParseExact(c.XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value, "dd/MM/yy", ci.DateTimeFormat, System.Globalization.DateTimeStyles.None, out holdingTime))
                                {
                                    despatchDueDate = holdingTime;
                                }
                                else
                                {
                                    despatchDueDateMessage = c.XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value;
                                }
                            }
                            else
                            {
                                despatchDueDateMessage = c.XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value;
                            }

                            DateTime? reservedUntil = null;
                            string reservedUntilMessage = null;

                            if (!String.IsNullOrEmpty(c.XPathSelectElement("descendant::fd[@id='C12']").Attribute("value").Value))
                            {
                                if (DateTime.TryParseExact(c.XPathSelectElement("descendant::fd[@id='C12']").Attribute("value").Value, "dd/MM/yy", ci.DateTimeFormat, System.Globalization.DateTimeStyles.None, out holdingTime))
                                {
                                    reservedUntil = holdingTime;
                                }
                                else
                                {
                                    reservedUntilMessage = c.XPathSelectElement("descendant::fd[@id='C12']").Attribute("value").Value;
                                }
                            }
                            else
                            {
                                reservedUntilMessage = c.XPathSelectElement("descendant::fd[@id='C12']").Attribute("value").Value;
                            }

                            orders.Add(new OutstandingOrderResult(dataSource.UniVerseAccount)
                            {
                                OrderNumber = Convert.ToInt32(c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value),
                                LineNumber = Convert.ToInt32(c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value),
                                OrderDate = orderDate,
                                OrderQuantity = Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value),
                                Product = c.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value,
                                Description = c.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value,
                                CustomerOrderReference = c.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value,
                                Status = c.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value,
                                //Tracking = c.XPathSelectElement("descendant::fd[@id='C9']").Attribute("value").Value.Replace("*", "-"),
                                Tracking = c.XPathSelectElement("descendant::fd[@id='C9']").Attribute("value").Value,
                                BaseProduct = c.XPathSelectElement("descendant::fd[@id='C10']").Attribute("value").Value,
                                DespatchStatusDate = despatchDueDate,
                                DespatchStatusDateMessage = despatchDueDateMessage,
                                ReservedUntil = reservedUntil,
                                ReservedUntilMessage = reservedUntilMessage,
                                ReserveAction = c.XPathSelectElement("descendant::fd[@id='C13']").Attribute("value").Value,
                                Batch = c.XPathSelectElement("descendant::fd[@id='C14']").Attribute("value").Value,
                                CustomerOrderNumber = c.XPathSelectElement("descendant::fd[@id='C15']").Attribute("value").Value
                            });
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e);
                            System.Diagnostics.Debug.WriteLine(c);
                        }
                    });

                    lock (locker)
                    {
                        result.Results.AddRange(orders);
                    }
                }
            });

            return result;
        }

        public static OutstandingOrdersReturn ReserveOrders(string tradeDataSourceJson, PassOutstandingOrdersSearch objSearch)
        {
            var result = new OutstandingOrdersReturn();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.CheckOrders)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
                {
                    var uvFileName = "RESERVE_ORDERS";

                    var cusrefType = "CUSREF.RESV";

                    var dateInclusion = "";

                    var selectionCriteria = "AND BO.IND = 'Y' AND RESERVE.FLAG # '' AND WI.OFLN.RESV.REL.INPROG # 'Y'";

                    var ordering = "BY SORDNO";

                    var searchTermInclusion = "";

                    if (!String.IsNullOrEmpty(objSearch.searchTerm))
                    {
                        searchTermInclusion = $"((SORDNO = '{objSearch.searchTerm}') OR (WI.STDPROD = '[{objSearch.searchTerm}]') OR (SS.CUSORDNO.MCL = '[{objSearch.searchTerm.ToLowerInvariant()}]')) ";
                    }

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

                    dateInclusion = $" AND DATEONFILE >= '{fromDt:dd/MM/yyyy}' AND DATEONFILE <= '{toDt:dd/MM/yyyy}' ";


                    var uvSelectStatement = $"{cusrefType} = '{dataSource.CustomerReference}' {selectionCriteria} {dateInclusion} {ordering}";

                    var queryResult = qe.Run(uvFileName,
                        uvSelectStatement,
                        "SORDNO DATEONFILE DELREF CUSORDNO SS.CUSORDNO.MCL",
                        searchTermInclusion,
                        "XML",
                        null,
                        false,
                        60);

                    //queryResult = queryResult.Replace("&", " and ");//Regex.Replace(queryResult, "&*", "");

                    //queryResult = queryResult.Replace(@"#38;", "");

                    //queryResult = queryResult.Replace(@"prefix_txt", "p");

                    var xDoc = XDocument.Parse(queryResult);

                    var orderCount = xDoc.Descendants("record").Count();

                    var orders = new List<OutstandingOrderResult>();

                    xDoc.Descendants("record").ToList().ForEach(c =>
                    {
                        try
                        {
                            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-GB");

                            DateTime holdingTime = DateTime.Now;

                            DateTime? orderDate = null;

                            if (!String.IsNullOrEmpty(c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value))
                            {
                                orderDate = DateTime.ParseExact(c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value, "dd/MM/yy", ci.DateTimeFormat);
                            }

                            orders.Add(new OutstandingOrderResult(dataSource.UniVerseAccount)
                            {
                                OrderNumber = Convert.ToInt32(c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value),
                                OrderDate = orderDate,
                                DeliveryReference = c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value,
                                CustomerOrderReference = c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value,
                                ReservedUntil = null, // Reserved Until does not make sense at the Order level.
                                ReservedUntilMessage = null // Nor is it actually a column at this level on the old b2b. Whoopsie!
                            });
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e);
                            System.Diagnostics.Debug.WriteLine(c);
                        }
                    });

                    lock (locker)
                    {
                        result.Results.AddRange(orders);
                    }
                }
            });

            return result;
        }

        public static OutstandingOrdersReturn ReserveOrderLines(string tradeDataSourceJson, PassOutstandingOrdersSearch objSearch)
        {
            var result = new OutstandingOrdersReturn();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.CheckOrders)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
                {
                    var uvFileName = "RESERVE_ORDERS";

                    var cusrefType = "CUSREF.RESV";

                    var dateInclusion = "";

                    var selectionCriteria = "AND BO.IND = 'Y' AND RESERVE.FLAG # '' AND WI.OFLN.RESV.REL.INPROG # 'Y'";

                    var ordering = "BY SORDNO";

                    var searchTermInclusion = "";

                    if (!String.IsNullOrEmpty(objSearch.searchTerm))
                    {
                        searchTermInclusion = $" AND ((ORDNO = '{objSearch.searchTerm}') OR (WI.STDPROD = '[{objSearch.searchTerm.ToUpperInvariant()}]') OR (SS.CUS.ORD.REF.LWR = '[{objSearch.searchTerm.ToLowerInvariant()}]')) ";
                    }

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

                    dateInclusion = $" AND DATEONFILE >= '{fromDt:dd/MM/yyyy}' AND DATEONFILE <= '{toDt:dd/MM/yyyy}' ";


                    var uvSelectStatement = $"{cusrefType} = '{dataSource.CustomerReference}' {selectionCriteria} {searchTermInclusion} {dateInclusion} {ordering}";

                    var queryResult = qe.Run(uvFileName,
                        uvSelectStatement,
                        "SORDNO DATEONFILE DELREF CUSORDNO CUSREF CUSNAME",
                        "",
                        "XML",
                        null,
                        false,
                        60);

                    //queryResult = queryResult.Replace("&", " and ");//Regex.Replace(queryResult, "&*", "");

                    //queryResult = queryResult.Replace(@"#38;", "");

                    //queryResult = queryResult.Replace(@"prefix_txt", "p");

                    var xDoc = XDocument.Parse(queryResult);

                    var orderCount = xDoc.Descendants("record").Count();

                    var orders = new List<OutstandingOrderResult>();

                    xDoc.Descendants("record").ToList().ForEach(c =>
                    {
                        try
                        {
                            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-GB");

                            DateTime holdingTime = DateTime.Now;

                            DateTime? orderDate = null;

                            if (!String.IsNullOrEmpty(c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value))
                            {
                                orderDate = DateTime.ParseExact(c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value, "dd/MM/yy", ci.DateTimeFormat);
                            }

                            DateTime? despatchDueDate = null;
                            string despatchDueDateMessage = null;

                            if (!String.IsNullOrEmpty(c.XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value))
                            {
                                if (DateTime.TryParseExact(c.XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value, "dd/MM/yy", ci.DateTimeFormat, System.Globalization.DateTimeStyles.None, out holdingTime))
                                {
                                    despatchDueDate = holdingTime;
                                }
                                else
                                {
                                    despatchDueDateMessage = c.XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value;
                                }
                            }
                            else
                            {
                                despatchDueDateMessage = c.XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value;
                            }

                            DateTime? reservedUntil = null;
                            string reservedUntilMessage = null;

                            if (!String.IsNullOrEmpty(c.XPathSelectElement("descendant::fd[@id='C12']").Attribute("value").Value))
                            {
                                if (DateTime.TryParseExact(c.XPathSelectElement("descendant::fd[@id='C12']").Attribute("value").Value, "dd/MM/yy", ci.DateTimeFormat, System.Globalization.DateTimeStyles.None, out holdingTime))
                                {
                                    reservedUntil = holdingTime;
                                }
                                else
                                {
                                    reservedUntilMessage = c.XPathSelectElement("descendant::fd[@id='C12']").Attribute("value").Value;
                                }
                            }
                            else
                            {
                                reservedUntilMessage = c.XPathSelectElement("descendant::fd[@id='C12']").Attribute("value").Value;
                            }

                            orders.Add(new OutstandingOrderResult(dataSource.UniVerseAccount)
                            {
                                OrderNumber = Convert.ToInt32(c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value),
                                LineNumber = Convert.ToInt32(c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value),
                                OrderDate = orderDate,
                                OrderQuantity = Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value),
                                Product = c.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value,
                                Description = c.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value,
                                CustomerOrderReference = c.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value,
                                Status = c.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value,
                                Tracking = c.XPathSelectElement("descendant::fd[@id='C9']").Attribute("value").Value,
                                BaseProduct = c.XPathSelectElement("descendant::fd[@id='C10']").Attribute("value").Value,
                                DespatchStatusDate = despatchDueDate,
                                DespatchStatusDateMessage = despatchDueDateMessage,
                                ReservedUntil = reservedUntil,
                                ReservedUntilMessage = reservedUntilMessage,
                                ReserveAction = c.XPathSelectElement("descendant::fd[@id='C13']").Attribute("value").Value,
                            });
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e);
                            System.Diagnostics.Debug.WriteLine(c);
                        }
                    });

                    lock (locker)
                    {
                        result.Results.AddRange(orders);
                    }
                }
            });

            return result;
        }

        public static ReserveModifyResult FetchReserveDetail(string tradeDataSourceJson, PassReserveFetch objReserve)
        {
            var result = new ReserveModifyResult();

            if (objReserve.deliveryReference == "C")
            {
                result.invoiceAddress = FetchCustomerInvoiceAddress(tradeDataSourceJson);
            }
            else
            {
                result.deliveryAddress = FetchDeliveryAddress(tradeDataSourceJson, objReserve.orderNumber);
            }

            return result;
        }

        public static OrderDeliveryInfoReturn FetchReserveLines(string tradeDataSourceJson, PassReserveFetch objReserve)
        {
            var result = new OrderDeliveryInfoReturn();

            var locker = new object();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var dataSource = dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.CheckOrders)
                                        .Single()
                                        .DataSources
                                        .Where(p => p.UniVerseAccount == objReserve.universeAccount)
                                        .Single();

            using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
            {
                var xDoc = XDocument.Parse(qe.Run("LIST_ORDER_LINES",
                    $"CUSREF.RESV = '{dataSource.CustomerReference}' AND ORDNO = '{objReserve.orderNumber}' AND BO.IND = 'Y' AND RESERVE.FLAG # '' AND WI.OFLN.RESV.REL.INPROG # 'Y'",
                    "LINE ORDERNO WI.STDPROD WI.RESERVED LINEMEMO DESCRIPTION QTYORD.ORD MBATCH.NO.WI CUS.ORD.REF WI.STATUS WI.DESP.STATUS.DATE WI.DELREF.LINK RESERVE.FLAG"));

                var lines = new List<OrderLineDetail>();

                xDoc.Descendants("record").ToList().ForEach(c =>
                {
                    System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-GB");

                    lines.Add(new OrderLineDetail(dataSource.UniVerseAccount)
                    {
                        LineNumber = c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value,
                        OrderNumber = c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value,
                        Product = c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value,
                        Reserved = c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value,
                        Memo = c.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value,
                        Description = c.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value,
                        Quantity = Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value),
                        BatchNumber = c.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value,
                        CustomerOrderReference = c.XPathSelectElement("descendant::fd[@id='C9']").Attribute("value").Value,
                        OrderStatus = c.XPathSelectElement("descendant::fd[@id='C10']").Attribute("value").Value,
                        DespatchStatus = c.XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value,
                        DeliveryReference = c.XPathSelectElement("descendant::fd[@id='C12']").Attribute("value").Value,
                        ReserveFlag = c.XPathSelectElement("descendant::fd[@id='C13']").Attribute("value").Value
                    });
                });

                lock (locker)
                {
                    result.OrderLines.AddRange(lines);
                }
            }

            if (result.OrderLines.Where(p => !String.IsNullOrEmpty(p.DeliveryReference)).Any())
            {
                var delRef = result.OrderLines.Where(p => !String.IsNullOrEmpty(p.DeliveryReference)).Select(p => p.DeliveryReference).First();

                var split = delRef.Split(new string[] { "," }, StringSplitOptions.None);

                using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
                {
                    var xDoc = XDocument.Parse(qe.Run(split[1],
                        split[2],
                        ""));

                    var rec = xDoc.Descendants("record").FirstOrDefault();

                    if (rec != null)
                    {
                        lock (locker)
                        {
                            result.DeliveryInfo = new OrderDeliveryInfo
                            {
                                DeliveryName = rec.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value,
                                AddressLine1 = rec.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value,
                                AddressLine2 = rec.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value,
                                AddressLine3 = rec.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value,
                                AddressLine4 = rec.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value,
                                PostalCode = rec.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value,
                                PhoneNumber = rec.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value,
                                HaulierDescription = rec.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value
                            };
                        }
                    }
                }
            }

            return result;
        }

        public static OrderDeliveryInfoReturn OrderDeliveryInfo(string tradeDataSourceJson, PassOrderNumber objOrderNumber)
        {
            var result = new OrderDeliveryInfoReturn();

            var locker = new object();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.CheckOrders)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
                {
                    var xDoc = XDocument.Parse(qe.Run("ORDER_LINE_ENQ",
                        $"ORDERNO = '{objOrderNumber.orderNumber}'",
                        "LINE ORDERNO WI.STDPROD WI.RESERVED LINEMEMO"));

                    var lines = new List<OrderLineDetail>();

                    xDoc.Descendants("record").ToList().ForEach(c =>
                    {
                        System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-GB");

                        lines.Add(new OrderLineDetail(dataSource.UniVerseAccount)
                        {
                            LineNumber = c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value,
                            OrderNumber = c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value,
                            Product = c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value,
                            Reserved = c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value,
                            Memo = c.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value,
                            Description = c.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value,
                            Quantity = Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value),
                            BatchNumber = c.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value,
                            CustomerOrderReference = c.XPathSelectElement("descendant::fd[@id='C9']").Attribute("value").Value,
                            OrderStatus = c.XPathSelectElement("descendant::fd[@id='C10']").Attribute("value").Value,
                            DespatchStatus = c.XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value,
                            DeliveryReference = c.XPathSelectElement("descendant::fd[@id='C12']").Attribute("value").Value,
                            Tracking = c.XPathSelectElement("descendant::fd[@id='C13']").Attribute("value").Value,
                            WebProduct = c.XPathSelectElement("descendant::fd[@id='C14']").Attribute("value").Value
                        });
                    });

                    lock (locker)
                    {
                        result.OrderLines.AddRange(lines);
                    }
                }

                if (result.OrderLines.Where(p => !String.IsNullOrEmpty(p.DeliveryReference)).Any())
                {
                    var delRef = result.OrderLines.Where(p => !String.IsNullOrEmpty(p.DeliveryReference)).Select(p => p.DeliveryReference).First();

                    var split = delRef.Split(new string[] { "," }, StringSplitOptions.None);

                    using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
                    {
                        var xDoc = XDocument.Parse(qe.Run(split[1],
                            split[2],
                            ""));

                        var rec = xDoc.Descendants("record").FirstOrDefault();

                        if (rec != null)
                        {
                            lock (locker)
                            {
                                result.DeliveryInfo = new OrderDeliveryInfo
                                {
                                    DeliveryName = rec.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value,
                                    AddressLine1 = rec.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value,
                                    AddressLine2 = rec.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value,
                                    AddressLine3 = rec.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value,
                                    AddressLine4 = rec.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value,
                                    PostalCode = rec.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value,
                                    PhoneNumber = rec.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value,
                                    HaulierDescription = rec.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value
                                };
                            }
                        }
                    }
                }
            });

            return result;
        }
    
        public static ParcelCountReturn CheckParcelCount(string tradeDataSourceJson, PassParcelCount objParcel)
        {
            var result = new ParcelCountReturn();

            var locker = new object();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.CheckOrders)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
                {
                    var split = objParcel.documentNumber.Split(new string[] { "," }, StringSplitOptions.None);

                    var xDoc = XDocument.Parse(qe.Run(split[0],
                        $"DOC.NO = '{split[1]}'",
                        ""));

                    var trackingDetails = new List<ParcelTrackingDetail>();

                    xDoc.Descendants("record").ToList().ForEach(c =>
                    {
                        var parcels = c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value
                            .Split(Framework.ValueMark)
                            .ToList();

                        var products = c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value
                            .Split(Framework.ValueMark)
                            .ToList();

                        var descriptions = c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value
                            .Split(Framework.ValueMark)
                            .ToList();

                        var trackingUrls = c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value
                            .Split(Framework.ValueMark)
                            .ToList();

                        for (var i = 0; i < parcels.Count; i++)
                        {
                            var viable = true;

                            if (!(parcels.Count > i))
                            {
                                viable = false;
                            }

                            if (!(products.Count() > i))
                            {
                                viable = false;
                            }

                            if (!(descriptions.Count() > i))
                            {
                                viable = false;
                            }

                            if (!(trackingUrls.Count() > i))
                            {
                                viable = false;
                            }

                            if (viable)
                            {
                                trackingDetails.Add(new ParcelTrackingDetail()
                                {
                                    Parcel = parcels[i],
                                    ProductCodes = products[i],
                                    Descriptions = descriptions[i],
                                    TrackingURL = trackingUrls[i]
                                });
                            }
                        }
                    });

                    lock(locker)
                    {
                        result.Results.AddRange(trackingDetails);
                    }
                }
            });

            return result;
        }

        public static ReserveModifyResult ModifyReserveDeliveryReference(string tradeDataSourceJson, PassReserveModify objReserveModify)
        {
            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            // Now... this... might not be the best way about it.
            // Why? Because... as I have just accidentally proven, it's possible (though it should never happen) that order numbers conflict
            // across UniVerse accounts. And so if you have multiple data sources for which you are executing this method, you're going to end
            // up accidentally updating data you did not intend to.

            // So: when querying for the data in the first place (building lists of OutstandingOrderRecords), record against each row the origin
            // dataSource. Pass that in when modifying anything to do with the line / order in question.

            var dataSource = dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.CheckOrders)
                                        .Single()
                                        .DataSources
                                        .Where(p => p.UniVerseAccount == objReserveModify.universeAccount)
                                        .Single();

            using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
            {
                var uvFileName = "MODIFY_RESERVE_ORDER";

                var queryResult = qe.Run(uvFileName,
                    "",
                    "",
                    $@"{objReserveModify.orderNumber}
N
{dataSource.CustomerReference}
{objReserveModify.deliveryReference}",
                    "XML",
                    null,
                    false,
                    60);

                if (queryResult != "")
                {
                    throw new Exception("Failed modifying reserve order");
                }
            }

            return FetchReserveDetail(tradeDataSourceJson, objReserveModify);
        }

        public static ReserveModifyResult ModifyReserveReleaseOrder(string tradeDataSourceJson, PassReserveModify objReserveModify)
        {
            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var dataSource = dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.CheckOrders)
                                        .Single()
                                        .DataSources
                                        .Where(p => p.UniVerseAccount == objReserveModify.universeAccount)
                                        .Single();

            using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
            {
                var uvFileName = "MODIFY_RESERVE_ORDER";

                var queryResult = qe.Run(uvFileName,
                    "",
                    "",
                    $@"{objReserveModify.orderNumber}
R
{dataSource.CustomerReference}
{objReserveModify.deliveryReference}",
                    "XML",
                    null,
                    false,
                    60);

                if (queryResult != "")
                {
                    throw new Exception("Failed modifying reserve order");
                }
            }

            return FetchReserveDetail(tradeDataSourceJson, objReserveModify);
        }

        public static ModifyReserveLineResult ModifyReserveLine(string tradeDataSourceJson, PassReserveLine objLine)
        {
            var result = new ModifyReserveLineResult();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var dataSource = dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.CheckOrders)
                                        .Single()
                                        .DataSources
                                        .Where(p => p.UniVerseAccount == objLine.universeAccount)
                                        .Single();

            using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
            {
                var uvFileName = "RESERVE_ORDER_UPDATE";

                //                var queryResult = qe.Run(uvFileName,
                //                    "",
                //                    "",
                //                    objLine.action == "D" ? $@"{objLine.lineKey}
                //{objLine.action}" : $@"{objLine.lineKey}
                //{objLine.quantity}
                //{objLine.lineReference}
                //{objLine.lineMemo?.Replace("\n", "|") ?? " "}",
                //                    "XML",
                //                    null,
                //                    false,
                //                    60);

                var webData = $"";

                switch (objLine.action)
                {
                    case "D":
                        webData = $@"{objLine.lineKey}
{objLine.action}";
                        break;
                    case "R":
                        webData = $@"{objLine.lineKey}
{objLine.action}
{objLine.quantity}
{objLine.lineReference}
{objLine.lineMemo?.Replace("\n", "|") ?? " "}";
                        break;
                    case "Q":
                        webData = $@"{objLine.lineKey}
{objLine.action}
{objLine.quantity}
{objLine.lineReference}";
                        break;
                    case "L":
                        webData = $@"{objLine.lineKey}
{objLine.action}


{objLine.lineMemo?.Replace("\n", "|") ?? " "}";
                        break;
                }

                var queryResult = qe.Run(uvFileName,
                    "",
                    "",
                    webData,
                    "XML",
                    null,
                    false,
                    60);

                if (queryResult != "")
                {
                    throw new Exception("Failed modifying reserve order");
                }
            }

            return result;
        }
    }
}
