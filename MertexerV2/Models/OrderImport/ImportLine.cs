using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;

namespace MertexerV2.Models.OrderImport
{
    public class ImportLine
    {
        public int FileLineNumber { get; set; }
        //-------------------------------- Standard order fields
        public string TradeAccountNumber { get; set; }
        public string OrderReference { get; set; }
        public string DeliveryName { get; set; }
        public string DeliveryAddressLine1 { get; set; }
        public string DeliveryAddressLine2 { get; set; }
        public string DeliveryTown { get; set; }
        public string DeliveryCounty { get; set; }
        public string DeliveryPostcode { get; set; }
        public string DeliveryCountryCode { get; set; }
        public string DeliveryPhoneNumber { get; set; }
        public string Email { get; set; }
        public string ProductCode { get; set; }
        public string Quantity { get; set; }
        public string Type { get; set; }
        public string LineReference { get; set; }
        public string Memo { get; set; }
        public string ShippingAlertEmailAddress { get; set; }
        public string ContactPhone { get; set; }

        //-------------------------------- Distributor only fields beneath
        public string HaulierCode { get; set; }
        public string CarriageCode { get; set; }
        public string HaulierAccountNumber { get; set; }
        public string TaxesDutiesToInvoiceAccount { get; set; }
        public string DeliveryTerms { get; set; }
        public string DeliveryType { get; set; }
        public string EndCustomer { get; set; }

        //  -------------------------------- Internal only fields beneath
        public string HeldForSales { get; set; }
        public string PromoCode { get; set; }
        public string DiscountPercentage { get; set; }
    }

    public sealed class ImportLineMap : ClassMap<ImportLine>
    {
        public ImportLineMap()
        {
            Map(p => p.FileLineNumber).ConvertUsing(row => ((CsvReader)row).Parser.Context.RawRow);
            Map(p => p.TradeAccountNumber).Index(0);
            Map(p => p.OrderReference).Index(1);
            Map(p => p.DeliveryName).Index(2);
            Map(p => p.DeliveryAddressLine1).Index(3);
            Map(p => p.DeliveryAddressLine2).Index(4);
            Map(p => p.DeliveryTown).Index(5);
            Map(p => p.DeliveryCounty).Index(6);
            Map(p => p.DeliveryPostcode).Index(7);
            Map(p => p.DeliveryCountryCode).Index(8);
            Map(p => p.DeliveryPhoneNumber).Index(9);
            Map(p => p.Email).Index(10);
            Map(p => p.ProductCode).Index(11);
            Map(p => p.Quantity).Index(12);
            Map(p => p.Type).Index(13);
            Map(p => p.LineReference).Index(14);
            Map(p => p.Memo).Index(15);
            Map(p => p.ShippingAlertEmailAddress).Index(16);
            Map(p => p.ContactPhone).Index(17);
            Map(p => p.HaulierCode).Index(18);
            Map(p => p.CarriageCode).Index(19);
            Map(p => p.HaulierAccountNumber).Index(20);
            Map(p => p.TaxesDutiesToInvoiceAccount).Index(21);
            Map(p => p.DeliveryTerms).Index(22);
            Map(p => p.DeliveryType).Index(23);
            Map(p => p.EndCustomer).Index(24);
            Map(p => p.HeldForSales).Index(25);
            Map(p => p.PromoCode).Index(26);
            Map(p => p.DiscountPercentage).Index(27);
        }
    }
}
