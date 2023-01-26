using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using MertexerV2.Utilities;
using MertexerV2.Models;
using Tester.Utilities;
using System.Data.SqlClient;
using MertexerV2.Models.OrderImport;

namespace Tester.Tests.ValidateCSV
{
    /*
     * 
       -------------------------------- Standard order fields
       0  TradeAccountNumber
       1  OrderReference
       2  DeliveryName
       3  DeliveryAddressLine1
       4  DeliveryAddressLine2
       5  DeliveryTown
       6  DeliveryCounty
       7  DeliveryPostcode
       8  DeliveryCountryCode
       9  DeliveryPhoneNumber
       10 Email
       11 ProductCode
       12 Quantity
       13 Type
       14 LineReference
       15 Memo [This is the minimum length, I believe, for "Functional" as the current state of affairs is...]
       16 ShippingAlertEmailAddress
       17 ContactPhone
       -------------------------------- Distributor only fields beneath
       18 HaulierCode
       19 CarriageCode
       20 HaulierAccountNumber
       21 TaxesDutiesToInvoiceAccount
       22 DeliveryTerms
       23 DeliveryType [O&L think this is Carriage Code, but their files are only ever processed as SORDI and never DISTRIBUTOR]
       24 EndCustomer

     */

    public static class ValidateCSV
    {
        internal static List<string> KnownGoodFiles { get; set; }

        internal static List<string> KnownBadFiles { get; set; }

        internal static List<string> AllFiles { get; set; }

        internal static readonly string BaseImportDirectory = Environment.CurrentDirectory + "..\\..\\..\\Tests\\ValidateCSV";

        internal static readonly string[] Delimiter = new string[] { "," };

        internal static readonly List<string> ValidTypes = new List<string> {
            "BUNDLE",
            "PRODUCT",
            "SAMPLES CUTTING",
            "RETURNABLE SAMPLE",
            "RESERVE",
            "RESERVE WITH BATCH CUTTING",
            "HANGER",
            "SHOWLENGTH",
            "WATERFALL ONLY"
        }; 

        public static void Run(int tradeOrderImportFileId)
        {
            // We have two collections of files: known-good and known-bad.
            // This test should parse both, and the results should tally exactly with the a-priori knowledge of the sets.
            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine("Beginning CSV Validation Tests...");

            Console.WriteLine(TesterFramework.WavyLineBreak);

            MertexerV2.SQLFunctions.Order.ProcessImport(tradeOrderImportFileId);

            Console.WriteLine(TesterFramework.WavyLineBreak);

            Console.WriteLine($"CSV Validation Tests Complete");

            Console.WriteLine(TesterFramework.WavyLineBreak);
        }
    }

    public class FileImportCheckingCollection
    {
        public List<FileImportGeneratedOrderCount> Files { get; set; }

        public FileImportCheckingCollection()
        {
            Files = new List<FileImportGeneratedOrderCount>();
        }
    }

    public class FileImportGeneratedOrderCount
    {
        public long TradeOrderImportFileID { get; set; }
        public int GeneratedOrderHeadCount { get; set; }
        public int ShouldHaveGeneratedOrderHeadCount { get; set; }
        public FileValidationResult Result { get; set; }
    }
}