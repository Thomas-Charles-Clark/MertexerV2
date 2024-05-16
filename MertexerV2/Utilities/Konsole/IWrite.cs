using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Utilities.Konsole
{
    // thanks to Goblinfactory.ProgressBar.dll
    public interface IWrite
    {
        void WriteLine(ConsoleColor color, string format, params object[] args);

        void WriteLine(string format, params object[] args);

        void Write(ConsoleColor color, string format, params object[] args);

        void Write(string format, params object[] args);
    }
}
