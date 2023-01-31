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
    [UniVerseFileName("STADDITIONAL")]
    public class FlameCode : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public FlameCode(string key) : base(key)
        {
            Initialise();
        }

        public FlameCode() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(FlameCode).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            FlameCodeFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Description
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("DETAILS")]
        public string Detail { get; set; }
        
        /// <summary>
        /// Quick Description
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("BACKCOAT")]
        public string BackCoating { get; set; }

        /// <summary>
        /// Back Coating Required:
        /// "B"ackcoat Only;
        /// "N"ot Required;
        /// "I"nterline Only;
        /// "C"hoice of Backcoating or Interlining
        /// </summary>
        [UniVerseIndex(3)]
        [UniVerseFieldName("BCR")]
        public string BackCoatingRequired { get; set; }

        /// <summary>
        /// Image Font (Must be in the range "01" to "40")
        /// </summary>
        [UniVerseIndex(4)]
        [UniVerseFieldName("LABEL.DESC")]
        public string LabelDescription { get; set; }
    }
}
