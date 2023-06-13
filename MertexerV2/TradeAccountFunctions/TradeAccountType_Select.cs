using MertexerV2.GeneralFunctions;
using MertexerV2.Models;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace MertexerV2.TradeAccountFunctions
{
    public static partial class TradeAccountTypeFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "TradeAccountTypeSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string TradeAccountTypeCodes)
        {
            if (String.IsNullOrEmpty(TradeAccountTypeCodes))
            {
                foreach (var r in Selector<TradeAccountType>.SelectAllNonParallel(
                    true,
                    false,
                    50,
                    UniVerseAccount))
                {
                    yield return r;
                }
            }
            else
            {
                var results = Selector<TradeAccountType>.Select(
                    JsonConvert.DeserializeObject<IEnumerable<TradeAccountTypeCode>>(TradeAccountTypeCodes).Select(p => p.Code),
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

        public static void TradeAccountTypeSelect_FillRow(
            object obj,
            out SqlString Code,
            out SqlString Name)
        {
            var result = (TradeAccountType)obj;

            Code = result.Key.TrimReduceTruncate(10);
            Name = result.Name.TrimReduceTruncate(50);
        }



        /*
         
            -- =============================================
            -- Author:		Thomas Clark
            -- Create date: 21/04/2023
            -- Description: Selects Trade Accounts
            -- =============================================
            SET ANSI_NULLS ON
            GO

            SET QUOTED_IDENTIFIER ON
            GO
            ALTER FUNCTION clr.fnc_Mertex_TestClass_Select
            (

            )
            RETURNS TABLE
            (
            [TestValue] NVARCHAR(2)
            )
            AS
            EXTERNAL NAME [MertexerV2].[MertexerV2.TradeAccountFunctions.TradeAccountTypeFunctions].[SelectTest]
            GO
         
         */

        //[Microsoft.SqlServer.Server.SqlFunction(
        //    DataAccess = DataAccessKind.Read,
        //    FillRowMethodName = "TestSelect_FillRow")]
        //[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        //public static IEnumerable SelectTest()
        //{
        //    var list = new List<TestClass>() { new TestClass("ABCDEFGHIJ") };

        //    return new ArrayList(list);
        //}

        //public static void TestSelect_FillRow(
        //    object obj,
        //    out SqlString TestValue)
        //{
        //    var result = (TestClass)obj;

        //    TestValue = result.TestValue.TrimReduceTruncate(10); // throws  Trying to convert return value or output parameter of size 20 bytes to a T-SQL type with a smaller size limit of 4 bytes.
        //}
    }
    public class TestClass
    {
        public string TestValue { get; set; }

        public TestClass(string tv)
        {
            TestValue = tv;
        }
    }
}
