using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using Qhta.Xml.Serialization;

public class Group
{
  [XmlAttribute(Namespace = "http://www.cpandl.com")]
  public string GroupName;

  [XmlAttribute(DataType = "base64Binary")]
  public Byte[] GroupNumber;

  [XmlAttribute(DataType = "date", AttributeName = "CreationDate")]
  public DateTime Today;
}

public static class XmlAttributeAttributeTest
{
  public static void Run()
  {
    SerializeObject("Attributes.xml");
  }

  public static void SerializeObject(string filename)
  {
    // Create an instance of the XmlSerializer class.
    var mySerializer =
      new QXmlSerializer(typeof(Group));

    // Writing the file requires a TextWriter.
    using (TextWriter textWriter = new StreamWriter(filename))
    {
      var xmlWriter = XmlWriter.Create(textWriter, new XmlWriterSettings { Indent = true });
      // Create an instance of the class that will be serialized.
      Group myGroup = new Group();

      // Set the object properties.
      myGroup.GroupName = ".NET";

      Byte[] hexByte = new Byte[2]
      {
      Convert.ToByte(100),
      Convert.ToByte(50)
      };
      myGroup.GroupNumber = hexByte;

      DateTime myDate = new DateTime(2001, 1, 10);
      myGroup.Today = myDate;
      // Serialize the class, and close the TextWriter.
      mySerializer.Serialize(xmlWriter, myGroup);
      xmlWriter.Close();
    }
  }
}