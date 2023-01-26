using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MertexerV2.Models
{
    public class OrderLine
    {
        /// <summary>
        /// 1.
        /// The Mertex Customer Reference (account number).
        /// </summary>
        public string CustomerReference { get; set; }

        /// <summary>
        /// 2.
        /// The Line Number in the Basket
        ///     For new lines set to -1 (we may not want to do this, as we already have line numbers and need consistency with our gift boxing instructions).
        ///         The setting of this to -1 kicks in two things: it brings in the next available line number, and also creates the basket if one did not exist already.
        ///         So probably just keep on creating the basket manually before adding lines.
        ///     For existing lines to amend or remove set to the actual line number in the basket
        /// </summary>
        public string LineNumber { get; set; }

        /// <summary>
        /// 3.
        /// The Mertex Product Code
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 4.
        /// The quantity to be ordered, in OrderUnits (12)
        /// </summary>
        public string QuantityOrdered { get; set; }

        /// <summary>
        /// 5.
        /// "C" for a Cutting Sample
        /// "A" for an Approval/Returnable Sample
        /// </summary>
        public string SampleIndicator { get; set; }

        /// <summary>
        /// 6.
        /// Y – Back Order the Line
        /// </summary>
        public string BackOrderIndicator { get; set; }

        /// <summary>
        /// 7.
        /// Y – Reserve
        /// </summary>
        public string ReserveIndicator { get; set; }

        /// <summary>
        /// 8.
        /// A percentage discount to apply to the order line
        /// </summary>
        public string DiscountPercentage { get; set; }

        /// <summary>
        /// 9.
        /// A pipe delimited list of the number of items (pieces) to be allocated of each Item Length.
        /// In line with Item Lengths and Item Single Batch Indicator
        /// For Gift Boxes populate this with a pipe delimited list of the Order Line Numbers to include in the Gift Box
        /// </summary>
        public string ItemCounts { get; set; }

        /// <summary>
        /// 10.
        /// A pipe delimited list of the length of items (pieces) to be allocated for each Item Count.
        /// In line with Item Counts and Item Single Batch Indicator
        /// For Gift Boxes populate this with a pipe delimited list of the Order Line Quantities to include in the Gift Box
        /// </summary>
        public string ItemLengths { get; set; }

        /// <summary>
        /// 11.
        /// A memo to be applied to the order line
        /// </summary>
        public string LineMemo { get; set; }

        /// <summary>
        /// 12.
        /// The order unit
        /// </summary>
        public string OrderUnit { get; set; }

        /// <summary>
        /// 13.
        /// The Customer's Order Reference for this line
        /// For Gift Boxes this is the Gift Box recipient
        /// </summary>
        public string CustomerOrderReference_GiftBoxRecipient { get; set; }

        /// <summary>
        /// 14.
        /// Not used
        /// </summary>
        public string AutoKey { get; set; }

        /// <summary>
        /// 15.
        /// For reservations, should a cutting be provided also? Ignored if Reserve Indicator is not Y
        /// For Gift Boxes this is the Gift Box Message
        /// </summary>
        public string WithCuttingIndicator_GiftBoxMessage { get; set; }

        /// <summary>
        /// 16.
        /// For reservations, is a specific batch required? Ignored if Reserve Indicator is not Y
        /// </summary>
        public string SpecificBatchIndicator { get; set; }

        /// <summary>
        /// 17.
        /// The Unit Price for the Line. If blank, Mertex calculates the field.
        /// </summary>
        public string LinePrice { get; set; }

        /// <summary>
        /// 18.
        /// The Value for the Line including VAT. If blank Mertex calculates the field.
        /// </summary>
        public string LineValue { get; set; }

        /// <summary>
        /// 19.
        /// The VAT Code to be used for the line.
        /// </summary>
        public string LineVATCode { get; set; }

        /// <summary>
        /// 20.
        /// The amount of VAT to be charged for the line.
        /// </summary>
        public string LineVATValue { get; set; }

        /// <summary>
        /// 21.
        /// Is the order line a Cutting or a Piece?
        /// </summary>
        public string CutPieceIndicator { get; set; }

        /// <summary>
        /// 22.
        /// Is the product a non-stock / sundry product?
        /// For Gift Boxes set this to G, otherwise set to Y when a Non Stock 
        /// (when you're adding a Sundry Code instead of a Product Code), leave blank otherwise.
        /// </summary>
        public string NonStockIndicator_SundryCode { get; set; }

        /// <summary>
        /// 23.
        /// A pipe delimited list of flags denoting if each item (piece) is to be taken from a single batch.
        /// In line with Item Counts and Item Lengths.
        /// Note the default value for each item is Y, if set to N this will appear in a 
        /// line memo when back ordering a line. Note that the routine can only allocate within a single batch
        /// </summary>
        public string ItemSingleBatchIndicator { get; set; }

        /// <summary>
        /// 24.
        /// A discount value to apply to the order line
        /// </summary>
        public string LineDiscountValue { get; set; }

        /// <summary>
        /// 25.
        /// The Promotion code to apply to the order line
        /// </summary>
        public string PromotionCode { get; set; }

        /// <summary>
        /// 26.
        /// Override the default calculated line location with the value provided.
        /// </summary>
        public string LineLocationOverride { get; set; }

        /// <summary>
        /// 27.
        /// Pipe delimited list of references.
        /// </summary>
        public string OrderLineReferences { get; set; }

        /// <summary>
        /// 28.
        /// Pipe delimited, denotes the type of line memo being created, including:
        /// A(All documents)
        /// O(Order Confirmation)
        /// F(Proforma Invoice)
        /// P(Pick / Cut List)
        /// K(Packing Note)
        /// D(Delivery Note)
        /// I(Invoice)
        /// Any other non null value will be treated as A.
        /// 
        /// When working with Line Memos you have the choice to set a Line Memo Type for each line. If you do not set a Line Memo Type then A (All Documents) is assumed.
        /// To apply multiple different types of Line Memo to a single line then populate the properties as follows:
        /// Line 1, Memo Line 1 I
        /// Line 1, Memo Line 2 I
        /// Line 2, Memo Line 1 K
        /// Line 2, Memo Line 2 K
        /// = "I|I|K|K"
        /// The above example will apply two lines of memo to both the Invoice and Packing Note Memos.
        /// </summary>
        public string LineMemoTypes { get; set; }

        /// <summary>
        /// 29.
        /// A pipe delimited list of Pre Allocation IDs to apply to this order basket line
        /// </summary>
        public string PreAllocationIDs { get; set; }

        /// <summary>
        /// 30.
        /// Enables ordering of hanger, showlengths and waterfalls, either empty or one of:
        /// H(Hanger)
        /// S(Showlength)
        /// W(Waterfall)
        /// </summary>
        public string ProcessType { get; set; }

        /// <summary>
        /// 31.
        /// Feeler Product Code, Hanger orders only
        /// </summary>
        public string FeelerCode { get; set; }

        /// <summary>
        /// 32.
        /// Feeler Product Length
        /// </summary>
        public string FeelerLength { get; set; }

        /// <summary>
        /// 33.
        /// Pipe delimited list of answers given to Process questions
        /// </summary>
        public string ProcessAnswers { get; set; }

        /// <summary>
        /// 34.
        /// "Not used, leave blank"
        /// </summary>
        public string NotUsed34 { get; set; }

        /// <summary>
        /// 35.
        /// "Not used, leave blank"
        /// </summary>
        public string NotUsed35 { get; set; }

        /// <summary>
        /// 36.
        /// Forward Order indicator.
        /// Y or blank
        /// </summary>
        public string ForwardOrder { get; set; }

        /// <summary>
        /// 37.
        /// Forward Order
        /// A date in DD/MM/YYYY format
        /// Ignored if Forward Order is not Y
        /// </summary>
        public string ForwardOrderDate { get; set; }

        /// <summary>
        /// 38.
        /// Process Special Details
        /// Pipe delimited string applicable to process details set in fields 30 and 33
        /// </summary>
        public string ProcessSpecialDetails { get; set; }

        public OrderLine(DataRow row)
        {
            CustomerReference = row["CustomerReference"]?.ToString();
            LineNumber = row["LineNumber"]?.ToString();
            ProductCode = row["ProductCode"]?.ToString();
            QuantityOrdered = row["QuantityOrdered"]?.ToString();
            SampleIndicator = row["SampleIndicator"]?.ToString();
            BackOrderIndicator = row["BackOrderIndicator"]?.ToString();
            ReserveIndicator = row["ReserveIndicator"]?.ToString();
            DiscountPercentage = row["DiscountPercentage"]?.ToString();
            ItemCounts = row["ItemCounts"]?.ToString();
            ItemLengths = row["ItemLengths"]?.ToString();
            LineMemo = row["LineMemo"]?.ToString();
            OrderUnit = row["OrderUnit"]?.ToString();
            CustomerOrderReference_GiftBoxRecipient = row["CustomerOrderReference_GiftBoxRecipient"]?.ToString();
            AutoKey = row["AutoKey"]?.ToString();
            WithCuttingIndicator_GiftBoxMessage = row["WithCuttingIndicator_GiftBoxMessage"]?.ToString();
            SpecificBatchIndicator = row["SpecificBatchIndicator"]?.ToString();
            LinePrice = row["LinePrice"]?.ToString();
            LineValue = row["LineValue"]?.ToString();
            LineVATCode = row["LineVATCode"]?.ToString();
            LineVATValue = row["LineVATValue"]?.ToString();
            CutPieceIndicator = row["CutPieceIndicator"]?.ToString();
            NonStockIndicator_SundryCode = row["NonStockIndicator_SundryCode"]?.ToString();
            ItemSingleBatchIndicator = row["ItemSingleBatchIndicator"]?.ToString();
            LineDiscountValue = row["LineDiscountValue"]?.ToString();
            PromotionCode = row["PromotionCode"]?.ToString();
            LineLocationOverride = row["LineLocationOverride"]?.ToString();
            OrderLineReferences = row["OrderLineReferences"]?.ToString();
            LineMemoTypes = row["LineMemoTypes"]?.ToString();
            PreAllocationIDs = row["PreAllocationIDs"]?.ToString();
            ProcessType = row["ProcessType"]?.ToString();
            FeelerCode = row["FeelerCode"]?.ToString();
            FeelerLength = row["FeelerLength"]?.ToString();
            ProcessAnswers = row["ProcessAnswers"]?.ToString();
            NotUsed34 = row["NotUsed34"]?.ToString();
            NotUsed35 = row["NotUsed35"]?.ToString();
            ForwardOrder = row["ForwardOrder"]?.ToString();
            ForwardOrderDate = row["ForwardOrderDate"]?.ToString();
            ProcessSpecialDetails = row["ProcessSpecialDetails"]?.ToString();
        }

        public string WebData() => $@"{CustomerReference}
{LineNumber}
{ProductCode}
{QuantityOrdered}
{SampleIndicator}
{BackOrderIndicator}
{ReserveIndicator}
{DiscountPercentage}
{ItemCounts}
{ItemLengths}
{LineMemo}
{OrderUnit}
{CustomerOrderReference_GiftBoxRecipient}
{AutoKey}
{WithCuttingIndicator_GiftBoxMessage}
{SpecificBatchIndicator}
{LinePrice}
{LineValue}
{LineVATCode}
{LineVATValue}
{CutPieceIndicator}
{NonStockIndicator_SundryCode}
{ItemSingleBatchIndicator}
{LineDiscountValue}
{PromotionCode}
{LineLocationOverride}
{OrderLineReferences}
{LineMemoTypes}
{PreAllocationIDs}
{ProcessType}
{FeelerCode}
{FeelerLength}
{ProcessAnswers}
{NotUsed34}
{NotUsed35}
{ForwardOrder}
{ForwardOrderDate}
{ProcessSpecialDetails}";

        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
