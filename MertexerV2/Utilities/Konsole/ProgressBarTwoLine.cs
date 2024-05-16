using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Utilities.Konsole
{
    public class ProgressBarTwoLine : IProgressBar
    {
        private int _max;

        private readonly char _character;

        private readonly IConsole _console;

        private int _y;

        private int _current = 1;

        private ConsoleColor _c;

        private const string FORMAT = "Item {0,-5} of {1,-5}. ({2,-3}%) ";

        private static object _locker = new object();

        private string _line1 = "";

        private string _line2 = "";

        private string _item = "";

        public int Y => _y;

        public int Current => _current;

        public string Line1 => _line1;

        public string Line2 => _line2;

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

        internal ProgressBarTwoLine(int max, int? textWidth, char character, IConsole console)
        {
            lock (_locker)
            {
                _console = console;
                _y = _console.CursorTop;
                _c = _console.ForegroundColor;
                _current = 0;
                _max = max;
                _character = character;
                _console.WriteLine("");
                _console.WriteLine("");
            }
        }

        public void Refresh(int current, string format, params object[] args)
        {
            string item = string.Format(format, args);
            Refresh(current, item);
        }

        public void Refresh(int current, string item)
        {
            lock (_locker)
            {
                _item = item;
                string text = item ?? "";
                ConsoleState state = _console.State;
                _console.CursorVisible = false;
                _current = current;
                try
                {
                    float num = ((Max > 0) ? ((float)current / (float)_max) : 0f);
                    string text2 = new string(_character, (int)((float)(_console.Width - 30) * num));
                    string text3 = $"Item {current,-5} of {_max,-5}. ({(int)(num * 100f),-3}%) ";
                    int num2 = _console.Width - (text2.Length + text3.Length + 1);
                    _console.CursorTop = _y;
                    _console.CursorLeft = 0;
                    _console.ForegroundColor = _c;
                    _console.Write(text3);
                    _console.ForegroundColor = ConsoleColor.Green;
                    _console.Write(text2);
                    _console.WriteLine((num2 > 0) ? new string(' ', num2) : "");
                    _console.ForegroundColor = _c;
                    _line2 = text.PadRight(_console.Width - 2);
                    _console.WriteLine(_line2);
                    _line1 = text3 + " " + text2;
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
