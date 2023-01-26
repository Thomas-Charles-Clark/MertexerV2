using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using U2.Data.Client;
using U2.Data.Client.UO;

namespace MertexerV2SQL.DAL
{
    public class MertexSQLConnection : IDisposable
    {
        public U2Connection Connection { get; set; }

        private string Database { get; set; }

        public MertexSQLConnection(string database = "MERTEX", int timeoutSeconds = 9000)
        {
            Database = database;
            
            try
            {
                var connStringBuilder = new U2ConnectionStringBuilder
                {
                    UserID = MertexConnectionDetails.UserID,
                    Password = MertexConnectionDetails.Password,
                    Server = MertexConnectionDetails.Server,
                    ServerType = MertexConnectionDetails.ServerType,
                    Database = Database ?? MertexConnectionDetails.Database,
                    Connect_Timeout = timeoutSeconds,
                    QueryTimeout = timeoutSeconds,
                    Pooling = false,
                    MinPoolSize = 1,
                    MaxPoolSize = 10,
                    PersistSecurityInfo = false
                };

                var connString = connStringBuilder.ToString();

                Connection = new U2Connection(connStringBuilder.ToString());

                var stopWatch = new System.Diagnostics.Stopwatch();

                stopWatch.Start();

                Connection.Open();

                stopWatch.Stop();

                Connection.Close();

#if DEBUG
                Console.WriteLine($"{this.GetType().Name} instantiated and connected to {Database ?? MertexConnectionDetails.Database} in {stopWatch.ElapsedMilliseconds}ms\r\n");
#endif
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex);
#endif
                throw ex;
            }
        }

        ~MertexSQLConnection()
        {
            Dispose(false);
        }

        protected void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                if (Connection.IsOpen)
                {
                    Connection.Close();
                }

                Connection.Dispose();
                Connection = null;

#if DEBUG
                Console.WriteLine($"MertexSQLConnection disposed successfully.\r\n");
#endif
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
    }
}
