using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MertexerV2.Utilities.Konsole
{
    public class ProgressBar : IProgressBar
    {
        private readonly IProgressBar _bar;

        public int Y => _bar.Y;

        public int Current => _bar.Current;

        public string Line1 => _bar.Line1;

        public string Line2 => _bar.Line2;

        public int Max
        {
            get
            {
                return _bar.Max;
            }
            set
            {
                _bar.Max = value;
            }
        }

        public ProgressBar(int max)
            : this(max, null, '#', PbStyle.SingleLine, new ThreadsafeWriter())
        {
        }

        public ProgressBar(int max, int textWidth)
            : this(max, textWidth, '#', PbStyle.SingleLine, new ThreadsafeWriter())
        {
        }

        public ProgressBar(int max, int textWidth, char character)
            : this(max, textWidth, character, PbStyle.SingleLine, new ThreadsafeWriter())
        {
        }

        public ProgressBar(PbStyle style, int max)
            : this(max, null, '#', style, new ThreadsafeWriter())
        {
        }

        public ProgressBar(PbStyle style, int max, int textWidth)
            : this(max, textWidth, '#', style, new ThreadsafeWriter())
        {
        }

        public ProgressBar(PbStyle style, int max, int textWidth, char character)
            : this(max, textWidth, character, style, new ThreadsafeWriter())
        {
        }

        internal ProgressBar(IConsole console, int max)
            : this(max, null, '#', PbStyle.SingleLine, console)
        {
        }

        internal ProgressBar(IConsole console, int max, int textWidth)
            : this(max, textWidth, '#', PbStyle.SingleLine, console)
        {
        }

        internal ProgressBar(IConsole console, int max, int textWidth, char character)
            : this(max, textWidth, character, PbStyle.SingleLine, console)
        {
        }

        internal ProgressBar(IConsole console, PbStyle style, int max)
            : this(max, null, '#', style, console)
        {
        }

        internal ProgressBar(IConsole console, PbStyle style, int max, int textWidth)
            : this(max, textWidth, '#', style, console)
        {
        }

        internal ProgressBar(IConsole console, PbStyle style, int max, int textWidth, char character)
            : this(max, textWidth, character, style, console)
        {
        }

        private ProgressBar(int max, int? textWidth, char character, PbStyle style, IConsole console)
        {
            switch (style)
            {
                case PbStyle.DoubleLine:
                    _bar = new ProgressBarTwoLine(max, textWidth, character, console);
                    break;
                case PbStyle.SingleLine:
                    _bar = new ProgressBarSlim(max, textWidth, character, console);
                    break;
            }
        }

        public void Refresh(int current, string format, params object[] args)
        {
            _bar.Refresh(current, format, args);
        }

        public void Refresh(int current, string item)
        {
            _bar.Refresh(current, item);
        }

        public void Next(string item)
        {
            _bar.Next(item);
        }
    }
}
