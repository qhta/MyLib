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
    public void TestTooLongStringTypeConverter()
    {
      var converter = new StringTypeConverter();
      string? str1 = "12345678901234567890";
      converter.MaxLength = 10;
      var str2 = converter.ConvertTo(str1, typeof(string)) as string;
      Assert.That(str2, Is.EqualTo("1234567890"));
      if (str2 != null)
      {
        var str3 = converter.ConvertFrom(str1) as string;
        Assert.That(str3, Is.EqualTo("1234567890"));
      }
    }

    [Test]
    public void TestTooShortStringTypeConverter()
    {
      var converter = new StringTypeConverter();
      string? str1 = "1234567890";
      converter.MinLength = 20;
      var str2 = converter.ConvertTo(str1, typeof(string)) as string;
      Assert.That(str2, Is.EqualTo("1234567890          "));
      if (str2 != null)
      {
        var str3 = converter.ConvertFrom(str1) as string;
        Assert.That(str3, Is.EqualTo("1234567890          "));
      }
    }

    [Test]
    public void TestCharStringTypeConverter()
    {
      var converter = new StringTypeConverter { ExpectedType = typeof(char) };
      var sb = new StringBuilder();
      string? str1 = "char";
      var str2 = converter.ConvertTo(str1, typeof(string));
      Assert.That(str2, Is.EqualTo(str1));
      if (str2 != null)
      {
        var str3 = converter.ConvertFrom(str2);
        Assert.That(str3, Is.TypeOf<Char>());
        Assert.That(str3, Is.EqualTo('c'));
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

    [Test]
    public void TestWhitespacesReplaceStringTypeConverter()
    {
      var converter = new StringTypeConverter { Whitespaces = WhitespaceBehavior.Replace };
      string? str1 = " a b \t c \r\n d \xA0 e ";
      var str0 = " a b   c    d   e ";
      var str2 = converter.ConvertTo(str1, typeof(string)) as string;
      Assert.That(str2, Is.EqualTo(str0));
      if (str2 != null)
      {
        var str3 = converter.ConvertFrom(str1);
        Assert.That(str3, Is.EqualTo(str2));
      }
    }

    [Test]
    public void TestWhitespacesCollapseStringTypeConverter()
    {
      var converter = new StringTypeConverter { Whitespaces = WhitespaceBehavior.Collapse };
      string? str1 = " a b \t c \r\n d \xA0 e ";
      var str0 = "a b c d e";
      var str2 = converter.ConvertTo(str1, typeof(string)) as string;
      Assert.That(str2, Is.EqualTo(str0));
      if (str2 != null)
      {
        var str3 = converter.ConvertFrom(str1);
        Assert.That(str3, Is.EqualTo(str2));
      }
    }

    [Test]
    public void TestNormalizedStringStringTypeConverter()
    {
      var converter = new StringTypeConverter { XsdType = XsdSimpleType.NormalizedString };
      string? str1 = " a b \t c \r\n d \xA0 e ";
      var str0 = " a b   c    d   e ";
      var str2 = converter.ConvertTo(str1, typeof(string)) as string;
      Assert.That(str2, Is.EqualTo(str0));
      if (str2 != null)
      {
        var str3 = converter.ConvertFrom(str1);
        Assert.That(str3, Is.EqualTo(str2));
      }
    }


    [Test]
    public void TestTokenStringTypeConverter()
    {
      var converter = new StringTypeConverter { XsdType = XsdSimpleType.Token };
      string? str1 = "  Being a Dog Is \r\n  a Full-Time Job";
      var str0 = "Being a Dog Is a Full-Time Job";
      var str2 = converter.ConvertTo(str1, typeof(string)) as string;
      Assert.That(str2, Is.EqualTo(str0));
      if (str2 != null)
      {
        var str3 = converter.ConvertFrom(str1);
        Assert.That(str3, Is.EqualTo(str2));
      }
    }

    [Test]
    public void TestNotationStringTypeConverter()
    {
      var converter = new StringTypeConverter { XsdType = XsdSimpleType.Notation };
      Assert.That(converter.WhitespacesFixed, Is.True);
      string? str1 = " a b \t c \r\n d \xA0 e ";
      var str0 = "a b c d e";
      var str2 = converter.ConvertTo(str1, typeof(string)) as string;
      Assert.That(str2, Is.EqualTo(str0));
      if (str2 != null)
      {
        var str3 = converter.ConvertFrom(str1);
        Assert.That(str3, Is.EqualTo(str2));
      }
    }

    [Test]
    public void TestNmTokenStringTypeConverter()
    {
      var converter = new StringTypeConverter { XsdType = XsdSimpleType.NmToken };
      var validStrings = new string[]
      {
        "ABC", "Abc", "A12", "123", "1950-10-04",
      };
      foreach (var str in validStrings)
        SubTestValidString(str, converter);
      var invalidStrings = new string[]
      {
        "A B", "A,B"
      };
      foreach (var str in invalidStrings)
        SubTestInvalidString(str, converter);
    }

    [Test]
    public void TestNameStringTypeConverter()
    {
      var converter = new StringTypeConverter { XsdType = XsdSimpleType.Name };
      var validStrings = new string[]
      {
        "ABC", "Abc", "A12", "A_B", "_12", "A.B", "A-B", "A:B", ":AB"
      };
      foreach (var str in validStrings)
        SubTestValidString(str, converter);
      var invalidStrings = new string[]
      {
        "A B", "A,B", "123", "1950-10-04",".B", "-AB"
      };
      foreach (var str in invalidStrings)
        SubTestInvalidString(str, converter);
    }

    [Test]
    public void TestNCNameStringTypeConverter()
    {
      var converter = new StringTypeConverter { XsdType = XsdSimpleType.NcName };
      var validStrings = new string[]
      {
        "ABC", "Abc", "A12", "A_B", "_12", "A.B", "A-B"
      };
      foreach (var str in validStrings)
        SubTestValidString(str, converter);
      var invalidStrings = new string[]
      {
        "A B", "A,B", "123", "1950-10-04",".B", "-AB", "A:B", ":AB"
      };
      foreach (var str in invalidStrings)
        SubTestInvalidString(str, converter);
    }


    [Test]
    public void TestLanguageStringTypeConverter()
    {
      var converter = new StringTypeConverter { XsdType = XsdSimpleType.Language };
      var validStrings = new string[]
      {
        "en", "en-US", "fr", "fr-FR"
      };
      foreach (var str in validStrings)
        SubTestValidString(str, converter);
    }

    [Test]
    public void TestEnumerationsStringTypeConverter()
    {
      var converter = new StringTypeConverter { CaseInsensitive = true };
      var enumerations = new string[]
      {
        "ABC", "DEF", "GHI"
      };
      converter.Enumerations = enumerations;
      var validStrings = new string[]
      {
        "ABC", "Abc", "abc", "GHI", 
      };
      foreach (var str in validStrings)
        SubTestValidString(str, converter);
      var invalidStrings = new string[]
      {
        "GHL"
      };
      foreach (var str in invalidStrings)
        SubTestInvalidString(str, converter);
    }

    private void SubTestValidString(string str1, StringTypeConverter converter)
    {
      var str2 = converter.ConvertTo(str1, typeof(string)) as string;
      Assert.That(str2, Is.EqualTo(str1));
      if (str2 != null)
      {
        var str3 = converter.ConvertFrom(str1);
        Assert.That(str3, Is.EqualTo(str2));
      }
    }
    private void SubTestInvalidString(string str1, StringTypeConverter converter)
    {
      Assert.Throws(typeof(InvalidOperationException), () =>
      {
        var str2 = converter.ConvertTo(str1, typeof(string)) as string;
        Assert.That(str2, Is.EqualTo(str1));
        if (str2 != null)
        {
          var str3 = converter.ConvertFrom(str1);
          Assert.That(str3, Is.EqualTo(str2));
        }
      });
    }
  }
}