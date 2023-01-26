using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.PreAllocation
{
    public class PreAllocationStatus
    {
        public string PreAllocationID { get; set; }
        public string ProductCode { get; set; }
        public string LocationCode { get; set; }
        public decimal Quantity { get; set; }
        public string Batch { get; set; }
        public string Piece { get; set; }
        public DateTime ExpiryDT { get; set; }
        public decimal QuantityOrderedLessStock { get; set; }
        public decimal BatchQuantities { get; set; }
        public string ReserveIndicator { get; set; }
        public bool RemeasureRequired { get; set; }
        public decimal ReserveBuffer { get; set; }

        public PreAllocationStatus(string preAllocationId,
            string productCode,
            string locationCode,
            decimal quantity,
            string batch,
            string piece,
            string expiryDate,
            string expiryTime,
            decimal quantityOrderedLessStock,
            decimal batchQuantities,
            string reserveIndicator,
            bool remeasureRequired,
            decimal reserveBuffer)
        {
            PreAllocationID = preAllocationId;
            ProductCode = productCode;
            LocationCode = locationCode;
            Quantity = quantity;
            Batch = batch;
            Piece = piece;
            ExpiryDT = DateTime.ParseExact($"{expiryDate} {expiryTime}", "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            QuantityOrderedLessStock = quantityOrderedLessStock;
            BatchQuantities = batchQuantities;
            ReserveIndicator = reserveIndicator;
            RemeasureRequired = remeasureRequired;
            ReserveBuffer = reserveBuffer;
        }
    }
}
