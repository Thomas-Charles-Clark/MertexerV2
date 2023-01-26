// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniErrorMessage
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

namespace IBMU2.UODOTNET
{
  internal sealed class UniErrorMessage
  {
    private UniErrorMessage()
    {
    }

    internal static string GetErrorMessage(int aErrorCode)
    {
      string aExtraInfo = new string("".ToCharArray());
      return UniErrorMessage.GetErrorMessage(aErrorCode, aExtraInfo);
    }

    internal static string GetErrorMessage(int aErrorCode, string aExtraInfo)
    {
      string str = "[IBM U2][UODOTNET - UNICLIENT][ErrorCode=" + (object) aErrorCode + "] ";
      string errorMessage;
      switch (aErrorCode)
      {
        case 0:
          errorMessage = str + "No Error";
          break;
        case 86:
          errorMessage = str + "Connection Pooling is OFF.";
          break;
        case 87:
          errorMessage = str + "Invalid Operation when Connection Pooling is ON.";
          break;
        case 88:
          errorMessage = str + "Connection Pooling Session cannot be started because non pooled session is active.";
          break;
        case 89:
          errorMessage = str + "You cannot change from Connection Pooling ON to Connection Pooling OFF.";
          break;
        case 90:
          errorMessage = str + "The Connection Pool has already been created.";
          break;
        case 91:
          errorMessage = str + " Please verify UniVerse or UniData Version. This feature may not be supported in older UniVerse or UniData version.";
          break;
        case 92:
          errorMessage = str + "Connection Pooling is OFF. Please verify UniVerse or UniData Version. This feature may not be supported in older UniVerse or UniData version.";
          break;
        case 93:
          errorMessage = str + "MinPoolSize cannot exceed authorized number of Connection Pooling licenses.";
          break;
        case 14002:
          errorMessage = str + "File: [" + aExtraInfo + "] No such file or directory";
          break;
        case 14013:
          errorMessage = str + "Permission denied";
          break;
        case 14022:
          errorMessage = str + "Invalid Argument";
          break;
        case 14023:
          errorMessage = str + "Invalid File";
          break;
        case 14024:
          errorMessage = str + "Too many open files";
          break;
        case 14028:
          errorMessage = str + "No space left on device";
          break;
        case 14551:
          errorMessage = str + "Network is unreachable";
          break;
        case 22001:
          errorMessage = str + "Bad Field Number";
          break;
        case 22002:
          errorMessage = str + "Buffer size too small";
          break;
        case 22003:
          errorMessage = str + "Illegal record ID";
          break;
        case 22004:
          errorMessage = str + "The last record in the select list has been read";
          break;
        case 30001:
          errorMessage = str + "This Record was not found";
          break;
        case 30002:
          errorMessage = str + "This file or record is locked by another user";
          break;
        case 30096:
          errorMessage = str + "Unsupported Server Operation.  This operation is not supported at this release of the server.";
          break;
        case 30097:
          errorMessage = str + "The select operation failed";
          break;
        case 30098:
          errorMessage = str + "The task lock number specified is invalid";
          break;
        case 30101:
          errorMessage = str + "The fileSeek() operation failed";
          break;
        case 30103:
          errorMessage = str + "The key used to set or retrieve an @variable is invalid";
          break;
        case 30105:
          errorMessage = str + "Unable to load the subroutine on the server";
          break;
        case 30106:
          errorMessage = str + "Wrong number of arguments supplied to the subroutine";
          break;
        case 30107:
          errorMessage = str + "The subroutine failed to complete successfully";
          break;
        case 30108:
          errorMessage = str + "The I-type operation failed to complete successfully";
          break;
        case 30109:
          errorMessage = str + "The I-type failed to load";
          break;
        case 30110:
          errorMessage = str + "The I-type has not been compiled";
          break;
        case 30111:
          errorMessage = str + "This is not an I-Type or the I-type is corrupt";
          break;
        case 30112:
          errorMessage = str + "The specified filename is null";
          break;
        case 30113:
          errorMessage = str + "writeEOF() failed";
          break;
        case 30114:
          errorMessage = str + "An Execute is currently active on the server";
          break;
        case 30115:
          errorMessage = str + "No Execute is currently active on the server";
          break;
        case 30124:
          errorMessage = str + "Cannot perform this operation while a transaction is active";
          break;
        case 30125:
          errorMessage = str + "Cannot access part files";
          break;
        case 30126:
          errorMessage = str + "Failed to cancel an execute";
          break;
        case 30127:
          errorMessage = str + "Bad key for the hostType method";
          break;
        case 30128:
          errorMessage = str + "The creation of the sequential file failed";
          break;
        case 30129:
          errorMessage = str + "Failed to duplicate a pipe handle";
          break;
        case 31000:
          errorMessage = str + "No VOC record";
          break;
        case 31001:
          errorMessage = str + "No pathname in VOC record";
          break;
        case 39101:
          errorMessage = str + "The server is not responding";
          break;
        case 39119:
          errorMessage = str + "The server is waiting for input to a command";
          break;
        case 39120:
          errorMessage = str + "The session is not open";
          break;
        case 39121:
          errorMessage = str + "The UniVerse license has expired";
          break;
        case 39122:
          errorMessage = str + "The client and the server are not running at the same release level";
          break;
        case 39123:
          errorMessage = str + "The client or server is not running at the same release level as the communications support";
          break;
        case 39125:
          errorMessage = str + "The directory does not exist, or is not a UniVerse account";
          break;
        case 39127:
          errorMessage = str + "Cannot find the UV account directory";
          break;
        case 39128:
          errorMessage = str + "An invalid pathname was found in the UV.ACCOUNT file";
          break;
        case 39129:
          errorMessage = str + "The account name supplied is not a valid account";
          break;
        case 39130:
          errorMessage = str + "The UV.ACCOUNT file could not be found or opened";
          break;
        case 39131:
          errorMessage = str + "Failed to attach to the specified account";
          break;
        case 39134:
          errorMessage = str + "The user limit has been reached on the server";
          break;
        case 39135:
          errorMessage = str + "NLS is not available";
          break;
        case 39136:
          errorMessage = str + "NLS Map not found";
          break;
        case 39137:
          errorMessage = str + "NLS Locale support is not available";
          break;
        case 39138:
          errorMessage = str + "NLS Locale not found";
          break;
        case 39139:
          errorMessage = str + "NLS Locale category not found";
          break;
        case 39207:
          errorMessage = str + "Error [" + (object) aErrorCode + "] occurred on server.  Possible server-side licensing failure.";
          break;
        case 40001:
          errorMessage = str + "Pointer error in a sequential file operation";
          break;
        case 40002:
          errorMessage = str + "The session is already open";
          break;
        case 40003:
          errorMessage = str + "Invalid parameter supplied to the subroutine";
          break;
        case 40005:
          errorMessage = str + "nextBlock() was used, but no more blocks available";
          break;
        case 40006:
          errorMessage = str + "reply() method called without being at a UVS_REPLY state";
          break;
        case 40007:
          errorMessage = str + "The dictionary entry does not have a valid TYPE field";
          break;
        case 40008:
          errorMessage = str + "The dictionary entry is invalid";
          break;
        case 40009:
          errorMessage = str + "Unable to convert the data in the field";
          break;
        case 45000:
          errorMessage = str + "File [" + aExtraInfo + "] was previously closed.  Must reopen to perform this operation";
          break;
        case 45001:
          errorMessage = str + "Maximum number of UniObjects for .NET sessions already open";
          break;
        case 45002:
          errorMessage = str + "Cannot perform operation on a null recordID";
          break;
        case 80011:
          errorMessage = str + "The user name provided is incorrect";
          break;
        case 80019:
          errorMessage = str + "The password provided is incorrect";
          break;
        case 80144:
          errorMessage = str + "The account has expired";
          break;
        case 80147:
          errorMessage = str + "Unable to run as the given user";
          break;
        case 80148:
          errorMessage = str + "Unable to update user details";
          break;
        case 81007:
          errorMessage = str + "No more connections available";
          break;
        case 81014:
          errorMessage = str + "Cannot find the requested service in the uvrpcservices file";
          break;
        case 81015:
          errorMessage = str + "The connection has timed out";
          break;
        case 81016:
          errorMessage = str + "The connection was refused as the RPC daemon is not running";
          break;
        case 84444:
          errorMessage = str + "Cannon multuplex this connection";
          break;
        case 84445:
          errorMessage = str + "Specified security mode requires a proxy connection";
          break;
        case 84446:
          errorMessage = str + "Bad SSL mode";
          break;
        case 88889:
          errorMessage = str + "Cannot instantiate this object in this manner.  Must use the UniSession object";
          break;
        default:
          errorMessage = str + "Unknown Error [" + (object) aErrorCode + "]  Occurred";
          break;
      }
      return errorMessage;
    }
  }
}
