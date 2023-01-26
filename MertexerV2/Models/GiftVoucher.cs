using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace MertexerV2.Models
{
    public class GiftVoucher
    {
        public string CustomerAccountCode { get; set; }
        public string OrderLineNumber { get; set; }
        public string GiftVoucherCode { get; set; }
        public string ProductCode { get; set; }
        public string GiftVoucherMessage { get; set; }
        public string Price { get; set; }
        public string GiftVoucherVATCode { get; set; }
        public string GiftVoucherVATValue { get; set; }
        public string RecipientName { get; set; }
        public string Quantity { get; set; }
        public string NumberOf { get; set; }
        public string GiftWrappedLineNumbers { get; set; }

        public GiftVoucher(DataRow row)
        {
            CustomerAccountCode = row["CustomerAccountCode"].ToString();
            OrderLineNumber = row["OrderLineNumber"].ToString();
            ProductCode = row["ProductCode"].ToString();
            Quantity = row["OrderQuantity"].ToString();
            NumberOf = row["NumberOf"].ToString();
            GiftVoucherCode = row["VoucherCode"].ToString();
            GiftWrappedLineNumbers = row["GiftWrappedLineNumbers"].ToString();
            RecipientName = row["RecipientName"].ToString();
            GiftVoucherMessage = row["GiftVoucherMessage"].ToString();
            Price = row["GiftVoucherPrice"].ToString();
            GiftVoucherVATCode = row["GiftVoucherVATCode"].ToString();
            GiftVoucherVATValue = row["GiftVoucherVATValue"].ToString();
        }
        
        /// <summary>
        /// For field positions reference, please see Models/OrderLine.cs
        /// Both GiftVouchers and GiftBoxes are sent to the same line creation routine, and so carry values in the same places.
        /// </summary>
        /// <returns></returns>
        public String WebData()
        {
            String webData = "";
            webData += CustomerAccountCode + "\r\n";
            webData += OrderLineNumber + "\r\n";
            webData += ProductCode + "\r\n";
            webData += Quantity + "\r\n";
            webData += "\r\n";
            webData += "\r\n";
            webData += "\r\n";
            webData += "\r\n";
            webData += NumberOf + "\r\n";
            webData += GiftVoucherCode + "\r\n";
            webData += "\r\n";
            webData += GiftWrappedLineNumbers + "\r\n";
            webData += HttpUtility.HtmlDecode(RecipientName) + "\r\n";
            webData += "\r\n";
            webData += HttpUtility.HtmlDecode(GiftVoucherMessage) + "\r\n";
            webData += "\r\n"; 
            webData += Price + "\r\n";
            webData += Price + "\r\n";
            webData += GiftVoucherVATCode + "\r\n";
            webData += GiftVoucherVATValue + "\r\n";
            webData += "\r\n";
            webData += "\r\n";

            return webData;
        }

        public override string ToString()
        {
            string output = "";

            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\tOrderLineNumber: " + OrderLineNumber;
            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\t\tCustomerAccountCode: " + CustomerAccountCode;
            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\t\tProductCode: " + ProductCode;
            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\t\tQuantity: " + Quantity;
            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\t\tNumberOf: " + NumberOf;
            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\t\tVoucherCode: " + GiftVoucherCode;
            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\t\tGiftWrappedLineNumbers: " + GiftWrappedLineNumbers;
            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\t\tRecipientName: " + RecipientName;
            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\t\tMessage: " + GiftVoucherMessage;
            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\t\tPrice: " + Price;
            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\t\tOrderLineVATCode: " + GiftVoucherVATCode;
            output += Environment.NewLine + "\t\t\t\t\t\t\t\t\t\tOrderLineVATValue: " + GiftVoucherVATValue;

            return output;
        }
    }
}
