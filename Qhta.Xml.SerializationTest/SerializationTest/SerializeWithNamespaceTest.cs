using System.Xml;
using System.Xml.Serialization;

public class ClassForSerializeWithNamespace
{
}

public static class SerializeWithNamespaceTest
{
  public static void Run()
  {
    SerializeWithNamespace();
  }

  public static void SerializeWithNamespace()
  {
    var testInstance = new ClassForSerializeWithNamespace();
    var serializer = new XmlSerializer(typeof(ClassForSerializeWithNamespace), "http://specifiedNamespace");
    var namespaces = new XmlSerializerNamespaces();
    namespaces.Add("def", "http://specifiedNamespace");
    using (var txtWriter = File.CreateText("SerializeWithNamespace.xml"))
    {
      using (var xmlWriter = XmlWriter.Create(txtWriter, new XmlWriterSettings { Indent = true }))
      {
        serializer.Serialize(xmlWriter, testInstance, namespaces);
      }
    }
  }
}