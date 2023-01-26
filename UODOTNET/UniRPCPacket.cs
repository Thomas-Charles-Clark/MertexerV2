// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniRPCPacket
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

namespace IBMU2.UODOTNET
{
  internal class UniRPCPacket : UniRPC
  {
    public const int m_UNIRPC_INT = 0;
    public const int m_UNIRPC_DOUBLE = 1;
    public const int m_UNIRPC_CHAR = 2;
    public const int m_UNIRPC_STRING = 3;
    public const int m_UNIRPC_INT_PTR = 4;
    public const int m_UNIRPC_DOUBLE_PTR = 5;
    public const int m_UNIRPC_FUNCNAME = 6;
    private UniRPCConnection m_CurrentConnection = (UniRPCConnection) null;
    private UniRPCPMessage m_MessageObject = (UniRPCPMessage) null;

    internal UniRPCPacket()
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniRPCPacket::UniRPCPacket()");
      this.m_MessageObject = new UniRPCPMessage();
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniRPCPacket::UniRPCPacket()");
    }

    public UniRPCPacket(UniRPCConnection aConnection)
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniRPCPacket( UniRPCConnection aConnection)");
      if (aConnection == null || !aConnection.Connected)
      {
        string message = "An open connection is required for this constructor." + UniRPCErrorMessage.GetErrorMessage(81002);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, "UniRPCPacket( UniRPCConnection aConnection) " + message);
        throw new UniRPCPacketException(message);
      }
      this.m_CurrentConnection = aConnection;
      this.CurrentVersion = aConnection.Version;
      this.m_MessageObject = new UniRPCPMessage();
      this.m_MessageObject.CurrentVersion = aConnection.CurrentVersion;
      this.m_MessageObject.CompressionThreshold = aConnection.CompressionThreshold;
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniRPCPacket( UniRPCConnection aConnection)");
    }

    public UniRPCPMessage UniRPCPMessage
    {
      get => this.m_MessageObject;
      set => this.m_MessageObject = value;
    }

    public UniRPCConnection UniRPCConnection => this.m_CurrentConnection;
  }
}
