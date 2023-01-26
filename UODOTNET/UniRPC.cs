// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniRPC
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

namespace IBMU2.UODOTNET
{
  internal class UniRPC : UniRoot
  {
    protected const int m_UNIRPC_VERSION = 2;
    protected const int m_UNIRPC_VERSION_LOWEST = 1;
    private int m_CurrentVersion;

    internal UniRPC() => this.m_CurrentVersion = 1;

    public int CurrentVersion
    {
      get => this.m_CurrentVersion;
      set => this.m_CurrentVersion = value;
    }

    public int Version => 2;

    public bool IsCompressionSupported() => false;

    public bool IsEncryptionSupported() => this.m_CurrentVersion > 1;
  }
}
