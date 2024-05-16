using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Utilities.Konsole
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Align to the left, padd with spaces, and truncate any oveflow characters.
        /// </summary>
        internal static string FixLeft(this string src, int len)
        {
            int slen = src.Length;
            if (slen > len) return src.Substring(0, len);
            return string.Format("{0}{1}", src, new string(' ', len - slen));
        }
    }
}
