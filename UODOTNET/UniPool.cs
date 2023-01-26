// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniPool
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

using System;
using System.Collections;
using System.Threading;

namespace IBMU2.UODOTNET
{
  internal sealed class UniPool : UniRoot
  {
    private const int m_DefaultRetryNewInTimes = 10;
    private string m_Service;
    private string m_Userid = "";
    private string m_Password = "";
    private string m_Server = "";
    private string m_Account = "";
    private int m_MinPoolSize = 0;
    private int m_MaxPoolSize = 0;
    private ArrayList m_AvailableConnections = (ArrayList) null;
    private ArrayList m_BusyConnections = (ArrayList) null;
    private int m_RetryNewInTimes = 0;
    private int m_NextRetryNewInTimes = 0;
    private int m_PortNumber = 31438;

    internal UniPool(
      string hostname,
      string userid,
      string password,
      string account,
      string service,
      int min_poolsize,
      int max_poolsize,
      int port_number)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniPool::UniPool()");
        this.m_Userid = userid;
        this.m_Password = password;
        this.m_Server = hostname;
        this.m_Account = account;
        this.m_Service = service;
        this.m_PortNumber = port_number;
        if (this.m_AvailableConnections == null && this.m_BusyConnections == null)
        {
          this.m_MinPoolSize = min_poolsize;
          this.m_MaxPoolSize = max_poolsize;
          if (this.m_MinPoolSize > this.m_MaxPoolSize)
            this.m_MinPoolSize = this.m_MaxPoolSize;
          this.m_AvailableConnections = new ArrayList(this.m_MinPoolSize);
          this.m_BusyConnections = new ArrayList();
          int num = 0;
          try
          {
            for (num = 0; num < this.m_MinPoolSize; ++num)
              this.m_AvailableConnections.Add((object) this.MakeNewConnection());
          }
          catch (Exception ex)
          {
            for (int index = 0; index < this.m_AvailableConnections.Count; ++index)
              ((UniSession) this.m_AvailableConnections[index]).Disconnect();
            if (num > 0 && num < this.m_MinPoolSize)
            {
              string errorMessage = UniErrorMessage.GetErrorMessage(93);
              Exception exception = new Exception(errorMessage);
              exception.Source = Environment.NewLine + "UniPool Class";
              UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniPool::UniPool()" + errorMessage);
              throw exception;
            }
            throw ex;
          }
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniPool::UniPool()");
      }
    }

    ~UniPool()
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniPool::~UniPool()");
      UniObjects.PerformanceCounter.ConnectionPools.Decrement();
      this.Dispose(false);
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniPool::~UniPool()");
    }

    internal UniSession FindSession(int pTimeout)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniPool::FindSession()");
        if (this.m_AvailableConnections.Count != 0)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniPool::FindSession()AvailableConnections is " + (object) this.m_AvailableConnections.Count);
          UniSession availableConnection = (UniSession) this.m_AvailableConnections[this.m_AvailableConnections.Count - 1];
          this.m_AvailableConnections.RemoveAt(this.m_AvailableConnections.Count - 1);
          if (availableConnection.UniRPCConnection.IsServerAlive)
          {
            this.m_BusyConnections.Add((object) availableConnection.UniRPCConnection);
            return availableConnection;
          }
          Monitor.PulseAll((object) this);
          return this.FindSession(pTimeout);
        }
        if (this.TotalConnections() < this.m_MaxPoolSize)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniPool::FindSession()AvailableConnections is " + (object) this.m_AvailableConnections.Count + "  TotalConnections()  < m_MaxPoolSize");
          if (this.m_RetryNewInTimes == 0)
          {
            try
            {
              UniSession session = this.MakeNewConnection();
              this.m_BusyConnections.Add((object) session.UniRPCConnection);
              this.m_NextRetryNewInTimes = 10;
              return session;
            }
            catch (Exception ex)
            {
              this.m_RetryNewInTimes = this.m_NextRetryNewInTimes;
              this.m_NextRetryNewInTimes *= 2;
              UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniPool::FindSession():MakeNewConnection() failed due to " + ex.Message);
            }
          }
          else
            --this.m_RetryNewInTimes;
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniPool::FindSession()Before Calling Wait()");
        DateTime now = DateTime.Now;
        bool flag = Monitor.Wait((object) this, pTimeout);
        TimeSpan timeSpan = DateTime.Now - now;
        UniObjects.PerformanceCounter.AverageWaitTime.IncrementBy(timeSpan.Ticks * 10L);
        UniObjects.PerformanceCounter.AverageWaitTimeBase.Increment();
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniPool::FindSession()After Calling Wait()");
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniPool::FindSession()DiffTime" + timeSpan.ToString() + "  Ticks " + (object) timeSpan.Ticks);
        if (flag)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniPool::FindSession()Wait() returned true");
          return this.FindSession(pTimeout - (int) timeSpan.TotalMilliseconds);
        }
        UniObjects.PerformanceCounter.FailedConnects.Increment();
        string errorMessage = UniErrorMessage.GetErrorMessage(81015);
        Exception exception = new Exception(errorMessage);
        exception.Source = Environment.NewLine + "UniPool Class";
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniPool::FindSession()" + errorMessage);
        throw exception;
      }
    }

    internal void Free(UniSession us)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter  UniPool:: Free() ");
        if (this.m_AvailableConnections.Contains((object) us) || !us.IsActive)
          return;
        if (us.IsActive && us.IsRPCError)
        {
          this.CloseServerConnection(us);
        }
        else
        {
          if (us.UniCommand != null)
          {
            switch (us.UniCommand.CommandStatus)
            {
              case 1:
              case 2:
                string message1 = UniErrorMessage.GetErrorMessage(30114) + " : Command Active";
                Exception exception1 = new Exception(message1);
                exception1.Source = Environment.NewLine + "UniPool Class";
                UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniPool::Free()" + message1);
                throw exception1;
            }
          }
          if (us.UniTransaction != null && us.UniTransaction.IsActive())
          {
            string message2 = UniErrorMessage.GetErrorMessage(30114) + " : Transaction Active";
            Exception exception2 = new Exception(message2);
            exception2.Source = Environment.NewLine + "UniPool Class";
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniPool::Free()" + message2);
            throw exception2;
          }
          this.m_BusyConnections.Remove((object) us.UniRPCConnection);
          us.FreedTime = DateTime.Now;
          UniSession uniSession = (UniSession) us.ClonePooled();
          us.InitDefaultValues();
          this.m_AvailableConnections.Add((object) uniSession);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniPool:: Free() Calling PulseAll()");
          Monitor.PulseAll((object) this);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit  UniPool:: Free() ");
        }
      }
    }

    internal void CloseIdleConnections()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter  UniPool:: CloseIdleConnections() ");
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniPool:: CloseIdleConnections() There are " + (object) this.m_AvailableConnections.Count + "free connections");
        for (int index = 0; index < this.m_AvailableConnections.Count; ++index)
        {
          UniSession availableConnection = (UniSession) this.m_AvailableConnections[index];
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniPool::CloseIdleConnections() FreedTime" + (object) availableConnection.FreedTime);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniPool::CloseIdleConnections() DateTime.Now" + (object) DateTime.Now);
          DateTime freedTime = availableConnection.FreedTime;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniPool::CloseIdleConnections() CalculatedDateTime" + (object) freedTime.AddMilliseconds((double) UniObjects.IdleRemoveThreshold));
          DateTime now = DateTime.Now;
          freedTime = availableConnection.FreedTime;
          DateTime t2 = freedTime.AddMilliseconds((double) UniObjects.IdleRemoveThreshold);
          int num = DateTime.Compare(now, t2);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniPool::CloseIdleConnections() diff = " + (object) num);
          if (num >= 0 && this.m_AvailableConnections.Count > this.m_MinPoolSize)
          {
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniPool::CloseIdleConnections() Calling Disconnect() for sessions " + availableConnection.UserName + availableConnection.Password + availableConnection.HostName + availableConnection.Account);
            availableConnection.Disconnect();
            this.m_AvailableConnections.Remove((object) availableConnection);
            --index;
          }
          else
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniPool::CloseIdleConnections() : Fail to disconnect()");
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit  UniPool:: CloseIdleConnections() ");
      }
    }

    internal void CloseServerConnection(UniSession us)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter  UniPool:: CloseServerConnection() ");
        this.m_BusyConnections.Remove((object) us.UniRPCConnection);
        us.Disconnect();
        this.AdjustMinPoolSizeConnections();
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter  UniPool:: CloseIdleConnections() ");
      }
    }

    internal void CloseAllConnections()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniPool::CloseAllConnections()");
        this.CloseConnections(this.m_AvailableConnections);
        this.m_AvailableConnections.Clear();
        this.CloseBusyConnections(this.m_BusyConnections);
        this.m_BusyConnections.Clear();
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniPool::CloseAllConnections()");
      }
    }

    internal void AdjustMinPoolSizeConnections()
    {
      lock (this)
      {
        if (this.TotalConnections() >= this.m_MinPoolSize)
          return;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniPool::AdjustMinPoolSizeConnections()");
        UniSession uniSession = (UniSession) null;
        try
        {
          uniSession = this.MakeNewConnection();
        }
        catch (Exception ex)
        {
        }
        this.m_AvailableConnections.Add((object) uniSession);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniPool::AdjustMinPoolSizeConnections()");
      }
    }

    private int TotalConnections()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniPool::TotalConnections()");
        int num = this.m_AvailableConnections.Count + this.m_BusyConnections.Count;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniPool::TotalConnections()");
        return num;
      }
    }

    private UniSession MakeNewConnection()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniPool::MakeNewConnection()");
        UniSession uniSession = new UniSession();
        uniSession.IsPooledSession = true;
        uniSession.SetServiceType(this.m_Service);
        uniSession.Connect(this.m_Server, this.m_PortNumber, this.m_Userid, this.m_Password, this.m_Account);
        uniSession.CreationTime = DateTime.Now;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniPool::MakeNewConnection()");
        UniObjects.PerformanceCounter.PooledConnections.Increment();
        return uniSession;
      }
    }

    private void CloseConnections(ArrayList pUniSessions)
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniPool::CloseConnections()");
      if (pUniSessions == null)
        return;
      for (int index = 0; index < pUniSessions.Count; ++index)
      {
        UniSession pUniSession = (UniSession) pUniSessions[index];
        if (pUniSession.IsActive)
          pUniSession.Disconnect();
      }
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniPool::CloseConnections()");
    }

    private void CloseBusyConnections(ArrayList pRPCConnections)
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniPool::CloseBusyConnections()");
      if (pRPCConnections == null)
        return;
      for (int index = 0; index < pRPCConnections.Count; ++index)
        ((UniRPCConnection) pRPCConnections[index]).Close();
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniPool::CloseBusyConnections()");
    }

    internal string User => this.m_Userid;

    internal string Password => this.m_Password;

    internal string Server => this.m_Server;

    internal string Account => this.m_Account;

    internal int MaxPoolSize => this.m_MaxPoolSize;

    internal int MinPoolSize => this.m_MinPoolSize;
  }
}
