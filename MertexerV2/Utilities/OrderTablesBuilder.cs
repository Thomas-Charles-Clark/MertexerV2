using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Utilities
{
    public static class TableBuilder
    {
        public static DataTable OrderHeaderTable()
        {
            var result = new DataTable
            {
                TableName = "OrderHead"
            };

            //result.Columns.Add("InvoiceTerms");

            //result.Columns.Add("CustomerReference");
            //result.Columns.Add("DeliveryReference");
            //result.Columns.Add("DeliveryAddress");
            //result.Columns.Add("EmailAddress");
            //result.Columns.Add("CustomerOrderReference");

            //result.Columns.Add("HaulierCode");
            //result.Columns.Add("PriorityOrder");
            //result.Columns.Add("GiftWrap");
            //result.Columns.Add("PartDeliveryAllowed");
            //result.Columns.Add("OrderValue");

            //result.Columns.Add("CarriageCode");
            //result.Columns.Add("CarriageVATCode");
            //result.Columns.Add("CarriageValue");
            //result.Columns.Add("CarriageVATValue");
            //result.Columns.Add("OrderNumber");

            //result.Columns.Add("USTaxExemptionCertificate");
            //result.Columns.Add("FromCSVFile");
            //result.Columns.Add("MassSampleEntryBasketIDs");
            //result.Columns.Add("HoldForSales");
            //result.Columns.Add("PromotionCode");

            //result.Columns.Add("ContractOrDisplay");
            //result.Columns.Add("Urgent");
            //result.Columns.Add("BespokeSampleType");
            //result.Columns.Add("CustomerFullDispatch");
            //result.Columns.Add("AutoAllocationPriority");

            //result.Columns.Add("SendExpress");
            //result.Columns.Add("CustomerPaying");
            //result.Columns.Add("HaulierAccountNo");
            //result.Columns.Add("TaxesDutiesToInvoiceAccount");
            //result.Columns.Add("RetailOrder");

            //result.Columns.Add("ContactName");
            //result.Columns.Add("ContactPhone");
            //result.Columns.Add("DeliveryTerms");
            //result.Columns.Add("DeliveryType");
            //result.Columns.Add("OrderTaker");

            //result.Columns.Add("TradeAccount");
            //result.Columns.Add("LoyaltyCard");
            //result.Columns.Add("DistributorCode");
            //result.Columns.Add("TransferGLCode");
            //result.Columns.Add("ApplyMiscellaneousCharge");

            //result.Columns.Add("ChargeCode");
            //result.Columns.Add("MiscellaneousCharge");
            //result.Columns.Add("PreferredContactMethod");
            //result.Columns.Add("ContactEmailFax");
            //result.Columns.Add("FullDispatchOnly");

            //result.Columns.Add("HeldForDelivery");
            //result.Columns.Add("ExpressReason");
            //result.Columns.Add("UPSReturnsLabel");
            //result.Columns.Add("CarriageExempt");
            //result.Columns.Add("HeldForExpress");

            //result.Columns.Add("CreditHeld");
            //result.Columns.Add("FurnitureSystem");
            //result.Columns.Add("EndCustomer");
            //result.Columns.Add("WebHeld");
            //result.Columns.Add("FOCIndicator");

            //result.Columns.Add("FOCBudgetManager");
            //result.Columns.Add("OrderPromotionCode");
            //result.Columns.Add("OrderPromotionDiscount");
            //result.Columns.Add("HeaderReferences");
            //result.Columns.Add("SendOrderConfirmationEmail");

            //result.Columns.Add("MiscellaneousAccountAddress");
            //result.Columns.Add("RaisedAsResultOfClaim");
            //result.Columns.Add("EAD");
            //result.Columns.Add("InvoiceClauseCodes");
            //result.Columns.Add("InvoiceClauseText");

            //result.Columns.Add("InvoiceClauseComments");
            //result.Columns.Add("TillNumber");
            //result.Columns.Add("SessionNumber");
            //result.Columns.Add("PaymentType");
            //result.Columns.Add("CreditCardType");

            //result.Columns.Add("PaymentAmount");
            //result.Columns.Add("InvoiceCommentCodes");
            //result.Columns.Add("InvoiceCommentText");
            //result.Columns.Add("PackingNoteCommentCodes");
            //result.Columns.Add("PackingNoteCommentText");

            //result.Columns.Add("DeliveryNoteCommentCodes");
            //result.Columns.Add("DeliveryNoteCommentText");
            //result.Columns.Add("ProformaCommentCodes");
            //result.Columns.Add("ProformaCommentText");
            //result.Columns.Add("ConfirmationCommentCodes");

            //result.Columns.Add("ConfirmationCommentText");
            //result.Columns.Add("PickCutListCommentCodes");
            //result.Columns.Add("PickCutListCommentText");
            //result.Columns.Add("HeldOrderReasonCode");
            //result.Columns.Add("OrderMemo");

            //result.Columns.Add("CallType");
            //result.Columns.Add("OrderType");
            //result.Columns.Add("SampleOrderOnly");
            //result.Columns.Add("UseInvoiceAddress");

            //result.Columns.Add("SalesRepresentativeCode");
            //result.Columns.Add("SalesAgentCode");
            //result.Columns.Add("ChargeOn");
            //result.Columns.Add("BillDutyAccountNumber");
            //result.Columns.Add("HoldForSalesReasonCode");

            return result;
        }

        public static DataTable OrderLineTable()
        {
            var result = new DataTable
            {
                TableName = "OrderLines"
            };

            //result.Columns.Add("CustomerReference");
            //result.Columns.Add("LineNumber");
            //result.Columns.Add("ProductCode");
            //result.Columns.Add("QuantityOrdered");
            //result.Columns.Add("SampleIndicator");
            //result.Columns.Add("BackOrderIndicator");
            //result.Columns.Add("ReserveIndicator");
            //result.Columns.Add("DiscountPercentage");
            //result.Columns.Add("ItemCounts");
            //result.Columns.Add("ItemLengths");
            //result.Columns.Add("LineMemo");
            //result.Columns.Add("OrderUnit");
            //result.Columns.Add("CustomerOrderReference_GiftBoxRecipient");
            //result.Columns.Add("Autokey");
            //result.Columns.Add("WithCuttingIndicator_GiftBoxMessage");
            //result.Columns.Add("SpecificBatchIndicator");
            //result.Columns.Add("LinePrice");
            //result.Columns.Add("LineValue");
            //result.Columns.Add("LineVATCode");
            //result.Columns.Add("LineVATValue");
            //result.Columns.Add("CutPieceIndicator");
            //result.Columns.Add("NonStockIndicator_SundryCode");
            //result.Columns.Add("ItemSingleBatchIndicator");
            //result.Columns.Add("LineDiscountValue");
            //result.Columns.Add("PromotionCode");
            //result.Columns.Add("LineLocationOverride");
            //result.Columns.Add("OrderLineReferences");
            //result.Columns.Add("LineMemoTypes");
            //result.Columns.Add("PreAllocationIDs");
            //result.Columns.Add("ProcessType");
            //result.Columns.Add("FeelerCode");
            //result.Columns.Add("FeelerLength");
            //result.Columns.Add("ProcessAnswers");
            //result.Columns.Add("NotUsed34");
            //result.Columns.Add("NotUsed35");
            //result.Columns.Add("ForwardOrder");
            //result.Columns.Add("ForwardOrderDate");
            //result.Columns.Add("ProcessSpecialDetails");

            return result;
        }

        public static DataTable GiftBoxTable()
        {
            var dt = OrderLineTable();

            dt.TableName = "GiftBoxes";

            return dt;
        }

        public static DataTable DataSourceJsonTable()
        {
            return new DataTable()
            {
                TableName = "DataSourceJson"
            };
        }
    }
}