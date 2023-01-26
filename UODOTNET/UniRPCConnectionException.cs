// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniRPCConnectionException
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

using System;

namespace IBMU2.UODOTNET
{
  public class UniRPCConnectionException : ApplicationException
  {
    public UniRPCConnectionException() => this.Source = "UniRPCConnection Class";

    public UniRPCConnectionException(string message)
      : base(message)
    {
      this.Source = "UniRPCConnection Class";
    }

    public UniRPCConnectionException(string message, Exception inner)
      : base(message, inner)
    {
      this.Source = "UniRPCConnection Class";
    }
  }
}
