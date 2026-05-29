using MertexerV2;
using MertexerV2.Models;
using MertexerV2.Models.Status;
using MertexerV2.Models.VAT;
using MertexerV2.Models.EORI;
using MertexerV2.SQLFunctions;
using MertexerV2.Utilities;
using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Tester.Utilities;
using MertexerV2.Models.PreAllocation;
using Tester.Tests.OrderSearch;
using Newtonsoft.Json;
using System.Collections.Generic;
using Tester.Tests.EDAReplacement;
using MertexerV2.Models.EDAReplacement;
using System.Threading.Tasks;
using System.Data.SqlTypes;
using MertexerV2.GeneralFunctions;
using MertexerV2.GeneralFunctions.Pricing;
using MertexerV2.ProductFunctions;
using MertexerV2.Models.Product;
using MertexerV2.Models.Pricing;
using MertexerV2.Models.General;
using MertexerV2.Models.Invoices;
using System.Globalization;
using MertexerV2.InvoiceFunctions;
using MertexerV2.Models.Stock;
using MertexerV2.Models.Accounts;
using MertexerV2.AccountFunctions;
using MertexerV2.TradeAccountFunctions;
using System.CodeDom;
using MertexerV2.RetailFunctions;
using System.Diagnostics.Tracing;
using MertexerV2.EDAReplacement;
using MertexerV2.Models.CustomerCare;
using System.Security.Cryptography.X509Certificates;
//using MertexerV2SQL;
using System.Management.Instrumentation;
using MertexerV2.PricingFunctions;
using System.Text;
using Tester.Tests.LineStatus;
using System.IO;
using MertexerV2.Models.SalesAnalysis;
using MertexerV2.Models.PurchaseLedger;
using MertexerV2.SupplierFunctions;
using MertexerV2.Models.Supplier;
using MertexerV2.SalesLedgerFunctions;
using MertexerV2.Models.SalesLedger;
using MertexerV2.Models.Credit;
using MertexerV2.Models.GeneralLedger;
using MertexerV2.GeneralLedgerFunctions;
using MertexerV2.PurchaseLedgerFunctions;
using MertexerV2.Models.Company;

using CFramework = ConsoleFramework.Framework;
using MertexerV2.Models.ExchangeRate;
using Newtonsoft.Json.Linq;
using static System.Collections.Specialized.BitVector32;
using MertexerV2.CompanyFileFunctions;
using System.Linq.Expressions;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;
using System.Windows.Forms;
using MertexerV2.Models.Bank;
using Newtonsoft.Json.Schema;

namespace Tester
{
    class Program
    {
        private static readonly string WigglyLine = "~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~";

        static void Main()
        {
            JsonConvert.DefaultSettings = () => {
                return new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Include,
                    //ContractResolver = new OrderedContractResolver()
                };
            };

            Console.OutputEncoding = Encoding.Unicode;

            CFramework.SetConsoleColourBusy();

            CFramework.WriteHeading("MertexerV2.dll Tests");

            //var tradeAccountCode = "K0098";

            var sw = new System.Diagnostics.Stopwatch();

            var uniVerseAccount = "MERTEX";

			//var tradeAccountCode = "K0098";

			sw.Start();

            CFramework.WriteSubHeading($"Working in {uniVerseAccount}");

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
            //
            //  INVCLCODE / INVCLDESC / INVCLAUSE issue:
            //  
            //  SLCUST<0> "INVCLDESC" derives F(OECLAUSES<1> "DESCRIPTION")<202>
            //  SLCUST<202.M> "INVCLCODE" holds keys into OECLAUSES
            //  SLCUST<203.M> "INVCLAUSE"*** holds the result of the lookup F(OECLAUSES<2.M>)<202.M>
            //
            //  ***THIS IS NOT A DERIVED FIELD* because of unforgivably stupid reasons
            //                                  actually this sort of thing was commonplace back when
            //                                  systems were limited by hard drive read speeds - moving
            //                                  the reading head to a different location or even a different
            //                                  platter was so slow that it was beneficial to store data congruently
            //                                  at the cost of it becoming stale - as long as you catered for this potential-for-staleness
            //                                  e.g. by offering or mandating a propagation of any change across records holding the data
            /* 
			    INVCLDESC < 0.M >   (field position 0 because it is a derived field, reading from OECLAUSES)
                INVCLCODE < 202.M > (must hold key(s) to OECLAUSES)
                INVCLAUSE < 203.M > (ACTUALLY HOLDS THE TEXT VALUE FROM THE OECLAUSE TABLE BASED ON THE KEY LOOKUP WHEN FIRST CREATED)
			*/
            //  So to repair the damage / bring any historical SLCUST records' INVCLAUSE fields up-to-date, 
            //  we'd need to run a fix routine to:
            //      a) var allClauses = OECLAUSES
            //      b) foreach (var i = 0; i < customer.InvoiceClauseCodes.length; i++) {
            //      c)  customer.InvoiceClauses[i] = allClauses.FirstOrDefault(c => c.Code == customer.InvoiceClauseCodes[i]).Clause;
            //      d) }
            //
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

            //var tradeAccountJson = $@"{{ ""Code"": ""K0098"", ""InvoiceClauseCodes"": [""CB4""] }}";

            //var result = TradeAccountFunctions.Upsert(uniVerseAccount, tradeAccountJson);

            //var results = InvoiceClauseFunctions.Select(uniVerseAccount, null).OfType<InvoiceClause>().ToList();

            //         var results = TradeAccountFunctions.UpsertMultiple(uniVerseAccount, JsonConvert.SerializeObject(new[] { 
            //             new 
            //             {
            //	    Code = "Q6135",
            //	    CreditControlRegion = (string)null
            //	}
            //}));

            //var results = EDAReplacement.FetchOrderLinesForOrderHeads(uniVerseAccount, @"[{ ""OrderID"": ""1215905201"" }]", null, null, null)
            //    .OfType<TOELine>().ToList();

            //Console.WriteLine(JsonConvert.SerializeObject(results));

            ValidateJSON("C:\\\\Users\\tclark\\source\\repos\\MertexerV2\\MertexerV2\\Notes\\FullSalesLedgerDataSet.json");

            CFramework.SetConsoleColourSuccess();

            sw.Stop();

            Console.WriteLine($"Completed in {sw.Elapsed}");

            Console.WriteLine();

            Console.WriteLine("Press any key to continue...");

            Console.ReadKey();
        }

        private static string Dictionary(string fileName, string database = "MERTEX")
        {
            using (var ce = new CommandExecutor(database))
            {
                return ce.Dictionary(fileName);
            }
        }

        private static string DictionaryXML(string fileName, string database = "MERTEX")
        {
            using (var ce = new CommandExecutor(database))
            {
                return ce.DictionaryXML(fileName);
            }
        }


        private static string DictionaryVertical(string fileName)
        {
            using (var ce = new CommandExecutor())
            {
                return ce.DictionaryVertical(fileName);
            }
        }

        private static string DictionaryItem(string fileName, string[] fieldNames = null, string universeAccount = "MERTEX")
        {
            var result = "";

            using (var ce = new CommandExecutor(universeAccount))
            {
                result = ce.DictionaryItem(fileName, fieldNames);
            }

            //Console.WriteLine(item);

            return result;
        }

        private static string Item(string fileName, string recordId, bool lineSuppression = true, string universeAccount = "MERTEX")
        {
            var item = "";

            using (var ce = new CommandExecutor(universeAccount))
            {
                item = ce.Item(fileName, recordId, lineSuppression);
            }

            return item;
        }

        public static int WTAccessInternalImportDenial(string universeAccount)
        {
            int result = 0;

            using (var ce = new CommandExecutor(universeAccount))
            {
                // need to alter every record in WTACCESS

                var fields = new string[] { "IMPORT" };

                var wtaccess = ce.Session.CreateUniFile("WTACCESS");

                var keys = new List<string>();

                var listCmd = $"LIST WTACCESS PIN FMT\"40L\" ID.SUPP COL.HDR.SUPP COUNT.SUP";
                var listResult = ce.Arbitrary(listCmd);
                var theseKeys = listResult
                    .Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                keys.AddRange(theseKeys);

                var batches = ListExtension.Split(keys, 1000);

                var locker = new object();

                //Parallel.ForEach(batches, (batch) =>
                //{
                foreach (var batch in batches)
                {
                    try
                    {
                        var records = wtaccess.ReadRecords(batch.ToArray(), fields);

                        foreach (var b in batch)
                        {
                            var readResult = wtaccess.ReadField(b, 116);

                            var toWrite = ce.Session.CreateUniDynArray();

                            toWrite.Insert(0, "N");

                            wtaccess.LockRecord(b, 1);
                            wtaccess.WriteField(b, 116, toWrite);
                            wtaccess.UnlockRecord(b);
                        }
                    }
                    catch (IBMU2.UODOTNET.UniFileException ufe)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ufe);
                    }
                }
                //});
            }

            return result;
        }

        public static void ValidateJSON (string filePath)
		{
			try
			{
				// Stream the file from disk using minimal memory
				using (StreamReader sr = new StreamReader(filePath))
				using (JsonTextReader reader = new JsonTextReader(sr))
				{
					// Read through tokens sequentially
					while (reader.Read())
					{
						// Optional: Log progress periodically based on depth or path
					}
				}
				Console.WriteLine("Success: The JSON file is perfectly valid.");
			}
			catch (JsonReaderException ex)
			{
				// Catch the specific parsing error and output coordinates
				Console.WriteLine("Breakage found in JSON structure!");
				Console.WriteLine($"Error Message: {ex.Message}");
				Console.WriteLine($"Line Number: {ex.LineNumber}");
				Console.WriteLine($"Line Position (Character): {ex.LinePosition}");
				Console.WriteLine($"Path where error occurred: {ex.Path}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"System Error: {ex.Message}");
			}
		}
    }

    public class SessionResult
    {
        public long SessionNo { get; set; }
        public DateTime OpenedOn { get; set; }
        public TimeSpan OpenedAt { get; set; }
        public string Label { get; set; }
    }
}