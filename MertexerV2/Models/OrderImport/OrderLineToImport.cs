using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.OrderImport
{
    public class OrderLineToImport
    {
        public string ProductCode { get; set; }
        public decimal Quantity { get; set; }
        public string Type { get; set; }
        public string LineReference { get; set; }
        public string Memo { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public string PromoCode { get; set; }
        public int FileLineNumber { get; set; }

        public OrderLineToImport (
            int fileLineNumber,
            string productCode,
            decimal quantity,
            string type,
            string lineReference,
            string memo,
            string promoCode = null,
            decimal? discountPercentage = null) 
        {
            FileLineNumber = fileLineNumber;
            ProductCode = productCode;
            Quantity = quantity;
            Type = type;
            LineReference = lineReference;
            Memo = memo;
            DiscountPercentage = discountPercentage;
            PromoCode = promoCode;
        }
    }
}