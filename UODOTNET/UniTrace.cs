// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniTrace
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace IBMU2.UODOTNET
{
  internal sealed class UniTrace : UniRoot
  {
    private BooleanSwitch m_UniBooleanSwitch = (BooleanSwitch) null;
    private TraceSwitch m_UniTraceSwitch = (TraceSwitch) null;
    private TextWriterTraceListener m_UniListener = (TextWriterTraceListener) null;
    private int m_RPCDUMP = 0;

    public int RPCDUMP
    {
      set => this.m_RPCDUMP = value;
      get => this.m_RPCDUMP;
    }

    internal void UniLog(UniRoot.UniTraceLevel TraceLevel, string message)
    {
      try
      {
        if (this.m_UniTraceSwitch == null)
          return;
        message = "ThreadID: " + (object) Thread.CurrentThread.GetHashCode() + " Name :" + Thread.CurrentThread.Name + " " + message;
        switch (TraceLevel)
        {
          case UniRoot.UniTraceLevel.UO_ERROR:
            Trace.WriteLineIf(this.m_UniTraceSwitch.TraceError, message);
            break;
          case UniRoot.UniTraceLevel.UO_WARNING:
            Trace.WriteLineIf(this.m_UniTraceSwitch.TraceWarning, message);
            break;
          case UniRoot.UniTraceLevel.UO_INFO:
            Trace.WriteLineIf(this.m_UniTraceSwitch.TraceInfo, message);
            break;
          case UniRoot.UniTraceLevel.UO_VERBOSE:
            Trace.WriteLineIf(this.m_UniTraceSwitch.TraceVerbose, message);
            break;
          default:
            Trace.WriteLineIf(this.m_UniTraceSwitch.TraceInfo, message);
            break;
        }
      }
      catch (ObjectDisposedException ex)
      {
      }
    }

    internal UniTrace()
    {
      try
      {
        if (this.m_UniBooleanSwitch == null)
          this.m_UniBooleanSwitch = new BooleanSwitch("UniBooleanSwitch", "UniBooleanSwitch Operations");
        if (this.m_UniTraceSwitch == null)
        {
          this.m_UniTraceSwitch = new TraceSwitch("UniTraceSwitch", "UniTraceSwitch Operations");
          if (this.m_UniTraceSwitch.Level != TraceLevel.Off)
          {
            Trace.WriteLine("===================================================");
            Trace.Write("Trace started : ");
            Trace.WriteLine((object) DateTime.Now);
            Trace.WriteLine("===================================================");
          }
        }
        if (this.m_UniListener != null || Trace.Listeners.Count != 1 || this.m_UniTraceSwitch.Level == TraceLevel.Off)
          return;
        this.m_UniListener = new TextWriterTraceListener((Stream) new FileStream(Path.GetTempPath() + "\\" + UniObjects.UniResourceManager.GetString("TRACE_FILE_NAME"), FileMode.Create, FileAccess.Write, FileShare.ReadWrite));
        if (Trace.Listeners.Contains((TraceListener) this.m_UniListener))
          return;
        Trace.Listeners.Add((TraceListener) this.m_UniListener);
      }
      catch (Exception ex)
      {
        this.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message + ex.StackTrace);
      }
    }
  }
}
