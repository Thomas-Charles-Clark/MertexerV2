using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Utilities
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal class UniVerseBooleanTrueWhen : Attribute
    {
        protected string value;

        public UniVerseBooleanTrueWhen(string v)
        {
            value = v;
        }

        public string Value
        {
            get
            {
                return value;
            }
        }
    }
}
