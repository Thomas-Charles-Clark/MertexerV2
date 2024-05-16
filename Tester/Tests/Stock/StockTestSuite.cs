using MertexerV2.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tester.Utilities;

namespace Tester.Tests.Stock
{
    public static class StockTestSuite
    {
        public static void Run(string account)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning Stock Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            //using (var queryExecutor = new QueryExecutor(account))
            //{
            //    var result = queryExecutor.Run(
            //        uvFileName: "B2C_STOCK_ENQUIRY",
            //        uvSelectStatement: "\"F1190/99\"",
            //        fieldsToReturn: "PRODCODE WI.TOTFREE WI.DUEIN.SEL WI.DUEIN.OSQTY WI.DUEIN.OSQTY.TOTAL",
            //        webData: "",
            //        returnType: "XML");
            //    Console.WriteLine("\r\n");
            //}

            //MertexerV2.SQLFunctions.Stock.BatchPiece("F1190/99", "WH", "S");

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            Console.WriteLine($"Beginning Realtime stock check");

            var results = MertexerV2.SQLFunctions.Stock.B2CEnquiry(TesterFramework.FetchB2CRealtimeStockCheckJson());

            Console.WriteLine($"Realtime stock check operation returned {(results as ArrayList).Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine();

            Console.WriteLine();

            stopWatch.Reset();

            stopWatch.Start();

            Console.WriteLine($"Beginning Discontinued stock check");

            var discoResults = MertexerV2.SQLFunctions.Stock.B2CEnquiry(TesterFramework.FetchB2CDiscontinuedStockCheckJson());

            Console.WriteLine($"Discontinued stock check operation returned {(discoResults as ArrayList).Count} in {stopWatch.Elapsed:g}");

            stopWatch.Stop();

            Console.WriteLine($"Stock Tests Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            //Console.Beep(37, 300);
            //Console.Beep(2820, 300);
            //System.Threading.Thread.Sleep(20);
            //Console.Beep(2800, 190);
            //Console.Beep(2800, 190);
            //System.Threading.Thread.Sleep(20);
            //Console.Beep(3800, 300);
        }
    }
}