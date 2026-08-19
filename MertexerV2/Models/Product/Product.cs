using MertexerV2.Models.Interfaces;
using MertexerV2.ProductFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
///     Double Check the UniVerseIndex on each of them
/// </summary>
namespace MertexerV2.Models.Product
{
    [UniVerseFileName("STPRODMASTER")]
    public class Product : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public Product(string key) : base(key)
        {
            Initialise();
        }

        public Product() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(Product).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            ProductFunctions.ProductFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Description
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("PRODDESC")]
        public string Description { get; set; }

        /// <summary>
        /// Product Group
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("GROUPCODE")]
        public string ProductGroup { get; set; }

        /// <summary>
        /// Item Type
        /// </summary>
        [UniVerseIndex(3)]
        [UniVerseFieldName("ITEMTYPE")]
        public string ItemType { get; set; }

        /// <summary>
        /// Mertex Search Description
        /// </summary>
        [UniVerseIndex(6)]
        [UniVerseFieldName("SORTNAME")]
        public string MertexSearchDescription { get; set; }

        /// <summary>
        /// Unit of Purchase
        /// </summary>
        [UniVerseIndex(7)]
        [UniVerseFieldName("SS.U.PURCHASE")]
        public string UnitOfPurchase { get; set; }

        /// <summary>
        /// Unit of Stock
        /// </summary>
        [UniVerseIndex(8)]
        [UniVerseFieldName("SS.U.STOCK")]
        public string UnitOfStock { get; set; }

        /// <summary>
        /// VAT Code
        /// </summary>
        [UniVerseIndex(10)]
        [UniVerseFieldName("VATCODE")]
        public string VATCode { get; set; }

        /// <summary>
        /// Design Name
        /// </summary>
        [UniVerseIndex(11)]
        [UniVerseFieldName("DESIGN.DES")]
        public string DesignName { get; set; }

        /// <summary>
        /// Is this item stocked at piece level (fabric normally is)
        /// </summary>
        [UniVerseIndex(13)]
        [UniVerseFieldName("PIECELEVEL")]
        public bool PieceLevelStock { get; set; }

        /// <summary>
        /// Rack
        /// </summary>
        [UniVerseIndex(20)]
        [UniVerseFieldName("RACK")]
        public string Rack { get; set; }

        /// <summary> *new*
        /// Sales Analysis (A)
        /// </summary>
        [UniVerseIndex(21)]
        [UniVerseFieldName("PRODACC.MNT")]
        public string SalesAnaylsisA { get; set; }

        /// <summary>
        /// Minimum Order Quantity
        /// </summary>
        [UniVerseIndex(27)]
        [UniVerseFieldName("ZZMINORD")]
        public decimal? MinimumOrderQuantity { get; set; }

        /// <summary> *new*
        /// Multiple Batches? -- No idea -- Y/N
        /// </summary>
        [UniVerseIndex(28)]
        [UniVerseFieldName("BATCH.PROD")]
        public bool MultipleBatches { get; set; }

        /// <summary>
        /// Last Cost Price
        /// </summary>
        [UniVerseIndex(36)]
        [UniVerseMultiValue]
        [UniVerseConversion("MR44")]
        [UniVerseFieldName("LAST.COST.P")]
        [JsonConverter(typeof(StringToArrayConverter<IEnumerable<decimal?>>))]
        public IEnumerable<decimal?> LastCostPrices { get; set; }

        /// <summary>
        /// Lastest Cost Price
        /// </summary>
        [UniVerseIndex(36,1)]
        [UniVerseConversion("MR44")]
        [UniVerseFieldName("LATEST.COST.PRICE")]
        public decimal? LatestCostPrice { get; set; }

        /// <summary>
        /// Quality
        /// </summary>
        [UniVerseIndex(57)]
        [UniVerseFieldName("QUALITY")]
        public string Quality { get; set; }

        /// <summary>
        /// Quality Composition Code
        /// </summary>
        [UniVerseIndex(57, 2)]
        [UniVerseFieldName("QUAL.COMPCODE")]
        public string Composition { get; set; }

        /// <summary>
        /// Finished Width
        /// </summary>
        [UniVerseIndex(57, 9)]
        [UniVerseFieldName("QUAL.FIN.WIDTH")]
        [UniVerseConversion("MR11")]
        public decimal? FinishedWidth { get; set; }

        /// <summary>
        /// Supplier Code (MV)
        /// </summary>
        [UniVerseIndex(70)]
        [UniVerseMultiValue]
        [UniVerseFieldName("SUPPREF")]
        [JsonConverter(typeof(StringToArrayConverter<IEnumerable<string>>))]
        public IEnumerable<string> SupplierCodes { get; set; }

        /// <summary>
        /// Supplier Code (One)
        /// </summary>
        [UniVerseIndex(70, 1)]
        [UniVerseFieldName("SUPPREF.1")]
        public string SupplierCodePrimary { get; set; }

        /// <summary>
        /// Supplier Part Number
        /// </summary>
        [UniVerseIndex(71, 1)]
        [UniVerseFieldName("SUPPPARTNO.1")]
        public string SupplierPartNumber { get; set; }

        /// <summary>
        /// Override - Manufacturer's Lead time (Work Days)
        /// </summary>
        [UniVerseIndex(72, 1)]
        [UniVerseFieldName("LEADTIME1")]
        public byte? LeadTime { get; set; }

        /// <summary>
        /// Customer Service Notes (MV)
        /// </summary>
        [UniVerseIndex(90)]
        [UniVerseMultiValue]
        [UniVerseFieldName("PRODDATA1")]
        [JsonConverter(typeof(StringToArrayConverter<IEnumerable<string>>))]
        public IEnumerable<string> CustomerServiceNotes { get; set; }

        /// <summary>
        /// Vertical Pattern Repeat (cm)
        /// </summary>
        [UniVerseIndex(94)]
        [UniVerseFieldName("VPATT.REP")]
        public decimal? VerticalPatternRepeat { get; set; }

        /// <summary>
        /// Net Weight (kg)
        /// </summary>
        [UniVerseIndex(115)]
        [UniVerseConversion("MR33")]
        [UniVerseFieldName("NET.WEIGHT")]
        public decimal? NetWeight { get; set; }

        /// <summary>
        /// Horizontal Pattern Repeat (cm)
        /// </summary>
        [UniVerseIndex(117)]
        [UniVerseFieldName("HPATT.REP")]
        public decimal? HorizontalPatternRepeat { get; set; }

        /// <summary>
        /// Intrastat
        /// </summary>
        [UniVerseIndex(126)]
        [UniVerseFieldName("INTRASTAT")]
        public string ExportCommodityCode { get; set; }

        /// <summary>
        /// Country of Origin
        /// </summary>
        [UniVerseIndex(127)]
        [UniVerseFieldName("ORCOUNTRY")]
        public string CountryOfOrigin { get; set; }

        /// <summary>
        /// Supplier Units
        /// </summary>
        [UniVerseIndex(128)]
        [UniVerseFieldName("SUPUNITS")]
        public string SupplierUnits { get; set; }

        /// <summary>
        /// Conversion Factor (?) Ask Allison MacKenzie / Matt
        /// refer to STCONV 
        /// </summary>
        [UniVerseIndex(129)]
        [UniVerseFieldName("SU.CONV")]
        [UniVerseConversion("MR44")]
        public string ConversionFactor { get; set; }

        /// <summary>
        /// Aftercare Codes (MV)
        /// </summary>
        [UniVerseIndex(178)]
        [UniVerseMultiValue]
        [UniVerseFieldName("AFTCARECODES")]
        [JsonConverter(typeof(StringToArrayConverter<IEnumerable<string>>))]
        public IEnumerable<string> AftercareCodes { get; set; }

        /// <summary> *new*
        /// Product Type (Largely this just lends formula for various text descriptions)
        /// </summary>
        [UniVerseIndex(191)]
        [UniVerseFieldName("PRODTYPE.DES")]
        public string ProductTypeTextFormula { get; set; }

        /// <summary>
        /// Other Information - Sample Label
        /// </summary>
        [UniVerseIndex(193)]
        [UniVerseFieldName("WEBDESC.DES")]
        public string SampleLabelText { get; set; }

        /// <summary> *new*
        /// Sales Analysis (D)
        /// </summary>
        [UniVerseIndex(202)]
        [UniVerseFieldName("PRODALYS")]
        public string SalesAnalysisD { get; set; }

        /// <summary> *new*
        /// Stock Analysis (D)
        /// </summary>
        [UniVerseIndex(203)]
        [UniVerseFieldName("STOCKALYS")]
        public string StockAnalysisD { get; set; }

        /// <summary>
        /// Full Description (MV)
        /// </summary>
        [UniVerseIndex(216)]
        [UniVerseMultiValue]
        [UniVerseFieldName("FULL.DESC")]
        [JsonConverter(typeof(StringToArrayConverter<IEnumerable<string>>))]
        public IEnumerable<string> FullDescriptions { get; set; }

        /// <summary> *new*
        /// Allocate at Batch Level? Y/N
        /// </summary>
        [UniVerseIndex(218)]
        [UniVerseFieldName("ALLOC.AT.BATCH")]
        public bool AllocateAtBatchLevel { get; set; }

        /// <summary> *new*
        /// Valuation cost to use
        /// </summary>
        [UniVerseIndex(260)]
        [UniVerseFieldName("VALREPFLD")]
        public string ValuationCostToUse { get; set; }

        /// <summary>
        /// Pattern Match Code
        /// </summary>
        [UniVerseIndex(263)]
        [UniVerseFieldName("PATTERN.MATCH")]
        public string PatternMatchCode { get; set; }

        /// <summary>
        /// UK Flame Retardancy
        /// </summary>
        [UniVerseIndex(264)]
        [UniVerseFieldName("UK.FLAME")]
        public string UKFlameRetardancy { get; set; }

        /// <summary>
        /// Contract Flame Retardancy
        /// </summary>
        [UniVerseIndex(265)]
        [UniVerseFieldName("CONTRACT.FLAME")]
        public string ContractFlameRetardancy { get; set; }

        /// <summary> *new*
        /// Form Code 
        /// </summary>
        [UniVerseIndex(271)]
        [UniVerseFieldName("FORM.CODE")]
        public string FormCode { get; set; }

        /// <summary>
        /// Pattern Book
        /// </summary>
        [UniVerseIndex(273, 1)]
        [UniVerseFieldName("PATBOOK.1")]
        public string PatternBook { get; set; }


        public DateTime? LaunchDT { get; set; }


        /// <summary>
        /// Launch Date
        /// </summary>
        [UniVerseIndex(275)]
        [UniVerseFieldName("SS.LAUNCH")]
        public string LaunchDate
        {
            get
            {
                if (LaunchDT.HasValue)
                {
                    return Framework.UniVerseDateString(LaunchDT.Value);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (Int32.TryParse(value, out int valInt))
                    {
                        LaunchDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        LaunchDT = null;
                    }
                }
            }
        }

        /// <summary>
        /// Manufacturer's ID
        /// </summary>
        [UniVerseIndex(277, 1)]
        [UniVerseFieldName("MID.1")]
        public string ManufacturersID { get; set; }

        /// <summary>
        /// Stock Code
        /// </summary>
        [UniVerseIndex(278)]
        [UniVerseFieldName("STOCKCODE")]
        public string StockCode { get; set; }

        /// <summary>
        /// Upholstery Grade
        /// </summary>
        [UniVerseIndex(284)]
        [UniVerseFieldName("UPDGRADE")]
        public string UpholsteryGrade { get; set; }

        /// <summary>
        /// Usage Codes (MV)
        /// </summary>
        [UniVerseIndex(285)]
        [UniVerseMultiValue]
        [UniVerseFieldName("USECODE")]
        [JsonConverter(typeof(StringToArrayConverter<IEnumerable<string>>))]
        public IEnumerable<string> UsageCodes { get; set; }

        /// <summary> *new*
        /// Maximum Remnant Length
        /// </summary>
        [UniVerseIndex(287)]
        [UniVerseConversion("MR33")]
        [UniVerseFieldName("MAXREMLENGTH")]
        public decimal? MaximumRemnantLength { get; set; }

        /// <summary>
        /// Product Status
        /// </summary>
        [UniVerseIndex(288)]
        [UniVerseFieldName("CUR.STAT")]
        public string Status { get; set; }

        /// <summary> *new*
        /// Piece Allocation
        /// </summary>
        [UniVerseIndex(300)]
        [UniVerseFieldName("PIECE.ALLOC")]
        public bool PieceAllocation { get; set; }

        /// <summary> *new*
        /// Repetitive Batches? "Is it the same batch number"? Repeating batch means "same batch number again and again", or:
        /// this stock doesn't actually exist in batches, the concept of batching does not apply.
        /// 
        /// Fabric, Wallpaper and some other related products are generally manufactured in batches. These are considered to be
        /// Non-repetitive batched products, as the batch number changes with each batch manufactured.
        /// </summary>
        [UniVerseIndex(302)]
        [UniVerseFieldName("REPETATIVE.BATCHES")]
        public bool RepetitiveBatches { get; set; }

        /// <summary> *new*
        /// Sample Product
        /// </summary>
        [UniVerseIndex(328)]
        [UniVerseFieldName("SAMPLE.PROD")]
        public bool SampleProduct { get; set; }

        /// <summary>
        /// Drop Pattern Repeat
        /// </summary>
        [UniVerseIndex(333)]
        [UniVerseFieldName("DROPPATREP")]
        [UniVerseConversion("MR11")]
        public decimal? DropPatternRepeat { get; set; }

        /// <summary> *new*
        /// Print Pieces...? 
        /// Y, N, R, S, P or B
        /// </summary>
        [UniVerseIndex(341)]
        [UniVerseFieldName("PRINTPIECES")]
        public string PieceBatchOnPickList { get; set; }

        /// <summary>
        /// Paint - Volatile Organic Compounds
        /// </summary>
        [UniVerseIndex(346)]
        [UniVerseFieldName("PAINT.VOC")]
        public string PaintVOC { get; set; }

        /// <summary>
        /// EAN Code
        /// </summary>
        [UniVerseIndex(383)]
        [UniVerseFieldName("EAN.PRODCODE")]
        public string EAN { get; set; }

        /// <summary>
        /// Import Commodity Code
        /// </summary>
        [UniVerseIndex(399)]
        [UniVerseFieldName("COMMODITY.DG")]
        public string ImportCommodityCode { get; set; }

        /// <summary>
        /// US Commodity Code
        /// </summary>
        [UniVerseIndex(403)]
        [UniVerseFieldName("USCOMM")]
        public string USCommodityCode { get; set; }

        /// <summary>
        /// FlamCode
        /// </summary>
        [UniVerseIndex(429)]
        [UniVerseFieldName("ADDITIONAL")]
        public string FlamCode { get; set; }

        /// <summary> *new*
        /// Piece Only Orders?
        /// </summary>
        [UniVerseIndex(462)]
        [UniVerseFieldName("PIECE.ONLY.ORD")]
        public bool PieceOnlyOrders { get; set; }

        /// <summary>
        /// Linked Documents (MV)
        /// </summary>
        [UniVerseIndex(520)]
        [UniVerseMultiValue]
        [UniVerseFieldName("LINK.DOCS")]
        [JsonConverter(typeof(StringToArrayConverter<IEnumerable<string>>))]
        public IEnumerable<string> LinkedDocuments { get; set; }

        /// <summary>
        /// Linked Documents' Descriptions (MV)
        /// </summary>
        [UniVerseIndex(521)]
        [UniVerseMultiValue]
        [UniVerseFieldName("LINK.DOCS.DESC")]
        [JsonConverter(typeof(StringToArrayConverter<IEnumerable<string>>))]
        public IEnumerable<string> LinkedDocumentDescriptions { get; set; }

        /// <summary>
        /// Test Pass Codes (MV)
        /// </summary>
        [UniVerseIndex(532)]
        [UniVerseMultiValue]
        [UniVerseFieldName("MISC1FLD")]
        [JsonConverter(typeof(StringToArrayConverter<IEnumerable<string>>))]
        public IEnumerable<string> TestCodes { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        [UniVerseIndex(533)]
        [UniVerseFieldName("MISC2FLD")]
        public string Type { get; set; }

        /// <summary>
        /// Supplier
        /// </summary>
        [UniVerseIndex(551)]
        [UniVerseFieldName("AUTO.PROC.SUPP")]
        public string Supplier { get; set; }

        /// <summary>
        /// Mylands Product Code
        /// </summary>
        [UniVerseIndex(563)]
        [UniVerseFieldName("PRODCLASS1")]
        public string MylandsProductCode { get; set; }

        /// <summary> *new*
        /// Product Class 2
        /// </summary>
        [UniVerseIndex(564)]
        [UniVerseFieldName("PRODCLASS2")]
        public string ProductClass2 { get; set; }

        /// <summary>
        /// Product Class 3
        /// </summary>
        [UniVerseIndex(565)]
        [UniVerseFieldName("PRODCLASS3")]
        public string ProductClass3 { get; set; }

        /// <summary> *new*
        /// Cutting Length
        /// </summary>
        [UniVerseIndex(610)]
        [UniVerseConversion("MR23")]
        [UniVerseFieldName("CUTTING.LEN.DES")]
        public decimal? CuttingLength { get; set; }

        /// <summary> *new*
        /// Minimum Replenishment
        /// </summary>
        [UniVerseIndex(612)]
        [UniVerseConversion("MR23")]
        [UniVerseFieldName("PICK.MIN.STOCK.TDW")]
        public decimal? MinimumReplenishment { get; set; }

        /// <summary> *new*
        /// Bulk Rack
        /// </summary>
        [UniVerseIndex(614)]
        [UniVerseFieldName("BULK.RACK")]
        public string BulkRack { get; set; }

        /// <summary>
        /// B2B Code (?) 3 = a PO is always required, regardless of stock holding,
        /// Raising a Sales Order for RL Fabric, we do not stock it, it knows this part number is set to always
        /// create a PO for the exact quantity of your order and sends it to the supplier
        /// 
        /// Not many suppliers use this
        /// 
        /// 1 through to 5,
        /// needs writing in better English
        /// 
        /// 1: 
        /// 2: 
        /// 3: 
        /// 4: 
        /// 5: 
        /// 
        /// </summary>
        [UniVerseIndex(618)]
        [UniVerseFieldName("MTNMINSTKLVL.DES")]
        public string B2BCode { get; set; }

        /// <summary> *new*
        /// Auto Allocate Multiple Times
        /// </summary>
        [UniVerseIndex(653)]
        [UniVerseFieldName("AUTO.ALLOC.MULT")]
        public bool AutoAllocateMultipleTimes { get; set; }

        /// <summary>
        /// Flame Retardant
        /// </summary>
        [UniVerseIndex(664)]
        [UniVerseFieldName("FLAME.DES")]
        public string FlameRetardant { get; set; }

        /// <summary>
        /// Clearance Discount
        /// </summary>
        [UniVerseIndex(665)]
        [UniVerseConversion("MR23")]
        [UniVerseFieldName("CLR.DISC.DES")]
        public decimal? ClearanceDiscount { get; set; }

        /// <summary>
        /// Martindale
        /// </summary>
        [UniVerseIndex(666)]
        [UniVerseFieldName("MARTINDALE.DES")]
        public int? Martindale { get; set; }


        /// <summary>
        /// Shipping Lead Time (Working Days)
        /// </summary>
        [UniVerseIndex(672, 1)]
        [UniVerseFieldName("SHIPPING.LEADTIME.1")]
        [UniVerseConversion("MR00")]
        public byte? ShippingLeadTimeWorkingDays { get; set; }

        /// <summary> *new*
        /// Order Type 
        /// </summary>
        [UniVerseIndex(682)]
        [UniVerseFieldName("ORDER.TYPE.DES")]
        public string OrderType { get; set; }

        /// <summary>
        /// Web Product (Y/N) Old B2B manual control. 
        /// </summary>
        [UniVerseIndex(703)]
        [UniVerseFieldName("WEB.PRODUCT")]
        public bool WebProduct { get; set; }

        /// <summary> *new*
        /// Label Logo 
        /// </summary>
        [UniVerseIndex(707)]
        [UniVerseFieldName("LOGO.DES")]
        public string LabelLogo { get; set; }

        /// <summary> *new*
        /// Full Order Only
        /// </summary>
        [UniVerseIndex(735)]
        [UniVerseFieldName("SO.ALLOC.FULL.DES")]
        public bool FullOrderOnly { get; set; }

        /// <summary> *new*
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
        /// Collection
        /// </summary>
        [UniVerseIndex(753)]
        [UniVerseFieldName("PROD.COLL.DES")]
        public string Collection { get; set; }

        /// <summary>
        /// Picking List Group
        /// </summary>
        [UniVerseIndex(756)]
        [UniVerseFieldName("PICK.LIST.GRP.DES")]
        public string PickListGroup { get; set; }

        /// <summary>
        /// Cutter Instructions (MV)
        /// </summary>
        [UniVerseIndex(786)]
        [UniVerseMultiValue]
        [UniVerseFieldName("CUTTERS.MESS.DES")]
        [JsonConverter(typeof(StringToArrayConverter<IEnumerable<string>>))]
        public IEnumerable<string> CutterInstructions { get; set; }

        /// <summary>
        /// Mylands Base Product Code
        /// </summary>
        [UniVerseIndex(800)]
        [UniVerseFieldName("SS.PRODCLASS4")]
        public string MylandsBaseProductCode { get; set; }

        /// <summary>
        /// Product Developer
        /// </summary>
        [UniVerseIndex(801)]
        [UniVerseFieldName("SS.PRODCLASS5")]
        public string ProductDeveloperCode { get; set; }

        /// <summary>
        /// Design Code
        /// </summary>
        [UniVerseIndex(802)]
        [UniVerseFieldName("SS.DESIGN")]
        public string DesignCode { get; set; }

        /// <summary>
        /// Supplier Design Code(s) (MV)
        /// </summary>
        [UniVerseIndex(806)]
        [UniVerseMultiValue]
        [UniVerseFieldName("SUP.DESIGN")]
        [JsonConverter(typeof(StringToArrayConverter<IEnumerable<string>>))]
        public IEnumerable<string> SupplierDesignCodes { get; set; }

        /// <summary>
        /// Supplier Colour Name(s) (MV)
        /// </summary>
        [UniVerseIndex(807)]
        [UniVerseMultiValue]
        [UniVerseFieldName("SUP.COLOUR")]
        [JsonConverter(typeof(StringToArrayConverter<IEnumerable<string>>))]
        public IEnumerable<string> SupplierColourNames { get; set; }

        /// <summary>
        /// Bespoke Rug (Y/N)
        /// </summary>
        [UniVerseIndex(811)]
        [UniVerseFieldName("BESPOKE.RUG.DES")]
        public bool BespokeRug { get; set; }

        /// <summary>
        /// Rug Pile Height (mm)
        /// </summary>
        [UniVerseIndex(832)]
        [UniVerseFieldName("CARE.RPH.DES")]
        public string RugPileHeight { get; set; }

        /// <summary>
        /// Rug Pile Weight (kg)
        /// </summary>
        [UniVerseIndex(833)]
        [UniVerseFieldName("CARE.RPW.DES")]
        public string RugPileWeight { get; set; }

        /// <summary>
        /// Include in Pattern Book Analysis
        /// </summary>
        [UniVerseIndex(848)]
        [UniVerseFieldName("INCL.PB.STOCK.DES")]
        public bool IncludeInPatternBookAnalysis { get; set; }

        /// <summary>
        /// Additional Product (e.g. Cushion Pad)
        /// </summary>
        [UniVerseIndex(864)]
        [UniVerseFieldName("ADD.PRODS.DES")]
        public string AdditionalProduct { get; set; }

        /// <summary>
        /// Cost with Components
        /// </summary>
        [UniVerseIndex(869)]
        [UniVerseFieldName("ORIG.COST.DES")]
        [UniVerseConversion("MR23")]
        public decimal? CostWithComponents { get; set; }

        /// <summary>
        /// Product Buyer
        /// </summary>
        [UniVerseIndex(873)]
        [UniVerseFieldName("PRODUCT.BUYER.DES")]
        public string ProductBuyer { get; set; }

        [JsonAlsoUpdates("MostRecentCarryForwardDate")]
        public DateTime? MostRecentCarryForwardDT { get; set; }

        /// <summary>
        /// Most Recent Carry Forward
        /// </summary>
        [UniVerseIndex(902,1)]
        [UniVerseFieldName("SS.CFWD.DT.DES")]
        public string MostRecentCarryForwardDate
        {
            get
            {
                if (MostRecentCarryForwardDT.HasValue)
                {
                    return Framework.UniVerseDateString(MostRecentCarryForwardDT.Value);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (Int32.TryParse(value, out int valInt))
                    {
                        MostRecentCarryForwardDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        MostRecentCarryForwardDT = null;
                    }
                }
            }
        }

        /// <summary>
        /// Price List Description
        /// </summary>
        [UniVerseIndex(903)]
        [UniVerseFieldName("PRLISTD.DES")]
        public string PriceListDescription { get; set; }

        /// <summary>
        /// Price List Pack Size
        /// </summary>
        [UniVerseIndex(905)]
        [UniVerseFieldName("PKSIZE.DES")]
        public string PriceListPackSize { get; set; }

        /// <summary>
        /// Additional Text
        /// </summary>
        [UniVerseIndex(906)]
        [UniVerseFieldName("ADDTEXT.DES")]
        public string AdditionalText { get; set; }

        /// <summary>
        /// Launch Product (Y/N)
        /// </summary>
        [UniVerseIndex(908)]
        [UniVerseFieldName("LAUNCH.PRODUCT.DES")]
        public bool LaunchProduct { get; set; }

        /// <summary>
        /// Market
        /// </summary>
        [UniVerseIndex(928)]
        [UniVerseFieldName("SS.PRODCLASS6")]
        public string Market { get; set; }

        /// <summary> *new*
        /// Part Number Range Start
        /// </summary>
        [UniVerseIndex(1005)]
        [UniVerseFieldName("RANGE.FROM")]
        public string PartNumberRangeStart { get; set; }

        /// <summary> *new*
        /// Part Number Range End
        /// </summary>
        [UniVerseIndex(1006)]
        [UniVerseFieldName("RANGE.TO")]
        public string PartNumberRangeEnd { get; set; }

        /// <summary> *new*
        /// Range Name
        /// </summary>
        [UniVerseIndex(1007)]
        [UniVerseFieldName("RANGE.NAME")]
        public string PartNumberRangeName { get; set; }

        /// <summary> *new*
        /// Last Range Used
        /// </summary>
        [UniVerseIndex(1008)]
        [UniVerseFieldName("RANGE.LAST")]
        public string PartNumberLastRange { get; set; }

        /// <summary> *new*
        /// Generate EANs?
        /// </summary>
        [UniVerseIndex(1009)]
        [UniVerseFieldName("GEN.EANBARCODE")]
        public bool CreateEANCodes { get; set; }

        /// <summary>
        /// Purchase Order Email (who should the PO be emailed to)
        /// </summary>
        [UniVerseIndex(1011)]
        [UniVerseFieldName("PO.EMAIL")]
        public string PurchaseOrderEmailAddress { get; set; }

        /// <summary>
        /// Basket Name
        /// </summary>
        [UniVerseIndex(1021)]
        [UniVerseFieldName("BASKET.NAME")]
        public string BasketName { get; set; }

        /// <summary>
        /// Product Type
        /// </summary>
        [UniVerseIndex(1026)]
        [UniVerseFieldName("SS.PRODTYPE")]
        public short? ProductType { get; set; }

        /// <summary>
        /// Colourway Name
        /// </summary>
        [UniVerseIndex(1028)]
        [UniVerseFieldName("SS.COLNAME")]
        public string ColourwayName { get; set; }

        /// <summary>
        /// Cost (in Currency)
        /// </summary>
        [UniVerseIndex(1031)]
        [UniVerseFieldName("SS.COST.CURR")]
        [UniVerseConversion("MR23")]
        public decimal? CostInCurrency { get; set; }

        /// <summary>
        /// Cost Price Target (£)
        /// </summary>
        [UniVerseIndex(1032)]
        [UniVerseFieldName("SS.COST.TARGET")]
        [UniVerseConversion("MR23")]
        public decimal? CostPriceTarget { get; set; }

        /// <summary>
        /// Currency
        /// </summary>
        [UniVerseIndex(1033)]
        [UniVerseFieldName("SS.CURRENCY")]
        public string Currency { get; set; }

        /// <summary>
        /// Exchange Rate
        /// </summary>
        [UniVerseIndex(1034)]
        [UniVerseFieldName("SS.EXCRATE")]
        [UniVerseConversion("MR23")]
        public decimal? ExchangeRate { get; set; }

        /// <summary>
        /// Final Cost Price (£)
        /// </summary>
        [UniVerseIndex(1035)]
        [UniVerseFieldName("SS.FINALCOST")]
        [UniVerseConversion("MR23")]
        public decimal? FinalCostPriceSterling { get; set; }

        /// <summary>
        /// Cost Excluding Additional Product (is this in Currency or Sterling?)
        /// If this is a Euro Supplier, this will be in Euro
        /// This follows the Supplier
        /// 
        /// We need to store this in both Currency and Sterling.
        /// </summary>
        [UniVerseIndex(1045)]
        [UniVerseFieldName("SS.COST.EX.PAD")]
        [UniVerseConversion("MR23")]
        public decimal? CostPriceExcludingAdditional { get; set; }

        /// <summary>
        /// Suggested Pricing - UK RRP Inc. VAT
        /// </summary>
        [UniVerseIndex(1046)]
        [UniVerseFieldName("SS.UKRRP")]
        [UniVerseConversion("MR23")]
        public decimal? UKRRPIncVAT { get; set; }

        /// <summary>
        /// Suggested Pricing - UK RRP Ex. VAT
        /// </summary>
        [UniVerseIndex(1047)]
        [UniVerseFieldName("SS.UKTRADE")]
        [UniVerseConversion("MR23")]
        public decimal? UKRRPExVAT { get; set; }

        /// <summary>
        /// Suggested Pricing - Euro Trade
        /// </summary>
        [UniVerseIndex(1049)]
        [UniVerseFieldName("SS.EUROTRADE")]
        [UniVerseConversion("MR23")]
        public decimal? EuroTradeExVAT { get; set; }

        /// <summary>
        /// Suggested Pricing - UK Gross Margin %
        /// </summary>
        [UniVerseIndex(1050)]
        [UniVerseFieldName("SS.EUROGM")]
        [UniVerseConversion("MR23")]
        public decimal? UKGrossMarginPercentage { get; set; }

        /// <summary>
        /// Product Developer Comments
        /// </summary>
        [UniVerseIndex(1052)]
        [UniVerseFieldName("SS.DEVCOM")]
        public string ProductDeveloperComments { get; set; }

        /// <summary>
        /// Merchandiser Code
        /// </summary>
        [UniVerseIndex(1053)]
        [UniVerseFieldName("SS.MERCH")]
        public int? MerchandiserCode { get; set; }

        /// <summary>
        /// Order Group - Table backed, select an order group value - from file SS.ORDERFORMGROUP
        /// </summary>
        [UniVerseIndex(1054)]
        [UniVerseFieldName("SS.ORDERGROUP")]
        public short? OrderFormGroup { get; set; }

        /// <summary>
        /// Packaging Description
        /// </summary>
        [UniVerseIndex(1056)]
        [UniVerseFieldName("SS.PACK.DESC")]
        public string PackagingDescription { get; set; }

        /// <summary>
        /// Shipping Terms
        /// </summary>
        [UniVerseIndex(1057)]
        [UniVerseFieldName("SS.SHIPTERM")]
        public string DeliveryTerms { get; set; }

        /// <summary>
        /// Knots per square metre
        /// </summary>
        [UniVerseIndex(1059)]
        [UniVerseFieldName("SS.KNOTS")]
        public short? KnotsPerSquareMetre { get; set; }

        /// <summary>
        /// Weight per square metre (kg)
        /// </summary>
        [UniVerseIndex(1060)]
        [UniVerseFieldName("SS.WEIGHSQM")]
        public decimal? WeightPerSquareMetre { get; set; }

        /// <summary>
        /// Transport - Shipping Terms... DDU, DDP, FOB, this is all Intrastat related, in theory goods are coming road / air / sea / probably don't need this to be fair.
        /// </summary>
        [UniVerseIndex(1062)]
        [UniVerseFieldName("SS.TRANSPORT")]
        public string Transport { get; set; }

        /// <summary>
        /// Cost per square metre (in Currency)
        /// </summary>
        [UniVerseIndex(1063)]
        [UniVerseFieldName("SS.COST.PER.SQM.CURR")]
        [UniVerseConversion("MR23")]
        public decimal? CostPerSquareMetreCurrency { get; set; }

        /// <summary>
        /// Cost per square metre (in Sterling)
        /// </summary>
        [UniVerseIndex(1064)]
        [UniVerseFieldName("SS.COST.PER.SQM.STG")]
        [UniVerseConversion("MR23")]
        public decimal? CostPerSquareMetreSterling { get; set; }

        /// <summary>
        /// Swatch Cost - is this in Currency or Sterling
        /// 
        /// This is currently in Sterling but really we would need both
        /// </summary>
        [UniVerseIndex(1071)]
        [UniVerseFieldName("SS.SWATCH.COST")]
        [UniVerseConversion("MR23")]
        public decimal? SwatchCost { get; set; }

        /// <summary>
        /// UK RRP Ex. VAT (but... what about SS.UKTRADE ==  This is probably defunct)
        /// </summary>
        //[UniVerseIndex(1073)]
        //[UniVerseFieldName("UK.RRP.EX.VAT")]
        //public string UKRRPExVAT_PMU { get; set; }

        /// <summary>
        /// Sales Quantity Forecast - Wholesale
        /// </summary>
        [UniVerseIndex(1074)]
        [UniVerseFieldName("SS.FC.TRADE")]
        public short? SalesQuantityForecast_Wholesale { get; set; }

        /// <summary>
        /// Sales Quantity Forecast - eCommerce
        /// </summary>
        [UniVerseIndex(1075)]
        [UniVerseFieldName("SS.FC.ECOM")]
        public short? SalesQuantityForecast_eCommerce { get; set; }

        /// <summary>
        /// Sales Quantity Forecast - Retail
        /// </summary>
        [UniVerseIndex(1076)]
        [UniVerseFieldName("SS.FC.RETAIL")]
        public short? SalesQuantityForecast_Retail { get; set; }

        /// <summary>
        /// Sales Quantity Forecast - DG Inc.
        /// </summary>
        [UniVerseIndex(1077)]
        [UniVerseFieldName("SS.FC.DGINC")]
        public short? SalesQuantityForecast_DGInc { get; set; }

        /// <summary>
        /// Length (cm)
        /// </summary>
        [UniVerseIndex(1078)]
        [UniVerseFieldName("SS.LENGTH")]
        public decimal? LengthCM { get; set; }

        /// <summary>SS.LENGTH
        /// Width (cm)
        /// </summary>
        [UniVerseIndex(1079)]
        [UniVerseFieldName("SS.WIDTH")]
        public decimal? WidthCM { get; set; }

        /// <summary>
        /// Height (cm)
        /// </summary>
        [UniVerseIndex(1080)]
        [UniVerseFieldName("SS.HEIGHT")]
        public decimal? HeightCM { get; set; }

        /// <summary>
        /// Length (inches)
        /// </summary>
        [UniVerseIndex(1081)]
        [UniVerseFieldName("SS.LENGTH.IN")]
        public decimal? LengthInches { get; set; }

        /// <summary>
        /// Width (inches)
        /// </summary>
        [UniVerseIndex(1082)]
        [UniVerseFieldName("SS.WIDTH.IN")]
        public decimal? WidthInches { get; set; }

        /// <summary>
        /// Height (inches)
        /// </summary>
        [UniVerseIndex(1083)]
        [UniVerseFieldName("SS.HEIGHT.IN")]
        public decimal? HeightInches { get; set; }

        /// <summary>
        /// Size Description - Backed by a table, find out the table SS.SIZE.DESC
        /// </summary>
        [UniVerseIndex(1084)]
        [UniVerseFieldName("SS.SIZEDESC")]
        public string SizeDescription { get; set; }

        /// <summary>
        /// Length (feet)
        /// </summary>
        [UniVerseIndex(1085)]
        [UniVerseFieldName("SS.LENGTH.FEET")]
        public decimal? LengthFeet { get; set; }

        /// <summary>
        /// Width (feet)
        /// </summary>
        [UniVerseIndex(1086)]
        [UniVerseFieldName("SS.WIDTH.FEET")]
        public decimal? WidthFeet { get; set; }

        /// <summary>
        /// Height (feet)
        /// </summary>
        [UniVerseIndex(1087)]
        [UniVerseFieldName("SS.HEIGHT.FEET")]
        public decimal? HeightFeet { get; set; }

        /// <summary>
        /// Attach Lithography - Sampling Requirements - Do we want to attach a printout of the whole pattern to the cutting
        /// 
        /// We probably want to extend this to offer a litho against both cutting and returnable 
        /// </summary>
        [UniVerseIndex(1173)]
        [UniVerseFieldName("ATT.LITHO")]
        public bool AttachLithography { get; set; }

        /// <summary>
        /// Sample Cutting Size - Should be a lookup, need CRUD, done in CM WxH
        /// </summary>
        [UniVerseIndex(1174)]
        [UniVerseFieldName("SS.SAMP.CUT")]
        public string SampleCuttingSize { get; set; }

        /// <summary>
        /// Returnable Cutting Size - Should be a lookup, need CRUD, done in CM WxH
        /// </summary>
        [UniVerseIndex(1175)]
        [UniVerseFieldName("SS.RET.CUT")]
        public string ReturnableSize { get; set; }

        /// <summary>
        /// Digital Design
        /// </summary>
        [UniVerseIndex(1176)]
        [UniVerseFieldName("SS.DIGITAL")]
        public bool DigitalDesign { get; set; }

        /// <summary>
        /// Wide Width Product (Y) Auto set if more than 200cm
        /// </summary>
        [UniVerseIndex(1177)]
        [UniVerseFieldName("SS.WIDE.WIDTH")]
        public bool WideWidth { get; set; }

        /// <summary>
        /// DG Special Colour (Y/N)
        /// </summary>
        [UniVerseIndex(1178)]
        [UniVerseFieldName("SS.DG.SPEC.COL")]
        public bool SpecialColour { get; set; }

        /// <summary>
        /// Diameter (cm)
        /// </summary>
        [UniVerseIndex(1179)]
        [UniVerseFieldName("SS.DIAM")]
        public decimal? DiameterCM { get; set; }

        ///// <summary>
        ///// Oeko-Tex Standard (Y/N) 
        ///// </summary>
        [UniVerseIndex(1180)]
        [UniVerseFieldName("SS.ADD.CARR")]
        public bool OekoTexStandard { get; set; }

        /// <summary>
        /// Master Approved (Y/N)
        /// </summary>
        [UniVerseIndex(1181)]
        [UniVerseFieldName("SS.MASTER")]
        public bool MasterApproved { get; set; }

        /// <summary>
        /// Flat Shot Image provided by:
        /// </summary>
        [UniVerseIndex(1182)]
        [UniVerseFieldName("SS.PHOTO.TYPE")]
        public string PhotoType { get; set; }

        /// <summary>
        /// Engineered Panel - Special part numbers might have coatings etc, we use that to make pattern books / cutting
        /// Yes or No
        /// </summary>
        [UniVerseIndex(1183)]
        [UniVerseFieldName("SS.ENG.PAN")]
        public bool EngineeredPanel { get; set; }

        /// <summary>
        /// Vertical Repeating Pattern (Inches)
        /// Default: (((VPATT.REP | 2.54)) * 1000)"MR23"[M]
        /// </summary>
        [UniVerseIndex(1184)]
        [UniVerseFieldName("SS.V.PAT.IN")]
        public decimal? VerticalPatternRepeatInches { get; set; }

        /// <summary>
        /// Horizontal Repeating Pattern (Inches)
        /// Default: (((HPATT.REP | 2.54)) * 1000)"MR23"[M]
        /// </summary>
        [UniVerseIndex(1185)]
        [UniVerseFieldName("SS.H.PAT.IN")]
        public decimal? HorizontalPatternRepeatInches { get; set; }

        /// <summary>
        /// Additional Product Cost (£)
        /// </summary>
        [UniVerseIndex(1500)]
        [UniVerseFieldName("SS.PAD.COST")]
        [UniVerseConversion("MR23")]
        public decimal? AdditionalProductCostSterling { get; set; }

        /// <summary>
        /// Web Pattern
        /// </summary>
        [UniVerseIndex(1501)]
        [UniVerseFieldName("SS.PATTERN")]
        public string WebPattern { get; set; }

        /// <summary>
        /// Maximum Fold Size
        /// </summary>
        [UniVerseIndex(1502)]
        [UniVerseFieldName("SS.MAXFOLD")]
        public byte? MaximumFoldSize { get; set; }

        /// <summary>
        /// Bespoke Hanger
        /// </summary>
        [UniVerseIndex(1503)]
        [UniVerseFieldName("SS.HANGER")]
        public bool BespokeHanger { get; set; }

        /// <summary>
        /// Bespoke Waterfall
        /// </summary>
        [UniVerseIndex(1504)]
        [UniVerseFieldName("SS.WATERFALL")]
        public bool BespokeWaterfall { get; set; }

        /// <summary>
        /// Bespoke Showlength
        /// </summary>
        [UniVerseIndex(1505)]
        [UniVerseFieldName("SS.SHOWLENGTH")]
        public bool BespokeShowlength { get; set; }

        /// <summary>
        /// Paste the Wall (Y/N)
        /// </summary>
        [UniVerseIndex(1506)]
        [UniVerseFieldName("SS.WPPASTE")]
        public bool PasteTheWall { get; set; }

        /// <summary>
        /// Length of Wallpaper Roll
        /// </summary>
        [UniVerseIndex(1508)]
        [UniVerseFieldName("ROLLLENGTH")]
        public decimal? RollLength { get; set; }

        /// <summary>
        /// Leather
        /// </summary>
        [UniVerseIndex(1517)]
        [UniVerseFieldName("LEATHER")]
        public bool Leather { get; set; }

        /// <summary>
        /// Product Type Description
        /// </summary>
        [UniVerseIndex(1518)]
        [UniVerseFieldName("PRODTYPEDESC")]
        public string ProductTypeDescription { get; set; }

        /// <summary>
        /// Linked Design in Diff Width
        /// </summary>
        [UniVerseIndex(1519)]
        [UniVerseFieldName("WIDTHEQIV")]
        public string LinkedDesignDifferentWidth { get; set; }

        /// <summary>
        /// Showlength in Store
        /// </summary>
        [UniVerseIndex(1520)]
        [UniVerseFieldName("SS.SHOW")]
        public bool ShowlengthInStore { get; set; }

        /// <summary>
        /// Roll Coverage (m2)
        /// </summary>
        [UniVerseIndex(1523)]
        [UniVerseFieldName("ROLLCOVER")]
        [UniVerseConversion("MR22")]
        public decimal? RollCoverage { get; set; }

        /// <summary>
        /// Stock Hanger
        /// </summary>
        [UniVerseIndex(1525)]
        [UniVerseFieldName("STOCKHANGER")]
        public string StockHanger { get; set; }

        /// <summary>
        /// Showlength Text
        /// </summary>
        [UniVerseIndex(1526)]
        [UniVerseFieldName("SHOWTEXT")]
        public string ShowlengthText { get; set; }

        /// <summary>
        /// Paint Coverage (square metre)
        /// </summary>
        [UniVerseIndex(1543)]
        [UniVerseFieldName("COVERAGE")]
        public decimal? PaintCoverageSquareMetre { get; set; }

        /// <summary>
        /// Paint Qualities
        /// </summary>
        [UniVerseIndex(1544)]
        [UniVerseFieldName("PAINTQUAL")]
        public string PaintQualities { get; set; }

        /// <summary>
        /// Same-day despatch order cut-off time
        /// </summary>
        [UniVerseIndex(1545)]
        [UniVerseFieldName("ORDERCUTOFF")]
        public string OrderCutOffTime { get; set; }

        /// <summary>
        /// Paint Sheen Percentage
        /// </summary>
        [UniVerseIndex(1546)]
        [UniVerseFieldName("PAINTSHEEN")]
        public byte? PaintSheenPercentage { get; set; }

        /// <summary>
        /// Paint Coverage Text
        /// </summary>
        [UniVerseIndex(1547)]
        [UniVerseFieldName("COVERAGETEXT")]
        public string PaintCoverageText { get; set; }

        /// <summary>
        /// Paint Sample Pot Code
        /// </summary>
        [UniVerseIndex(1548)]
        [UniVerseFieldName("SAMPLEPOTCODE")]
        public string PaintSamplePotCode { get; set; }

        /// <summary>
        /// Paint Minimum Coats
        /// </summary>
        [UniVerseIndex(1549)]
        [UniVerseFieldName("MINCOATS")]
        public byte? PaintMinimumCoats { get; set; }

        /// <summary>
        /// John Lewis Stock Reference
        /// </summary>
        [UniVerseIndex(1550)]
        [UniVerseFieldName("JLSTOCKREF")]
        public string JLStockReference { get; set; }

        /// <summary>
        /// Is this product reversible
        /// </summary>
        [UniVerseIndex(1551)]
        [UniVerseFieldName("REVERSIBLE")]
        public bool Reversible { get; set; }

        /// <summary>
        /// Wallpaper Hanging Instructions
        /// </summary>
        [UniVerseIndex(1552)]
        [UniVerseFieldName("HANGINST")]
        public string WallpaperHangingInstructions { get; set; }

        public DateTime? RelaunchedDT { get; set; }

        /// <summary>
        /// Relaunched Date
        /// </summary>
        [UniVerseIndex(1553)]
        [UniVerseFieldName("RELAUNCHED")]
        public string RelaunchedDate
        {
            get
            {
                if (RelaunchedDT.HasValue)
                {
                    return Framework.UniVerseDateString(RelaunchedDT.Value);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    if (Int32.TryParse(value, out int valInt))
                    {
                        RelaunchedDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {
                        RelaunchedDT = null;
                    }
                }
            }
        }

        /// <summary>
        /// Calculate total weight in SOP - Paint - check with Matt, Portal might have obsoleted this.
        /// </summary>
        [UniVerseIndex(1554)]
        [UniVerseFieldName("CHECK.WEIGHT")]
        public bool CheckWeight { get; set; }

        /// <summary>
        /// Paint Base
        /// </summary>
        [UniVerseIndex(1555)]
        [UniVerseFieldName("PAINT.BASE")]
        public string PaintBase { get; set; }

        /// <summary>
        /// Undercoat Colour
        /// </summary>
        [UniVerseIndex(1556)]
        [UniVerseFieldName("UNDERCOATCOLOUR")]
        public string PaintUndercoatColour { get; set; }

        /// <summary>
        /// Paint Colour Number
        /// </summary>
        [UniVerseIndex(1557)]
        [UniVerseFieldName("PAINTCOLNO")]
        public string PaintColourNumber { get; set; }

        /// <summary>
        /// Paint 1 Litre Undercoat Part Number
        /// </summary>
        [UniVerseIndex(1558)]
        [UniVerseFieldName("UC.CODE.1LTR")]
        public string Paint1LitreUndercoatPartNumber { get; set; }

        /// <summary>
        /// Paint 2.5 Litre Undercoat Part Number
        /// </summary>
        [UniVerseIndex(1559)]
        [UniVerseFieldName("UC.CODE.2.5LTR")]
        public string Paint2PointFiveLitreUndercoatPartNumber { get; set; }

        /// <summary>
        /// Country of Manufacture
        /// </summary>
        [UniVerseIndex(1560)]
        [UniVerseFieldName("CMANF")]
        public string CountryOfManufacture { get; set; }

        /// <summary>
        /// Country where Stock is Held
        /// </summary>
        [UniVerseIndex(1561)]
        [UniVerseFieldName("CSTOCK")]
        public string CountryWhereStockHeld { get; set; }

        /// <summary>
        /// Lead Time (Working Days)
        /// </summary>
        [UniVerseIndex(1567)]
        [UniVerseFieldName("LEADSM")]
        public byte? LeadTimeWorkingDays { get; set; }

        /// <summary>
        /// UNKNOWN? "What is this field" - No idea, defunct - Check if empty, if empty defunct it
        /// </summary>
        [UniVerseIndex(1568)]
        [UniVerseFieldName("LEADSW")]
        public byte? LeadSW { get; set; }

        /// <summary>
        /// Furniture Seat Type
        /// </summary>
        [UniVerseIndex(1572)]
        [UniVerseFieldName("FURNSEAT")]
        public string FurnitureSeatType { get; set; }

        /// <summary>
        /// Furniture Category Code
        /// </summary>
        [UniVerseIndex(1573)]
        [UniVerseFieldName("FURNCATCODE")] //  (Which cat can sit on this furniture...)
        public string FurnitureCategoryCode { get; set; }

        /// <summary>
        /// Image Type (Close Up, Full Width, N/A)
        /// </summary>
        [UniVerseIndex(1574)]
        [UniVerseFieldName("IMAGETYPE")]
        public string ImageType { get; set; }

        /// <summary>
        /// Paint Finish
        /// </summary>
        [UniVerseIndex(1580)]
        [UniVerseFieldName("PAINTFINISH")]
        public string PaintFinish { get; set; }

        /// <summary>
        /// Linked Bespoke Hanger Part Number
        /// </summary>
        [UniVerseIndex(1581)]
        [UniVerseFieldName("LINKEDHANGER")]
        public string LinkedBespokeHangerPartNumber { get; set; }

        /// <summary>
        /// Bulk clearance price in £ per M / Roll (how does this apply to rugs...) [Rugs only] 
        /// Keep it, needs to be on all product types - if you bought a bulk load of it we have to clear it at £2/m instead of £10/m
        /// </summary>
        [UniVerseIndex(1582)]
        [UniVerseFieldName("BULKCLEARPRICE")]
        public decimal? BulkClearancePriceSterling { get; set; }

        /// <summary>
        /// Linked Bespoke Rug Part Number
        /// </summary>
        [UniVerseIndex(1583)]
        [UniVerseFieldName("LINKEDBESRUG")]
        public string LinkedBespokeRugPartNumber { get; set; }

        /// <summary>
        /// Linked Rug Sample Part Number
        /// </summary>
        [UniVerseIndex(1584)]
        [UniVerseFieldName("LINKEDRUGSAMP")]
        public string LinkedBespokeRugSamplePartNumber { get; set; }

        /// <summary>
        /// Linked A6 Paint Card Part Number
        /// </summary>
        [UniVerseIndex(1585)]
        [UniVerseFieldName("LINKEDA6CARD")]
        public string LinkedA6PaintCardPartNumber { get; set; }

        /// <summary>
        /// Volumetric Weight
        /// </summary>
        [UniVerseIndex(1586)]
        [UniVerseFieldName("VOLUMEWEIGHT")]
        public decimal? VolumetricWeight { get; set; }

        /// <summary>
        /// Volumetric (Stock) Length (cm)
        /// </summary>
        [UniVerseIndex(1588)]
        [UniVerseFieldName("VLENGTH")]
        public decimal? VolumetricLengthCM { get; set; }

        /// <summary>
        /// Volumetric (Stock) Width (cm)
        /// </summary>
        [UniVerseIndex(1589)]
        [UniVerseFieldName("VWIDTH")]
        public decimal? VolumetricWidthCM { get; set; }

        /// <summary>
        /// Volumetric (Stock) Height (cm)
        /// </summary>
        [UniVerseIndex(1590)]
        [UniVerseFieldName("VHEIGHT")]
        public decimal? VolumetricHeightCM { get; set; }

        /// <summary>
        /// Paint Type
        /// </summary>
        [UniVerseIndex(1591)]
        [UniVerseFieldName("PAINTTYPE")]
        public string PaintType { get; set; }

        /// <summary>
        /// Fabric Pole Size (Standard or 3m)
        /// </summary>
        [UniVerseIndex(1592)]
        [UniVerseFieldName("POLETYPE")]
        public string PoleSize { get; set; }

        /// <summary>
        /// Customs Commodity Description
        /// </summary>
        [UniVerseIndex(1593)]
        [UniVerseFieldName("SS.FIELD6")]
        public string CustomsCommodityDescription { get; set; }

        /// <summary>
        /// JL Stock Code MTM
        /// </summary>
        [UniVerseIndex(1594)]
        [UniVerseFieldName("SS.FIELD7")]
        public string JLStockCodeMTM { get; set; }

        /// <summary>
        /// FRL Part Number
        /// </summary>
        [UniVerseIndex(1595)]
        [UniVerseFieldName("SS.FIELD8")]
        public string FRLPartNumber { get; set; }

        /// <summary>
        /// Folia Part Number
        /// </summary>
        [UniVerseIndex(1596)]
        [UniVerseFieldName("SS.FIELD9")]
        public string FoliaPartNumber { get; set; }

        /// <summary>
        /// Price List Delivery Band
        /// </summary>
        [UniVerseIndex(1597)]
        [UniVerseFieldName("SS.FIELD10")]
        public string PriceListDeliveryBand { get; set; }

        /// <summary>
        /// Spare Field 11
        /// </summary>
        [UniVerseIndex(1598)]
        [UniVerseFieldName("SS.FIELD11")]
        public string SpareField11 { get; set; }

        /// <summary>
        /// Spare Field 12
        /// </summary>s
        [UniVerseIndex(1599)]
        [UniVerseFieldName("SS.FIELD12")]
        public string SpareField12 { get; set; }

        /// <summary>
        /// Spare Field 13
        /// </summary>
        [UniVerseIndex(1600)]
        [UniVerseFieldName("SS.FIELD13")]
        public string SpareField13 { get; set; }

        /// <summary>
        /// Spare Field 14
        /// </summary>
        [UniVerseIndex(1601)]
        [UniVerseFieldName("SS.FIELD14")]
        public string SpareField14 { get; set; }


        #region Linked Entities

        [UniVerseForeignKey("AdditionalDescriptionText", "MertexerV2.Models.Product.OrderFormGroup")]
        public virtual OrderFormGroup objOrderFormGroup { get; set; }

        [UniVerseForeignKey("Quality", "MertexerV2.Models.Product.Quality")]
        public virtual Quality objQuality { get; set; }

        #endregion
    }
}
