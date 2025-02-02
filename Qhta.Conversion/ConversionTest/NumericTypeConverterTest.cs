﻿using System.Diagnostics;
using System.Globalization;
using System.Net.WebSockets;
using System.Text;

using NUnit.Framework;

using Qhta.Conversion;

namespace ConversionTest;

public class NumericTypeConverterTest
{
  [SetUp]
  public void Setup()
  {
  }

  [Test]
  public void TestNullNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    object? value = null;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.Null);
    var value2 = converter.ConvertFrom(str!);
    Assert.That(value2, Is.EqualTo(value));
  }

  [Test]
  public void TestEmptyNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    var value2 = converter.ConvertFrom("");
    Assert.That(value2, Is.EqualTo(null));
  }

  [Test]
  public void TestZeroNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    object value = 0;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo("0"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestOneNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    object value = 1;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo("1"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestNegativeOneNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    object value = -1;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo("-1"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestOneAndHalfNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    object value = 1.5;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo("1.5"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestNegativeOneAndHalfNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    object value = -1.5;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo("-1.5"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestFloatNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    object value = 1.5E32;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo("1.5E+32"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestNegativeFloatNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    object value = -1.5E32;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo("-1.5E+32"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestMaxIntNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    object value = int.MaxValue;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString()));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestMinIntNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    object value = int.MinValue;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString()));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestMaxInt64NumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    object value = Int64.MaxValue;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString()));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestMinInt64NumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    object value = Int64.MinValue;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString()));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestMaxInt16NumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    object value = Int16.MaxValue;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString()));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestMinInt16NumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    object value = Int16.MinValue;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString()));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestMaxSByteNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    object value = SByte.MaxValue;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString()));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestMinSByteNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    object value = SByte.MinValue;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString()));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestMaxUInt64NumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    object value = UInt64.MaxValue;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString()));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestMaxUInt32NumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    object value = UInt32.MaxValue;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString()));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestMaxUInt16NumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    object value = UInt16.MaxValue;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString()));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestMaxByteNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    object value = Byte.MaxValue;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString()));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestMaxDecimalNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    object value = Decimal.MaxValue;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString()));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestMinDecimalNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    object value = Decimal.MinValue;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString()));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestMaxDoubleNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    var value = Double.MaxValue;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString(CultureInfo.InvariantCulture)));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestMinDoubleNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    var value = Double.MinValue;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString(CultureInfo.InvariantCulture)));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestMaxFloatNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    var value = float.MaxValue;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString(CultureInfo.InvariantCulture)));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      if (value2 is double dd)
        Assert.That((float)dd, Is.EqualTo(value));
      else
        Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestMinFloatNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    var value = float.MinValue;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString(CultureInfo.InvariantCulture)));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      if (value2 is double dd)
        Assert.That((float)dd, Is.EqualTo(value));
      else
        Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestDoubleEpsilonNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    var value = Double.Epsilon;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString(CultureInfo.InvariantCulture)));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestDoubleNegativeEpsilonNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    var value = -Double.Epsilon;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString(CultureInfo.InvariantCulture)));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestFloatEpsilonNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    var value = float.Epsilon;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString(CultureInfo.InvariantCulture)));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      if (value2 is double dd)
        Assert.That((float)dd, Is.EqualTo(value));
      else
        Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestFloatNegativeEpsilonNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    var value = -float.Epsilon;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString(CultureInfo.InvariantCulture)));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      if (value2 is double dd)
        Assert.That((float)dd, Is.EqualTo(value));
      else
        Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestDoublePositiveInfinityNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    var value = Double.PositiveInfinity;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString(CultureInfo.InvariantCulture)));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestDoubleNegativeInfinityNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    var value = Double.NegativeInfinity;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString(CultureInfo.InvariantCulture)));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestDoubleNanNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    var value = Double.NaN;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString(CultureInfo.InvariantCulture)));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestFloatPositiveInfinityNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    var value = float.PositiveInfinity;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString(CultureInfo.InvariantCulture)));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      if (value2 is double dd)
        Assert.That((float)dd, Is.EqualTo(value));
      else
        Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestFloatNegativeInfinityNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    var value = float.NegativeInfinity;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString(CultureInfo.InvariantCulture)));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      if (value2 is double dd)
        Assert.That((float)dd, Is.EqualTo(value));
      else
        Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestFloatNaNNumericTypeConverter()
  {
    var converter = new NumericTypeConverter();
    var value = float.NaN;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString(CultureInfo.InvariantCulture)));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      if (value2 is double dd)
        Assert.That((float)dd, Is.EqualTo(value));
      else
        Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestHexFormatNumericTypeConverter()
  {
    var converter = new NumericTypeConverter{Format = "X"};
    var value = 0x1234567890ABCDEF;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo("1234567890ABCDEF"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestHexSmallFormatNumericTypeConverter()
  {
    var converter = new NumericTypeConverter { Format = "x" };
    var value = 0x1234567890ABCDEF;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo("1234567890abcdef"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestExpectedTypeNumericTypeConverter()
  {
    var converter = new NumericTypeConverter{ ExpectedType = typeof(byte) };
    var value = 255;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString()));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
      Assert.That(value2, Is.TypeOf<byte>());
    }
  }

  [Test]
  public void TestExpectedTypeOverflowNumericTypeConverter()
  {
    Assert.Throws(typeof(OverflowException), () =>
    {
      var converter = new NumericTypeConverter { ExpectedType = typeof(byte) };
      var value = 256;
      var str = converter.ConvertTo(value, typeof(string));
      Assert.That(str, Is.EqualTo(value.ToString()));
      if (str != null)
      {
        var value2 = converter.ConvertFrom(str);
        Assert.That(value2, Is.EqualTo(value));
        Assert.That(value2, Is.TypeOf<byte>());
      }
    });
  }

  [Test]
  public void TestLeadingSpacesNumericTypeConverter()
  {
    var converter = new NumericTypeConverter { NumberStyle = NumberStyles.AllowLeadingWhite };
    var value = 123;
    var str = converter.ConvertTo(value, typeof(string)) as string;
    Assert.That(str, Is.EqualTo("123"));
    if (str != null)
    {
      str = "   "+str;
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestTrailingSpacesNumericTypeConverter()
  {
    var converter = new NumericTypeConverter { NumberStyle = NumberStyles.AllowTrailingWhite };
    var value = 123;
    var str = converter.ConvertTo(value, typeof(string)) as string;
    Assert.That(str, Is.EqualTo("123"));
    if (str != null)
    {
      str = str + "   ";
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestGroupingThousandsNumericTypeConverter()
  {
    var converter = new NumericTypeConverter { Format="###,###,###", NumberStyle = NumberStyles.AllowThousands };
    var value = 123456789;
    var str = converter.ConvertTo(value, typeof(string)) as string;
    Assert.That(str, Is.EqualTo("123,456,789"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }


  [Test]
  public void TestTotalDigitsNumericTypeConverter()
  {
    var converter = new NumericTypeConverter { TotalDigits=5 };
    var value = 123;
    var str = converter.ConvertTo(value, typeof(string)) as string;
    Assert.That(str, Is.EqualTo("00123"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestFractionDigitsNumericTypeConverter()
  {
    var converter = new NumericTypeConverter { FractionDigits = 4 };
    var value = 123.45;
    var str = converter.ConvertTo(value, typeof(string)) as string;
    Assert.That(str, Is.EqualTo("123.4500"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestMaxExclusiveNumericTypeConverter()
  {
    var converter = new NumericTypeConverter { MaxExclusive = 123 };
    var value = 124;
    var str = converter.ConvertTo(value, typeof(string)) as string;
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
    Assert.Throws(typeof(InvalidDataException), () =>
    {
      value = 123;
      str = converter.ConvertTo(value, typeof(string)) as string;
      if (str != null)
      {
        var value2 = converter.ConvertFrom(str);
        Assert.That(value2, Is.EqualTo(value));
      }
    });
  }

  [Test]
  public void TestMinInclusiveNumericTypeConverter()
  {
    var converter = new NumericTypeConverter { MinInclusive = 124 };
    var value = 124;
    var str = converter.ConvertTo(value, typeof(string)) as string;
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
    Assert.Throws(typeof(InvalidDataException), () =>
    {
      value = 123;
      str = converter.ConvertTo(value, typeof(string)) as string;
      if (str != null)
      {
        var value2 = converter.ConvertFrom(str);
        Assert.That(value2, Is.EqualTo(value));
      }
    });
  }


  [Test]
  public void TestMaxInclusiveNumericTypeConverter()
  {
    var converter = new NumericTypeConverter { MaxInclusive = 123 };
    var value = 123;
    var str = converter.ConvertTo(value, typeof(string)) as string;
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
    Assert.Throws(typeof(InvalidDataException), () =>
    {
      value = 124;
      str = converter.ConvertTo(value, typeof(string)) as string;
      if (str != null)
      {
        var value2 = converter.ConvertFrom(str);
        Assert.That(value2, Is.EqualTo(value));
      }
    });
  }

  [Test]
  public void TestMinExclusiveNumericTypeConverter()
  {
    var converter = new NumericTypeConverter { MinExclusive = 124 };
    var value = 123;
    var str = converter.ConvertTo(value, typeof(string)) as string;
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
    Assert.Throws(typeof(InvalidDataException), () =>
    {
      value = 124;
      str = converter.ConvertTo(value, typeof(string)) as string;
      if (str != null)
      {
        var value2 = converter.ConvertFrom(str);
        Assert.That(value2, Is.EqualTo(value));
      }
    });
  }
}
