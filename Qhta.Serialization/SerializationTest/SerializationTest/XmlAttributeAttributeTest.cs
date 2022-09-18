using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using Qhta.Xml.Serialization;
using Qhta.TestHelper;

namespace TestData
{
  public class Group
  {
    [XmlAttribute(Namespace = "http://www.cpandl.com")]
    public string GroupName;

    [XmlAttribute(DataType = "base64Binary")]
    public Byte[] GroupNumber;

    [XmlAttribute(DataType = "date", AttributeName = "CreationDate")]
    public DateTime Today;
  }
}

namespace SerializationTest
{
  using TestData;

  public class XmlAttributeAttributeTest : SerializerTest<Group>
  {

    public override bool Run()
    {
      return base.Run("Attributes.xml");
    }

    protected override Group CreateObject()
    {
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
      return myGroup;
    }

    protected override void ShowObject(Group obj)
    {
    }
  }
}
