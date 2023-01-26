using MertexerV2.Models.Credit;
using MertexerV2.Models.DataSource;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MertexerV2.SQLFunctions
{
    public class Credit
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "Check_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Check(string TradeDataSourceJSON)
        {
            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(TradeDataSourceJSON);

            var locker = new object();

            CreditCheckResult result = null;// var result = new CreditCheckResult();

            var preResults = new List<CreditCheckResult>();

            // Need to check for    EnquiryType.CreditControls
            //                      EnquiryType.CreditStatus

            var creditControls = dataSources.EnquiryTypes
                                            .Where(p => p.EnquiryType == EnquiryType.CreditControls)
                                            .Single()
                                            .DataSources
                                            .Single();

            using (var qe = new QueryExecutor(creditControls.UniVerseAccount))
            {
                var xDoc = XDocument.Parse(
                        qe.Run(
                            "SLCUST",
                            $"CUSREF=\"{creditControls.CustomerReference}\"",
                            "CRSTOP CRSTAT RETAIL.TILL.CUST CRLIMIT",
                            "",
                            "XML"
                        )
                    );

                var c = xDoc.Descendants("record").Single();

                result = new CreditCheckResult(
                    c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value,
                    c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value,
                    c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value,
                    c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value);
            }

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.CreditStatus)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
                {
                    var xDoc = XDocument.Parse(
                        qe.Run(
                            "SLCUST",
                            $"CUSREF=\"{dataSource.CustomerReference}\"",
                            "BALANCE BALANCE.CUR ORDOH ORDOH.CUR RES.ORDOH.DES RES.ORDOH.CUR.DES",
                            "",
                            "XML"
                        )
                    );

                    var r = new List<CreditCheckResult>();

                    xDoc.Descendants("record").ToList().ForEach(c =>
                    {
                        r.Add(new CreditCheckResult(
                            c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value, // Balance
                            c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value, // Balance (Currency)
                            c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value, // Outstanding
                            c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value, // Outstanding (Currency)
                            c.XPathSelectElement("descendant::fd[@id='C5']").Attribute("value").Value, // Reserves
                            c.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value) // Reserves (Currency)
                        );
                    });

                    lock (locker)
                    {
                        preResults.AddRange(r);
                    }
                }
            });

            result.Balance = preResults.Select(p => p.Balance).Sum();
            result.BalanceCurrency = preResults.Select(p => p.BalanceCurrency).Sum();
            result.OutstandingBalance = preResults.Select(p => p.OutstandingBalance).Sum();
            result.OutstandingBalanceCurrency = preResults.Select(p => p.OutstandingBalanceCurrency).Sum();
            result.OutstandingReserves = preResults.Select(p => p.OutstandingReserves).Sum();
            result.OutstandingReservesCurrency = preResults.Select(p => p.OutstandingReservesCurrency).Sum();

            return new ArrayList(new List<CreditCheckResult>() { result });
        }

        public static void Check_FillRow(
            object obj,
            out SqlString CreditStop,
            out SqlString CreditStatus,
            out SqlString RetailTillCust,
            out SqlDecimal CreditLimit,
            out SqlDecimal Balance,
            out SqlDecimal BalanceCurrency,
            out SqlDecimal OutstandingBalance,
            out SqlDecimal OutstandingBalanceCurrency,
            out SqlDecimal OutstandingReserves,
            out SqlDecimal OutstandingReservesCurrency
            )
        {
            var checkResult = (CreditCheckResult)obj;

            CreditStop = checkResult.CreditStop;
            CreditStatus = checkResult.CreditStatus;
            RetailTillCust = checkResult.RetailTillCust;
            CreditLimit = checkResult.CreditLimit ?? SqlDecimal.Null;
            Balance = checkResult.Balance ?? SqlDecimal.Null;
            BalanceCurrency = checkResult.BalanceCurrency ?? SqlDecimal.Null;
            OutstandingBalance = checkResult.OutstandingBalance ?? SqlDecimal.Null;
            OutstandingBalanceCurrency = checkResult.OutstandingBalanceCurrency ?? SqlDecimal.Null;
            OutstandingReserves = checkResult.OutstandingReserves ?? SqlDecimal.Null;
            OutstandingReservesCurrency = checkResult.OutstandingReservesCurrency ?? SqlDecimal.Null;
        }
    }
}
