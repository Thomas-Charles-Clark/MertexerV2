using MertexerV2.ProductFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Linq;

namespace MertexerV2.Models.Product
{
    [UniVerseFileName("CLQUALDES")]
    public class Quality : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public Quality(string key) : base(key)
        {

        }

        public Quality() : base()
        {
            InitialiseFromEmptyConstructor();
        }

        public override void InitialiseFromEmptyConstructor()
        {
            typeof(Quality).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            QualityFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        [UniVerseIndex(1)]
        [UniVerseFieldName("DESIGN")]
        public string Design { get; set; }

        [UniVerseIndex(2)]
        [UniVerseFieldName("COMPCODE")]
        public string Composition { get; set; }

        [UniVerseIndex(3)]
        [UniVerseFieldName("WIDTH")]
        public decimal? Width { get; set; }

        [UniVerseIndex(4)]
        [UniVerseFieldName("APPGMS")]
        public decimal? WeightGSM { get; set; }

        [UniVerseIndex(6)]
        [UniVerseFieldName("DESC")]
        public string Description { get; set; }

        [UniVerseIndex(9)]
        [UniVerseConversion("MR11")]
        [UniVerseFieldName("FIN.WIDTH")]
        public decimal? FinishedWidth { get; set; }

        public DateTime? CreatedDT { get; set; }

        [UniVerseIndex(12)]
        [UniVerseFieldName("SS.ONFILE.INT")]
        public string CreatedDate
        {
            get
            {
                if (CreatedDT.HasValue)
                {
                    return Framework.UniVerseDateString(CreatedDT.Value);
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
                        CreatedDT = Framework.DateTimeFromUniVerseOffset(valInt);
                    }
                    else
                    {

                    }
                }
            }
        }

        [UniVerseIndex(25)]
        [UniVerseFieldName("GMSLIN")]
        public decimal? GramsPerLinearMetre { get; set; }

        [UniVerseIndex(59)]
        [UniVerseConversion("MR12")]
        [UniVerseFieldName("REMEASURE.LGTH")]
        public decimal? RemeasureLength { get; set; }

        [UniVerseIndex(96)]
        [UniVerseFieldName("SEP.MAT.LENGTH")]
        public bool SeparateMaterialLength { get; set; }

        [UniVerseIndex(98)]
        [UniVerseFieldName("ALT.RAW.MATS")]
        public bool AlternativeRawMaterials { get; set; }
    }
}