// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniDynArray
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

using System;
using System.Collections;

namespace IBMU2.UODOTNET
{
  public class UniDynArray : UniRoot
  {
    private byte[] m_Value = (byte[]) null;
    private ArrayList m_DynArray = (ArrayList) null;
    private bool m_IsArrayActive = false;
    private bool m_IsByteArrayActive = false;
    private UniSession m_Session = (UniSession) null;
    private int m_FileEncryptionType = 0;

    public UniDynArray(UniSession aSession) => this.InitDefaultValues(aSession, string.Empty);

    public int Count(int aField, int aValue, int aSubValue) => this.CountMarks(aField, aValue, aSubValue, 0);

    public UniDynArray(UniSession aSession, byte[] pData)
    {
      if (aSession == null || aSession.UniRPCConnection == null)
      {
        UniDynArrayException dynArrayException = new UniDynArrayException(UniErrorMessage.GetErrorMessage(39120));
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, dynArrayException.Message + " Source: " + dynArrayException.Source + " Method: UniCommand(UniSession aSession)");
        throw dynArrayException;
      }
      this.m_Session = aSession;
      this.m_FileEncryptionType = aSession.EncryptionType;
      int length = pData.Length;
      int destinationIndex = 0;
      byte[] destinationArray;
      if (this.m_Value == null)
      {
        destinationArray = new byte[length + destinationIndex];
      }
      else
      {
        destinationIndex = this.m_Value.Length;
        destinationArray = new byte[length + destinationIndex];
      }
      Array.Copy((Array) pData, 0, (Array) destinationArray, destinationIndex, length);
      this.m_Value = destinationArray;
      this.m_IsArrayActive = false;
      this.m_IsByteArrayActive = true;
    }

    public UniDynArray(UniSession aSession, string pString) => this.InitDefaultValues(aSession, pString);

    public int Count(int aField, int aValue) => this.Count(aField, aValue, 0);

    public int Count(int aField) => this.Count(aField, 0, 0);

    public int Count() => this.Count(0, 0, 0);

    public int Dcount(int aField, int aValue, int aSubValue) => this.ToByteArray().Length == 0 ? 0 : this.CountMarks(aField, aValue, aSubValue, -1) + 1;

    public int Dcount(int aField, int aValue) => this.Dcount(aField, aValue, 0);

    public int Dcount(int aField) => this.Dcount(aField, 0, 0);

    public int Dcount() => this.Dcount(0, 0, 0);

    public void Delete(int aField, int aValue, int aSubValue)
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "Enter UniDynArray::Delete( int aField, int aValue, int aSubValue )");
      this.CheckArrayState(false);
      if (aField <= 0 || aField > this.m_DynArray.Count)
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniDynArray::Delete( int aField, int aValue, int aSubValue ) Return from ( aField <= 0 ) || ( aField > m_DynArray.Count )");
      else if (aValue == 0)
      {
        this.m_DynArray.RemoveAt(aField - 1);
        this.m_IsByteArrayActive = false;
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniDynArray::Delete( int aField, int aValue, int aSubValue ) Return from aValue == 0 ");
      }
      else
      {
        ArrayList dyn = (ArrayList) this.m_DynArray[aField - 1];
        if (aValue < 0 || aValue > dyn.Count)
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniDynArray::Delete( int aField, int aValue, int aSubValue ) Return from ( aValue < 0 ) || ( aValue > fieldVector.Count)");
        else if (aSubValue == 0)
        {
          dyn.RemoveAt(aValue - 1);
          this.m_IsByteArrayActive = false;
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniDynArray::Delete( int aField, int aValue, int aSubValue ) Return from aSubValue == 0");
        }
        else
        {
          ArrayList arrayList = (ArrayList) dyn[aValue - 1];
          if (aSubValue < 0 || aSubValue > arrayList.Count)
          {
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniDynArray::Delete( int aField, int aValue, int aSubValue ) Return from ( aSubValue < 0 ) || ( aSubValue > valueVector.Count )");
          }
          else
          {
            this.m_IsByteArrayActive = false;
            arrayList.RemoveAt(aSubValue - 1);
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "Exit UniDynArray::Delete( int aField, int aValue, int aSubValue )");
          }
        }
      }
    }

    public void Delete(int aField, int aValue) => this.Delete(aField, aValue, 0);

    public void Delete(int aField) => this.Delete(aField, 0, 0);

    public UniDynArray Extract(int aField, int aValue, int aSubValue)
    {
      ArrayList arrayList1 = new ArrayList();
      if (aField == -1)
        return this;
      this.CheckArrayState(false);
      if (aField > this.m_DynArray.Count || aField < -1)
        return new UniDynArray(this.m_Session, string.Empty);
      if (aField == 0)
      {
        if (aValue == 0 && aSubValue == 0)
          return new UniDynArray(this.m_Session, string.Empty);
        aField = 1;
      }
      ArrayList dyn = (ArrayList) this.m_DynArray[aField - 1];
      if (aValue == 0 && aSubValue == 0)
      {
        int count1 = dyn.Count;
        for (int index1 = 0; index1 < count1; ++index1)
        {
          ArrayList arrayList2 = (ArrayList) dyn[index1];
          int count2 = arrayList2.Count;
          for (int index2 = 0; index2 < count2; ++index2)
          {
            byte[] c = (byte[]) arrayList2[index2];
            arrayList1.AddRange((ICollection) c);
            if (index2 != count2 - 1)
              arrayList1.Add((object) this.m_Session.GetMarkCharacter(3));
          }
          if (index1 != count1 - 1)
            arrayList1.Add((object) this.m_Session.GetMarkCharacter(2));
        }
        byte[] pData = new byte[arrayList1.Count];
        arrayList1.CopyTo((Array) pData);
        return new UniDynArray(this.m_Session, pData);
      }
      if (aValue == 0)
        aValue = 1;
      if (aValue > dyn.Count || aValue < 0)
        return new UniDynArray(this.m_Session, string.Empty);
      ArrayList arrayList3 = (ArrayList) dyn[aValue - 1];
      if (aSubValue == 0)
      {
        int count = arrayList3.Count;
        for (int index = 0; index < count; ++index)
        {
          byte[] c = (byte[]) arrayList3[index];
          arrayList1.AddRange((ICollection) c);
          if (index != count - 1)
            arrayList1.Add((object) this.m_Session.GetMarkCharacter(3));
        }
        byte[] pData = new byte[arrayList1.Count];
        arrayList1.CopyTo((Array) pData);
        return new UniDynArray(this.m_Session, pData);
      }
      return aSubValue > arrayList3.Count || aSubValue < 0 ? new UniDynArray(this.m_Session, string.Empty) : new UniDynArray(this.m_Session, (byte[]) arrayList3[aSubValue - 1]);
    }

    public UniDynArray Extract(int aField, int aValue) => this.Extract(aField, aValue, 0);

    public UniDynArray Extract(int aField) => this.Extract(aField, 0, 0);

    public void Insert(int aField, int aValue, int aSubValue, string insertString)
    {
      if (insertString.Length == 0)
      {
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, "UniDynArray::Insert( int aField, int aValue, int aSubValue, string insertString ) Return from insertString.Equals(\"\") ");
      }
      else
      {
        this.CheckArrayState(true);
        if (aField < 0)
        {
          ArrayList arrayList1 = new ArrayList();
          ArrayList arrayList2 = new ArrayList();
          this.m_DynArray.Add((object) arrayList1);
          arrayList1.Add((object) arrayList2);
          arrayList2.Add((object) this.m_Session.UniCodeStringToByteArray(insertString));
          if (!this.HasMarkCharacters(this.m_Session.UniCodeStringToByteArray(insertString)))
            return;
          this.RegenerateDynArray();
        }
        else if (aField == 0)
        {
          ArrayList arrayList3 = new ArrayList();
          ArrayList arrayList4 = new ArrayList();
          this.m_DynArray.Insert(0, (object) arrayList3);
          arrayList3.Insert(0, (object) arrayList4);
          byte[] byteArray = this.m_Session.UniCodeStringToByteArray(insertString);
          arrayList4.Add((object) byteArray);
          if (!this.HasMarkCharacters(this.m_Session.UniCodeStringToByteArray(insertString)))
            return;
          this.RegenerateDynArray();
        }
        else
        {
          if (aField > this.m_DynArray.Count)
          {
            for (int count = this.m_DynArray.Count; count < aField - 1; ++count)
            {
              ArrayList arrayList5 = new ArrayList();
              ArrayList arrayList6 = new ArrayList();
              this.m_DynArray.Add((object) arrayList5);
              arrayList5.Add((object) arrayList6);
              arrayList6.Add((object) this.m_Session.UniCodeStringToByteArray(string.Empty));
            }
            if (aValue != 0 || aSubValue != 0)
              this.m_DynArray.Add((object) new ArrayList());
          }
          if (aValue == 0 && aSubValue == 0)
          {
            ArrayList arrayList7 = new ArrayList();
            ArrayList arrayList8 = new ArrayList();
            this.m_DynArray.Insert(aField - 1, (object) arrayList7);
            arrayList7.Insert(0, (object) arrayList8);
            arrayList8.Add((object) this.m_Session.UniCodeStringToByteArray(insertString));
            if (!this.HasMarkCharacters(this.m_Session.UniCodeStringToByteArray(insertString)))
              return;
            this.RegenerateDynArray();
          }
          else
          {
            if (aValue == 0)
              aValue = 1;
            ArrayList dyn = (ArrayList) this.m_DynArray[aField - 1];
            if (aValue < 0)
              aValue = dyn.Count + 1;
            if (aValue > dyn.Count)
            {
              for (int count = dyn.Count; count < aValue - 1; ++count)
              {
                ArrayList arrayList = new ArrayList();
                dyn.Add((object) arrayList);
                arrayList.Add((object) this.m_Session.UniCodeStringToByteArray(string.Empty));
              }
              if (aSubValue != 0)
              {
                ArrayList arrayList = new ArrayList();
                dyn.Add((object) arrayList);
              }
            }
            if (aSubValue == 0)
            {
              ArrayList arrayList = new ArrayList();
              dyn.Insert(aValue - 1, (object) arrayList);
              arrayList.Add((object) this.m_Session.UniCodeStringToByteArray(insertString));
              if (!this.HasMarkCharacters(this.m_Session.UniCodeStringToByteArray(insertString)))
                return;
              this.RegenerateDynArray();
            }
            else
            {
              ArrayList arrayList = (ArrayList) dyn[aValue - 1];
              if (aSubValue < 0)
              {
                byte[] byteArray = this.m_Session.UniCodeStringToByteArray(string.Empty);
                byte[] numArray = (byte[]) null;
                if (arrayList.Count > 0)
                  numArray = (byte[]) arrayList[0];
                if (numArray != null && arrayList.Count == 1 && byteArray.Length == arrayList.Count)
                  arrayList[0] = (object) insertString;
                else
                  arrayList.Add((object) this.m_Session.UniCodeStringToByteArray(insertString));
              }
              else
              {
                if (aSubValue > arrayList.Count)
                {
                  for (int count = arrayList.Count; count < aSubValue - 1; ++count)
                    arrayList.Add((object) this.m_Session.UniCodeStringToByteArray(string.Empty));
                }
                byte[] byteArray = this.m_Session.UniCodeStringToByteArray(string.Empty);
                byte[] numArray = (byte[]) null;
                if (arrayList.Count > 0)
                  numArray = (byte[]) arrayList[0];
                if (numArray != null && arrayList.Count == 1 && byteArray.Length == numArray.Length)
                  arrayList[0] = (object) this.m_Session.UniCodeStringToByteArray(insertString);
                else
                  arrayList.Insert(aSubValue - 1, (object) this.m_Session.UniCodeStringToByteArray(insertString));
              }
              if (!this.HasMarkCharacters(this.m_Session.UniCodeStringToByteArray(insertString)))
                return;
              this.RegenerateDynArray();
            }
          }
        }
      }
    }

    public void Insert(int aField, int aValue, string aString) => this.Insert(aField, aValue, 0, aString);

    public void Insert(int aField, string aString) => this.Insert(aField, 0, 0, aString);

    public int Length(int aField, int aValue, int aSubValue)
    {
      if (aField < 0)
        return 0;
      this.CheckArrayState(false);
      return this.Extract(aField, aValue, aSubValue).ToString().Length;
    }

    public int Length(int aField, int aValue) => this.Length(aField, aValue, 0);

    public int Length(int aField) => this.Length(aField, 0, 0);

    public UniDynArray Remove(int aField, int aValue, int aSubValue)
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniDynArray::Remove( int aField, int aValue, int aSubValue )");
      this.CheckArrayState(true);
      UniDynArray uniDynArray = this.Extract(aField, aValue, aSubValue);
      this.Delete(aField, aValue, aSubValue);
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniDynArray::Remove( int aField, int aValue, int aSubValue )");
      return uniDynArray;
    }

    public UniDynArray Remove(int aField, int aValue) => this.Remove(aField, aValue, 0);

    public UniDynArray Remove(int aField) => this.Remove(aField, 0, 0);

    public void Replace(int aField, int aValue, int aSubValue, string replaceString)
    {
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Enter UniDynArray::Replace( int aField, int aValue, int aSubValue, string  replaceString )");
      if (aField < 0 || aValue < 0 || aSubValue < 0)
      {
        this.Insert(aField, aValue, aSubValue, replaceString);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniDynArray::Replace( int aField, int aValue, int aSubValue, string  replaceString )");
      }
      else if (aField == 0)
      {
        this.Insert(1, aValue, aSubValue, replaceString);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniDynArray::Replace( int aField, int aValue, int aSubValue, string  replaceString )");
      }
      else
      {
        this.CheckArrayState(true);
        if (aField > this.m_DynArray.Count)
        {
          for (int count = this.m_DynArray.Count; count < aField; ++count)
          {
            ArrayList arrayList1 = new ArrayList();
            ArrayList arrayList2 = new ArrayList();
            this.m_DynArray.Add((object) arrayList1);
            arrayList1.Add((object) arrayList2);
            arrayList2.Add((object) this.m_Session.UniCodeStringToByteArray(string.Empty));
          }
        }
        ArrayList dyn = (ArrayList) this.m_DynArray[aField - 1];
        if (aValue == 0)
        {
          ArrayList arrayList3 = new ArrayList();
          ArrayList arrayList4 = new ArrayList();
          this.m_DynArray[aField - 1] = (object) arrayList3;
          arrayList3.Add((object) arrayList4);
          arrayList4.Add((object) this.m_Session.UniCodeStringToByteArray(replaceString));
          if (this.HasMarkCharacters(this.m_Session.UniCodeStringToByteArray(replaceString)))
            this.RegenerateDynArray();
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniDynArray::Replace( int aField, int aValue, int aSubValue, string  replaceString )");
        }
        else
        {
          if (aValue > dyn.Count)
          {
            for (int count = dyn.Count; count < aValue; ++count)
            {
              ArrayList arrayList = new ArrayList();
              dyn.Add((object) arrayList);
              arrayList.Add((object) this.m_Session.UniCodeStringToByteArray(string.Empty));
            }
          }
          ArrayList arrayList5 = (ArrayList) dyn[aValue - 1];
          if (aSubValue > arrayList5.Count)
          {
            for (int count = arrayList5.Count; count < aSubValue; ++count)
              arrayList5.Add((object) this.m_Session.UniCodeStringToByteArray(string.Empty));
          }
          if (aSubValue == 0)
          {
            ArrayList arrayList6 = new ArrayList();
            dyn[aValue - 1] = (object) arrayList6;
            arrayList6.Add((object) this.m_Session.UniCodeStringToByteArray(replaceString));
          }
          else
            arrayList5[aSubValue - 1] = (object) this.m_Session.UniCodeStringToByteArray(replaceString);
          if (this.HasMarkCharacters(this.m_Session.UniCodeStringToByteArray(replaceString)))
            this.RegenerateDynArray();
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_INFO, "Exit UniDynArray::Replace( int aField, int aValue, int aSubValue, string  replaceString )");
        }
      }
    }

    public void Replace(int aField, int aValue, string aString) => this.Replace(aField, aValue, 0, aString);

    public void Replace(int aField, string aString) => this.Replace(aField, 0, 0, aString);

    public byte[] ToByteArray()
    {
      if (this.m_IsByteArrayActive)
        return this.m_Value;
      ArrayList arrayList1 = new ArrayList();
      this.CheckArrayState(false);
      int count1 = this.m_DynArray.Count;
      for (int index1 = 0; index1 < count1; ++index1)
      {
        ArrayList dyn = (ArrayList) this.m_DynArray[index1];
        int count2 = dyn.Count;
        for (int index2 = 0; index2 < count2; ++index2)
        {
          ArrayList arrayList2 = (ArrayList) dyn[index2];
          int count3 = arrayList2.Count;
          for (int index3 = 0; index3 < count3; ++index3)
          {
            byte[] c = (byte[]) arrayList2[index3];
            arrayList1.AddRange((ICollection) c);
            if (index3 != count3 - 1)
              arrayList1.Add((object) this.m_Session.GetMarkCharacter(3));
          }
          if (index2 != count2 - 1)
            arrayList1.Add((object) this.m_Session.GetMarkCharacter(2));
        }
        if (index1 < count1 - 1)
          arrayList1.Add((object) this.m_Session.GetMarkCharacter(1));
      }
      byte[] byteArray = new byte[arrayList1.Count];
      arrayList1.CopyTo((Array) byteArray);
      this.m_Value = byteArray;
      this.m_IsByteArrayActive = true;
      return byteArray;
    }

    public override string ToString() => this.m_Session.ByteArrayToUniCodeString(this.ToByteArray());

    internal string[] ToArrayString(char pDelim)
    {
      string[] arrayString = (string[]) null;
      string str = this.ToString();
      if (str.Length > 0)
        arrayString = str.Split(pDelim);
      return arrayString;
    }

    public void PrintByteArray()
    {
      int num1 = 0;
      foreach (byte num2 in this.m_Value)
      {
        string message = string.Format("byte[{0}] = {1}", (object) num1, (object) num2);
        Console.WriteLine("byte[{0}] = {1}", (object) num1, (object) num2);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_VERBOSE, message);
        ++num1;
      }
    }

    private void RegenerateDynArray()
    {
      this.m_Value = this.ToByteArray();
      this.ConstructDynArray(this.m_Value);
      this.m_IsArrayActive = true;
      this.m_IsByteArrayActive = true;
    }

    private void CheckArrayState(bool changeStringState)
    {
      if (!this.m_IsArrayActive)
      {
        this.ConstructDynArray(this.m_Value);
        this.m_IsArrayActive = true;
      }
      if (!changeStringState)
        return;
      this.m_IsByteArrayActive = false;
    }

    private bool HasMarkCharacters(byte[] pByteArray) => Array.IndexOf((Array) pByteArray, (object) this.m_Session.GetMarkCharacter(1), 0) != -1 || Array.IndexOf((Array) pByteArray, (object) this.m_Session.GetMarkCharacter(2), 0) != -1 || Array.IndexOf((Array) pByteArray, (object) this.m_Session.GetMarkCharacter(3), 0) != -1;

    private void ConstructDynArray(byte[] itemArray)
    {
      int num1 = 0;
      this.m_DynArray = new ArrayList();
      if (itemArray == null || itemArray.Length == 0)
        return;
      int num2;
      for (; num1 != -1; num1 = num2)
      {
        ArrayList arrayList1 = new ArrayList();
        this.m_DynArray.Add((object) arrayList1);
        num2 = Array.IndexOf((Array) itemArray, (object) this.m_Session.GetMarkCharacter(1), num1);
        byte[] numArray1;
        if (num2 != -1)
        {
          int length = num2 - num1;
          numArray1 = new byte[length];
          Array.Copy((Array) itemArray, num1, (Array) numArray1, 0, length);
        }
        else
        {
          int length = itemArray.Length - num1;
          numArray1 = new byte[length];
          Array.Copy((Array) itemArray, num1, (Array) numArray1, 0, length);
        }
        int num3;
        for (int index1 = 0; index1 != -1; index1 = num3)
        {
          ArrayList arrayList2 = new ArrayList();
          arrayList1.Add((object) arrayList2);
          num3 = Array.IndexOf((Array) numArray1, (object) this.m_Session.GetMarkCharacter(2), index1);
          byte[] numArray2;
          if (num3 != -1)
          {
            int length = num3 - index1;
            numArray2 = new byte[length];
            Array.Copy((Array) numArray1, index1, (Array) numArray2, 0, length);
          }
          else
          {
            int length = numArray1.Length - index1;
            numArray2 = new byte[length];
            Array.Copy((Array) numArray1, index1, (Array) numArray2, 0, length);
          }
          int num4;
          for (int index2 = 0; index2 != -1; index2 = num4)
          {
            num4 = Array.IndexOf((Array) numArray2, (object) this.m_Session.GetMarkCharacter(3), index2);
            byte[] destinationArray;
            if (num4 != -1)
            {
              int length = num4 - index2;
              destinationArray = new byte[length];
              Array.Copy((Array) numArray2, index2, (Array) destinationArray, 0, length);
            }
            else
            {
              int length = numArray2.Length - index2;
              destinationArray = new byte[length];
              Array.Copy((Array) numArray2, index2, (Array) destinationArray, 0, length);
            }
            arrayList2.Add((object) destinationArray);
            if (num4 != -1)
              ++num4;
          }
          if (num3 != -1)
            ++num3;
        }
        if (num2 != -1)
          ++num2;
      }
    }

    private int CountMarks(int aField, int aValue, int aSubValue, int except_val)
    {
      if (this.ToByteArray().Length == 0)
        return 0;
      this.CheckArrayState(false);
      int count1 = this.m_DynArray.Count;
      if (aField < 0 || aField > count1)
        return 0;
      if (aField == 0)
        return count1 - 1;
      ArrayList dyn = (ArrayList) this.m_DynArray[aField - 1];
      int count2 = dyn.Count;
      if (aValue < 0 || aValue > count2)
        return except_val;
      if (aValue == 0)
        return count2 == 1 && dyn[0].ToString().Equals("[]") ? except_val : count2 - 1;
      ArrayList arrayList = (ArrayList) dyn[aValue - 1];
      int count3 = arrayList.Count;
      if (aSubValue < 0 || aSubValue > count3)
        return except_val;
      if (aSubValue != 0)
        return count3;
      return count3 == 1 && arrayList[0].ToString().Length == 0 ? except_val : count3 - 1;
    }

    private void InitDefaultValues(UniSession aSession, string pString)
    {
      if (aSession == null || aSession.UniRPCConnection == null)
      {
        UniDynArrayException dynArrayException = new UniDynArrayException(UniErrorMessage.GetErrorMessage(39120));
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, dynArrayException.Message + " Source: " + dynArrayException.Source + " Method: UniCommand(UniSession aSession)");
        throw dynArrayException;
      }
      this.m_Session = aSession;
      byte[] byteArray = this.m_Session.UniCodeStringToByteArray(pString);
      this.m_FileEncryptionType = aSession.EncryptionType;
      int length = byteArray.Length;
      int destinationIndex = 0;
      byte[] destinationArray;
      if (this.m_Value == null)
      {
        destinationArray = new byte[length + destinationIndex];
      }
      else
      {
        destinationIndex = this.m_Value.Length;
        destinationArray = new byte[length + destinationIndex];
      }
      Array.Copy((Array) byteArray, 0, (Array) destinationArray, destinationIndex, length);
      this.m_Value = destinationArray;
      this.m_IsArrayActive = false;
      this.m_IsByteArrayActive = true;
    }

    public string StringValue => this.ToString();

    internal ArrayList UniDynArrayList
    {
      get
      {
        this.RegenerateDynArray();
        return this.m_DynArray;
      }
    }

    public UniSession CurrentSession => this.m_Session;
  }
}
