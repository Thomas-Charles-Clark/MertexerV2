using StockChecker.Functions;
using StockChecker.Models;
using StockChecker.Utilities;

using System;
using System.Diagnostics;
using System.Linq;

namespace StockChecker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var sw = Stopwatch.StartNew();

            Console.OutputEncoding = System.Text.Encoding.Unicode;

            Framework.SetConsoleColourGreen();

            Framework.WriteHeading("StockChecker");

            var results = B2C.B2CEnquiry().OfType<B2CEnquiryResult>().ToList();

            sw.Stop();

            Framework.WriteLine($"Execution completed in {sw.Elapsed}");

            Framework.WriteLine("Press any key to continue...");

            Console.ReadKey();
        }
    }
}
