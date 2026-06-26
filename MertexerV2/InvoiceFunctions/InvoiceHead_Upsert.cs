using MertexerV2.Models;
using MertexerV2.Models.Invoices;
using MertexerV2.Models.Product;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.InvoiceFunctions
{
    public static partial class InvoiceHeadFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string InvoiceHeadJSON)
        {
            var upsertedCount = 0;

            var invoiceHeads = JsonConvert.DeserializeObject<IEnumerable<object>>(InvoiceHeadJSON);

            var fileName = typeof(InvoiceHead).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                foreach(var invoiceHead in invoiceHeads)
				{
					// ih carries defaults for all fields except those which were passed in as JSON.
					// DO NOT WRITE THIS TO THE FILE! Otherwise bad people will find you and remove your swim bladder.
					var ih = JsonConvert.DeserializeObject<InvoiceHead>(JsonConvert.SerializeObject(invoiceHead));

					try
					{
						var extantRecord = uniVerseFile.ReadField(ih.Key, 0).StringValue;
					}
					catch (IBMU2.UODOTNET.UniFileException e)
					{
                        throw e;
					}

					uniVerseFile.LockRecord(ih.Key, 1);

					UniUpsert<InvoiceHead>.Upsert(JsonConvert.SerializeObject(invoiceHead), uniVerseFile, UniVerseAccount);

					uniVerseFile.UnlockRecord(ih.Key);

                    upsertedCount++;
				}
			}
            
            return $"Upserted {upsertedCount} record{(upsertedCount > 1 ? "" : "s")}";
        }
    }
}
