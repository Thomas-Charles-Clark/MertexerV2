using MertexerV2.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.EDAReplacement
{
    public class OEOANALInvoiceMovement
    {
        public string DespatchHeadID { get; set; }
        public string OrderID { get; set; }
        public string OEOANALKey { get; set; }
        public string DespatchStation { get; set; }
        public DateTime? MovementDT { get; set; }

        public OEOANALInvoiceMovement(
            string despatchHeadId, 
            string orderId, 
            string oeoanalKey, 
            string despatchStation, 
            string movementDate, 
            string movementTime)
        {
            DespatchHeadID = despatchHeadId;
            OrderID = orderId;
            OEOANALKey = oeoanalKey;
            DespatchStation = despatchStation;

            if (Int32.TryParse(movementDate, out int md))
            {
                MovementDT = Framework.DateTimeFromUniVerseOffset(md);

                if (TimeSpan.TryParse(movementTime, out TimeSpan ts))
                {
                    MovementDT += ts;
                }
            }
        }
    }

    public class DespatchHeadIDOrderID
    {
        public string DespatchHeadID { get; set; }
        public string OrderID { get; set; }
    }

    public class DespatchHeadIDOrderIDOEOANALKey : DespatchHeadIDOrderID
    {
        public string OEOANALKey { get; set; }
    }

    public class Parcel
    {
        public string ParcelNumber { get; set; }
        public string ParcelID { get; set; }
        public string HaulierCode { get; set; }
        public string TrackingURL { get; set; }

        public decimal? NetWeight { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public decimal? Depth { get; set; }

        // This is bullshit 03/03/2022
        public string PclNo { get; set; }

        public List<ParcelContent> ParcelContents { get; set; }

        public Parcel(
            string parcelNumber,
            string parcelId,
            string haulierCode,
            string trackingUrl)
        {
            ParcelNumber = parcelNumber;

            ParcelID = parcelId;

            HaulierCode = haulierCode;

            TrackingURL = trackingUrl;

            ParcelContents = new List<ParcelContent>();
        }

        public Parcel (
            string parcelNumber,
            string parcelId,
            string haulierCode,
            string weight,
            string width,
            string height,
            string depth,
            string pclNo)
        {
            ParcelNumber = parcelNumber;

            ParcelID = parcelId;

            HaulierCode = haulierCode;

            if (Decimal.TryParse(weight, out decimal netWeight))
            {
                NetWeight = netWeight / 100.0M;
            }

            if (Decimal.TryParse(width, out decimal wi))
            {
                Width = wi;
            }

            if (Decimal.TryParse(height, out decimal h))
            {
                Height = h;
            }

            if (Decimal.TryParse(depth, out decimal d))
            {
                Depth = d;
            }

            PclNo = pclNo;

            ParcelContents = new List<ParcelContent>();
        }
    }

    public class ParcelContent
    {
        public string ParcelID { get; set; }
        public string Batch { get; set; }
        public string Piece { get; set; }
        public string ProductCode { get; set; }
        public decimal QuantityDespatched { get; set; }
        public decimal? QuantityInvoiced { get; set; }

        public string OriginalParcelID { get; set; }

        public ParcelContent(
            string pid, 
            string b,
            string p,
            string pc,
            decimal q,
            decimal? iq = null,
            string originalParcelId = null)
        {
            ParcelID = pid;
            Batch = b;
            Piece = p;
            ProductCode = pc;
            QuantityDespatched = q;
            QuantityInvoiced = iq;
            OriginalParcelID = originalParcelId;
        }
    }

    public class OEDNHead
    {
        public string OrderID { get; set; }
        public string DespatchHeadID { get; set; }
        public string Source { get; set; }
        public int? DespatchedDate { get; set; }
        public string TimeOnFile { get; set; }
        public DateTime? DespatchedDT
        {
            get
            {
                if (DespatchedDate != null)
                {
                    var date = Framework.DateTimeFromUniVerseOffset(DespatchedDate.Value);

                    if (!String.IsNullOrEmpty(TimeOnFile))
                    {
                        var ts = new TimeSpan();


                        if (Int32.TryParse(TimeOnFile, out int tof))
                        {
                            ts = TimeSpan.FromSeconds(tof);
                        }

                        date = date.Add(ts);
                    }

                    return date;
                }

                return null;
            }
            set
            {

            }
        }

        public string DeliveryName { get; set; }
        public string DeliveryAddressLine1 { get; set; }
        public string DeliveryAddressLine2 { get; set; }
        public string DeliveryTown { get; set; }
        public string DeliveryCounty { get; set; }
        public string DeliveryPostcode { get; set; }
        public string DeliveryCountry { get; set; }
        public string HaulierCode { get; set; }
        public string CarriageCode { get; set; }
        public decimal? DeliveryChargeSterling { get; set; }
        public decimal? DeliveryChargeCurrency { get; set; }
        public List<Parcel> Parcels { get; set; }

        // This doesn't belong as a ctor param: only HIST recs will have one and it's filled manually see EDAReplacement.cs:2351
        public string DespatchedBy { get; set; }

        public OEDNHead(
            string source,
            string orderId,
            string despatchHeadId,
            string despatchedDate,
            string despatchedTime,
            string deliveryName,
            string deliveryAddressLine1,
            string deliveryAddressLine2,
            string deliveryTown,
            string deliveryCounty,
            string deliveryPostcode,
            string deliveryCountry,
            string carriageCode,
            string haulierCode,
            string deliveryChargeSterling,
            string deliveryChargeCurrency,
            List<Parcel> parcels = null)
        {
            Source = String.IsNullOrEmpty(source) ? null : source;

            OrderID = String.IsNullOrEmpty(orderId) ? null : orderId;

            DespatchHeadID = String.IsNullOrEmpty(despatchHeadId) ? null : despatchHeadId;


            if (Int32.TryParse(despatchedDate, out int dd))
            {
                DespatchedDate = dd;
            }

            TimeOnFile = String.IsNullOrEmpty(despatchedTime) ? null : despatchedTime;

            DeliveryName = String.IsNullOrEmpty(deliveryName) ? null : deliveryName;

            DeliveryAddressLine1 = String.IsNullOrEmpty(deliveryAddressLine1) ? null : deliveryAddressLine1;

            DeliveryAddressLine2 = String.IsNullOrEmpty(deliveryAddressLine2) ? null : deliveryAddressLine2;

            DeliveryTown = String.IsNullOrEmpty(deliveryTown) ? null : deliveryTown;

            DeliveryCounty = String.IsNullOrEmpty(deliveryCounty) ? null : deliveryCounty;

            DeliveryPostcode = String.IsNullOrEmpty(deliveryPostcode) ? null : deliveryPostcode;

            DeliveryCountry = String.IsNullOrEmpty(deliveryCountry) ? null : deliveryCountry;

            HaulierCode = String.IsNullOrEmpty(haulierCode) ? null : haulierCode;

            CarriageCode = String.IsNullOrEmpty(carriageCode) ? null : carriageCode;

            if (Decimal.TryParse(deliveryChargeSterling, out decimal dcs))
            {
                DeliveryChargeSterling = dcs / 100;
            }

            if (Decimal.TryParse(deliveryChargeCurrency, out decimal dcc))
            {
                DeliveryChargeCurrency = dcc / 100;
            }

            Parcels = parcels;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
