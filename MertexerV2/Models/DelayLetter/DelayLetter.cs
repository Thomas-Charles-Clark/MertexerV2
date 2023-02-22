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
    [UniVerseFileName("OE.RECONF.LET.DES1")]
    public class DelayLetter : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public DelayLetter(string key) : base(key)
        {
            Initialise();
        }

        public DelayLetter() : base()
        {
            Initialise();
        }


        public override void Initialise()
        {
            typeof(DelayLetter).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            DelayLetterFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Name
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("SO.PRODCODES.M.DES")]
        [UniVerseMultiValue]
        public IEnumerable<string> ProductCodes { get; set; }

        /// <summary>
        /// Market
        /// </summary>
        [UniVerseIndex(2)]
        [UniVerseFieldName("PREV.SO.DATE.M.DES")]
        public IEnumerable<string> PreviousDates 
        {
            get
            {
                if (PreviousDTs.Any())
                {
                    return PreviousDTs.Select(dt => Framework.UniVerseDateString(dt));
                }
                else
                {
                    return new List<string>();
                }
            }
            set
            {
                if (value != null)
                {
                    PreviousDTs = value.Select(v => Framework.DateTimeFromUniVerseOffset(Convert.ToInt32(v)));
                }
                else
                {
                    PreviousDTs = new List<DateTime>();
                }
            }
        }

        public IEnumerable<DateTime> PreviousDTs { get;set; }
    }
}
