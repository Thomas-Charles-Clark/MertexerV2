using MertexerV2.Models.Interfaces;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Utilities
{
    public abstract class UniVerseSerializationBase<T> : IDeserializeFromUniDynArray
    {
        /// <summary>
        /// Key
        /// </summary>
        protected T _key { get; set; }

        [JsonProperty("Code")]
        public virtual T Key
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
        public UniVerseSerializationBase(T key)
        {
            _key = key;
        }

        public UniVerseSerializationBase()
        {

        }

        public abstract void Initialise();

        public string ToStringInOrder()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Framework.JsonSettingsIndentedIgnoreNullsAlphabetical);
        }
    }
}
