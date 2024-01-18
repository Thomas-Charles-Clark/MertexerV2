using MertexerV2.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MertexerV2.Utilities
{
    public class QueryExecutor : MertexConnection
    {
        public QueryExecutor() : base()
        {

        }

        public QueryExecutor(string database) : base(database)
        {

        }

        ~QueryExecutor()
        {
            base.Dispose(false);
        }

        public string Run(
            string uvFileName,
            string uvSelectStatement,
            string fieldsToReturn = "",
            string webData = "",
            string returnType = "XML",
            string identifier = "",
            bool htmlResult = false,
            int timeoutSeconds = 30)
        {
            var counter = 0;
            var foundCount = 0;

            var id = String.IsNullOrEmpty(identifier) ? Guid.NewGuid().ToString() : identifier;

            return Run(uvFileName,
                uvSelectStatement,
                ref counter,
                ref foundCount,
                id,
                fieldsToReturn,
                webData,
                returnType,
                htmlResult,
                timeoutSeconds
            );
        }

        public string Run(
            string uvFileName,
            string uvSelectStatement,
            ref int counter,
            ref int foundCount,
            string id,
            string fieldsToReturn = "",
            string webData = "",
            string returnType = "XML",
            bool htmlResult = false,
            int timeoutSeconds = 30)
        {
            var sub = Session.CreateUniSubroutine("RF.ASPLINK.V3", 15);

            Session.Timeout = timeoutSeconds;

            var pageSize = 0;
            var pageNumber = 1;

            sub.SetArg(0, $"MertexerV2-{id}");      // Identifier - Keith requested that I pass something.
            sub.SetArg(1, uvFileName);          // fileName ?? ""
            sub.SetArg(2, uvSelectStatement?.Replace("'", "\"") ?? "");
            sub.SetArg(3, fieldsToReturn);      // fieldsToReturn ?? ""  
                                                // without any selectionCriteria, RFUOdotNET forces the fieldsToReturn to empty.
                                                // if String.IsNullOrEmpty(criteria) sub.SetArg(3, "");
            sub.SetArg(4, returnType);          // returnType ?? "XML" or "TEXT"
            sub.SetArg(5, $"{pageSize}");       // pageSize // 0 == all pages
            sub.SetArg(6, $"{pageNumber}");     // pageNumber // 1 == page one
            sub.SetArg(7, $"{webData}");        // webData
            sub.SetArg(8, "");
            sub.SetArg(9, "");
            sub.SetArg(10, "");
            sub.SetArg(11, "");
            sub.SetArg(12, "");
            sub.SetArg(13, "");
            sub.SetArg(14, "");

            var watch = new System.Diagnostics.Stopwatch();

            watch.Start();

            sub.Call();

            watch.Stop();

            var result = !htmlResult ? sub.GetArg(12) : sub.GetArg(14);
            var returnFlag = sub.GetArg(13);


#if DEBUG
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("uvFileName:");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\t{uvFileName}\r\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("selectionCriteria:");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\t{uvSelectStatement}\r\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("fieldsToReturn:");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\t{fieldsToReturn}\r\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("webData:");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\t{String.Join($"\r\n\t", webData.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))}\r\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("result:");
            Console.ForegroundColor = ConsoleColor.Yellow;
            if (returnType == "XML")
            {
                try
                {
                    var xDoc = XDocument.Parse(result?.Trim(new char[] { '\"' }));
                    Console.WriteLine($"\t{String.Join($"\r\n\t", xDoc.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None))}\r\n");
                }
                catch
                {
                    Console.WriteLine($"{String.Join(System.Environment.NewLine, result.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Select(p => $"\t{p}"))}\r\n");
                }
            }
            else
            {
                Console.WriteLine($"{String.Join(System.Environment.NewLine, result.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Select(p => $"\t{p}"))}\r\n");
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Query returned in {watch.ElapsedMilliseconds}ms\r\n");

            if (++counter % 10 == 0)
                Console.WriteLine();
#endif


            //foundCount += xDoc.Root.Descendants("record").Count();

            if (returnFlag != "0")
            {
                throw new Exception(result);
            }

            return result.TrimStart(new char[] { '"' }).TrimEnd(new char[] { '"' });
        }
    }
}
