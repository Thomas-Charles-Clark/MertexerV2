using MertexerV2.Models.General;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tester.Utilities;

namespace Tester.Tests.Framework
{
    public static class FrameworkTestSuite
    {
        public static IEnumerable<CodeDescription> CodeGroups(string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite CodeGroups Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetOrderEntryCodeGroups(universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite CodeGroups Test Complete: Saw {results.Count()} CodeGroups");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite CodeGroups Test Failed");

                return null;
            }
        }

        public static IEnumerable<CodeDescription> Markets(string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite Markets Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetMarkets(universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite Markets Test Complete: Saw {results.Count()} Markets");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite Markets Test Failed");

                return null;
            }
        }

        public static IEnumerable<CodeDescription> CustomerTypes(string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite CustomerTypes Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetCustomerTypes(universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite CustomerTypes Test Complete: Saw {results.Count()} CustomerTypes");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite CustomerTypes Test Failed");

                return null;
            }
        }

        public static IEnumerable<CodeDescription> CustomerClasses(string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite CustomerClasses Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetCustomerClasses(universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite CustomerClasses Test Complete: Saw {results.Count()} CustomerClasses");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite CustomerClasses Test Failed");

                return null;
            }
        }

        public static IEnumerable<CodeDescription> SalesRepresentatives(string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite SalesRepresentatives Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetSalesRepresentatives(universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite SalesRepresentatives Test Complete: Saw {results.Count()} SalesRepresentatives");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite SalesRepresentatives Test Failed");

                return null;
            }
        }

        public static IEnumerable<CodeDescription> SalesAgents(string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite SalesAgents Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetSalesAgents(universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite SalesAgents Test Complete: Saw {results.Count()} SalesRepresentatives");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite SalesAgents Test Failed");

                return null;
            }
        }

        public static IEnumerable<CodeDescription> SalesAreas(string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite SalesAreas Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetSalesAreas(universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite SalesAreas Test Complete: Saw {results.Count()} SalesAreas");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite SalesAreas Test Failed");

                return null;
            }
        }

        public static IEnumerable<CodeDescription> SalesDivisions(string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite SalesDivisions Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetSalesDivisions(universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite SalesDivisions Test Complete: Saw {results.Count()} SalesDivisions");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite SalesDivisions Test Failed");

                return null;
            }
        }

        public static IEnumerable<CodeDescription> VATCodes(string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite VATCodes Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetVATCodes(universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite VATCodes Test Complete: Saw {results.Count()} VATCodes");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite VATCodes Test Failed");

                return null;
            }
        }

        public static IEnumerable<CodeDescription> Currencies(string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite Currencies Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetCurrencies(universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite Currencies Test Complete: Saw {results.Count()} Currencies");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite Currencies Test Failed");

                return null;
            }
        }

        public static IEnumerable<CodeDescription> PriceLists(string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite PriceLists Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetPriceLists(universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite PriceLists Test Complete: Saw {results.Count()} PriceLists");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite PriceLists Test Failed");

                return null;
            }
        }

        public static IEnumerable<CodeDescription> Banks(string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite Banks Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetBanks(universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite Banks Test Complete: Saw {results.Count()} Banks");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite Banks Test Failed");

                return null;
            }
        }

        public static IEnumerable<CodeDescription> CreditControllers(string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite CreditControllers Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetCreditControllers(universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite CreditControllers Test Complete: Saw {results.Count()} CreditControllers");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite CreditControllers Test Failed");

                return null;
            }
        }

        public static IEnumerable<CodeDescription> PaymentTerms(string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite PaymentTerms Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetPaymentTerms(universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite PaymentTerms Test Complete: Saw {results.Count()} PaymentTerms");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite PaymentTerms Test Failed");

                return null;
            }
        }

        public static IEnumerable<CodeDescription> Hauliers(string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite Hauliers Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetHauliers(universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite Hauliers Test Complete: Saw {results.Count()} Hauliers");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite Hauliers Test Failed");

                return null;
            }
        }

        public static IEnumerable<CodeDescription> CarriageServices(string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite CarriageServices Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetCarriageServices(universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite CarriageServices Test Complete: Saw {results.Count()} CarriageServices");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite CarriageServices Test Failed");

                return null;
            }
        }

        public static IEnumerable<CodeDescription> DeliveryAddresses(string tradeAccountCode, string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite DeliveryAddresses Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetDeliveryAddresses(tradeAccountCode, universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite DeliveryAddresses Test Complete: Saw {results.Count()} DeliveryAddresses");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite DeliveryAddresses Test Failed");

                return null;
            }
        }

        public static IEnumerable<CodeDescription> StockLocations(string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite StockLocations Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetStockLocations(universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite StockLocations Test Complete: Saw {results.Count()} StockLocations");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite StockLocations Test Failed");

                return null;
            }
        }

        public static IEnumerable<CodeDescription> PromotionCodes(string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite PromotionCodes Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetPromotionCodes(universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite PromotionCodes Test Complete: Saw {results.Count()} PromotionCodes");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite PromotionCodes Test Failed");

                return null;
            }
        }

        public static IEnumerable<CodeDescription> SellingCompanies(string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite SellingCompanies Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetSellingCompanies(universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite SellingCompanies Test Complete: Saw {results.Count()} SellingCompanies");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite SellingCompanies Test Failed");

                return null;
            }
        }

        public static IEnumerable<CodeDescription> Distributors(string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite Distributors Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetDistributors(universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite Distributors Test Complete: Saw {results.Count()} Distributors");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite Distributors Test Failed");

                return null;
            }
        }

        public static IEnumerable<CodeDescription> Languages(string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite Languages Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetLanguages(universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite Languages Test Complete: Saw {results.Count()} Languages");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite Languages Test Failed");

                return null;
            }
        }

        public static IEnumerable<CodeDescription> ProductExclusionLists(string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite ProductExclusionLists Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetProductExclusionLists(universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite ProductExclusionLists Test Complete: Saw {results.Count()} ProductExclusionLists");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite ProductExclusionLists Test Failed");

                return null;
            }
        }

        public static IEnumerable<CodeDescription> StockUnits(string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite StockUnits Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetStockUnits(universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite StockUnits Test Complete: Saw {results.Count()} StockUnits");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite StockUnits Test Failed");

                return null;
            }
        }

        public static IEnumerable<CodeDescription> OrderCategories(string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite OrderCategories Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetOrderCategories(universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite OrderCategories Test Complete: Saw {results.Count()} OrderCategories");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite OrderCategories Test Failed");

                return null;
            }
        }

        public static IEnumerable<CodeDescription> StockistCategories(string universeAccount = "MERTEX")
        {
            try
            {
                TesterFramework.SetConsoleColourTesting();

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine("FrameworkTestSuite StockistCategories Test...");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                var results = MertexerV2.Utilities.Framework.GetStockistCategories(universeAccount);

                Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));

                Console.WriteLine(TesterFramework.WavyLineBreak);

                Console.WriteLine($"FrameworkTestSuite StockistCategories Test Complete: Saw {results.Count()} StockistCategories");

                Console.WriteLine(TesterFramework.WavyLineBreak);

                return results;
            }
            catch (Exception ex)
            {
                TesterFramework.SetConsoleColourError();

                Console.WriteLine($"\n{ex.ToString().Replace(System.Environment.NewLine, $"{System.Environment.NewLine}")}");

                Console.WriteLine("\nFrameworkTestSuite StockistCategories Test Failed");

                return null;
            }
        }
    }
}
