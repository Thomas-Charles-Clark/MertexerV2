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

namespace MertexerV2.SQLFunctions
{
    public partial class PreAllocation
    {
        [Microsoft.SqlServer.Server.SqlFunction(
            DataAccess = DataAccessKind.Read,
            FillRowMethodName = "Update_FillRow",
            TableDefinition = "PreAllocationID NVARCHAR(36), Batch NVARCHAR(20), Piece SqlString(20)")]
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        public static IEnumerable Update(
            string PreAllocationID, 
            string ProductCode,
            string LocationCode,
            decimal Quantity,
            string Unit,
            string Batch,
            string Piece,
            DateTime ExpiryDT,
            string ReserveIndicator,
            string SoftAllocated
            )
        {
            var results = new List<PreAllocationCreation>();

            using (var ac = new AllocationController())
            {
                var xDoc = XDocument.Parse(

                    ac.UpdatePreAllocation(
                        PreAllocationID,
                        ProductCode,
                        LocationCode,
                        Quantity.ToString("N2"),
                        Unit,
                        Batch,
                        Piece,
                        ExpiryDT.ToString("dd/MM/yyyy"),
                        ExpiryDT.ToString("HH:mm:ss"),
                        ReserveIndicator,
                        SoftAllocated
                    )
                );

                results.Add(new PreAllocationCreation(
                    PreAllocationID,
                    xDoc.Root.Element("BATCH").Value,
                    xDoc.Root.Element("PIECE").Value,
                    xDoc.Root.Element("RACK")?.Value ?? null,
                    Convert.ToDecimal(xDoc.Root.Element("QTYALLOC").Value),
                    Convert.ToBoolean(Convert.ToInt32(String.IsNullOrEmpty(xDoc.Root.Element("REMEASURE")?.Value) ? "0" : xDoc.Root.Element("REMEASURE").Value)),
                    Convert.ToDecimal(xDoc.Root.Element("RESBUFFER").Value)));
            }

            return new ArrayList(results);
        }

        public static void Update_FillRow(
            object obj,
            out SqlString PreAllocationID,
            out SqlString Batch,
            out SqlString Piece,
            out SqlString Rack,
            out SqlDecimal QuantityAllocated,
            out SqlBoolean RemeasureTriggered,
            out SqlDecimal ReserveBuffer
            )
        {
            var preAlloc = (PreAllocationCreation)obj;

            PreAllocationID = preAlloc.PreAllocationID;
            Batch = preAlloc.Batch;
            Piece = preAlloc.Piece;
            Rack = preAlloc.Rack ?? SqlString.Null;
            QuantityAllocated = preAlloc.QuantityAllocated;
            RemeasureTriggered = preAlloc.RemeasureTriggered;
            ReserveBuffer = preAlloc.ReserveBuffer;
        }
    }
}