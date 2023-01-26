using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MertexerV2.DAL;
//using U2.Data.Client;

namespace MertexerV2.Utilities
{
    public class SQLCommandExecutor : MertexConnection
    {
        public SQLCommandExecutor(string database = "MERTEX") : base(database, nativeMode: false)
        {

        }

        ~SQLCommandExecutor()
        {
            base.Dispose(false);
        }

        public string Run(string command)//, List<U2Parameter> commandParams = null)
        {
            var cmd = Session.CreateUniCommand();// Connection.CreateCommand();

#if DEBUG
            Console.WriteLine($"Running U2Command: \r\n\t{command}\r\n");
#endif
            //cmd.CommandText = command;

            cmd.Command = command;

            //if (commandParams != null)
            //{
            //    cmd.
            //    //cmd.
            //    cmd.Parameters.AddRange(commandParams.ToArray());
            //}

            var stopWatch = new System.Diagnostics.Stopwatch();

            stopWatch.Start();

            string result = "";

            try
            {
                //var reader = cmd.ExecuteReader();

                cmd.Execute();

                //while (reader.Read())
                //{
                //    for (var i = 0; i < reader.FieldCount; i++)
                //    {
                //        Console.WriteLine(reader.GetString(i));
                //        result += reader.GetString(i);
                //    }
                //}
            }
            catch (Exception ex)
            {
                throw ex;
            }

            stopWatch.Stop();

#if DEBUG
            Console.WriteLine($"U2Command Output (response time: {stopWatch.ElapsedMilliseconds}ms): \r\n{result}\r\n");
#endif

            return result;
        }
    }
}
