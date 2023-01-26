using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.PreAllocation
{
    public class PreAllocationCreation
    {
        public string PreAllocationID { get; set; }

        public string Batch { get; set; }

        public string Piece { get; set; }

        public string Rack { get; set; }

        public decimal QuantityAllocated { get; set; }

        public bool RemeasureTriggered { get; set; }

        public decimal ReserveBuffer { get; set; }

        public PreAllocationCreation(
            string preAllocationId,
            string batch,
            string piece,
            string rack,
            decimal qtyAlloc,
            bool remeasure,
            decimal reserveBuffer)
        {
            PreAllocationID = preAllocationId;
            Batch = batch;
            Piece = piece;
            Rack = rack;
            QuantityAllocated = qtyAlloc;
            RemeasureTriggered = remeasure;
            ReserveBuffer = reserveBuffer;
        }
    }
}