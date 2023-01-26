// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniDynArrayException
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

using System;

namespace IBMU2.UODOTNET
{
  public class UniDynArrayException : ApplicationException
  {
    public UniDynArrayException() => this.Source = "UniDynArray Class";

    public UniDynArrayException(string message)
      : base(message)
    {
      this.Source = "UniDynArray Class";
    }

    public UniDynArrayException(string message, Exception inner)
      : base(message, inner)
    {
      this.Source = "UniDynArray Class";
    }
  }
}
