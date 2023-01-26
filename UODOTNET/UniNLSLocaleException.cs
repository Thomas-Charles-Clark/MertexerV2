// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniNLSLocaleException
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

using System;

namespace IBMU2.UODOTNET
{
  public class UniNLSLocaleException : ApplicationException
  {
    public UniNLSLocaleException() => this.Source = Environment.NewLine + "UniNLSLocale Class";

    public UniNLSLocaleException(string message)
      : base(message)
    {
      this.Source = Environment.NewLine + "UniNLSLocale Class";
    }

    public UniNLSLocaleException(string message, Exception inner)
      : base(message, inner)
    {
      this.Source = Environment.NewLine + "UniNLSLocale Class";
    }
  }
}
