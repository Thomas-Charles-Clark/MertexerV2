using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Utilities.Konsole
{
    internal static class IntegerExtensions
    {
        internal static int Max(this int src, int max)
        {
            return src > max ? max : src;
        }
    }
}
