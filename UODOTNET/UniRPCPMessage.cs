// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniRPCPMessage
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

using System;
using System.Diagnostics;
using System.IO;

namespace IBMU2.UODOTNET
{
  internal class UniRPCPMessage : UniRPC
  {
    private const byte m_UNIRPC_PATT_CHECK = 108;
    private const int m_UNIRPC_HEADER_SIZE = 24;
    private const int m_UNIRPC_DEFAULT_PACKET_TYPE = 0;
    private const int m_UNIRPC_MAX_PACKET_ARGS = 2048;
    private const int m_UNIRPC_DEFAULT_DATA_SIZE = 32768;
    private const int m_UNIRPC_INT_LENGTH = 1;
    private const int m_UNIRPC_DOUBLE_LENGTH = 1;
    private const byte m_UNIRPC_UNCOMPRESSED = 0;
    private const byte m_UNIRPC_COMPRESSED = 1;
    private byte[] m_HeaderBuffer = (byte[]) null;
    private byte[] m_DataInfoBuffer = (byte[]) null;
    private byte[] m_DataBuffer = (byte[]) null;
    private byte[] m_DataBufferEncrypted = (byte[]) null;
    private byte[] m_DataBufferCompressed = (byte[]) null;
    private int m_DataOffset;
    private int m_CurrentArgumentCount;
    private int m_CurrentCompressionThreshold;
    private bool m_IsCompressed;
    private bool m_IsEncrypted;

    private BinaryReader m_In;

    internal UniRPCPMessage()
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniRPCPMessage::UniRPCPMessage()");
      this.m_HeaderBuffer = new byte[24];
      this.m_DataInfoBuffer = new byte[16384];
      this.m_DataBuffer = new byte[32768];
      this.ResetHeader();
      this.ResetData();
      this.m_CurrentCompressionThreshold = 0;
      this.m_In = null;
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniRPCPMessage::UniRPCPMessage()");
    }

    private void ResetHeader()
    {
      for (int index = 0; index < this.m_HeaderBuffer.Length; ++index)
        this.m_HeaderBuffer[index] = (byte) 0;
      this.m_HeaderBuffer[0] = (byte) 108;
      this.m_HeaderBuffer[1] = (byte) (SupportClass.URShift(1, 0) & (int) byte.MaxValue);
      this.m_HeaderBuffer[8] = (byte) (SupportClass.URShift(0, 24) & (int) byte.MaxValue);
      this.m_HeaderBuffer[9] = (byte) (SupportClass.URShift(0, 16) & (int) byte.MaxValue);
      this.m_HeaderBuffer[10] = (byte) (SupportClass.URShift(0, 8) & (int) byte.MaxValue);
      this.m_HeaderBuffer[11] = (byte) (SupportClass.URShift(0, 0) & (int) byte.MaxValue);
      this.m_HeaderBuffer[12] = (byte) (SupportClass.URShift(2, 0) & (int) byte.MaxValue);
      this.m_HeaderBuffer[13] = (byte) 0;
      this.m_HeaderBuffer[14] = (byte) 0;
    }

    private void ResetData()
    {
      this.m_DataOffset = 0;
      this.m_CurrentArgumentCount = 0;
      this.m_IsCompressed = false;
      this.m_IsEncrypted = false;
      if (this.m_DataBuffer.Length <= 32768)
        return;
      this.m_DataBuffer = new byte[32768];
    }

    public int ArgumentCount => this.m_CurrentArgumentCount;

    public int CompressionThreshold
    {
      get => this.m_CurrentCompressionThreshold;
      set => this.m_CurrentCompressionThreshold = this.IsCompressionSupported() || value == 0 ? value : throw new UniRPCPacketException(UniRPCErrorMessage.GetErrorMessage(81026));
    }

    public int GetLength(byte anEncryptionType)
    {
      if (!this.m_IsEncrypted)
      {
        this.Encrypt(anEncryptionType);
        this.m_IsEncrypted = true;
      }
      if (!this.m_IsCompressed)
      {
        this.Compress();
        this.m_IsCompressed = true;
      }
      return 24 + this.m_DataBufferCompressed.Length;
    }

    private void Encrypt(byte anEncryptionType)
    {
      this.m_DataBufferEncrypted = new byte[this.m_CurrentArgumentCount * 8 + this.m_DataOffset];
      if (anEncryptionType == (byte) 1)
      {
        int index1;
        for (index1 = 0; index1 < this.m_CurrentArgumentCount * 8; ++index1)
          this.m_DataBufferEncrypted[index1] = (byte) ((uint) this.m_DataInfoBuffer[index1] ^ (uint) (byte) this.CurrentVersion);
        int index2 = 0;
        while (index2 < this.m_DataOffset)
        {
          this.m_DataBufferEncrypted[index1] = (byte) ((uint) this.m_DataBuffer[index2] ^ (uint) (byte) this.CurrentVersion);
          ++index2;
          ++index1;
        }
      }
      else
      {
        Array.Copy((Array) this.m_DataInfoBuffer, 0, (Array) this.m_DataBufferEncrypted, 0, this.m_CurrentArgumentCount * 8);
        Array.Copy((Array) this.m_DataBuffer, 0, (Array) this.m_DataBufferEncrypted, this.m_CurrentArgumentCount * 8, this.m_DataOffset);
      }
      this.WriteEncryptionMask(anEncryptionType);
      this.WriteArgumentCount(this.m_CurrentArgumentCount);
    }

    private void Compress()
    {
      if (this.m_CurrentCompressionThreshold > 0 && this.m_CurrentArgumentCount * 8 + this.m_DataOffset >= this.m_CurrentCompressionThreshold)
      {
        this.m_DataBufferCompressed = this.m_DataBufferEncrypted;
        this.WriteHeaderLength(this.m_DataBufferCompressed.Length);
        this.WriteCompressionMask((byte) 1);
      }
      else
      {
        this.m_DataBufferCompressed = this.m_DataBufferEncrypted;
        this.WriteHeaderLength(this.m_DataBufferCompressed.Length);
        this.WriteCompressionMask((byte) 0);
      }
    }

    private void WriteCompressionMask(byte aMask) => this.m_HeaderBuffer[13] = aMask;

    private void WriteEncryptionMask(byte aMask) => this.m_HeaderBuffer[14] = aMask;

    private void WriteHeaderLength(int aLength)
    {
      this.m_HeaderBuffer[4] = (byte) (SupportClass.URShift(aLength, 24) & (int) byte.MaxValue);
      this.m_HeaderBuffer[5] = (byte) (SupportClass.URShift(aLength, 16) & (int) byte.MaxValue);
      this.m_HeaderBuffer[6] = (byte) (SupportClass.URShift(aLength, 8) & (int) byte.MaxValue);
      this.m_HeaderBuffer[7] = (byte) (SupportClass.URShift(aLength, 0) & (int) byte.MaxValue);
    }

    private void WriteArgumentCount(int aArgumentCount)
    {
      this.m_HeaderBuffer[20] = (byte) (SupportClass.URShift(aArgumentCount, 8) & (int) byte.MaxValue);
      this.m_HeaderBuffer[21] = (byte) (SupportClass.URShift(aArgumentCount, 0) & (int) byte.MaxValue);
    }

    public int Size => 24 + this.m_CurrentArgumentCount * 8 + this.m_DataOffset;

    public char[] ReadCharArray(int anIndex)
    {
      int startIndex = anIndex < this.m_CurrentArgumentCount ? this.DataItemOffset(anIndex) : throw new UniRPCPacketException(UniRPCErrorMessage.GetErrorMessage(81004));
      int length = (((int) this.m_DataInfoBuffer[anIndex * 8] & (int) byte.MaxValue) << 24) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 1] & (int) byte.MaxValue) << 16) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 2] & (int) byte.MaxValue) << 8) + ((int) this.m_DataInfoBuffer[anIndex * 8 + 3] & (int) byte.MaxValue);
      if ((((int) this.m_DataInfoBuffer[anIndex * 8 + 4] & (int) byte.MaxValue) << 24) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 5] & (int) byte.MaxValue) << 16) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 6] & (int) byte.MaxValue) << 8) + ((int) this.m_DataInfoBuffer[anIndex * 8 + 7] & (int) byte.MaxValue) == 2)
        return new string(SupportClass.ToCharArray(this.m_DataBuffer), startIndex, length).ToCharArray();
      throw new UniRPCPacketException(UniRPCErrorMessage.GetErrorMessage(81004));
    }

    private int DataItemOffset(int anIndex)
    {
      int num1 = 0;
      for (int index = 0; index < anIndex; ++index)
      {
        switch ((((int) this.m_DataInfoBuffer[index * 8 + 4] & (int) byte.MaxValue) << 24) + (((int) this.m_DataInfoBuffer[index * 8 + 5] & (int) byte.MaxValue) << 16) + (((int) this.m_DataInfoBuffer[index * 8 + 6] & (int) byte.MaxValue) << 8) + ((int) this.m_DataInfoBuffer[index * 8 + 7] & (int) byte.MaxValue))
        {
          case 0:
            num1 += 4;
            break;
          case 1:
            num1 += 8;
            break;
          case 2:
          case 6:
            int num2 = (((int) this.m_DataInfoBuffer[index * 8] & (int) byte.MaxValue) << 24) + (((int) this.m_DataInfoBuffer[index * 8 + 1] & (int) byte.MaxValue) << 16) + (((int) this.m_DataInfoBuffer[index * 8 + 2] & (int) byte.MaxValue) << 8) + ((int) this.m_DataInfoBuffer[index * 8 + 3] & (int) byte.MaxValue) + 1;
            num1 = num1 + num2 + ((num2 + 3 & -4) - num2);
            break;
          case 3:
            int num3 = (((int) this.m_DataInfoBuffer[index * 8] & (int) byte.MaxValue) << 24) + (((int) this.m_DataInfoBuffer[index * 8 + 1] & (int) byte.MaxValue) << 16) + (((int) this.m_DataInfoBuffer[index * 8 + 2] & (int) byte.MaxValue) << 8) + ((int) this.m_DataInfoBuffer[index * 8 + 3] & (int) byte.MaxValue);
            num1 = num1 + num3 + ((num3 + 3 & -4) - num3);
            break;
          case 4:
            int num4 = (((int) this.m_DataInfoBuffer[index * 8] & (int) byte.MaxValue) << 24) + (((int) this.m_DataInfoBuffer[index * 8 + 1] & (int) byte.MaxValue) << 16) + (((int) this.m_DataInfoBuffer[index * 8 + 2] & (int) byte.MaxValue) << 8) + ((int) this.m_DataInfoBuffer[index * 8 + 3] & (int) byte.MaxValue);
            num1 += 4 * num4;
            break;
          case 5:
            int num5 = (((int) this.m_DataInfoBuffer[index * 8] & (int) byte.MaxValue) << 24) + (((int) this.m_DataInfoBuffer[index * 8 + 1] & (int) byte.MaxValue) << 16) + (((int) this.m_DataInfoBuffer[index * 8 + 2] & (int) byte.MaxValue) << 8) + ((int) this.m_DataInfoBuffer[index * 8 + 3] & (int) byte.MaxValue);
            num1 += 8 * num5;
            break;
          default:
            int num6 = (((int) this.m_DataInfoBuffer[index * 8] & (int) byte.MaxValue) << 24) + (((int) this.m_DataInfoBuffer[index * 8 + 1] & (int) byte.MaxValue) << 16) + (((int) this.m_DataInfoBuffer[index * 8 + 2] & (int) byte.MaxValue) << 8) + ((int) this.m_DataInfoBuffer[index * 8 + 3] & (int) byte.MaxValue);
            num1 = num1 + num6 + ((num6 + 3 & -4) - num6);
            break;
        }
      }
      return num1;
    }

    public byte ReadCompressionMask() => this.m_HeaderBuffer[13];

    public double ReadDouble(int anIndex)
    {
      int sourceIndex = anIndex < this.m_CurrentArgumentCount ? this.DataItemOffset(anIndex) : throw new UniRPCPacketException(UniRPCErrorMessage.GetErrorMessage(81004));
      if ((((int) this.m_DataInfoBuffer[anIndex * 8 + 4] & (int) byte.MaxValue) << 24) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 5] & (int) byte.MaxValue) << 16) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 6] & (int) byte.MaxValue) << 8) + ((int) this.m_DataInfoBuffer[anIndex * 8 + 7] & (int) byte.MaxValue) != 1)
        throw new UniRPCPacketException(UniRPCErrorMessage.GetErrorMessage(81004));
      byte[] destinationArray = new byte[8];
      Array.Copy((Array) this.m_DataInfoBuffer, sourceIndex, (Array) destinationArray, 0, 8);
      Array.Reverse((Array) destinationArray, 0, 8);
      return BitConverter.ToDouble(destinationArray, 0);
    }

    public double[] ReadDoubleArray(int anIndex)
    {
      int sourceIndex = anIndex < this.m_CurrentArgumentCount ? this.DataItemOffset(anIndex) : throw new UniRPCPacketException(UniRPCErrorMessage.GetErrorMessage(81004));
      int length = (((int) this.m_DataInfoBuffer[anIndex * 8] & (int) byte.MaxValue) << 24) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 1] & (int) byte.MaxValue) << 16) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 2] & (int) byte.MaxValue) << 8) + ((int) this.m_DataInfoBuffer[anIndex * 8 + 3] & (int) byte.MaxValue);
      int num = (((int) this.m_DataInfoBuffer[anIndex * 8 + 4] & (int) byte.MaxValue) << 24) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 5] & (int) byte.MaxValue) << 16) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 6] & (int) byte.MaxValue) << 8) + ((int) this.m_DataInfoBuffer[anIndex * 8 + 7] & (int) byte.MaxValue);
      double[] numArray = new double[length];
      if (num != 5)
        throw new UniRPCPacketException(UniRPCErrorMessage.GetErrorMessage(81004));
      for (int index = 0; index < length; ++index)
      {
        byte[] destinationArray = new byte[8];
        Array.Copy((Array) this.m_DataInfoBuffer, sourceIndex, (Array) destinationArray, 0, 8);
        Array.Reverse((Array) destinationArray, 0, 8);
        numArray[index] = BitConverter.ToDouble(destinationArray, 0);
      }
      return numArray;
    }

    public byte ReadEncryptionMask() => this.m_HeaderBuffer[14];

    public int ReadHeaderVersion() => (int) this.m_HeaderBuffer[1] & (int) byte.MaxValue;

    public int ReadHeaderVersionHighest() => (int) this.m_HeaderBuffer[12] & (int) byte.MaxValue;

    public int ReadInteger(int anIndex)
    {
      int num1 = anIndex < this.m_CurrentArgumentCount ? this.DataItemOffset(anIndex) : throw new UniRPCPacketException(UniRPCErrorMessage.GetErrorMessage(81004));
      if ((((int) this.m_DataInfoBuffer[anIndex * 8 + 4] & (int) byte.MaxValue) << 24) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 5] & (int) byte.MaxValue) << 16) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 6] & (int) byte.MaxValue) << 8) + ((int) this.m_DataInfoBuffer[anIndex * 8 + 7] & (int) byte.MaxValue) != 0)
        throw new UniRPCPacketException(UniRPCErrorMessage.GetErrorMessage(81004));
      byte[] dataBuffer1 = this.m_DataBuffer;
      int index1 = num1;
      int num2 = index1 + 1;
      int num3 = ((int) dataBuffer1[index1] & (int) byte.MaxValue) << 24;
      byte[] dataBuffer2 = this.m_DataBuffer;
      int index2 = num2;
      int num4 = index2 + 1;
      int num5 = ((int) dataBuffer2[index2] & (int) byte.MaxValue) << 16;
      int num6 = num3 + num5;
      byte[] dataBuffer3 = this.m_DataBuffer;
      int index3 = num4;
      int num7 = index3 + 1;
      int num8 = ((int) dataBuffer3[index3] & (int) byte.MaxValue) << 8;
      int num9 = num6 + num8;
      byte[] dataBuffer4 = this.m_DataBuffer;
      int index4 = num7;
      int num10 = index4 + 1;
      int num11 = (int) dataBuffer4[index4] & (int) byte.MaxValue;
      return num9 + num11;
    }

    public int[] ReadIntegerArray(int anIndex)
    {
      int num1 = anIndex < this.m_CurrentArgumentCount ? this.DataItemOffset(anIndex) : throw new UniRPCPacketException(UniRPCErrorMessage.GetErrorMessage(81004));
      int length = (((int) this.m_DataInfoBuffer[anIndex * 8] & (int) byte.MaxValue) << 24) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 1] & (int) byte.MaxValue) << 16) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 2] & (int) byte.MaxValue) << 8) + ((int) this.m_DataInfoBuffer[anIndex * 8 + 3] & (int) byte.MaxValue);
      int num2 = (((int) this.m_DataInfoBuffer[anIndex * 8 + 4] & (int) byte.MaxValue) << 24) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 5] & (int) byte.MaxValue) << 16) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 6] & (int) byte.MaxValue) << 8) + ((int) this.m_DataInfoBuffer[anIndex * 8 + 7] & (int) byte.MaxValue);
      int[] numArray1 = new int[length];
      if (num2 != 4)
        throw new UniRPCPacketException(UniRPCErrorMessage.GetErrorMessage(81004));
      for (int index1 = 0; index1 < length; ++index1)
      {
        int[] numArray2 = numArray1;
        int index2 = index1;
        byte[] dataBuffer1 = this.m_DataBuffer;
        int index3 = num1;
        int num3 = index3 + 1;
        int num4 = ((int) dataBuffer1[index3] & (int) byte.MaxValue) << 24;
        byte[] dataBuffer2 = this.m_DataBuffer;
        int index4 = num3;
        int num5 = index4 + 1;
        int num6 = ((int) dataBuffer2[index4] & (int) byte.MaxValue) << 16;
        int num7 = num4 + num6;
        byte[] dataBuffer3 = this.m_DataBuffer;
        int index5 = num5;
        int num8 = index5 + 1;
        int num9 = ((int) dataBuffer3[index5] & (int) byte.MaxValue) << 8;
        int num10 = num7 + num9;
        byte[] dataBuffer4 = this.m_DataBuffer;
        int index6 = num8;
        num1 = index6 + 1;
        int num11 = (int) dataBuffer4[index6] & (int) byte.MaxValue;
        int num12 = num10 + num11;
        numArray2[index2] = num12;
      }
      return numArray1;
    }

    public int ReadMessageType() => (((int) this.m_HeaderBuffer[8] & (int) byte.MaxValue) << 24) + (((int) this.m_HeaderBuffer[9] & (int) byte.MaxValue) << 16) + (((int) this.m_HeaderBuffer[10] & (int) byte.MaxValue) << 8) + ((int) this.m_HeaderBuffer[11] & (int) byte.MaxValue);

    public int ReadReturnCode() => (((int) this.m_HeaderBuffer[16] & (int) byte.MaxValue) << 24) + (((int) this.m_HeaderBuffer[17] & (int) byte.MaxValue) << 16) + (((int) this.m_HeaderBuffer[18] & (int) byte.MaxValue) << 8) + ((int) this.m_HeaderBuffer[19] & (int) byte.MaxValue);

    public byte[] ReadByteArray(int anIndex)
    {
      int sourceIndex = anIndex < this.m_CurrentArgumentCount ? this.DataItemOffset(anIndex) : throw new UniRPCPacketException(UniRPCErrorMessage.GetErrorMessage(81004));
      int length = (((int) this.m_DataInfoBuffer[anIndex * 8] & (int) byte.MaxValue) << 24) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 1] & (int) byte.MaxValue) << 16) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 2] & (int) byte.MaxValue) << 8) + ((int) this.m_DataInfoBuffer[anIndex * 8 + 3] & (int) byte.MaxValue);
      switch ((((int) this.m_DataInfoBuffer[anIndex * 8 + 4] & (int) byte.MaxValue) << 24) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 5] & (int) byte.MaxValue) << 16) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 6] & (int) byte.MaxValue) << 8) + ((int) this.m_DataInfoBuffer[anIndex * 8 + 7] & (int) byte.MaxValue))
      {
        case 2:
        case 3:
          byte[] destinationArray = new byte[length];
          Array.Copy((Array) this.m_DataBuffer, sourceIndex, (Array) destinationArray, 0, length);
          return destinationArray;
        default:
          throw new UniRPCPacketException(UniRPCErrorMessage.GetErrorMessage(81004));
      }
    }

    public int ReadType(int anIndex)
    {
      if (anIndex >= this.m_CurrentArgumentCount)
        throw new UniRPCPacketException(UniRPCErrorMessage.GetErrorMessage(81004));
      return (((int) this.m_DataInfoBuffer[anIndex * 8 + 4] & (int) byte.MaxValue) << 24) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 5] & (int) byte.MaxValue) << 16) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 6] & (int) byte.MaxValue) << 8) + ((int) this.m_DataInfoBuffer[anIndex * 8 + 7] & (int) byte.MaxValue);
    }

    public void Receive(BinaryReader aDataIn)
    {
      if (aDataIn == null)
        throw new UniRPCPacketException("The DataInputStream is dead." + UniRPCErrorMessage.GetErrorMessage(81002));
      try
      {
        this.m_In = aDataIn;
        this.m_HeaderBuffer = aDataIn.ReadBytes(this.m_HeaderBuffer.Length);
        int count = this.ReadHeaderLength();
        this.m_CurrentArgumentCount = this.ReadArgumentCount();
        this.m_DataBufferCompressed = new byte[count];
        this.m_DataBufferCompressed = aDataIn.ReadBytes(count);
        this.Decompress();
        this.Decrypt();
      }
      catch (IOException ex)
      {
        throw new UniRPCPacketException(ex.Message + UniRPCErrorMessage.GetErrorMessage(81009));
      }
    }

    public void RetryReceive()
    {
        if (this.m_In == null)
            throw new UniRPCPacketException("The DataInputStream is dead." + UniRPCErrorMessage.GetErrorMessage(81002));
        try
        {
            this.m_HeaderBuffer = this.m_In.ReadBytes(this.m_HeaderBuffer.Length);
            int count = this.ReadHeaderLength();
            this.m_CurrentArgumentCount = this.ReadArgumentCount();
            this.m_DataBufferCompressed = new byte[count];
            this.m_DataBufferCompressed = this.m_In.ReadBytes(count);
            this.Decompress();
            this.Decrypt();
        }
        catch (IOException ex)
        {
            throw new UniRPCPacketException(ex.Message + UniRPCErrorMessage.GetErrorMessage(81009));
        }
    }

    private int ReadHeaderLength() => (((int) this.m_HeaderBuffer[4] & (int) byte.MaxValue) << 24) + (((int) this.m_HeaderBuffer[5] & (int) byte.MaxValue) << 16) + (((int) this.m_HeaderBuffer[6] & (int) byte.MaxValue) << 8) + ((int) this.m_HeaderBuffer[7] & (int) byte.MaxValue);

    private int ReadArgumentCount() => (((int) this.m_HeaderBuffer[20] & (int) byte.MaxValue) << 8) + ((int) this.m_HeaderBuffer[21] & (int) byte.MaxValue);

    private void Decompress()
    {
      if (this.m_HeaderBuffer[13] == (byte) 1)
        this.m_DataBufferEncrypted = this.m_DataBufferCompressed;
      else
        this.m_DataBufferEncrypted = this.m_DataBufferCompressed;
    }

    private void Decrypt()
    {
      byte num = this.ReadEncryptionMask();
      int aLength = this.m_DataBufferEncrypted.Length - this.m_CurrentArgumentCount * 8;
      this.ResizeDataBuffer(aLength);
      this.m_DataOffset = aLength;
      if (num == (byte) 1)
      {
        int index1;
        for (index1 = 0; index1 < this.m_CurrentArgumentCount * 8; ++index1)
          this.m_DataInfoBuffer[index1] = (byte) ((uint) this.m_DataBufferEncrypted[index1] ^ (uint) (byte) this.CurrentVersion);
        int index2 = 0;
        while (index2 < this.m_DataOffset)
        {
          this.m_DataBuffer[index2] = (byte) ((uint) this.m_DataBufferEncrypted[index1] ^ (uint) (byte) this.CurrentVersion);
          ++index2;
          ++index1;
        }
      }
      else
      {
        Array.Copy((Array) this.m_DataBufferEncrypted, 0, (Array) this.m_DataInfoBuffer, 0, this.m_CurrentArgumentCount * 8);
        Array.Copy((Array) this.m_DataBufferEncrypted, this.m_CurrentArgumentCount * 8, (Array) this.m_DataBuffer, 0, this.m_DataOffset);
      }
      this.WriteHeaderLength(this.m_CurrentArgumentCount * 8 + this.m_DataOffset);
    }

    private void ResizeDataBuffer(int aLength)
    {
      if (aLength <= this.m_DataBuffer.Length && this.m_DataBuffer.Length == 32768 || aLength <= this.m_DataBuffer.Length)
        return;
      if (aLength > this.m_DataBuffer.Length)
        this.m_DataBuffer = new byte[(aLength / 32768 + 1) * 32768];
      else
        this.m_DataBuffer = new byte[32768];
    }

    public void Send(BinaryWriter aDataOut, byte anEncryptionMask)
    {
      if (aDataOut == null)
        throw new UniRPCPacketException("The DataOutputStream is dead." + UniRPCErrorMessage.GetErrorMessage(81002));
      try
      {
        if (!this.m_IsEncrypted)
        {
          this.Encrypt(anEncryptionMask);
          this.m_IsEncrypted = true;
        }
        if (!this.m_IsCompressed)
        {
          this.Compress();
          this.m_IsCompressed = true;
        }
        int rpcdump = UniObjects.Trace.RPCDUMP;
        if (rpcdump > 3)
          this.Dump(rpcdump);
        byte[] headerBuffer = this.m_HeaderBuffer;
        aDataOut.Write(headerBuffer);
        byte[] bufferCompressed = this.m_DataBufferCompressed;
        aDataOut.Write(bufferCompressed);
        this.m_IsEncrypted = false;
        this.m_IsCompressed = false;
      }
      catch (IOException ex)
      {
        throw new UniRPCPacketException(ex.Message + UniRPCErrorMessage.GetErrorMessage(81009));
      }
    }

    public void SetCompressionThreshold(int aThreshold) => this.m_CurrentCompressionThreshold = this.IsCompressionSupported() || aThreshold == 0 ? aThreshold : throw new UniRPCPacketException("Compression is not supported on this connection." + UniRPCErrorMessage.GetErrorMessage(81026));

    public void SetVersionCurrent(int aVersion)
    {
      this.CurrentVersion = aVersion;
      this.m_HeaderBuffer[0] = (byte) 108;
      this.m_HeaderBuffer[1] = (byte) (SupportClass.URShift(this.CurrentVersion, 0) & (int) byte.MaxValue);
    }

    public void WriteString(int anIndex, string aString)
    {
      if (anIndex == 0)
      {
        this.ResetData();
      }
      else
      {
        if (anIndex != this.m_CurrentArgumentCount)
          throw new UniRPCPacketException("All packet arguments must be inserted in order, starting with zero." + UniRPCErrorMessage.GetErrorMessage(81010));
        if (anIndex >= 2048)
          throw new UniRPCPacketException("A maximum of " + (object) 2048 + " packet arguments are supported." + UniRPCErrorMessage.GetErrorMessage(81010));
      }
      byte[] byteArray = SupportClass.ToByteArray(aString);
      this.m_DataInfoBuffer[anIndex * 8] = (byte) (SupportClass.URShift(byteArray.Length, 24) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 1] = (byte) (SupportClass.URShift(byteArray.Length, 16) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 2] = (byte) (SupportClass.URShift(byteArray.Length, 8) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 3] = (byte) (SupportClass.URShift(byteArray.Length, 0) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 4] = (byte) (SupportClass.URShift(3, 24) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 5] = (byte) (SupportClass.URShift(3, 16) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 6] = (byte) (SupportClass.URShift(3, 8) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 7] = (byte) (SupportClass.URShift(3, 0) & (int) byte.MaxValue);
      ++this.m_CurrentArgumentCount;
      if (byteArray.Length + 3 > this.m_DataBuffer.Length - this.m_DataOffset)
        this.IncreaseDataBuffer(byteArray.Length + 3);
      for (int index = 0; index < byteArray.Length; ++index)
        this.m_DataBuffer[this.m_DataOffset++] = byteArray[index];
      this.m_DataOffset += (byteArray.Length + 3 & -4) - byteArray.Length;
    }

    public void WriteByteArray(int anIndex, byte[] bArray)
    {
      if (anIndex == 0)
      {
        this.ResetData();
      }
      else
      {
        if (anIndex != this.m_CurrentArgumentCount)
          throw new UniRPCPacketException("All packet arguments must be inserted in order, starting with zero." + UniRPCErrorMessage.GetErrorMessage(81010));
        if (anIndex >= 2048)
          throw new UniRPCPacketException("A maximum of " + (object) 2048 + " packet arguments are supported." + UniRPCErrorMessage.GetErrorMessage(81010));
      }
      this.m_DataInfoBuffer[anIndex * 8] = (byte) (SupportClass.URShift(bArray.Length, 24) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 1] = (byte) (SupportClass.URShift(bArray.Length, 16) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 2] = (byte) (SupportClass.URShift(bArray.Length, 8) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 3] = (byte) (SupportClass.URShift(bArray.Length, 0) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 4] = (byte) (SupportClass.URShift(3, 24) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 5] = (byte) (SupportClass.URShift(3, 16) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 6] = (byte) (SupportClass.URShift(3, 8) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 7] = (byte) (SupportClass.URShift(3, 0) & (int) byte.MaxValue);
      ++this.m_CurrentArgumentCount;
      if (bArray.Length + 3 > this.m_DataBuffer.Length - this.m_DataOffset)
        this.IncreaseDataBuffer(bArray.Length + 3);
      for (int index = 0; index < bArray.Length; ++index)
        this.m_DataBuffer[this.m_DataOffset++] = bArray[index];
      this.m_DataOffset += (bArray.Length + 3 & -4) - bArray.Length;
    }

    private void IncreaseDataBuffer(int length)
    {
      byte[] destinationArray = new byte[this.m_DataBuffer.Length + (length / 32768 + 1) * 32768];
      Array.Copy((Array) this.m_DataBuffer, 0, (Array) destinationArray, 0, this.m_DataBuffer.Length);
      this.m_DataBuffer = destinationArray;
    }

    public void WriteCharArray(int anIndex, char[] aCharArray)
    {
      if (anIndex == 0)
      {
        this.ResetData();
      }
      else
      {
        if (anIndex != this.m_CurrentArgumentCount)
          throw new UniRPCPacketException("All packet arguments must be inserted in order, starting with zero." + UniRPCErrorMessage.GetErrorMessage(81010));
        if (anIndex >= 2048)
          throw new UniRPCPacketException("A maximum of " + (object) 2048 + " packet arguments are supported." + UniRPCErrorMessage.GetErrorMessage(81010));
      }
      byte[] numArray = new byte[aCharArray.Length + 1];
      for (int index = 0; index < aCharArray.Length; ++index)
        numArray[index] = (byte) ((uint) aCharArray[index] & (uint) byte.MaxValue);
      numArray[aCharArray.Length] = (byte) 0;
      this.m_DataInfoBuffer[anIndex * 8] = (byte) (SupportClass.URShift(numArray.Length - 1, 24) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 1] = (byte) (SupportClass.URShift(numArray.Length - 1, 16) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 2] = (byte) (SupportClass.URShift(numArray.Length - 1, 8) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 3] = (byte) (SupportClass.URShift(numArray.Length - 1, 0) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 4] = (byte) (SupportClass.URShift(2, 24) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 5] = (byte) (SupportClass.URShift(2, 16) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 6] = (byte) (SupportClass.URShift(2, 8) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 7] = (byte) (SupportClass.URShift(2, 0) & (int) byte.MaxValue);
      ++this.m_CurrentArgumentCount;
      if (numArray.Length + 3 > this.m_DataBuffer.Length - this.m_DataOffset)
        this.IncreaseDataBuffer(numArray.Length + 3);
      for (int index = 0; index < numArray.Length; ++index)
        this.m_DataBuffer[this.m_DataOffset++] = numArray[index];
      this.m_DataOffset += (numArray.Length + 3 & -4) - numArray.Length;
    }

    public void WriteDouble(int anIndex, double aDouble)
    {
      if (anIndex == 0)
      {
        this.ResetData();
      }
      else
      {
        if (anIndex != this.m_CurrentArgumentCount)
          throw new UniRPCPacketException("All packet arguments must be inserted in order, starting with zero." + UniRPCErrorMessage.GetErrorMessage(81010));
        if (anIndex >= 2048)
          throw new UniRPCPacketException("A maximum of " + (object) 2048 + " packet arguments are supported." + UniRPCErrorMessage.GetErrorMessage(81010));
      }
      this.m_DataInfoBuffer[anIndex * 8] = (byte) (SupportClass.URShift(1, 24) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 1] = (byte) (SupportClass.URShift(1, 16) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 2] = (byte) (SupportClass.URShift(1, 8) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 3] = (byte) (SupportClass.URShift(1, 0) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 4] = (byte) (SupportClass.URShift(1, 24) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 5] = (byte) (SupportClass.URShift(1, 16) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 6] = (byte) (SupportClass.URShift(1, 8) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 7] = (byte) (SupportClass.URShift(1, 0) & (int) byte.MaxValue);
      ++this.m_CurrentArgumentCount;
      if (8 > this.m_DataBuffer.Length - this.m_DataOffset)
        this.IncreaseDataBuffer(8);
      long int64 = Convert.ToInt64(aDouble);
      this.m_DataBuffer[this.m_DataOffset++] = (byte) ((uint) (int) SupportClass.URShift(int64, 56) & (uint) byte.MaxValue);
      this.m_DataBuffer[this.m_DataOffset++] = (byte) ((uint) (int) SupportClass.URShift(int64, 48) & (uint) byte.MaxValue);
      this.m_DataBuffer[this.m_DataOffset++] = (byte) ((uint) (int) SupportClass.URShift(int64, 40) & (uint) byte.MaxValue);
      this.m_DataBuffer[this.m_DataOffset++] = (byte) ((uint) (int) SupportClass.URShift(int64, 32) & (uint) byte.MaxValue);
      this.m_DataBuffer[this.m_DataOffset++] = (byte) ((uint) (int) SupportClass.URShift(int64, 24) & (uint) byte.MaxValue);
      this.m_DataBuffer[this.m_DataOffset++] = (byte) ((uint) (int) SupportClass.URShift(int64, 16) & (uint) byte.MaxValue);
      this.m_DataBuffer[this.m_DataOffset++] = (byte) ((uint) (int) SupportClass.URShift(int64, 8) & (uint) byte.MaxValue);
      this.m_DataBuffer[this.m_DataOffset++] = (byte) ((uint) (int) SupportClass.URShift(int64, 0) & (uint) byte.MaxValue);
    }

    public void WriteDoubleArray(int anIndex, double[] aDouble)
    {
      if (anIndex == 0)
      {
        this.ResetData();
      }
      else
      {
        if (anIndex != this.m_CurrentArgumentCount)
          throw new UniRPCPacketException("All packet arguments must be inserted in order, starting with zero." + UniRPCErrorMessage.GetErrorMessage(81010));
        if (anIndex >= 2048)
          throw new UniRPCPacketException("A maximum of " + (object) 2048 + " packet arguments are supported." + UniRPCErrorMessage.GetErrorMessage(81010));
      }
      this.m_DataInfoBuffer[anIndex * 8] = (byte) (SupportClass.URShift(1, 24) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 1] = (byte) (SupportClass.URShift(1, 16) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 2] = (byte) (SupportClass.URShift(1, 8) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 3] = (byte) (SupportClass.URShift(1, 0) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 4] = (byte) (SupportClass.URShift(5, 24) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 5] = (byte) (SupportClass.URShift(5, 16) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 6] = (byte) (SupportClass.URShift(5, 8) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 7] = (byte) (SupportClass.URShift(5, 0) & (int) byte.MaxValue);
      ++this.m_CurrentArgumentCount;
      if (aDouble.Length * 8 > this.m_DataBuffer.Length - this.m_DataOffset)
        this.IncreaseDataBuffer(aDouble.Length * 8);
      for (int index = 0; index < aDouble.Length; ++index)
      {
        long int64 = Convert.ToInt64(aDouble[index]);
        this.m_DataBuffer[this.m_DataOffset++] = (byte) ((uint) (int) SupportClass.URShift(int64, 56) & (uint) byte.MaxValue);
        this.m_DataBuffer[this.m_DataOffset++] = (byte) ((uint) (int) SupportClass.URShift(int64, 48) & (uint) byte.MaxValue);
        this.m_DataBuffer[this.m_DataOffset++] = (byte) ((uint) (int) SupportClass.URShift(int64, 40) & (uint) byte.MaxValue);
        this.m_DataBuffer[this.m_DataOffset++] = (byte) ((uint) (int) SupportClass.URShift(int64, 32) & (uint) byte.MaxValue);
        this.m_DataBuffer[this.m_DataOffset++] = (byte) ((uint) (int) SupportClass.URShift(int64, 24) & (uint) byte.MaxValue);
        this.m_DataBuffer[this.m_DataOffset++] = (byte) ((uint) (int) SupportClass.URShift(int64, 16) & (uint) byte.MaxValue);
        this.m_DataBuffer[this.m_DataOffset++] = (byte) ((uint) (int) SupportClass.URShift(int64, 8) & (uint) byte.MaxValue);
        this.m_DataBuffer[this.m_DataOffset++] = (byte) ((uint) (int) SupportClass.URShift(int64, 0) & (uint) byte.MaxValue);
      }
    }

    public void WriteInteger(int anIndex, int anInteger)
    {
      if (anIndex == 0)
      {
        this.ResetData();
      }
      else
      {
        if (anIndex != this.m_CurrentArgumentCount)
          throw new UniRPCPacketException("All packet arguments must be inserted in order, starting with zero." + UniRPCErrorMessage.GetErrorMessage(81010));
        if (anIndex >= 2048)
          throw new UniRPCPacketException("A maximum of " + (object) 2048 + " packet arguments are supported." + UniRPCErrorMessage.GetErrorMessage(81010));
      }
      this.m_DataInfoBuffer[anIndex * 8] = (byte) (SupportClass.URShift(1, 24) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 1] = (byte) (SupportClass.URShift(1, 16) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 2] = (byte) (SupportClass.URShift(1, 8) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 3] = (byte) (SupportClass.URShift(1, 0) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 4] = (byte) (SupportClass.URShift(0, 24) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 5] = (byte) (SupportClass.URShift(0, 16) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 6] = (byte) (SupportClass.URShift(0, 8) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 7] = (byte) (SupportClass.URShift(0, 0) & (int) byte.MaxValue);
      ++this.m_CurrentArgumentCount;
      if (4 > this.m_DataBuffer.Length - this.m_DataOffset)
        this.IncreaseDataBuffer(4);
      this.m_DataBuffer[this.m_DataOffset++] = (byte) (SupportClass.URShift(anInteger, 24) & (int) byte.MaxValue);
      this.m_DataBuffer[this.m_DataOffset++] = (byte) (SupportClass.URShift(anInteger, 16) & (int) byte.MaxValue);
      this.m_DataBuffer[this.m_DataOffset++] = (byte) (SupportClass.URShift(anInteger, 8) & (int) byte.MaxValue);
      this.m_DataBuffer[this.m_DataOffset++] = (byte) (SupportClass.URShift(anInteger, 0) & (int) byte.MaxValue);
    }

    public void WriteArrayInteger(int anIndex, int[] anInteger)
    {
      if (anIndex == 0)
      {
        this.ResetData();
      }
      else
      {
        if (anIndex != this.m_CurrentArgumentCount)
          throw new UniRPCPacketException("All packet arguments must be inserted in order, starting with zero." + UniRPCErrorMessage.GetErrorMessage(81010));
        if (anIndex >= 2048)
          throw new UniRPCPacketException("A maximum of " + (object) 2048 + " packet arguments are supported." + UniRPCErrorMessage.GetErrorMessage(81010));
      }
      this.m_DataInfoBuffer[anIndex * 8] = (byte) (SupportClass.URShift(1, 24) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 1] = (byte) (SupportClass.URShift(1, 16) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 2] = (byte) (SupportClass.URShift(1, 8) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 3] = (byte) (SupportClass.URShift(1, 0) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 4] = (byte) (SupportClass.URShift(4, 24) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 5] = (byte) (SupportClass.URShift(4, 16) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 6] = (byte) (SupportClass.URShift(4, 8) & (int) byte.MaxValue);
      this.m_DataInfoBuffer[anIndex * 8 + 7] = (byte) (SupportClass.URShift(4, 0) & (int) byte.MaxValue);
      ++this.m_CurrentArgumentCount;
      if (anInteger.Length * 4 > this.m_DataBuffer.Length - this.m_DataOffset)
        this.IncreaseDataBuffer(anInteger.Length * 4);
      for (int index = 0; index < anInteger.Length; ++index)
      {
        this.m_DataBuffer[this.m_DataOffset++] = (byte) (SupportClass.URShift(anInteger[index], 24) & (int) byte.MaxValue);
        this.m_DataBuffer[this.m_DataOffset++] = (byte) (SupportClass.URShift(anInteger[index], 16) & (int) byte.MaxValue);
        this.m_DataBuffer[this.m_DataOffset++] = (byte) (SupportClass.URShift(anInteger[index], 8) & (int) byte.MaxValue);
        this.m_DataBuffer[this.m_DataOffset++] = (byte) (SupportClass.URShift(anInteger[index], 0) & (int) byte.MaxValue);
      }
    }

    public void WriteMessageType(int aType)
    {
      this.m_HeaderBuffer[8] = (byte) (SupportClass.URShift(aType, 24) & (int) byte.MaxValue);
      this.m_HeaderBuffer[9] = (byte) (SupportClass.URShift(aType, 16) & (int) byte.MaxValue);
      this.m_HeaderBuffer[10] = (byte) (SupportClass.URShift(aType, 8) & (int) byte.MaxValue);
      this.m_HeaderBuffer[11] = (byte) (SupportClass.URShift(aType, 0) & (int) byte.MaxValue);
    }

    public void WriteReturnCode(int aReturnCode)
    {
      this.m_HeaderBuffer[16] = (byte) (SupportClass.URShift(aReturnCode, 24) & (int) byte.MaxValue);
      this.m_HeaderBuffer[17] = (byte) (SupportClass.URShift(aReturnCode, 16) & (int) byte.MaxValue);
      this.m_HeaderBuffer[18] = (byte) (SupportClass.URShift(aReturnCode, 8) & (int) byte.MaxValue);
      this.m_HeaderBuffer[19] = (byte) (SupportClass.URShift(aReturnCode, 0) & (int) byte.MaxValue);
    }

    internal void Dump(int debugLevel)
    {
      if (debugLevel > 5)
      {
        Trace.WriteLine("Packet Header Detail:");
        if (debugLevel > 7)
        {
          Trace.WriteLine(" headerBuffer=0x");
          for (int index = 0; index < this.m_HeaderBuffer.Length; ++index)
          {
            string message = Convert.ToString((int) this.m_HeaderBuffer[index] & (int) byte.MaxValue, 16);
            if (message.Length == 1)
              Trace.WriteLine("0" + message);
            else
              Trace.WriteLine(message);
            if ((index + 1) % 4 == 0)
              Trace.WriteLine(" ");
          }
          Trace.WriteLine("");
        }
        Trace.WriteLine(" Version=" + (object) ((int) this.m_HeaderBuffer[1] & (int) byte.MaxValue));
        Trace.WriteLine(" Sequence=" + (object) ((((int) this.m_HeaderBuffer[2] & (int) byte.MaxValue) << 8) + ((int) this.m_HeaderBuffer[3] & (int) byte.MaxValue)));
        if (this.m_IsEncrypted && this.m_IsCompressed)
          Trace.WriteLine(" Encrypted & Compressed Length=");
        else if (this.m_IsEncrypted)
          Trace.WriteLine(" Encrypted Length=");
        else
          Trace.WriteLine(" Data Length=");
        Trace.WriteLine((object) ((((int) this.m_HeaderBuffer[4] & (int) byte.MaxValue) << 24) + (((int) this.m_HeaderBuffer[5] & (int) byte.MaxValue) << 16) + (((int) this.m_HeaderBuffer[6] & (int) byte.MaxValue) << 8) + ((int) this.m_HeaderBuffer[7] & (int) byte.MaxValue)));
        Trace.WriteLine(" Type=" + (object) ((((int) this.m_HeaderBuffer[8] & (int) byte.MaxValue) << 24) + (((int) this.m_HeaderBuffer[9] & (int) byte.MaxValue) << 16) + (((int) this.m_HeaderBuffer[10] & (int) byte.MaxValue) << 8) + ((int) this.m_HeaderBuffer[11] & (int) byte.MaxValue)));
        Trace.WriteLine(" Highest Supported Version=" + (object) ((int) this.m_HeaderBuffer[12] & (int) byte.MaxValue));
        Trace.WriteLine(" Compression Mask=0x" + Convert.ToString((int) this.m_HeaderBuffer[13] & (int) byte.MaxValue, 16));
        Trace.WriteLine(" Encryption Mask=0x" + Convert.ToString((int) this.m_HeaderBuffer[14] & (int) byte.MaxValue, 16));
        Trace.WriteLine(" Return Code=" + (object) ((((int) this.m_HeaderBuffer[16] & (int) byte.MaxValue) << 24) + (((int) this.m_HeaderBuffer[17] & (int) byte.MaxValue) << 16) + (((int) this.m_HeaderBuffer[18] & (int) byte.MaxValue) << 8) + ((int) this.m_HeaderBuffer[19] & (int) byte.MaxValue)));
        Trace.WriteLine(" Argument Count=" + (object) ((((int) this.m_HeaderBuffer[20] & (int) byte.MaxValue) << 8) + ((int) this.m_HeaderBuffer[21] & (int) byte.MaxValue)));
        Trace.WriteLine(" Proc Length=" + (object) ((((int) this.m_HeaderBuffer[22] & (int) byte.MaxValue) << 8) + ((int) this.m_HeaderBuffer[23] & (int) byte.MaxValue)));
      }
      if (debugLevel <= 3)
        return;
      Trace.WriteLine("Packet Data Detail:");
      if (debugLevel > 7)
      {
        Trace.WriteLine(" dataInfoBuffer=0x");
        for (int index = 0; index < this.m_CurrentArgumentCount * 8; ++index)
        {
          string message = Convert.ToString((int) this.m_DataInfoBuffer[index] & (int) byte.MaxValue, 16);
          if (message.Length == 1)
            Trace.WriteLine("0" + message);
          else
            Trace.WriteLine(message);
          if ((index + 1) % 4 == 0)
          {
            if ((index + 1) % 24 == 0 && index + 1 < this.m_CurrentArgumentCount * 8)
            {
              Trace.WriteLine("");
              Trace.WriteLine("                0x");
            }
            else
              Trace.WriteLine(" ");
          }
        }
        Trace.WriteLine("");
        Trace.WriteLine(" dataBuffer=0x");
        for (int index = 0; index < this.m_DataOffset; ++index)
        {
          string message = Convert.ToString((int) this.m_DataBuffer[index] & (int) byte.MaxValue, 16);
          if (message.Length == 1)
            Trace.WriteLine("0" + message);
          else if (message.Length == 2)
            Trace.WriteLine(message);
          else
            Trace.WriteLine("(" + message + ")");
          if ((index + 1) % 4 == 0)
          {
            if ((index + 1) % 24 == 0)
            {
              Trace.WriteLine("");
              Trace.WriteLine("            0x");
            }
            else
              Trace.WriteLine(" ");
          }
        }
        Trace.WriteLine("");
        if (debugLevel > 8)
        {
          Trace.WriteLine(" dataBufferEncrypted=0x");
          for (int index = 0; index < this.m_DataBufferEncrypted.Length; ++index)
          {
            string message = Convert.ToString((int) this.m_DataBufferEncrypted[index] & (int) byte.MaxValue, 16);
            if (message.Length == 1)
              Trace.WriteLine("0" + message);
            else if (message.Length == 2)
              Trace.WriteLine(message);
            else
              Trace.WriteLine("(" + message + ")");
            if ((index + 1) % 4 == 0)
            {
              if ((index + 1) % 24 == 0)
              {
                Trace.WriteLine("");
                Trace.WriteLine("                     0x");
              }
              else
                Trace.WriteLine(" ");
            }
          }
          Trace.WriteLine("");
          Trace.WriteLine(" dataBufferCompressed=0x");
          for (int index = 0; index < this.m_DataBufferCompressed.Length; ++index)
          {
            string message = Convert.ToString((int) this.m_DataBufferCompressed[index] & (int) byte.MaxValue, 16);
            if (message.Length == 1)
              Trace.WriteLine("0" + message);
            else if (message.Length == 2)
              Trace.WriteLine(message);
            else
              Trace.WriteLine("(" + message + ")");
            if ((index + 1) % 4 == 0)
            {
              if ((index + 1) % 24 == 0)
              {
                Trace.WriteLine("");
                Trace.WriteLine("                      0x");
              }
              else
                Trace.WriteLine(" ");
            }
          }
          Trace.WriteLine("");
        }
      }
      for (int anIndex = 0; anIndex < this.m_CurrentArgumentCount; ++anIndex)
      {
        Trace.Write(" Argument[" + (object) anIndex + "]");
        Trace.Write("=Length=" + (object) ((((int) this.m_DataInfoBuffer[anIndex * 8] & (int) byte.MaxValue) << 24) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 1] & (int) byte.MaxValue) << 16) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 2] & (int) byte.MaxValue) << 8) + ((int) this.m_DataInfoBuffer[anIndex * 8 + 3] & (int) byte.MaxValue)));
        int num = (((int) this.m_DataInfoBuffer[anIndex * 8 + 4] & (int) byte.MaxValue) << 24) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 5] & (int) byte.MaxValue) << 16) + (((int) this.m_DataInfoBuffer[anIndex * 8 + 6] & (int) byte.MaxValue) << 8) + ((int) this.m_DataInfoBuffer[anIndex * 8 + 7] & (int) byte.MaxValue);
        Trace.Write("=Type=" + (object) num);
        Trace.Write("=Value=");
        try
        {
          switch (num)
          {
            case 0:
              Trace.Write((object) this.ReadInteger(anIndex));
              break;
            case 1:
              Trace.Write((object) this.ReadDouble(anIndex));
              break;
            case 2:
              Trace.Write(new string(this.ReadCharArray(anIndex)));
              break;
            case 3:
              break;
            case 4:
              foreach (int readInteger in this.ReadIntegerArray(anIndex))
                Trace.Write(readInteger.ToString() + ",");
              break;
            case 5:
              foreach (double readDouble in this.ReadDoubleArray(anIndex))
                Trace.Write(readDouble.ToString() + ",");
              break;
            default:
              Trace.Write("Unrecognized Data Type");
              break;
          }
        }
        catch (UniRPCPacketException ex)
        {
          Trace.WriteLine("UniRPCMessage.dump():PacketException:" + ex.Message);
        }
        Trace.WriteLine("");
      }
      Trace.WriteLine("");
    }
  }
}
