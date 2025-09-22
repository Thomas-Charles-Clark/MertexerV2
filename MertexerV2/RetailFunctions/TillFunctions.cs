using MertexerV2.GeneralFunctions;
using MertexerV2.Models;
using MertexerV2.Models.General;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.RetailFunctions
{
    public static class TillFunctions
    {
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "FetchOpenTills_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchOpenTills(string UniVerseAccount)
        {
            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var tillNumbers = ce.Arbitrary($@"LIST OESESSION TILLNO WITH OPEN.FLAG = Y AND SS.OPENED.ON.INT >= {Framework.UniVerseDateString(DateTime.Now)} COUNT.SUP ID.SUPP COL.HDR.SUPP")
                    .Split(Framework.EnvironmentNewLineDelimiter, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                foreach (var till in Selector<RetailTill>.Select(tillNumbers, true, false, 1000, UniVerseAccount))
                {
                    yield return till;
                }
            }
        }
        
        public static void FetchOpenTills_FillRow(
            object obj,
            out SqlString TillNumber,
            out SqlString Name
            )
        {
            var p = (RetailTill)obj;

            TillNumber = p.Key?.TrimReduceTruncate(5);
            Name = p.Description?.TrimReduceTruncate(50);
        }

        
        [SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "FetchRetailTillSessions_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable FetchRetailTillSessions(string UniVerseAccount)
        {
            using (var ce = new CommandExecutor(UniVerseAccount))
            {
                var tillNumbers = ce.Arbitrary($@"LIST OESESSION @ID WITH OPEN.FLAG = Y AND SS.OPENED.ON.INT >= {Framework.UniVerseDateString(DateTime.Now)} COUNT.SUP ID.SUPP COL.HDR.SUPP")
                    .Split(Framework.EnvironmentNewLineDelimiter, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                if (tillNumbers.Any())
                {
                    foreach (var till in Selector<RetailTillSession>.Select(tillNumbers, true, false, 1000, UniVerseAccount))
                    {
                        yield return till;
                    }
                }
            }
        }

        public static void FetchRetailTillSessions_FillRow(
            object obj,
            out SqlString SessionNumber,
            out SqlBoolean Open,
            out SqlString OpenedBy,
            out SqlDateTime OpenedDT,
            out SqlString TillNumber,
            out SqlString Label
            )
        {
            var p = (RetailTillSession)obj;

            SessionNumber = p.Key?.TrimReduceTruncate(10);
            Open = p.Open;
            OpenedBy = p.OpenedBy?.TrimReduceTruncate(50);
            OpenedDT = p.OpenedDT ?? SqlDateTime.Null;
            TillNumber = p.TillNumber?.TrimReduceTruncate(50);
            Label = p.Label?.TrimReduceTruncate(50);
        }
    }
}
