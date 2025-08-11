using CsvHelper.Configuration.Attributes;

using MertexerV2.PricingFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.Pricing
{
    [UniVerseFileName("CONTPRICE")]
    public class ContractPrice : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public ContractPrice(string key) : base(key)
        {
            Initialise();
        }

        public ContractPrice() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(ContractPrice).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            ContractPriceFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
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
        /// Sort
        /// </summary>
        [UniVerseIndex(9)]
        [UniVerseFieldName("SORT")]
        public string SortType { get; set; }

        /// <summary>
        /// Quality
        /// </summary>
        [UniVerseIndex(10)]
        [UniVerseFieldName("QUALITY")]
        public string QualityCode { get; set; }

        /// <summary>
        /// VAT Inclusive
        /// </summary>
        [UniVerseIndex(29)]
        [UniVerseFieldName("VAT.INCLUSIVE")]
        public bool VATInclusive { get; set; }

        /// <summary>
        /// Sales Division (Brand) Code
        /// </summary>
        [UniVerseIndex(36)]
        [UniVerseFieldName("SALES.DIVISION.DES")]
        public string BrandCode { get; set; }

        /// <summary>
        /// Product Group Code
        /// </summary>
        [UniVerseIndex(37)]
        [UniVerseFieldName("PROD.GROUP.DES")]
        public string ProductGroupCode { get; set; }

        /// <summary>
        /// Item Type Code
        /// </summary>
        [UniVerseIndex(18)]
        [UniVerseFieldName("ITEMTYPE")]
        public string ItemTypeCode { get; set; }



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
        [UniVerseConversion("MR44")]
        [UniVerseFieldName("CONT.PRICE")]
        [UniVerseMultiValue]
        public IEnumerable<decimal?> ContractPrices { get; set; }


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

    public class ContractPriceExplosion
    {
        public string Code { get; set; }
        public string TradeAccountCode { get; set; }
        public string ProductCode { get; set; }
        public string SortType { get; set; }
        public string QualityCode { get; set; }
        public bool VATInclusive { get; set; }
        public string BrandCode { get; set; }
        public string ProductGroupCode { get; set; }
        public string ItemTypeCode { get; set; }
        public DateTime? StartDT { get; set; }
        public DateTime? ExpiryDT { get; set; }
        public decimal? ContractPrice { get; set; }
        public DateTime? LastAmendedDT { get; set; }
        public decimal? QuantityRequired { get; set; }

        public ContractPriceExplosion(
            ContractPrice cp,
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
            VATInclusive = cp.VATInclusive;
            BrandCode = cp.BrandCode;
            ProductGroupCode = cp.ProductGroupCode;
            ItemTypeCode = cp.ItemTypeCode;
            StartDT = startDt;
            ExpiryDT = expiryDt;
            ContractPrice = contractPrice;
            LastAmendedDT = lastAmendedDT;
            QuantityRequired = quantityRequired;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Framework.JsonSettingsIndentedIgnoreNullsAlphabetical);
        }
    }
}