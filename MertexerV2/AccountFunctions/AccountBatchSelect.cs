using MertexerV2.Models.Accounts;
using MertexerV2.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MertexerV2.AccountFunctions
{
    public static partial class AccountFunctions
    {
        //public static IEnumerable<TradeAccount> BatchSelectTradeAccounts(string uniVerseAccount, string tradeAccountListJson)
        //{
        //    var results = new List<TradeAccount>();

        //    var allCodes = JsonConvert.DeserializeObject<List<TradeAccountCode>>(tradeAccountListJson);

        //    var tradeAccountCodeBatches = ListExtension.Split(allCodes, 100);

        //    var locker = new Object();

        //    Parallel.ForEach(tradeAccountCodeBatches, (tradeAccountCodes) =>
        //    {
        //        var theseTradeAccounts = new List<TradeAccount>();

        //        using (var ce = new CommandExecutor(uniVerseAccount))
        //        {
        //            var slcustFile = ce.Session.CreateUniFile("SLCUST");

        //            var records = slcustFile.ReadRecords(tradeAccountCodes.Select(p => p.Code).ToArray());

        //            foreach (var ta in tradeAccountCodes)
        //            {
        //                var rec = records.GetRecord(ta.Code);

        //                var tradeAccount = new TradeAccount()
        //                {
        //                    CUSREF = ta.Code
        //                };

        //                typeof(TradeAccount).GetProperties().ToList().ForEach(pi =>
        //                {
        //                    var indices = pi.GetCustomAttributesData().Where(p => p.AttributeType == typeof(UniVerseIndexAttribute))
        //                                                    .SingleOrDefault()?
        //                                                    .ConstructorArguments;
        //                    if (indices != null)
        //                    {
        //                        var fieldIndex = (int)indices[0].Value;

        //                        var valueIndex = (int)indices[1].Value;

        //                        var subValueIndex = (int)indices[2].Value;

        //                        var value = rec.Record.Extract(fieldIndex, valueIndex, subValueIndex);

        //                        pi.SetValue(tradeAccount, String.IsNullOrEmpty(value.StringValue) ? null : value.StringValue);
        //                    }
        //                });

        //                theseTradeAccounts.Add(tradeAccount);
        //            }
        //        }

        //        var sslPostContactRecords = new List<SSLPostDocumentRead>();

        //        // fetch SSLPOST key + contact.email
        //        using (var qe = new QueryExecutor(uniVerseAccount))
        //        {
        //            var xDoc = XDocument.Parse(qe.Run(
        //                "SSLPOST.CONTACTS",
        //                $"CUSREF = {(String.Join(" ", tradeAccountCodes.Select(p => p.Code)))}",
        //                "DOCTYPE CONTACT.EMAIL CUSREF",
        //                "",
        //                "XML",
        //                "",
        //                false,
        //                60));

        //            xDoc.Descendants("record").ToList().ForEach(c =>
        //            {
        //                var docType = c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value;
        //                var email = c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value;
        //                var cusRef = c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value;

        //                sslPostContactRecords.Add(new SSLPostDocumentRead()
        //                {
        //                    W_SSLPOST_DOCTYPE = docType,
        //                    W_SSLPOST_EMAIL = email,
        //                    CUSREF = cusRef
        //                });
        //            });
        //        }

        //        theseTradeAccounts.ForEach(c => 
        //        {
        //            c.SSLPOSTJSON = sslPostContactRecords.Where(p => p.CUSREF == c.CUSREF).Select(p => new SSLPostDocument() 
        //            {
        //                W_SSLPOST_DOCTYPE = p.W_SSLPOST_DOCTYPE,
        //                W_SSLPOST_EMAIL = p.W_SSLPOST_EMAIL
        //            }).ToList();
        //        });

        //        lock (locker)
        //        {
        //            results.AddRange(theseTradeAccounts);
        //        }
        //    });

        //    return results;
        //}
    }
}
