using MertexerV2.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tester.Utilities;

namespace Tester.Tests
{
    public static class B2C_STOCK_ITEM_BREAKDOWN
    {
        public static void Run (int productCount = 10, string account = "PLAY", string[] stockLocations = null)
        {
            Console.WriteLine(Framework.WavyLineBreak);

            Console.WriteLine($"Running B2C_STOCK_ITEM_BREAKDOWN_PRODUCT on {account} with {productCount} products. Checking {(stockLocations == null ? "all" : string.Join(",", stockLocations))} stock locations.\r\n");

            var productCodesString = "";

            using (var commandExecutor = new CommandExecutor(account))
            {
                productCodesString = commandExecutor.Arbitrary($@"LIST PRODCODE STPRODMASTER BY.DSND PRODCODE WITH GROUPCODE = ""FA"" AND CUR.STAT = ""C"" AND PRODCODE LIKE ""FDG..."" SAMPLE {productCount} ID.SUP COUNT.SUP COL.SUP HDR.SUP");
            }

            var productCodesArray = productCodesString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            var uvSelectStatement = $"PRODCODE = \"{String.Join("\" OR PRODCODE = \"", productCodesArray)}\"";

            using (var queryExecutor = new QueryExecutor(account))
            {
                queryExecutor.Run(
                    uvFileName: "B2C_STOCK_ITEM_BREAKDOWN_PRODUCT",
                    uvSelectStatement: uvSelectStatement,
                    fieldsToReturn: "",
                    webData: "",
                    returnType: "XML");
            }

            Console.WriteLine(Framework.WavyLineBreak);
        }

        public static void Run(string productCode, string account = "PLAY", string[] stockLocations = null)
        {
            Console.WriteLine(Framework.WavyLineBreak);

            Console.WriteLine($"Running B2C_STOCK_ITEM_BREAKDOWN_PRODUCT on {account} for {productCode}. Checking {(stockLocations == null ? "all" : string.Join(",", stockLocations))} stock locations.\r\n");

            var uvSelectStatement = $"PRODCODE = \"{productCode}\"";

            using (var queryExecutor = new QueryExecutor(account))
            {
                queryExecutor.Run(
                    uvFileName: "B2C_STOCK_ITEM_BREAKDOWN_PRODUCT",
                    uvSelectStatement: uvSelectStatement,
                    fieldsToReturn: "",
                    webData: "",
                    returnType: "XML");
            }

            Console.WriteLine(Framework.WavyLineBreak);
        }
    }
}
