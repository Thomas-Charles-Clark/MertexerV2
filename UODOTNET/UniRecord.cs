// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniRecord
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

using System.Text;

namespace IBMU2.UODOTNET
{
  public class UniRecord : UniRoot
  {
    private string m_RecId;
    private UniDynArray m_RetData;
    private int m_Status;
    private int m_ReturnValue;

    public UniDynArray Record
    {
      get => this.m_RetData;
      set => this.m_RetData = value;
    }

    public string RecordID
    {
      get => this.m_RecId;
      set => this.m_RecId = value;
    }

    public int RecordStatus
    {
      get => this.m_Status;
      set => this.m_Status = value;
    }

    public int RecordReturnValue
    {
      get => this.m_ReturnValue;
      set => this.m_ReturnValue = value;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("\n");
      stringBuilder.Append("Record ID : ");
      stringBuilder.Append(this.m_RecId.ToString());
      stringBuilder.Append("\n");
      stringBuilder.Append("Record Status : ");
      stringBuilder.Append(this.m_Status);
      stringBuilder.Append("\n");
      stringBuilder.Append("Record ReturnValue : ");
      stringBuilder.Append(this.m_ReturnValue);
      stringBuilder.Append("\n");
      stringBuilder.Append("Record : ");
      stringBuilder.Append(this.m_RetData.ToString());
      return stringBuilder.ToString();
    }
  }
}
