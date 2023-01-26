using MertexerV2.Models.DataSource;
using MertexerV2SQL.Models.Stock;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using U2.Data.Client;
using U2.Data.Client.UO;

namespace MertexerV2SQL.Stock
{
    public static class StockFunctions
    {
        /// <summary>
        /// This method will first call PIECE_STOCK_ENQUIRY
        /// For Fabric & Wallpaper part numbers we should always get some data back, this will be in a <record></record> element.
        /// If there is no <record></record> element, we assume that the part number is probably a finished product part number which
        /// does not report stock from PIECE_STOCK_ENQUIRY. 
        /// We then call B2C_STOCK_ENQUIRY.
        /// If that also returns no stock data, we then check STPRODMASTER to see if this is actually a valid part number. If it is,
        /// we'll return XML reflecting zero stock and no purchase orders for this part number. If it isn't, we'll return an error message.
        /// </summary>
        /// <param name="partNumber">The incoming part number queried for. We must sanitise this in some way.</param>
        /// <returns></returns>
        public static ProductStockInfoReturn PerformStockEnquiry(string partNumber)
        {
            U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder();
            conn_str.UserID = MertexConnectionDetails.UserID;
            conn_str.Password = MertexConnectionDetails.Password;
            conn_str.Server = MertexConnectionDetails.Server;
            conn_str.Database = MertexConnectionDetails.Database;
            conn_str.ServerType = MertexConnectionDetails.ServerType;
            conn_str.AccessMode = "Native";
            conn_str.RpcServiceType = "uvcs";

            string resultString = "";
            
            using (U2Connection con = new U2Connection(conn_str.ConnectionString))
            {
                con.Open();

                UniSession us1 = con.UniSession;

                var sub = us1.CreateUniSubroutine("RF.ASPLINK.V3", 15);

                sub.SetArg(0, "b2c.manager|" + Guid.NewGuid());
                sub.SetArg(1, "PIECE_STOCK_ENQUIRY"); // Sub... Subroutine Name.
                sub.SetArg(2, "PRODCODE = \"" + partNumber + "\" AND WT.BASEPROD.DISC = \"Y\""); //Criteria
                                                                                                    //"WI.STATUS.IMAGE PRODCODE PRODDESC WI.SPRICE PRODCODE WT.SAMPLE PSE.PO.DUE.WI PSE.PONO.WI PSE.PO.OSQTY.WI PSE.BATCH.WI PSE.ITEMS.WI PSE.FREELEN.WI WI.GRP.CODE WI.GRP.DESC"
                sub.SetArg(3, "PSE.PO.DUE.WI PSE.PONO.WI PSE.PO.OSQTY.WI PSE.BATCH.WI PSE.ITEMS.WI PSE.FREELEN.WI PRODCODE WI.TOTFREE"); //Fields to return
                sub.SetArg(4, "XML"); //returnType
                sub.SetArg(5, "0"); //pageSize
                sub.SetArg(6, "1"); //pageNumber
                sub.SetArg(7, ""); //pstrWebData
                sub.SetArg(8, ""); //unknown
                sub.SetArg(9, ""); //unknown
                sub.SetArg(10, ""); //unknown
                sub.SetArg(11, ""); //unknown
                sub.SetArg(12, ""); //result
                sub.SetArg(13, ""); //return flag
                sub.SetArg(14, ""); //unknown

                /*
                    * WebLogin            I          0
                    * Filename            I          1
                    * Selection Criteria  I          2
                    * Fields to Display   I          3
                    * Record delimeter    I          4
                    * Page Size           I          5
                    * Page Number         I          6
                    * WebData             I          7
                    * Field Descriptions  O          8
                    * Query Title         O          9
                    * Total Records       O          10
                    * Total Pages         O          11
                    * Result              O          12
                    * Return Flag         O          13
                    * HTML Output         O          14
                    */

                sub.Call();

                resultString = sub.GetArg(12);

                con.Close();
            }

            XDocument xmlResult = XDocument.Parse(resultString);

            var nfi = new System.Globalization.CultureInfo("en-GB").NumberFormat;

            var rec = xmlResult.Descendants("record");

            var fds = xmlResult.Descendants("record").Descendants("fd");

            if (fds.Count() == 0)
            {
                XDocument noFDsDoc =
                    new XDocument(
                        new XElement("product", new XAttribute("code", partNumber),
                            new XElement("stock", new XAttribute("total", ((decimal)0).ToString("N2", nfi))),
                            new XElement("purchaseOrders", new XAttribute("total", ((decimal)0).ToString("N2", nfi)))
                        )
                    );

                ProductStockInfoReturn noFDsReturn = new ProductStockInfoReturn()
                {
                    Code = partNumber,
                    TotalStock = "0",
                    Batches = new List<Batch>(),
                    PurchaseOrders = new List<PurchaseOrder>()
                };

                return noFDsReturn;
            }
            else
            {
                var purchaseOrderDates = fds.Where(p => p.Attribute("id").Value == "C1").Single().Attribute("value").Value;
                var purchaseOrderNumbers = fds.Where(p => p.Attribute("id").Value == "C2").Single().Attribute("value").Value;
                var purchaseOrderLengths = fds.Where(p => p.Attribute("id").Value == "C3").Single().Attribute("value").Value;

                var purchaseOrderDateList = purchaseOrderDates.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var purchaseOrderNumberList = purchaseOrderNumbers.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var purchaseOrderLengthList = purchaseOrderLengths.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var totalPurchaseOrderLength = purchaseOrderLengthList.ConvertAll(c => Convert.ToDecimal(c)).Sum();

                var batches = fds.Where(p => p.Attribute("id").Value == "C4").Single().Attribute("value").Value;
                var batchItems = fds.Where(p => p.Attribute("id").Value == "C5").Single().Attribute("value").Value;
                var itemLengths = fds.Where(p => p.Attribute("id").Value == "C6").Single().Attribute("value").Value;

                var batchList = batches.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var batchItemsList = batchItems.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var itemsLengthList = itemLengths.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var totalLength = itemsLengthList.ConvertAll(c => Convert.ToDecimal(c)).Sum();

                var wiTotalFree = fds.Where(p => p.Attribute("id").Value == "C8").Single().Attribute("value").Value;


                if (Decimal.TryParse(wiTotalFree, out decimal wiTotalFreeDecimal))
                {
                    if (wiTotalFreeDecimal > totalLength)
                    {
                        totalLength = wiTotalFreeDecimal;
                    }
                }

                var resultPurchaseOrders = new List<PurchaseOrder>();
                for (var i = 0; i < purchaseOrderNumberList.Count; i++)
                {
                    resultPurchaseOrders.Add(new PurchaseOrder() { PurchaseOrderNumber = purchaseOrderNumberList[i], DueDate = purchaseOrderDateList[i], Length = purchaseOrderLengthList[i] });
                }

                var resultBatches = new List<Batch>();
                for (var i = 0; i < batchList.Count; i++)
                {
                    resultBatches.Add(new Batch() { BatchNumber = batchList[i], ItemCount = batchItemsList[i], Length = itemsLengthList[i] });
                }

                var groupedBatches = resultBatches.GroupBy(x => x.BatchNumber).Select(p => new Batch
                {
                    BatchNumber = p.Key,
                    ItemCount = p.Select(q => Convert.ToDecimal(q.ItemCount == "" ? "0" : q.ItemCount)).Sum().ToString("N2", nfi),
                    Length = p.Select(q => (Convert.ToDecimal(q.Length == "" ? "0" : q.Length) * Convert.ToDecimal(q.ItemCount == "" ? "0" : q.ItemCount))).Sum().ToString("N2", nfi)
                })
                .ToList();

                var result = new ProductStockInfoReturn()
                {
                    Code = partNumber,
                    TotalStock = totalLength.ToString("N2", nfi),
                    Batches = groupedBatches.Select(x => new Batch()
                    {
                        BatchNumber = x.BatchNumber,
                        ItemCount = x.ItemCount,
                        Length = x.Length,
                        Pieces = resultBatches.Where(p => p.BatchNumber == x.BatchNumber).Select(p => new BatchPiece() { Quantity = p.ItemCount, Length = p.Length }).ToList()
                    }).ToList(),
                    TotalPurchaseOrderLength = totalPurchaseOrderLength.ToString("N2", nfi),
                    PurchaseOrders = resultPurchaseOrders.Select(p => new PurchaseOrder()
                    {
                        PurchaseOrderNumber = p.PurchaseOrderNumber,
                        DueDate = p.DueDate,
                        Length = p.Length
                    }).ToList()
                };

                return result;
            }
        }

        public static ProductStockInfoReturn FinishedProductStockEnquiry(string partNumber)
        {
            U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder();
            conn_str.UserID = MertexConnectionDetails.UserID;
            conn_str.Password = MertexConnectionDetails.Password;
            conn_str.Server = MertexConnectionDetails.Server;
            conn_str.Database = MertexConnectionDetails.Database;
            conn_str.ServerType = MertexConnectionDetails.ServerType;
            conn_str.AccessMode = "Native";
            conn_str.RpcServiceType = "uvcs";

            string resultString = "";
           
            using (U2Connection con = new U2Connection(conn_str.ConnectionString))
            {
                con.Open();

                UniSession us1 = con.UniSession;

                var sub = us1.CreateUniSubroutine("RF.ASPLINK.V3", 15);

                //FDG2685/01 useful for example data

                sub.SetArg(0, "b2c.manager|" + Guid.NewGuid());
                sub.SetArg(1, "B2C_STOCK_ENQUIRY"); // Sub... Subroutine Name. What the hell.
                sub.SetArg(2, partNumber); //Criteria
                sub.SetArg(3, "PRODCODE WI.TOTFREE WI.DUEIN.SEL WI.DUEIN.OSQTY"); //Fields to return
                sub.SetArg(4, "XML"); //returnType
                sub.SetArg(5, "0"); //pageSize
                sub.SetArg(6, "1"); //pageNumber
                sub.SetArg(7, ""); //pstrWebData
                sub.SetArg(8, ""); //unknown
                sub.SetArg(9, ""); //unknown
                sub.SetArg(10, ""); //unknown
                sub.SetArg(11, ""); //unknown
                sub.SetArg(12, ""); //result
                sub.SetArg(13, ""); //return flag
                sub.SetArg(14, ""); //unknown

                sub.Call();

                resultString = sub.GetArg(12);

                con.Close();
            }

            XDocument xmlResult = XDocument.Parse(resultString);

            var nfi = new System.Globalization.CultureInfo("en-GB").NumberFormat;

            var rec = xmlResult.Descendants("record");

            var fds = xmlResult.Descendants("record").Descendants("fd");

            if (fds.Count() == 0)
            {
                XDocument noFDsDoc =
                    new XDocument(
                        new XElement("product", new XAttribute("code", partNumber),
                            new XElement("stock", new XAttribute("total", ((decimal)0).ToString("N2", nfi))),
                            new XElement("purchaseOrders", new XAttribute("total", ((decimal)0).ToString("N2", nfi)))
                        )
                    );

                ProductStockInfoReturn noFDsReturn = new ProductStockInfoReturn()
                {
                    Code = partNumber,
                    TotalStock = "0",
                    Batches = new List<Batch>(),
                    PurchaseOrders = new List<PurchaseOrder>()
                };

                return noFDsReturn;
            }
            else
            {
                var purchaseOrderDates = fds.Where(p => p.Attribute("id").Value == "C1").Single().Attribute("value").Value;
                var purchaseOrderNumbers = fds.Where(p => p.Attribute("id").Value == "C2").Single().Attribute("value").Value;
                var purchaseOrderLengths = fds.Where(p => p.Attribute("id").Value == "C3").Single().Attribute("value").Value;

                var purchaseOrderDateList = purchaseOrderDates.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var purchaseOrderNumberList = purchaseOrderNumbers.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var purchaseOrderLengthList = purchaseOrderLengths.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var totalPurchaseOrderLength = 0M;

                if (purchaseOrderLengthList.Count == 1)
                {
                    if (purchaseOrderLengthList[0] == "TBC" || purchaseOrderLengthList[0] == "None Due")
                    {
                        // noop
                    }
                    else
                    {
                        purchaseOrderLengthList.ConvertAll(c => Convert.ToDecimal(c)).Sum();
                    }
                }
                else
                {
                    purchaseOrderLengthList.ConvertAll(c => Convert.ToDecimal(c)).Sum();
                }

                var batches = fds.Where(p => p.Attribute("id").Value == "C4").Single().Attribute("value").Value;
                var batchItems = fds.Where(p => p.Attribute("id").Value == "C5").SingleOrDefault()?.Attribute("value").Value;
                var itemLengths = fds.Where(p => p.Attribute("id").Value == "C6").SingleOrDefault()?.Attribute("value").Value;

                var batchList = batches.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var batchItemsList = batchItems?.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var itemsLengthList = itemLengths?.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var totalLength = 0M;

                if (itemsLengthList?.Count == 1)
                {
                    if (itemsLengthList[0] == "TBC" || itemsLengthList[0] == "None Due")
                    {
                        // noop
                    }
                    else
                    {
                        itemsLengthList.ConvertAll(c => Convert.ToDecimal(c)).Sum();
                    }
                }
                else
                {
                    itemsLengthList?.ConvertAll(c => Convert.ToDecimal(c)).Sum();
                }

                var wiTotalFree = fds.Where(p => p.Attribute("id").Value == "C8").SingleOrDefault()?.Attribute("value").Value;


                if (Decimal.TryParse(wiTotalFree, out decimal wiTotalFreeDecimal))
                {
                    if (wiTotalFreeDecimal > totalLength)
                    {
                        totalLength = wiTotalFreeDecimal;
                    }
                }

                var resultPurchaseOrders = new List<PurchaseOrder>();
                for (var i = 0; i < purchaseOrderNumberList.Count; i++)
                {
                    resultPurchaseOrders.Add(new PurchaseOrder() { PurchaseOrderNumber = purchaseOrderNumberList[i], DueDate = purchaseOrderDateList[i], Length = purchaseOrderLengthList[i] });
                }

                var resultBatches = new List<Batch>();
                for (var i = 0; i < batchList.Count; i++)
                {
                    if (batchItemsList != null && itemsLengthList != null)
                        resultBatches.Add(new Batch() { BatchNumber = batchList[i], ItemCount = batchItemsList[i], Length = itemsLengthList[i] });
                }

                var groupedBatches = resultBatches.GroupBy(x => x.BatchNumber).Select(p => new Batch
                {
                    BatchNumber = p.Key,
                    ItemCount = p.Select(q => Convert.ToDecimal(q.ItemCount == "" ? "0" : q.ItemCount)).Sum().ToString("N2", nfi),
                    Length = p.Select(q => (Convert.ToDecimal(q.Length == "" ? "0" : q.Length) * Convert.ToDecimal(q.ItemCount == "" ? "0" : q.ItemCount))).Sum().ToString("N2", nfi)
                })
                .ToList();

                var result = new ProductStockInfoReturn()
                {
                    Code = partNumber,
                    TotalStock = totalLength.ToString("N2", nfi),
                    Batches = groupedBatches.Select(x => new Batch()
                    {
                        BatchNumber = x.BatchNumber,
                        ItemCount = x.ItemCount,
                        Length = x.Length,
                        Pieces = resultBatches.Where(p => p.BatchNumber == x.BatchNumber).Select(p => new BatchPiece() { Quantity = p.ItemCount, Length = p.Length }).ToList()
                    }).ToList(),
                    TotalPurchaseOrderLength = totalPurchaseOrderLength.ToString("N2", nfi),
                    PurchaseOrders = resultPurchaseOrders.Select(p => new PurchaseOrder()
                    {
                        PurchaseOrderNumber = p.PurchaseOrderNumber,
                        DueDate = p.DueDate,
                        Length = p.Length
                    }).ToList()
                };

                return result;
            }
        }

        public static Boolean TestValidPartNumber(string partNumber)
        {
            U2ConnectionStringBuilder conn_str = new U2ConnectionStringBuilder();
            conn_str.UserID = MertexConnectionDetails.UserID;
            conn_str.Password = MertexConnectionDetails.Password;
            conn_str.Server = MertexConnectionDetails.Server;
            conn_str.Database = MertexConnectionDetails.Database;
            conn_str.ServerType = MertexConnectionDetails.ServerType;

            var result = false;

            using (U2Connection conn = new U2Connection(conn_str.ConnectionString))
            {
                U2Command comm = conn.CreateCommand();

                comm.CommandText = @"
                    SELECT
                        PRODCODE
                    FROM 
                        STPRODMASTER
                    WHERE
                        PRODCODE = @ProductCode
                    ";
                comm.Parameters.Add("@ProductCode", partNumber);


                conn.Open();

                var reader = comm.ExecuteReader();

                while (reader.Read())
                {
                    result = !reader.IsDBNull(0);
                }

                reader.Close();

                conn.Close();
            }

            return result;
        }
    }
}
