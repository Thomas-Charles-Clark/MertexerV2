// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniTransaction
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

using System;

namespace IBMU2.UODOTNET
{
  public class UniTransaction : UniRoot
  {
    private const int UVI_TRANS_START = 1;
    private const int UVI_TRANS_COMMIT = 2;
    private const int UVI_TRANS_ROLLBACK = 3;
    private const int UVI_AT_TRANSACTION = 5;
    private const int UVI_AT_TRANSACTION_LEVEL = 11;
    private bool m_IsTransActive = false;
    private int m_UniReturnCode = 0;
    private UniSession m_Session = (UniSession) null;
    private int m_UniTransactionStatus = 0;
    private int m_FileEncryptionType = 0;

    internal UniTransaction(UniSession pSession)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniTransaction::UniTransaction(UniSession pSession)");
        if (pSession == null)
        {
          UniTransactionException transactionException = new UniTransactionException(UniErrorMessage.GetErrorMessage(39120));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, transactionException.Message + " Source: " + transactionException.Source + " Method: " + (object) transactionException.TargetSite);
          throw transactionException;
        }
        this.m_Session = pSession;
        this.m_IsTransActive = false;
        this.m_FileEncryptionType = pSession.EncryptionType;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniTransaction::UniTransaction(UniSession pSession)");
      }
    }

    public void Commit()
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniTransaction::Commit()");
      this.ServerTransaction(2);
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniTransaction::Commit()");
    }

    public void Rollback()
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniTransaction::Rollback()");
      this.ServerTransaction(3);
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniTransaction::Rollback()");
    }

    public void Begin()
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniTransaction::Begin()");
      this.ServerTransaction(1);
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniTransaction::Begin()");
    }

    public int GetLevel()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniTransaction::GetLevel()");
        UniCommand uniCommand = this.m_Session.UniCommand;
        if (uniCommand != null)
        {
          switch (uniCommand.CommandStatus)
          {
            case 1:
            case 2:
              UniTransactionException transactionException = new UniTransactionException(UniErrorMessage.GetErrorMessage(30114));
              UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, transactionException.Message + " Source: " + transactionException.Source + " Method: " + (object) transactionException.TargetSite);
              throw transactionException;
          }
        }
        try
        {
          UniDynArray uniDynArray = this.m_Session.GetValue(11);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniTransaction::GetLevel() " + (object) uniDynArray);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniTransaction::GetLevel()");
          return Convert.ToInt32(uniDynArray.ToString());
        }
        catch (UniSessionException ex)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
          throw new UniTransactionException(ex.Message);
        }
      }
    }

    public bool IsActive()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniTransaction::IsActive()");
        if (this.m_Session.UniCommand != null)
        {
          switch (this.m_Session.UniCommand.CommandStatus)
          {
            case 1:
            case 2:
              UniTransactionException transactionException = new UniTransactionException(UniErrorMessage.GetErrorMessage(30114));
              UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, transactionException.Message + " Source: " + transactionException.Source + " Method: " + (object) transactionException.TargetSite);
              throw transactionException;
          }
        }
        UniDynArray uniDynArray;
        try
        {
          uniDynArray = this.m_Session.GetValue(5);
        }
        catch (UniSessionException ex)
        {
          throw new UniTransactionException(ex.Message);
        }
        this.m_IsTransActive = uniDynArray.ToString().Length != 0;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniTransaction::IsActive() " + Convert.ToString(this.m_IsTransActive));
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniTransaction::IsActive()");
        return this.m_IsTransActive;
      }
    }

    private int ServerTransaction(int aKey)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniTransaction::ServerTransaction( int aKey )");
        if (this.m_Session.UniCommand != null)
        {
          switch (this.m_Session.UniCommand.CommandStatus)
          {
            case 1:
            case 2:
              UniTransactionException transactionException = new UniTransactionException(UniErrorMessage.GetErrorMessage(30114));
              UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, transactionException.Message + " Source: " + transactionException.Source + " Method: " + (object) transactionException.TargetSite);
              throw transactionException;
          }
        }
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 57);
          outPacket.UniRPCPMessage.WriteInteger(1, aKey);
          this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
          this.m_UniReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          this.m_UniTransactionStatus = this.m_UniReturnCode == 0 ? 0 : inPacket.UniRPCPMessage.ReadInteger(0);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniTransaction::ServerTransaction( int aKey ) " + (object) this.m_UniReturnCode);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniTransaction::ServerTransaction( int aKey )");
          return this.m_UniReturnCode;
        }
        catch (UniRPCConnectionException ex)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
          this.m_Session.IsRPCError = true;
          throw new UniTransactionException(ex.Message);
        }
      }
    }
  }
}
