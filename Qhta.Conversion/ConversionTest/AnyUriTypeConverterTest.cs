using System.Diagnostics;
using System.Text;

using NUnit.Framework;

using Qhta.Conversion;

namespace ConversionTest;

public class AnyUriTypeConverterTest
{
  [SetUp]
  public void Setup()
  {
  }

  [Test]
  public void TestNullUriTypeConverter()
  {
    var converter = new AnyUriTypeConverter();
    Uri? value = null;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.Null);
    var value2 = converter.ConvertFrom(str!);
    Assert.That(value2, Is.EqualTo(value));
  }

  [Test]
  public void TestEmptyUriTypeConverter()
  {
    var converter = new AnyUriTypeConverter();
    var value2 = converter.ConvertFrom("");
    Assert.That(value2, Is.EqualTo(null));
  }

  [Test]
  public void TestValidUriTypeConverter()
  {
    var converter = new UriTypeConverter();
    var str0 = "HTTP://www.Contoso.com:80/thick%20and%20thin.htm";
    Uri value = new Uri(str0);
    var str = converter.ConvertTo(value, typeof(string)) as string;
    Assert.That(str, Is.EqualTo(str0));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }


}
