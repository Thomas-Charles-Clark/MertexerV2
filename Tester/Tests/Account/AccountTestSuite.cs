using MertexerV2.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tester.Utilities;
using Newtonsoft.Json;
using MertexerV2.Models.EDAReplacement;
using MertexerV2.GeneralFunctions;
using MertexerV2.Models;
using MertexerV2.TradeAccountFunctions;

namespace Tester.Tests.Account
{
    public static class AccountTestSuite
    {
        public static ListProformaInvoicesReturn ProformaInvoices(string tradeDataSourceJson, PassProformaInvoiceSearch objSearch)
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("AccountTestSuite ProformaInvoices Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var result = MertexerV2.Accounts.AccountFunctions.ProformaInvoices(tradeDataSourceJson, objSearch);

                Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"AccountTestSuite ProformaInvoices Test Complete.");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return result;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nAccountTestSuite ProformaInvoices Test Failed");

                return null;
            }
        }

        public static InvoiceSearchReturn InvoiceSearch(string tradeDataSourceJson, PassInvoiceSearch objSearch)
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("AccountTestSuite InvoiceSearch Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var result = MertexerV2.Accounts.AccountFunctions.InvoiceSearch(tradeDataSourceJson, objSearch);

                Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"AccountTestSuite InvoiceSearch Test Complete.");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return result;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nAccountTestSuite InvoiceSearch Test Failed");

                return null;
            }
        }

        public static AccountSummaryReturn AccountSummary(string tradeDataSourceJson)
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("AccountTestSuite AccountSummary Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var result = MertexerV2.Accounts.AccountFunctions.AccountSummary(tradeDataSourceJson);

                Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"AccountTestSuite AccountSummary Test Complete.");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return result;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nAccountTestSuite AccountSummary Test Failed");

                return null;
            }
        }

        public static ListInvoicesReturn AccountBalanceSummary(string tradeDataSourceJson, string criteria)
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("AccountTestSuite AccountBalanceSummary Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var result = MertexerV2.Accounts.AccountFunctions.ListInvoices(tradeDataSourceJson, criteria);

                Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"AccountTestSuite AccountBalanceSummary Test Complete.");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return result;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nAccountTestSuite AccountSummary Test Failed");

                return null;
            }
        }

        //public static List<MertexPCALookupResult> AddressBookSearch(string tradeDataSourceJson, PassPCAPredictFind objSearch)
        //{
        //    try
        //    {
        //        TesterFramework.SetConsoleColourTesting();

        //        Console.WriteLine(TesterFramework.WavyLineBreak);

        //        Console.WriteLine("AccountTestSuite AddressBookSearch Test...");

        //        Console.WriteLine(TesterFramework.WavyLineBreak);

        //        var result = MertexerV2SQL.Accounts.AccountFunctionsUniSQL.AddressBookSearch(tradeDataSourceJson, objSearch);

        //        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));

        //        Console.WriteLine($"Saw {result.Count} Results.");

        //        Console.WriteLine(TesterFramework.WavyLineBreak);

        //        Console.WriteLine($"AccountTestSuite AddressBookSearch Test Complete.");

        //        Console.WriteLine(TesterFramework.WavyLineBreak);

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        TesterFramework.SetConsoleColourError();

        //        Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

        //        Console.WriteLine("\nAccountTestSuite AddressBookSearch Test Failed");

        //        return null;
        //    }
        //}

        public static void AccountBatchSelect(string uniVerseAccount, string tradeAccountListJson)
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("AccountTestSuite BatchSelect Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

                sw.Start();

                var results = TradeAccountFunctions.Select(uniVerseAccount, tradeAccountListJson).OfType<TradeAccount>().ToList();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"AccountTestSuite BatchSelect Complete: {results.OfType<TradeAccount>().Count()}");

                sw.Stop();

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine($"Fetched {results.Count()} SLCUST records in {sw.Elapsed}");

                Console.WriteLine(TesterFramework.WavyLineBreak);
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nAccountTestSuite BatchSelect Test Failed");
            }
        }

        public static void UpsertAccount(string tradeDataSourceJson, string tradeAccountJson)
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("AccountTestSuite UpsertAccount Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var accountCode = MertexerV2.TradeAccountFunctions.TradeAccountFunctions.Upsert(tradeDataSourceJson, tradeAccountJson);

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"AccountTestSuite UpsertAccount Complete: {accountCode}");

                Console.WriteLine(TesterFramework.WavyLineBreak);
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nAccountTestSuite UpsertAccount Test Failed");
            }
        }

        public static void TradeAccountList(string uniVerseAccount, bool? accountOpen, DateTime? createdAfterDt, DateTime? createdBeforeDt)
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("AccountTestSuite TradeAccountList Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var accounts = MertexerV2.EDAReplacement.EDAReplacement.ListTradeAccounts(uniVerseAccount, accountOpen, createdAfterDt, createdBeforeDt).OfType<SLCustStub>().ToList();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"AccountTestSuite TradeAccountList Complete: {accounts.Count()}");

                Console.WriteLine(TesterFramework.WavyLineBreak);
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nAccountTestSuite UpsertAccount Test Failed");
            }
        }
    }
}
