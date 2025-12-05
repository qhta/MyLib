using System.IO;
using System.Windows.Markup;

namespace XamlSerializationTest
{
  using System.Xml;

  namespace Root
  {
    public class Class1
    {
      
      public Specific.Class1 Subclass1 { get; set; } = new();

      
      public Class2 Subclass2 { get; set; } = new();
    }
  }

  namespace Specific
  {
    public class Class1
    {
      public string Name { get; set; } = "Subclass1";
    }
  }

  namespace Root
  {

    public class Class2
    {
      public string Name { get; set; } = "Class2";
    }
  }

  public static class SerializeTwoClassesWithNamespaceTest
  {
    public static void Run()
    {
      SerializeWithNamespace();
    }

    public static void SerializeWithNamespace()
    {
      var testInstance = new Root.Class1();
      using (var txtWriter = File.CreateText("SerializeTwoClassesWithNamespace.xml"))
      {
        using (var xmlWriter = XmlWriter.Create(txtWriter, new XmlWriterSettings { Indent = true }))
        {
          XamlWriter.Save(testInstance, xmlWriter);
        }
      }
    }

  }
}