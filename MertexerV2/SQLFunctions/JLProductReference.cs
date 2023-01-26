using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MertexerV2.Models.Accounts;
using Microsoft.SqlServer.Server;

namespace MertexerV2.SQLFunctions
{
    public static class JLProductReference
    {
        [Microsoft.SqlServer.Server.SqlFunction(DataAccess = DataAccessKind.Read)]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static string Fetch(string UniVerseAccount, string JohnLewisOrderReference, short LineNumber)
        {
            return EDAReplacement.EDAReplacement.JohnLewisProductReference(UniVerseAccount, JohnLewisOrderReference, LineNumber);
        }
    }
}
