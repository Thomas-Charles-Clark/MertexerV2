using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2SQL.Utilities
{
    public static class Framework
    {
        internal static readonly char ValueMark = (char)253;
        internal static readonly char SubValueMark = (char)252;

        internal static readonly char[] SpaceDelimiter = new char[] { ' ' };
        internal static readonly char[] AsteriskDelimiter = new char[] { '*' };
        internal static readonly char[] ValueMarkDelimiter = new char[] { (char)253 };
        internal static readonly char[] SubValueMarkDelimiter = new char[] { (char)252 };

        public static readonly string WavyLineBreak = $"\r\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\r\n";
    }

}
