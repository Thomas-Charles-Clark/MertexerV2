using MertexerV2.Models.Interfaces;
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
    /// <summary>
    /// Defunct: EMPTY TABLE 26/01/2023
    /// </summary>
    [UniVerseFileName("STPATBK.DES")]
    public class PatternBook : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public PatternBook(string key) : base(key)
        {
            Initialise();
        }

        public PatternBook() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(PatternBook).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            PatternBookFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Sequence Number (MV)
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseMultiValue]
        [UniVerseFieldName("SEQ.NO")]
        public IEnumerable<string> SortOrders { get; set; }

        /// <summary>
        /// Fabric Code (MV)
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseMultiValue]
        [UniVerseFieldName("FABCODE")]
        public IEnumerable<string> FabricCodes { get; set; }
    }
}
