using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tester.Utilities;

namespace Tester.Tests.Credit
{
    public static class CreditTestSuite
    {
        public static void Run(string tradeDataSourceJson)
        {
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning Credit Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            MertexerV2.SQLFunctions.Credit.Check(tradeDataSourceJson);

            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine($"Credit Tests Complete.");

            Console.WriteLine(TesterFramework.WavyLineBreak);
        }
    }
}
