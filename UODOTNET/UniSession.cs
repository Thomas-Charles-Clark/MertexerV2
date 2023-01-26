// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniSession
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace IBMU2.UODOTNET
{
  public class UniSession : UniRoot
  {
    private const string m_MAC_ADDRESS = "JAVA";
    private const string m_DEVICE_NAME = "unknown host";
    private string m_UserID = "";
    private string m_Password = "";
    private string m_Account = "";
    private string m_HostName = "";
    private bool m_IsActive = false;
    private bool m_IsRPCError = false;
    private UniRPCConnection m_Conn = (UniRPCConnection) null;
    private byte[] m_UniMarkCharacters;
    private string m_LicenseToken = "";
    private string m_UniSubKey = "";
    private int m_HostPort;
    private int m_TransportType;
    private int m_CompressionThreshold = 0;
    private int m_Timeout = UniObjects.TimeOut;
    private int m_EncryptionType = 0;
    private int m_HostType = 0;
    private bool m_IsNLSEnabled = false;
    private bool m_IsNLSLocalesEnabled = false;
    private bool m_IsNLSUnidataSession = false;
    private UniCommand m_UniCommand = (UniCommand) null;
    private UniTransaction m_UniTransaction = (UniTransaction) null;
    private UniNLSLocale m_UniNLSLocale = (UniNLSLocale) null;
    private UniNLSMap m_UniNLSMap = (UniNLSMap) null;
    private int m_BlockingStrategy = 2;
    private int m_LockStrategy = 0;
    private int m_ReleaseStrategy = 12;
    private int m_UniReturnCode = 0;
    private int m_NumOpenFiles = 0;
    private int m_UniStatus = 0;
    private DateTime m_CreationTime;
    private DateTime m_FreedTime;
    private static Encoding m_Encoding = Encoding.Default;
    private string m_Service = "defcs";
    private int m_MaxOpenFiles = 32;
    private bool m_IsPooledSession = false;

    public UniDataSet CreateUniDataSet()
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::CreateUniDataSet() no argument");
      UniDataSet uniDataSet = new UniDataSet(this);
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::CreateUniDataSet() no argument");
      return uniDataSet;
    }

    internal UniDataSet CreateUniDataSet(
      string[] RecId,
      byte[] RetData,
      byte[] StatusData,
      byte[] RetValData)
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::CreateUniDataSet() with argument");
      UniDataSet uniDataSet = new UniDataSet(this, RecId, RetData, StatusData, RetValData);
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::CreateUniDataSet() with argument");
      return uniDataSet;
    }

    public UniXML CreateUniXML() => new UniXML(this);

    public UniNLSLocale CreateUniNLSLocale()
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::CreateUniNLSLocale()");
      this.CheckEntryConditions();
      if (this.m_IsNLSEnabled && !this.m_IsNLSUnidataSession && this.m_UniNLSLocale == null)
        this.m_UniNLSLocale = new UniNLSLocale(this);
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::CreateUniNLSLocale()");
      return this.m_UniNLSLocale;
    }

    public UniNLSMap CreateUniNLSMap()
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::CreateUniNLSMap()");
      this.CheckEntryConditions();
      if (this.m_IsNLSEnabled && !this.m_IsNLSUnidataSession && this.m_UniNLSMap == null)
        this.m_UniNLSMap = new UniNLSMap(this);
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::CreateUniNLSMap()");
      return this.m_UniNLSMap;
    }

    public UniFile CreateUniFile(string pFileName)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::CreateUniFile()");
        this.CheckEntryConditions();
        try
        {
          UniFile uniFile = new UniFile(this, pFileName, 0);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::CreateUniFile()");
          return uniFile;
        }
        catch (UniFileException ex)
        {
          string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          throw new UniSessionException(message);
        }
      }
    }

    public UniDictionary CreateUniDictionary(string pFileName)
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::CreateUniDictionary()");
      this.CheckEntryConditions();
      try
      {
        UniDictionary uniDictionary = new UniDictionary(this, pFileName, 1);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::CreateUniDictionary()");
        return uniDictionary;
      }
      catch (UniFileException ex)
      {
        string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
        throw new UniSessionException(message);
      }
    }

    public UniSelectList CreateUniSelectList(int aSelectListNumber)
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::CreateUniSelectList()");
      this.CheckEntryConditions();
      try
      {
        UniSelectList uniSelectList = new UniSelectList(this, aSelectListNumber);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::CreateUniSelectList()");
        return uniSelectList;
      }
      catch (UniSelectListException ex)
      {
        string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
        throw new UniSessionException(message);
      }
    }

    public UniSubroutine CreateUniSubroutine(string aSubName, int aNumArgs)
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::CreateUniSubroutine()");
      this.CheckEntryConditions();
      try
      {
        UniSubroutine uniSubroutine = new UniSubroutine(this, aSubName, aNumArgs);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::CreateUniSubroutine()");
        return uniSubroutine;
      }
      catch (UniSubroutineException ex)
      {
        string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
        throw new UniSessionException(message);
      }
    }

    public UniTransaction CreateUniTransaction()
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::CreateUniTransaction()");
      this.CheckEntryConditions();
      if (this.m_UniTransaction == null)
      {
        try
        {
          this.m_UniTransaction = new UniTransaction(this);
        }
        catch (UniTransactionException ex)
        {
          string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          throw new UniSessionException(message);
        }
      }
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::CreateUniTransaction()");
      return this.m_UniTransaction;
    }

    public UniDynArray CreateUniDynArray(string s)
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::CreateUniDynArray()");
      this.CheckEntryConditions();
      try
      {
        UniDynArray uniDynArray = new UniDynArray(this, s);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::CreateUniDynArray()");
        return uniDynArray;
      }
      catch (UniDynArrayException ex)
      {
        string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
        throw new UniSessionException(message);
      }
    }

    public UniDynArray CreateUniDynArray()
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::CreateUniDynArray()");
      this.CheckEntryConditions();
      try
      {
        UniDynArray uniDynArray = new UniDynArray(this);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::CreateUniDynArray()");
        return uniDynArray;
      }
      catch (UniDynArrayException ex)
      {
        string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
        throw new UniSessionException(message);
      }
    }

    public UniSequentialFile CreateSequentialFile(
      string pFileName,
      string pRecordID,
      bool pCreateFlag)
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::CreateSequentialFile()");
      this.CheckEntryConditions();
      try
      {
        UniSequentialFile sequentialFile = new UniSequentialFile(this, pFileName, pRecordID, pCreateFlag);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::CreateSequentialFile()");
        return sequentialFile;
      }
      catch (UniSequentialFileException ex)
      {
        string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
        throw new UniSessionException(message);
      }
    }

    public UniCommand CreateUniCommand()
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::CreateUniCommand()");
      this.CheckEntryConditions();
      if (this.m_UniCommand == null)
      {
        try
        {
          this.m_UniCommand = new UniCommand(this);
        }
        catch (UniCommandException ex)
        {
          string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          throw new UniSessionException(message);
        }
      }
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::CreateUniCommand()");
      return this.m_UniCommand;
    }

    internal object ClonePooled()
    {
      UniSession uniSession = (UniSession) this.MemberwiseClone();
      uniSession.m_UniCommand = (UniCommand) null;
      uniSession.m_UniStatus = 0;
      uniSession.m_UniTransaction = (UniTransaction) null;
      return (object) uniSession;
    }

    public void ReleaseTaskLock(int pLockNum)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::ReleaseTaskLock()");
        this.CheckEntryConditions();
        if (pLockNum >= 0)
        {
          if (pLockNum <= 64)
          {
            try
            {
              UniRPCPacket inPacket = this.m_Conn.InPacket;
              UniRPCPacket outPacket = this.m_Conn.OutPacket;
              outPacket.UniRPCPMessage.WriteInteger(0, 59);
              outPacket.UniRPCPMessage.WriteInteger(1, pLockNum);
              this.m_Conn.Call();
              this.m_UniReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
            }
            catch (UniRPCConnectionException ex)
            {
              string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace;
              UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
              this.m_IsRPCError = true;
              throw new UniSessionException(message);
            }
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::ReleaseTaskLock()");
            return;
          }
        }
        UniSessionException sessionException = new UniSessionException(UniErrorMessage.GetErrorMessage(30098));
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sessionException.Message + " Source: " + sessionException.Source + " Method: void ReleaseTaskLock( int pLockNum ) ");
        throw sessionException;
      }
    }

    public void CreateTaskLock(int aLockNum)
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::CreateTaskLock()");
        this.CheckEntryConditions();
        if (aLockNum >= 0)
        {
          if (aLockNum <= 64)
          {
            try
            {
              UniRPCPacket inPacket = this.m_Conn.InPacket;
              UniRPCPacket outPacket = this.m_Conn.OutPacket;
              outPacket.UniRPCPMessage.WriteInteger(0, 31);
              outPacket.UniRPCPMessage.WriteInteger(1, aLockNum);
              this.m_Conn.Call();
              this.m_UniReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
              if (this.m_UniReturnCode != 0)
              {
                UniSessionException sessionException = new UniSessionException(UniErrorMessage.GetErrorMessage(30002));
                UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sessionException.Message + " Source: " + sessionException.Source + " Method: CreateTaskLock( int aLockNum )");
                throw sessionException;
              }
            }
            catch (UniRPCConnectionException ex)
            {
              string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace;
              UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
              this.m_IsRPCError = true;
              throw new UniSessionException(message);
            }
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::CreateTaskLock()");
            return;
          }
        }
        UniSessionException sessionException1 = new UniSessionException(UniErrorMessage.GetErrorMessage(30098));
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sessionException1.Message + " Source: " + sessionException1.Source + " Method: CreateTaskLock( int aLockNum )");
        throw sessionException1;
      }
    }

    public UniDynArray GetAtVariable(int aTokenVal)
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::GetAtVariable()");
      if (aTokenVal < 1 || aTokenVal > 10)
      {
        UniSessionException sessionException = new UniSessionException("@Variable key must be between 1-10" + UniErrorMessage.GetErrorMessage(14022));
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sessionException.Message + " Source: " + sessionException.Source + " Method: String GetAtVariable( int aTokenVal )");
        throw sessionException;
      }
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::GetAtVariable()");
      return this.GetValue(aTokenVal);
    }

    public void SetAtVariable(int aTokenVal, UniDynArray aAtVariable)
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::SetAtVariable()");
      if (aTokenVal == 7)
        this.SetValue(aTokenVal, aAtVariable);
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::SetAtVariable()");
    }

    public string Encrypt(string aString)
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::Encrypt()");
      byte[] byteArray = this.UniCodeStringToByteArray(aString);
      for (int index = 0; index < byteArray.Length; ++index)
        byteArray[index] = this.ServerVersion != 1 ? (byte) ((uint) byteArray[index] ^ 4U) : (byte) ((uint) byteArray[index] ^ 3U);
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::Encrypt()");
      return this.ByteArrayToUniCodeString(byteArray);
    }

    public byte[] UniCodeStringToByteArray(string pStringVal)
    {
      try
      {
        Encoding unicode = Encoding.Unicode;
        byte[] bytes = unicode.GetBytes(pStringVal);
        return Encoding.Convert(unicode, this.UOEncoding, bytes);
      }
      catch (ArgumentNullException ex)
      {
        string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
        throw new UniSessionException(message);
      }
    }

    public string ByteArrayToUniCodeString(byte[] ByteArray) => Encoding.Unicode.GetString(Encoding.Convert(this.UOEncoding, Encoding.Unicode, ByteArray, 0, ByteArray.Length));

    public byte GetMarkCharacter(int aMarkChar)
    {
      if (aMarkChar < 0 || aMarkChar > 5)
      {
        UniSessionException sessionException = new UniSessionException("Mark Character  must be between IM-SQLNULL" + UniErrorMessage.GetErrorMessage(14022));
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sessionException.Message + " Source: " + sessionException.Source + " Method: byte GetMarkCharacter( int aMarkChar )");
        throw sessionException;
      }
      return this.m_UniMarkCharacters[aMarkChar];
    }

    internal char GetMarkCharacterAsChar(int aMarkChar) => this.ByteArrayToUniCodeString(new byte[1]
    {
      this.GetMarkCharacter(aMarkChar)
    }).ToCharArray(0, 1)[0];

    public string GetDelimitedString(string[] pRecordID, int pDelimeter)
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::GetDelimitedString()");
      StringCollection stringCollection = new StringCollection();
      stringCollection.AddRange(pRecordID);
      StringBuilder stringBuilder = new StringBuilder();
      byte[] bytes = new byte[1];
      byte markCharacter = this.GetMarkCharacter(pDelimeter);
      int num = (int) Convert.ToChar(markCharacter);
      bytes[0] = markCharacter;
      string str1 = Encoding.Unicode.GetString(Encoding.Convert(this.UOEncoding, Encoding.Unicode, bytes, 0, bytes.Length));
      foreach (string str2 in stringCollection)
      {
        stringBuilder.Append(str2);
        stringBuilder.Append(str1);
      }
      int length = stringBuilder.Length;
      stringBuilder.Remove(length - 1, 1);
      string delimitedString = stringBuilder.ToString();
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniSession GetDelimitedString is " + delimitedString);
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::GetDelimitedString()");
      return delimitedString;
    }

    public byte[] GetDelimitedByteArrayRecordID(string[] pRecordID, int pDelimeter)
    {
      byte markCharacter = this.GetMarkCharacter(pDelimeter);
      ArrayList arrayList = new ArrayList();
      StringCollection stringCollection = new StringCollection();
      stringCollection.AddRange(pRecordID);
      foreach (string pStringVal in stringCollection)
      {
        byte[] byteArray = this.UniCodeStringToByteArray(pStringVal);
        arrayList.AddRange((ICollection) byteArray);
        arrayList.Add((object) markCharacter);
      }
      byte[] byteArrayRecordId = new byte[arrayList.Count - 1];
      arrayList.CopyTo(0, (Array) byteArrayRecordId, 0, arrayList.Count - 1);
      return byteArrayRecordId;
    }

    public byte[] GetDelimitedByteArrayRecordID(int[] pFieldNumber, int pDelimeter)
    {
      byte markCharacter = this.GetMarkCharacter(pDelimeter);
      ArrayList arrayList1 = new ArrayList();
      ArrayList arrayList2 = new ArrayList();
      arrayList2.AddRange((ICollection) pFieldNumber);
      foreach (object obj in arrayList2)
      {
        byte[] byteArray = this.UniCodeStringToByteArray(Convert.ToString(obj));
        arrayList1.AddRange((ICollection) byteArray);
        arrayList1.Add((object) markCharacter);
      }
      byte[] byteArrayRecordId = new byte[arrayList1.Count - 1];
      arrayList1.CopyTo(0, (Array) byteArrayRecordId, 0, arrayList1.Count - 1);
      return byteArrayRecordId;
    }

    public string GetDelimitedString(int[] pFieldNumber, int pDelimeter)
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::GetDelimitedString()");
      ArrayList arrayList = new ArrayList();
      arrayList.AddRange((ICollection) pFieldNumber);
      StringBuilder stringBuilder = new StringBuilder();
      byte[] bytes = new byte[1];
      byte markCharacter = this.GetMarkCharacter(pDelimeter);
      bytes[0] = markCharacter;
      string str = Encoding.Unicode.GetString(Encoding.Convert(this.UOEncoding, Encoding.Unicode, bytes, 0, bytes.Length));
      foreach (object obj in arrayList)
      {
        stringBuilder.Append(obj);
        stringBuilder.Append(str);
      }
      int length = stringBuilder.Length;
      stringBuilder.Remove(length - 1, 1);
      string delimitedString = stringBuilder.ToString();
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniSession GetDelimitedString is " + delimitedString);
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::GetDelimitedString()");
      return delimitedString;
    }

    public string Iconv(string aString, string aConvCode)
    {
      try
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::Iconv()");
        this.m_UniStatus = 0;
        UniRPCPacket inPacket = this.m_Conn.InPacket;
        UniRPCPacket outPacket = this.m_Conn.OutPacket;
        outPacket.UniRPCPMessage.WriteInteger(0, 26);
        outPacket.UniRPCPMessage.WriteByteArray(1, this.UniCodeStringToByteArray(aString));
        outPacket.UniRPCPMessage.WriteByteArray(2, this.UniCodeStringToByteArray(aConvCode));
        this.m_Conn.Call();
        if (inPacket.UniRPCPMessage.ReadInteger(0) == 0)
        {
          this.m_UniStatus = inPacket.UniRPCPMessage.ReadInteger(1);
          if (this.m_UniStatus == 0)
          {
            string uniCodeString = this.ByteArrayToUniCodeString(inPacket.UniRPCPMessage.ReadByteArray(2));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniSession::Iconv() Converted String is " + uniCodeString);
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::Iconv()");
            return uniCodeString;
          }
        }
      }
      catch (UniRPCConnectionException ex)
      {
        string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
        this.m_IsRPCError = true;
        throw new UniSessionException(message);
      }
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::Iconv()");
      return aString;
    }

    public string Oconv(string aString, string aConvCode)
    {
      try
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::Oconv()");
        this.m_UniStatus = 0;
        UniRPCPacket inPacket = this.m_Conn.InPacket;
        UniRPCPacket outPacket = this.m_Conn.OutPacket;
        outPacket.UniRPCPMessage.WriteInteger(0, 33);
        outPacket.UniRPCPMessage.WriteByteArray(1, this.UniCodeStringToByteArray(aString));
        outPacket.UniRPCPMessage.WriteByteArray(2, this.UniCodeStringToByteArray(aConvCode));
        this.m_Conn.Call();
        this.m_UniReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
        if (this.m_UniReturnCode == 0)
        {
          this.m_UniStatus = inPacket.UniRPCPMessage.ReadInteger(1);
          if (this.m_UniStatus == 0)
          {
            byte[] ByteArray = inPacket.UniRPCPMessage.ReadByteArray(2);
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniSession::Oconv() Converted String is " + this.ByteArrayToUniCodeString(ByteArray));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::Oconv()");
            return this.ByteArrayToUniCodeString(ByteArray);
          }
        }
      }
      catch (UniRPCConnectionException ex)
      {
        string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
        this.m_IsRPCError = true;
        throw new UniSessionException(message);
      }
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::Oconv()");
      return aString;
    }

    ~UniSession()
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::~UniSession()");
      UniObjects.CloseSession(this);
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::~UniSession()");
    }

    internal UniSession()
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::UniSession()");
      this.InitDefaultValues();
      this.m_Conn = new UniRPCConnection();
      if (this.m_Conn != null)
      {
        this.m_HostPort = this.m_Conn.Port;
        this.m_TransportType = this.m_Conn.TransportType;
      }
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::UniSession()");
    }

    internal UniSession(UniSession aSession)
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::UniSession(UniSession aSession)");
      if (aSession == null)
      {
        UniSessionException sessionException = new UniSessionException(UniErrorMessage.GetErrorMessage(39120));
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sessionException.Message + " Source: " + sessionException.Source + " Method: " + (object) sessionException.TargetSite);
        throw sessionException;
      }
      this.m_Conn = new UniRPCConnection(aSession.m_Conn);
      this.InitDefaultValues();
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::UniSession(UniSession aSession)");
    }

    internal void Connect(string pHostName, string pUserID, string pPassword, string pAccount)
    {
      this.m_HostName = pHostName;
      this.m_UserID = pUserID;
      this.m_Password = pPassword;
      this.m_Account = pAccount;
      this.Connect();
    }

    internal void Connect(
      string pHostName,
      int pPortNumber,
      string pUserID,
      string pPassword,
      string pAccount)
    {
      this.m_HostPort = pPortNumber;
      this.Connect(pHostName, pUserID, pPassword, pAccount);
    }

    internal void Connect()
    {
      lock (this)
      {
        try
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::Connect()");
          this.m_Conn.Host = this.m_HostName;
          this.m_Conn.Port = this.m_HostPort;
          this.m_Conn.Service = this.m_Service;
          this.m_Conn.TimeoutSeconds = this.m_Timeout;
          this.m_Conn.TransportType = this.m_TransportType;
          this.m_Conn.Connect();
          UniRPCPacket inPacket = this.m_Conn.InPacket;
          UniRPCPacket outPacket = this.m_Conn.OutPacket;
          if (!new Regex("(?<user>[^@]+)").Match(this.m_UserID).Success)
          {
            UniSessionException sessionException = new UniSessionException(UniErrorMessage.GetErrorMessage(80011));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sessionException.Message + " Source: " + sessionException.Source + " Method: void Connect()");
            throw sessionException;
          }
          if (!new Regex("(?<Account>[^@]+)").Match(this.m_Account).Success)
          {
            UniSessionException sessionException = new UniSessionException(UniErrorMessage.GetErrorMessage(39125));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sessionException.Message + " Source: " + sessionException.Source + " Method: void Connect()");
            throw sessionException;
          }
          switch (this.ServerVersion)
          {
            case 1:
              outPacket.UniRPCPMessage.WriteInteger(0, 3);
              outPacket.UniRPCPMessage.WriteInteger(1, 4);
              outPacket.UniRPCPMessage.WriteByteArray(2, this.UniCodeStringToByteArray(this.m_UserID));
              outPacket.UniRPCPMessage.WriteByteArray(3, this.UniCodeStringToByteArray(this.Encrypt(this.m_Password)));
              outPacket.UniRPCPMessage.WriteByteArray(4, this.UniCodeStringToByteArray(this.m_Account));
              outPacket.UniRPCPMessage.WriteByteArray(5, this.UniCodeStringToByteArray(this.m_LicenseToken));
              outPacket.UniRPCPMessage.WriteInteger(6, 0);
              break;
            default:
              outPacket.UniRPCPMessage.WriteInteger(0, 4);
              outPacket.UniRPCPMessage.WriteInteger(1, 6);
              outPacket.UniRPCPMessage.WriteByteArray(2, this.UniCodeStringToByteArray(this.m_UserID));
              outPacket.UniRPCPMessage.WriteByteArray(3, this.UniCodeStringToByteArray(this.Encrypt(this.m_Password)));
              outPacket.UniRPCPMessage.WriteByteArray(4, this.UniCodeStringToByteArray(this.m_Account));
              outPacket.UniRPCPMessage.WriteByteArray(5, this.UniCodeStringToByteArray(this.m_LicenseToken));
              outPacket.UniRPCPMessage.WriteInteger(6, 0);
              outPacket.UniRPCPMessage.WriteInteger(7, this.IPAddress);
              outPacket.UniRPCPMessage.WriteByteArray(8, this.UniCodeStringToByteArray(this.MacAddress));
              outPacket.UniRPCPMessage.WriteByteArray(9, this.UniCodeStringToByteArray(this.DeviceName));
              outPacket.UniRPCPMessage.WriteByteArray(10, this.UniCodeStringToByteArray(this.DeviceSubKey));
              if (UniObjects.UOPooling)
              {
                if (this.IsCPSupported(this.m_Conn.ServerId))
                {
                  outPacket.UniRPCPMessage.WriteInteger(11, 1);
                  break;
                }
                UniSessionException sessionException = new UniSessionException(UniErrorMessage.GetErrorMessage(92));
                UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sessionException.Message + " Source: UniSession Class Method: Connect()");
                throw sessionException;
              }
              break;
          }
          this.m_Conn.Call();
          this.m_UniReturnCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (this.m_UniReturnCode == 0)
          {
            this.NLSInit();
            this.GetHostInfo();
            this.m_IsActive = true;
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "UniSession::Connect()Connection Passed");
            this.m_Conn.HostType = this.HostType;
          }
          else
          {
            UniSessionException sessionException = new UniSessionException(UniErrorMessage.GetErrorMessage(this.m_UniReturnCode));
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sessionException.Message + " Source: " + sessionException.Source + " Method: void Connect()");
            throw sessionException;
          }
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_IsRPCError = true;
          throw new UniSessionException(message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::Connect()");
      }
    }

    private bool IsCPSupported(string ServerId) => ServerId.StartsWith("udapi") || ServerId.StartsWith("uvapi");

    internal void Disconnect()
    {
      lock (this)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::Disconnect()");
        if (!this.m_IsActive)
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::Disconnect() Connection is not Active");
        }
        else
        {
          try
          {
            if (UniObjects.PerformanceCounter != null)
              UniObjects.PerformanceCounter.PooledConnections.Decrement();
            this.m_Conn.Close();
            this.InitDefaultValues();
          }
          catch (UniRPCConnectionException ex)
          {
            string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace;
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
            this.m_IsRPCError = true;
            throw new UniSessionException(message);
          }
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::Disconnect()");
        }
      }
    }

    internal void InitCommandObject()
    {
      this.m_UniCommand = (UniCommand) null;
      this.m_UniTransaction = (UniTransaction) null;
    }

    internal void InitDefaultValues()
    {
      this.m_IsNLSUnidataSession = false;
      this.m_UniMarkCharacters = UniTokens.defaultMarkArray;
      this.m_IsActive = false;
      this.m_IsRPCError = false;
      this.m_IsNLSEnabled = false;
      this.m_UniCommand = (UniCommand) null;
      this.m_UniTransaction = (UniTransaction) null;
      this.m_UniNLSLocale = (UniNLSLocale) null;
      this.m_UniNLSMap = (UniNLSMap) null;
      this.m_BlockingStrategy = 2;
      this.m_LockStrategy = 0;
      this.m_ReleaseStrategy = 12;
      this.m_UniReturnCode = 0;
      this.m_NumOpenFiles = 0;
      this.m_UniStatus = 0;
      this.m_Conn = (UniRPCConnection) null;
    }

    private void CheckEntryConditions()
    {
      if (!this.m_IsActive)
      {
        UniSessionException sessionException = new UniSessionException(UniErrorMessage.GetErrorMessage(39120));
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sessionException.Message + " Source: " + sessionException.Source + " Method: void CheckEntryConditions() ");
        throw sessionException;
      }
      if (this.m_UniCommand != null && (this.m_UniCommand.CommandStatus == 1 || this.m_UniCommand.CommandStatus == 2))
      {
        UniSessionException sessionException = new UniSessionException(UniErrorMessage.GetErrorMessage(30114));
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sessionException.Message + " Source: " + sessionException.Source + " Method: void CheckEntryConditions() ");
        throw sessionException;
      }
    }

    private void GetHostInfo()
    {
      lock (this)
      {
        try
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::GetHostInfo ");
          UniRPCPacket inPacket = this.m_Conn.InPacket;
          this.m_Conn.OutPacket.UniRPCPMessage.WriteInteger(0, 69);
          this.m_Conn.Call();
          if (inPacket.UniRPCPMessage.ReadInteger(0) == 0)
          {
            this.m_HostType = inPacket.UniRPCPMessage.ReadInteger(2);
            this.m_UserID = this.ByteArrayToUniCodeString(inPacket.UniRPCPMessage.ReadByteArray(1));
          }
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_IsRPCError = true;
          throw new UniSessionException(message);
        }
        catch (UniRPCPacketException ex)
        {
          string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          throw new UniSessionException(message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::GetHostInfo ");
      }
    }

    internal UniDynArray GetValue(int pKey)
    {
      lock (this)
      {
        try
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::GetValue() ");
          UniRPCPacket inPacket = this.m_Conn.InPacket;
          UniRPCPacket outPacket = this.m_Conn.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 25);
          outPacket.UniRPCPMessage.WriteInteger(1, pKey);
          this.m_Conn.Call();
          int aErrorCode = inPacket.UniRPCPMessage.ReadInteger(0);
          if (aErrorCode == 0)
          {
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::GetValue() ");
            return new UniDynArray(this, inPacket.UniRPCPMessage.ReadByteArray(1));
          }
          UniSessionException sessionException = new UniSessionException(UniErrorMessage.GetErrorMessage(aErrorCode));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sessionException.Message + " Source: " + sessionException.Source + " Method: string GetValue(int pKey)");
          throw sessionException;
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_IsRPCError = true;
          throw new UniSessionException(message);
        }
      }
    }

    private void SetValue(int pKey, UniDynArray pNewVal)
    {
      lock (this)
      {
        try
        {
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::SetValue ");
          UniRPCPacket inPacket = this.m_Conn.InPacket;
          UniRPCPacket outPacket = this.m_Conn.OutPacket;
          outPacket.UniRPCPMessage.WriteInteger(0, 50);
          outPacket.UniRPCPMessage.WriteInteger(1, pKey);
          outPacket.UniRPCPMessage.WriteByteArray(2, pNewVal.ToByteArray());
          this.m_Conn.Call();
          inPacket.UniRPCPMessage.ReadInteger(0);
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_IsRPCError = true;
          throw new UniSessionException(message);
        }
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::SetValue ");
      }
    }

    private void NLSInit()
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::NLSInit ");
      if (this.ServerVersion == 1)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniSession::NLSInit Old Database Server");
      }
      else
      {
        try
        {
          this.m_Conn.OutPacket.UniRPCPMessage.WriteInteger(0, 75);
          this.m_Conn.Call();
          this.m_UniReturnCode = this.m_Conn.InPacket.UniRPCPMessage.ReadInteger(0);
          int num1 = this.m_Conn.InPacket.UniRPCPMessage.ReadInteger(1);
          int num2 = this.m_Conn.InPacket.UniRPCPMessage.ReadInteger(2);
          if (num1 == 1)
          {
            this.m_IsNLSEnabled = true;
            if (num2 == 1)
              this.m_IsNLSLocalesEnabled = true;
          }
          if (num1 == 2)
            this.m_IsNLSUnidataSession = true;
          if (!this.m_IsNLSEnabled)
          {
            if (!this.m_IsNLSUnidataSession)
              goto label_17;
          }
          string encodingName = UniSession.m_Encoding.EncodingName;
          this.m_UniNLSMap = new UniNLSMap(this);
          try
          {
            this.m_UniNLSMap.SetName(encodingName);
            this.m_UniMarkCharacters = this.m_UniNLSMap.GetUniMarks();
          }
          catch (UniNLSMapException ex)
          {
            this.m_UniNLSMap.SetName("DEFAULT");
            this.m_UniMarkCharacters = this.m_UniNLSMap.GetUniMarks();
          }
          if (this.m_IsNLSLocalesEnabled)
          {
            this.m_UniNLSLocale = new UniNLSLocale(this);
            string name = CultureInfo.CurrentCulture.Name;
            try
            {
              this.m_UniNLSLocale.SetLocaleName(name);
            }
            catch (UniNLSLocaleException ex)
            {
              this.m_UniNLSLocale.SetLocaleName("DEFAULT");
            }
          }
        }
        catch (UniRPCConnectionException ex)
        {
          string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite + ex.StackTrace;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_IsRPCError = true;
          throw new UniSessionException(message);
        }
label_17:
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniSession::NLSInit ");
      }
    }

    private UniSession CreateDeepCopy() => this;

    internal void SetServiceType(string pServiceType) => this.m_Service = pServiceType;

    public bool IsActive => this.m_IsActive;

    internal bool IsRPCError
    {
      get => this.m_IsRPCError;
      set => this.m_IsRPCError = value;
    }

    public string UserName => this.m_UserID;

    public string Password => this.m_Password;

    public string Account => this.m_Account;

    public string HostName => this.m_HostName;

    public int CompressionThreshold
    {
      get => this.m_CompressionThreshold;
      set
      {
        if (!this.CompressionEnabled)
        {
          UniSessionException sessionException = new UniSessionException(UniErrorMessage.GetErrorMessage(81026));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sessionException.Message + " Source: " + sessionException.Source + " Property: int CompressionThreshold");
          throw sessionException;
        }
        if (value < 0)
        {
          UniSessionException sessionException = new UniSessionException("Compression threshold must be a positive value" + UniErrorMessage.GetErrorMessage(14022));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sessionException.Message + " Source: " + sessionException.Source + " Property: int CompressionThreshold");
          throw sessionException;
        }
        this.m_CompressionThreshold = value;
      }
    }

    public int EncryptionType
    {
      get => this.m_EncryptionType;
      set
      {
        if (!this.EncryptionEnabled)
        {
          UniSessionException sessionException = new UniSessionException(UniErrorMessage.GetErrorMessage(81025));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sessionException.Message + " Source: " + sessionException.Source + " Property: int EncryptionType");
          throw sessionException;
        }
        if (value == 0 || value == 1)
        {
          this.m_EncryptionType = value;
        }
        else
        {
          UniSessionException sessionException = new UniSessionException("Invalid Encryption Type" + UniErrorMessage.GetErrorMessage(81025));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sessionException.Message + " Source: " + sessionException.Source + " Property: int EncryptionType");
          throw sessionException;
        }
      }
    }

    public string DeviceSubKey => this.m_UniSubKey;

    public int IPAddress
    {
      get
      {
        try
        {
          string hostName = Dns.GetHostName();
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Get Property UniSession::IPAddress HostName is " + hostName);
          int hashCode = Dns.GetHostByName(hostName).AddressList[0].GetHashCode();
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Get Property UniSession::IPAddress IPAddress is " + (object) hashCode);
          return hashCode;
        }
        catch (SocketException ex)
        {
          UniSessionException sessionException = new UniSessionException("SocketException caught!!!" + ex.Source + ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sessionException.Message + " Source: " + sessionException.Source + " Property: int IPAddress");
          throw sessionException;
        }
        catch (ArgumentNullException ex)
        {
          UniSessionException sessionException = new UniSessionException("ArgumentNullException caught!!!" + ex.Source + ex.Message + " Source: " + ex.Source + " Property: int IPAddress");
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sessionException.Message + " Source: " + sessionException.Source + " Method: " + (object) sessionException.TargetSite);
          throw sessionException;
        }
        catch (Exception ex)
        {
          UniSessionException sessionException = new UniSessionException("Exception caught!!!" + ex.Source + ex.Message + " Source: " + ex.Source + " Property: int IPAddress");
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sessionException.Message + " Source: " + sessionException.Source + " Method: " + (object) sessionException.TargetSite);
          throw sessionException;
        }
      }
    }

    public string MacAddress => "JAVA";

    public string DeviceName
    {
      get
      {
        try
        {
          return Dns.GetHostName();
        }
        catch (Exception ex)
        {
          string message = ex.Message;
          return "unknown host";
        }
      }
    }

    public int HostPort => this.m_HostPort;

    public string Service => this.m_Service;

    public int HostType => this.m_HostType;

    public int ServerVersion => this.m_Conn != null ? this.m_Conn.Version : -1;

    public int Timeout
    {
      get => this.m_Timeout;
      set
      {
        if (value < 0)
        {
          UniSessionException sessionException = new UniSessionException("Timeout value must be a positive value" + UniErrorMessage.GetErrorMessage(14022));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sessionException.Message + " Source: " + sessionException.Source + " Property: int Timeout");
          throw sessionException;
        }
        this.m_Timeout = value;
        if (this.m_Conn == null)
          return;
        try
        {
          this.m_Conn.TimeoutSeconds = value;
        }
        catch (UniRPCConnectionException ex)
        {
          string message = "Get Property UniSession::Timeout " + ex.Message + " Source: " + ex.Source + " Property: int Timeout";
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          this.m_IsRPCError = true;
          throw new UniSessionException(message);
        }
      }
    }

    public int TransportType
    {
      get => this.m_TransportType;
      set => this.m_TransportType = value;
    }

    public int Status => this.m_UniStatus;

    public bool CompressionEnabled => this.m_Conn != null && this.m_Conn.IsCompressionSupported();

    public bool EncryptionEnabled => this.m_Conn != null && this.m_Conn.IsEncryptionSupported();

    public bool NLSEnabled => this.m_IsNLSEnabled;

    public bool NLSLocalesEnabled => this.m_IsNLSLocalesEnabled;

    public int BlockingStrategy
    {
      get => this.m_BlockingStrategy;
      set
      {
        if (value < 1 || value > 2)
        {
          UniSessionException sessionException = new UniSessionException("UniSession.BlockingStrategy failed:  Strategy eitherWAIT_ON_LOCKED (1) or RETURN_ON_LOCKED(2)" + UniErrorMessage.GetErrorMessage(14022));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sessionException.Message + " Source: " + sessionException.Source + " Property: int BlockingStrategy");
          throw sessionException;
        }
        this.m_BlockingStrategy = value;
      }
    }

    public int LockStrategy
    {
      get => this.m_LockStrategy;
      set
      {
        if (value < 0 || value > 2)
        {
          UniSessionException sessionException = new UniSessionException("UniSession.LockStrategy failed:  Strategy eitherNO_LOCKS (0) or EXCLUSIVE_UPDATE(1) or SHARED_READ(2)" + UniErrorMessage.GetErrorMessage(14022));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sessionException.Message + " Source: " + sessionException.Source + " Property: int LockStrategy");
          throw sessionException;
        }
        this.m_LockStrategy = value;
      }
    }

    public int ReleaseStrategy
    {
      get => this.m_ReleaseStrategy;
      set
      {
        if (value < 0 || value > 8)
        {
          UniSessionException sessionException = new UniSessionException("UniSession.LockStrategy failed:  Strategy eitherWRITE_RELEASE (1), READ_RELEASE (2), EXPLICIT_RELEASE (4) or CHANGE_RELEASE (8)" + UniErrorMessage.GetErrorMessage(14022));
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, sessionException.Message + " Source: " + sessionException.Source + " Property: int ReleaseStrategy");
          throw sessionException;
        }
        this.m_ReleaseStrategy = value;
      }
    }

    public int MaxOpenFiles => this.m_MaxOpenFiles;

    public int CurrentOpenFiles => this.m_NumOpenFiles;

    public Encoding UOEncoding
    {
      set => UniSession.m_Encoding = value;
      get => UniSession.m_Encoding;
    }

    internal UniRPCConnection UniRPCConnection => this.m_Conn;

    internal UniCommand UniCommand => this.m_UniCommand;

    internal UniTransaction UniTransaction => this.m_UniTransaction;

    internal DateTime CreationTime
    {
      get => this.m_CreationTime;
      set => this.m_CreationTime = value;
    }

    internal DateTime FreedTime
    {
      get => this.m_FreedTime;
      set => this.m_FreedTime = value;
    }

    internal bool IsPooledSession
    {
      get => this.m_IsPooledSession;
      set => this.m_IsPooledSession = value;
    }
  }
}
