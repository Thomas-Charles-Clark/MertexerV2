// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniRPCSocket
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

using System.Net.Sockets;

namespace IBMU2.UODOTNET
{
  internal class UniRPCSocket : Socket
  {
    protected internal UniRPCSocket(AddressFamily a, SocketType b, ProtocolType p)
      : base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
    {
    }
  }
}
