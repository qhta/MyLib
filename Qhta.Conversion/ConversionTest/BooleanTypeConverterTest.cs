using System.Diagnostics;
using System.Text;

using NUnit.Framework;

using Qhta.Conversion;

namespace ConversionTest;

public class BooleanTypeConverterTest
{
  [SetUp]
  public void Setup()
  {
  }

  [Test]
  public void TestNullBooleanTypeConverter()
  {
    var converter = new BooleanTypeConverter();
    Boolean? value = null;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.Null);
    var value2 = converter.ConvertFrom(str!);
    Assert.That(value2, Is.EqualTo(value));
  }

  [Test]
  public void TestEmptyBooleanTypeConverter()
  {
    var converter = new BooleanTypeConverter();
    var value2 = converter.ConvertFrom("");
    Assert.That(value2, Is.EqualTo(null));
  }

  [Test]
  public void TestTrueBooleanTypeConverter()
  {
    var converter = new BooleanTypeConverter();
    Boolean value = true;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo("true"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestFalseBooleanTypeConverter()
  {
    var converter = new BooleanTypeConverter();
    Boolean value = false;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo("false"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestOneBooleanTypeConverter()
  {
    var converter = new BooleanTypeConverter{ Mode = BooleanConversionMode.Numeric};
    Boolean value = true;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo("1"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestZeroBooleanTypeConverter()
  {
    var converter = new BooleanTypeConverter { Mode = BooleanConversionMode.Numeric };
    Boolean value = false;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo("0"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestOnBooleanTypeConverter()
  {
    var converter = new BooleanTypeConverter { Mode = BooleanConversionMode.OnOff };
    Boolean value = true;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo("on"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestOffBooleanTypeConverter()
  {
    var converter = new BooleanTypeConverter { Mode = BooleanConversionMode.OnOff };
    Boolean value = false;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo("off"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestWłBooleanTypeConverter()
  {
    var converter = new BooleanTypeConverter { XsdType = XsdSimpleType.String };
    converter.Enumerations = new string[] { "true", "false", "1", "0", "wł", "wył" };
    Boolean value = true;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo("wł"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom("WŁ");
      Assert.That(value2, Is.EqualTo(value));
    }
  }

}
