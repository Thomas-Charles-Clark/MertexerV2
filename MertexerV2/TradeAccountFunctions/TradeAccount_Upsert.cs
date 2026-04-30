using MertexerV2.Models;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.TradeAccountFunctions
{
    public static partial class TradeAccountFunctions
    {
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string TradeAccountJSON)
        {
            var tradeAccount = JsonConvert.DeserializeObject<TradeAccount>(TradeAccountJSON);

            var fileName = typeof(TradeAccount).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(tradeAccount.Key, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);

                    dataSet.Insert(tradeAccount.Key);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(tradeAccount.Key, 1);

                UniUpsert<TradeAccount>.Upsert(TradeAccountJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(tradeAccount.Key);

                // need to find some way to call 
                // REGEN.INDEX SLCUST SORTNAME
                // so that the SB+ index gets rebuilt.
                // ^ That got done, yay! See MertexSSH solution.

                //#if DEBUG
                //                Console.WriteLine(indexRebuildResult);
                //#endif
            }

            return tradeAccount.Key;
        }

		[SqlFunction]
		public static string UpsertMultiple(string UniVerseAccount, string TradeAccountsJSON)
		{
			var tradeAccounts = JsonConvert.DeserializeObject<IEnumerable<object>>(TradeAccountsJSON);

			var fileName = typeof(TradeAccount).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
					.Cast<UniVerseFileNameAttribute>()
					.Single()
					.FileName;

			using (var ce = new CommandExecutor(UniVerseAccount))
			{
				var uniVerseFile = ce.Session.CreateUniFile(fileName);

                foreach(var tradeAccount in tradeAccounts)
				{
					// ta carries defaults for all fields except those which were passed in as JSON.
					// DO NOT WRITE THIS TO THE FILE! Otherwise bad people will find you and remove your swim bladder.
					var ta = JsonConvert.DeserializeObject<TradeAccount>(JsonConvert.SerializeObject(tradeAccount));

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
					UniUpsert<TradeAccount>.Upsert(JsonConvert.SerializeObject(tradeAccount), uniVerseFile, UniVerseAccount);

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

			return $"{tradeAccounts.Count()} Trade Accounts UPSERTed";
		}
	}
}
