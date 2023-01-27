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
    [UniVerseFileName("CLCOMP")]
    public class Composition : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public Composition(string key) : base(key)
        {
            Initialise();
        }

        public Composition() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(Composition).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            CompositionFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Description One
        /// </summary>
        [UniVerseIndex(1, 1)]
        [UniVerseFieldName("DESC1")]
        public string DescriptionOne { get; set; }

        /// <summary>
        /// Description Two
        /// </summary>
        [UniVerseIndex(1, 2)]
        [UniVerseFieldName("DESC2")]
        public string DescriptionTwo { get; set; }

        /// <summary>
        /// Description Three
        /// </summary>
        [UniVerseIndex(1, 3)]
        [UniVerseFieldName("DESC3")]
        public string DescriptionThree { get; set; }

        /// <summary>
        /// Description Four
        /// </summary>
        [UniVerseIndex(1, 4)]
        [UniVerseFieldName("SS.DESC4")]
        public string DescriptionFour { get; set; }

        /// <summary>
        /// Description Five
        /// </summary>
        [UniVerseIndex(1, 5)]
        [UniVerseFieldName("SS.DESC5")]
        public string DescriptionFive { get; set; }

        public string Description
        {
            get
            {
                return $"{DescriptionOne} {DescriptionTwo} {DescriptionThree} {DescriptionFour} {DescriptionFive}";
            }
        }
    }
}
