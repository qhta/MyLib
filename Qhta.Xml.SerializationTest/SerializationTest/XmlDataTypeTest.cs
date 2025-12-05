//#define StdSerialization

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using Qhta.Xml.Serialization;
using Qhta.TestHelper;
using System.ComponentModel;

namespace TestData
{
  public class AttributedData
  {
    [XmlElement(DataType = "integer")]
    public string StringAsInteger { get; set; } = string.Empty;

#if !StdSerialization
    [XmlElement(DataType = "integer")]
#endif
    public Int32 IntAsInteger { get; set; }

#if !StdSerialization
    [XmlElement(DataType = "boolean")]
#endif
    public int IntAsBool { get; set; }

#if !StdSerialization
    [XmlElement(DataType = "int")]
#endif
    public bool BoolAsInt { get; set; }

    [XmlElement]
    public DateTime Now { get; set; }

    [XmlElement(DataType = "dateTime")]
    public DateTime DateTime { get; set; }

    [XmlElement(DataType = "date")]
    public DateTime Date { get; set; }

    [XmlElement(DataType = "time")]
    public DateTime Time { get; set; }

#if !StdSerialization
    [XmlElement(DataType = "date")]
#endif
    public DateOnly DateOnly { get; set; }

#if !StdSerialization
    [XmlElement(DataType = "time")]
#endif
    public TimeOnly TimeOnly { get; set; }


    /// <summary>
    /// Original property which must be specially formatted.
    /// </summary>
    [XmlIgnore]
    public DateTime? LastModified
    {
      get; set;
    }

    /// <summary>
    /// The date and time of the last modification.
    /// </summary>
    [XmlElement("LastModified")]
    public string? LastModifiedXml
    {
      get => LastModified?.ToString("yyyy-MM-dd HH:mm:ss");

      set => LastModified = DateTime.Parse(value ?? "");
    }

    [XmlElement(DataType = "base64Binary")]
    public byte[]? Base64Binary { get; set; }

    [XmlElement(DataType = "hexBinary")]
    public byte[]? HexBinary { get; set; }

    [XmlElement(DataType = "boolean")]
    public bool BoolTrue { get; set; }

    [XmlElement(DataType = "boolean")]
    public bool BoolFalse { get; set; }

    [XmlElement(DataType = "float")]
    public float Float { get; set; }

    [XmlElement(DataType = "double")]
    public double Double { get; set; }

    [XmlElement(DataType = "decimal")]
    public decimal Decimal { get; set; }
  }
}

namespace SerializationTest
{
  using TestData;

  public class XmlDataTypeTest : SerializerTest<AttributedData>
  {

    public override bool? Run()
    {
#if StdSerialization
      UseStdSerializer = true;
#endif
      NewLineOnAttributes = true;
      ShowOutputXml = true;
      return base.RunOnFile("XmlDataType.xml");
    }

    protected override AttributedData CreateObject()
    {
      var now = DateTime.Parse("2022-09-21T21:30:11.5338655+02:00");
      var nowShort = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
      var data = new AttributedData
      {
        Now = now,
        DateTime = now,
        Date = now.Date,
        Time = new DateTime(1, 1, 1, now.Hour, now.Minute, now.Second),
        DateOnly = DateOnly.FromDateTime(now),
        TimeOnly = TimeOnly.FromDateTime(now),
        LastModified = nowShort,
        Base64Binary = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 },
        HexBinary = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 },
        BoolTrue = true,
        BoolFalse = false,
        Float = 1.1234567890123456789F,
        Double = 11234567890123456789,
        Decimal = 12345.67890M,
        IntAsBool = 1,
        BoolAsInt = true,
        StringAsInteger = "123456789012345678901234567890",
        IntAsInteger = 1234567890,
      };
      return data;
    }

  }
}
