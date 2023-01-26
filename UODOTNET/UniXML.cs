// Decompiled with JetBrains decompiler
// Type: IBMU2.UODOTNET.UniXML
// Assembly: UODOTNET, Version=2.1.0.7149, Culture=neutral, PublicKeyToken=335f3fbd4be82339
// MVID: E7861CAA-9C07-4D40-B738-BD78A3391394
// Assembly location: C:\Users\tclark\source\repos\MertexerV2\MertexerV2\ExternalDependencies\UODOTNET.dll

using System;
using System.Data;
using System.IO;

namespace IBMU2.UODOTNET
{
  public class UniXML : UniRoot
  {
    private UniSession m_Session = (UniSession) null;
    private string m_Xmlstr = (string) null;
    private string m_Xsdstr = (string) null;
    private string m_Errstr = (string) null;
    private int m_Errorcode = 0;

    internal UniXML(UniSession us) => this.m_Session = us;

    public void GenerateXML(string cmd)
    {
      string empty = string.Empty;
      UniSubroutine uniSubroutine;
      try
      {
        uniSubroutine = this.m_Session.CreateUniSubroutine("*GETXMLSUB", 6);
        uniSubroutine.SetArg(0, cmd);
        uniSubroutine.SetArg(1, empty);
        uniSubroutine.Call();
      }
      catch (UniSubroutineException ex)
      {
        string message = ex.Message + UniErrorMessage.GetErrorMessage(91);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
        throw new UniXMLException(message);
      }
      this.m_Errstr = uniSubroutine.GetArg(4);
      this.m_Errorcode = Convert.ToInt32(this.m_Errstr);
      if (this.m_Errorcode == 0)
      {
        this.m_Xmlstr = uniSubroutine.GetArg(2);
        this.m_Xsdstr = uniSubroutine.GetArg(3);
      }
      else
      {
        this.m_Errstr = uniSubroutine.GetArg(5);
        UniXMLException uniXmlException = new UniXMLException(this.m_Errstr);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniXmlException.Message + " Source: " + uniXmlException.Source + " Method: " + (object) uniXmlException.TargetSite);
        throw uniXmlException;
      }
    }

    public void GenerateXML(string cmd, string options)
    {
      UniSubroutine uniSubroutine;
      try
      {
        uniSubroutine = this.m_Session.CreateUniSubroutine("*GETXMLSUB", 6);
        uniSubroutine.SetArg(0, cmd);
        uniSubroutine.SetArg(1, options);
        uniSubroutine.Call();
      }
      catch (UniSubroutineException ex)
      {
        string message = ex.Message + UniErrorMessage.GetErrorMessage(91);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
        throw new UniXMLException(message);
      }
      this.m_Errstr = uniSubroutine.GetArg(4);
      this.m_Errorcode = Convert.ToInt32(this.m_Errstr);
      if (this.m_Errorcode == 0)
      {
        this.m_Xmlstr = uniSubroutine.GetArg(2);
        this.m_Xsdstr = uniSubroutine.GetArg(3);
      }
      else
      {
        this.m_Errstr = uniSubroutine.GetArg(5);
        this.m_Xmlstr = (string) null;
        this.m_Xsdstr = (string) null;
        UniXMLException uniXmlException = new UniXMLException(this.m_Errstr);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniXmlException.Message + " Source: " + uniXmlException.Source + " Method: " + (object) uniXmlException.TargetSite);
        throw uniXmlException;
      }
    }

    public DataSet GetDataSet()
    {
      DataSet dataSet = new DataSet();
      if (this.m_Xmlstr == null || this.m_Xmlstr.Length == 0)
      {
        this.m_Errstr = "Empty xml data.";
        UniXMLException uniXmlException = new UniXMLException(this.m_Errstr);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniXmlException.Message + " Source: " + uniXmlException.Source + " Method: " + (object) uniXmlException.TargetSite);
        throw uniXmlException;
      }
      StringReader reader = new StringReader(this.m_Xmlstr);
      int num = (int) dataSet.ReadXml((TextReader) reader, XmlReadMode.Auto);
      reader.Close();
      return dataSet;
    }

    public void GenerateXMLUsingXmap(string xmapname)
    {
      string empty = string.Empty;
      UniSubroutine uniSubroutine;
      try
      {
        uniSubroutine = this.m_Session.CreateUniSubroutine("*DBTOXMLXMAP", 5);
        uniSubroutine.SetArg(0, xmapname);
        uniSubroutine.SetArg(1, empty);
        uniSubroutine.Call();
      }
      catch (UniSubroutineException ex)
      {
        string message = ex.Message + UniErrorMessage.GetErrorMessage(91);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
        throw new UniXMLException(message);
      }
      this.m_Errstr = uniSubroutine.GetArg(3);
      this.m_Errorcode = Convert.ToInt32(this.m_Errstr);
      if (this.m_Errorcode == 0)
      {
        this.m_Xmlstr = uniSubroutine.GetArg(2);
        this.m_Xsdstr = (string) null;
      }
      else
      {
        this.m_Errstr = uniSubroutine.GetArg(4);
        UniXMLException uniXmlException = new UniXMLException(this.m_Errstr);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniXmlException.Message + " Source: " + uniXmlException.Source + " Method: " + (object) uniXmlException.TargetSite);
        throw uniXmlException;
      }
    }

    public void UpdateDataUsingXmap(string xmapname)
    {
      if (this.m_Xmlstr != null)
      {
        if (this.m_Xmlstr.Length != 0)
        {
          UniSubroutine uniSubroutine;
          try
          {
            uniSubroutine = this.m_Session.CreateUniSubroutine("*XMLTODBXMAP", 4);
            uniSubroutine.SetArg(0, xmapname);
            uniSubroutine.SetArg(1, this.m_Xmlstr);
            uniSubroutine.SetArg(2, "1");
            uniSubroutine.Call();
          }
          catch (UniSubroutineException ex)
          {
            string message = ex.Message + UniErrorMessage.GetErrorMessage(91);
            UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
            throw new UniXMLException(message);
          }
          this.m_Errstr = uniSubroutine.GetArg(2);
          this.m_Errorcode = Convert.ToInt32(this.m_Errstr);
          if (this.m_Errorcode == 0)
            return;
          this.m_Errstr = uniSubroutine.GetArg(3);
          UniXMLException uniXmlException = new UniXMLException(this.m_Errstr);
          UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniXmlException.Message + " Source: " + uniXmlException.Source + " Method: " + (object) uniXmlException.TargetSite);
          throw uniXmlException;
        }
      }
      this.m_Errstr = "No xmldata to update server file";
      UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, this.m_Errstr);
      throw new UniXMLException(this.m_Errstr);
    }

    public void UpdateDataUsingXmap(string xmapname, string xmlname)
    {
      UniSubroutine uniSubroutine;
      try
      {
        uniSubroutine = this.m_Session.CreateUniSubroutine("*XMLTODBXMAP", 4);
        uniSubroutine.SetArg(0, xmapname);
        uniSubroutine.SetArg(1, xmlname);
        uniSubroutine.SetArg(2, "0");
        uniSubroutine.Call();
      }
      catch (UniSubroutineException ex)
      {
        string message = ex.Message + UniErrorMessage.GetErrorMessage(91);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, message);
        throw new UniXMLException(message);
      }
      this.m_Errstr = uniSubroutine.GetArg(2);
      this.m_Errorcode = Convert.ToInt32(this.m_Errstr);
      if (this.m_Errorcode != 0)
      {
        this.m_Errstr = uniSubroutine.GetArg(3);
        UniXMLException uniXmlException = new UniXMLException(this.m_Errstr);
        UniRoot.UniLog(UniRoot.UniTraceLevel.UO_ERROR, uniXmlException.Message + " Source: " + uniXmlException.Source + " Method: " + (object) uniXmlException.TargetSite);
        throw uniXmlException;
      }
    }

    public string XMLString
    {
      get => this.m_Xmlstr;
      set => this.m_Xmlstr = value;
    }

    public string XSDString
    {
      get => this.m_Xsdstr;
      set => this.m_Xsdstr = value;
    }

    public string Errmsg => this.m_Errstr;

    public int Errcode => this.m_Errorcode;
  }
}
