using MertexerV2.Models.SalesAnalysis;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tester.Utilities;

namespace Tester.Tests.LineStatus
{
    public static class SalesAnalysisTestSuite
    {
        public static IEnumerable<SalesAnalysis> Select(string uniVerseAccount, string salesAnalysisIds, DateTime? fromDt, DateTime? toDt)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning SalesAnalysis Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.InvoiceFunctions.SalesAnalysisFunctions.Select(uniVerseAccount, salesAnalysisIds, fromDt, toDt).OfType<SalesAnalysis>().ToList();

            stopWatch.Stop();

            Console.WriteLine($"SalesAnalysis parallel operation returned {results.Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"SalesAnalysis Tests Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            //Console.Beep(37, 300);
            //Console.Beep(2820, 300);
            //System.Threading.Thread.Sleep(20);
            //Console.Beep(2800, 190);
            //Console.Beep(2800, 190);
            //System.Threading.Thread.Sleep(20);
            //Console.Beep(3800, 300);

            return results;
        }


        public static IEnumerable<SalesAnalysisHistory> HistorySelect(string uniVerseAccount, string salesAnalysisIds, DateTime? fromDt, DateTime? toDt)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning SalesAnalysisHistory Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            var stopWatch = new Stopwatch();

            stopWatch.Start();

            var results = MertexerV2.InvoiceFunctions.SalesAnalysisFunctions.HistorySelect(uniVerseAccount, salesAnalysisIds, fromDt, toDt).OfType<SalesAnalysisHistory>().ToList();

            stopWatch.Stop();

            Console.WriteLine($"SalesAnalysisHistory parallel operation returned {results.Count} in {stopWatch.Elapsed:g}");

            Console.WriteLine($"SalesAnalysisHistory Tests Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            //Console.Beep(37, 300);
            //Console.Beep(2820, 300);
            //System.Threading.Thread.Sleep(20);
            //Console.Beep(2800, 190);
            //Console.Beep(2800, 190);
            //System.Threading.Thread.Sleep(20);
            //Console.Beep(3800, 300);

            return results;
        }
    }
}
