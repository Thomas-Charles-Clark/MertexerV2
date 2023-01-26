using MertexerV2.Models.DataSource;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.SQLFunctions
{
    public class Dedupe
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "CheckOrder_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable CheckOrder(string TradeDataSourceJSON, string OrderReference)
        {
            var results = new List<string>();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(TradeDataSourceJSON);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.CheckOrders)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                var r = new List<string>();

                using (var ce = new MertexerV2.Utilities.CommandExecutor(dataSource.UniVerseAccount))
                {
                    r = ce.Arbitrary($"LIST ID DATEONFILE TOEHEAD WITH CUSREF = \"{dataSource.CustomerReference}\" AND EVAL\"OCONV(CUSORDNO.DES, 'MCL')\" = \"{OrderReference?.ToLowerInvariant()}\" BY.DSND ID ID.SUP COL.HDR.SUPP COUNT.SUP")
                        .Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }

                r.Remove("OCONV ( CUSORDNO.DES , MCL )  ");

                var enUS = new CultureInfo("en-US");

                for (var i = 0; i < r.Count; i++)
                {
                    var t = r[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (t.Length == 2)
                    {
                        if (DateTime.TryParseExact(t[1], "MM/dd/yy", enUS, DateTimeStyles.None, out DateTime orderDate))
                        {
                            t[1] = orderDate.ToString("dd/MM/yy");
                        }
                    }
                    r[i] = String.Join(" - placed on ", t);
                };

                lock(locker)
                {
                    results.AddRange(r);
                }
            });

            return new ArrayList(results);
        }

        public static void CheckOrder_FillRow(
            object obj,
            out SqlString Record
            )
        {
            var checkResult = (string)obj;

            Record = checkResult;
        }

        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "CheckLine_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable CheckLine(string TradeDataSourceJSON, string ProductCode, decimal Quantity, string LineReference, byte MonthsBack)
        {
            var results = new List<string>();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(TradeDataSourceJSON);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.CheckOrders)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                using (var ce = new Utilities.CommandExecutor(dataSource.UniVerseAccount))
                {
                    var date = Utilities.Framework.UniVerseDateString(DateTime.Now.AddMonths(-MonthsBack));

                    //string cmd = $"LIST TOELINE WITH CUSREF = \"{dataSource.CustomerReference}\" AND PRODCODE = \"{ProductCode}\" AND EVAL\"OCONV(CUS.ORD.REF, 'MCL')\" = \"{LineReference?.Trim()?.ToLowerInvariant()}\" AND QTYORD.STK = {Quantity:N2} AND CUSORDDATE GT \"{DateTime.Now.AddMonths(-MonthsBack):dd/MM/yy}\" AND (QTYCAN.ORD = \"\" OR QTYCAN.ORD = 0) COL.HDR.SUPP COUNT.SUP";
                    string cmd = $"LIST TOELINE @ID FMT\"40L\" WITH CUSREF = \"{dataSource.CustomerReference}\" AND PRODCODE = \"{ProductCode}\" AND SS.CUS.ORD.REF.MCU = \"{LineReference?.Trim()?.ToUpperInvariant()}\" AND QTYORD.STK = {Quantity:N2} AND (QTYCAN.ORD = \"\" OR QTYCAN.ORD = 0) AND DATEONFILE.INT >= {date} ID.SUPP COL.HDR.SUPP COUNT.SUP";

                    var arrayResult = ce.Arbitrary(cmd)
                        .Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();

                    arrayResult.RemoveAll(p => p.StartsWith("OCONV"));// Remove("OCONV ( SS.CUS.ORD.REF.MCU , MCU )  ");

                    lock (locker)
                    {
                        results.AddRange(arrayResult);
                    }
                }
            });

            return new ArrayList(results);
        }

        public static void CheckLine_FillRow(
            object obj,
            out SqlString Record
            )
        {
            var checkResult = (string)obj;

            Record = checkResult;
        }
    }
}
