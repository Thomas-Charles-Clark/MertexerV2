// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniSequentialFile
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

using System;
using System.Collections;

namespace IBMU2.UODOTNET
{
  public class UniSequentialFile : UniRoot
  {
    private int m_UniSeqFileHandle = 0;
    private string m_UniSeqFileName = "";
    private string m_UniSeqRecordName = "";
    private UniDynArray m_UniSeqReturnString = (UniDynArray) null;
    private int m_UniSeqReturnCode = -1;
    private int m_UniSeqCreateFlag = 0;
    private int m_UniSeqBlockSize = 0;
    private int m_UniSeqTimeout = 0;
    private bool m_IsFileOpen = false;
    private UniSession m_Session = (UniSession) null;
    private int m_UniSeqStatus = 0;
    private int m_FileEncryptionType = 0;

    internal UniSequentialFile(
      UniSession pSession,
      string pFileName,
      string pRecordID,
      bool pCreateFlag)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSequentialFile::UniSequentialFile(UniSession pSession, string pFileName,string pRecordID,bool pCreateFlag)");
        if (pSession == null)
        {
          UniSequentialFileException sequentialFileException = new UniSequentialFileException(UniErrorMessage.GetErrorMessage(39120));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sequentialFileException.Message + " Source: " + sequentialFileException.Source + " Method: " + (object) sequentialFileException.TargetSite);
          throw sequentialFileException;
        }
        this.m_Session = pSession;
        this.m_UniSeqFileName = pFileName;
        this.m_UniSeqRecordName = pRecordID;
        this.m_FileEncryptionType = pSession.EncryptionType;
        this.m_IsFileOpen = false;
        this.m_UniSeqTimeout = 0;
        this.m_UniSeqBlockSize = 0;
        this.m_UniSeqFileHandle = 0;
        this.m_UniSeqCreateFlag = !pCreateFlag ? 0 : 1;
        this.Open();
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSequentialFile::UniSequentialFile(UniSession pSession, string pFileName,string pRecordID,bool pCreateFlag)");
      }
    }

    public void Open()
    {
      lock (this)
      {
        try
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSequentialFile::Open()");
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 35);
          outPacket.UniRPCPMessage.WriteByteArray(1, this.m_Session.UniCodeStringToByteArray(this.m_UniSeqFileName));
          outPacket.UniRPCPMessage.WriteByteArray(2, this.m_Session.UniCodeStringToByteArray(this.m_UniSeqRecordName));
          outPacket.UniRPCPMessage.WriteInteger(3, this.m_UniSeqCreateFlag);
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniSeqReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniSeqReturnCode == 0)
          {
            this.m_UniSeqStatus = inPacket.UniRPCPMessage.ReadInteger(1);
            this.m_UniSeqFileHandle = inPacket.UniRPCPMessage.ReadInteger(2);
            this.m_IsFileOpen = true;
          }
          else
          {
            UniSequentialFileException sequentialFileException = new UniSequentialFileException(UniErrorMessage.GetErrorMessage(this.m_UniSeqReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sequentialFileException.Message + " Source: " + sequentialFileException.Source + " Method: " + (object) sequentialFileException.TargetSite);
            throw sequentialFileException;
          }
        }
        catch (UniRPCConnectionException ex)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
          this.m_Session.IsRPCError = true;
          throw new UniSequentialFileException(ex.Message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSequentialFile::Open()");
      }
    }

    public void Close()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSequentialFile::Close()");
        this.CheckEntryConditions(true);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 6);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniSeqFileHandle);
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniSeqReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniSeqReturnCode == 0)
          {
            this.m_IsFileOpen = false;
            this.m_UniSeqFileHandle = 0;
            this.m_Session = (UniSession) null;
          }
          else
          {
            UniSequentialFileException sequentialFileException = new UniSequentialFileException(UniErrorMessage.GetErrorMessage(this.m_UniSeqReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sequentialFileException.Message + " Source: " + sequentialFileException.Source + " Method: " + (object) sequentialFileException.TargetSite);
            throw sequentialFileException;
          }
        }
        catch (UniRPCConnectionException ex)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
          this.m_Session.IsRPCError = true;
          throw new UniSequentialFileException(ex.Message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSequentialFile::Close()");
      }
    }

    public void FileSeek(int aRelPos, int aOffset)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSequentialFile::FileSeek( int aRelPos, int aOffset )");
        if (aRelPos < 0 || aRelPos > 2)
        {
          UniSequentialFileException sequentialFileException = new UniSequentialFileException(UniErrorMessage.GetErrorMessage(14022));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sequentialFileException.Message + " Source: " + sequentialFileException.Source + " Method: " + (object) sequentialFileException.TargetSite);
          throw sequentialFileException;
        }
        this.CheckEntryConditions(false);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 47);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniSeqFileHandle);
          outPacket.UniRPCPMessage.WriteInteger(2, aOffset);
          outPacket.UniRPCPMessage.WriteInteger(3, aRelPos);
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniSeqReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniSeqReturnCode != 0)
          {
            UniSequentialFileException sequentialFileException = new UniSequentialFileException(UniErrorMessage.GetErrorMessage(this.m_UniSeqReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sequentialFileException.Message + " Source: " + sequentialFileException.Source + " Method: " + (object) sequentialFileException.TargetSite);
            throw sequentialFileException;
          }
        }
        catch (UniRPCConnectionException ex)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
          this.m_Session.IsRPCError = true;
          throw new UniSequentialFileException(ex.Message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSequentialFile::FileSeek( int aRelPos, int aOffset )");
      }
    }

    public UniDynArray ReadBlk()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSequentialFile::ReadBlk()");
        this.CheckEntryConditions(true);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 38);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniSeqFileHandle);
          int anInteger = this.m_UniSeqBlockSize == 0 ? 8192 : this.m_UniSeqBlockSize;
          outPacket.UniRPCPMessage.WriteInteger(2, anInteger);
          ArrayList arrayList = new ArrayList();
          while (this.m_UniSeqStatus == 0)
          {
            this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
            this.m_UniSeqReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
            if (this.m_UniSeqReturnCode == 0)
            {
              this.m_UniSeqStatus = inPacket.UniRPCPMessage.ReadInteger(1);
              if (this.m_UniSeqBlockSize != 0)
              {
                this.m_UniSeqReturnString = new UniDynArray(this.m_Session, inPacket.UniRPCPMessage.ReadByteArray(2));
                this.ReadSize = this.m_UniSeqReturnString.ToString().Length;
                UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniSequentialFile::ReadBlk() " + this.m_UniSeqReturnString.ToString());
                UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSequentialFile::ReadBlk()");
                return this.m_UniSeqReturnString;
              }
              byte[] c = inPacket.UniRPCPMessage.ReadByteArray(2);
              arrayList.AddRange((ICollection) c);
            }
            else
            {
              this.ReadSize = 0;
              UniSequentialFileException sequentialFileException = new UniSequentialFileException(UniErrorMessage.GetErrorMessage(this.m_UniSeqReturnCode));
              UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sequentialFileException.Message + " Source: " + sequentialFileException.Source + " Method: " + (object) sequentialFileException.TargetSite);
              throw sequentialFileException;
            }
          }
          byte[] pData = new byte[arrayList.Count];
          arrayList.CopyTo((Array) pData, 0);
          this.m_UniSeqReturnString = new UniDynArray(this.m_Session, pData);
          this.ReadSize = this.m_UniSeqReturnString.ToString().Length;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniSequentialFile::ReadBlk() " + this.m_UniSeqReturnString.ToString());
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSequentialFile::ReadBlk()");
          return this.m_UniSeqReturnString;
        }
        catch (UniRPCConnectionException ex)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
          this.m_Session.IsRPCError = true;
          throw new UniSequentialFileException(ex.Message);
        }
      }
    }

    public UniDynArray ReadLine()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSequentialFile::ReadLine()");
        this.CheckEntryConditions(true);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 41);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniSeqFileHandle);
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniSeqReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniSeqReturnCode == 0)
          {
            this.m_UniSeqStatus = inPacket.UniRPCPMessage.ReadInteger(1);
            this.m_UniSeqReturnString = new UniDynArray(this.m_Session, inPacket.UniRPCPMessage.ReadByteArray(2));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniSequentialFile::ReadLine() " + this.m_UniSeqReturnString.ToString());
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSequentialFile::ReadLine()");
            return this.m_UniSeqReturnString;
          }
          this.m_UniSeqStatus = 0;
          UniSequentialFileException sequentialFileException = new UniSequentialFileException(UniErrorMessage.GetErrorMessage(this.m_UniSeqReturnCode));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sequentialFileException.Message + " Source: " + sequentialFileException.Source + " Method: " + (object) sequentialFileException.TargetSite);
          throw sequentialFileException;
        }
        catch (UniRPCConnectionException ex)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
          this.m_Session.IsRPCError = true;
          throw new UniSequentialFileException(ex.Message);
        }
      }
    }

    public void WriteBlk(string aString)
    {
      lock (this)
        this.WriteBlk(new UniDynArray(this.m_Session, aString));
    }

    public void WriteBlk(UniDynArray aString)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSequentialFile::WriteBlk( string  aString )");
        this.CheckEntryConditions(true);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 62);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniSeqFileHandle);
          outPacket.UniRPCPMessage.WriteByteArray(2, aString.ToByteArray());
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniSeqReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniSeqReturnCode == 0)
          {
            this.m_UniSeqStatus = inPacket.UniRPCPMessage.ReadInteger(1);
          }
          else
          {
            this.m_UniSeqStatus = 0;
            UniSequentialFileException sequentialFileException = new UniSequentialFileException(UniErrorMessage.GetErrorMessage(this.m_UniSeqReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sequentialFileException.Message + " Source: " + sequentialFileException.Source + " Method: " + (object) sequentialFileException.TargetSite);
            throw sequentialFileException;
          }
        }
        catch (UniRPCConnectionException ex)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
          this.m_Session.IsRPCError = true;
          throw new UniSequentialFileException(ex.Message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSequentialFile::WriteBlk( string  aString )");
      }
    }

    public void WriteLine(string aString)
    {
      lock (this)
        this.WriteLine(new UniDynArray(this.m_Session, aString));
    }

    public void WriteLine(UniDynArray aString)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSequentialFile::WriteLine( string  aString )");
        this.CheckEntryConditions(true);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 64);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniSeqFileHandle);
          outPacket.UniRPCPMessage.WriteByteArray(2, aString.ToByteArray());
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniSeqReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniSeqReturnCode == 0)
          {
            this.m_UniSeqStatus = inPacket.UniRPCPMessage.ReadInteger(1);
          }
          else
          {
            this.m_UniSeqStatus = 0;
            UniSequentialFileException sequentialFileException = new UniSequentialFileException(UniErrorMessage.GetErrorMessage(this.m_UniSeqReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sequentialFileException.Message + " Source: " + sequentialFileException.Source + " Method: " + (object) sequentialFileException.TargetSite);
            throw sequentialFileException;
          }
        }
        catch (UniRPCConnectionException ex)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
          this.m_Session.IsRPCError = true;
          throw new UniFileException(ex.Message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSequentialFile::WriteLine( string  aString )");
      }
    }

    public void WriteEOF()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSequentialFile::WriteEOF()");
        this.CheckEntryConditions(true);
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 60);
          outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniSeqFileHandle);
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniSeqReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniSeqReturnCode != 0)
          {
            UniSequentialFileException sequentialFileException = new UniSequentialFileException(UniErrorMessage.GetErrorMessage(this.m_UniSeqReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sequentialFileException.Message + " Source: " + sequentialFileException.Source + " Method: " + (object) sequentialFileException.TargetSite);
            throw sequentialFileException;
          }
        }
        catch (UniRPCConnectionException ex)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
          this.m_Session.IsRPCError = true;
          throw new UniSequentialFileException(ex.Message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSequentialFile::WriteEOF()");
      }
    }

    public bool IsFileOpen => this.m_IsFileOpen;

    public int ReadSize
    {
      get => this.m_UniSeqBlockSize;
      set
      {
        lock (this)
        {
          if (value < 0)
            value = 0;
          this.m_UniSeqBlockSize = value;
        }
      }
    }

    public int EncryptionType
    {
      get => this.m_FileEncryptionType;
      set => this.m_FileEncryptionType = value;
    }

    public int UniSequentialStatus
    {
      get => this.m_UniSeqStatus;
      set => this.m_UniSeqStatus = value;
    }

    public int TimeOut
    {
      get
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniSequentialFile::TimeOut Property " + (object) this.m_UniSeqTimeout);
        return this.m_UniSeqTimeout;
      }
      set
      {
        lock (this)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSequentialFile::TimeOut Property");
          this.CheckEntryConditions(true);
          if (value < 0)
            value = 0;
          this.m_UniSeqTimeout = value;
          try
          {
            UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
            UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
            outPacket.UniRPCPMessage.WriteInteger(0, 55);
            outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniSeqFileHandle);
            outPacket.UniRPCPMessage.WriteInteger(2, this.m_UniSeqTimeout);
            this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
            this.m_UniSeqReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
            if (this.m_UniSeqReturnCode != 0)
            {
              UniSequentialFileException sequentialFileException = new UniSequentialFileException(UniErrorMessage.GetErrorMessage(this.m_UniSeqReturnCode));
              UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sequentialFileException.Message + " Source: " + sequentialFileException.Source + " Method: " + (object) sequentialFileException.TargetSite);
              throw sequentialFileException;
            }
          }
          catch (UniRPCConnectionException ex)
          {
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
            this.m_Session.IsRPCError = true;
            throw new UniSequentialFileException(ex.Message);
          }
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSequentialFile::TimeOut Property");
        }
      }
    }

    private void CheckEntryConditions(bool shouldThrow)
    {
      if (!this.IsFileOpen && shouldThrow)
      {
        string str = this.m_UniSeqFileName + ":" + this.m_UniSeqRecordName;
        this.m_UniSeqStatus = -1;
        UniSequentialFileException sequentialFileException = new UniSequentialFileException(UniErrorMessage.GetErrorMessage(45000) + str);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sequentialFileException.Message + " Source: " + sequentialFileException.Source + " Method: " + (object) sequentialFileException.TargetSite);
        throw sequentialFileException;
      }
      if (this.m_Session.UniCommand != null)
      {
        switch (this.m_Session.UniCommand.CommandStatus)
        {
          case 1:
          case 2:
            UniSequentialFileException sequentialFileException1 = new UniSequentialFileException(UniErrorMessage.GetErrorMessage(30114));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sequentialFileException1.Message + " Source: " + sequentialFileException1.Source + " Method: " + (object) sequentialFileException1.TargetSite);
            throw sequentialFileException1;
        }
      }
      this.m_UniSeqStatus = 0;
    }
  }
}
