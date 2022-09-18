using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Qhta.Xml.Serialization;
using Qhta.TestHelper;


public abstract class SerializerTest<ObjectType>
{
  public abstract bool Run();

  public bool Run(String filename)
  {
    var filenameOK = filename.Insert(filename.LastIndexOf('.'), ".OK");
    TraceTextWriter traceWriter = new TraceTextWriter(true, true);
    var obj = CreateObject();
    bool serializeOK = false;
    bool? deserializeOK = null;
    SerializeObject(filename, obj);
    if (File.Exists(filename))
    {
      if (File.Exists(filenameOK))
      {
        var xmlFileComparer = new XmlFileComparer(new FileCompareOptions(), traceWriter);
        serializeOK = xmlFileComparer.CompareFiles(filename, filenameOK);
        if (serializeOK)
          Console.WriteLine("Serialization passed");
        else
          Console.WriteLine("Serialization difference");
      }
      else
      {
        Console.WriteLine("Serialization passed");
        serializeOK = true;
      }
      var temp = DeserializeObject(filename);
      if (temp is ObjectType obj2)
      {
        ShowObject(obj2);
        var objectComparer = new ObjectComparer(traceWriter);
        if (objectComparer.DeepCompare(obj, obj2))
        {
          deserializeOK = true;
          Console.WriteLine("Deserialization passed");
        }
        else
        {
          deserializeOK = false;
          Console.WriteLine("Deserialization difference");
        }
      }
      else
        Console.WriteLine("Deserialization failed");
    }
    else
      Console.WriteLine("Serialization failed");
    return serializeOK && (deserializeOK ?? true);
  }

  protected abstract ObjectType CreateObject();

  private void SerializeObject(string filename, ObjectType obj)
  {
    // Create an instance of the XmlSerializer class;
    // specify the type of object to serialize.
    var serializer = new QXmlSerializer(typeof(ObjectType));
    using (var textWriter = new StreamWriter(filename))
    {
      XmlWriter writer = XmlWriter.Create(textWriter, new XmlWriterSettings { Indent = true });
      // Serialize the purchase order, and close the TextWriter.
      serializer.Serialize(writer, obj);
      writer.Close();
    }
  }

  protected object? DeserializeObject(string filename)
  {
    // Create an instance of the XmlSerializer class;
    // specify the type of object to be deserialized.
    var serializer = new QXmlSerializer(typeof(ObjectType));
    /* If the XML document has been altered with unknown
    nodes or attributes, handle them with the
    UnknownNode and UnknownAttribute events.*/
    serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
    serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);

    // A FileStream is needed to read the XML document.
    FileStream fs = new FileStream(filename, FileMode.Open);
    /* Use the Deserialize method to restore the object's state with
    data from the XML document. */
    return serializer.Deserialize(fs);
  }

  private static void serializer_UnknownNode
  (object sender, XmlNodeEventArgs e)
  {
    Console.WriteLine("Unknown Node:" + e.Name + "\t" + e.Text);
  }

  private static void serializer_UnknownAttribute
  (object sender, XmlAttributeEventArgs e)
  {
    System.Xml.XmlAttribute attr = e.Attr;
    Console.WriteLine("Unknown attribute " +
    attr.Name + "='" + attr.Value + "'");
  }

  protected abstract void ShowObject(ObjectType obj);
}
