﻿using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

using Qhta.Xml.Serialization;

using Root;

//[assembly: ContractNamespace("http://rootNamespace", ClrNamespace = "Root")]
//[assembly: ContractNamespace("http://specificNamespace", ClrNamespace = "Specific")]

namespace Root
{
  [XmlRoot(Namespace = "http://rootNamespace")]
  public class Class1
  {
    [XmlElement]
    public Specific.Class1 Subclass1 { get; set; } = new();

    [XmlElement]
    public Class2 Subclass2 { get; set; } = new();
  }
}

namespace Specific
{
  [XmlRoot(Namespace = "http://specificNamespace")]
  public class Class1
  {
    [XmlAttribute]
    public string Name { get; set; } = "Subclass1";
    public Root.Class1? Parent { get; set; }
  }
}

namespace Root
{

  [XmlRoot(Namespace = "http://rootNamespace")]
  public class Class2
  {
    [XmlAttribute]
    public string Name { get; set; } = "Class2";
  }
}

namespace SerializationTest
{
  using Root;

  public class SerializeTwoClassesWithNamespaceTest: SerializerTest<Class1>
  {
    public override bool? Run()
    {
      SerializeWithNamespace();
      return true;
    }

    protected override Class1 CreateObject()
    {
      var testInstance = new Root.Class1();
      return testInstance;
    }

    protected override void ShowObject(Class1 obj)
    {
    }

    public void SerializeWithNamespace()
    {
      var testInstance = new Root.Class1();
      var namespaces = new XmlSerializerNamespaces();
      namespaces.Add("root", "http://rootNamespace");
      namespaces.Add("spec", "http://specificNamespace");
      var serializer = new QXmlSerializer(typeof(Root.Class1), "http://rootNamespace");
      using (var txtWriter = File.CreateText("SerializeTwoClassesWithNamespace.xml"))
      {
        using (var xmlWriter = XmlWriter.Create(txtWriter, new XmlWriterSettings { Indent = true }))
        {
          serializer.Serialize(xmlWriter, testInstance, namespaces);
        }
      }
    }

  }
}