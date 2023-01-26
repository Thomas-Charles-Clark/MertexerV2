// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniDataSet
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

using System;
using System.Collections;
using System.Text;

namespace IBMU2.UODOTNET
{
  public class UniDataSet : UniRoot, IEnumerable
  {
    private Hashtable m_HashList;
    private ArrayList m_IndexList;
    private UniSession m_Session;

    public IEnumerator GetEnumerator() => (IEnumerator) new UniDataSet.UniDataSetEnumerator(this);

    public UniDataSet(UniSession pSession)
    {
      this.m_Session = pSession;
      this.m_HashList = new Hashtable();
      this.m_IndexList = new ArrayList();
    }

    public UniDataSet(
      UniSession pSession,
      string[] RecId,
      byte[] RetData,
      byte[] StatusData,
      byte[] RetValData)
    {
      try
      {
        int length = RecId.Length;
        this.m_Session = pSession;
        int nStartIndex1 = 0;
        int nIndex1 = 0;
        int nStartIndex2 = 0;
        int nIndex2 = 0;
        int nStartIndex3 = 0;
        int nIndex3 = 0;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniDataSet::UniDataSet(UniSession pSession,string[] RecId, byte [] RetData, byte [] StatusData, byte [] RetValData)");
        this.m_HashList = new Hashtable();
        this.m_IndexList = new ArrayList();
        for (int index = 0; index < length; ++index)
        {
          UniRecord uniRecord = new UniRecord();
          uniRecord.RecordID = RecId[index];
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, " Record Id: " + uniRecord.RecordID.ToString());
          byte[] record1 = this.ExtractRecord(RetData, nStartIndex1, ref nIndex1);
          nStartIndex1 = nIndex1 + 1;
          UniDynArray uniDynArray = new UniDynArray(pSession, record1);
          uniRecord.Record = uniDynArray;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, " Record : " + uniDynArray.ToString());
          byte[] record2 = this.ExtractRecord(StatusData, nStartIndex2, ref nIndex2);
          nStartIndex2 = nIndex2 + 1;
          byte[] bytes1 = Encoding.Convert(this.m_Session.UOEncoding, Encoding.Unicode, record2);
          char[] chars1 = new char[Encoding.Unicode.GetCharCount(bytes1, 0, bytes1.Length)];
          Encoding.Unicode.GetChars(bytes1, 0, bytes1.Length, chars1, 0);
          string str1 = new string(chars1);
          uniRecord.RecordStatus = Convert.ToInt32(str1);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, " Staus : " + (object) uniRecord.RecordStatus);
          byte[] record3 = this.ExtractRecord(RetValData, nStartIndex3, ref nIndex3);
          nStartIndex3 = nIndex3 + 1;
          byte[] bytes2 = Encoding.Convert(this.m_Session.UOEncoding, Encoding.Unicode, record3);
          char[] chars2 = new char[Encoding.Unicode.GetCharCount(bytes2, 0, bytes2.Length)];
          Encoding.Unicode.GetChars(bytes2, 0, bytes2.Length, chars2, 0);
          string str2 = new string(chars2);
          uniRecord.RecordReturnValue = Convert.ToInt32(str2);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, " Ret Val : " + (object) uniRecord.RecordReturnValue);
          this.m_HashList.Add((object) RecId[index], (object) uniRecord);
          this.m_IndexList.Add((object) RecId[index]);
        }
      }
      catch (Exception ex)
      {
        string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
        throw new UniDataSetException(message);
      }
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniDataSet::UniDataSet(UniSession pSession,string[] RecId, byte [] RetData, byte [] StatusData, byte [] RetValData)");
    }

    public void Add(string pUniRecID) => this.Add(pUniRecID, new UniDynArray(this.m_Session, ""));

    public void Add(string pUniRecID, UniDynArray pUniRecord) => this[pUniRecID] = new UniRecord()
    {
      RecordID = pUniRecID,
      Record = pUniRecord,
      RecordReturnValue = 0,
      RecordStatus = 0
    };

    public void Add(string pUniRecID, UniRecord pUniRec)
    {
      try
      {
        UniRecord uniRecord = new UniRecord();
        uniRecord.RecordID = pUniRec.RecordID;
        uniRecord.Record = pUniRec.Record;
        uniRecord.RecordReturnValue = pUniRec.RecordReturnValue;
        uniRecord.RecordStatus = pUniRec.RecordStatus;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniDynArray Set Indexer and Set Value is " + uniRecord.ToString());
        this.m_HashList.Add((object) pUniRecID, (object) uniRecord);
        this.m_IndexList.Add((object) pUniRecID);
      }
      catch (Exception ex)
      {
        string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
        throw new UniDataSetException(message);
      }
    }

    public void Add(string pUniRecID, string pRecord)
    {
      UniDynArray pUniRecord = new UniDynArray(this.m_Session, pRecord);
      this.Add(pUniRecID, pUniRecord);
    }

    public void Insert(int pIndexLoc, string pUniRecID, UniDynArray pUniRecord)
    {
      if (pIndexLoc < 0)
        pIndexLoc = 0;
      if (pIndexLoc > this.RowCount)
      {
        this.Add(pUniRecID, pUniRecord);
      }
      else
      {
        try
        {
          UniRecord uniRecord = new UniRecord();
          uniRecord.RecordID = pUniRecID;
          uniRecord.Record = pUniRecord;
          uniRecord.RecordReturnValue = 0;
          uniRecord.RecordStatus = 0;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniDynArray Set Indexer and Set Value is " + pUniRecord.ToString());
          this.m_HashList.Add((object) pUniRecID, (object) uniRecord);
          this.m_IndexList.Insert(pIndexLoc, (object) pUniRecID);
        }
        catch (Exception ex)
        {
          string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          throw new UniDataSetException(message);
        }
      }
    }

    public void Insert(int pIndexLoc, string pRecID, string pRecord)
    {
      UniDynArray pUniRecord = new UniDynArray(this.m_Session, pRecord);
      if (pIndexLoc < 0)
        pIndexLoc = 0;
      if (pIndexLoc > this.RowCount)
      {
        this.Add(pRecID, pUniRecord);
      }
      else
      {
        try
        {
          UniRecord uniRecord = new UniRecord();
          uniRecord.RecordID = pRecID;
          uniRecord.Record = pUniRecord;
          uniRecord.RecordReturnValue = 0;
          uniRecord.RecordStatus = 0;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniDynArray Set Indexer and Set Value is " + pUniRecord.ToString());
          this.m_HashList.Add((object) pRecID, (object) uniRecord);
          this.m_IndexList.Insert(pIndexLoc, (object) pRecID);
        }
        catch (Exception ex)
        {
          string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          throw new UniDataSetException(message);
        }
      }
    }

    public void Insert(int pIndexLoc, string pRecID, UniRecord pRecord)
    {
      if (pIndexLoc < 0)
        pIndexLoc = 0;
      if (pIndexLoc > this.RowCount)
      {
        this.Add(pRecID, pRecord);
      }
      else
      {
        try
        {
          this.m_HashList.Add((object) pRecID, (object) new UniRecord()
          {
            RecordID = pRecord.RecordID,
            Record = pRecord.Record,
            RecordReturnValue = pRecord.RecordReturnValue,
            RecordStatus = pRecord.RecordStatus
          });
          this.m_IndexList.Insert(pIndexLoc, (object) pRecID);
        }
        catch (Exception ex)
        {
          string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          throw new UniDataSetException(message);
        }
      }
    }

    public void Insert(string pUniRecID, UniDynArray pUniRecord)
    {
      if (this.m_IndexList.Count == 0)
      {
        this.Add(pUniRecID, pUniRecord);
      }
      else
      {
        UniDataSet.UniDataSetEnumerator enumerator = (UniDataSet.UniDataSetEnumerator) this.GetEnumerator();
        enumerator.MoveNext();
        this.Insert(enumerator.Current, pUniRecID, pUniRecord);
      }
    }

    public void Insert(string pUniRecID) => this.Insert(pUniRecID, new UniDynArray(this.m_Session, string.Empty));

    public void Insert(string pUniRecID, string pRecord)
    {
      if (this.m_IndexList.Count == 0)
      {
        this.Add(pUniRecID, pRecord);
      }
      else
      {
        UniDynArray pUniRecord = new UniDynArray(this.m_Session, pRecord);
        UniDataSet.UniDataSetEnumerator enumerator = (UniDataSet.UniDataSetEnumerator) this.GetEnumerator();
        enumerator.MoveNext();
        this.Insert(enumerator.Current, pUniRecID, pUniRecord);
      }
    }

    public void Insert(string pUniRecID, UniRecord pRecord)
    {
      if (this.m_IndexList.Count == 0)
      {
        this.Add(pUniRecID, pRecord);
      }
      else
      {
        UniDataSet.UniDataSetEnumerator enumerator = (UniDataSet.UniDataSetEnumerator) this.GetEnumerator();
        enumerator.MoveNext();
        this.Insert(enumerator.Current, pUniRecID, pRecord);
      }
    }

    public UniRecord GetRecord(string pUniRecID) => this[pUniRecID];

    public UniRecord GetRecord(int nIndex)
    {
      string index = (string) this.m_IndexList[nIndex];
      if (index != null)
        return this[index];
      UniDataSetException dataSetException = new UniDataSetException(UniErrorMessage.GetErrorMessage(30001) + " For Index " + (object) nIndex);
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, dataSetException.Message + " Source: " + dataSetException.Source + " Method: UniDynArray this[string  RecID]");
      throw dataSetException;
    }

    public int GetRecordStatus(string pUniRecID)
    {
      if (this.m_HashList.Contains((object) pUniRecID))
        return ((UniRecord) this.m_HashList[(object) pUniRecID]).RecordReturnValue;
      UniDataSetException dataSetException = new UniDataSetException(UniErrorMessage.GetErrorMessage(30001));
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, dataSetException.Message + " Source: " + dataSetException.Source + " Method: UniDynArray Remove(string pUniRecID)");
      throw dataSetException;
    }

    public int GetRecordStatus(int nIndex)
    {
      string index = (string) this.m_IndexList[nIndex];
      if (this.m_HashList.Contains((object) index))
        return ((UniRecord) this.m_HashList[(object) index]).RecordReturnValue;
      UniDataSetException dataSetException = new UniDataSetException(UniErrorMessage.GetErrorMessage(30001));
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, dataSetException.Message + " Source: " + dataSetException.Source + " Method: UniDynArray Remove(string pUniRecID)");
      throw dataSetException;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      IDictionaryEnumerator enumerator = this.m_HashList.GetEnumerator();
      while (enumerator.MoveNext())
      {
        UniRecord uniRecord = (UniRecord) enumerator.Value;
        stringBuilder.Append(uniRecord.ToString());
      }
      return stringBuilder.ToString();
    }

    public bool Relative(int numRows)
    {
      this.CurrentRow += numRows;
      if (this.CurrentRow < 0)
      {
        int num = this.First ? 1 : 0;
        return false;
      }
      if (this.CurrentRow < this.RowCount)
        return true;
      int num1 = this.AfterLast ? 1 : 0;
      return false;
    }

    public bool Absolute(int rowNum)
    {
      this.CurrentRow = rowNum >= 0 ? rowNum : this.RowCount + rowNum;
      if (this.CurrentRow < 0)
      {
        int num = this.First ? 1 : 0;
        return false;
      }
      if (this.CurrentRow < this.RowCount)
        return true;
      int num1 = this.AfterLast ? 1 : 0;
      return false;
    }

    public void Remove(string pUniRecID)
    {
      if (this.m_HashList.Contains((object) pUniRecID))
      {
        this.m_HashList.Remove((object) pUniRecID);
        this.m_IndexList.Remove((object) pUniRecID);
      }
      else
      {
        UniDataSetException dataSetException = new UniDataSetException(UniErrorMessage.GetErrorMessage(30001) + " For Record ID " + pUniRecID);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, dataSetException.Message + " Source: " + dataSetException.Source + " Method: UniDynArray Remove(string pUniRecID)");
        throw dataSetException;
      }
    }

    public void Clear()
    {
      this.m_HashList.Clear();
      this.m_IndexList.Clear();
    }

    public int RowCount => this.m_HashList.Count;

    public int CurrentRow
    {
      get
      {
        UniDataSet.UniDataSetEnumerator enumerator = (UniDataSet.UniDataSetEnumerator) this.GetEnumerator();
        enumerator.MoveNext();
        return enumerator.Current;
      }
      set
      {
        UniDataSet.UniDataSetEnumerator enumerator = (UniDataSet.UniDataSetEnumerator) this.GetEnumerator();
        enumerator.MoveNext();
        enumerator.Current = value;
      }
    }

    public UniRecord this[string RecID]
    {
      get
      {
        if (this.m_HashList.Contains((object) RecID))
        {
          UniRecord hash = (UniRecord) this.m_HashList[(object) RecID];
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniDynArray Get Indexer and Return Value is " + (object) hash.Record);
          return hash;
        }
        UniDataSetException dataSetException = new UniDataSetException(UniErrorMessage.GetErrorMessage(30001) + " For Record ID " + RecID);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, dataSetException.Message + " Source: " + dataSetException.Source + " Method: UniDynArray this[string  RecID]");
        throw dataSetException;
      }
      set
      {
        try
        {
          UniRecord uniRecord = new UniRecord();
          uniRecord.RecordID = value.RecordID;
          uniRecord.Record = value.Record;
          uniRecord.RecordReturnValue = value.RecordReturnValue;
          uniRecord.RecordStatus = value.RecordStatus;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniDynArray Set Indexer and Set Value is " + value.ToString());
          this.m_HashList.Add((object) RecID, (object) uniRecord);
          this.m_IndexList.Add((object) RecID);
        }
        catch (Exception ex)
        {
          string message = ex.Message + " Source: " + ex.Source + " Method: " + (object) ex.TargetSite;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
          throw new UniDataSetException(message);
        }
      }
    }

    public UniRecord this[int nIndex]
    {
      get
      {
        string index = (string) this.m_IndexList[nIndex];
        if (index != null)
          return this[index];
        UniDataSetException dataSetException = new UniDataSetException(UniErrorMessage.GetErrorMessage(30001) + " For Index " + (object) nIndex);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, dataSetException.Message + " Source: " + dataSetException.Source + " Method: UniDynArray this[string  RecID]");
        throw dataSetException;
      }
    }

    public string DelimitedRecordID
    {
      get
      {
        StringBuilder stringBuilder = new StringBuilder();
        byte[] bytes = new byte[1];
        byte markCharacter = this.m_Session.GetMarkCharacter(0);
        bytes[0] = markCharacter;
        string str = Encoding.Unicode.GetString(Encoding.Convert(this.m_Session.UOEncoding, Encoding.Unicode, bytes, 0, bytes.Length));
        IDictionaryEnumerator enumerator = this.m_HashList.GetEnumerator();
        while (enumerator.MoveNext())
        {
          stringBuilder.Append(enumerator.Key);
          stringBuilder.Append(str);
        }
        int length = stringBuilder.Length;
        stringBuilder.Remove(length - 1, 1);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniDynArray Get DelimitedRecordID is " + stringBuilder.ToString());
        return stringBuilder.ToString();
      }
    }

    public byte[] GetDelimitedByteArrayRecordID()
    {
      byte markCharacter = this.m_Session.GetMarkCharacter(0);
      ArrayList arrayList = new ArrayList();
      IDictionaryEnumerator enumerator = this.m_HashList.GetEnumerator();
      while (enumerator.MoveNext())
      {
        byte[] byteArray = this.m_Session.UniCodeStringToByteArray(((UniRecord) enumerator.Value).RecordID);
        arrayList.AddRange((ICollection) byteArray);
        arrayList.Add((object) markCharacter);
      }
      byte[] byteArrayRecordId = new byte[arrayList.Count - 1];
      arrayList.CopyTo(0, (Array) byteArrayRecordId, 0, arrayList.Count - 1);
      return byteArrayRecordId;
    }

    public string DelimitedRecord
    {
      get
      {
        StringBuilder stringBuilder = new StringBuilder();
        byte[] bytes = new byte[1];
        byte markCharacter = this.m_Session.GetMarkCharacter(0);
        bytes[0] = markCharacter;
        string str1 = Encoding.Unicode.GetString(Encoding.Convert(this.m_Session.UOEncoding, Encoding.Unicode, bytes, 0, bytes.Length));
        IDictionaryEnumerator enumerator = this.m_HashList.GetEnumerator();
        while (enumerator.MoveNext())
        {
          string str2 = ((UniRecord) enumerator.Value).Record.ToString();
          stringBuilder.Append(str2);
          stringBuilder.Append(str1);
        }
        int length = stringBuilder.Length;
        stringBuilder.Remove(length - 1, 1);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniDynArray Get DelimitedRecord is " + stringBuilder.ToString());
        return stringBuilder.ToString();
      }
    }

    public byte[] GetDelimitedByteArrayRecord()
    {
      byte markCharacter = this.m_Session.GetMarkCharacter(0);
      ArrayList arrayList = new ArrayList();
      IDictionaryEnumerator enumerator = this.m_HashList.GetEnumerator();
      while (enumerator.MoveNext())
      {
        UniRecord uniRecord = (UniRecord) enumerator.Value;
        arrayList.AddRange((ICollection) uniRecord.Record.ToByteArray());
        arrayList.Add((object) markCharacter);
      }
      byte[] delimitedByteArrayRecord = new byte[arrayList.Count - 1];
      arrayList.CopyTo(0, (Array) delimitedByteArrayRecord, 0, arrayList.Count - 1);
      return delimitedByteArrayRecord;
    }

    public bool Last => this.CurrentRow == this.RowCount - 1;

    public bool First => this.CurrentRow == 0;

    public bool AfterLast => this.CurrentRow >= this.RowCount;

    public bool BeforeFirst => this.CurrentRow < 0;

    private byte[] ExtractRecord(byte[] pSourceByteArray, int nStartIndex, ref int nIndex)
    {
      int num = Array.IndexOf((Array) pSourceByteArray, (object) this.m_Session.GetMarkCharacter(0), nStartIndex);
      byte[] destinationArray;
      if (num != -1)
      {
        int length = num - nStartIndex;
        destinationArray = new byte[length];
        Array.Copy((Array) pSourceByteArray, nStartIndex, (Array) destinationArray, 0, length);
      }
      else
      {
        int length = pSourceByteArray.Length - nStartIndex;
        destinationArray = new byte[length];
        Array.Copy((Array) pSourceByteArray, nStartIndex, (Array) destinationArray, 0, length);
      }
      nIndex = num;
      return destinationArray;
    }

    private class UniDataSetEnumerator : IEnumerator
    {
      private int position = -1;
      private UniDataSet t;

      public UniDataSetEnumerator(UniDataSet t) => this.t = t;

      public bool MoveNext()
      {
        if (this.position >= this.t.m_HashList.Count - 1)
          return false;
        ++this.position;
        return true;
      }

      public void Reset() => this.position = -1;

      object IEnumerator.Current => (object) (UniRecord) this.t.m_HashList[(object) (string) this.t.m_IndexList[this.position]];

      public int Current
      {
        get => this.position;
        set => this.position = value;
      }
    }
  }
}
