using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StockChecker.Utilities
{
    internal class Framework
    {
        private static ConsoleColor StoredColour;

        private static char HorizontalBar = '\u2501';
        private static char TopLeftCorner = '\u250F';
        private static char TopRightCorner = '\u2513';
        private static char VerticalBar = '\u2503';
        private static char BottomRightCorner = '\u251B';
        private static char BottomLeftCorner = '\u2517';

        private static int HeadingWidth = 80;

        private static int MaxHeadingTextLineLength = 40;

        private static void WriteHeadingTopLine()
        {
            Console.Write(TopLeftCorner);

            for (var i = 0; i < HeadingWidth - 2; i++)
            {
                Console.Write(HorizontalBar);
            }

            Console.WriteLine(TopRightCorner);
        }

        private static void WriteHeadingEmptyLine()
        {
            Console.Write(VerticalBar);

            for (var i = 0; i < HeadingWidth - 2; i++)
            {
                Console.Write(' ');
            }

            Console.WriteLine(VerticalBar);
        }

        private static void WriteHeadingTextLine(string line)
        {
            Console.Write(VerticalBar);

            if (line.Length % 2 != 0)
            {
                line += " ";
            }

            for (var i = 0; i < (HeadingWidth / 2) - 1 - line.Length / 2; i++)
            {
                Console.Write(" ");
            }

            Console.Write(line);

            for (var i = 0; i < (HeadingWidth / 2) - 1 - line.Length / 2; i++)
            {
                Console.Write(" ");
            }

            Console.WriteLine(VerticalBar);
        }

        private static void WriteHeadingText(string text)
        {
            // ta: https://stackoverflow.com/questions/4398270/how-to-split-string-preserving-whole-words#answer-4398471
            if (text.Length > MaxHeadingTextLineLength)
            {
                var characterCounter = 0;

                var lines = text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                            .GroupBy(w => (characterCounter += w.Length + 1) / MaxHeadingTextLineLength)
                            .Select(g => String.Join(" ", g));

                foreach (var l in lines)
                {
                    WriteHeadingTextLine(l);
                }
            }
            else
            {
                WriteHeadingTextLine(text);
            }
        }

        private static void WriteHeadingBottomLine()
        {
            Console.Write(BottomLeftCorner);

            for (var i = 0; i < HeadingWidth - 2; i++)
            {
                Console.Write(HorizontalBar);
            }

            Console.WriteLine(BottomRightCorner);
        }

        public static void WriteHeading(string text)
        {
#if DEBUG
            WriteHeadingTopLine();
            WriteHeadingEmptyLine();
            WriteHeadingText(text);
            WriteHeadingEmptyLine();
            WriteHeadingBottomLine();
#endif
        }

        public static void WriteLine(string text)
        {
#if DEBUG
            Console.WriteLine($"{text}");
#endif
        }

        public static void Write(string text)
        {
#if DEBUG
            Console.Write(text);
#endif
        }

        public static void SetConsoleColourGreen()
        {
            StoredColour = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Green;
        }

        public static void SetConsoleColourTesting()
        {
            StoredColour = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Yellow;
        }

        public static void SetConsoleColourError()
        {
            StoredColour = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Red;
        }

        public static void RestoreConsoleColour()
        {
            Console.ForegroundColor = StoredColour;
        }
    }
}
