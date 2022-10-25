using System.Diagnostics;
using System.Text;
using System.Xml;
using NUnit.Framework;

using Qhta.Conversion;

namespace ConversionTest;

public class XmlQualifiedNameTypeConverterTest
{
  [SetUp]
  public void Setup()
  {
  }

  [Test]
  public void TestNullUriTypeConverter()
  {
    var converter = new XmlQualifiedNameTypeConverter();
    Uri? value = null;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.Null);
    var value2 = converter.ConvertFrom(str!);
    Assert.That(value2, Is.EqualTo(value));
  }

  [Test]
  public void TestEmptyUriTypeConverter()
  {
    var converter = new XmlQualifiedNameTypeConverter();
    var value2 = converter.ConvertFrom("");
    Assert.That(value2, Is.EqualTo(null));
  }

  [Test]
  public void TestQualifiedNameTypeConverter()
  {
    var converter = new XmlQualifiedNameTypeConverter();
    var str0 = "prefix:name";
    var value = new XmlQualifiedName("name", "prefix");
    var str = converter.ConvertTo(value, typeof(string)) as string;
    Assert.That(str, Is.EqualTo(str0));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }


}
