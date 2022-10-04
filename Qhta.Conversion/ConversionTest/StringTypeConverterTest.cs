using System.ComponentModel;
using System.Text;

using Qhta.Conversion;

namespace ConversionTest
{
  public class StringTypeConverterTest
  {
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestNullStringTypeConverter()
    {
      var converter = new StringTypeConverter();
      string? str1 = null;
      var str2 = converter.ConvertTo(str1, typeof(string));
      Assert.That(str2, Is.EqualTo(str1));
      var str3 = converter.ConvertFrom(str2!);
      Assert.That(str3, Is.EqualTo(str2));
    }

    [Test]
    public void TestEmptyStringTypeConverter()
    {
      var converter = new StringTypeConverter();
      string? str1 = string.Empty;
      var str2 = converter.ConvertTo(str1, typeof(string));
      Assert.That(str2, Is.EqualTo(str1));
      if (str2 != null)
      {
        var str3 = converter.ConvertFrom(str2);
        Assert.That(str3, Is.EqualTo(str1));
      }
    }

    [Test]
    public void TestShortStringTypeConverter()
    {
      var converter = new StringTypeConverter();
      string? str1 = "Short string";
      var str2 = converter.ConvertTo(str1, typeof(string));
      Assert.That(str2, Is.EqualTo(str1));
      if (str2 != null)
      {
        var str3 = converter.ConvertFrom(str2);
        Assert.That(str3, Is.EqualTo(str1));
      }
    }

    [Test]
    public void TestLongStringTypeConverter()
    {
      var converter = new StringTypeConverter();
      var sb = new StringBuilder();
      for (int i=0; i<100000; i++)
        sb.Append("Long string ");
      string? str1 = sb.ToString();
      var str2 = converter.ConvertTo(str1, typeof(string));
      Assert.That(str2, Is.EqualTo(str1));
      if (str2 != null)
      {
        var str3 = converter.ConvertFrom(str2);
        Assert.That(str3, Is.EqualTo(str1));
      }
    }

    [Test]
    public void TestAsciiStringTypeConverter()
    {
      var converter = new StringTypeConverter();
      var sb = new StringBuilder();
      for (int i = 0; i < 128; i++)
        sb.Append((char)i);
      string? str1 = sb.ToString();
      var str2 = converter.ConvertTo(str1, typeof(string));
      Assert.That(str2, Is.EqualTo(str1));
      if (str2 != null)
      {
        var str3 = converter.ConvertFrom(str2);
        Assert.That(str3, Is.EqualTo(str1));
      }
    }

    [Test]
    public void TestEscapeSequencesStringTypeConverter()
    {
      var converter = new StringTypeConverter { UseEscapeSequences = true };
      var sb = new StringBuilder();
      for (int i = 0; i < 0xFFFF; i++)
        sb.Append((char)i);
      string? str1 = sb.ToString();
      var str2 = converter.ConvertTo(str1, typeof(string)) as string;
      if (str2 != null)
      {
        foreach (var ch in str2)
          Assert.IsTrue(!char.IsControl(ch), $"Invalid character \\u{((UInt16)ch):X4} in output");
      }
      if (str2 != null)
      {
        var str3 = converter.ConvertFrom(str2);
        Assert.That(str3, Is.EqualTo(str1));
      }
    }

    [Test]
    public void TestHtmlEntitiesStringTypeConverter()
    {
      var converter = new StringTypeConverter { UseHtmlEntities = true };
      var sb = new StringBuilder();
      for (int i = 0; i < 0xFFFF; i++)
        sb.Append((char)i);
      string? str1 = sb.ToString();
      var str2 = converter.ConvertTo(str1, typeof(string)) as string;
      if (str2 != null)
      {
        foreach (var ch in str2)
          Assert.IsTrue(!char.IsControl(ch), $"Invalid character \\u{((UInt16)ch):X4} in output");
      }
      if (str2 != null)
      {
        var str3 = converter.ConvertFrom(str2);
        Assert.That(str3, Is.EqualTo(str1));
      }
    }
  }
}