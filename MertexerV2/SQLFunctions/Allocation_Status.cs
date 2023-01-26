using MertexerV2.Controllers;
using MertexerV2.Models.PreAllocation;
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
    public partial class PreAllocation
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "Status_FillRow",
            TableDefinition = "PreAllocationID nvarchar(36), ProductCode nvarchar(20), LocationCode nvarchar(20), Quantity decimal(7, 2), Batch nvarchar(20), Piece nvarchar(20), ExpiryDT datetime2(2)")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Status(
            string PreAllocationIDs = null,
            string ProductCodes = null,
            DateTime? FromDT = null,
            DateTime? ToDT = null
            )
        {
            var results = new List<PreAllocationStatus>();

            XDocument xDoc = null;

            using (var ac = new AllocationController())
            {
                if (PreAllocationIDs != null)
                {
                    xDoc = XDocument.Parse(
                       ac.PreAllocationStatusByIDs(
                           PreAllocationIDs
                       )
                   );
                }
                else if (ProductCodes != null)
                {
                    xDoc = XDocument.Parse(
                       ac.PreAllocationStatusByProductCodes(
                           ProductCodes
                       )
                   );
                }
                else if (FromDT.HasValue && ToDT.HasValue)
                {
                    xDoc = XDocument.Parse(
                       ac.PreAllocationStatusByDateRange(
                           FromDT?.ToString("dd/MM/yyyy"),
                           ToDT?.ToString("dd/MM/yyyy")
                       )
                   );
                }

                xDoc.Descendants("record").ToList().ForEach(c =>
                {
                    results.Add(new PreAllocationStatus(
                        c.XPathSelectElement("descendant::fd[@id='C1']").Attribute("value").Value,
                        c.XPathSelectElement("descendant::fd[@id='C2']").Attribute("value").Value,
                        c.XPathSelectElement("descendant::fd[@id='C3']").Attribute("value").Value,
                        Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C4']").Attribute("value").Value),
                        c.XPathSelectElement("descendant::fd[@id='C6']").Attribute("value").Value,
                        c.XPathSelectElement("descendant::fd[@id='C7']").Attribute("value").Value,
                        c.XPathSelectElement("descendant::fd[@id='C8']").Attribute("value").Value,
                        c.XPathSelectElement("descendant::fd[@id='C9']").Attribute("value").Value,
                        Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C10']").Attribute("value").Value),
                        Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C11']").Attribute("value").Value),
                        c.XPathSelectElement("descendant::fd[@id='C12']").Attribute("value").Value,
                        Convert.ToBoolean(Convert.ToInt32(String.IsNullOrEmpty(c.XPathSelectElement("descendant::fd[@id='C13']").Attribute("value").Value) ? "0" : c.XPathSelectElement("descendant::fd[@id='C13']").Attribute("value").Value)),
                        Convert.ToDecimal(c.XPathSelectElement("descendant::fd[@id='C14']").Attribute("value").Value)
                    ));
                });
            }

            return new ArrayList(results);
        }

        public static void Status_FillRow(
            object obj,
            out SqlString PreAllocationID,
            out SqlString ProductCode,
            out SqlString LocationCode,
            out SqlDecimal Quantity,
            out SqlString Batch,
            out SqlString Piece,
            out SqlDateTime ExpiryDT,
            out SqlDecimal QuantityOrderedLessStock,
            out SqlDecimal BatchQuantities,
            out SqlString ReserveIndicator,
            out SqlBoolean RemeasureRequired,
            out SqlDecimal ReserveBuffer
            )
        {
            var preAlloc = (PreAllocationStatus)obj;

            PreAllocationID = preAlloc.PreAllocationID;
            ProductCode = preAlloc.ProductCode;
            LocationCode = preAlloc.LocationCode;
            Quantity = preAlloc.Quantity;
            Batch = preAlloc.Batch;
            Piece = preAlloc.Piece;
            ExpiryDT = preAlloc.ExpiryDT;
            QuantityOrderedLessStock = preAlloc.QuantityOrderedLessStock;
            BatchQuantities = preAlloc.BatchQuantities;
            ReserveIndicator = preAlloc.ReserveIndicator;
            RemeasureRequired = preAlloc.RemeasureRequired;
            ReserveBuffer = preAlloc.ReserveBuffer;
        }
    }
}