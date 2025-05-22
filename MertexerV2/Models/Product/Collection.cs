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
    [UniVerseFileName("PROD.COLL.DES")]
    public class Collection : UniVerseSerializationBase<string>
    {
        [JsonConstructor]
        public Collection(string key) : base(key)
        {
            Initialise();
        }

        public Collection() : base()
        {
            Initialise();
        }

        public override void Initialise()
        {
            typeof(Collection).GetProperties().ToList().ForEach(p => Framework.InitialiseUniProperty(this, p));
        }

        public void Upsert(string uniVerseAccount = "MERTEX")
        {
            CollectionFunctions.Upsert(uniVerseAccount, JsonConvert.SerializeObject(this));
        }

        /// <summary>
        /// Name
        /// </summary>
        [UniVerseIndex(1)]
        [UniVerseFieldName("COLL.DESC.DES")]
        public string Name { get; set; }

        public DateTime? LaunchDT { get; set; }

        [UniVerseIndex(4)]
        [UniVerseFieldName("SS.DATE.INTRO.DES.INT")]
        public string DateIntroduced
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

                    }
                }
            }
        }

    }
}
