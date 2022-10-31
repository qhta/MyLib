using System.Diagnostics;
using System.Globalization;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

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

  [Test]
  public void TestGenericFormatTimeSpanTypeConverter()
  {
    var converter = new TimeSpanTypeConverter { Format = "G" };
    var now = DateTime.Now;
    var value = now-new DateTime(2000,1,1);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString("G", CultureInfo.InvariantCulture)));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestDurationFormatTimeSpanTypeConverter()
  {
    var converter = new TimeSpanTypeConverter { Format = "D" };
    var now = DateTime.Now;
    var value = now - new DateTime(2000, 1, 1);
    value = new TimeSpan(value.Days, value.Hours, value.Minutes, value.Seconds, value.Milliseconds);
    var str = converter.ConvertTo(value, typeof(string));
    var str2 = value.ToString("G", CultureInfo.InvariantCulture);
    var chars = str2.ToArray();
    string replacements = "DHMS";
    int k = 0;
    for (int i = 0; i < chars.Length; i++)
    {
      if (chars[i] == ':')
        chars[i] = replacements[k++];
    }
    k = str2.IndexOf('.');
    if (k < 0)
      k = chars.Length;
    str2 = new string(chars[0..k]);
    str2=str2.Replace("D0", "D");
    str2=str2.Replace("H0", "H");
    str2 = str2.Replace("M0", "M");
    var ms = value.Milliseconds;
    string mstr = "";
    if (ms != 0)
      mstr = "." + ms.ToString("D3");
    str2 = "P"+str2+mstr+'S';
    Assert.That(str, Is.EqualTo(str2));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }
}
