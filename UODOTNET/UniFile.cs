// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniFile
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll
using System;

namespace IBMU2.UODOTNET
{
  public class UniFile : UniRoot
  {
    private const bool THROW_EXCEPTION = true;
    private const bool NO_EXCEPTION = false;
    private const bool CHECK_RECORD = true;
    private const bool NO_RECORD = false;
    private int m_FileEncryptionType = 0;
    private int m_UniFileHandle = 0;
    internal UniSession m_Session = (UniSession) null;
    private string m_UniFileName = "";
    private string m_UniRecordID = "";
    private UniDynArray m_UniRecord;
    private int m_UniFileLockFlag = 0;
    private int m_UniFileDictFlag = 0;
    private int m_UniFileType = 0;
    private int m_UniFileReturnCode = 0;
    private int m_UniFileLockCount = 0;
    private int m_UniFileReleaseStrategy = 0;
    private int m_UniFileLockStrategy = 0;
    private int m_UniFileBlockingStrategy = 0;
    private bool m_IsFileOpen = false;
    private int m_UniFileStatus = 0;

    internal UniFile(UniSession pSession, string pFileName, int pOption)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::UniFile(UniSession pSession, string pFileName, int pOption)");
        if (pSession == null || pSession.UniRPCConnection == null)
        {
          UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(39120));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: UniFile(UniSession pSession, string pFileName, int pOption)");
          throw uniFileException;
        }
        switch (pFileName)
        {
          case "":
          case null:
            UniFileException uniFileException1 = new UniFileException(UniErrorMessage.GetErrorMessage(30112));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException1.Message + " Source: " + uniFileException1.Source + " Method: UniFile(UniSession pSession, string pFileName, int pOption)");
            throw uniFileException1;
          default:
            this.m_Session = pSession;
            this.m_UniFileName = pFileName;
            this.m_UniFileDictFlag = pOption;
            this.m_UniFileLockCount = 0;
            this.m_IsFileOpen = false;
            this.m_FileEncryptionType = pSession.EncryptionType;
            this.UniFileBlockingStrategy = pSession.BlockingStrategy;
            this.UniFileLockStrategy = pSession.LockStrategy;
            this.UniFileReleaseStrategy = pSession.ReleaseStrategy;
            this.Open();
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::UniFile(UniSession pSession, string pFileName, int pOption)");
            break;
        }
      }
    }

    public void ClearFile()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::ClearFile()");
        this.CheckEntryConditions(true, false);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 3);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileHandle);
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniFileReturnCode != 0)
          {
            UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: void ClearFile()");
            throw uniFileException;
          }
        }
        catch (UniRPCConnectionException ex)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(ex.Message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::ClearFile()");
      }
    }

    public void Close()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::Close()");
        this.CheckEntryConditions(false, false);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 5);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileHandle);
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          this.m_UniFileLockCount = 0;
          this.m_IsFileOpen = false;
          if (this.m_UniFileReturnCode != 0)
          {
            UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: void Close()");
            throw uniFileException;
          }
        }
        catch (UniRPCConnectionException ex)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(ex.Message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::Close()");
      }
    }

    public void DeleteRecord()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::DeleteRecord()");
        this.CheckEntryConditions(true, true);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 14);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileHandle);
          outPacket.UniRPCPMessage.WriteInteger(2, this.FindDeleteLock());
          outPacket.UniRPCPMessage.WriteByteArray(3, this.m_Session.UniCodeStringToByteArray(this.m_UniRecordID));
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniFileReturnCode == 0)
          {
            this.m_UniFileStatus = 0;
          }
          else
          {
            this.m_UniFileStatus = inPacket.UniRPCPMessage.ReadInteger(1);
            UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: void DeleteRecord()");
            throw uniFileException;
          }
        }
        catch (UniRPCConnectionException ex)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(ex.Message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::DeleteRecord()");
      }
    }

    public void DeleteRecord(UniDataSet aDataSet)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::DeleteRecord( UniDataSet aDataSet )");
        this.CheckEntryConditions(true, false);
        if (aDataSet.RowCount == 0)
          aDataSet.Insert("");
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 79);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileHandle);
          outPacket.UniRPCPMessage.WriteInteger(2, this.FindReadLock());
          outPacket.UniRPCPMessage.WriteByteArray(3, aDataSet.GetDelimitedByteArrayRecordID());
          outPacket.UniRPCPMessage.WriteByteArray(4, aDataSet.GetDelimitedByteArrayRecord());
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          inPacket.UniRPCPMessage.ReadByteArray(1);
          inPacket.UniRPCPMessage.ReadByteArray(2);
          if (this.m_UniFileReturnCode != 0)
          {
            if (this.m_UniFileReturnCode != 88888)
            {
              UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
              UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: void DeleteRecord( UniDataSet aDataSet )");
              throw uniFileException;
            }
          }
        }
        catch (UniRPCConnectionException ex)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(ex.Message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::DeleteRecord( UniDataSet aDataSet )");
      }
    }

    public void DeleteRecord(string aRecordIDObj)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::DeleteRecord( string  aRecordIDObj )");
        this.m_UniRecordID = aRecordIDObj;
        this.DeleteRecord();
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::DeleteRecord( string  aRecordIDObj )");
      }
    }

    public UniDynArray GetAkInfo(string akNameObj)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::GetAkInfo( string  akNameObj )");
        this.CheckEntryConditions(true, false);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 27);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileHandle);
          outPacket.UniRPCPMessage.WriteInteger(2, akNameObj.Length);
          outPacket.UniRPCPMessage.WriteCharArray(3, akNameObj.ToCharArray());
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniFileReturnCode == 0)
          {
            this.m_UniFileStatus = 0;
            this.m_UniRecord = new UniDynArray(this.m_Session, inPacket.UniRPCPMessage.ReadByteArray(1));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::GetAkInfo( string  akNameObj )");
            return this.m_UniRecord;
          }
          UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: UniDynArray GetAkInfo( string  akNameObj )");
          throw uniFileException;
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + this.m_UniFileName;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(message);
        }
      }
    }

    public void Open()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::Open()");
        this.CheckEntryConditions(false, false);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 34);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileDictFlag);
          outPacket.UniRPCPMessage.WriteByteArray(2, this.m_Session.UniCodeStringToByteArray(this.m_UniFileName));
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniFileStatus = 0;
          this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniFileReturnCode == 0)
          {
            this.m_UniFileStatus = inPacket.UniRPCPMessage.ReadInteger(1);
            this.m_UniFileHandle = inPacket.UniRPCPMessage.ReadInteger(2);
            this.m_UniFileType = this.m_UniFileStatus;
            this.m_IsFileOpen = true;
          }
          else
          {
            UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: void Open()");
            throw uniFileException;
          }
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + this.m_UniFileName;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::Open()");
      }
    }

    public bool IsRecordLocked()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::IsRecordLocked()");
        this.CheckEntryConditions(true, true);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 44);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileHandle);
          outPacket.UniRPCPMessage.WriteByteArray(2, this.m_Session.UniCodeStringToByteArray(this.m_UniRecordID));
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniFileReturnCode == 0)
          {
            int num = inPacket.UniRPCPMessage.ReadInteger(1);
            this.m_UniFileStatus = inPacket.UniRPCPMessage.ReadInteger(2);
            if (num == 0)
            {
              UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::IsRecordLocked() lock status is false");
              return false;
            }
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::IsRecordLocked() lock status is true");
            return true;
          }
          UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: bool IsRecordLocked()");
          throw uniFileException;
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + this.m_UniFileName;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(message);
        }
      }
    }

    public bool IsRecordLocked(string aRecordIDObj)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::IsRecordLocked( string  aRecordIDObj )");
        this.m_UniRecordID = aRecordIDObj;
        bool flag = this.IsRecordLocked();
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::IsRecordLocked( string  aRecordIDObj )");
        return flag;
      }
    }

    public UniDynArray iType(string aRecordID, string aITypeID)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::iType( string  aRecordID, string  aITypeID )");
        this.m_UniRecordID = aRecordID;
        this.CheckEntryConditions(true, true);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 29);
          outPacket.UniRPCPMessage.WriteByteArray(1, this.m_Session.UniCodeStringToByteArray(this.m_UniFileName));
          outPacket.UniRPCPMessage.WriteByteArray(2, this.m_Session.UniCodeStringToByteArray(this.m_UniRecordID));
          outPacket.UniRPCPMessage.WriteByteArray(3, this.m_Session.UniCodeStringToByteArray(aITypeID));
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniFileReturnCode == 0)
          {
            UniDynArray uniDynArray = new UniDynArray(this.m_Session, inPacket.UniRPCPMessage.ReadByteArray(1));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniFile::iType( string  aRecordID, string  aITypeID ) " + uniDynArray.ToString());
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::iType( string  aRecordID, string  aITypeID )");
            return uniDynArray;
          }
          UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: string  iType( string  aRecordID, string  aITypeID )");
          throw uniFileException;
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + this.m_UniFileName;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(message);
        }
      }
    }

    public void LockFile()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::LockFile()");
        this.CheckEntryConditions(true, false);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 20);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileHandle);
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniFileReturnCode != 0)
          {
            this.m_UniFileStatus = inPacket.UniRPCPMessage.ReadInteger(1);
            UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: void LockFile()");
            throw uniFileException;
          }
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + this.m_UniFileName;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::LockFile()");
      }
    }

    public void LockRecord(int aLockFlag)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::LockRecord( int aLockFlag )");
        if (aLockFlag == 0)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "return from UniFile::LockRecord( int aLockFlag ) UniObjectsTokens.UVT_NO_LOCKS  found ");
        }
        else
        {
          this.CheckEntryConditions(true, true);
          this.m_UniFileLockFlag = 2;
          if (aLockFlag == 2)
            this.m_UniFileLockFlag = 4;
          if (this.m_UniFileBlockingStrategy == 1)
            ++this.m_UniFileLockFlag;
          try
          {
            UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
            UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
            outPacket.UniRPCPMessage.WriteInteger(0, 43);
            outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileHandle);
            outPacket.UniRPCPMessage.WriteInteger(2, this.m_UniFileLockFlag);
            outPacket.UniRPCPMessage.WriteByteArray(3, this.m_Session.UniCodeStringToByteArray(this.m_UniRecordID));
            this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
            this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
            if (this.m_UniFileReturnCode != 0)
            {
              this.m_UniFileStatus = inPacket.UniRPCPMessage.ReadInteger(1);
              UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
              UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: void LockRecord( int aLockFlag )");
              throw uniFileException;
            }
          }
          catch (UniRPCConnectionException ex)
          {
            string message = ex.Message + this.m_UniFileName;
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
            this.m_Session.IsRPCError = true;
            throw new UniFileException(message);
          }
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::LockRecord( int aLockFlag )");
        }
      }
    }

    public void LockRecord(string aRecordID, int aLockFlag)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::LockRecord( string  aRecordID, int aLockFlag )");
        this.m_UniRecordID = aRecordID;
        this.LockRecord(aLockFlag);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::LockRecord( string  aRecordID, int aLockFlag )");
      }
    }

    public void LockRecord(UniDataSet aDataSet, int aLockFlag)
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::LockRecord( UniDataSet aDataSet, int aLockFlag )");
      this.CheckEntryConditions(true, false);
      this.m_UniFileLockFlag = 2;
      if (aLockFlag == 2)
        this.m_UniFileLockFlag = 4;
      if (this.m_UniFileBlockingStrategy == 1)
        ++this.m_UniFileLockFlag;
      try
      {
        UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
        UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
        outPacket.UniRPCPMessage.WriteInteger(0, 83);
        outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileHandle);
        outPacket.UniRPCPMessage.WriteInteger(2, this.m_UniFileLockFlag);
        outPacket.UniRPCPMessage.WriteByteArray(3, aDataSet.GetDelimitedByteArrayRecordID());
        outPacket.UniRPCPMessage.WriteByteArray(4, aDataSet.GetDelimitedByteArrayRecord());
        this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
        this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
        if (this.m_UniFileReturnCode != 0)
        {
          if (this.m_UniFileReturnCode != 88888)
          {
            UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: void LockRecord( UniDataSet aDataSet, int aLockFlag )");
            throw uniFileException;
          }
        }
      }
      catch (UniRPCConnectionException ex)
      {
        string message = ex.Message + this.m_UniFileName;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
        this.m_Session.IsRPCError = true;
        throw new UniFileException(message);
      }
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::LockRecord( UniDataSet aDataSet, int aLockFlag )");
    }

    public UniDynArray Read()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::Read()");
        this.CheckEntryConditions(true, true);
        this.m_UniRecord = (UniDynArray) null;
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 37);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileHandle);
          outPacket.UniRPCPMessage.WriteInteger(2, this.FindReadLock());
          outPacket.UniRPCPMessage.WriteByteArray(3, this.m_Session.UniCodeStringToByteArray(this.m_UniRecordID));
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          this.m_UniFileStatus = inPacket.UniRPCPMessage.ReadInteger(1);
          if (this.m_UniFileReturnCode == 0)
          {
            this.m_UniRecord = new UniDynArray(this.m_Session, inPacket.UniRPCPMessage.ReadByteArray(2));
            this.Lock();
            this.DoReadRelease();
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::Read()");
            return this.m_UniRecord;
          }
          if (this.m_UniFileReturnCode == 14005)
          {
            string str;
            switch (this.m_UniFileStatus)
            {
              case 1:
                str = "A bad partitioning algorithm exists for this file";
                break;
              case 2:
                str = "No such part file";
                break;
              case 3:
                str = "Record ID contains unmappable NLS characters";
                break;
              case 4:
                str = "Record data contains unmappable NLS characters";
                break;
              default:
                str = "An IO Error has occured";
                break;
            }
            UniFileException uniFileException = new UniFileException(str + UniErrorMessage.GetErrorMessage(14005));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: UniDynArray \tRead()");
            throw uniFileException;
          }
          UniFileException uniFileException1 = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException1.Message + " Source: " + uniFileException1.Source + " Method: UniDynArray \tRead()");
          throw uniFileException1;
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + this.m_UniFileName;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(message);
        }
      }
    }

    public UniDynArray Read(string aRecordID)
    {
      lock (this)
      {
        this.m_UniRecordID = aRecordID;
        return this.Read();
      }
    }

    public UniDynArray ReadField(int aFieldNumber)
    {
      lock (this)
        return this.ReadField(this.m_UniRecordID, aFieldNumber);
    }

    public UniDynArray ReadField(string aRecordID, int aFieldNumber)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::ReadField( string  aRecordID, int aFieldNumber )");
        this.m_UniRecordID = aRecordID;
        this.CheckEntryConditions(true, false);
        this.m_UniRecord = (UniDynArray) null;
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 42);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileHandle);
          outPacket.UniRPCPMessage.WriteInteger(2, this.FindReadLock());
          outPacket.UniRPCPMessage.WriteInteger(3, aFieldNumber);
          outPacket.UniRPCPMessage.WriteByteArray(4, this.m_Session.UniCodeStringToByteArray(this.m_UniRecordID));
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniFileStatus = inPacket.UniRPCPMessage.ReadInteger(1);
          this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniFileReturnCode == 0)
          {
            this.m_UniRecord = new UniDynArray(this.m_Session, inPacket.UniRPCPMessage.ReadByteArray(2));
            this.Lock();
            this.DoReadRelease();
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniFile::ReadField( string  aRecordID, int aFieldNumber ) " + this.m_UniRecord.ToString());
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::ReadField( string  aRecordID, int aFieldNumber )");
            return this.m_UniRecord;
          }
          if (this.m_UniFileReturnCode == 14005)
          {
            string str;
            switch (this.m_UniFileStatus)
            {
              case 1:
                str = "A bad partitioning algorithm exists for this file";
                break;
              case 2:
                str = "No such part file";
                break;
              case 3:
                str = "Record ID contains unmappable NLS characters";
                break;
              case 4:
                str = "Record data contains unmappable NLS characters";
                break;
              default:
                str = "An IO Error has occured";
                break;
            }
            UniFileException uniFileException = new UniFileException(str + UniErrorMessage.GetErrorMessage(14005));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: UniDynArray ReadField( string  aRecordID, int aFieldNumber )");
            throw uniFileException;
          }
          UniFileException uniFileException1 = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException1.Message + " Source: " + uniFileException1.Source + " Method: UniDynArray ReadField( string  aRecordID, int aFieldNumber )");
          throw uniFileException1;
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + this.m_UniFileName;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(message);
        }
      }
    }

    public UniDynArray ReadFields(int[] aFieldNumberSet)
    {
      lock (this)
        return this.ReadFields(this.m_UniRecordID, aFieldNumberSet);
    }

    public UniDynArray ReadFields(string aRecordID, int[] aFieldNumberSet)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::ReadFields( string  aRecordID, int[] aFieldNumberSet )");
        this.CheckEntryConditions(true, false);
        byte[] byteArrayRecordId = this.m_Session.GetDelimitedByteArrayRecordID(aFieldNumberSet, 1);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 77);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileHandle);
          outPacket.UniRPCPMessage.WriteInteger(2, this.FindReadLock());
          outPacket.UniRPCPMessage.WriteByteArray(3, this.m_Session.UniCodeStringToByteArray(aRecordID));
          outPacket.UniRPCPMessage.WriteByteArray(4, this.m_Session.UniCodeStringToByteArray(string.Empty));
          outPacket.UniRPCPMessage.WriteByteArray(5, byteArrayRecordId);
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          inPacket.UniRPCPMessage.ReadByteArray(1);
          inPacket.UniRPCPMessage.ReadByteArray(2);
          byte[] pData = inPacket.UniRPCPMessage.ReadByteArray(3);
          if (this.m_UniFileReturnCode != 0)
          {
            UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: UniDataSet ReadField( string[] aRecordIDSet,  int aFieldNumberSet )");
            throw uniFileException;
          }
          this.Lock();
          this.DoReadRelease();
          this.m_UniRecord = new UniDynArray(this.m_Session, pData);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::ReadFields( string  aRecordID, int[] aFieldNumberSet )");
          return new UniDynArray(this.m_Session, pData);
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + this.m_UniFileName;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(message);
        }
      }
    }

    public UniDynArray ReadNamedField(string aFieldName)
    {
      lock (this)
        return this.ReadNamedField(this.m_UniRecordID, aFieldName);
    }

    public UniDynArray ReadNamedField(string aRecordID, string aFieldName)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::ReadNamedField( string  aRecordID, string aFieldName )");
        this.CheckEntryConditions(true, true);
        UniDataSet uniDataSet = this.ReadRecords(new string[1]
        {
          aRecordID
        }, new string[1]{ aFieldName });
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniFile::ReadNamedField( string  aRecordID, string aFieldName ) " + uniDataSet[aRecordID].ToString());
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::ReadNamedField( string  aRecordID, string aFieldName )");
        return uniDataSet[aRecordID].Record;
      }
    }

    public UniDynArray ReadNamedFields(string[] pFieldNames)
    {
      lock (this)
        return this.ReadNamedFields(this.m_UniRecordID, pFieldNames);
    }

    public UniDynArray ReadNamedFields(string pRecordID, string[] pFieldNames)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::ReadNamedField( string  aRecordID, string aFieldName )");
        this.CheckEntryConditions(true, true);
        UniDataSet uniDataSet = this.ReadRecords(new string[1]
        {
          pRecordID
        }, pFieldNames);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniFile::ReadNamedField( string  aRecordID, string aFieldName ) " + uniDataSet[pRecordID].ToString());
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::ReadNamedField( string  aRecordID, string aFieldName )");
        return uniDataSet[pRecordID].Record;
      }
    }

    public UniDataSet ReadRecords(string[] aRecordIDSet, string[] aFieldNameSet)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::ReadNamedField( string[]  aRecordIDSet, string aFieldNameSet )");
        this.CheckEntryConditions(true, false);
        byte[] byteArrayRecordId1 = this.m_Session.GetDelimitedByteArrayRecordID(aRecordIDSet, 0);
        byte[] byteArrayRecordId2 = this.m_Session.GetDelimitedByteArrayRecordID(aFieldNameSet, 1);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 78);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileHandle);
          outPacket.UniRPCPMessage.WriteInteger(2, this.FindReadLock());
          outPacket.UniRPCPMessage.WriteByteArray(3, byteArrayRecordId1);
          outPacket.UniRPCPMessage.WriteByteArray(4, this.m_Session.UniCodeStringToByteArray(string.Empty));
          outPacket.UniRPCPMessage.WriteByteArray(5, byteArrayRecordId2);
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          byte[] RetValData = inPacket.UniRPCPMessage.ReadByteArray(1);
          byte[] StatusData = inPacket.UniRPCPMessage.ReadByteArray(2);
          byte[] RetData = inPacket.UniRPCPMessage.ReadByteArray(3);
          UniDataSet uniDataSet = this.m_Session.CreateUniDataSet(aRecordIDSet, RetData, StatusData, RetValData);
          if (this.m_UniFileReturnCode != 0)
          {
            UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: UniDataSet ReadNamedField( string[]  aRecordIDSet, string aFieldNameSet )");
            throw uniFileException;
          }
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::ReadNamedField( string[]  aRecordIDSet, string aFieldNameSet )");
          return uniDataSet;
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + this.m_UniFileName;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(message);
        }
      }
    }

    public UniDataSet ReadRecords(string[] aRecordIDSet, int[] aFieldNameSet)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::ReadNamedField( string[]  aRecordIDSet, string aFieldNameSet )");
        this.CheckEntryConditions(true, false);
        byte[] byteArrayRecordId1 = this.m_Session.GetDelimitedByteArrayRecordID(aRecordIDSet, 0);
        byte[] byteArrayRecordId2 = this.m_Session.GetDelimitedByteArrayRecordID(aFieldNameSet, 1);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 77);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileHandle);
          outPacket.UniRPCPMessage.WriteInteger(2, this.FindReadLock());
          outPacket.UniRPCPMessage.WriteByteArray(3, byteArrayRecordId1);
          outPacket.UniRPCPMessage.WriteByteArray(4, this.m_Session.UniCodeStringToByteArray(string.Empty));
          outPacket.UniRPCPMessage.WriteByteArray(5, byteArrayRecordId2);
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          byte[] RetValData = inPacket.UniRPCPMessage.ReadByteArray(1);
          byte[] StatusData = inPacket.UniRPCPMessage.ReadByteArray(2);
          byte[] RetData = inPacket.UniRPCPMessage.ReadByteArray(3);
          UniDataSet uniDataSet = this.m_Session.CreateUniDataSet(aRecordIDSet, RetData, StatusData, RetValData);
          if (this.m_UniFileReturnCode != 0)
          {
            UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: UniDataSet ReadNamedField( string[]  aRecordIDSet, string aFieldNameSet )");
            throw uniFileException;
          }
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::ReadNamedField( string[]  aRecordIDSet, string aFieldNameSet )");
          return uniDataSet;
        }
        catch (System.IndexOutOfRangeException iex)
        {
            string message = iex.Message + this.m_UniFileName;
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);

            UniDynArray r = null;
            var retryCount = 0;
            while(retryCount < 3 && r != null)
            {
                r = ReadRecordsWithoutCatching(aRecordIDSet, aFieldNameSet);
                r++;
            }
            return r;
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + this.m_UniFileName;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(message);
        }
      }
    }
    public UniDataSet ReadRecordsWithoutCatching(string[] aRecordIDSet, int[] aFieldNameSet)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::ReadNamedField( string[]  aRecordIDSet, string aFieldNameSet )");
        this.CheckEntryConditions(true, false);
        byte[] byteArrayRecordId1 = this.m_Session.GetDelimitedByteArrayRecordID(aRecordIDSet, 0);
        byte[] byteArrayRecordId2 = this.m_Session.GetDelimitedByteArrayRecordID(aFieldNameSet, 1);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 77);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileHandle);
          outPacket.UniRPCPMessage.WriteInteger(2, this.FindReadLock());
          outPacket.UniRPCPMessage.WriteByteArray(3, byteArrayRecordId1);
          outPacket.UniRPCPMessage.WriteByteArray(4, this.m_Session.UniCodeStringToByteArray(string.Empty));
          outPacket.UniRPCPMessage.WriteByteArray(5, byteArrayRecordId2);
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          byte[] RetValData = inPacket.UniRPCPMessage.ReadByteArray(1);
          byte[] StatusData = inPacket.UniRPCPMessage.ReadByteArray(2);
          byte[] RetData = inPacket.UniRPCPMessage.ReadByteArray(3);
          UniDataSet uniDataSet = this.m_Session.CreateUniDataSet(aRecordIDSet, RetData, StatusData, RetValData);
          if (this.m_UniFileReturnCode != 0)
          {
            UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: UniDataSet ReadNamedField( string[]  aRecordIDSet, string aFieldNameSet )");
            throw uniFileException;
          }
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::ReadNamedField( string[]  aRecordIDSet, string aFieldNameSet )");
          return uniDataSet;
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + this.m_UniFileName;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(message);
        }
      }
    }

    public UniDataSet ReadRecords(string[] aRecordIDSet)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::Read( string[] aRecordIDSet)");
        this.CheckEntryConditions(true, false);
        byte[] byteArrayRecordId = this.m_Session.GetDelimitedByteArrayRecordID(aRecordIDSet, 0);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 76);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileHandle);
          outPacket.UniRPCPMessage.WriteInteger(2, this.FindReadLock());
          outPacket.UniRPCPMessage.WriteByteArray(3, byteArrayRecordId);
          outPacket.UniRPCPMessage.WriteByteArray(4, this.m_Session.UniCodeStringToByteArray(string.Empty));
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          byte[] RetValData = inPacket.UniRPCPMessage.ReadByteArray(1);
          byte[] StatusData = inPacket.UniRPCPMessage.ReadByteArray(2);
          byte[] RetData = inPacket.UniRPCPMessage.ReadByteArray(3);
          UniDataSet uniDataSet = this.m_Session.CreateUniDataSet(aRecordIDSet, RetData, StatusData, RetValData);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::Read( string[] aRecordIDSet)");
          return uniDataSet;
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + this.m_UniFileName;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(message);
        }
        catch (UniDataSetException ex)
        {
          string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          throw new UniFileException(message);
        }
      }
    }

    public void UnlockFile()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::UnlockFile()");
        this.CheckEntryConditions(true, false);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 21);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileHandle);
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniFileReturnCode != 0)
          {
            this.m_UniFileStatus = inPacket.UniRPCPMessage.ReadInteger(1);
            UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: void UnlockFile()");
            throw uniFileException;
          }
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + this.m_UniFileName;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::UnlockFile()");
      }
    }

    public void UnlockRecord()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::UnlockRecord()");
        this.CheckEntryConditions(true, true);
        this.ReleaseLock(this.m_UniRecordID, 0);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::UnlockRecord()");
      }
    }

    public void UnlockRecord(string aRecordID)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::UnlockRecord( string  aRecordID)");
        this.m_UniRecordID = aRecordID;
        this.UnlockRecord();
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::UnlockRecord( string  aRecordID)");
      }
    }

    public void UnlockRecord(string[] aRecordIDSet)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::UnlockRecord( UniDataSet aDataSet )");
        this.CheckEntryConditions(true, false);
        try
        {
          byte[] byteArrayRecordId = this.m_Session.GetDelimitedByteArrayRecordID(aRecordIDSet, 0);
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 84);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileHandle);
          outPacket.UniRPCPMessage.WriteInteger(2, 0);
          outPacket.UniRPCPMessage.WriteByteArray(3, byteArrayRecordId);
          outPacket.UniRPCPMessage.WriteByteArray(4, this.m_Session.UniCodeStringToByteArray(string.Empty));
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniFileReturnCode != 0)
          {
            if (this.m_UniFileReturnCode != 88888)
            {
              UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
              UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: void UnlockRecord( UniDataSet aDataSet )");
              throw uniFileException;
            }
          }
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + this.m_UniFileName;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::UnlockRecord( UniDataSet aDataSet )");
      }
    }

    public void Write()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::Write()");
        this.CheckEntryConditions(true, true);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 61);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileHandle);
          outPacket.UniRPCPMessage.WriteInteger(2, this.FindWriteLock());
          outPacket.UniRPCPMessage.WriteByteArray(3, this.m_Session.UniCodeStringToByteArray(this.m_UniRecordID));
          outPacket.UniRPCPMessage.WriteByteArray(4, this.m_UniRecord.ToByteArray());
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniFileReturnCode != 0)
          {
            this.m_UniFileStatus = inPacket.UniRPCPMessage.ReadInteger(1);
            UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: void\tWrite()");
            throw uniFileException;
          }
          this.UnLock();
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::Write()");
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + this.m_UniFileName;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(message);
        }
      }
    }

    public void Write(string aRecordID, UniDynArray aRecordData)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::Write( string  aRecordID, UniDynArray aRecordData )");
        this.m_UniRecordID = aRecordID;
        this.m_UniRecord = aRecordData;
        this.Write();
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::Write( string  aRecordID, UniDynArray aRecordData )");
      }
    }

    public void Write(string aRecordID, string aRecordData)
    {
      lock (this)
      {
        UniDynArray aRecordData1 = new UniDynArray(this.m_Session, aRecordData);
        this.Write(aRecordID, aRecordData1);
      }
    }

    internal void WriteField(int uniFieldNumber)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::WriteField( int uniFieldNumber )");
        this.CheckEntryConditions(true, true);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 65);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileHandle);
          outPacket.UniRPCPMessage.WriteInteger(2, this.FindWriteLock());
          outPacket.UniRPCPMessage.WriteInteger(3, uniFieldNumber);
          outPacket.UniRPCPMessage.WriteByteArray(4, this.m_Session.UniCodeStringToByteArray(this.m_UniRecordID));
          outPacket.UniRPCPMessage.WriteByteArray(5, this.m_UniRecord.ToByteArray());
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniFileReturnCode != 0)
          {
            this.m_UniFileStatus = inPacket.UniRPCPMessage.ReadInteger(1);
            UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: void\tWriteField( int uniFieldNumber )");
            throw uniFileException;
          }
          this.UnLock();
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::WriteField( int uniFieldNumber )");
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + this.m_UniFileName;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(message);
        }
      }
    }

    public void WriteField(string aRecordID, int aFieldNumber, UniDynArray aRecordData)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::WriteField( string aRecordID, UniDynArray aRecordData, int aFieldNumber )");
        this.m_UniRecordID = aRecordID;
        this.m_UniRecord = aRecordData;
        this.WriteField(aFieldNumber);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::WriteField( string aRecordID, UniDynArray aRecordData, int aFieldNumber )");
      }
    }

    public void WriteField(string aRecordID, int aFieldNumber, string aRecordData)
    {
      lock (this)
      {
        UniDynArray uniDynArray = new UniDynArray(this.m_Session, aRecordData);
        this.WriteField(aRecordID, aFieldNumber, aRecordData);
      }
    }

    public void WriteField(int aFieldNumber, string aRecordData)
    {
      lock (this)
      {
        UniDynArray aRecordData1 = new UniDynArray(this.m_Session, aRecordData);
        this.WriteField(this.m_UniRecordID, aFieldNumber, aRecordData1);
      }
    }

    public void WriteField(int aFieldNumber, UniDynArray aRecordData)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::WriteField( string aRecordID, UniDynArray aRecordData, int aFieldNumber )");
        this.m_UniRecord = aRecordData;
        this.WriteField(aFieldNumber);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::WriteField( string aRecordID, UniDynArray aRecordData, int aFieldNumber )");
      }
    }

    public void WriteField(string aRecordID, int aFieldNumber)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::WriteField( string aRecordID, UniDynArray aRecordData, int aFieldNumber )");
        this.m_UniRecordID = aRecordID;
        this.WriteField(aFieldNumber);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::WriteField( string aRecordID, UniDynArray aRecordData, int aFieldNumber )");
      }
    }

    public void WriteNamedField(string aFieldName, UniDynArray aRecordData)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::WriteNamedField( string aFieldName, UniDynArray aString )");
        this.WriteNamedField(this.m_UniRecordID, aFieldName, aRecordData);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::WriteNamedField( string aFieldName, UniDynArray aString )");
      }
    }

    public void WriteNamedField(string aFieldName, string aRecordData)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::WriteNamedField( string aFieldName, string aString )");
        this.WriteNamedField(this.m_UniRecordID, aFieldName, new UniDynArray(this.m_Session, aRecordData));
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::WriteNamedField( string aFieldName, string  aString )");
      }
    }

    public void WriteNamedField(string aRecordID, string aFieldName, UniDynArray aRecordData)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::WriteNamedField( string aRecordID, string aFieldName, UniDynArray aString )");
        this.CheckEntryConditions(true, false);
        this.WriteRecords(new string[1]{ aFieldName }, new UniDataSet(this.m_Session)
        {
          {
            aRecordID,
            aRecordData
          }
        });
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::WriteNamedField( string aRecordID, string aFieldName, UniDynArray aString )");
      }
    }

    public void WriteFields(int[] aFieldNumberSet)
    {
      lock (this)
        this.WriteFields(this.m_UniRecordID, aFieldNumberSet, this.m_UniRecord);
    }

    public void WriteFields(string aRecID, int[] aFieldNumberSet)
    {
      lock (this)
        this.WriteFields(aRecID, aFieldNumberSet, this.m_UniRecord);
    }

    public void WriteFields(string aRecID, int[] aFieldNumberSet, UniDynArray aRecordData)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::WriteField( UniDataSet aDataSet, int uniFieldNumber )");
        this.CheckEntryConditions(true, false);
        byte[] byteArrayRecordId = this.m_Session.GetDelimitedByteArrayRecordID(aFieldNumberSet, 1);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 81);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileHandle);
          outPacket.UniRPCPMessage.WriteInteger(2, this.FindWriteLock());
          outPacket.UniRPCPMessage.WriteByteArray(3, this.m_Session.UniCodeStringToByteArray(aRecID));
          outPacket.UniRPCPMessage.WriteByteArray(4, aRecordData.ToByteArray());
          outPacket.UniRPCPMessage.WriteByteArray(5, byteArrayRecordId);
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniFileReturnCode != 0)
          {
            if (this.m_UniFileReturnCode != 88888)
            {
              UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
              UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: void WriteField( UniDataSet aDataSet, int uniFieldNumber )");
              throw uniFileException;
            }
          }
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + this.m_UniFileName;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::WriteField( UniDataSet aDataSet, int uniFieldNumber )");
      }
    }

    public void WriteNamedFields(string[] aFieldNameSet)
    {
      lock (this)
        this.WriteNamedFields(this.m_UniRecordID, aFieldNameSet, this.m_UniRecord);
    }

    public void WriteNamedFields(string aRecID, string[] aFieldNameSet)
    {
      lock (this)
        this.WriteNamedFields(aRecID, aFieldNameSet, this.m_UniRecord);
    }

    public void WriteNamedFields(string aRecID, string[] aFieldNameSet, UniDynArray aRecordData)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::WriteNamedField( string aRecordID, string aFieldName, UniDynArray aString )");
        this.CheckEntryConditions(true, false);
        this.WriteRecords(aFieldNameSet, new UniDataSet(this.m_Session)
        {
          {
            aRecID,
            aRecordData
          }
        });
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::WriteNamedField( string aRecordID, string aFieldName, UniDynArray aString )");
      }
    }

    public void WriteRecords(UniDataSet aDataSet)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::Write( UniDataSet aDataSet )");
        this.CheckEntryConditions(true, false);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 80);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileHandle);
          outPacket.UniRPCPMessage.WriteInteger(2, this.FindWriteLock());
          outPacket.UniRPCPMessage.WriteByteArray(3, aDataSet.GetDelimitedByteArrayRecordID());
          outPacket.UniRPCPMessage.WriteByteArray(4, aDataSet.GetDelimitedByteArrayRecord());
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniFileReturnCode != 0)
          {
            if (this.m_UniFileReturnCode != 88888)
            {
              UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
              UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: void Write( UniDataSet aDataSet )");
              throw uniFileException;
            }
          }
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + this.m_UniFileName;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::Write( UniDataSet aDataSet )");
      }
    }

    public void WriteRecords(string[] aFieldNameSet, UniDataSet aDataSet)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::WriteNamedField( UniDataSet aDataSet, string aFieldName )");
        this.CheckEntryConditions(true, false);
        byte[] byteArrayRecordId = this.m_Session.GetDelimitedByteArrayRecordID(aFieldNameSet, 1);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 82);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileHandle);
          outPacket.UniRPCPMessage.WriteInteger(2, this.FindWriteLock());
          outPacket.UniRPCPMessage.WriteByteArray(3, aDataSet.GetDelimitedByteArrayRecordID());
          outPacket.UniRPCPMessage.WriteByteArray(4, aDataSet.GetDelimitedByteArrayRecord());
          outPacket.UniRPCPMessage.WriteByteArray(5, byteArrayRecordId);
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniFileReturnCode != 0)
          {
            if (this.m_UniFileReturnCode != 88888)
            {
              UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
              UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: void WriteNamedField( UniDataSet aDataSet, string aFieldName )");
              throw uniFileException;
            }
          }
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + this.m_UniFileName;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::WriteNamedField( UniDataSet aDataSet, string aFieldName )");
      }
    }

    public void WriteRecords(int[] aFieldNumberSet, UniDataSet aDataSet)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniFile::WriteNamedField( UniDataSet aDataSet, string aFieldName )");
        this.CheckEntryConditions(true, false);
        byte[] byteArrayRecordId = this.m_Session.GetDelimitedByteArrayRecordID(aFieldNumberSet, 1);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 82);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileHandle);
          outPacket.UniRPCPMessage.WriteInteger(2, this.FindWriteLock());
          outPacket.UniRPCPMessage.WriteByteArray(3, aDataSet.GetDelimitedByteArrayRecordID());
          outPacket.UniRPCPMessage.WriteByteArray(4, aDataSet.GetDelimitedByteArrayRecord());
          outPacket.UniRPCPMessage.WriteByteArray(5, byteArrayRecordId);
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniFileReturnCode != 0)
          {
            if (this.m_UniFileReturnCode != 88888)
            {
              UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
              UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: void WriteNamedField( UniDataSet aDataSet, string aFieldName )");
              throw uniFileException;
            }
          }
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + this.m_UniFileName;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniFile::WriteNamedField( UniDataSet aDataSet, string aFieldName )");
      }
    }

    private int FindWriteLock()
    {
      int writeLock = (this.m_UniFileReleaseStrategy & 1) == 0 ? 5 : 0;
      if (this.m_UniFileBlockingStrategy == 1)
        ++writeLock;
      return writeLock;
    }

    private void UnLock()
    {
      if (this.m_UniFileLockStrategy == 0)
        return;
      --this.m_UniFileLockCount;
      if (this.m_UniFileLockCount >= 0)
        return;
      this.m_UniFileLockCount = 0;
    }

    private void Lock()
    {
      if (this.m_UniFileLockStrategy == 0)
        return;
      ++this.m_UniFileLockCount;
    }

    private void DoReadRelease()
    {
      if ((this.m_UniFileReleaseStrategy & 2) == 0)
        return;
      this.SingleRelease();
    }

    private void SingleRelease() => this.ReleaseLock(this.m_UniRecordID, 0);

    private void ReleaseLock(string aRecordID, int aClearFlag)
    {
      lock (this)
      {
        this.CheckEntryConditions(true, false);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 45);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniFileHandle);
          outPacket.UniRPCPMessage.WriteInteger(2, aClearFlag);
          outPacket.UniRPCPMessage.WriteByteArray(3, this.m_Session.UniCodeStringToByteArray(this.m_UniRecordID));
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniFileReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniFileReturnCode != 0)
          {
            UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(this.m_UniFileReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: void ReleaseLock( string  aRecordID, int aClearFlag )");
            throw uniFileException;
          }
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + this.m_UniFileName;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(message);
        }
      }
    }

    public bool IsFileOpen => this.m_IsFileOpen;

    public int UniFileBlockingStrategy
    {
      get => this.m_UniFileBlockingStrategy;
      set
      {
        if (value < 1 || value > 2)
        {
          UniFileException uniFileException = new UniFileException("Blocking strategy must be either UniObjectsTokens.UVT_WAIT_LOCKED(1) or UniObjectsTokens.UVT_RETURN_LOCKED(2)" + UniErrorMessage.GetErrorMessage(14022));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Property: int  UniFileBlockingStrategy");
          throw uniFileException;
        }
        this.m_UniFileBlockingStrategy = value;
      }
    }

    public int UniFileLockStrategy
    {
      get => this.m_UniFileLockStrategy;
      set
      {
        if (value < 0 || value > 2)
        {
          UniFileException uniFileException = new UniFileException("Locking strategy must be either UniObjectsTokens.UVT_NO_LOCKS(0), UniObjectsTokens.UVT_EXCLUSIVE_UPDATE(1) or UniObjectsTokens.UVT_SHARED_READ(2)" + UniErrorMessage.GetErrorMessage(14022));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Property: int  UniFileLockStrategy");
          throw uniFileException;
        }
        this.m_UniFileLockStrategy = value;
      }
    }

    public int UniFileReleaseStrategy
    {
      get => this.m_UniFileReleaseStrategy;
      set
      {
        if (value < 0 || value > 15)
        {
          UniFileException uniFileException = new UniFileException("UniFile.setReleaseStrategy() failed:  Strategy eitherWRITE_RELEASE (1), READ_RELEASE (2), EXPLICIT_RELEASE (4) or CHANGE_RELEASE (8)" + UniErrorMessage.GetErrorMessage(14022));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Property: int  UniFileReleaseStrategy");
          throw uniFileException;
        }
        this.m_UniFileReleaseStrategy = value;
      }
    }

    public int FileStatus => this.m_UniFileStatus;

    public int EncryptionType
    {
      get => this.m_FileEncryptionType;
      set => this.m_FileEncryptionType = value;
    }

    public string RecordID
    {
      get => this.m_UniRecordID;
      set
      {
        this.DoOnChangeRelease();
        this.m_UniRecordID = value;
      }
    }

    public UniDynArray Record
    {
      get => this.m_UniRecord;
      set => this.m_UniRecord = value;
    }

    public string RecordString
    {
      get => this.m_UniRecord.ToString();
      set => this.m_UniRecord = new UniDynArray(this.m_Session, value);
    }

    public string FileName => this.m_UniFileName;

    public int FileType => this.m_UniFileType;

    internal int FileHandle => this.m_UniFileHandle;

    private void DoOnChangeRelease()
    {
      if ((this.m_UniFileReleaseStrategy & 8) == 0)
        return;
      this.SingleRelease();
    }

    private int FindDeleteLock()
    {
      int deleteLock = 0;
      if ((this.m_UniFileReleaseStrategy & 1) == 0)
        deleteLock = 3;
      if (this.m_UniFileBlockingStrategy == 1)
        ++deleteLock;
      return deleteLock;
    }

    private int FindReadLock()
    {
      int readLock = 0;
      switch (this.m_UniFileLockStrategy)
      {
        case 1:
          readLock = 2;
          break;
        case 2:
          readLock = 4;
          break;
      }
      if (this.m_UniFileBlockingStrategy == 1)
        ++readLock;
      return readLock;
    }

    private void CheckEntryConditions(bool shouldThrow, bool checkForRecord)
    {
      if (!this.IsFileOpen && shouldThrow)
      {
        this.m_UniFileStatus = -1;
        UniFileException uniFileException = new UniFileException(UniErrorMessage.GetErrorMessage(45000));
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException.Message + " Source: " + uniFileException.Source + " Method: void CheckEntryConditions( bool shouldThrow, bool checkForRecord )");
        throw uniFileException;
      }
      if (this.m_Session.UniCommand != null)
      {
        switch (this.m_Session.UniCommand.CommandStatus)
        {
          case 1:
          case 2:
            UniFileException uniFileException1 = new UniFileException(UniErrorMessage.GetErrorMessage(30114));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException1.Message + " Source: " + uniFileException1.Source + " Method: void CheckEntryConditions( bool shouldThrow, bool checkForRecord )");
            throw uniFileException1;
        }
      }
      if (checkForRecord && (this.m_UniRecordID == null || this.m_UniRecordID.Length == 0))
      {
        UniFileException uniFileException2 = new UniFileException(UniErrorMessage.GetErrorMessage(45002));
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniFileException2.Message + " Source: " + uniFileException2.Source + " Method: void CheckEntryConditions( bool shouldThrow, bool checkForRecord )");
        throw uniFileException2;
      }
      this.m_UniFileStatus = 0;
    }
  }
}
