using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2SQL.Models.Stock
{
    #region Product Stock Info

    public class ProductStockInfoReturn
    {
        public string Code { get; set; }
        public string TotalStock { get; set; }
        public List<Batch> Batches { get; set; }
        public List<PurchaseOrder> PurchaseOrders { get; set; }
        public string TotalPurchaseOrderLength { get; set; }
    }

    public class BatchPiece
    {
        public string Quantity { get; set; }
        public string Length { get; set; }
    }

    public class Batch
    {
        public string BatchNumber { get; set; }
        public string ItemCount { get; set; }
        public string Length { get; set; }

        public List<BatchPiece> Pieces { get; set; }
    }

    public class PurchaseOrder
    {
        public string PurchaseOrderNumber { get; set; }
        public string DueDate { get; set; }
        public string Length { get; set; }
    }
    #endregion
}
