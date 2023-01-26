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
        /// Product Code
        /// </summary>
        private T _key { get; set; }

        //[UniVerseIndex(0)]
        //[UniVerseFieldName("@ID")]
        public string Key
        {
            get
            {
//#if DEBUG
//                return $"{_key}".EndsWith("*TESTING") ? $"{_key}" : $"{_key}*TESTING";
//#else
                return $"{_key}";
//#endif
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

        public abstract void InitialiseFromEmptyConstructor();

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
