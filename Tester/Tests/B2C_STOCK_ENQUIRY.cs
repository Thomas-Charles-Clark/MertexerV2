using MertexerV2.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tester.Utilities;

namespace Tester.Tests
{
    public static class B2C_STOCK_ENQUIRY
    {
        public static void Run (int productCount = 10, string account = "PLAY")
        {
            Console.WriteLine(Framework.WavyLineBreak);

            Console.WriteLine($"Running B2C_STOCK_ENQUIRY on {account} with {productCount} products.\r\n");

            var productCodesString = "";

            using (var commandExecutor = new CommandExecutor(database: account, nativeMode: true, timeoutSeconds: 1))
            {
                productCodesString = commandExecutor.Arbitrary($@"LIST PRODCODE STPRODMASTER BY.DSND PRODCODE WITH GROUPCODE = ""FA"" AND CUR.STAT = ""C"" AND PRODCODE LIKE ""FDG..."" SAMPLE {productCount} ID.SUP COUNT.SUP COL.SUP HDR.SUP");
            }

            var productCodesArray = productCodesString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            var uvSelectStatement = $@"""{String.Join(@""" """, productCodesArray)}""";

            using (var queryExecutor = new QueryExecutor(account))
            {
                queryExecutor.Run(
                    uvFileName: "B2C_STOCK_ENQUIRY",
                    uvSelectStatement: uvSelectStatement,
                    fieldsToReturn: "PRODCODE WI.TOTFREE WI.DUEIN.SEL WI.DUEIN.OSQTY WI.DUEIN.OSQTY.TOTAL",
                    webData: "",
                    returnType: "XML");
            }

            Console.WriteLine(Framework.WavyLineBreak);
        }
    }
}
