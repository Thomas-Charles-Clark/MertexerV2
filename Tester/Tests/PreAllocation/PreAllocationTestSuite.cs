using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tester.Utilities;

namespace Tester.Tests.PreAllocation
{
    public static class PreAllocationTestSuite
    {
        public static void Run(string account)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning PreAllocation Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            //MertexerV2.SQLFunctions.PreAllocation.Delete("TESTING");

            MertexerV2.SQLFunctions.PreAllocation.Create("TESTING", "FDG2735/01", "WH", 2, "M", null, null, DateTime.Now.AddMinutes(10), null, null);

            //MertexerV2.SQLFunctions.PreAllocation.Update("TESTING", "FDG2735/01", "WH", 2, "M", null, null, DateTime.Now.AddMinutes(10), null, null);


            Console.WriteLine($"PreAllocation Tests Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);
        }
    }
}
