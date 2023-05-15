using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MertexerV2.DAL;
using MertexerV2.Models.TCL;
//using U2.Data.Client;

namespace MertexerV2.Utilities
{
    public class CommandExecutor : MertexConnection
    {
        public CommandExecutor(string database = "MERTEX", bool nativeMode = true, int timeoutSeconds = 90) : base(database, nativeMode, timeoutSeconds)
        {

        }

        ~CommandExecutor()
        {
            base.Dispose(false);
        }

        public string Item(string fileName, string recordId, bool lineSup = true)
        {
            var cmd = Session.CreateUniCommand();

            cmd.Command = $"LIST.ITEM {fileName} {recordId} {(lineSup ? "LINE.SUP" : "")}";
            cmd.Execute();
            var response_str = cmd.Response;

#if DEBUG
            Console.WriteLine("UniCommand Output: " + Environment.NewLine + response_str + Environment.NewLine);
#endif

            return response_str;
        }

        public string Dictionary(string fileName)
        {
            var cmd = Session.CreateUniCommand();

            cmd.Command = $"LIST DICT {fileName}";
            cmd.Execute();
            var response_str = cmd.Response;

#if DEBUG
            Console.WriteLine("UniCommand Output: " + Environment.NewLine + response_str + Environment.NewLine);
#endif
            return response_str;
        }

        public string DictionaryItem(string fileName, string fieldName)
        {
            var cmd = Session.CreateUniCommand();

            cmd.Command = $"LIST.ITEM DICT {fileName} \"{fieldName}\"";
            
            cmd.Execute();
            var response_str = cmd.Response;

            //#if DEBUG
            //            Console.WriteLine("UniCommand Output: " + Environment.NewLine + response_str + Environment.NewLine);
            //#endif 
            return response_str;
        }

        public string DictionaryItem(string fileName, string[] fieldNames = null)
        {
            var cmd = Session.CreateUniCommand();

            if (fieldNames != null)
            {
                cmd.Command = $"LIST.ITEM DICT {fileName} \"{(String.Join("\" \"", fieldNames))}\"";
            }
            else
            {
                cmd.Command = $"LIST.ITEM DICT {fileName}";
            }

            cmd.Execute();
            var response_str = cmd.Response;

//#if DEBUG
//            Console.WriteLine("UniCommand Output: " + Environment.NewLine + response_str + Environment.NewLine);
//#endif 
            return response_str;
        }

        public string DictionaryXML(string fileName)
        {
            var cmd = Session.CreateUniCommand();

            cmd.Command = $"LIST DICT {fileName} TOXML";
            cmd.Execute();
            var response_str = cmd.Response;

//#if DEBUG
//            Console.WriteLine("UniCommand Output: " + Environment.NewLine + response_str + Environment.NewLine);
//#endif
            return response_str;
        }


        public string DictionaryVertical(string fileName)
        {
            var cmd = Session.CreateUniCommand();

            cmd.Command = $"LIST.ITEM DICT {fileName}";
            cmd.Execute();
            var response_str = cmd.Response;

#if DEBUG
            Console.WriteLine("UniCommand Output: " + Environment.NewLine + response_str + Environment.NewLine);
#endif
            return response_str;
        }


        public string Arbitrary(string command)
        {
            var cmd = Session.CreateUniCommand();

#if DEBUG
            Console.WriteLine($"Running UniCommand: \r\n\t{command}\r\n");
#endif

            var stopWatch = new System.Diagnostics.Stopwatch();

            stopWatch.Start();

            cmd.Command = command;
            cmd.Execute();

            var response_str = cmd.Response;

            response_str = cmd.Response;

            stopWatch.Stop();

//#if DEBUG
//            Console.WriteLine($"UniCommand Output (response time: {stopWatch.ElapsedMilliseconds}ms): \r\n{response_str}\r\n");
//#endif 

            return response_str;
        }

        /// Remembeer to COL.HDR.SUPP ID.SUP COUNT.SUPP
        public static TCLItemResult CrunchTCLResult(string input)
        {
            if (String.IsNullOrEmpty(input)) 
            {
                throw new ArgumentNullException("CrunchTCLResult expected input");
            }

            input = input.Trim();

            var result = new TCLItemResult();

            var lineSplit = input.Split(new string[] { System.Environment.NewLine },
                StringSplitOptions.None).ToList();

            for (var i = lineSplit.Count - 1; i > 0; i--)
            {
                if (i > 0)
                {
                    if (lineSplit[i].StartsWith(" "))
                    {
                        lineSplit[i - 1] += lineSplit[i];
                        lineSplit[i] = null;
                    }
                }
            }

            lineSplit.RemoveAll(p => p == null);

            var columnSplitter = new System.Text.RegularExpressions.Regex("^\\d+\\s{1}");

            foreach (var i in lineSplit)
            {
                var regexResult = columnSplitter.Match(i);

                var columnIndex = regexResult.Value;

                var columnValue = i.Substring(columnIndex.Length);

                result.Columns.Add(new ColumnValue()
                {
                    Index = Convert.ToInt32(columnIndex),
                    Value = columnValue
                }); ;
            }

            return result;
        }
    }
}
