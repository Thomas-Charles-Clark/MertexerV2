// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniNLSMap
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

namespace IBMU2.UODOTNET
{
  public class UniNLSMap : UniRoot
  {
    private byte[] m_NLSMarkChars;
    private string m_ClientName;
    private UniSession m_Session = (UniSession) null;
    private int m_UniNLSMapStatus = 0;
    private int m_UniReturnCode = -1;
    private int m_FileEncryptionType = 0;

    internal UniNLSMap(UniSession aSession)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniNLSMap::UniNLSMap(UniSession aSession)");
        this.m_Session = aSession;
        this.m_NLSMarkChars = new byte[6];
        this.m_ClientName = string.Empty;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniNLSMap::UniNLSMap(UniSession aSession)");
      }
    }

    public string ServerMapName
    {
      get
      {
        lock (this)
        {
          this.CheckEntryConditions();
          try
          {
            UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
            this.m_Session.UniRPCConnection.OutPacket.UniRPCPMessage.WriteInteger(0, 71);
            this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
            this.m_UniReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
            if (this.m_UniReturnCode == 0)
            {
              string uniCodeString = this.m_Session.ByteArrayToUniCodeString(inPacket.UniRPCPMessage.ReadByteArray(1));
              UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniNLSMap::ServerMapName property " + uniCodeString);
              return uniCodeString;
            }
            UniNLSMapException uniNlsMapException = new UniNLSMapException(UniErrorMessage.GetErrorMessage(this.m_UniReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniNlsMapException.Message + " Source: " + uniNlsMapException.Source + " Method: " + (object) uniNlsMapException.TargetSite);
            throw uniNlsMapException;
          }
          catch (UniRPCConnectionException ex)
          {
            string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite;
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
            this.m_Session.IsRPCError = true;
            throw new UniNLSMapException(message);
          }
        }
      }
    }

    public string ClientMapName
    {
      get
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniNLSMap::ClientMapName property " + this.m_ClientName);
        return this.m_ClientName;
      }
    }

    public void SetName(string pName)
    {
      lock (this)
      {
        this.m_ClientName = pName;
        this.CheckEntryConditions();
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 70);
          outPacket.UniRPCPMessage.WriteByteArray(1, this.m_Session.UniCodeStringToByteArray(this.m_ClientName));
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniReturnCode == 0)
          {
            this.m_UniNLSMapStatus = inPacket.UniRPCPMessage.ReadInteger(1);
            for (int index = 0; index < 6; ++index)
            {
              byte[] numArray = inPacket.UniRPCPMessage.ReadByteArray(2 + index);
              this.m_NLSMarkChars[6 - index - 1] = numArray[0];
            }
          }
          else
          {
            UniNLSMapException uniNlsMapException = new UniNLSMapException(UniErrorMessage.GetErrorMessage(this.m_UniReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniNlsMapException.Message + " Source: " + uniNlsMapException.Source + " Method: " + (object) uniNlsMapException.TargetSite);
            throw uniNlsMapException;
          }
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_Session.IsRPCError = true;
          throw new UniNLSMapException(message);
        }
      }
    }

    public byte[] GetUniMarks() => this.m_NLSMarkChars;

    private void CheckEntryConditions()
    {
      if (this.m_Session.UniCommand == null)
        return;
      switch (this.m_Session.UniCommand.CommandStatus)
      {
        case 1:
        case 2:
          UniNLSMapException uniNlsMapException = new UniNLSMapException(UniErrorMessage.GetErrorMessage(30114));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniNlsMapException.Message + " Source: " + uniNlsMapException.Source + " Method: " + (object) uniNlsMapException.TargetSite);
          throw uniNlsMapException;
      }
    }
  }
}
