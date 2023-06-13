using MertexerV2.Models;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.AccountFunctions
{
    public static partial class DeliveryAddressFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string DeliveryAddressJSON)
        {
            var DeliveryAddress = JsonConvert.DeserializeObject<DeliveryAddress>(DeliveryAddressJSON);

            var fileName = typeof(DeliveryAddress).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(DeliveryAddress.Key ?? "", 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record

                    if (String.IsNullOrEmpty(DeliveryAddress.Key))
                    {
                        var keys = ce.Arbitrary($"LIST DELADD @ID FMT\"40L\" WITH CUSREF = \"{DeliveryAddress.TradeAccountCode}\" ID.SUP COUNT.SUP COL.HDR.SUPP")
                            .Split(Framework.EnvironmentNewLineDelimiter, StringSplitOptions.RemoveEmptyEntries)
                            .ToList();

                        if (keys.Count == 0)
                        {
                            keys.Add($"{DeliveryAddress.TradeAccountCode}-0");
                        }

                        var references = keys.Select(p =>
                            {
                                var split = p.Split(new char[] { '-' });
                                if (split.Count() > 1)
                                    return split[1];
                                else
                                    return "0";
                            })
                            .Distinct()
                            .ToList();

                        var NumericReferences = new List<int>();

                        references.ForEach(c =>
                        {
                            if (Int32.TryParse(c, out int referenceNumber))
                            {
                                NumericReferences.Add(referenceNumber);
                            }
                        });

                        var nextReference = NumericReferences.Max() + 1;

                        var newDeliveryReference = $"{DeliveryAddress.TradeAccountCode}-{nextReference}";

                        DeliveryAddress = new DeliveryAddress(newDeliveryReference, DeliveryAddress);

                        DeliveryAddressJSON = JsonConvert.SerializeObject(DeliveryAddress);
                    }

                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);

                    dataSet.Insert(DeliveryAddress.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);



                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(DeliveryAddress.Key, 1);

                UniUpsert<DeliveryAddress>.Upsert(DeliveryAddressJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(DeliveryAddress.Key);

            }

            return DeliveryAddress.Key;
        }
    }
}
