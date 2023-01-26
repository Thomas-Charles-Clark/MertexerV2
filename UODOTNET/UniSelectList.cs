// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniSelectList
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

namespace IBMU2.UODOTNET
{
  public class UniSelectList : UniRoot
  {
    private int m_UniSelectListNumber = -1;
    private int m_UniReturnCode = 0;
    private int m_NumberOfSelectListItems = 0;
    private bool m_LastRecordRead = false;
    private UniSession m_Session = (UniSession) null;
    private int m_UniSelStatus = 0;
    private int m_FileEncryptionType = 0;

    internal UniSelectList(UniSession pSession, int pSelectListNumber)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSelectList::UniSelectList(UniSession pSession, int pSelectListNumber)");
        if (pSession == null)
        {
          UniSelectListException selectListException = new UniSelectListException(UniErrorMessage.GetErrorMessage(39120));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, selectListException.Message + " Source: " + selectListException.Source + " Method: " + (object) selectListException.TargetSite);
          throw selectListException;
        }
        if (pSelectListNumber < 0 || pSelectListNumber > 10)
        {
          UniSelectListException selectListException = new UniSelectListException("Select List Number must be between 0 and 10" + UniErrorMessage.GetErrorMessage(14022));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, selectListException.Message + " Source: " + selectListException.Source + " Method: " + (object) selectListException.TargetSite);
          throw selectListException;
        }
        this.m_Session = pSession;
        this.m_UniSelectListNumber = pSelectListNumber;
        this.m_FileEncryptionType = pSession.EncryptionType;
        this.m_LastRecordRead = false;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSelectList::UniSelectList(UniSession pSession, int pSelectListNumber)");
      }
    }

    public void ClearList()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSelectList::ClearList()");
        this.CheckEntryConditions();
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 4);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniSelectListNumber);
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniReturnCode == 0)
          {
            this.m_LastRecordRead = true;
            this.m_NumberOfSelectListItems = 0;
          }
          else
          {
            UniSelectListException selectListException = new UniSelectListException(UniErrorMessage.GetErrorMessage(this.m_UniReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, selectListException.Message + " Source: " + selectListException.Source + " Method: " + (object) selectListException.TargetSite);
            throw selectListException;
          }
        }
        catch (UniRPCConnectionException ex)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
          this.m_Session.IsRPCError = true;
          throw new UniSelectListException(ex.Message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSelectList::ClearList()");
      }
    }

    public void FormList(string pRecID)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSelectList::FormList( UniDynArray  aString )");
        this.CheckEntryConditions();
        if (pRecID.Length == 0)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniSelectList::FormList( UniDynArray  aString ) Return from  aString.Equals(\"\")");
        }
        else
        {
          try
          {
            UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
            UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
            outPacket.UniRPCPMessage.WriteInteger(0, 23);
            outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniSelectListNumber);
            outPacket.UniRPCPMessage.WriteByteArray(2, this.m_Session.UniCodeStringToByteArray(pRecID));
            this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
            this.m_UniReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
            if (this.m_UniReturnCode == 0)
            {
              this.m_LastRecordRead = false;
            }
            else
            {
              UniSelectListException selectListException = new UniSelectListException(UniErrorMessage.GetErrorMessage(this.m_UniReturnCode));
              UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, selectListException.Message + " Source: " + selectListException.Source + " Method: " + (object) selectListException.TargetSite);
              throw selectListException;
            }
          }
          catch (UniRPCConnectionException ex)
          {
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
            this.m_Session.IsRPCError = true;
            throw new UniSelectListException(ex.Message);
          }
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSelectList::FormList( UniDynArray  aString )");
        }
      }
    }

    public void FormList(string[] pRecIDSet)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSelectList::FormList(string[] pRecIDSet)");
        if (pRecIDSet == null)
        {
          UniSelectListException selectListException = new UniSelectListException(UniErrorMessage.GetErrorMessage(14022));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, selectListException.Message + " Source: " + selectListException.Source + " Method: " + (object) selectListException.TargetSite);
          throw selectListException;
        }
        this.FormList(this.m_Session.GetDelimitedString(pRecIDSet, 1));
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSelectList::FormList(string[] pRecIDSet)");
      }
    }

    public void GetList(string aListName)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSelectList::GetList( string  aListName )");
        this.CheckEntryConditions();
        if (aListName.Length == 0)
          return;
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 24);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniSelectListNumber);
          outPacket.UniRPCPMessage.WriteByteArray(2, this.m_Session.UniCodeStringToByteArray(aListName));
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniReturnCode == 0)
          {
            this.m_LastRecordRead = false;
          }
          else
          {
            UniSelectListException selectListException = new UniSelectListException(UniErrorMessage.GetErrorMessage(this.m_UniReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, selectListException.Message + " Source: " + selectListException.Source + " Method: " + (object) selectListException.TargetSite);
            throw selectListException;
          }
        }
        catch (UniRPCConnectionException ex)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
          this.m_Session.IsRPCError = true;
          throw new UniSelectListException(ex.Message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSelectList::GetList( string  aListName )");
      }
    }

    public string Next()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSelectList::Next()");
        this.CheckEntryConditions();
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 40);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniSelectListNumber);
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniReturnCode == 0)
          {
            this.m_LastRecordRead = false;
            string uniCodeString = this.m_Session.ByteArrayToUniCodeString(inPacket.UniRPCPMessage.ReadByteArray(1));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniSelectList::Next() " + uniCodeString.ToString());
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSelectList::Next()");
            return uniCodeString;
          }
          if (this.m_UniReturnCode == 22004)
          {
            this.m_LastRecordRead = true;
            this.m_UniReturnCode = 0;
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniSelectList::Next() m_UniReturnCode == UniTokens.UVE_LRR " + string.Empty);
            return string.Empty;
          }
          UniSelectListException selectListException = new UniSelectListException(UniErrorMessage.GetErrorMessage(this.m_UniReturnCode));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, selectListException.Message + " Source: " + selectListException.Source + " Method: " + (object) selectListException.TargetSite);
          throw selectListException;
        }
        catch (UniRPCConnectionException ex)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
          this.m_Session.IsRPCError = true;
          throw new UniSelectListException(ex.Message);
        }
      }
    }

    public UniDynArray ReadList()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSelectList::ReadList()");
        this.CheckEntryConditions();
        this.m_NumberOfSelectListItems = 0;
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 39);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniSelectListNumber);
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniReturnCode == 0)
          {
            this.m_NumberOfSelectListItems = inPacket.UniRPCPMessage.ReadInteger(1);
            UniDynArray uniDynArray;
            if (this.m_NumberOfSelectListItems > 0)
            {
              uniDynArray = new UniDynArray(this.m_Session, inPacket.UniRPCPMessage.ReadByteArray(2));
              this.m_LastRecordRead = false;
            }
            else
            {
              uniDynArray = new UniDynArray(this.m_Session, string.Empty);
              this.m_LastRecordRead = true;
            }
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniSelectList::ReadList() " + uniDynArray.ToString());
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSelectList::ReadList()");
            return uniDynArray;
          }
          if (this.m_UniReturnCode == 22004)
          {
            this.m_LastRecordRead = true;
            this.m_UniReturnCode = 0;
            UniDynArray uniDynArray = new UniDynArray(this.m_Session, string.Empty);
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniSelectList::ReadList() m_UniReturnCode == UniTokens.UVE_LRR " + uniDynArray.ToString());
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSelectList::ReadList() ");
            return uniDynArray;
          }
          UniSelectListException selectListException = new UniSelectListException(UniErrorMessage.GetErrorMessage(this.m_UniReturnCode));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, selectListException.Message + " Source: " + selectListException.Source + " Method: " + (object) selectListException.TargetSite);
          throw selectListException;
        }
        catch (UniRPCConnectionException ex)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
          this.m_Session.IsRPCError = true;
          throw new UniSelectListException(ex.Message);
        }
      }
    }

    public string[] ReadListAsStringArray()
    {
      lock (this)
      {
        string[] strArray = (string[]) null;
        UniDynArray uniDynArray = this.ReadList();
        if (uniDynArray.Count() > 0)
          strArray = uniDynArray.ToString().Split(this.m_Session.GetMarkCharacterAsChar(1));
        return strArray;
      }
    }

    public void SaveList(string aListName)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSelectList::SaveList( string aListName )");
        this.CheckEntryConditions();
        if (aListName.Length == 0)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSelectList::SaveList( string aListName ) Return from aListName.Equals(\"\")");
        }
        else
        {
          try
          {
            UniCommand uniCommand = this.m_Session.CreateUniCommand();
            string str = "SAVE.LIST " + aListName + " FROM " + (object) this.m_UniSelectListNumber;
            uniCommand.Command = str;
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSelectList::SaveList( string aListName )" + str);
            uniCommand.Execute();
            if (uniCommand.CommandStatus != 0)
              uniCommand.Reply(string.Empty);
          }
          catch (UniCommandException ex)
          {
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
            throw new UniSelectListException(ex.Message);
          }
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSelectList::SaveList( string aListName )");
        }
      }
    }

    public void Select(UniFile uniFile)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSelectList::Select( UniFile uniFile )");
        this.CheckEntryConditions();
        if (uniFile == null)
        {
          UniSelectListException selectListException = new UniSelectListException(UniErrorMessage.GetErrorMessage(14023));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, selectListException.Message + " Source: " + selectListException.Source + " Method: " + (object) selectListException.TargetSite);
          throw selectListException;
        }
        if (!uniFile.IsFileOpen)
        {
          UniSelectListException selectListException = new UniSelectListException(UniErrorMessage.GetErrorMessage(45000) + uniFile.FileName);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, selectListException.Message + " Source: " + selectListException.Source + " Method: " + (object) selectListException.TargetSite);
          throw selectListException;
        }
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 48);
          outPacket.UniRPCPMessage.WriteInteger(1, uniFile.FileHandle);
          outPacket.UniRPCPMessage.WriteInteger(2, this.m_UniSelectListNumber);
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniReturnCode == 0)
          {
            this.m_LastRecordRead = false;
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSelectList::Select( UniFile uniFile )");
          }
          else
          {
            UniSelectListException selectListException = new UniSelectListException(UniErrorMessage.GetErrorMessage(this.m_UniReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, selectListException.Message + " Source: " + selectListException.Source + " Method: " + (object) selectListException.TargetSite);
            throw selectListException;
          }
        }
        catch (UniRPCConnectionException ex)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
          this.m_Session.IsRPCError = true;
          throw new UniSelectListException(ex.Message);
        }
      }
    }

    public void Select(UniDictionary uniFile) => this.Select((UniFile) uniFile);

    public void SelectAlternateKey(UniFile uniFile, string aIndexName) => this.DoAKSelect(uniFile, aIndexName, -1, string.Empty);

    public void SelectAlternateKey(UniDictionary unid, string aIndexName) => this.DoAKSelect((UniFile) unid, aIndexName, -1, string.Empty);

    public void SelectMatchingAK(UniFile uniFile, string aIndexName, string aIndexValue) => this.DoAKSelect(uniFile, aIndexName, aIndexValue.Length, aIndexValue);

    public void SelectMatchingAK(UniDictionary unid, string aIndexName, string aIndexValue) => this.DoAKSelect((UniFile) unid, aIndexName, aIndexValue.Length, aIndexValue);

    private void DoAKSelect(UniFile uniFile, string aIndexName, int aIndexLen, string aIndexValue)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSelectList::DoAKSelect( UniFile uniFile,  string aIndexName, int aIndexLen, string aIndexValue )");
        this.CheckEntryConditions();
        if (uniFile == null)
        {
          UniSelectListException selectListException = new UniSelectListException(UniErrorMessage.GetErrorMessage(14023));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, selectListException.Message + " Source: " + selectListException.Source + " Method: " + (object) selectListException.TargetSite);
          throw selectListException;
        }
        if (!uniFile.IsFileOpen)
        {
          UniSelectListException selectListException = new UniSelectListException(UniErrorMessage.GetErrorMessage(45000) + uniFile.FileName);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, selectListException.Message + " Source: " + selectListException.Source + " Method: " + (object) selectListException.TargetSite);
          throw selectListException;
        }
        try
        {
          this.m_UniSelStatus = 0;
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 49);
          outPacket.UniRPCPMessage.WriteInteger(1, uniFile.FileHandle);
          outPacket.UniRPCPMessage.WriteInteger(2, this.m_UniSelectListNumber);
          outPacket.UniRPCPMessage.WriteByteArray(3, this.m_Session.UniCodeStringToByteArray(aIndexName));
          outPacket.UniRPCPMessage.WriteInteger(4, aIndexLen);
          outPacket.UniRPCPMessage.WriteCharArray(5, aIndexValue.ToCharArray());
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          this.m_UniSelStatus = inPacket.UniRPCPMessage.ReadInteger(1);
          this.m_LastRecordRead = this.m_UniSelStatus == 1;
        }
        catch (UniRPCConnectionException ex)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
          this.m_Session.IsRPCError = true;
          throw new UniSelectListException(ex.Message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSelectList::DoAKSelect( UniFile uniFile,  string aIndexName, int aIndexLen, string aIndexValue )");
      }
    }

    public bool LastRecordRead => this.m_LastRecordRead;

    private void CheckEntryConditions()
    {
      if (this.m_Session.UniCommand == null)
        return;
      switch (this.m_Session.UniCommand.CommandStatus)
      {
        case 1:
        case 2:
          UniSelectListException selectListException = new UniSelectListException(UniErrorMessage.GetErrorMessage(30114));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, selectListException.Message + " Source: " + selectListException.Source + " Method: " + (object) selectListException.TargetSite);
          throw selectListException;
      }
    }
  }
}
