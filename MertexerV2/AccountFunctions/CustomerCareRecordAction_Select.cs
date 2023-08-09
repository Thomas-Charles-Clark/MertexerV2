using MertexerV2.GeneralFunctions;
using MertexerV2.Models.CustomerCare;
using MertexerV2.Models.ExchangeRate;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.AccountFunctions
{
    public class CCRActionKey
    {
        public string Key { get; set; }
    }

    public static partial class CustomerCareRecordFunctions
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "CustomerCareRecordSelectAction_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable SelectAction(string UniVerseAccount, string CCRActionKeys)
        {
            if (String.IsNullOrEmpty(CCRActionKeys))
            {
                foreach (var q in Selector<CustomerCareRecordAction>.SelectAllNonParallel(
                   true,
                   false,
                   50,
                   UniVerseAccount))
                {
                    yield return q;
                }
            }
            else
            {
                var results = Selector<CustomerCareRecordAction>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<CCRActionKey>>(CCRActionKeys).Select(p => p.Key),
                    true,
                    false,
                    50,
                    UniVerseAccount)
                    .ToList();

                foreach (var r in results)
                {
                    yield return r;
                }
            }
        }

        public static void CustomerCareRecordSelectAction_FillRow(
            object obj,
            out SqlString Key,
            out SqlString ActionedBy,
            out SqlDateTime ActionedDT,
            out SqlString AddedText,
            out SqlString AssignedToPerson,
            out SqlString AssignedToDepartment,
            out SqlString ContactName
            )
        {
            var p = (CustomerCareRecordAction)obj;

            Key = p.Key?.TrimReduceTruncate(20);
            ActionedBy = p.ActionedBy?.TrimReduceTruncate(10);
            ActionedDT = p.ActionedDT ?? SqlDateTime.Null;
            AddedText = p.AddedText.Any() ? JsonConvert.SerializeObject(p.AddedText) : SqlString.Null;
            AssignedToPerson = p.AssignedToPerson?.TrimReduceTruncate(50);
            AssignedToDepartment = p.AssignedToDepartment?.TrimReduceTruncate(50);
            ContactName = p.ContactName?.TrimReduceTruncate(100);
        }
    }
}
