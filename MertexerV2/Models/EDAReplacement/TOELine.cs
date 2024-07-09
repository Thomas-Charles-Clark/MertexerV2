using MertexerV2.SQLFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.EDAReplacement
{
    public class ToelineBatchesPieces
    {
        public string OrderLineID { get; set; }
        public string ProductCode { get; set; }
        public string BatchesString { get; set; }
        public string PiecesString { get; set; }

        public List<string> Batches
        {
            get
            {
                return BatchesString?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            set { }
        }

        public List<string> Pieces
        {
            get
            {
                return PiecesString?.Split(Framework.ValueMarkDelimiter, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            set {}
        }

        public ToelineBatchesPieces (
            string orderLineId, 
            string productCode,
            string batches,
            string pieces)
        {
            OrderLineID = orderLineId;
            ProductCode = productCode;
            BatchesString = batches;
            PiecesString = pieces;
        } 
    }

    public class TOELine
    {
        public string Key { get; set; }
        public string OrderID { get; set; }
        public short LineNumber { get; set; }
        public string CustomerLineReference { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public decimal QuantityOrdered { get; set; }
        public decimal? QuantityCancelled { get; set; }
        public bool BackOrderIndicator { get; set; }
        public byte? LineStatusID { get; set; }
        public string DetailedLineStatus { get; set; }
        public int? LineStatusDate { get; set; }
        public string LineStatusTime { get; set; }
        public DateTime? LineStatusDT
        {
            get
            {
                if (LineStatusDate != null)
                {
                    var date = Framework.DateTimeFromUniVerseOffset(LineStatusDate.Value);

                    if (!String.IsNullOrEmpty(LineStatusTime))
                    {
                        date = date.Add(TimeSpan.Parse(LineStatusTime));
                    }

                    return date;
                }

                return null;
            }
            set { }
        }

        public int? EstimatedDespatchDate { get; set; }
        public string EstimatedDespatchTime { get; set; }

        public DateTime? EstimatedDespatchDT
        {
            get
            {
                if (EstimatedDespatchDate != null)
                {
                    var date = Framework.DateTimeFromUniVerseOffset(EstimatedDespatchDate.Value);

                    if (!String.IsNullOrEmpty(EstimatedDespatchTime))
                    {
                        date = date.Add(TimeSpan.Parse(EstimatedDespatchTime));
                    }

                    return date;
                }

                return null;
            }
            set { }
        }

        public string DespatchNoteNumber { get; set; }

        public string DespatchNoteLineKey { get; set; }


        public decimal? DiscountPercentage { get; set; }

        public decimal? UnitValueSterling { get; set; }

        public decimal? LineValueSterling { get; set; }

        public decimal? LineDiscountValueSterling { get; set; }

        public decimal? LineVATValueSterling { get; set; }

        public decimal? UnitValueCurrency { get; set; }

        public decimal? LineValueCurrency { get; set; }

        public decimal? LineDiscountValueCurrency { get; set; }

        public decimal? LineVATValueCurrency { get; set; }

        public string VATCode { get; set; }

        public decimal? VATRate { get; set; }

        public string StockLocationCode { get; set; }

        public string PurchaseOrderLineID { get; set; }

        public string PartnerProductCode { get; set; }

        public string ReserveIndicator { get; set; }

        public int? CurrentlySuggestedDeliveryDate { get; set; }

        public DateTime? CurrentlySuggestedDeliveryDT
        {
          get
          {
            if (CurrentlySuggestedDeliveryDate != null)
            {
              var date = Framework.DateTimeFromUniVerseOffset(CurrentlySuggestedDeliveryDate.Value);

              return date;
            }

            return null;
          }
          set { }
        }

        public bool AllocateAtPicking { get; set; }

        public string PickedBy { get; set; }

        public IEnumerable<string> Batches { get; set; }

        public IEnumerable<string> Pieces { get; set; }
        
        public int? ReleasedDate { get; set; }

        public DateTime? ReleasedDT
        {
            get
            {
                if (ReleasedDate != null)
                {
                    var date = Framework.DateTimeFromUniVerseOffset(ReleasedDate.Value);

                    return date;
                }

                return null;
            }
            set { }
        }

        public string ReleasedBy { get; set; }

        public IEnumerable<decimal?> AllocatedQuantities { get; set; }

        public string PickingHeadID { get; set; }

        public string PickingLineID { get; set; }

        public TOELine(
            string key,
            string orderId,
            string lineNumber,
            string customerLineReference,
            string productCode,
            string productDescription,
            string quantityOrdered,
            string quantityCancelled,
            string backOrderIndicator,
            string lineStatusId,
            string detailedLineStatus,
            string lineStatusDate,
            string lineStatusTime,
            string deliveryNoteNumber,
            string deliveryNoteLineKey,
            string discountPercentage,
            string unitValueSterling,
            string lineValueSterling,
            string lineDiscountValueSterling,
            string lineVATValueSterling,
            string unitValueCurrency,
            string lineValueCurrency,
            string lineDiscountValueCurrency,
            string lineVATValueCurrency,
            string vatCode,
            string vatRate,
            string estimatedDespatchDate,
            string stockLocation,
            string purchaseOrderLineId,
            string partnerProductCode,
            string reserveIndicator, 
            string currentSuggestedDeliveryDate,
            string allocateAtPicking,
            string pickedBy,
            string batches,
            string pieces,
            string releasedDate,
            string releasedBy,
            string allocatedQuantities,
            string pickingHeadId,
            string pickingLineId)
        {
            Key = key;

            OrderID = String.IsNullOrEmpty(orderId) ? null : orderId;

            if (Int16.TryParse(lineNumber, out short ln))
            {
                LineNumber = ln;
            }

            CustomerLineReference = String.IsNullOrEmpty(customerLineReference) ? null : customerLineReference.Replace(Framework.ValueMark, '|'); ;

            ProductCode = String.IsNullOrEmpty(productCode) ? null : productCode;

            ProductDescription = String.IsNullOrEmpty(productDescription) ? null : productDescription;

            if (Decimal.TryParse(quantityOrdered, out decimal qtyOrd))
            {
                QuantityOrdered = qtyOrd;
            }

            if (Decimal.TryParse(quantityCancelled, out decimal qtyCan))
            {
                QuantityCancelled = qtyCan;
            }

            BackOrderIndicator = backOrderIndicator == "Y" ? true : false;

            if (Byte.TryParse(lineStatusId, out byte lineStatus))
            {
                LineStatusID = lineStatus;
            }

            DetailedLineStatus = String.IsNullOrEmpty(detailedLineStatus) ? null : detailedLineStatus;

            if (Int32.TryParse(lineStatusDate, out int lsd))
            {
                LineStatusDate = lsd;
            }

            LineStatusTime = String.IsNullOrEmpty(lineStatusTime) ? null : lineStatusTime;

            DespatchNoteNumber = String.IsNullOrEmpty(deliveryNoteNumber) ? null : deliveryNoteNumber;

            DespatchNoteLineKey = String.IsNullOrEmpty(deliveryNoteLineKey) ? null : deliveryNoteLineKey;

            if (Decimal.TryParse(discountPercentage, out decimal dp))
            {
                DiscountPercentage = dp;
            }

            if (Decimal.TryParse(unitValueSterling, out decimal uvs))
            {
                UnitValueSterling = uvs;
            }

            if (Decimal.TryParse(lineValueSterling, out decimal lvs))
            {
                LineValueSterling = lvs;
            }

            if (Decimal.TryParse(lineDiscountValueSterling, out decimal ldvs))
            {
                LineDiscountValueSterling = ldvs;
            }

            if (Decimal.TryParse(lineVATValueSterling, out decimal lvvs))
            {
                LineVATValueSterling = lvvs;
            }

            if (Decimal.TryParse(unitValueCurrency, out decimal uvc))
            {
                UnitValueCurrency = uvc;
            }

            if (Decimal.TryParse(lineValueCurrency, out decimal lvc))
            {
                LineValueCurrency = lvc;
            }

            if (Decimal.TryParse(lineDiscountValueCurrency, out decimal ldvc))
            {
                LineDiscountValueCurrency = ldvc;
            }

            if (Decimal.TryParse(lineVATValueCurrency, out decimal lvvc))
            {
                LineVATValueCurrency = lvvc;
            }

            VATCode = String.IsNullOrEmpty(vatCode) ? null : vatCode;

            if (Decimal.TryParse(vatRate, out decimal vr))
            {
                VATRate = 1 + (vr / 100.0m);
            }

            if (Int32.TryParse(estimatedDespatchDate, out int edd))
            {
                EstimatedDespatchDate = edd;
            }

            StockLocationCode = String.IsNullOrEmpty(stockLocation) ? null : stockLocation;

            PurchaseOrderLineID = String.IsNullOrEmpty(purchaseOrderLineId) ? null : purchaseOrderLineId;

            PartnerProductCode = String.IsNullOrEmpty(partnerProductCode) ? null : partnerProductCode;

            ReserveIndicator = String.IsNullOrEmpty(reserveIndicator) ? null : reserveIndicator;

            if (Int32.TryParse(currentSuggestedDeliveryDate, out int csdd))
            {
              CurrentlySuggestedDeliveryDate = csdd;
            }

            AllocateAtPicking = allocateAtPicking == "Y" ? true : false;

            PickedBy = String.IsNullOrEmpty(pickedBy) ? null : pickedBy;

            Batches = String.IsNullOrEmpty(batches) ? new string[] { } : batches.Split(Framework.ValueMarkDelimiter);

            Pieces = String.IsNullOrEmpty(pieces) ? new string[] { } : pieces.Split(Framework.ValueMarkDelimiter);

            if (Int32.TryParse(releasedDate, out int rdd))
            {
                ReleasedDate = rdd;
            }

            ReleasedBy = String.IsNullOrEmpty(ReleasedBy) ? null : releasedBy;

            AllocatedQuantities = String.IsNullOrEmpty(allocatedQuantities) ? new decimal?[] { } : allocatedQuantities.Split(Framework.ValueMarkDelimiter).Select(p => Convert(p));

            PickingHeadID = String.IsNullOrEmpty(pickingHeadId) ? null : pickingHeadId;

            PickingLineID = String.IsNullOrEmpty(pickingLineId) ? null : pickingLineId;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static decimal? Convert(string value)
        {
            if (Decimal.TryParse($"{value}", out decimal v))
            {
                return v;
            }

            return null;
        }
    }
}
