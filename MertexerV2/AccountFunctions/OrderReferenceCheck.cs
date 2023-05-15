using MertexerV2.Models;
using MertexerV2.Models.DataSource;
using MertexerV2.Utilities;
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

namespace MertexerV2.AccountFunctions
{
    public class DuplicateChecks
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "OrderReferenceUniqueCheck_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable OrderReference(
            string TradeDataSourceJson,
            string OrderReference,
            byte MonthsBack = 3)
        {
            var results = new List<string>();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(TradeDataSourceJson);

            var locker = new object();

            foreach(var dataSource in dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.CheckOrders)
                                        .Single()
                                        .DataSources)
            {
                var r = new List<string>();

                using (var ce = new CommandExecutor(dataSource.UniVerseAccount))
                {
                    var command = $"LIST TOEHEAD @ID DATEONFILE WITH CUSREF = \"{dataSource.CustomerReference}\" AND DATEONFILE.INT >= {Framework.UniVerseDateString(DateTime.Now.AddMonths(-MonthsBack))} AND SS.CUSORDNO.MCL = \"[{OrderReference?.Trim()?.ToLowerInvariant()}]\" BY.DSND ID ID.SUP COL.HDR.SUPP COUNT.SUP";

                    var preResult = ce.Arbitrary(command);

                    r = preResult
                        .Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }

                r.RemoveAll(p => p.StartsWith("OCONV"));

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

                results.AddRange(r);
            };

            return results;
        }

        public static void OrderReferenceUniqueCheck_FillRow(
            object obj,
            out SqlString OrderID
            )
        {
            var p = (string)obj;

            OrderID = p?.TrimReduceTruncate(50);
        }
    }
}
