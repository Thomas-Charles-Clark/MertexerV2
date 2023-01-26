using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Stock
{
    public class BatchPieceResult
    {
        public string Type { get; set; }
        public string ProductCode { get; set; }
        public string LocationCode { get; set; }
        public string Batch { get; set; }
        public string Piece { get; set; }
        public string Rack { get; set; }
        public string RackType { get; set; }
        public decimal Quantity { get; set; }
        public decimal? OnReserve { get; set; }
        public bool? Pickable { get; set; }
        public string PurchaseOrderNumber { get; set; }
        public DateTime? PurchaseOrderDate { get; set; }
        public bool? PurchaseOrderStatus { get; set; }
        public string PurchaseOrderMessage { get; set; }

        public BatchPieceResult(
            string type,
            string productCode,
            string batch,
            string piece,
            bool? pickable,
            string rack,
            string rackType,
            decimal quantity,
            decimal? onReserve,
            string poNumber,
            string poDueDate,
            bool? poStatus,
            string locationCode)
        {
            Type = type;

            ProductCode = productCode;

            Batch = batch;
            Piece = piece;
            Pickable = pickable;
            Rack = rack;
            RackType = rackType;
            Quantity = quantity;
            OnReserve = onReserve;
            PurchaseOrderNumber = poNumber;
            PurchaseOrderStatus = poStatus;

            DateTime poRealDate = DateTime.MinValue;

            if (DateTime.TryParse(poDueDate, out poRealDate))
            {
                PurchaseOrderDate = poRealDate;
            }
            else
            {
                PurchaseOrderMessage = poDueDate;
            }

            LocationCode = locationCode;
        }

        public override string ToString()
        {
            return $@"Query Type: {Type}
Product Code: {ProductCode}
Batch: {Batch}
Piece: {Piece}
Pickable: {Pickable}
Rack: {Rack}
RackType: {RackType}
Quantity: {Quantity}
OnReserve: {OnReserve}
PurchaseOrderNo: {PurchaseOrderNumber}
PurchaseOrderStatus: {PurchaseOrderStatus}";
        }
    }
}
