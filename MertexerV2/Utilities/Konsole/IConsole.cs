using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Utilities.Konsole
{
    public interface IConsole : IWrite
    {
        ConsoleState State { get; set; }

        int AbsoluteX { get; }

        int AbsoluteY { get; }

        int Width { get; }

        int Height { get; }

        int CursorTop { get; set; }

        int CursorLeft { get; set; }

        Colors Colors { get; set; }

        ConsoleColor ForegroundColor { get; set; }

        ConsoleColor BackgroundColor { get; set; }

        bool CursorVisible { get; set; }

        void PrintAt(int x, int y, string format, params object[] args);

        void PrintAt(int x, int y, string text);

        void PrintAt(int x, int y, char c);

        void PrintAtColor(ConsoleColor foreground, int x, int y, string text, ConsoleColor? background);

        void Clear();

        void Clear(ConsoleColor? backgroundColor);
    }
}
