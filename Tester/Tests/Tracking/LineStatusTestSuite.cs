using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tester.Utilities;

namespace Tester.Tests.Tracking
{
    public static class TrackingTestSuite
    {
        public static void Run(byte transactionSourceId, DateTime fromDt)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning Tracking Tests...");

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

            var results = MertexerV2.SQLFunctions.LineTracking.BatchCheck(transactionSourceId, fromDt);

            stopWatch.Stop();

            Console.WriteLine($"Tracking parallel operation returned {(results as ArrayList).Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"Tracking Tests Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.Beep(37, 300);
            Console.Beep(2820, 300);
            System.Threading.Thread.Sleep(20);
            Console.Beep(2800, 190);
            Console.Beep(2800, 190);
            System.Threading.Thread.Sleep(20);
            Console.Beep(3800, 300);
        }
    }
}
