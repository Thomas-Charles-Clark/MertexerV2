using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using U2.Data.Client;
//using U2.Data.Client.UO;
using MertexerV2.Models;
using MertexerV2.DAL;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using MertexerV2.Utilities;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MertexerV2
{
    /////
    /////   22/09/2021 Hasn't this been defunct for a couple of years now?
    /////

    /// <summary>
    /// Utilises the U2.Data.Client and U2.Data.Client.UO objects to place orders on Mertex.
    /// </summary>
    public class OrderController : MertexConnection
    {
        public OrderController() : base()
        {

        }

        /// <summary>
        /// In Debug mode, connects to PLAY. In Release, connects to MERTEX.
        /// </summary>
#if DEBUG
        public OrderController(string database = "PLAY") : base(database)
#else
        public OrderController(string database = "MERTEX") : base(database)
#endif
        {

        }

        ~OrderController()
        {
            base.Dispose(false);
        }

        public string PlaceOrder(Order order)
        {
            CreateBasket(order);

            AddLines(order);

            AddGiftBoxes(order);

            AddHeader(order);

            return ConfirmOrder(order);
        }

        public bool CreateBasket(Order order)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine($"Creating Basket {order.BasketID}");
                var result = ExecuteOrderParagraph("B2C_GET_BASKET", order.BasketID, null, order.BasketID);
                Console.WriteLine(result);
                return true;
            }
            catch (Exception e)
            {
                throw new Exception($"Exception thrown attempting to call B2C_GET_BASKET with BasketID {order.BasketID}. Please see inner exception for details.", e);
            }
        }

        /// <summary>
        /// Call this routine once for each line you want to add to an online basket.
        /// When adding order lines for a B2C customer UVFileName should be set as "B2C_SALES_ORDER_ALLOC", otherwise use "SALES_ORDER_ALLOC".
        /// 
        /// We must have some way of determining whether or not to utilise B2C_.
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool AddLines(Order order)
        {
            foreach (var line in order.OrderLines)
            {
                try
                {
                    Console.WriteLine();
                    Console.WriteLine($"Adding line");
                    Console.WriteLine(line.ToString());

                    var result = ExecuteOrderParagraph("B2C_SALES_ORDER_ALLOC", order.BasketID, line.WebData());

                    if (result.ToLowerInvariant().Contains("stock") || result.ToLowerInvariant().Contains("location"))
                    {
                        line.BackOrderIndicator = "Y";
                        result = ExecuteOrderParagraph("B2C_SALES_ORDER_ALLOC", order.BasketID, line.WebData());
                    }

                    if (result.Trim() != "")
                    {
                        throw new Exception($"Error submitting OrderHeadID {order.OrderNumber}, Line {line.LineNumber}: SALES_ORDER_ALLOC returned {result}");
                    }

                    Console.WriteLine(result);
                }
                catch (Exception e)
                {
                    throw new Exception($"Exception thrown attempting to call B2C_SALES_ORDER_ALLOC for OrderHeadID {order.OrderNumber}, Line {line.LineNumber}. Please see inner exception for details.", e);
                }
            }

            return true;
        }
    
        public bool AddGiftBoxes(Models.Order order)
        {
            foreach (var giftBox in order.GiftBoxes)
            {
                try
                {
                    Console.WriteLine();
                    Console.WriteLine($"Adding gift box");
                    Console.WriteLine(giftBox.ToString());
                    var result = ExecuteOrderParagraph("B2C_SALES_ORDER_ALLOC", order.BasketID, giftBox.WebData());

                    if (result.Trim() != "")
                    {
                        throw new Exception($"Error submitting OrderHeadID {order.OrderNumber}, GiftBox {giftBox.GiftBoxID}: SALES_ORDER_ALLOC returned {result}");
                    }

                    Console.WriteLine(result);
                }
                catch (Exception e)
                {
                    throw new Exception($"Exception thrown attempting to call B2C_SALES_ORDER_ALLOC for OrderHeadID {order.OrderNumber}, GiftBox {giftBox.GiftBoxID}. Please see inner exception for details.", e);
                }
            }

            return true;
        }

        public bool AddHeader(Models.Order order)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine($"Adding header");
                Console.WriteLine(order.HeaderToString());
                var result = ExecuteOrderParagraph("SALES_ORDER_DELDETS", order.BasketID, order.DeliveryDetailsWebData());

                if (result.Trim() != "")
                {
                    throw new Exception($"Exception while calling SALES_ORDER_DELDETS for {order.OrderNumber} - result was {result}");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Exception thrown attempting to call SALES_ORDER_DELDETS for OrderHeadID {order.OrderNumber}. Please see inner exception for details.", e);
            }

            return true;
        }

        public string ConfirmOrder(Models.Order order)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine($"Confirming order");

#if DEBUG
                //var result = "FORCED FAILURE"; 
                var result = ExecuteOrderParagraph("SALES_ORDER_CREATE", order.BasketID, order.CreateOrderWebData());
#else
                var result = ExecuteOrderParagraph("SALES_ORDER_CREATE", order.BasketID, order.CreateOrderWebData());
#endif

                if (result?.Trim().Replace("\"", "") != order.OrderNumber)
                {
                    throw new Exception($"Exception while calling SALES_ORDER_CREATE for {order.OrderNumber} - result was {result}");
                }

                return result;
            }
            catch (Exception e)
            {
                throw new Exception($"Exception thrown attempting to call SALES_ORDER_CREATE for OrderHeadID {order.OrderNumber}. Please see inner exception for details.", e);
            }
        }

        public string ExecuteOrderParagraph(string uvFileName, string basketId, string webData, string uvSelectStatement = null)
        {
            var sub = Session.CreateUniSubroutine("RF.ASPLINK.V3", 15);

            Session.Timeout = 60;

            var fieldsToReturn = "";
            var returnType = "XML";
            var pageSize = 0;
            var pageNumber = 1;

            sub.SetArg(0, basketId);
            sub.SetArg(1, uvFileName);
            sub.SetArg(2, uvSelectStatement?.Replace("'", "\"") ?? "");
            sub.SetArg(3, fieldsToReturn);  // fieldsToReturn ?? ""  
            sub.SetArg(4, returnType);      // returnType ?? "XML" / "TEXT";
            sub.SetArg(5, $"{pageSize}");   // pageSize // 0 == all pages
            sub.SetArg(6, $"{pageNumber}"); // pageNumber // 1 == page one
            sub.SetArg(7, $"{webData}");    // webData. Here goes some supermassive strings separated by \r\n.
            sub.SetArg(8, "");  // undocumented
            sub.SetArg(9, "");  // undocumented
            sub.SetArg(10, ""); // undocumented
            sub.SetArg(11, ""); // undocumented
            sub.SetArg(12, ""); // undocumented
            sub.SetArg(13, ""); // undocumented
            sub.SetArg(14, ""); // undocumented

            var watch = new System.Diagnostics.Stopwatch();

            watch.Start();

            sub.Call();

            watch.Stop();

            var result = sub.GetArg(12);
            var returnFlag = sub.GetArg(13);


#if DEBUG
//            Console.WriteLine($@"uvFileName:         
//{uvFileName}

//selectionCriteria:  
//{uvSelectStatement}

//fieldsToReturn:     
//{fieldsToReturn}

//webData:        
//{webData}

//result:
//{result}

//");
            //Console.WriteLine($"Query returned in {watch.ElapsedMilliseconds}ms\r\n");
#endif

            return sub.GetArg(12);
        }
        
        public void ClearBasket(Models.Order order)
        {
            var sub = Session.CreateUniSubroutine("RF.ASPLINK.V3", 15);

            var fileName = "CLEAR_BASKET";
            var selectionCriteria = "";
            var fieldsToReturn = "";
            var returnType = "TEXT";
            var pageSize = 0;
            var pageNumber = 1;
            var webData = "";

            sub.SetArg(0, order.BasketID);
            sub.SetArg(1, fileName);
            sub.SetArg(2, selectionCriteria?.Replace("'", "\"") ?? "");
            sub.SetArg(3, fieldsToReturn);
            sub.SetArg(4, returnType);
            sub.SetArg(5, $"{pageSize}");
            sub.SetArg(6, $"{pageNumber}");
            sub.SetArg(7, $"{webData}");
            sub.SetArg(8, "");
            sub.SetArg(9, "");
            sub.SetArg(10, "");
            sub.SetArg(11, "");
            sub.SetArg(12, "");
            sub.SetArg(13, "");
            sub.SetArg(14, "");

            sub.Call();

            Console.WriteLine($"Issued {fileName} command, got:\r\n{sub.GetArg(12)}");
        }

        //public string B2C_GetBasket(Models.Order order)
        //{
        //    var sub = Session.CreateUniSubroutine("RF.ASPLINK.V3", 15);

        //    var fileName = "B2C_GET_BASKET";
        //    var selectionCriteria = order.BasketID;
        //    var fieldsToReturn = "LINENO CUSPRODCODE.WI PRODDESC QTYORD.ORD SAMPLE.IND DISC.PERC ITEM.CNT ITEM.LEN LINE.NOTES CUS.ORD.REF RESERVE.IND WITH.CUTTING SPECIFIC.BATCH ALLOCATED WI.STATUS.IMAGE LINE.STATUS ORDPRICE.CUR";
        //    var returnType = "XML";
        //    var pageSize = 0;
        //    var pageNumber = 1;
        //    var webData = "";

        //    sub.SetArg(0, order.BasketID);
        //    sub.SetArg(1, fileName);
        //    sub.SetArg(2, selectionCriteria);
        //    sub.SetArg(3, fieldsToReturn);
        //    sub.SetArg(4, returnType);
        //    sub.SetArg(5, $"{pageSize}");
        //    sub.SetArg(6, $"{pageNumber}");
        //    sub.SetArg(7, $"{webData}");
        //    sub.SetArg(8, "");
        //    sub.SetArg(9, "");
        //    sub.SetArg(10, "");
        //    sub.SetArg(11, "");
        //    sub.SetArg(12, "");
        //    sub.SetArg(13, "");
        //    sub.SetArg(14, "");

        //    sub.Call();

        //    Console.WriteLine($"Issued {fileName} command, got:\r\n{sub.GetArg(12)}");

        //    return $"{sub.GetArg(12)}";
        //}

        public XDocument UpdateOrderStatus(int OrderHeadID)
        {
            using (var qe = new QueryExecutor())
            {
                qe.Session.Timeout = 40;

                //var eResultXML = new XmlDocument();

                var xDoc = XDocument.Parse(qe.Run("LIST_ORDER_LINES",
                    $@"ORDERNO=""{OrderHeadID}""",
                    "ORDNO LINE DATEONFILE QTYORD.ORD WI.STDPROD TEXT.DESC CUS.ORD.REF STATUS WI.PARCEL.TRACK.URL CURSUG.DELDATE QTYCAN.ORD"));

                //eResultXML.LoadXml(result);

                DataTable OrderStatusTable = new DataTable();
                OrderStatusTable.Columns.Add("OrderHeadID", typeof(int));
                OrderStatusTable.Columns.Add("OrderLineNo", typeof(short));
                OrderStatusTable.Columns.Add("OrderStatusID", typeof(byte));
                OrderStatusTable.Columns.Add("EstimatedDeliveryDate", typeof(DateTime));
                OrderStatusTable.Columns.Add("Code", typeof(string));
                OrderStatusTable.Columns.Add("Quantity", typeof(short));
                OrderStatusTable.Columns.Add("CancelledQuantity", typeof(short));
                OrderStatusTable.Columns.Add("ParcelTrackingURL", typeof(string));

                xDoc.Descendants("record").ToList().ForEach(c =>
                {
                    //c.XPathSelectElement("descendant::fd[@id='C9']").Attribute("value").Value = System.Web.UpdateOrderStatusHttpUtility.UrlEncode(c.XPathSelectElement("descendant::fd[@id='C9']").Attribute("value").Value);

                    var parcelTrackingInfo = c.XPathSelectElement("descendant::fd[@id='C9']").Attribute("value").Value.Split(new string[] { "{UV_URL_Seperator}" }, StringSplitOptions.None);

                    foreach (string trackingUrl in parcelTrackingInfo)
                    {
                        DateTime? estDelDate = null;

                        if (!String.IsNullOrEmpty(c.XPathSelectElement("descendant::fd[@id='C10']").Attribute("value").Value)
                            && c.XPathSelectElement("descendant::fd[@id='C10']").Attribute("value").Value != "TBA")
                        {
                            estDelDate = DateTime.Parse(c.XPathSelectElement("descendant::fd[@id='C10']").Attribute("value").Value);
                        }

                        OrderStatusTable.Rows.Add(
                            OrderHeadID,
                            Convert.ToInt16(c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value),
                            Convert.ToByte(c.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value),
                            estDelDate,
                            Convert.ToString(c.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value),
                            Convert.ToInt16(Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value)),
                            Convert.ToInt16(Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value)),
                            trackingUrl
                        );
                    }
                });

                using (SqlConnection conn = new SqlConnection("Data Source=VM-SQL1;Initial Catalog=Flock;Integrated Security=false;Persist Security Info=True;User ID=dg;Password=mattm00"))
                {
                    SqlCommand cmd = new SqlCommand("adm.OrderStatus_Merge", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.Clear();
                    SqlParameter os = cmd.Parameters.AddWithValue("@OrderStatuses", OrderStatusTable);
                    os.SqlDbType = SqlDbType.Structured;
                    os.TypeName = "MertexOrderStatusTableType";
                    conn.Open();
                    cmd.ExecuteReader();
                }

                return xDoc;
            }
        }
    }
}
