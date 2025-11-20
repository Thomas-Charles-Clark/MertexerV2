using MertexerV2.Models;
using MertexerV2.Models.General;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MertexerV2.TradeAccountFunctions
{
    public static partial class TradeAccountPartPaymentPlanFunctions
	{
        [SqlFunction]
        public static string Upsert(string UniVerseAccount, string TradeAccountPartPaymentPlanJSON)
        {
            var obj = JsonConvert.DeserializeObject<TradeAccountPartPaymentPlanCode>(TradeAccountPartPaymentPlanJSON);

            var fileName = typeof(TradeAccountPartPaymentPlan).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                try
                {
                    var extantRecord = uniVerseFile.ReadField(obj.Code, 0).StringValue;
                }
                catch (IBMU2.UODOTNET.UniFileException)
                {
                    // extant record was not found: this is a creation request
                    // create first (lock the entire file for as little time as possible)
                    // then lock the specific record
                    // then write all the fields
                    // then unlock the record
                    var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                    dataSet.Insert(obj.Code);

                    //slcustFile.LockFile();
                    uniVerseFile.WriteRecords(dataSet);
                    //slcustFile.UnlockFile();
                }

                uniVerseFile.LockRecord(obj.Code, 1);

                UniUpsert<TradeAccountPartPaymentPlan>.Upsert(TradeAccountPartPaymentPlanJSON, uniVerseFile, UniVerseAccount);

                uniVerseFile.UnlockRecord(obj.Code);

            }

            return obj.Code;
        }
    }
}
