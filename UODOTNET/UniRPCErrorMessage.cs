// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniRPCErrorMessage
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

namespace IBMU2.UODOTNET
{
  internal sealed class UniRPCErrorMessage
  {
    private UniRPCErrorMessage()
    {
    }

    public static string GetErrorMessage(int aErrorCode)
    {
      string str = "[IBM U2][UODOTNET - UNIRPC][ErrorCode=" + (object) aErrorCode + "] ";
      string errorMessage;
      switch (aErrorCode)
      {
        case 81001:
          errorMessage = str + "The connection is bad, and may be passing corrupt data";
          break;
        case 81002:
          errorMessage = str + "No RPC Connection active.";
          break;
        case 81004:
          errorMessage = str + "An argument was requested from the RPC that was of an invalid type";
          break;
        case 81005:
          errorMessage = str + "The version of the RPC is different between the client and server";
          break;
        case 81009:
          errorMessage = str + "The RPC failed";
          break;
        case 81011:
          errorMessage = str + "The host name is not valid, or the host is not responding";
          break;
        case 81019:
          errorMessage = str + "An invalid transport type has been used";
          break;
        case 81023:
          errorMessage = str + "These packets were created with a different connection.";
          break;
        case 81024:
          errorMessage = str + "You can only multiplex proxy connections.";
          break;
        case 81025:
          errorMessage = str + "Encryption is not supported on this connection.";
          break;
        case 81026:
          errorMessage = str + "Compression is not supported on this connection.";
          break;
        case 83000:
          errorMessage = str + "The proxy server login failed";
          break;
        case 83001:
          errorMessage = str + "The proxy server login authentication failed";
          break;
        case 83002:
          errorMessage = str + "This connection exceeded the maximum number of sessions supported by the proxy server.";
          break;
        case 83003:
          errorMessage = str + "This connection exceeded the maximum number of multiplexed server connections supported by the proxy server";
          break;
        case 83004:
          errorMessage = str + "Specified security mode requires a proxy connection";
          break;
        default:
          errorMessage = str + "Unknown Error [" + (object) aErrorCode + "]  Occurred";
          break;
      }
      return errorMessage;
    }
  }
}
