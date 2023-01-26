// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniRPCConnection
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace IBMU2.UODOTNET
{
  internal class UniRPCConnection : UniRPC
  {
    private bool m_VersionPromotion = true;
    private string m_CurrentHost;
    private IPAddress m_CurrentHostAddress;
    private int m_CurrentPort = -1;
    private int m_CurrentTransportType;
    private char[] m_CurrentService;
    private int m_CurrentTimeoutSeconds = 0;
    private int m_CurrentCompressionThreshold = 0;
    private byte m_CurrentEncryptionType;
    private byte m_LastEncryptionType;
    private bool m_IsActive = false;
    private bool m_IsConnected = false;
    private bool m_IsClient = true;
    private bool m_IsEncryptionEnabled = false;
    private UniRPCSocket m_Socket;
    private BinaryReader m_DataIn;
    private BinaryWriter m_DataOut;
    private UniRPCPacket m_InPacket;
    private UniRPCPacket m_OutPacket;
    private string m_ServerId = "";
    private int m_CurrentHostType = 0;

    ~UniRPCConnection() => this.Dispose(false);

    protected override void Dispose(bool disposing)
    {
      int num = disposing ? 1 : 0;
      base.Dispose(disposing);
    }

    internal UniRPCConnection()
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniRPCConnection::UniRPCConnection()");
      this.m_IsActive = false;
      this.m_IsConnected = false;
      this.m_CurrentTimeoutSeconds = UniObjects.TimeOut;
      this.m_CurrentPort = 31438;
      this.m_CurrentTransportType = 1;
      this.m_CurrentEncryptionType = (byte) 0;
      this.m_InPacket = new UniRPCPacket();
      this.m_OutPacket = new UniRPCPacket();
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniRPCConnection::UniRPCConnection()");
    }

    internal UniRPCConnection(UniRPCConnection aConnection)
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniRPCConnection::UniRPCConnection(UniRPCConnection aConnection)");
      if (aConnection == null || !aConnection.m_IsConnected)
      {
        string errorMessage = UniRPCErrorMessage.GetErrorMessage(81002);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, "UniRPCConnection::UniRPCConnection(UniRPCConnection aConnection)" + errorMessage);
        throw new UniRPCConnectionException(errorMessage);
      }
      this.m_IsActive = false;
      this.m_IsConnected = false;
      this.m_CurrentCompressionThreshold = aConnection.m_CurrentCompressionThreshold;
      this.m_CurrentHost = aConnection.m_CurrentHost;
      this.m_CurrentHostAddress = aConnection.m_CurrentHostAddress;
      this.m_CurrentPort = 31438;
      this.m_CurrentService = aConnection.m_CurrentService;
      this.m_CurrentTimeoutSeconds = aConnection.m_CurrentTimeoutSeconds;
      this.m_CurrentTransportType = aConnection.m_CurrentTransportType;
      this.m_CurrentEncryptionType = aConnection.m_CurrentEncryptionType;
      this.m_DataIn = aConnection.m_DataIn;
      this.m_DataOut = aConnection.m_DataOut;
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniRPCConnection::UniRPCConnection(UniRPCConnection aConnection)");
    }

    public virtual void AcceptConnect(UniRPCSocket aSocket)
    {
      try
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniRPCConnection::AcceptConnect()");
        this.m_Socket = aSocket;
        this.m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, this.m_CurrentTimeoutSeconds * 1000);
        this.m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, (object) new LingerOption(true, this.m_CurrentTimeoutSeconds));
        this.m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Debug, (object) true);
        this.m_DataOut = new BinaryWriter((Stream) this.m_Socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendBuffer));
        this.m_DataIn = new BinaryReader((Stream) this.m_Socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer));
        this.m_CurrentPort = ((IPEndPoint) this.m_Socket.LocalEndPoint).Port;
        this.m_CurrentTransportType = 1;
      }
      catch (SocketException ex)
      {
        string message = "System Error : " + ex.Message + "  and UODOTNET Error : " + UniRPCErrorMessage.GetErrorMessage(81002);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniRPCConnection::AcceptConnect()" + message);
        throw new UniRPCConnectionException(message);
      }
      catch (IOException ex)
      {
        string message = "System Error : " + ex.Message + "  and UODOTNET Error : " + UniRPCErrorMessage.GetErrorMessage(81002);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniRPCConnection::AcceptConnect()" + message);
        throw new UniRPCConnectionException(message);
      }
      this.m_IsActive = true;
      this.m_IsConnected = true;
      this.m_IsClient = false;
      this.m_LastEncryptionType = (byte) 0;
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniRPCConnection::AcceptConnect()");
    }

    public void WritePacket(UniRPCPacket anOutPacket)
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniRPCConnection::WritePacket");
      if (this.m_IsClient)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniRPCConnection::WritePacketm_IsClient is true");
        this.WritePacketInternal(this.m_CurrentEncryptionType);
      }
      else
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniRPCConnection::WritePacketm_IsClient is false");
        lock (this)
          this.WritePacketInternal(this.m_LastEncryptionType);
      }
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniRPCConnection::WritePacket");
    }

    public void ReadPacket(UniRPCPacket anInPacket)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniRPCConnection::ReadPacket( UniRPCPacket anInPacket )");
        this.ReadPacketInternal();
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniRPCConnection::ReadPacket( UniRPCPacket anInPacket )");
      }
    }

    public void Call()
    {
      if (this.m_IsEncryptionEnabled)
        this.Call(this.m_CurrentEncryptionType);
      else
        this.Call((byte) 0);
    }

    public void Call(byte anEncryptionType)
    {
      lock (this)
      {
        this.WritePacketInternal(anEncryptionType);
        this.ReadPacketInternal();
      }
    }

    public void Close()
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniRPCConnection::Close()");
      try
      {
        if (this.m_Socket.Connected)
        {
          this.m_Socket.Shutdown(SocketShutdown.Both);
          this.m_Socket.Close();
        }
      }
      catch (IOException ex)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniRPCConnection::Close() " + (ex.Message + UniRPCErrorMessage.GetErrorMessage(81001)));
        throw new UniRPCConnectionException();
      }
      this.m_IsConnected = false;
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniRPCConnection::Close()");
    }

    public void ConnectInternal(int responseIndex)
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniRPCConnection::ConnectInternal(int responseIndex)" + (object) responseIndex);
      try
      {
        IPEndPoint remoteEP = new IPEndPoint(this.m_CurrentHostAddress, this.m_CurrentPort);
        this.m_Socket = new UniRPCSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        this.m_Socket.Connect((EndPoint) remoteEP);
        this.m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, this.m_CurrentTimeoutSeconds * 1000);
        this.m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, (object) new LingerOption(true, this.m_CurrentTimeoutSeconds));
        this.m_Socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.Debug, 1);
        this.m_DataOut = new BinaryWriter((Stream) new NetworkStream((Socket) this.m_Socket));
        this.m_DataIn = new BinaryReader((Stream) new NetworkStream((Socket) this.m_Socket));
        this.m_IsClient = true;
        lock (this.m_Socket)
        {
          this.m_OutPacket.UniRPCPMessage.WriteCharArray(0, this.m_CurrentService);
          this.WritePacketInternal((byte) 0);
          this.ReadPacketInternal();
        }
        int num;
        if (this.m_InPacket.UniRPCPMessage.ArgumentCount == 2)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniRPCConnection::ConnectInternal(int responseIndex)m_InPacket.UniRPCPMessage.ArgumentCount is 2");
          num = this.m_InPacket.UniRPCPMessage.ReadInteger(1);
          this.m_ServerId = new string(Encoding.Default.GetChars(this.m_InPacket.UniRPCPMessage.ReadByteArray(0)));
        }
        else
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniRPCConnection::ConnectInternal(int responseIndex)m_InPacket.UniRPCPMessage.ArgumentCount is " + (object) responseIndex);
          num = this.m_InPacket.UniRPCPMessage.ReadInteger(responseIndex);
        }
        if (num == 0)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniRPCConnection::ConnectInternal(int responseIndex)response is 0");
          this.m_IsConnected = true;
          this.m_IsActive = true;
        }
        else
        {
          string message = "UniRPCConnection failed to establish a connection with the remote host." + UniRPCErrorMessage.GetErrorMessage(81002);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, "UniRPCConnection::ConnectInternal(int responseIndex) " + message);
          throw new UniRPCConnectionException(message);
        }
      }
      catch (IOException ex)
      {
        string message = ex.Message + UniRPCErrorMessage.GetErrorMessage(81002);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, "UniRPCConnection::ConnectInternal(int responseIndex) " + message);
        throw new UniRPCConnectionException(message);
      }
      catch (UniRPCPacketException ex)
      {
        string message = ex.Message + UniRPCErrorMessage.GetErrorMessage(81002);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, "UniRPCConnection::ConnectInternal(int responseIndex) " + message);
        throw new UniRPCConnectionException(message);
      }
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniRPCConnection::ConnectInternal(int responseIndex)");
    }

    public void Connect()
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniRPCConnection::Connect()");
      this.ConnectInternal(0);
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniRPCConnection::Connect()");
    }

    public void Connect(int responseIndex)
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniRPCConnection::Connect(int responseIndex)");
      this.ConnectInternal(responseIndex);
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniRPCConnection::Connect(int responseIndex)");
    }

    public void DisableEncryption()
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniRPCConnection::DisableEncryption()");
      this.m_IsEncryptionEnabled = false;
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniRPCConnection::DisableEncryption()");
    }

    public void EnableEncryption()
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniRPCConnection::EnableEncryption()");
      this.m_IsEncryptionEnabled = true;
      if (this.IsEncryptionSupported())
      {
        this.m_IsEncryptionEnabled = true;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniRPCConnection::EnableEncryption()");
      }
      else
      {
        string errorMessage = UniRPCErrorMessage.GetErrorMessage(81025);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniRPCConnection::EnableEncryption() " + errorMessage);
        throw new UniRPCConnectionException(errorMessage);
      }
    }

    private void WritePacketInternal(byte anEncryptionType)
    {
      if (anEncryptionType != (byte) 0)
      {
        if (this.IsEncryptionSupported())
        {
          if (anEncryptionType == (byte) 1)
            goto label_4;
        }
        string message = "Encryption or this type of encryption is not supported on this connection." + UniRPCErrorMessage.GetErrorMessage(81025);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, "UniRPCConnection::WritePacketInternal() " + message);
        throw new UniRPCConnectionException(message);
      }
label_4:
      try
      {
        this.m_OutPacket.UniRPCPMessage.Send(this.m_DataOut, anEncryptionType);
      }
      catch (UniRPCPacketException ex)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, "UniRPCConnection::WritePacketInternal() " + ex.Message + ex.StackTrace);
        throw new UniRPCConnectionException(ex.Message);
      }
    }

    private void ReadPacketInternal()
    {
      try
      {
        this.m_InPacket.UniRPCPMessage.Receive(this.m_DataIn);
        if (!this.m_IsClient)
          this.m_LastEncryptionType = this.m_InPacket.UniRPCPMessage.ReadEncryptionMask();
        if (this.m_VersionPromotion)
        {
          int num1 = this.m_InPacket.UniRPCPMessage.ReadHeaderVersion();
          int num2 = this.m_InPacket.UniRPCPMessage.ReadHeaderVersionHighest();
          if (2 <= num2)
          {
            if (2 < num1)
              throw new UniRPCConnectionException("Incompatible RPC transport layers" + UniRPCErrorMessage.GetErrorMessage(81005));
            this.CurrentVersion = 2;
          }
          else if (1 <= num2)
            this.CurrentVersion = num2;
          else
            this.CurrentVersion = 1;
        }
        this.m_VersionPromotion = false;
      }
      catch (UniRPCPacketException ex)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniRPCConnection::ReadPacketInternal()" + ex.Message + ex.StackTrace);
        throw new UniRPCConnectionException(ex.Message);
      }
    }

    public UniRPCPacket OutPacket => this.m_OutPacket;

    public UniRPCPacket InPacket => this.m_InPacket;

    public int CompressionThreshold
    {
      get => this.m_CurrentCompressionThreshold;
      set => this.m_CurrentCompressionThreshold = value;
    }

    public bool Connected => this.m_IsConnected;

    public string Host
    {
      get => this.m_CurrentHost;
      set
      {
        if (this.Connected)
        {
          string message = "This method can't be used on a connected connection" + UniRPCErrorMessage.GetErrorMessage(81023);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, "Set Property UniRPCConnection::Host" + message);
          throw new UniRPCConnectionException(message);
        }
        this.m_CurrentHost = new string(value.ToCharArray());
        try
        {
          try
          {
            this.m_CurrentHostAddress = Dns.GetHostByName(this.m_CurrentHost).AddressList[0];
          }
          catch (SocketException ex)
          {
            string message = "SocketException caught!!!" + ex.Source + ex.Message;
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, "Set Property UniRPCConnection::Host" + message);
            throw new UniRPCConnectionException(message);
          }
          catch (ArgumentNullException ex)
          {
            string message = "ArgumentNullException caught!!!" + ex.Source + ex.Message;
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, "Set Property UniRPCConnection::Host" + message);
            throw new UniRPCConnectionException(message);
          }
          catch (Exception ex)
          {
            string message = "Exception caught!!!" + ex.Source + ex.Message;
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, "Set Property UniRPCConnection::Host" + message);
            throw new UniRPCConnectionException(message);
          }
        }
        catch (Exception ex)
        {
          string message = ex.Message + UniRPCErrorMessage.GetErrorMessage(81011);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, "Set Property UniRPCConnection::Host" + message);
          throw new UniRPCConnectionException(message);
        }
      }
    }

    public int Port
    {
      get => this.m_CurrentPort;
      set
      {
        if (this.Connected)
        {
          string message = "This method can't be used on a connected connection" + UniRPCErrorMessage.GetErrorMessage(81023);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, "Set Property UniRPCConnection::Port" + message);
          throw new UniRPCConnectionException(message);
        }
        this.m_CurrentPort = value;
      }
    }

    public int TransportType
    {
      get => this.m_CurrentTransportType;
      set
      {
        if (this.Connected)
        {
          string message = "This method can't be used on a connected connection" + UniRPCErrorMessage.GetErrorMessage(81023);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, "Set Property UniRPCConnection::TransportType" + message);
          throw new UniRPCConnectionException(message);
        }
        this.m_CurrentTransportType = value;
      }
    }

    public string Service
    {
      get => new string(this.m_CurrentService);
      set
      {
        if (this.Connected)
        {
          string message = "This method can't be used on a connected connection" + UniRPCErrorMessage.GetErrorMessage(81023);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, "Set Property UniRPCConnection::Service" + message);
          throw new UniRPCConnectionException(message);
        }
        this.m_CurrentService = value.ToCharArray();
      }
    }

    public int TimeoutSeconds
    {
      get => this.m_CurrentTimeoutSeconds;
      set
      {
        this.m_CurrentTimeoutSeconds = value;
        try
        {
          if (this.m_Socket == null)
            return;
          this.m_Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, this.m_CurrentTimeoutSeconds * 1000);
        }
        catch (SocketException ex)
        {
          string message = ex.Message + UniRPCErrorMessage.GetErrorMessage(81009);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, "Set Property UniRPCConnection::TimeoutSeconds" + message);
          throw new UniRPCConnectionException(message);
        }
      }
    }

    public bool IsServerAlive
    {
      get
      {
        try
        {
          if (this.m_Socket != null)
          {
            if (this.m_CurrentHostType == 2)
            {
              this.m_Socket.Send(new byte[0], 0, 0, SocketFlags.None);
            }
            else
            {
              byte[] buffer = new byte[1];
              int num = this.m_Socket.Blocking ? 1 : 0;
              this.m_Socket.Blocking = false;
              this.m_Socket.Send(buffer, 0, 1, SocketFlags.OutOfBand);
              this.m_Socket.Receive(buffer, 0, 1, SocketFlags.OutOfBand);
              this.m_Socket.Blocking = true;
            }
          }
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniRPCConnection::IsSocketAlive  propertySend Keep Alive Ping OK , return true");
          return true;
        }
        catch (SocketException ex)
        {
          if (ex.ErrorCode == 10054)
          {
            this.m_Socket.Blocking = true;
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniRPCConnection::IsSocketAlive  propertySend Keep Alive Ping NOT OK , caught SocketException, return false. Error Code:" + (object) ex.ErrorCode);
            return false;
          }
          if (ex.ErrorCode != 10053)
            return true;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniRPCConnection::IsSocketAlive  propertySend Keep Alive Ping NOT OK , caught SocketException, return false. Error Code:" + (object) ex.ErrorCode);
          return false;
        }
      }
    }

    public byte EncryptionType
    {
      get => this.m_CurrentEncryptionType;
      set
      {
        if (value == (byte) 0 || value == (byte) 1)
        {
          this.m_CurrentEncryptionType = value;
        }
        else
        {
          string message = "This type of encryption is not supported on this connection" + UniRPCErrorMessage.GetErrorMessage(81025);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, "Set Property UniRPCConnection::EncryptionType" + message);
          throw new UniRPCConnectionException(message);
        }
      }
    }

    public bool IsActive => this.m_IsActive;

    internal string ServerId => this.m_ServerId;

    internal int HostType
    {
      set => this.m_CurrentHostType = value;
    }
  }
}
