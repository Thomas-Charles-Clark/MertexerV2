// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniSubroutine
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

namespace IBMU2.UODOTNET
{
  public class UniSubroutine : UniRoot
  {
    private const int UVSUB_INIT_SEND_ARGS = 3;
    private const int UVSUB_INIT_RETURN_ARGS = 1;
    private const int UVSUB_MAX_ARGS = 1024;
    private UniDynArray[] m_UniInputArgArray = (UniDynArray[]) null;
    private UniDynArray[] m_UniOutputArgArray = (UniDynArray[]) null;
    private string m_UniRoutineName = "";
    private int m_UniNumArgs = 0;
    private UniSession m_Session = (UniSession) null;
    private int m_UniSubReturnCode = -1;
    private int m_FileEncryptionType = 0;

    internal UniSubroutine(UniSession pSession, string pSubName, int pNumArgs)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSubroutine::UniSubroutine(UniSession pSession,string  pSubName, int pNumArgs)");
        if (pNumArgs < 0)
          pNumArgs = 1;
        if (pNumArgs > 1024)
          pNumArgs = 1024;
        if (pSession == null)
        {
          UniSubroutineException subroutineException = new UniSubroutineException(UniErrorMessage.GetErrorMessage(39120));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, subroutineException.Message + " Source: " + subroutineException.Source + " Method: " + (object) subroutineException.TargetSite);
          throw subroutineException;
        }
        this.m_Session = pSession;
        this.m_UniRoutineName = pSubName;
        this.m_UniNumArgs = pNumArgs;
        this.m_UniInputArgArray = new UniDynArray[this.m_UniNumArgs];
        this.m_UniOutputArgArray = new UniDynArray[this.m_UniNumArgs];
        for (int index = 0; index < this.m_UniNumArgs; ++index)
        {
          this.m_UniInputArgArray[index] = new UniDynArray(this.m_Session, string.Empty);
          this.m_UniOutputArgArray[index] = new UniDynArray(this.m_Session, string.Empty);
        }
        this.ResetArgs();
        this.m_UniSubReturnCode = 0;
        this.m_FileEncryptionType = pSession.EncryptionType;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSubroutine::UniSubroutine(UniSession pSession,string  pSubName, int pNumArgs)");
      }
    }

    public void ResetArgs()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSubroutine::ResetArgs()");
        for (int index = 0; index < this.m_UniNumArgs; ++index)
          this.m_UniOutputArgArray[index] = this.m_UniInputArgArray[index];
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSubroutine::ResetArgs()");
      }
    }

    public void Call()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSubroutine::Call()");
        if (this.m_Session.UniCommand != null)
        {
          switch (this.m_Session.UniCommand.CommandStatus)
          {
            case 1:
            case 2:
              UniSubroutineException subroutineException1 = new UniSubroutineException(UniErrorMessage.GetErrorMessage(30114));
              UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, subroutineException1.Message + " Source: " + subroutineException1.Source + " Method: " + (object) subroutineException1.TargetSite);
              throw subroutineException1;
          }
        }
        if (this.m_UniRoutineName != null)
        {
          if (this.m_UniRoutineName.Length != 0)
          {
            try
            {
              int anIndex1 = 3;
              UniRPCPacket inPacket = this.m_Session.UniRPCConnection.InPacket;
              UniRPCPacket outPacket = this.m_Session.UniRPCConnection.OutPacket;
              outPacket.UniRPCPMessage.WriteInteger(0, 52);
              outPacket.UniRPCPMessage.WriteInteger(1, this.m_UniNumArgs);
              outPacket.UniRPCPMessage.WriteByteArray(2, this.m_Session.UniCodeStringToByteArray(this.m_UniRoutineName));
              for (int index = 0; index < this.m_UniNumArgs; ++index)
              {
                outPacket.UniRPCPMessage.WriteByteArray(anIndex1, this.m_UniInputArgArray[index].ToByteArray());
                ++anIndex1;
              }
              this.m_Session.UniRPCConnection.Call((byte) this.m_FileEncryptionType);
              this.m_UniSubReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
              if (this.m_UniSubReturnCode != 0)
              {
                UniSubroutineException subroutineException2 = new UniSubroutineException(UniErrorMessage.GetErrorMessage(this.m_UniSubReturnCode));
                UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, subroutineException2.Message + " Source: " + subroutineException2.Source + " Method: " + (object) subroutineException2.TargetSite);
                throw subroutineException2;
              }
              int anIndex2 = 1;
              for (int index = 0; index < this.m_UniNumArgs; ++index)
              {
                byte[] pData = inPacket.UniRPCPMessage.ReadByteArray(anIndex2);
                this.m_UniOutputArgArray[index] = new UniDynArray(this.m_Session, pData);
                ++anIndex2;
              }
            }
            catch (UniRPCConnectionException ex)
            {
              UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, ex.Message);
              this.m_Session.IsRPCError = true;
              throw new UniSubroutineException(ex.Message);
            }
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSubroutine::Call()");
            return;
          }
        }
        UniSubroutineException subroutineException3 = new UniSubroutineException(UniErrorMessage.GetErrorMessage(30105));
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, subroutineException3.Message + " Source: " + subroutineException3.Source + " Method: " + (object) subroutineException3.TargetSite);
        throw subroutineException3;
      }
    }

    public string GetArg(int aArgNum)
    {
      lock (this)
        return this.GetArgDynArray(aArgNum).ToString();
    }

    public UniDynArray GetArgDynArray(int aArgNum)
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSubroutine::GetArg( int aArgNum )");
      if (aArgNum < 0 || aArgNum >= this.m_UniNumArgs)
      {
        UniSubroutineException subroutineException = new UniSubroutineException(UniErrorMessage.GetErrorMessage(40003));
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, subroutineException.Message + " Source: " + subroutineException.Source + " Method: " + (object) subroutineException.TargetSite);
        throw subroutineException;
      }
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSubroutine::GetArg( int aArgNum )");
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniSubroutine::GetArg( int aArgNum ) " + (object) this.m_UniOutputArgArray[aArgNum]);
      return this.m_UniOutputArgArray[aArgNum];
    }

    public void SetArg(int aArgNum, UniDynArray aArgVal)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSubroutine::SetArg( int aArgNum, string aArgVal )");
        if (aArgNum < 0 || aArgNum >= this.m_UniNumArgs)
        {
          UniSubroutineException subroutineException = new UniSubroutineException(UniErrorMessage.GetErrorMessage(40003));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, subroutineException.Message + " Source: " + subroutineException.Source + " Method: " + (object) subroutineException.TargetSite);
          throw subroutineException;
        }
        this.m_UniInputArgArray[aArgNum] = aArgVal;
        this.m_UniOutputArgArray[aArgNum] = aArgVal;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSubroutine::SetArg( int aArgNum, string aArgVal )");
      }
    }

    public void SetArg(int aArgNum, string aArgVal)
    {
      lock (this)
      {
        UniDynArray aArgVal1 = new UniDynArray(this.m_Session, aArgVal);
        this.SetArg(aArgNum, aArgVal1);
      }
    }

    public string RoutineName => this.m_UniRoutineName;

    public int ArgumentsNumber => this.m_UniNumArgs;
  }
}
