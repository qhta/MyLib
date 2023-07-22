using System.Xml;
using System.Xml.Serialization;

public class ClassWithNoNamespace
{
}

public static class NoNamespaceTest
{
  public static void Run()
  {
    WriteNoNamespaceClass();
  }

  public static void WriteNoNamespaceClass()
  {
    var noNamespaceInstance = new ClassWithNoNamespace();
    var serializer = new XmlSerializer(typeof(ClassWithNoNamespace));
    using (var txtWriter = File.CreateText("NoNamespaceClass.xml"))
    {
      using (var xmlWriter = XmlWriter.Create(txtWriter, new XmlWriterSettings { Indent = true }))
      {
        serializer.Serialize(xmlWriter, noNamespaceInstance);
      }
    }
  }
}