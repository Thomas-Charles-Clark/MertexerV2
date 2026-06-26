using MertexerV2.GeneralFunctions;
using MertexerV2.InvoiceFunctions;
using MertexerV2.Models;
using MertexerV2.Models.Invoices;
using MertexerV2.Models.Pricing;
using MertexerV2.Utilities;

using Microsoft.SqlServer.Server;
using Newtonsoft.Json;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace MertexerV2.OrderHeadFunctions
{
	public class OrderIDWrapper
	{
		public string OrderID { get; set; }
	}


	public static partial class OrderHeadFunctions
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "OrderSelect_FillRow")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Select(string UniVerseAccount, string OrderIDs)
		{
			// If we have nothing to filter on
			if (String.IsNullOrEmpty(OrderIDs))
			{
				// Return everything
				foreach (var q in Selector<OrderHead>.SelectAllNonParallel(
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
				// If we have specific invoice numbers
				if (!String.IsNullOrEmpty(OrderIDs))
				{
					// Return those
					var results = Selector<OrderHead>.Select(
						JsonConvert.DeserializeObject<IEnumerable<OrderIDWrapper>>(OrderIDs).Select(p => p.OrderID),
						true,
						true,
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

        public static void OrderSelect_FillRow(
            object obj,
            out SqlString OrderID,
            out SqlString DeliveryTerms,
            out SqlString InvoiceMemoLines
            )
        {
            var result = (OrderHead)obj;

            OrderID = result.Key;

            DeliveryTerms = result.DeliveryTerms;

            InvoiceMemoLines = result.InvoiceMemoLines?.Any() == true ? JsonConvert.SerializeObject(result.InvoiceMemoLines) : SqlString.Null;
        }
    }
}
