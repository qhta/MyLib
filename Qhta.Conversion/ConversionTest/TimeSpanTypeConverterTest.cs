using System.Diagnostics;
using System.Globalization;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;

using NUnit.Framework;

using Qhta.Conversion;

namespace ConversionTest;

public class TimeSpanTypeConverterTest
{
  [SetUp]
  public void Setup()
  {
  }

  [Test]
  public void TestNullTimeSpanTypeConverter()
  {
    var converter = new TimeSpanTypeConverter();
    object? value = null;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.Null);
    var value2 = converter.ConvertFrom(str!);
    Assert.That(value2, Is.EqualTo(value));
  }

  [Test]
  public void TestEmptyTimeSpanTypeConverter()
  {
    var converter = new TimeSpanTypeConverter();
    var value2 = converter.ConvertFrom("");
    Assert.That(value2, Is.EqualTo(null));
  }

  [Test]
  public void TestDefaultFormatTimeSpanTypeConverter()
  {
    var converter = new TimeSpanTypeConverter();
    var now = DateTime.Now;
    var value = now.TimeOfDay;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString()));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestShortenFormatTimeSpanTypeConverter()
  {
    var converter = new TimeSpanTypeConverter();
    var now = DateTime.Now;
    var value = now.TimeOfDay;
    value = new TimeSpan(value.Hours, value.Minutes, value.Seconds);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString(@"hh\:mm\:ss")));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestShortFormatTimeSpanTypeConverter()
  {
    var converter = new TimeSpanTypeConverter{ Format = @"hh\:mm" };
    var now = DateTime.Now;
    var value = now.TimeOfDay;
    value = new TimeSpan(value.Hours, value.Minutes, 0);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString(@"hh\:mm")));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

}
