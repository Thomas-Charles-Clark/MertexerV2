using MertexerV2;
using MertexerV2.Models;
using MertexerV2.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tester.Utilities;

namespace Tester.Tests.Ordering
{
    public static class OrderTestSuite
    {
        public static void Run(string account)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning Order Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var dataSet = new DataSet();

            var orderHeadTable = TableBuilder.OrderHeaderTable();

            var orderLineTable = TableBuilder.OrderLineTable();

            var giftBoxTable = TableBuilder.GiftBoxTable();

            dataSet.Tables.Add(orderHeadTable);

            dataSet.Tables.Add(orderLineTable);

            dataSet.Tables.Add(giftBoxTable);

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["VM-SQL1"].ConnectionString))
            {
                using (var comm = new SqlCommand("tom.MertexerV2_TestOrder_Select", conn))
                {
                    comm.CommandType = CommandType.StoredProcedure;

                    var adapter = new SqlDataAdapter(comm);

                    adapter.TableMappings.Add("Table", "OrderHead");
                    adapter.TableMappings.Add("Table1", "OrderLines");
                    //adapter.TableMappings.Add("Table2", "GiftBoxes");

                    adapter.Fill(dataSet);
                }
            }

            var ordersPlaced = new List<string>();

            var ordersFailed = new List<string>();

            var allOrders = new List<Order>();

            foreach (var orderHead in dataSet.Tables["OrderHead"].AsEnumerable())
            {
                var testOrderNumber = Properties.Settings.Default.testOrderNumber;
                orderHead["OrderNumber"] = testOrderNumber;

                Console.WriteLine(TesterFramework.WavyLineBreak);
                Console.WriteLine($"Attempting to construct order object {testOrderNumber}");

                var customerReference = orderHead["CustomerReference"].ToString();

                var orderLines = dataSet.Tables["OrderLines"]; // Not quite. We now want each line separately ordered! Okay... How.

                //var giftBoxes = dataSet.Tables["GiftBoxes"];

                foreach(var line in orderLines.AsEnumerable())
                {
                    line["CustomerReference"] = customerReference;
                }

                //foreach(var line in giftBoxes.AsEnumerable())
                //{
                //    line["CustomerReference"] = customerReference;
                //}

                using (var oc = new OrderController(account))
                {
                    //foreach (var orderLine in orderLines.AsEnumerable())
                    //{
                        //orderHead["OrderNumber"] = testOrderNumber;

                        //var olt = orderLines.Clone();

                        //olt.ImportRow(orderLine);

                        var order = new Order(
                            orderHead,
                            orderLines,
                            //olt,
                            giftBoxTable);

                        allOrders.Add(order);

                        Console.WriteLine($"Constructed order object {testOrderNumber} successfully.");

                        Console.WriteLine(TesterFramework.WavyLineBreak);
                        
                        Console.WriteLine($"Placing order {testOrderNumber}...");
                        
                        //var result = oc.PlaceOrder(order);
                        
                        //if (result.Contains(testOrderNumber.ToString()))
                        //{
                        //    ordersPlaced.Add(testOrderNumber.ToString());
                        //}
                        //else
                        //{
                        //    ordersFailed.Add(testOrderNumber.ToString());
                        //}

                        //Console.WriteLine($"{result}");

                        testOrderNumber++;

                        Properties.Settings.Default.testOrderNumber = testOrderNumber;
                        Properties.Settings.Default.Save();
                    //}
                }
            }

            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine($"Order Tests Complete: {ordersPlaced.Count} orders placed successfully, {ordersFailed.Count} orders failed.");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            if (ordersFailed.Count > 0)
            {
                Retry(account, allOrders.Where(p => ordersFailed.Contains(p.OrderNumber)).ToList());
            }
            else
            {
                Console.Beep(37, 300);
                Console.Beep(2820, 300);
                System.Threading.Thread.Sleep(20);
                Console.Beep(2800, 190);
                Console.Beep(2800, 190);
                System.Threading.Thread.Sleep(20);
                Console.Beep(3800, 300);
            }
        }

        public static void Retry(string account, List<Order> orders)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Retrying Failed Orders...");

            using (var oc = new OrderController(account))
            {
                foreach (var order in orders)
                {

                    Console.WriteLine($"Retrying order {order.OrderNumber}...");

                    var result = oc.PlaceOrder(order);

                    Console.WriteLine($"{result}");

                }
            }

            Console.WriteLine("Done...");

            Console.Beep(37, 300);
            Console.Beep(2820, 300);
            System.Threading.Thread.Sleep(20);
            Console.Beep(2800, 190);
            Console.Beep(2800, 190);
            System.Threading.Thread.Sleep(20);
            Console.Beep(3800, 300);

            Console.WriteLine(TesterFramework.WavyLineBreak);
        }
    }
}