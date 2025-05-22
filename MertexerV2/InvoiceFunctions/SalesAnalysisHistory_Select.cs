using MertexerV2.GeneralFunctions;
using MertexerV2.Models.Bank;
using MertexerV2.Models.SalesAnalysis;
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

namespace MertexerV2.InvoiceFunctions
{
    public static partial class SalesAnalysisFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "SalesAnalysisSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable HistorySelect(string UniVerseAccount, string SalesAnalysisIDs, DateTime? FromDT, DateTime? ToDT)
        {
            if (String.IsNullOrEmpty(SalesAnalysisIDs))
            {
                var filters = new List<Tuple<string, string, string>>();

                if (FromDT != null)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DOCDATE.DES", ">=", Framework.UniVerseDateString(FromDT.Value)));
                }

                if (ToDT != null)
                {
                    filters.Add(new Tuple<string, string, string>("SS.DOCDATE.DES", "<=", Framework.UniVerseDateString(ToDT.Value)));
                }

                // non parallel, 2k batch, 52k records, last month, about 6 minutes 9s
                // parallel, 2k batch, 52k records, last month, 

                if (filters.Any())
                {
#if DEBUG
                    var results = Selector<SalesAnalysisHistory>.Filter(
                        filters,
                        true,
                        false,
                        1000,
                        UniVerseAccount);

                    foreach (var r in results)
                    {
                        yield return r;
                    }
#else
                    foreach (var r in Selector<SalesAnalysisHistory>.FilterNonParallel(
                        filters,
                        true,
                        false,
                        1000,
                        UniVerseAccount))
                    {
                        yield return r;
                    }
#endif
                }
                else
                {
#if DEBUG
                    var results = Selector<SalesAnalysisHistory>.SelectAll(
                        true,
                        false,
                        50,
                        UniVerseAccount);

                    foreach (var r in results)
                    {
                        yield return r;
                    }
#else
                    foreach (var r in Selector<SalesAnalysisHistory>.SelectAllNonParallel(
                        true,
                        false,
                        50,
                        UniVerseAccount))
                    {
                        yield return r;
                    }
#endif
                }
            }
            else
            {
                var results = Selector<SalesAnalysisHistory>.Select(
                   JsonConvert.DeserializeObject<IEnumerable<SalesAnalysisCode>>(SalesAnalysisIDs).Select(p => p.SalesAnalysisID),
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
    }
}
