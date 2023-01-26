using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using U2.Data.Client;
//using U2.Data.Client.UO;
using IBMU2.UODOTNET;

namespace MertexerV2.DAL
{
    public class MertexConnection : IDisposable
    {
        public UniSession Session { get; set; }

        private string Database { get; set; }

        public MertexConnection(string database = "MERTEX", bool nativeMode = true, int timeoutSeconds = 90)
        {
            Database = database;
            
            try
            {
                //public U2Connection Connection { get; set; }

                //var connStringBuilder = new U2ConnectionStringBuilder
                //{
                //    UserID = MertexConnectionDetails.UserID,
                //    Password = MertexConnectionDetails.Password,
                //    Server = MertexConnectionDetails.Server,
                //    ServerType = MertexConnectionDetails.ServerType,
                //    Database = Database ?? MertexConnectionDetails.Database,
                //    Connect_Timeout = timeoutSeconds,
                //    QueryTimeout = timeoutSeconds,
                //    Pooling = false,
                //    MinPoolSize = 1,
                //    MaxPoolSize = 10,
                //    PersistSecurityInfo = false
                //};

                //if (nativeMode)
                //{
                //    connStringBuilder.AccessMode = MertexConnectionDetails.AccessMode;
                //    connStringBuilder.RpcServiceType = MertexConnectionDetails.RcpServiceType;
                //}

                //var connString = connStringBuilder.ToString();

                //Connection = new U2Connection(connStringBuilder.ToString());

                var stopWatch = new System.Diagnostics.Stopwatch();

                UniObjects.UOPooling = false;
                UniObjects.TimeOut = 15;

                stopWatch.Start();

                Session = UniObjects.OpenSession(
                    MertexConnectionDetails.Server,
                    MertexConnectionDetails.UserID,
                    MertexConnectionDetails.Password,
                    Database
                );

                stopWatch.Stop();

                //UniObjects.UOPooling = true;
                //UniObjects.MinPoolSize = 1;
                //UniObjects.MaxPoolSize = 10;
                //UniObjects.TimeOut = 9000;
                //UniObjects.PoolingOpenSessionTimeOut = 9000;

                //Session = Connection.UniSession;

#if DEBUG
                //Console.WriteLine($"{this.GetType().Name} instantiated and connected to {Database ?? MertexConnectionDetails.Database} in {stopWatch.ElapsedMilliseconds}ms\r\n");
#endif
            }
            catch (Exception ex)
            {
                if (Session != null && Session.IsActive)
                {
                    //Connection.Close();
                    Session = null;
                }

#if DEBUG
                //Console.WriteLine(ex);
#endif
                throw ex;
            }
        }

        ~MertexConnection()
        {
            Dispose(false);
        }

        protected void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                //if (Connection.IsOpen)
                //{
                //    Connection.Close();
                //}

                //Connection.Dispose();

                if (Session != null)
                {
                    UniObjects.CloseSession(Session);

                    Session.Dispose();
                }

#if DEBUG
                //Console.WriteLine($"MertexConnection disposed successfully.\r\n");
#endif
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
