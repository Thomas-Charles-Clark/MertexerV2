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
    public static partial class SSLPostContactFunctions
    {
        [SqlFunction]
        public static int Upsert(string UniVerseAccount, string SSLPostContactsJSON)
        {
            var sslPostContacts = JsonConvert.DeserializeObject<IEnumerable<SSLPostContact>>(SSLPostContactsJSON);

            var fileName = typeof(SSLPostContact).GetCustomAttributes(typeof(UniVerseFileNameAttribute), false)
                    .Cast<UniVerseFileNameAttribute>()
                    .Single()
                    .FileName;

            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var uniVerseFile = ce.Session.CreateUniFile(fileName);

                foreach(var contact in sslPostContacts)
                {
                    try
                    {
                        var extantRecord = uniVerseFile.ReadField(contact.Key, 0).StringValue;
                    }
                    catch (IBMU2.UODOTNET.UniFileException)
                    {
                        // extant record was not found: this is a creation request
                        // create first (lock the entire file for as little time as possible)
                        // then lock the specific record
                        // then write all the fields
                        // then unlock the record
                        var dataSet = new IBMU2.UODOTNET.UniDataSet(ce.Session);
                        dataSet.Insert(contact.Key);

                        //slcustFile.LockFile();
                        uniVerseFile.WriteRecords(dataSet);
                        //slcustFile.UnlockFile();
                    }

                    uniVerseFile.LockRecord(contact.Key, 1);

                    UniUpsert<SSLPostContact>.Upsert(JsonConvert.SerializeObject(contact), uniVerseFile, UniVerseAccount);

                    uniVerseFile.UnlockRecord(contact.Key);
                }
            }

            return 0;
        }
    }
}
