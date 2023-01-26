using MertexerV2.Models.Keys;
using MertexerV2.Utilities;
using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MertexerV2.SQLFunctions
{
    public partial class KeyFetcher
    {
        [SqlFunction(DataAccess = DataAccessKind.Read,
            SystemDataAccess = SystemDataAccessKind.Read,
            FillRowMethodName = "KeyFetcher_FillRow",
            TableDefinition = "")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Fetch(string KeyFieldName, string TableName, string DateFieldName, DateTime? FromDate, DateTime? ToDate)
        {
            var results = new List<MertexFileKey>();

            //XDocument xDoc = null;

            var selectCriteria = $"";

            if (DateFieldName != null && FromDate != null)
            {
                selectCriteria += $"WITH {DateFieldName} GT {FromDate?.ToString("dd/MM/yyyy")}";

                if (ToDate != null)
                {
                    selectCriteria += $" AND {DateFieldName} LT {ToDate?.ToString("dd/MM/yyyy")}";
                }
            }

            using (var ce = new CommandExecutor())
            {
                var keys = ce.Arbitrary($"LIST {KeyFieldName} {TableName} {selectCriteria} ID.SUP COL.HDR.SUPP COUNT.SUP");

                var keySplit = keys.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();

                keySplit.ForEach(c =>
                {
                    results.Add(new MertexFileKey()
                    {
                        ID = c.Trim()
                    });
                });
            }

            return new ArrayList(results.ToList());
        }

        public static void KeyFetcher_FillRow(
            object obj,
            out SqlString ID
            )
        {
            var toehead = (MertexFileKey)obj;

            ID = toehead.ID;
        }
    }
}
