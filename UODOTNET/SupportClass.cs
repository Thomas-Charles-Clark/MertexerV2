// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.SupportClass
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

namespace IBMU2.UODOTNET
{
  public class SupportClass : UniRoot
  {
    public static byte[] ToByteArray(sbyte[] sbyteArray)
    {
      byte[] byteArray = new byte[sbyteArray.Length];
      for (int index = 0; index < sbyteArray.Length; ++index)
        byteArray[index] = (byte) sbyteArray[index];
      return byteArray;
    }

    public static byte[] ToByteArray(string sourceString)
    {
      byte[] byteArray = new byte[sourceString.Length];
      for (int index = 0; index < sourceString.Length; ++index)
        byteArray[index] = (byte) sourceString[index];
      return byteArray;
    }

    public static sbyte[] ToSByteArray(byte[] byteArray)
    {
      sbyte[] sbyteArray = new sbyte[byteArray.Length];
      for (int index = 0; index < byteArray.Length; ++index)
        sbyteArray[index] = (sbyte) byteArray[index];
      return sbyteArray;
    }

    public static sbyte[] ToSByteArray(string sourceString)
    {
      sbyte[] sbyteArray = new sbyte[sourceString.Length];
      for (int index = 0; index < sourceString.Length; ++index)
        sbyteArray[index] = (sbyte) sourceString[index];
      return sbyteArray;
    }

    public static char[] ToCharArray(sbyte[] sByteArray)
    {
      char[] charArray = new char[sByteArray.Length];
      for (int index = 0; index < sByteArray.Length; ++index)
        charArray[index] = (char) sByteArray[index];
      return charArray;
    }

    public static char[] ToCharArray(byte[] byteArray)
    {
      char[] charArray = new char[byteArray.Length];
      for (int index = 0; index < byteArray.Length; ++index)
        charArray[index] = (char) byteArray[index];
      return charArray;
    }

    public static int URShift(int number, int bits) => number >= 0 ? number >> bits : (number >> bits) + (2 << ~bits);

    public static int URShift(int number, long bits) => SupportClass.URShift(number, (int) bits);

    public static long URShift(long number, int bits) => number >= 0L ? number >> bits : (number >> bits) + (2L << ~bits);

    public static long URShift(long number, long bits) => SupportClass.URShift(number, (int) bits);
  }
}
