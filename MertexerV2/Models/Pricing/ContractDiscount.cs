using MertexerV2.Models.Interfaces;
using MertexerV2.PricingFunctions;
using MertexerV2.ProductFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Pricing
{
    [UniVerseFileName("CONTDISC")]
    public class ContractDiscount : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public ContractDiscount(string key) : base(key)
        {
            Initialise();
        }

        public ContractDiscount() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(ContractDiscount).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            ContractDiscountFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Trade Account Code
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("CUSNO")]
        public string TradeAccountCode { get; set; }

        /// <summary>
        /// Product Code
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("PRODCODE")]
        public string ProductCode { get; set; }

        /// <summary>
        /// Quality
        /// </summary>
        [UniVerseIndex(9)]
        [UniVerseFieldName("QUALITY")]
        public string QualityCode { get; set; }

        /// <summary>
        /// Sort
        /// </summary>
        [UniVerseIndex(10)]
        [UniVerseFieldName("SORT")]
        public string SortType { get; set; }

        /// <summary>
        /// Item Type Code
        /// </summary>
        [UniVerseIndex(18)]
        [UniVerseFieldName("ITEMTYPE")]
        public string ItemTypeCode { get; set; }

        /// <summary>
        /// PromotionCode
        /// </summary>
        [UniVerseIndex(29)]
        [UniVerseFieldName("PROMOTION.CODE.DES")]
        public string PromotionCode { get; set; }

        /// <summary>
        /// Sales Division (Brand) Code
        /// </summary>
        [UniVerseIndex(32)]
        [UniVerseFieldName("SALES.DIVISION.DES")]
        public string BrandCode { get; set; }

        /// <summary>
        /// Product Group Code
        /// </summary>
        [UniVerseIndex(33)]
        [UniVerseFieldName("PROD.GROUP.DES")]
        public string ProductGroupCode { get; set; }

        [JsonAlsoUpdates("StartDates")]
        public IEnumerable<DateTime?> StartDTs { get; set; }

        /// <summary>
        /// Start Dates
        /// </summary>
        [UniVerseIndex(45)]
        [UniVerseFieldName("SS.START.DATE.DES")]
        [UniVerseMultiValue]
        [JsonIgnore]
        public IEnumerable<string> StartDates
        {
            get
            {
                if (StartDTs.Any())
                {
                    return StartDTs.Select(dt => Framework.UniVerseDateString(dt.Value)).ToList();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != null && value.Any())
                {
                    var results = new List<DateTime?>();

                    foreach (var v in value)
                    {
                        if (Int32.TryParse(v, out int valInt))
                        {
                            results.Add(Framework.DateTimeFromUniVerseOffset(valInt));
                        }
                        else
                        {
                            results.Add(null);
                        }
                    }

                    StartDTs = results;
                }
            }
        }

        [JsonAlsoUpdates("ExpiryDates")]
        public IEnumerable<DateTime?> ExpiryDTs { get; set; }

        /// <summary>
        /// Expiry Dates
        /// </summary>
        [UniVerseIndex(3)]
        [UniVerseFieldName("SS.EXP.DATE")]
        [UniVerseMultiValue]
        [JsonIgnore]
        public IEnumerable<string> ExpiryDates
        {
            get
            {
                if (ExpiryDTs.Any())
                {
                    return ExpiryDTs.Select(dt => Framework.UniVerseDateString(dt.Value)).ToList();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != null && value.Any())
                {
                    var results = new List<DateTime?>();

                    foreach (var v in value)
                    {
                        if (Int32.TryParse(v, out int valInt))
                        {
                            results.Add(Framework.DateTimeFromUniVerseOffset(valInt));
                        }
                        else
                        {
                            results.Add(null);
                        }
                    }

                    ExpiryDTs = results;
                }
            }
        }

        /// <summary>
        /// Contract Prices
        /// </summary>
        [UniVerseIndex(4)]
        [UniVerseConversion("MR33")]
        [UniVerseFieldName("CONT.DISC")]
        [UniVerseMultiValue]
        public IEnumerable<decimal?> ContractDiscounts { get; set; }


        [JsonAlsoUpdates("AmendedDates")]
        public IEnumerable<DateTime?> LastAmendedDTs { get; set; }

        /// <summary>
        /// Expiry Dates
        /// </summary>
        [UniVerseIndex(5)]
        [UniVerseFieldName("SS.LAST.AMEND")]
        [UniVerseMultiValue]
        [JsonIgnore]
        public IEnumerable<string> AmendedDates
        {
            get
            {
                if (LastAmendedDTs.Any())
                {
                    return LastAmendedDTs.Select(dt => Framework.UniVerseDateString(dt.Value)).ToList();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != null && value.Any())
                {
                    var results = new List<DateTime?>();

                    foreach (var v in value)
                    {
                        if (Int32.TryParse(v, out int valInt))
                        {
                            results.Add(Framework.DateTimeFromUniVerseOffset(valInt));
                        }
                        else
                        {
                            results.Add(null);
                        }
                    }

                    LastAmendedDTs = results;
                }
            }
        }

        /// <summary>
        /// Quantity Required
        /// </summary>
        [UniVerseIndex(6)]
        [UniVerseConversion("MR33")]
        [UniVerseFieldName("QTYREQ")]
        [UniVerseMultiValue]
        public IEnumerable<decimal> QuantitiesRequired { get; set; }

    }

    public class ContractDiscountExplosion
    {
        public string Code { get; set; }
        public string TradeAccountCode { get; set; }
        public string ProductCode { get; set; }
        public string SortType { get; set; }
        public string QualityCode { get; set; }
        public string PromotionCode { get; set; }
        public string BrandCode { get; set; }
        public string ProductGroupCode { get; set; }
        public string ItemTypeCode { get; set; }
        public string PriceUnit { get; set; }
        public decimal? UnitConversion { get; set; }
        public DateTime? StartDT { get; set; }
        public DateTime? ExpiryDT { get; set; }
        public decimal? ContractDiscount { get; set; }
        public DateTime? LastAmendedDT { get; set; }
        public decimal? QuantityRequired { get; set; }

        public ContractDiscountExplosion(
            ContractDiscount cp,
            DateTime? startDt,
            DateTime? expiryDt,
            decimal? contractPrice,
            DateTime? lastAmendedDT,
            decimal? quantityRequired)
        {
            Code = cp.Key;
            TradeAccountCode = cp.TradeAccountCode;
            ProductCode = cp.ProductCode;
            SortType = cp.SortType;
            QualityCode = cp.QualityCode;
            PromotionCode = cp.PromotionCode;
            BrandCode = cp.BrandCode;
            ProductGroupCode = cp.ProductGroupCode;
            ItemTypeCode = cp.ItemTypeCode;
            StartDT = startDt;
            ExpiryDT = expiryDt;
            ContractDiscount = contractPrice;
            LastAmendedDT = lastAmendedDT;
            QuantityRequired = quantityRequired;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Framework.JsonSettingsIndentedIgnoreNullsAlphabetical);
        }
    }
}
