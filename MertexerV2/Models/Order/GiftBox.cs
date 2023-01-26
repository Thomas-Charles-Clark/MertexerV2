using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace MertexerV2.Models
{
    public class GiftBox
    {
        public string CustomerRef { get; set; }
        public string ProductCode { get; set; }
        public string GiftBoxID { get; set; }
        public string OrderLineNumber { get; set; }
        public string Message { get; set; }
        public string OrderLinePrice { get; set; }
        public string OrderLineValue { get; set; }
        public string OrderLineVATCode { get; set; }
        public string OrderLineVATValue { get; set; }
        public string RecipientName { get; set; }
        public string GiftBoxedItems { get; set; }
        public string GiftBoxedQuantities { get; set; }
        public string SundryCode { get; set; }
        public string Quantity { get; set; }
        public string LineDiscVal { get; set; }

        public GiftBox(int index, DataRow row)
        {
            CustomerRef = row["CustomerReference"].ToString();
            GiftBoxID = $"{index + 1}";
            ProductCode = row["ProductCode"].ToString();
            OrderLineNumber = row["LineNumber"].ToString();
            Message = row["GiftBoxMessageText"].ToString();
            OrderLinePrice = row["LinePrice"].ToString();
            OrderLineValue = row["LineValue"].ToString();
            OrderLineVATCode = row["LineVATCode"].ToString();
            OrderLineVATValue = row["LineVATValue"].ToString();
            RecipientName = row["RecipientName"].ToString();
            GiftBoxedItems = row["GiftWrappedLineNumbers"].ToString();
            GiftBoxedQuantities = row["GiftWrappedLineQuantity"].ToString();
            SundryCode = row["SundryCode"].ToString();
            Quantity = row["OrderQuantity"].ToString();
        }

        public string WebData()
        {
            String webData = "";
            webData += CustomerRef + "\r\n";
            webData += OrderLineNumber + "\r\n";
            webData += ProductCode + "\r\n";
            webData += Quantity +"\r\n";
            webData += "\r\n";
            webData += "\r\n";
            webData += "\r\n";
            webData += "\r\n";
            webData += GiftBoxedItems + "\r\n";      // Pipe-Separated OrderLineNumbers to include in this gift box
            webData += GiftBoxedQuantities + "\r\n"; // Pipe-Separated Quantities, maps to the above
            webData += "\r\n";
            webData += "\r\n";
            webData += RecipientName + "\r\n";
            webData += "\r\n"; 
            webData += Message + "\r\n";
            webData += "\r\n";
            webData += OrderLinePrice + "\r\n";
            webData += OrderLineValue + "\r\n";
            webData += OrderLineVATCode + "\r\n";
            webData += OrderLineVATValue + "\r\n";
            webData += "\r\n";
            webData += SundryCode + "\r\n";             
            webData += "\r\n";
            webData += LineDiscVal;
            
            return webData;
        }

        public override string ToString()
        {
            string output = "";
            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\t\tGiftBoxID: " + GiftBoxID;
            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\t\tProductCode: " + ProductCode;
            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\t\tQuantity: " + Quantity;
            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\t\tOrderLineNumber: " + OrderLineNumber;
            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\t\tMessage: " + Message;
            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\t\tGiftBoxedLineNumbers: " + GiftBoxedItems;
            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\t\tGiftBoxedQuantities: " + GiftBoxedQuantities;
            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\t\tOrderLinePrice: " + OrderLinePrice;
            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\t\tOrderLineValue: " + OrderLinePrice;
            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\t\tOrderLineVATCode: " + OrderLineVATCode;
            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\t\tOrderLineVATValue: " + OrderLineVATValue;
            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\t\tRecipientName (currently omitted): " + RecipientName; 
            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\t\tSundryCode: " + SundryCode;
            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\t\tLineDiscVal: " + LineDiscVal;
            return output;
        }
    }
}
