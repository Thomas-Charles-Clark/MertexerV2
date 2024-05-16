using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Utilities.Konsole
{
    public class ProgressBarSlim : IProgressBar
    {
        private int _max;

        private readonly char _character;

        private readonly IConsole _console;

        private int _y;

        private int _current;

        private ConsoleColor _c;

        private static object _locker = new object();

        private string _line = "";

        private string _item = "";

        public int TextWidth { get; } = 30;


        public int Y => _y;

        public string Line1 => _line;

        [Obsolete("only implemented to satisfy the interface, but is not used.")]
        public string Line2 => string.Empty;

        public string Item
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;
                Refresh(Current, Item);
            }
        }

        public int Max
        {
            get
            {
                return _max;
            }
            set
            {
                _max = value;
                Refresh(_current, _item);
            }
        }

        public int Current => _current;

        public ProgressBarSlim(int max)
            : this(max, 35, new ThreadsafeWriter())
        {
        }

        public ProgressBarSlim(int max, int textWidth)
            : this(max, textWidth, '#', new ThreadsafeWriter())
        {
        }

        public ProgressBarSlim(int max, int textWidth, char character)
            : this(max, textWidth, character, new ThreadsafeWriter())
        {
        }

        public ProgressBarSlim(int max, IConsole console)
            : this(max, null, '#', console)
        {
        }

        public ProgressBarSlim(int max, int textWidth, IConsole console)
            : this(max, textWidth, '#', console)
        {
        }

        public ProgressBarSlim(int max, int? textWidth, char character, IConsole console)
        {
            lock (_locker)
            {
                TextWidth = GetTextWidth(console, textWidth);
                _console = console;
                _y = _console.CursorTop;
                _c = _console.ForegroundColor;
                _current = 0;
                _max = max;
                _character = character;
                _console.WriteLine("");
            }
        }

        internal static int GetTextWidth(IConsole console, int? width)
        {
            if (!width.HasValue)
            {
                return console.Width / 4;
            }

            return width.Value;
        }

        public void Refresh(int current, string format, params object[] args)
        {
            string itemText = string.Format(format, args);
            Refresh(current, itemText);
        }

        public void Refresh(int current, string itemText)
        {
            string text = itemText ?? "";
            string arg = text.FixLeft(TextWidth);
            lock (_locker)
            {
                _item = text;
                ConsoleState state = _console.State;
                _console.CursorVisible = false;
                _current = current.Max(Max);
                try
                {
                    float num = (float)_current / (float)_max;
                    int num2 = _console.Width - (TextWidth + 8);
                    string text2 = ((_current > 0) ? new string(_character, (int)((float)num2 * num)).PadRight(num2) : new string(' ', num2));
                    string text3 = $"{arg} ({(int)(num * 100f),-3}%) ";
                    _console.CursorTop = _y;
                    _console.CursorLeft = 0;
                    _console.ForegroundColor = _c;
                    _console.Write(text3);
                    _console.ForegroundColor = ConsoleColor.Green;
                    _console.Write(text2);
                    _line = text3 + " " + text2;
                }
                finally
                {
                    _console.State = state;
                }
            }
        }

        public void Next(string item)
        {
            _current++;
            Refresh(_current, item);
        }
    }
}
