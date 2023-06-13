using MertexerV2.GeneralFunctions;
using MertexerV2.Utilities;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Models.General
{
    [UniVerseFileName("STPRODCLASS8")]
    public class DeliveryTerms : UniVerseSerializationBase<string>
    {
        [JsonRequired]
        [JsonProperty("Key")]
        public override string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }

        [JsonConstructor]
        public DeliveryTerms(string key) : base(key)
        {
            Initialise();
        }

        public DeliveryTerms() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(DeliveryTerms).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            DeliveryTermsFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        // This won't work for UPSERTING from SQL - because Matt has the table mapped as though Code is the PK,
        // which in Mertex it isn't. This list will likely never change though, so I'm not going to lose sleep over it.

        /// <summary>
        /// Code
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("CODEDESC.DES")]
        public string Code { get;set; }

        /// <summary>
        /// Sort Order
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("SORT.SEQUENCE")]
        public byte SortOrder { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        [UniVerseIndex(3)]
        [UniVerseFieldName("CODEDESC.DES")]
        public string Description { get; set; }
    }
}
