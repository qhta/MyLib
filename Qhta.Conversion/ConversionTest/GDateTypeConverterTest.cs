using System.Diagnostics;
using System.Globalization;
using System.Net.WebSockets;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

using NUnit.Framework;

using Qhta.Conversion;

namespace ConversionTest;

public class GDateTypeConverterTest
{
  [SetUp]
  public void Setup()
  {
  }

  [Test]
  public void TestNullGDateTypeConverter()
  {
    var converter = new GDateTypeConverter();
    object? value = null;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.Null);
    var value2 = converter.ConvertFrom(str!);
    Assert.That(value2, Is.EqualTo(value));
  }

  [Test]
  public void TestEmptyGDateTypeConverter()
  {
    var converter = new GDateTypeConverter();
    var value2 = converter.ConvertFrom("");
    Assert.That(value2, Is.EqualTo(null));
  }

  [Test]
  public void TestYearOnlyGDateTypeConverter()
  {
    var converter = new GDateTypeConverter();
    var now = DateTime.Now;
    var value = new GDate(now.Year);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"{now.Year}"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestYearMonthGDateTypeConverter()
  {
    var converter = new GDateTypeConverter();
    var now = DateTime.Now;
    var value = new GDate(now.Year, now.Month);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"{now.Year}-{now.Month:D2}"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestYearMonthDayGDateTypeConverter()
  {
    var converter = new GDateTypeConverter();
    var now = DateTime.Now;
    var value = new GDate(now.Year, now.Month, now.Day);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"{now.Year}-{now.Month:D2}-{now.Day:D2}"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestYearMonthDayZoneGDateTypeConverter()
  {
    var converter = new GDateTypeConverter { ShowTimeZone = true };
    var now = DateTime.Now;
    var zone = 2;
    var value = new GDate(now.Year, now.Month, now.Day, zone);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"{now.Year}-{now.Month:D2}-{now.Day:D2}+{zone:D2}"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestYearMonthDayNegZoneGDateTypeConverter()
  {
    var converter = new GDateTypeConverter { ShowTimeZone = true };
    var now = DateTime.Now;
    var zone = 2;
    var value = new GDate(now.Year, now.Month, now.Day, -zone);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"{now.Year}-{now.Month:D2}-{now.Day:D2}-{zone:D2}"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestYearMonthDayZuluGDateTypeConverter()
  {
    var converter = new GDateTypeConverter { ShowTimeZone = true };
    var now = DateTime.Now;
    var value = new GDate(now.Year, now.Month, now.Day, 0);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"{now.Year}-{now.Month:D2}-{now.Day:D2}Z"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestMonthGDateTypeConverter()
  {
    var converter = new GDateTypeConverter();
    var now = DateTime.Now;
    var value = new GDate(0, now.Month);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"--{now.Month:D2}"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestMonthDayGDateTypeConverter()
  {
    var converter = new GDateTypeConverter();
    var now = DateTime.Now;
    var value = new GDate(0, now.Month, now.Day);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"--{now.Month:D2}-{now.Day:D2}"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestMonthDayZoneGDateTypeConverter()
  {
    var converter = new GDateTypeConverter { ShowTimeZone = true };
    var now = DateTime.Now;
    var zone = 2;
    var value = new GDate(0, now.Month, now.Day, zone);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"--{now.Month:D2}-{now.Day:D2}+{zone:D2}"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestMonthDayNegZoneGDateTypeConverter()
  {
    var converter = new GDateTypeConverter { ShowTimeZone = true };
    var now = DateTime.Now;
    var zone = 2;
    var value = new GDate(0, now.Month, now.Day, -zone);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"--{now.Month:D2}-{now.Day:D2}-{zone:D2}"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestMonthDayZuluGDateTypeConverter()
  {
    var converter = new GDateTypeConverter { ShowTimeZone = true };
    var now = DateTime.Now;
    var value = new GDate(0, now.Month, now.Day, 0);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"--{now.Month:D2}-{now.Day:D2}Z"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestDayGDateTypeConverter()
  {
    var converter = new GDateTypeConverter();
    var now = DateTime.Now;
    var value = new GDate(0, 0, now.Day);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"---{now.Day:D2}"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestDayZoneGDateTypeConverter()
  {
    var converter = new GDateTypeConverter { ShowTimeZone = true };
    var now = DateTime.Now;
    var zone = 2;
    var value = new GDate(0, 0, now.Day, zone);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"---{now.Day:D2}+{zone:D2}"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestDayNegZoneGDateTypeConverter()
  {
    var converter = new GDateTypeConverter { ShowTimeZone = true };
    var now = DateTime.Now;
    var zone = 2;
    var value = new GDate(0, 0, now.Day, -zone);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"---{now.Day:D2}-{zone:D2}"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestDayZuluGDateTypeConverter()
  {
    var converter = new GDateTypeConverter { ShowTimeZone = true };
    var now = DateTime.Now;
    var value = new GDate(0, 0, now.Day, 0);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"---{now.Day:D2}Z"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestGYearOnlyGDateTypeConverter()
  {
    var converter = new GDateTypeConverter { XsdType = XsdSimpleType.GYear };
    var now = DateTime.Now;
    var value = new GDate(now.Year, now.Month, now.Day);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"{now.Year}"));
    if (str != null)
    {
      var value1 = new GDate(now.Year);
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value1));
    }
  }

  [Test]
  public void TestGYearMonthGDateTypeConverter()
  {
    var converter = new GDateTypeConverter { XsdType = XsdSimpleType.GYearMonth };
    var now = DateTime.Now;
    var value = new GDate(now.Year, now.Month, now.Day);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"{now.Year}-{now.Month:D2}"));
    if (str != null)
    {
      var value1 = new GDate(now.Year, now.Month);
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value1));
    }
  }

  [Test]
  public void TestGMonthGDateTypeConverter()
  {
    var converter = new GDateTypeConverter { XsdType = XsdSimpleType.GMonth };
    var now = DateTime.Now;
    var value = new GDate(now.Year, now.Month, now.Day);
    ;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"--{now.Month:D2}"));
    if (str != null)
    {
      var value1 = new GDate(0, now.Month);
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value1));
    }
  }

  [Test]
  public void TestGMonthDayGDateTypeConverter()
  {
    var converter = new GDateTypeConverter { XsdType = XsdSimpleType.GMonthDay };
    var now = DateTime.Now;
    var value = new GDate(now.Year, now.Month, now.Day);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"--{now.Month:D2}-{now.Day:D2}"));
    if (str != null)
    {
      var value1 = new GDate(0, now.Month, now.Day);
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value1));
    }
  }

  [Test]
  public void TestGMonthDayZoneGDateTypeConverter()
  {
    var zone = 2;
    var converter = new GDateTypeConverter { XsdType = XsdSimpleType.GMonthDay, ShowTimeZone = true };
    var now = DateTime.Now;
    var value = new GDate(now.Year, now.Month, now.Day, zone);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"--{now.Month:D2}-{now.Day:D2}+{zone:D2}"));
    if (str != null)
    {
      var value1 = new GDate(0, now.Month, now.Day, zone);
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value1));
    }
  }

  [Test]
  public void TestGMonthDayNegZoneGDateTypeConverter()
  {
    var zone = 2;
    var converter = new GDateTypeConverter { XsdType = XsdSimpleType.GMonthDay, ShowTimeZone = true };
    var now = DateTime.Now;
    var value = new GDate(now.Year, now.Month, now.Day, -zone);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"--{now.Month:D2}-{now.Day:D2}-{zone:D2}"));
    if (str != null)
    {
      var value1 = new GDate(0, now.Month, now.Day, -zone);
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value1));
    }
  }

  [Test]
  public void TestGMonthDayZuluGDateTypeConverter()
  {
    var converter = new GDateTypeConverter { XsdType = XsdSimpleType.GMonthDay, ShowTimeZone = true };
    var now = DateTime.Now;
    var value = new GDate(now.Year, now.Month, now.Day, 0);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"--{now.Month:D2}-{now.Day:D2}Z"));
    if (str != null)
    {
      var value1 = new GDate(0, now.Month, now.Day);
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value1));
    }
  }

  [Test]
  public void TestGDayGDateTypeConverter()
  {
    var converter = new GDateTypeConverter { XsdType = XsdSimpleType.GDay };
    var now = DateTime.Now;
    var value = new GDate(now.Year, now.Month, now.Day); 
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"---{now.Day:D2}"));
    if (str != null)
    {
      var value1 = new GDate(0, 0, now.Day);
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value1));
    }
  }

  [Test]
  public void TestGDayZoneGDateTypeConverter()
  {
    var zone = 2;
    var converter = new GDateTypeConverter { XsdType = XsdSimpleType.GDay, ShowTimeZone = true };
    var now = DateTime.Now;
    var value = new GDate(now.Year, now.Month, now.Day, zone);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"---{now.Day:D2}+{zone:D2}"));
    if (str != null)
    {
      var value1 = new GDate(0, 0, now.Day, zone);
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value1));
    }
  }

  [Test]
  public void TesGtDayNegZoneGDateTypeConverter()
  {
    var zone = 2;
    var converter = new GDateTypeConverter { XsdType = XsdSimpleType.GDay, ShowTimeZone = true };
    var now = DateTime.Now;
    var value = new GDate(now.Year, now.Month, now.Day, -zone);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"---{now.Day:D2}-{zone:D2}"));
    if (str != null)
    {
      var value1 = new GDate(0, 0, now.Day, -zone);
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value1));
    }
  }

  [Test]
  public void TestGDayZuluGDateTypeConverter()
  {
    var converter = new GDateTypeConverter { XsdType = XsdSimpleType.GDay, ShowTimeZone = true };
    var now = DateTime.Now;
    var value = new GDate(now.Year, now.Month, now.Day, 0);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo($"---{now.Day:D2}Z"));
    if (str != null)
    {
      var value1 = new GDate(0, 0, now.Day, 0);
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value1));
    }
  }
}
