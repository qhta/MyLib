using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using Qhta.Xml.Serialization;
using Qhta.TestHelper;
using System.Xml.Linq;


/// <summary>
/// This is a base class for writing serializer tests.
/// It can test a standard XmlSerializer or new QXmlSerializer.
/// To choose the standard XmlSerializer set a property <see cref="UseStdSerializer"/> to true.
/// As a generic type, it needs a type of root object to create test object and initialize the serializer.
/// You must implement abstract <see cref="CreateObject"/> and <see cref="Run"/> methods in derived class.
/// You may implement an empty virtual <see cref="ShowObject"/> method by your own to see deserialization result.
/// You may change implementation of all virtual protected methods.
/// </summary>
/// <typeparam name="ObjectType">Root object type</typeparam>
public abstract class SerializerTest<ObjectType>
{

  /// <summary>
  ///  An instance of <see cref="TraceTextWriter"/> class.
  ///  It will be used to monitor serialization and deserialization errors,
  ///  comparing errors and comparison differences on console and in the Output Window.
  /// </summary>
  protected readonly TraceTextWriter TraceWriter = new TraceTextWriter(true, true);

  /// <summary>
  /// Change this property to true to choose standard XmlSerializer test.
  /// It controls <see cref="SerializeObject"/> and <see cref="DeserializeObject"/> methods.
  /// </summary>
  protected bool UseStdSerializer { get; set; }

  /// <summary>
  /// Change this property to true to serialize each attribute in separate line.
  /// </summary>
  protected bool NewLineOnAttributes { get; set; }

  /// <summary>
  /// Change this property to true to display result of serialization.
  /// </summary>
  protected bool ShowOutputXml { get; set; }

  /// <summary>
  /// Implement this method to create a test object before serialization.
  /// </summary>
  /// <returns></returns>
  protected abstract ObjectType CreateObject();

  /// <summary>
  /// Implement this method to run a test.
  /// Usually you invoke <see cref="RunOnFile"/> method with a filename.
  /// </summary>
  /// <returns>Result of the test: true - passed, false - failed, null - not run</returns>
  public abstract bool? Run();

  /// <summary>
  /// Implement this method by your own if you want to see the deserialized object.
  /// </summary>
  /// <param name="obj">Deserialized object to show</param>
  protected virtual void ShowObject(ObjectType obj)
  {
  }

  /// <summary>
  /// This method runs a test using a file with specified filename. Test consists of few steps:
  /// <list type="number">
  ///   <item>
  ///     A filename for a check file is prepared by preceding the <see cref="filename"/> param extension with ".OK",
  ///     e.g. "Test.xml" filename generates "Test.OK.xml" check file name.
  ///   </item>
  ///   <item>A test object is created using <see cref="CreateObject"/> method.</item>
  ///   <item>The test object is serialized using <see cref="SerializeObject"/> method.</item>
  ///   <item>
  ///     If the check file exists, then it is compared with a newly serialized file using <see cref="XmlFileComparer"/> class.
  ///     If files are different, a message is shown, but test continues.
  ///   </item>
  ///   <item>A newly serialized file is deserialized using <see cref="DeserializeObject"/> method.</item>
  ///   <item>
  ///      Deserialized object is compared with previously created test object using <see cref="ObjectComparer"/> class.
  ///   </item>
  /// </list>
  /// </summary>
  /// <param name="filename">A filename for test. If no path specified, it will be created in a default (usually "debug") folder of the solution.</param>
  /// <returns>True if test pass, false if test fails. This implementation does not return null, but overriden implementations can retun null.</returns>
  public virtual bool? RunOnFile(String filename)
  {
    try
    {
      var checkFilename = filename.Insert(filename.LastIndexOf('.'), ".OK");
      var obj = CreateObject();
      bool serializeOK = false;
      bool? deserializeOK = null;
      SerializeObject(filename, obj);
      if (File.Exists(filename))
      {
        if (File.Exists(checkFilename))
        {
          var xmlFileComparer = new XmlFileComparer(new FileCompareOptions(), TraceWriter);
          serializeOK = xmlFileComparer.CompareFiles(filename, checkFilename);
          if (serializeOK)
            TraceWriter.WriteLine("Serialization passed");
          else
            TraceWriter.WriteLine("Serialization difference");
        }
        else
        {
          TraceWriter.WriteLine("Serialization passed");
          serializeOK = true;
        }
        if (serializeOK && ShowOutputXml)
        {
          TraceWriter.ForegroundColor = ConsoleColor.Yellow;
          using (var reader = File.OpenText(filename))
          {
            do
            {
              var line = reader.ReadLine();
              if (line == null) break;
              TraceWriter.WriteLine(line);
            } while (true);
          }
          TraceWriter.ForegroundColor = ConsoleColor.White;
        }
        var temp = DeserializeObject(filename);
        if (temp is ObjectType obj2)
        {
          ShowObject(obj2);
          var objectComparer = new ObjectComparer(TraceWriter);
          if (objectComparer.DeepCompare(obj, obj2))
          {
            deserializeOK = true;
            TraceWriter.WriteLine("Deserialization passed");
          }
          else
          {
            deserializeOK = false;
            TraceWriter.WriteLine("Deserialization difference");
          }
        }
        else
          TraceWriter.WriteLine("Deserialization failed");
      }
      else
        TraceWriter.WriteLine("Serialization failed");
      return serializeOK && (deserializeOK ?? true);
    }
    catch (Exception ex)
    {
      TraceWriter.WriteLine($"{ex.GetType().Name}:");
      TraceWriter.WriteLine($"  {ex.Message}:");
      while (ex.InnerException != null)
      {
        ex = ex.InnerException;
        TraceWriter.WriteLine($"  {ex.Message}:");
      }
      return false;
    }
  }

  /// <summary>
  /// A switch method to serialize the test object in a file.
  /// Controlled by <see cref="UseStdSerializer"/> property.
  /// Invokes either <see cref="SerializeObjectWithStdSerializer"/> or <see cref="SerializeObjectWithNewSerializer"/> method.
  /// </summary>
  /// <param name="filename">A name of file for serialization output</param>
  /// <param name="obj">An object to serialize</param>
  protected virtual void SerializeObject(string filename, ObjectType obj)
  {
    if (UseStdSerializer)
      SerializeObjectWithStdSerializer(filename, obj);
    else
      SerializeObjectWithNewSerializer(filename, obj);
  }

  /// <summary>
  /// A method for serialization with standard XmlSerializer.
  /// Uses an instance of <see cref="XmlWriter"/> class with option <see cref="XmlWriterSettings.Indent"/>
  /// set to true and <see cref="XmlWriterSettings.NamespaceHandling"/> set to <see cref="NamespaceHandling.OmitDuplicates"/>.
  /// </summary>
  /// <param name="filename">A name of file for serialization output</param>
  /// <param name="obj">An object to serialize</param>
  protected virtual void SerializeObjectWithStdSerializer(string filename, ObjectType obj)
  {
    var serializer = new XmlSerializer(typeof(ObjectType));
    using (var textWriter = new StreamWriter(filename))
    {
      XmlWriter writer = XmlWriter.Create(textWriter,
        new XmlWriterSettings
        {
          Indent = true,
          NamespaceHandling = NamespaceHandling.OmitDuplicates,
          NewLineOnAttributes = NewLineOnAttributes,
        });
      serializer.Serialize(writer, obj);
      writer.Close();
    }
  }

  /// <summary>
  /// A method for serialization with new QXmlSerializer.
  /// Uses an instance of <see cref="XmlWriter"/> class with option <see cref="XmlWriterSettings.Indent"/>
  /// set to true and <see cref="XmlWriterSettings.NamespaceHandling"/> set to <see cref="NamespaceHandling.OmitDuplicates"/>.
  /// </summary>
  /// <param name="filename">A name of file for serialization output</param>
  /// <param name="obj">An object to serialize</param>
  protected virtual void SerializeObjectWithNewSerializer(string filename, ObjectType obj)
  {
    var serializer = new QXmlSerializer(typeof(ObjectType));
    using (var textWriter = new StreamWriter(filename))
    {
      XmlWriter writer = XmlWriter.Create(textWriter,
        new XmlWriterSettings
        {
          Indent = true,
          NamespaceHandling = NamespaceHandling.OmitDuplicates,
          NewLineOnAttributes = NewLineOnAttributes,
        });
      serializer.Serialize(writer, obj);
      writer.Close();
    }
  }

  /// <summary>
  /// A switch method to deserialize the test object from a file.
  /// Controlled by <see cref="UseStdSerializer"/> property.
  /// Invokes either <see cref="DeserializeObjectWithStdSerializer"/> or <see cref="DeserializeObjectWithNewSerializer"/> method.
  /// </summary>
  /// <param name="filename">A name of file for deserialization input</param>
  /// <returns>Deserialized object</returns>
  protected virtual object? DeserializeObject(string filename)
  {
    if (UseStdSerializer)
      return DeserializeObjectWithStdSerializer(filename);
    else
      return DeserializeObjectWithNewSerializer(filename);
  }

  /// <summary>
  /// A method for deserialization with standard XmlSerializer.
  /// It handles serializer UnknownNode, UnknownAttrtibute, 
  /// </summary>
  /// <param name="filename">A name of file for deserialization input</param>
  /// <returns>Deserialized object</returns>
  protected virtual object? DeserializeObjectWithStdSerializer(string filename)
  {
    var serializer = new XmlSerializer(typeof(ObjectType));
    serializer.UnknownNode += Serializer_UnknownNode;
    serializer.UnknownAttribute += Serializer_UnknownAttribute;
    serializer.UnknownElement += Serializer_UnknownElement;
    serializer.UnreferencedObject += Serializer_UnreferencedObject;

    FileStream fs = new FileStream(filename, FileMode.Open);
    return serializer.Deserialize(fs);
  }

  protected virtual object? DeserializeObjectWithNewSerializer(string filename)
  {
    var serializer = new QXmlSerializer(typeof(ObjectType));
    serializer.UnknownNode += Serializer_UnknownNode;
    serializer.UnknownAttribute += Serializer_UnknownAttribute;
    serializer.UnknownElement += Serializer_UnknownElement;
    serializer.UnreferencedObject += Serializer_UnreferencedObject;

    FileStream fs = new FileStream(filename, FileMode.Open);
    return serializer.Deserialize(fs);
  }

  protected virtual void Serializer_UnknownNode(object? sender, XmlNodeEventArgs e)
  {
    TraceWriter.WriteLine($"Unknown Node: {e.Name} in line {e.LineNumber} at pos {e.LinePosition}");
    TraceWriter.WriteLine($"  {e.Text}");
  }

  protected virtual void Serializer_UnknownAttribute(object? sender, XmlAttributeEventArgs e)
  {
    System.Xml.XmlAttribute attr = e.Attr;
    TraceWriter.WriteLine($"Unknown attribute {attr.Name} = \"{attr.Value}\" in line {e.LineNumber} at pos {e.LinePosition}");
    TraceWriter.WriteLine("  expected attributes: " + e.ExpectedAttributes);
  }

  private void Serializer_UnknownElement(object? sender, XmlElementEventArgs e)
  {
    System.Xml.XmlElement element = e.Element;
    TraceWriter.WriteLine($"Unknown element \"{element.Name}\" in line {e.LineNumber} at pos {e.LinePosition}");
    TraceWriter.WriteLine("  expected elements: " + e.ExpectedElements);
  }


  private void Serializer_UnreferencedObject(object? sender, UnreferencedObjectEventArgs e)
  {
    TraceWriter.WriteLine($"Unreferenced object \"{e.UnreferencedId}\" ");
  }
}
