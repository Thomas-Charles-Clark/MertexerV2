using MertexerV2.Models;
using MertexerV2.Models.Product;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.ProductFunctions
{
    public static partial class ProductFunctions
    {
		[SqlFunction]
		public static string Upsert(string UniVerseAccount, string ProductsJSON)
		{
			var products = JsonConvert.DeserializeObject<IEnumerable<object>>(ProductsJSON);

			var fileName = typeof(Product).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
					.Cast<UniVerseFileNameAttribute>()
					.Single()
					.FileName;

			using (var ce = new CommandExecutor(UniVerseAccount))
			{
				var uniVerseFile = ce.Session.CreateUniFile(fileName);

				foreach (var product in products)
				{
					// ta carries defaults for all fields except those which were passed in as JSON.
					// DO NOT WRITE THIS TO THE FILE! Otherwise bad people will find you and remove your swim bladder.
					var ta = JsonConvert.DeserializeObject<Product>(JsonConvert.SerializeObject(product));

					try
					{
						var extantRecord = uniVerseFile.ReadField(ta.Key, 0).StringValue;
					}
					catch (IBMU2.UODOTNET.UniFileException)
					{
						// extant record was not found: this is a creation request
						// create first (lock the entire file for as little time as possible)
						// then lock the specific record
						// then write all the fields
						// then unlock the record
						var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);

						dataSet.Insert(ta.Key);

						//slcustFile.LockFile();
						uniVerseFile.WriteRecords(dataSet);
						//slcustFile.UnlockFile();
					}

					uniVerseFile.LockRecord(ta.Key, 1);

					// Here we're taking the object (having, say, only two or three properties) and JSONifying it again
					// and then the Upsert method can use reflection to identify the properties and we'll only be updating
					// those fields which we care to update!
					UniUpsert<Product>.Upsert(JsonConvert.SerializeObject(product), uniVerseFile, UniVerseAccount);

					uniVerseFile.UnlockRecord(ta.Key);

				}

				// need to find some way to call 
				// REGEN.INDEX SLCUST SORTNAME
				// so that the SB+ index gets rebuilt.
				// ^ That got done, yay! See MertexSSH solution.

				//#if DEBUG
				//                Console.WriteLine(indexRebuildResult);
				//#endif
			}

			return $"{products.Count()} Products UPSERTed";
		}
	}
}
