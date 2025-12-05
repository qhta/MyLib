using System.Xml;
using System.Xml.Serialization;

[XmlRoot(Namespace = "http://rootNamespace")]
public class ClassForSerializeClassWithNamespaceTest
{
}

public static class SerializeClassWithNamespaceTest
{
  public static void Run()
  {
    SerializeWithNamespace();
  }

  public static void SerializeWithNamespace()
  {
    var testInstance = new ClassForSerializeClassWithNamespaceTest();
    var serializer = new XmlSerializer(typeof(ClassForSerializeClassWithNamespaceTest), "http://specifiedNamespace");
    var namespaces = new XmlSerializerNamespaces();
    namespaces.Add("def", "http://specifiedNamespace");
    namespaces.Add("root", "http://rootNamespace");
    using (var txtWriter = File.CreateText("SerializeClassWithNamespace.xml"))
    {
      using (var xmlWriter = XmlWriter.Create(txtWriter, new XmlWriterSettings { Indent = true }))
      {
        serializer.Serialize(xmlWriter, testInstance, namespaces);
      }
    }
  }
}