using MertexerV2.Utilities;
using Newtonsoft.Json;

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
        public decimal StockCheckMinimum { get; set; }
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
        public bool? CallOff { get; set; }

        public BatchPieceResult(
            string type,
            string productCode,
            decimal stockCheckMinimum,
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
            string locationCode,
            string poMessage)
        {
            Type = type;

            ProductCode = productCode;

            StockCheckMinimum = stockCheckMinimum;

            Batch = batch;
            Piece = piece;
            Pickable = pickable;
            Rack = rack;
            RackType = rackType;
            Quantity = quantity;
            OnReserve = onReserve;
            PurchaseOrderNumber = poNumber;
            PurchaseOrderStatus = poStatus;
            PurchaseOrderMessage = poMessage;

            CallOff = null;

            if (poDueDate != null)
            {
                if (poDueDate?.StartsWith("CO") == true)
                {
                    CallOff = true;

                    poDueDate = poDueDate.Replace("CO", "").Trim();
                }
                else 
                {
                    CallOff = false;
                }

                DateTime poRealDate = DateTime.MinValue;

                if (DateTime.TryParse(poDueDate, out poRealDate))
                {
                    PurchaseOrderDate = poRealDate;
                }
            }

            LocationCode = locationCode;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Framework.JsonSettingsIndentedIncludeNullsAlphabetical);
        }
    }
}
