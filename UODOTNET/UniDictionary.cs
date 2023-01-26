// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniDictionary
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

namespace IBMU2.UODOTNET
{
  public class UniDictionary : UniFile
  {
    internal UniDictionary(UniSession pSession, string pFileName, int pOption)
      : base(pSession, pFileName, pOption)
    {
    }

    public UniDynArray GetAssoc() => this.GetIndexedField(this.RecordID, 7);

    public void SetAssoc(UniDynArray aString) => this.SetIndexedField(this.RecordID, aString, 7);

    public void SetAssoc(string aRecordID, UniDynArray aString) => this.SetIndexedField(aRecordID, aString, 7);

    public UniDynArray GetConv() => this.GetIndexedField(this.RecordID, 3);

    public UniDynArray GetConv(string aRecordID) => this.GetIndexedField(aRecordID, 3);

    public void SetConv(UniDynArray aString) => this.SetIndexedField(this.RecordID, aString, 3);

    public void SetConv(string aRecordID, UniDynArray aString) => this.SetIndexedField(aRecordID, aString, 3);

    public UniDynArray GetFormat() => this.GetIndexedField(this.RecordID, 5);

    public UniDynArray GetFormat(string aRecordID) => this.GetIndexedField(aRecordID, 5);

    public void SetFormat(UniDynArray aString) => this.SetIndexedField(this.RecordID, aString, 5);

    public void SetFormat(string aRecordID, UniDynArray aString) => this.SetIndexedField(aRecordID, aString, 5);

    public UniDynArray GetLoc() => this.GetIndexedField(this.RecordID, 2);

    public UniDynArray GetLoc(string aRecordID) => this.GetIndexedField(aRecordID, 2);

    public void SetLoc(UniDynArray aString) => this.SetIndexedField(this.RecordID, aString, 2);

    public void SetLoc(string aRecordID, UniDynArray aString) => this.SetIndexedField(aRecordID, aString, 2);

    public UniDynArray GetName() => this.GetIndexedField(this.RecordID, 4);

    public UniDynArray GetName(string aRecordID) => this.GetIndexedField(aRecordID, 4);

    public void SetName(UniDynArray aString) => this.SetIndexedField(this.RecordID, aString, 4);

    public void SetName(string aRecordID, UniDynArray aString) => this.SetIndexedField(aRecordID, aString, 4);

    public UniDynArray GetSM() => this.GetIndexedField(this.RecordID, 6);

    public UniDynArray GetSM(string aRecordID) => this.GetIndexedField(aRecordID, 6);

    public void SetSM(UniDynArray aString) => this.SetIndexedField(this.RecordID, aString, 6);

    public void SetSM(string aRecordID, UniDynArray aString) => this.SetIndexedField(aRecordID, aString, 6);

    public UniDynArray GetSQLType() => this.GetIndexedField(this.RecordID, 8);

    public UniDynArray GetSQLType(string aRecordID) => this.GetIndexedField(aRecordID, 8);

    public void SetSQLType(UniDynArray aString) => this.SetIndexedField(this.RecordID, aString, 8);

    public void SetSQLType(string aString) => this.SetIndexedField(this.RecordID, new UniDynArray(this.m_Session, aString), 8);

    public void SetSQLType(string aRecordID, UniDynArray aString) => this.SetIndexedField(aRecordID, aString, 8);

    public void SetSQLType(string aRecordID, string aString) => this.SetIndexedField(aRecordID, new UniDynArray(this.m_Session, aString), 8);

    public UniDynArray GetFieldType(string aRecordID) => this.GetIndexedField(aRecordID, 1);

    public UniDynArray GetFieldType() => this.GetIndexedField(this.RecordID, 1);

    public void SetType(UniDynArray aString) => this.SetIndexedField(this.RecordID, aString, 1);

    public void SetType(string aRecordID, UniDynArray aString) => this.SetIndexedField(aRecordID, aString, 1);

    private UniDynArray GetIndexedField(string aRecordID, int aType) => this.ReadField(aRecordID, aType);

    private void SetIndexedField(string aRecordID, UniDynArray aString, int aType) => this.WriteField(aRecordID, aType, aString);
  }
}
