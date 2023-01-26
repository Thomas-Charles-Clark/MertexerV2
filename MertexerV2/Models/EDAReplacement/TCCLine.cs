using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MertexerV2.Utilities;
using Newtonsoft.Json;

namespace MertexerV2.Models.EDAReplacement
{
    public class TCCLine
    {
        public string ClaimID { get; set; }
        public string ClaimLineID { get; set; }
        public string OrderLineID { get; set; }
        public string ProductCode { get; set; }
        public string FaultCode { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal? ClaimQuantity { get; set; }
        public decimal? AcceptedQuantity { get; set; }
        public decimal? ReturnedQuantity { get; set; }
        public decimal? CreditAmount { get; set; }
        public bool ReturnToStock { get; set; }
        public decimal? OrderPriceSterlingEx { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public string Notes { get; set; }
        public DateTime? InvoiceDT { get; set; }

        public TCCLine(
            string claimId,
            string claimLineId,
            string orderLineId,
            string productCode,
            string faultCode,
            string invoiceNumber,
            string invoiceDt,
            string claimQuantity,
            string acceptedQuantity,
            string returnedQuantity,
            string creditAmount,
            string returnToStock,
            string orderPriceSterlingEx,
            string discountPercentage,
            string notes
            )
        {
            ClaimID = String.IsNullOrEmpty(claimId) ? null : claimId;

            ClaimLineID = String.IsNullOrEmpty(claimLineId) ? null : claimLineId;

            OrderLineID = String.IsNullOrEmpty(orderLineId) ? null : orderLineId;

            ProductCode = String.IsNullOrEmpty(productCode) ? null : productCode;

            FaultCode = String.IsNullOrEmpty(faultCode) ? null : faultCode;

            InvoiceNumber = String.IsNullOrEmpty(invoiceNumber) ? null : invoiceNumber;

            if (Int32.TryParse(invoiceDt, out int invdt))
            {
                InvoiceDT = Framework.DateTimeFromUniVerseOffset(invdt);
            }

            if (Decimal.TryParse(claimQuantity, out decimal cq))
                ClaimQuantity = cq;

            if (Decimal.TryParse(acceptedQuantity, out decimal aq))
                AcceptedQuantity = aq;

            if (Decimal.TryParse(returnedQuantity, out decimal rq))
                ReturnedQuantity = rq;

            if (Decimal.TryParse(creditAmount, out decimal ca))
                CreditAmount = ca;

            ReturnToStock = String.IsNullOrEmpty(returnToStock) ? false : returnToStock == "Y" ? true : false;

            if (Decimal.TryParse(orderPriceSterlingEx, out decimal opse))
                OrderPriceSterlingEx = opse;
        
            if (Decimal.TryParse(discountPercentage, out decimal disco))
            {
                if (disco > 0)
                    DiscountPercentage = 1 - (disco * 0.01m);
            }

            Notes = String.IsNullOrEmpty(notes) ? null : notes.Trim();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
