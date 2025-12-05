using System.Xml;
using System.Xml.Serialization;

public class ClassWithDefaultNamespace
{
}

public static class DefaultNamespaceTest
{
  public static void Run()
  {
    WriteDefaultNamespaceClass();
  }

  public static void WriteDefaultNamespaceClass()
  {
    var noNamespaceInstance = new ClassWithDefaultNamespace();
    var serializer = new XmlSerializer(typeof(ClassWithDefaultNamespace), "http://defaultNamespaceTest");
    using (var txtWriter = File.CreateText("DefaultNamespaceClass.xml"))
    {
      using (var xmlWriter = XmlWriter.Create(txtWriter, new XmlWriterSettings { Indent = true }))
      {
        serializer.Serialize(xmlWriter, noNamespaceInstance);
      }
    }
  }
}