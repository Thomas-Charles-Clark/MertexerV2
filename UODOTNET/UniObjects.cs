// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniObjects
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

using System;
using System.Collections;
using System.Configuration;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;

namespace IBMU2.UODOTNET
{
  public sealed class UniObjects : UniRoot
  {
    private static bool m_UOPooling = false;
    private static int m_SocketTimeout = 30000;
    private static Hashtable m_PoolList;
    private static int m_MinPoolSize = 1;
    private static int m_MaxPoolSize = 10;
    private static Thread m_PollingThread = (Thread) null;
    private static UniPerfCounter m_UniPerfCounter = (UniPerfCounter) null;
    private static int m_PoolingOpenSessionTimeout = 30000;
    private static int m_IdleRemoveThreshold = 300000;
    private static int m_IdleRemoveExecInterval = 300000;
    private static Timer m_Timer;
    private static ResourceManager m_RM = new ResourceManager("IBMU2.UODOTNET.UODOTNET.Resources", Assembly.GetExecutingAssembly());
    private static UniTrace m_UniTrace = new UniTrace();
    private static readonly UniObjects m_Instance = new UniObjects();

    public static UniSession OpenSession(
      string hostname,
      string userid,
      string password,
      string account)
    {
      return UniObjects.OpenSession(hostname, userid, password, account, UniObjects.m_RM.GetString("DEFAULT_SERVICE_TYPE"));
    }

    public static UniSession OpenSession(
      string hostname,
      int port,
      string userid,
      string password,
      string account)
    {
      return UniObjects.OpenSession(hostname, port, userid, password, account, UniObjects.m_RM.GetString("DEFAULT_SERVICE_TYPE"));
    }

    public static UniSession OpenSession(
      string hostname,
      string userid,
      string password,
      string account,
      string service)
    {
      UniSession uniSession;
      try
      {
        string empty = string.Empty;
        if (!UniObjects.m_UOPooling)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniObjects::OpenSession()" + UniObjects.ConnectStringCPOFF(hostname, 31438, userid, password, account, service));
          uniSession = new UniSession();
          uniSession.SetServiceType(service);
          uniSession.Connect(hostname, 31438, userid, password, account);
        }
        else
          uniSession = UniObjects.OpenSession(hostname, 31438, userid, password, account, service, UniObjects.m_MinPoolSize, UniObjects.m_MaxPoolSize);
      }
      catch (Exception ex)
      {
        throw ex;
      }
      return uniSession;
    }

    public static UniSession OpenSession(
      string hostname,
      int port,
      string userid,
      string password,
      string account,
      string service)
    {
      UniSession uniSession;
      try
      {
        string empty = string.Empty;
        if (!UniObjects.m_UOPooling)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniObjects::OpenSession()" + UniObjects.ConnectStringCPOFF(hostname, port, userid, password, account, service));
          uniSession = new UniSession();
          uniSession.SetServiceType(service);
          uniSession.Connect(hostname, port, userid, password, account);
        }
        else
          uniSession = UniObjects.OpenSession(hostname, port, userid, password, account, service, UniObjects.m_MinPoolSize, UniObjects.m_MaxPoolSize);
      }
      catch (Exception ex)
      {
        throw ex;
      }
      return uniSession;
    }

    public static void CloseSession(UniSession us)
    {
      try
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter  UniObjects:: CloseSession() ");
        if (!us.IsPooledSession)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniObjects:: CloseSession() ");
          us.Disconnect();
          return;
        }
        lock (typeof (UniObjects))
        {
          string key = us.HostName + us.UserName + us.Password + us.Account;
          UniPool pool = (UniPool) UniObjects.m_PoolList[(object) key];
          if (pool == null)
          {
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniObjects:: CloseSession() Could not found UniPool Class in HashTable");
            return;
          }
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniObjects:: CloseSession() Found UniPool Class in HashTable");
          pool.Free(us);
        }
      }
      catch (Exception ex)
      {
        string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
        throw new Exception(message);
      }
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit  UniObjects:: CloseSession() ");
    }

    internal static void OpenPool(
      string hostname,
      string userid,
      string password,
      string account,
      int min_poolsize,
      int max_poolsize)
    {
      UniObjects.OpenPool(hostname, userid, password, account, UniObjects.m_RM.GetString("DEFAULT_SERVICE_TYPE"), min_poolsize, max_poolsize);
    }

    internal static void OpenPool(
      string hostname,
      string userid,
      string password,
      string account,
      string service,
      int min_poolsize,
      int max_poolsize)
    {
      try
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniObjects::OpenPool()" + UniObjects.ConnectStringCPON(hostname, 31438, userid, password, account, service, min_poolsize, max_poolsize));
        if (!UniObjects.m_UOPooling)
        {
          Exception exception = new Exception(UniErrorMessage.GetErrorMessage(86));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, exception.Message + " Source: UniObjects Class Method: " + (object) exception.TargetSite);
          throw exception;
        }
        lock (typeof (UniObjects))
        {
          string key = hostname + userid + password + account;
          if ((UniPool) UniObjects.m_PoolList[(object) key] == null)
          {
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniObjects::OpenPool() Creating UniPool Class");
            UniPool uniPool = new UniPool(hostname, userid, password, account, service, min_poolsize, max_poolsize, 31438);
            UniObjects.m_PoolList.Add((object) key, (object) uniPool);
            UniObjects.m_UniPerfCounter.ConnectionPools.Increment();
          }
          else
          {
            Exception exception = new Exception(UniErrorMessage.GetErrorMessage(90));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, exception.Message + " Source: UniObjects Class Method: " + (object) exception.TargetSite);
            throw exception;
          }
        }
      }
      catch (Exception ex)
      {
        string message = ex.Message + " Source: UniObjects Class Method: " + (object) ex.TargetSite + ex.StackTrace;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
        throw new Exception(message);
      }
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniObjects::OpenPool()");
    }

    internal static UniSession OpenSession(
      string hostname,
      string userid,
      string password,
      string account,
      int min_poolsize,
      int max_poolsize)
    {
      return UniObjects.OpenSession(hostname, 31438, userid, password, account, UniObjects.m_RM.GetString("DEFAULT_SERVICE_TYPE"), min_poolsize, max_poolsize);
    }

    internal static UniSession OpenSession(
      string hostname,
      int port_number,
      string userid,
      string password,
      string account,
      string service,
      int min_poolsize,
      int max_poolsize)
    {
      UniPool uniPool = (UniPool) null;
      UniSession session;
      try
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniObjects::OpenSession()" + UniObjects.ConnectStringCPON(hostname, port_number, userid, password, account, service, min_poolsize, max_poolsize));
        if (!UniObjects.m_UOPooling)
        {
          Exception exception = new Exception(UniErrorMessage.GetErrorMessage(86));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, exception.Message + " Source: UniObjects Class Method: " + (object) exception.TargetSite);
          throw exception;
        }
        lock (typeof (UniObjects))
        {
          string key = hostname + userid + password + account;
          uniPool = (UniPool) UniObjects.m_PoolList[(object) key];
          if (uniPool == null)
          {
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniObjects::OpenSession() Creating UniPool Class");
            uniPool = new UniPool(hostname, userid, password, account, service, min_poolsize, max_poolsize, port_number);
            UniObjects.m_PoolList.Add((object) key, (object) uniPool);
            UniObjects.m_UniPerfCounter.ConnectionPools.Increment();
          }
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniObjects::OpenSession() Calling FindSession()");
        }
        session = uniPool.FindSession(UniObjects.m_PoolingOpenSessionTimeout);
      }
      catch (Exception ex)
      {
        string message = ex.Message + " Source: UniObjects Class Method: " + (object) ex.TargetSite + ex.StackTrace;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
        throw new Exception(message);
      }
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniObjects::OpenSession()");
      return session;
    }

    public static void CloseAllSessions()
    {
      try
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniObjects::CloseAllSessions()");
        if (!UniObjects.m_UOPooling)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniObjects:: CloseAllSessions() ");
          return;
        }
        lock (typeof (UniObjects))
        {
          IDictionaryEnumerator enumerator = UniObjects.m_PoolList.GetEnumerator();
          while (enumerator.MoveNext())
            ((UniPool) enumerator.Value).CloseAllConnections();
          UniObjects.m_PoolList.Clear();
        }
      }
      catch (UniSessionException ex)
      {
        string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
        throw new Exception(message);
      }
      catch (Exception ex)
      {
        string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
        throw new Exception(message);
      }
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniObjects::CloseAllSessions()");
    }

    public static void CloseServerConnection(UniSession us)
    {
      try
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter  UniObjects:: CloseServerConnection() ");
        if (!UniObjects.m_UOPooling)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniObjects:: CloseServerConnection() ");
          us.Disconnect();
          return;
        }
        lock (typeof (UniObjects))
        {
          string key = us.HostName + us.UserName + us.Password + us.Account;
          UniPool pool = (UniPool) UniObjects.m_PoolList[(object) key];
          if (pool == null)
          {
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniObjects:: CloseServerConnection() Could not found UniPool Class in HashTable");
            return;
          }
          pool.CloseServerConnection(us);
        }
      }
      catch (Exception ex)
      {
        string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
        throw new Exception(message);
      }
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit  UniObjects:: CloseServerConnection() ");
    }

    private static void PoolingThread()
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, "Enter UniObjects::PoolingThread() ");
      UniObjects.m_Timer = new Timer(new TimerCallback(UniObjects.CloseIdleSessions), (object) null, 0, UniObjects.m_IdleRemoveExecInterval);
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, "Exit UniObjects::PoolingThread() ");
    }

    private static void CloseIdleSessions(object o)
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, "Enter UniObjects::CloseIdleSessions() " + (object) DateTime.Now);
      IDictionaryEnumerator enumerator = UniObjects.m_PoolList.GetEnumerator();
      while (enumerator.MoveNext())
        ((UniPool) enumerator.Value).CloseIdleConnections();
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, "Exit UniObjects::CloseIdleSessions()");
    }

    ~UniObjects()
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniObjects::~UniObjects()  : This is a destructor of sealed singleton class" + (object) DateTime.Now);
      if (UniObjects.m_UOPooling && UniObjects.m_PollingThread != null)
      {
        UniObjects.m_PollingThread.Abort();
        UniObjects.m_PollingThread = (Thread) null;
      }
      UniObjects.CloseAllSessions();
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniObjects::~UniObjects()");
    }

    public static bool UOPooling
    {
      get => UniObjects.m_UOPooling;
      set
      {
        lock (typeof (UniObjects))
        {
          if (!UniObjects.m_UOPooling && value)
          {
            UniObjects.m_UOPooling = value;
            UniObjects.InitPerformanceCounter();
            UniObjects.InitDefaultValues();
          }
          else
          {
            if (!UniObjects.m_UOPooling || value)
              return;
            UniObjects.CloseAllSessions();
            UniObjects.m_UOPooling = value;
            UniObjects.m_PollingThread.Abort();
            UniObjects.m_PollingThread = (Thread) null;
          }
        }
      }
    }

    public static int TimeOut
    {
      get => UniObjects.m_SocketTimeout;
      set => UniObjects.m_SocketTimeout = value;
    }

    public static int MinPoolSize
    {
      get => UniObjects.m_MinPoolSize;
      set
      {
        if (value <= UniObjects.m_MaxPoolSize)
          UniObjects.m_MinPoolSize = value;
        if (UniObjects.m_MinPoolSize >= 1)
          return;
        UniObjects.m_MinPoolSize = 1;
      }
    }

    public static int MaxPoolSize
    {
      get => UniObjects.m_MaxPoolSize;
      set
      {
        if (value >= UniObjects.m_MinPoolSize)
          UniObjects.m_MaxPoolSize = value;
        else
          UniObjects.m_MaxPoolSize = UniObjects.m_MinPoolSize;
      }
    }

    public static int PoolingOpenSessionTimeOut
    {
      get => UniObjects.m_PoolingOpenSessionTimeout;
      set => UniObjects.m_PoolingOpenSessionTimeout = value;
    }

    public static int IdleRemoveThreshold
    {
      get => UniObjects.m_IdleRemoveThreshold;
      set => UniObjects.m_IdleRemoveThreshold = value;
    }

    public static int IdleRemoveExecInterval
    {
      get => UniObjects.m_IdleRemoveExecInterval;
      set => UniObjects.m_IdleRemoveExecInterval = value;
    }

    internal static UniPerfCounter PerformanceCounter => UniObjects.m_UniPerfCounter;

    internal static UniTrace Trace => UniObjects.m_UniTrace;

    internal static ResourceManager UniResourceManager => UniObjects.m_RM;

    private static void CheckConfigSetting(string key, string ConfigName, ref int pValue)
    {
      try
      {
        IDictionary config = (IDictionary) ConfigurationSettings.GetConfig(ConfigName);
        if (config != null)
        {
          string str = (string) config[(object) key];
          if (str != null && !str.Equals(string.Empty))
            pValue = Convert.ToInt32(str);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "key :" + key + " and value :" + (object) pValue);
      }
      catch (Exception ex)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_WARNING, ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace);
      }
    }

    private UniObjects()
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniObjects::UniObjects() This is a constructor of sealed singleton class");
      UniObjects.CheckConfigSetting("SockeTimeOut", "UO.NET/General", ref UniObjects.m_SocketTimeout);
      int pValue = -1;
      UniObjects.CheckConfigSetting("ConnectionPoolingOn", "UO.NET/ConnectionPooling", ref pValue);
      if (pValue != -1)
        UniObjects.m_UOPooling = Convert.ToBoolean(pValue);
      if (UniObjects.m_UOPooling)
      {
        UniObjects.InitDefaultValues();
        UniObjects.InitPerformanceCounter();
      }
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniObjects::UniObjects()");
    }

    private static void InitDefaultValues()
    {
      if (!UniObjects.m_UOPooling)
        return;
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Config File is " + AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
      UniObjects.CheckConfigSetting("MinimumPoolSize", "UO.NET/ConnectionPooling", ref UniObjects.m_MinPoolSize);
      UniObjects.CheckConfigSetting("MaximumPoolSize", "UO.NET/ConnectionPooling", ref UniObjects.m_MaxPoolSize);
      UniObjects.CheckConfigSetting("IdleRemoveThreshold", "UO.NET/ConnectionPooling", ref UniObjects.m_IdleRemoveThreshold);
      UniObjects.CheckConfigSetting("IdleRemoveExecInterval", "UO.NET/ConnectionPooling", ref UniObjects.m_IdleRemoveExecInterval);
      UniObjects.CheckConfigSetting("OpenSessionTimeOut", "UO.NET/ConnectionPooling", ref UniObjects.m_PoolingOpenSessionTimeout);
      UniObjects.m_PoolList = new Hashtable();
      UniObjects.m_PollingThread = new Thread(new ThreadStart(UniObjects.PoolingThread));
      UniObjects.m_PollingThread.Name = "Pooling Monitor Thread";
      UniObjects.m_PollingThread.Start();
    }

    private static void InitPerformanceCounter() => UniObjects.m_UniPerfCounter = new UniPerfCounter();

    private static string ConnectStringCPOFF(
      string hostname,
      int port,
      string userid,
      string password,
      string account,
      string service)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(" hostname : ");
      stringBuilder.Append(hostname);
      stringBuilder.Append(" port : ");
      stringBuilder.Append(port);
      stringBuilder.Append(" User : ");
      stringBuilder.Append(userid);
      stringBuilder.Append(" Password : ");
      stringBuilder.Append(password);
      stringBuilder.Append(" Account : ");
      stringBuilder.Append(account);
      stringBuilder.Append(" Service : ");
      stringBuilder.Append(service);
      return stringBuilder.ToString();
    }

    private static string ConnectStringCPON(
      string hostname,
      int port,
      string userid,
      string password,
      string account,
      string service,
      int min_poolsize,
      int max_poolsize)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(" hostname : ");
      stringBuilder.Append(hostname);
      stringBuilder.Append(" port : ");
      stringBuilder.Append(port);
      stringBuilder.Append(" User : ");
      stringBuilder.Append(userid);
      stringBuilder.Append(" Password : ");
      stringBuilder.Append(password);
      stringBuilder.Append(" Account : ");
      stringBuilder.Append(account);
      stringBuilder.Append(" MinPoolsize : ");
      stringBuilder.Append(min_poolsize);
      stringBuilder.Append(" MaxPoolsize : ");
      stringBuilder.Append(max_poolsize);
      stringBuilder.Append(" ConnectionPooling : ");
      stringBuilder.Append(UniObjects.m_UOPooling.ToString());
      stringBuilder.Append(" IdleRemoveExecInterval  : ");
      stringBuilder.Append(UniObjects.m_IdleRemoveExecInterval);
      stringBuilder.Append(" IdleRemoveThreshold   : ");
      stringBuilder.Append(UniObjects.m_IdleRemoveThreshold);
      stringBuilder.Append(" SocketTimeOut   : ");
      stringBuilder.Append(UniObjects.m_SocketTimeout);
      return stringBuilder.ToString();
    }
  }
}
