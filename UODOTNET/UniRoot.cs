// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniRoot
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

using System;

namespace IBMU2.UODOTNET
{
  public abstract class UniRoot : IDisposable
  {
    public void Dispose() => this.Dispose(true);

    protected virtual void Dispose(bool disposing)
    {
      int num = disposing ? 1 : 0;
    }

    ~UniRoot() => this.Dispose(false);

    internal static void UniLog(UniRoot.UniTraceLevel TraceLevel, string message) => UniObjects.Trace.UniLog(TraceLevel, message);

    internal enum UniTraceLevel
    {
      UO_ERROR = 1,
      UO_WARNING = 2,
      UO_INFO = 3,
      UO_VERBOSE = 4,
    }
  }
}
