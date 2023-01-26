using MertexerV2.Models.Accounts;
using MertexerV2.Models.DataSource;
using MertexerV2.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MertexerV2.Accounts
{
    public static partial class AccountFunctions
    {
        public static string ViewDocument(string tradeDataSourceJson, string docref)
        {
            var results = new List<string>();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Invoice)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
                {
                    var reply = qe.Run("ACCOUNTS_INVOICE", $"{docref}", "", "", "", "", true, 60);

                    if (!string.IsNullOrEmpty(reply))
                    {
                        lock (locker)
                        {
                            results.Add(reply);
                        }
                    }
                }
            });

            return results.FirstOrDefault();
        }

        public static string ViewProformaInvoice(string tradeDataSourceJson, string docref)
        {
            var results = new List<string>();

            var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(tradeDataSourceJson);

            var locker = new object();

            Parallel.ForEach(dataSources.EnquiryTypes
                                        .Where(p => p.EnquiryType == EnquiryType.Invoice)
                                        .Single()
                                        .DataSources,
            (dataSource) =>
            {
                using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
                {
                    var reply = qe.Run("ACCOUNTS_PROFORMA", $"{docref}", "", "", "", "", true);

                    if (!string.IsNullOrEmpty(reply))
                    {
                        lock (locker)
                        {
                            results.Add(reply);
                        }
                    }
                }
            });

            return results.FirstOrDefault();
        }
    }
}
