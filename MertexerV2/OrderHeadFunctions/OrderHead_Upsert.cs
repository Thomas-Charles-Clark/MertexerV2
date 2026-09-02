using MertexerV2.Models;
using MertexerV2.Models.Invoices;
using MertexerV2.Models.Pricing;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.OrderHeadFunctions
{
    public static partial class OrderHeadFunctions
	{
		[SqlFunction]
		public static string Upsert(string UniVerseAccount, string OrderHeadsJSON)
		{
			var upsertedCount = 0;

			var orderHeads = JsonConvert.DeserializeObject<IEnumerable<object>>(OrderHeadsJSON);

			var fileName = typeof(OrderHead).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
					.Cast<UniVerseFileNameAttribute>()
					.Single()
					.FileName;

			using (var ce = new CommandExecutor(UniVerseAccount))
			{
				var uniVerseFile = ce.Session.CreateUniFile(fileName);

				foreach (var orderHead in orderHeads)
				{
					// oh carries defaults for all fields except those which were passed in as JSON.
					// DO NOT WRITE THIS TO THE FILE! Otherwise bad people will find you and remove your swim bladder.
					var oh = JsonConvert.DeserializeObject<OrderHead>(JsonConvert.SerializeObject(orderHead));

					try
					{
						var extantRecord = uniVerseFile.ReadField(oh.Key, 0).StringValue;
					}
					catch (IBMU2.UODOTNET.UniFileException e)
					{
						throw e;
					}

					uniVerseFile.LockRecord(oh.Key, 1);

					UniUpsert<OrderHead>.Upsert(JsonConvert.SerializeObject(orderHead), uniVerseFile, UniVerseAccount);

					uniVerseFile.UnlockRecord(oh.Key);

					upsertedCount++;
				}
			}

			return $"Upserted {upsertedCount} record{(upsertedCount > 1 ? "" : "s")}";
		}
	}
}
