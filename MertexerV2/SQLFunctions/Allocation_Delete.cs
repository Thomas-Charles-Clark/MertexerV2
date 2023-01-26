using MertexerV2.Controllers;
using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MertexerV2.SQLFunctions
{
    public partial class PreAllocation
    {
        [Microsoft.SqlServer.Server.SqlFunction(DataAccess = DataAccessKind.Read)]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static int Delete(
            string PreAllocationID
            )
        {
            var result = 1;

            using (var ac = new AllocationController())
            {
                var stringResult = ac.RemovePreAllocation(
                    PreAllocationID
                );

                if (stringResult == "")
                    result = 0;
            }

            return result;
        }
    }
}