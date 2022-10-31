using System.ComponentModel;
using System.Diagnostics;
using System.Text;

using NUnit.Framework;

using Qhta.Conversion;

namespace ConversionTest;

public class GuidTypeConverterTest
{
  [SetUp]
  public void Setup()
  {
  }

  [Test]
  public void TestNullGuidTypeConverter()
  {
    var converter = new GuidTypeConverter();
    Guid? value = null;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.Null);
    var value2 = converter.ConvertFrom(str!);
    Assert.That(value2, Is.EqualTo(value));
  }

  [Test]
  public void TestEmptyGuidTypeConverter()
  {
    var converter = new GuidTypeConverter();
    var value2 = converter.ConvertFrom("");
    Assert.That(value2, Is.EqualTo(null));
  }

  [Test]
  public void TestValidGuidTypeConverter()
  {
    var converter = new GuidTypeConverter();
    Guid value = Guid.NewGuid();
    var str0 = value.ToString();
    var str = converter.ConvertTo(value, typeof(string)) as string;
    Assert.That(str, Is.EqualTo(str0));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestFormattedGuidTypeConverter()
  {
    var converter = new GuidTypeConverter();
    Guid value = Guid.NewGuid();
    var formats = new string[] { "N","D","B","P","X"};
    foreach (var format in formats)
    {
      var str0 = value.ToString(format);
      converter.Format = format;
      var str = converter.ConvertTo(value, typeof(string)) as string;
      Assert.That(str, Is.EqualTo(str0));
      TestContext.Out.WriteLine(str0);
      if (str != null)
      {
        var value2 = converter.ConvertFrom(str);
        Assert.That(value2, Is.EqualTo(value));
      }
    }
  }


}
