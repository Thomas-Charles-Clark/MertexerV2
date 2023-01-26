// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniPerfCounter
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

using System;
using System.Diagnostics;

namespace IBMU2.UODOTNET
{
  internal sealed class UniPerfCounter : UniRoot
  {
    private PerformanceCounter m_PooledConnections = (PerformanceCounter) null;
    private PerformanceCounter m_ConnectionPools = (PerformanceCounter) null;
    private PerformanceCounter m_FailedConnects = (PerformanceCounter) null;
    private PerformanceCounter m_AverageWaitTime = (PerformanceCounter) null;
    private PerformanceCounter m_AverageWaitTimeBase = (PerformanceCounter) null;
    private CounterCreationData m_Counter1 = (CounterCreationData) null;
    private CounterCreationData m_Counter2 = (CounterCreationData) null;
    private CounterCreationData m_Counter3 = (CounterCreationData) null;
    private CounterCreationData m_Counter4 = (CounterCreationData) null;
    private CounterCreationData m_Counter5 = (CounterCreationData) null;
    private CounterCreationDataCollection m_CounterDatas = (CounterCreationDataCollection) null;

    public UniPerfCounter()
    {
      try
      {
        PerformanceCounterCategory.Delete("UODOTNET Category");
      }
      catch (Exception ex)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniPerfCounter::PerformanceCounterCategory.Delete() failed due to" + ex.Message);
      }
      if (!PerformanceCounterCategory.Exists("UODOTNET Category"))
      {
        this.m_CounterDatas = new CounterCreationDataCollection();
        this.m_Counter1 = new CounterCreationData();
        this.m_Counter1.CounterName = "Current # pooled connections";
        this.m_Counter1.CounterHelp = "help string 1";
        this.m_Counter1.CounterType = PerformanceCounterType.NumberOfItems64;
        this.m_Counter2 = new CounterCreationData();
        this.m_Counter2.CounterName = "Current # connection pools";
        this.m_Counter2.CounterHelp = "help string 2";
        this.m_Counter2.CounterType = PerformanceCounterType.NumberOfItems64;
        this.m_Counter3 = new CounterCreationData();
        this.m_Counter3.CounterName = "Total # failed connects";
        this.m_Counter3.CounterHelp = "help string 3";
        this.m_Counter3.CounterType = PerformanceCounterType.NumberOfItems64;
        this.m_Counter4 = new CounterCreationData();
        this.m_Counter4.CounterName = "Average Wait Time";
        this.m_Counter4.CounterHelp = "help string 4";
        this.m_Counter4.CounterType = PerformanceCounterType.AverageTimer32;
        this.m_Counter5 = new CounterCreationData();
        this.m_Counter5.CounterName = "Average Wait Time Base";
        this.m_Counter5.CounterHelp = "help string 5";
        this.m_Counter5.CounterType = PerformanceCounterType.AverageBase;
        this.m_CounterDatas.Add(this.m_Counter1);
        this.m_CounterDatas.Add(this.m_Counter2);
        this.m_CounterDatas.Add(this.m_Counter3);
        this.m_CounterDatas.Add(this.m_Counter4);
        this.m_CounterDatas.Add(this.m_Counter5);
        PerformanceCounterCategory.Create("UODOTNET Category", "UODOTNET help", this.m_CounterDatas);
      }
      this.m_PooledConnections = new PerformanceCounter("UODOTNET Category", "Current # pooled connections", false);
      this.m_PooledConnections.RawValue = 0L;
      this.m_ConnectionPools = new PerformanceCounter("UODOTNET Category", "Current # connection pools", false);
      this.m_ConnectionPools.RawValue = 0L;
      this.m_FailedConnects = new PerformanceCounter("UODOTNET Category", "Total # failed connects", false);
      this.m_FailedConnects.RawValue = 0L;
      this.m_AverageWaitTime = new PerformanceCounter("UODOTNET Category", "Average Wait Time", false);
      this.m_AverageWaitTimeBase = new PerformanceCounter("UODOTNET Category", "Average Wait Time Base", false);
    }

    public PerformanceCounter ConnectionPools => this.m_ConnectionPools;

    public PerformanceCounter AverageWaitTime => this.m_AverageWaitTime;

    public PerformanceCounter AverageWaitTimeBase => this.m_AverageWaitTimeBase;

    public PerformanceCounter PooledConnections => this.m_PooledConnections;

    public PerformanceCounter FailedConnects => this.m_FailedConnects;
  }
}
