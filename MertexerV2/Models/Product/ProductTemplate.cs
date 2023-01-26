using MertexerV2.ProductFunctions;
using MertexerV2.Utilities;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Product
{
    [UniVerseFileName("STPRODTEMPLATE.DES")]
    public class ProductTemplate : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public ProductTemplate(string key) : base(key)
        {

        }

        public ProductTemplate() : base()
        {
            InitialiseFromEmptyConstructor();
        }

        public override void InitialiseFromEmptyConstructor()
        {
            typeof(ProductTemplate).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            ProductTemplateFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// No idea
        /// </summary>
        [UniVerseIndex(1,2)]
        [UniVerseFieldName("FIRST.USER.SCREEN")]
        public string FirstUserScreen { get; set; }

        /// <summary>
        /// Product Type (Largely this just lends formula for various text descriptions)
        /// </summary>
        [UniVerseIndex(191)]
        [UniVerseFieldName("PRODTYPE.DES")]
        public string ProductType { get; set; }

        /// <summary>
        /// Length at which to remeasure (fabric)
        /// </summary>
        [UniVerseIndex(57,59)]
        [UniVerseConversion("MR12")]
        [UniVerseFieldName("QUAL.REMEASURE.LGTH")]
        public string RemeasureLength { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [UniVerseIndex(1,1)]
        [UniVerseFieldName("TEMPLATE.DESC")]
        public string Description { get; set; }

        /// <summary>
        /// Group Code
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("GROUPCODE")]
        public string GroupCode { get; set; }

        /// <summary>
        /// Item Type
        /// </summary>
        [UniVerseIndex(3)]
        [UniVerseFieldName("ITEMTYPE")]
        public string ItemType { get; set; }

        /// <summary>
        /// Unit of Purchase
        /// </summary>
        [UniVerseIndex(7)]
        [UniVerseFieldName("UNITPURCHASE")]
        public string UnitOfPurchase { get; set; }

        /// <summary>
        /// Unit of Stock
        /// </summary>
        [UniVerseIndex(8)]
        [UniVerseFieldName("UNITSTOCK")]
        public string UnitOfStock { get; set; }

        /// <summary>
        /// VAT Code
        /// </summary>
        [UniVerseIndex(10)]
        [UniVerseFieldName("VATCODE")]
        public string VATCode { get; set; }

        /// <summary>
        /// Is this product stocked in pieces (fabric), or as discrete items?
        /// </summary>
        [UniVerseIndex(13)]
        [UniVerseFieldName("PIECE.LEVEL")]
        public string PieceLevelStock { get; set; }

        /// <summary>
        /// Sales Analysis (A) -- No idea
        /// </summary>
        [UniVerseIndex(21)]
        [UniVerseFieldName("PRODACC.MNT")]
        public string SalesAnalysisA { get; set; }

        /// <summary>
        /// Multiple Batches? -- No idea -- Y/N
        /// </summary>
        [UniVerseIndex(28)]
        [UniVerseFieldName("BATCH.PROD")]
        public string MultipleBatches { get; set; }

        /// <summary>
        /// Sales Analysis (D)
        /// </summary>
        [UniVerseIndex(202)]
        [UniVerseFieldName("PRODALYS")]
        public string SalesAnalysisD { get; set; }

        /// <summary>
        /// Stock Analysis (D)
        /// </summary>
        [UniVerseIndex(203)]
        [UniVerseFieldName("STOCKALYS")]
        public string StockAnalysisD { get; set; }

        /// <summary>
        /// Allocate at Batch Level? Y/N
        /// </summary>
        [UniVerseIndex(218)]
        [UniVerseFieldName("ALLOC.AT.BATCH")]
        public string AllocateAtBatchLevel { get; set; }

        /// <summary>
        /// Valuation cost to use
        /// </summary>
        [UniVerseIndex(260)]
        [UniVerseFieldName("VALREPFLD")]
        public string ValuationCostToUse { get; set; }

        /// <summary>
        /// Form Code
        /// </summary>
        [UniVerseIndex(271)]
        [UniVerseFieldName("FORM.CODE")]
        public string FormCode { get; set; }

        /// <summary>
        /// Maximum Remnant Length
        /// </summary>
        [UniVerseIndex(287)]
        [UniVerseFieldName("MAXREMLENGTH")]
        public string MaximumRemnantLength { get; set; }

        /// <summary>
        /// Current Status
        /// </summary>
        [UniVerseIndex(288)]
        [UniVerseFieldName("CUR.STAT")]
        public string CurrentStatus { get; set; }

        /// <summary>
        /// Piece Allocation
        /// </summary>
        [UniVerseIndex(300)]
        [UniVerseFieldName("PIECE.ALLOC")]
        public string PieceAllocation { get; set; }

        /// <summary>
        /// Repetitive Batches? "Is it the same batch number"? Repeating batch means "same batch number again and again", or:
        /// this stock doesn't actually exist in batches, the concept of batching does not apply.
        /// 
        /// Fabric, Wallpaper and some other related products are generally manufactured in batches. These are considered to be
        /// Non-repetitive batched products, as the batch number changes with each batch manufactured.
        /// </summary>
        [UniVerseIndex(302)]
        [UniVerseFieldName("REPETATIVE.BATCHES")]
        public string RepetitiveBatches { get; set; }

        /// <summary>
        /// Sample Product
        /// </summary>
        [UniVerseIndex(328)]
        [UniVerseFieldName("SAMPLE.PROD")]
        public string SampleProduct { get; set; }

        /// <summary>
        /// Print Pieces...? 
        /// Y, N, R, S, P or B
        /// </summary>
        [UniVerseIndex(341)]
        [UniVerseFieldName("PRINTPIECES")]
        public string PieceBatchOnPickList { get; set; }

        /// <summary>
        /// Piece Only Orders?
        /// </summary>
        [UniVerseIndex(462)]
        [UniVerseFieldName("PIECE.ONLY.ORD")]
        public string PieceOnlyOrders { get; set; }

        /// <summary>
        /// Product Class 2
        /// </summary>
        [UniVerseIndex(564)]
        [UniVerseFieldName("PRODCLASS2")]
        public string ProductClass2 { get; set; }

        /// <summary>
        /// Cutting Length
        /// </summary>
        [UniVerseIndex(610)]
        [UniVerseConversion("MR23")]
        [UniVerseFieldName("CUTTING.LEN.DES")]
        public string CuttingLength { get; set; }

        /// <summary>
        /// Minimum Replenishment
        /// </summary>
        [UniVerseIndex(612)]
        [UniVerseFieldName("PICK.MIN.STOCK.TDW")]
        public string MinimumReplenishment { get; set; }

        /// <summary>
        /// Bulk Rack
        /// </summary>
        [UniVerseIndex(614)]
        [UniVerseFieldName("BULK.RACK")]
        public string BulkRack { get; set; }

        /// <summary>
        /// Auto Allocate Multiple Times
        /// </summary>
        [UniVerseIndex(653)]
        [UniVerseFieldName("AUTO.ALLOC.MULT")]
        public string AutoAllocateMultipleTimes { get; set; }

        /// <summary> 
        /// Order Type 
        /// </summary>
        [UniVerseIndex(682)]
        [UniVerseFieldName("ORDER.TYPE.DES")]
        public string OrderType { get; set; }

        /// <summary>
        /// Label Logo
        /// </summary>
        [UniVerseIndex(707)]
        [UniVerseFieldName("LOGO.DES")]
        public string Logo { get; set; }

        /// <summary>
        /// Full Order Only
        /// </summary>
        [UniVerseIndex(735)]
        [UniVerseFieldName("SO.ALLOC.FULL.DES")]
        public string FullOrderOnly { get; set; }

        /// <summary>
        /// Charge On
        /// </summary>
        [UniVerseIndex(737)]
        [UniVerseFieldName("CHARGE.ON.DES")]
        public string ChargeOn { get; set; }

        /// <summary>
        /// Brand
        /// </summary>
        [UniVerseIndex(752)]
        [UniVerseFieldName("SALES.DIV.DES")]
        public string Brand { get; set; }

        /// <summary>
        /// Pick List Group
        /// </summary>
        [UniVerseIndex(756)]
        [UniVerseFieldName("PICK.LIST.GRP.DES")]
        public string PickingListGroupCode { get; set; }

        /// <summary>
        /// Bespoke Rug
        /// </summary>
        [UniVerseIndex(811)]
        [UniVerseFieldName("BESPOKE.RUG.DES")]
        public string BespokeRug { get; set; }

        /// <summary>
        /// Part Number Range Start
        /// </summary>
        [UniVerseIndex(1005)]
        [UniVerseFieldName("RANGE.FROM")]
        public string PartNumberRangeStart { get; set; }

        /// <summary>
        /// Part Number Range End
        /// </summary>
        [UniVerseIndex(1006)]
        [UniVerseFieldName("RANGE.TO")]
        public string PartNumberRangeEnd { get; set; }

        /// <summary>
        /// Range Name
        /// </summary>
        [UniVerseIndex(1007)]
        [UniVerseFieldName("RANGE.NAME")]
        public string RangeName { get; set; }

        /// <summary>
        /// Last Range Used
        /// </summary>
        [UniVerseIndex(1008)]
        [UniVerseFieldName("LAST.RANGE")]
        public string LastRange { get; set; }

        /// <summary>
        /// Generate EANs?
        /// </summary>
        [UniVerseIndex(1009)]
        [UniVerseFieldName("GEN.BARCODE")]
        public string CreateEANCodes { get; set; }
    }
}
