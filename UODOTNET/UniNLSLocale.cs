// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniNLSLocale
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

namespace IBMU2.UODOTNET
{
  public class UniNLSLocale : UniRoot
  {
    private string[] m_ClientArray;
    private UniSession m_Session = (UniSession) null;
    private int m_UniNLSLocaleStatus = 0;
    private int m_UniReturnCode = -1;
    private int m_FileEncryptionType = 0;

    internal UniNLSLocale(UniSession aSession)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniNLSLocale::UniNLSLocale(UniSession aSession)");
        this.m_Session = aSession;
        this.m_ClientArray = new string[6];
        for (int index = 0; index < this.m_ClientArray.Length; ++index)
          this.m_ClientArray[index] = string.Empty;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniNLSLocale::UniNLSLocale(UniSession aSession)");
      }
    }

    public UniDynArray ClientNames
    {
      get
      {
        lock (this)
        {
          UniDynArray clientNames = new UniDynArray(this.m_Session, string.Empty);
          for (int aField = 1; aField <= 5; ++aField)
            clientNames.Insert(aField, this.m_ClientArray[aField]);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniNLSLocale::ClientNames Property" + clientNames.ToString());
          return clientNames;
        }
      }
    }

    public UniDynArray ServerNames
    {
      get
      {
        UniDynArray serverNames = new UniDynArray(this.m_Session, string.Empty);
        for (int index = 1; index <= 5; ++index)
          serverNames.Insert(index, this.GetLocaleName(index));
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniNLSLocale::ServerNames Property" + serverNames.ToString());
        return serverNames;
      }
    }

    public void SetLocaleName(string aName, int anIndex)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniNLSLocale::SetLocaleName( string aName, int anIndex ) ");
        if (anIndex < 1 || anIndex > 5)
        {
          string errorMessage = UniErrorMessage.GetErrorMessage(14022);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, errorMessage);
          throw new UniNLSLocaleException(errorMessage);
        }
        this.m_ClientArray[anIndex] = aName;
        this.CheckEntryConditions();
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 72);
          outPacket.UniRPCPMessage.WriteInteger(1, anIndex);
          outPacket.UniRPCPMessage.WriteByteArray(2, this.m_Session.UniCodeStringToByteArray(aName));
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniReturnCode == 0)
          {
            this.m_UniNLSLocaleStatus = inPacket.UniRPCPMessage.ReadInteger(1);
          }
          else
          {
            UniNLSLocaleException nlsLocaleException = new UniNLSLocaleException(UniErrorMessage.GetErrorMessage(this.m_UniReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, nlsLocaleException.Message + " Source: " + nlsLocaleException.Source + " Method: " + (object) nlsLocaleException.TargetSite);
            throw nlsLocaleException;
          }
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_Session.IsRPCError = true;
          throw new UniNLSLocaleException(message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniNLSLocale::SetLocaleName( string aName, int anIndex ) ");
      }
    }

    public void SetLocaleName(UniDynArray aName, int anIndex)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniNLSLocale::SetLocaleName( string aName, int anIndex ) ");
        if (anIndex < 1 || anIndex > 5)
        {
          string errorMessage = UniErrorMessage.GetErrorMessage(14022);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, errorMessage);
          throw new UniNLSLocaleException(errorMessage);
        }
        this.m_ClientArray[anIndex] = aName.ToString();
        this.CheckEntryConditions();
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 72);
          outPacket.UniRPCPMessage.WriteInteger(1, anIndex);
          outPacket.UniRPCPMessage.WriteByteArray(2, aName.ToByteArray());
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniReturnCode == 0)
          {
            this.m_UniNLSLocaleStatus = inPacket.UniRPCPMessage.ReadInteger(1);
          }
          else
          {
            UniNLSLocaleException nlsLocaleException = new UniNLSLocaleException(UniErrorMessage.GetErrorMessage(this.m_UniReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, nlsLocaleException.Message + " Source: " + nlsLocaleException.Source + " Method: " + (object) nlsLocaleException.TargetSite);
            throw nlsLocaleException;
          }
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_Session.IsRPCError = true;
          throw new UniNLSLocaleException(message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniNLSLocale::SetLocaleName( string aName, int anIndex ) ");
      }
    }

    public void SetLocaleName(string aName)
    {
      lock (this)
      {
        UniDynArray uniDynArray = new UniDynArray(this.m_Session, aName);
        if (uniDynArray.Dcount() == 5)
        {
          for (int index = 1; index <= 5; ++index)
            this.SetLocaleName(uniDynArray.Extract(index).ToString(), index);
        }
        else
        {
          for (int anIndex = 1; anIndex <= 5; ++anIndex)
            this.SetLocaleName(aName, anIndex);
        }
      }
    }

    public void SetLocaleName(UniDynArray localeName)
    {
      lock (this)
      {
        if (localeName.Dcount() == 5)
        {
          for (int index = 1; index <= 5; ++index)
            this.SetLocaleName(localeName.Extract(index).ToString(), index);
        }
        else
        {
          for (int anIndex = 1; anIndex <= 5; ++anIndex)
            this.SetLocaleName(localeName, anIndex);
        }
      }
    }

    private string GetLocaleName(int localeKey)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniNLSLocale::GetLocaleName( int localeKey ) ");
        this.CheckEntryConditions();
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 73);
          outPacket.UniRPCPMessage.WriteInteger(1, localeKey);
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniReturnCode == 0)
          {
            string uniCodeString = this.m_Session.ByteArrayToUniCodeString(inPacket.UniRPCPMessage.ReadByteArray(1));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniNLSLocale::GetLocaleName( int localeKey ) " + uniCodeString);
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniNLSLocale::GetLocaleName( int localeKey ) ");
            return uniCodeString;
          }
          UniNLSLocaleException nlsLocaleException = new UniNLSLocaleException(UniErrorMessage.GetErrorMessage(this.m_UniReturnCode));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, nlsLocaleException.Message + " Source: " + nlsLocaleException.Source + " Method: " + (object) nlsLocaleException.TargetSite);
          throw nlsLocaleException;
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_Session.IsRPCError = true;
          throw new UniNLSLocaleException(message);
        }
      }
    }

    private void CheckEntryConditions()
    {
      if (this.m_Session.UniCommand == null)
        return;
      switch (this.m_Session.UniCommand.CommandStatus)
      {
        case 1:
        case 2:
          UniNLSLocaleException nlsLocaleException = new UniNLSLocaleException(UniErrorMessage.GetErrorMessage(30114));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, nlsLocaleException.Message + " Source: " + nlsLocaleException.Source + " Method: " + (object) nlsLocaleException.TargetSite);
          throw nlsLocaleException;
      }
    }
  }
}
