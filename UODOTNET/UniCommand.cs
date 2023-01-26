// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniCommand
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

using System.Text;

namespace IBMU2.UODOTNET
{
  public class UniCommand : UniRoot
  {
    private const bool m_SHOULD_THROW = true;
    private const bool m_NO_THROW = false;
    private const bool m_CHECK_STATE = true;
    private const bool m_NO_CHECK = false;
    private int m_CommandStatus = 0;
    private int m_CommandEncryptionType = 0;
    private string m_Command = (string) null;
    private StringBuilder m_CommandOutput = (StringBuilder) null;
    private string m_NextBuffer = (string) null;
    private int m_CommandReturnCode = 0;
    private int m_CommandBlockSize = 0;
    private int m_CommandAtSystemReturnCode = 0;
    private int m_CommandAtSelected = 0;
    private UniSession m_Session = (UniSession) null;

    internal UniCommand(UniSession aSession)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniCommand::UniCommand(UniSession aSession)");
        if (aSession == null || aSession.UniRPCConnection == null)
        {
          UniCommandException commandException = new UniCommandException(UniErrorMessage.GetErrorMessage(39120));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, commandException.Message + " Source: " + commandException.Source + " Method: UniCommand(UniSession aSession)");
          throw commandException;
        }
        this.m_Session = aSession;
        this.m_CommandStatus = 0;
        this.m_CommandBlockSize = 0;
        this.m_CommandEncryptionType = aSession.EncryptionType;
        this.m_CommandOutput = new StringBuilder();
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniCommand::UniCommand(UniSession aSession)");
      }
    }

    public void Cancel()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniCommand::Cancel()");
        this.CheckEntryConditions(true, false);
        if (this.m_CommandStatus != 2)
        {
          if (this.m_CommandStatus != 1)
            return;
        }
        try
        {
          UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
          UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 69);
          outPacket.UniRPCPMessage.WriteInteger(0, 68);
          this.m_Session.UniRPCConnection.Call((byte) this.m_CommandEncryptionType);
          this.m_CommandReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          this.m_CommandStatus = this.m_CommandReturnCode != 0 ? this.m_CommandReturnCode : 0;
        }
        catch (UniRPCConnectionException ex)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
          this.m_Session.IsRPCError = true;
          throw new UniCommandException(ex.Message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniCommand::Cancel()");
      }
    }

    public void Execute()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniCommand::Execute()");
        this.CheckEntryConditions(true, true);
        this.m_CommandAtSystemReturnCode = 0;
        this.m_CommandAtSelected = 0;
        this.m_CommandOutput = new StringBuilder();
        int pInternalReadSize = this.CommandBlockSize;
        if (pInternalReadSize == 0)
          pInternalReadSize = 8192;
        int num = this.DoExecute(15, pInternalReadSize, "");
        if (this.CommandBlockSize == 0)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniCommand::Execute() CommandBlockSize is 0");
          this.m_CommandStatus = num;
          while (num == 2)
          {
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniCommand::Execute() code == UniObjectsTokens.UVS_MORE");
            num = this.DoExecuteContinue(pInternalReadSize);
            if (num == 2 || num == 0)
              this.m_CommandOutput.Append(this.m_NextBuffer);
            this.m_CommandStatus = num;
          }
        }
        else
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniCommand::Execute() CommandBlockSize is NOT 0");
          if (num == 2)
            this.m_CommandStatus = 2;
          else
            this.m_CommandStatus = num;
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniCommand::Execute()");
      }
    }

    public void NextBlock()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniCommand::NextBlock()");
        int num1 = 0;
        this.CheckEntryConditions(false, false);
        if (this.m_CommandStatus != 2)
        {
          UniCommandException commandException = new UniCommandException(UniErrorMessage.GetErrorMessage(40005));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, commandException.Message + " Source: " + commandException.Source + " Method: void NextBlock()");
          throw commandException;
        }
        this.m_CommandAtSystemReturnCode = 0;
        this.m_CommandAtSelected = 0;
        this.m_CommandOutput = new StringBuilder();
        int pInternalReadSize = this.CommandBlockSize;
        if (pInternalReadSize == 0)
          pInternalReadSize = 8192;
        int num2 = this.DoExecuteContinue(pInternalReadSize);
        this.m_CommandOutput.Append(this.m_NextBuffer);
        if (this.CommandBlockSize == 0)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniCommand::NextBlock() CommandBlockSize is 0");
          this.m_CommandStatus = num2;
          while (num2 == 2)
          {
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniCommand::NextBlock() code == UniObjectsTokens.UVS_MORE");
            num2 = this.DoExecuteContinue(pInternalReadSize);
            if (num2 == 2 || num2 == 0)
              this.m_CommandOutput.Append(this.m_NextBuffer);
            this.m_CommandStatus = num2;
          }
        }
        else
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniCommand::NextBlock() CommandBlockSize is NOT 0");
          if (num2 == 2)
          {
            this.m_CommandStatus = 2;
            num1 = 0;
          }
          else
            this.m_CommandStatus = num2;
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniCommand::NextBlock()");
      }
    }

    public void Reply(string aReplyString)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniCommand::Reply( String aReplyString )");
        this.CheckEntryConditions(false, false);
        if (this.m_CommandStatus != 1)
        {
          UniCommandException commandException = new UniCommandException(UniErrorMessage.GetErrorMessage(40006));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, commandException.Message + " Source: " + commandException.Source + " Method: void Reply( String aReplyString )");
          throw commandException;
        }
        this.m_CommandStatus = 0;
        int num1 = this.m_CommandBlockSize;
        int num2 = 0;
        if (num1 == 0)
          num1 = 8192;
        int num3 = this.DoInputReply(num1, aReplyString);
        if (this.m_CommandBlockSize == 0)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniCommand::Reply() CommandBlockSize is 0");
          this.m_CommandStatus = num3;
          while (num3 == 2)
          {
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniCommand::Reply() code == UniObjectsTokens.UVS_MORE");
            num3 = this.DoExecuteContinue(num1);
            if (num3 == 2 || num3 == 0)
              this.m_CommandOutput.Append(this.m_NextBuffer);
          }
          this.m_CommandStatus = 0;
        }
        else
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniCommand::Reply() CommandBlockSize is NOT 0");
          if (num3 == 2)
          {
            this.m_CommandStatus = 2;
            num3 = 0;
          }
        }
        if (num3 == 1)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniCommand::Reply() code == UniObjectsTokens.UVS_REPLY");
          this.m_CommandStatus = 1;
          num2 = 0;
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniCommand::Reply( String aReplyString )");
      }
    }

    private int DoExecute(int pServerCommand, int pInternalReadSize, string pInputReply)
    {
      int num1 = 0;
      try
      {
        UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
        UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
        outPacket.UniRPCPMessage.WriteInteger(0, pServerCommand);
        switch (pServerCommand)
        {
          case 15:
            outPacket.UniRPCPMessage.WriteInteger(1, pInternalReadSize);
            outPacket.UniRPCPMessage.WriteByteArray(2, this.m_Session.UniCodeStringToByteArray(this.m_Command));
            break;
          case 66:
            outPacket.UniRPCPMessage.WriteInteger(1, pInternalReadSize);
            this.m_NextBuffer = (string) null;
            break;
          case 67:
            string pStringVal = pInputReply + "\n";
            outPacket.UniRPCPMessage.WriteByteArray(1, this.m_Session.UniCodeStringToByteArray(pStringVal));
            outPacket.UniRPCPMessage.WriteInteger(2, pInternalReadSize);
            break;
          default:
            UniCommandException commandException = new UniCommandException("Illegal DoExecute() serverCommand" + UniErrorMessage.GetErrorMessage(14022));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, commandException.Message + " Source: " + commandException.Source + " Method: int\tDoExecute( int pServerCommand, int pInternalReadSize, String pInputReply )");
            throw commandException;
        }
        this.m_Session.UniRPCConnection.Call((byte) this.m_CommandEncryptionType);
        int num2 = inPacket.UniRPCPMessage.ReadInteger(0);
        string str = (string) null;
        switch (num2)
        {
          case 1:
            str = this.m_Session.ByteArrayToUniCodeString(inPacket.UniRPCPMessage.ReadByteArray(1));
            this.m_Session.UniRPCConnection.ReadPacket(inPacket);
            this.m_CommandAtSystemReturnCode = inPacket.UniRPCPMessage.ReadInteger(1);
            this.m_CommandAtSelected = inPacket.UniRPCPMessage.ReadInteger(2);
            break;
          case 22002:
            str = this.m_Session.ByteArrayToUniCodeString(inPacket.UniRPCPMessage.ReadByteArray(1));
            num1 = 2;
            break;
          case 39119:
            str = this.m_Session.ByteArrayToUniCodeString(inPacket.UniRPCPMessage.ReadByteArray(1));
            num1 = 1;
            break;
          default:
            num1 = num2;
            break;
        }
        if (pServerCommand == 66)
          this.m_NextBuffer = str;
        else
          this.m_CommandOutput.Append(str);
        return num1;
      }
      catch (UniRPCConnectionException ex)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
        this.m_Session.IsRPCError = true;
        throw new UniCommandException(ex.Message);
      }
    }

    private int DoExecuteContinue(int pInternalReadSize) => this.DoExecute(66, pInternalReadSize, "");

    private int DoInputReply(int internalReadSize, string aInputReply) => this.DoExecute(67, internalReadSize, aInputReply);

    private void CheckEntryConditions(bool checkState, bool shouldThrow)
    {
      if (!checkState || this.m_Session.UniCommand == null)
        return;
      switch (this.m_Session.UniCommand.CommandStatus)
      {
        case 1:
        case 2:
          if (!shouldThrow)
            break;
          UniCommandException commandException = new UniCommandException(UniErrorMessage.GetErrorMessage(30114));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, commandException.Message + " Source: " + commandException.Source + " Method: void CheckEntryConditions( bool checkState, bool shouldThrow )");
          throw commandException;
      }
    }

    public int CommandStatus
    {
      get
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "Enter UniCommand::CommandStatus property is " + (object) this.m_CommandStatus);
        return this.m_CommandStatus;
      }
    }

    public int EncryptionType
    {
      get
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "Enter UniCommand::EncryptionType property is " + (object) this.m_CommandEncryptionType);
        return this.m_CommandEncryptionType;
      }
      set => this.m_CommandEncryptionType = value;
    }

    public int CommandBlockSize
    {
      get => this.m_CommandBlockSize;
      set
      {
        lock (this)
        {
          if (value < 0)
            value = 0;
          this.m_CommandBlockSize = value;
        }
      }
    }

    public int CommandAtSelected
    {
      get
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "Enter UniCommand::CommandAtSelected property is " + (object) this.m_CommandAtSelected);
        return this.m_CommandAtSelected;
      }
    }

    public string Command
    {
      get
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "Enter UniCommand::Command property is " + this.m_Command);
        return this.m_Command;
      }
      set => this.m_Command = value;
    }

    public int SystemReturnCode
    {
      get
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "Enter UniCommand::SystemReturnCode property is " + (object) this.m_CommandAtSystemReturnCode);
        return this.m_CommandAtSystemReturnCode;
      }
    }

    public string Response
    {
      get
      {
        string response = this.m_CommandOutput.ToString();
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "Enter UniCommand::Response property is " + response);
        return response;
      }
    }
  }
}
