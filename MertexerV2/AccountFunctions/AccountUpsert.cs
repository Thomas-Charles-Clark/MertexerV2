using MertexerV2.Models;
using MertexerV2.Models.DataSource;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MertexerV2.Accounts
{
    public static partial class AccountFunctions
    {
        //public static string Upsert(string TradeDataSourceJSON, string TradeAccountJSON)
        //{
        //    var dataSources = JsonConvert.DeserializeObject<TradeAccountDataSourceInfo>(TradeDataSourceJSON);

        //    var dataSource = dataSources.EnquiryTypes.Where(p => p.EnquiryType == EnquiryType.Account).Single()
        //        .DataSources.Single();

        //    var tradeAccount = JsonConvert.DeserializeObject<TradeAccount>(TradeAccountJSON);

        //    // cheers!
        //    // https://stackoverflow.com/questions/6637679/reflection-get-attribute-name-and-value-on-property#answer-13076438
        //    // Console.WriteLine(JsonConvert.SerializeObject(new
        //    // {
        //    //     tradeAccount = typeof(TradeAccount).GetProperties().ToList().Select(pi =>
        //    //     {
        //    //         return new
        //    //         {
        //    //             property = pi.Name,
        //    //             value = pi.GetValue(tradeAccount),
        //    //             uniVerseIndex = pi.GetCustomAttributesData().Where(p => p.AttributeType == typeof(UniVerseIndexAttribute))
        //    //                                 .SingleOrDefault()?
        //    //                                 .ConstructorArguments?.First().Value
        //    //         };
        //    //     }).ToList()
        //    // }, Formatting.Indented));

        //    using (var ce = new CommandExecutor(dataSource.UniVerseAccount))
        //    {
        //        var slcustFile = ce.Session.CreateUniFile("SLCUST");

        //        try
        //        {
        //            var extantRecord = slcustFile.ReadField(tradeAccount.CUSREF, 0).StringValue;
        //        }
        //        catch (IBMU2.UODOTNET.UniFileException)
        //        {
        //            // extant record was not found: this is a creation request
        //            // create first (lock the entire file for as little time as possible)
        //            // then lock the specific record
        //            // then write all the fields
        //            // then unlock the record
        //            var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
        //            dataSet.Insert(tradeAccount.CUSREF);

        //            slcustFile.LockFile();
        //            slcustFile.WriteRecords(dataSet);
        //            slcustFile.UnlockFile();
        //        }

        //        slcustFile.LockRecord(tradeAccount.CUSREF, 1);

        //        typeof(TradeAccount).GetProperties().ToList().ForEach(pi =>
        //        {
        //            var indices = pi.GetCustomAttributesData().Where(p => p.AttributeType == typeof(UniVerseIndexAttribute))
        //                                            .SingleOrDefault()?
        //                                            .ConstructorArguments;

        //            // if no UniVerseIndexAttribute was given, the field is not intended to be submitted.
        //            if (indices != null)
        //            {
        //                string value = (string)pi.GetValue(tradeAccount);

        //                // the UniDynArray throws when given null values: we should present with empty strings
        //                // so as to be sure that the field receives some value
        //                // or not, because... that's not working right.
        //                if (value == null)
        //                {
        //                    value = "";
        //                }

        //                var fieldIndex = (int)indices[0].Value;

        //                var valueIndex = (int)indices[1].Value;

        //                var subValueIndex = (int)indices[2].Value;

        //                var fieldArray = slcustFile.ReadField(tradeAccount.CUSREF, fieldIndex);// new IBMU2.UODOTNET.UniDynArray(ce.Session);

        //                if (valueIndex != 0 &&
        //                    subValueIndex != 0)
        //                {
        //                    fieldArray.Replace(1, valueIndex, subValueIndex, value);
        //                }
        //                else if (valueIndex != 0)
        //                {
        //                    fieldArray.Replace(1, valueIndex, value);
        //                }
        //                else
        //                {
        //                    fieldArray.Replace(1, value);
        //                }

        //                slcustFile.WriteField(tradeAccount.CUSREF, fieldIndex, fieldArray);
        //            }
        //        });

        //        slcustFile.UnlockRecord(tradeAccount.CUSREF);

        //        // do the SSLPOST.CONTACTS table if necessary
        //        //
        //        // SOooooOOOOooOOooooo... we have... a list of document types paired with email addresses
        //        // // we want ALL SSLPOST.CONTACTS records belonging with this CUSREF.
        //        // How do we get them.

        //        var sslPostContactsRemovals = new List<string>();

        //        using (var qe = new QueryExecutor(dataSource.UniVerseAccount))
        //        {
        //            var xDoc = XDocument.Parse(qe.Run(
        //                "SSLPOST.CONTACTS",
        //                $"CUSREF = \"{tradeAccount.CUSREF}\"",
        //                "CONTACT.KEY"));

        //            xDoc.Descendants("record").ToList().ForEach(c =>
        //            {
        //                var key = c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value;

        //                if (!tradeAccount.SSLPOSTJSON?.Where(p => $"{p.DocumentType}*{tradeAccount.CUSREF}" == key).Any() == true)
        //                {
        //                    sslPostContactsRemovals.Add(key);
        //                }
        //            });

        //            if (sslPostContactsRemovals.Any())
        //            {
        //                var sslpostContactsFile = ce.Session.CreateUniFile("SSLPOST.CONTACTS");

        //                sslPostContactsRemovals.ForEach(c =>
        //                {
        //                    sslpostContactsFile.DeleteRecord(c);
        //                });
        //            }
        //        }

        //        tradeAccount.SSLPOSTJSON?.ForEach(c => 
        //        {
        //            UpsertSSLPOSTContactRecord(tradeAccount.CUSREF, c, ce.Session);
        //        });
        //    }

        //    return tradeAccount.CUSREF;
        //}

        //internal static void UpsertSSLPOSTContactRecord(string cusRef, SSLPostDocument c, IBMU2.UODOTNET.UniSession session)
        //{
        //    var sslpostContactsFile = session.CreateUniFile("SSLPOST.CONTACTS");

        //    var key = $"{c.DocumentType}*{cusRef}";

        //    try
        //    {
        //        var extantRecord = sslpostContactsFile.ReadField(key, 0).StringValue;
        //    }
        //    catch (IBMU2.UODOTNET.UniFileException)
        //    {
        //        // extant record was not found: this is a creation request
        //        // create first (lock the entire file for as little time as possible)
        //        // then lock the specific record
        //        // then write all the fields
        //        // then unlock the record
        //        var dataSet = new IBMU2.UODOTNET.UniDataSet(session);
        //        dataSet.Insert(key);

        //        sslpostContactsFile.LockFile();
        //        sslpostContactsFile.WriteRecords(dataSet);
        //        sslpostContactsFile.UnlockFile();
        //    }

        //    sslpostContactsFile.LockRecord(key, 1);

        //    var fieldArray = sslpostContactsFile.ReadField(key, 1);// new IBMU2.UODOTNET.UniDynArray(ce.Session);

        //    fieldArray.Replace(1, c.Email);

        //    sslpostContactsFile.WriteField(key, 1, fieldArray);

        //    sslpostContactsFile.UnlockRecord();
        //}
    }
}
