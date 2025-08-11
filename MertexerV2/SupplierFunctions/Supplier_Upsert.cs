using IBMU2.UODOTNET;

using MertexerV2.Models.Product;
using MertexerV2.Models.Supplier;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.SupplierFunctions
{
    public static partial class SupplierFunctions
    {
        internal static string UpsertViaTransaction(ref UniSession session, string SupplierJSON)
        {
            var supplier = JsonConvert.DeserializeObject<Supplier>(SupplierJSON);

            var fileName = typeof(Supplier).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            var uniVerseFile = session.CreateUniFile(fileName);

            try
            {
                var extantRecord = uniVerseFile.ReadField(supplier.Key, 0).StringValue;
            }
            catch (UniFileException)
            {
                // extant record was not found: this is a creation request
                // create first (lock the entire file for as little time as possible)
                // then lock the specific record
                // then write all the fields
                // then unlock the record
                var dataSet = new UniDataSet(session);
                dataSet.Insert(supplier.Key);

                //slcustFile.LockFile();
                uniVerseFile.WriteRecords(dataSet);
                //slcustFile.UnlockFile();
            }

            uniVerseFile.LockRecord(supplier.Key, 1);

            UniUpsert<Supplier>.Upsert(SupplierJSON, uniVerseFile);

            return supplier.Key;
        }

        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string SupplierJSON)
        {
            var supplier = JsonConvert.DeserializeObject<Supplier>(SupplierJSON);

            var fileName = typeof(Supplier).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(supplier.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(supplier.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(supplier.Key, 1);

                UniUpsert<Supplier>.Upsert(SupplierJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(supplier.Key);
            }

            return supplier.Key;
        }
    }
}
