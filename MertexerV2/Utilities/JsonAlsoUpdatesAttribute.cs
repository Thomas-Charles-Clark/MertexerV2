using System;
using System.Collections.Generic;

namespace MertexerV2.Utilities
{
    internal class JsonAlsoUpdatesAttribute : Attribute
    {
        public IEnumerable<string> Fields { get; set; }

        public JsonAlsoUpdatesAttribute(params string[] fields) 
        {
            Fields = fields;
        }
    }
}
