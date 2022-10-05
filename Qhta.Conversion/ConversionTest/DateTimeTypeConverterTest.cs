using System.Diagnostics;
using System.Globalization;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;

using NUnit.Framework;

using Qhta.Conversion;

namespace ConversionTest;

public class DateTimeTypeConverterTest
{
  [SetUp]
  public void Setup()
  {
  }

  [Test]
  public void TestNullDateTimeTypeConverter()
  {
    var converter = new DateTimeTypeConverter();
    object? value = null;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.Null);
    var value2 = converter.ConvertFrom(str!);
    Assert.That(value2, Is.EqualTo(value));
  }

  [Test]
  public void TestEmptyDateTimeTypeConverter()
  {
    var converter = new DateTimeTypeConverter();
    var value2 = converter.ConvertFrom("");
    Assert.That(value2, Is.EqualTo(null));
  }

  [Test]
  public void TestDefaultModeDateTimeTypeConverter()
  {
    var converter = new DateTimeTypeConverter();
    var now = DateTime.Now;
    var value = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString("yyyy-MM-dd HH:mm:ss")));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestDateTimeModeDateTimeTypeConverter()
  {
    var converter = new DateTimeTypeConverter{ Mode = DateTimeConversionMode.DateTime};
    var now = DateTime.Now;
    var value = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString("yyyy-MM-dd HH:mm:ss")));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestDefaultModeTimeZeroDateTimeTypeConverter()
  {
    var converter = new DateTimeTypeConverter();
    var now = DateTime.Now;
    var value = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString("yyyy-MM-dd")));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestDateTimeModeTimeZeroDateTimeTypeConverter()
  {
    var converter = new DateTimeTypeConverter { Mode = DateTimeConversionMode.DateTime };
    var now = DateTime.Now;
    var value = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString("yyyy-MM-dd HH:mm:ss")));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestDateOnlyModeDateTimeTypeConverter()
  {
    var converter = new DateTimeTypeConverter { Mode = DateTimeConversionMode.DateOnly };
    var now = DateTime.Now;
    var value = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString("yyyy-MM-dd")));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      value = now.Date;
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestTimeOnlyModeDateTimeTypeConverter()
  {
    var converter = new DateTimeTypeConverter { Mode = DateTimeConversionMode.TimeOnly };
    var now = DateTime.Now;
    var value = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString("HH:mm:ss")));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      value = new DateTime(1, 1, 1, now.Hour, now.Minute, now.Second);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestDefaultModeDateTypeConverter()
  {
    var converter = new DateTimeTypeConverter();
    var now = DateTime.Now;
    var value = DateOnly.FromDateTime(now);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString("yyyy-MM-dd")));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.TypeOf<DateTime>());
      var value3 = DateTime.Parse((string)str);
      Assert.That(value2, Is.EqualTo(value3));
    }
  }

  [Test]
  public void TestDateExpectedTypeConverter()
  {
    var converter = new DateTimeTypeConverter{ ExpectedType = typeof(DateOnly)};
    var now = DateTime.Now;
    var value = DateOnly.FromDateTime(now);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString("yyyy-MM-dd")));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.TypeOf<DateOnly>());
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestTimeExpectedTypeConverter()
  {
    var converter = new DateTimeTypeConverter { ExpectedType = typeof(TimeOnly) };
    var now = DateTime.Now;
    now = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
    var value = TimeOnly.FromDateTime(now);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString("HH:mm:ss")));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.TypeOf<TimeOnly>());
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestFullTimeExpectedTypeConverter()
  {
    var converter = new DateTimeTypeConverter { ExpectedType = typeof(TimeOnly), ShowFullTime = true};
    var now = DateTime.Now;
    var value = TimeOnly.FromDateTime(now);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString("HH:mm:ss.FFFFFFF")));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.TypeOf<TimeOnly>());
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestFullTimeAndZoneExpectedTypeConverter()
  {
    var converter = new DateTimeTypeConverter { ExpectedType = typeof(TimeOnly), ShowFullTime = true, ShowTimeZone = true };
    var now = DateTime.Now;
    var value = TimeOnly.FromDateTime(now);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString("HH:mm:ss.FFFFFFF") + "+02:00"));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.TypeOf<TimeOnly>());
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestWebDefaultDateTimeTypeConverter()
  {
    var converter = new DateTimeTypeConverter{ DateTimeSeparator = 'T', ShowFullTime = true, ShowTimeZone = true };
    var now = DateTime.Now;
    var value = now;
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz")));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestCurrentCultureDateTimeTypeConverter()
  {
    var culture = CultureInfo.CurrentCulture;
    var converter = new DateTimeTypeConverter();
    var now = DateTime.Now;
    var value = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
    var str = converter.ConvertTo(null, culture, value, typeof(string));
    var dtFormat = culture.DateTimeFormat;
    var format = dtFormat.ShortDatePattern + ' ' + dtFormat.LongTimePattern;
    Assert.That(str, Is.EqualTo(value.ToString(format)));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(null, culture, str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestInvariantCultureDateTimeTypeConverter()
  {
    var culture = CultureInfo.InvariantCulture;
    var converter = new DateTimeTypeConverter();
    var now = DateTime.Now;
    var value = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
    var str = converter.ConvertTo(null, culture, value, typeof(string));
    var dtFormat = culture.DateTimeFormat;
    var format = dtFormat.ShortDatePattern + ' ' + dtFormat.LongTimePattern;
    Assert.That(str, Is.EqualTo(value.ToString(format)));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(null, culture, str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestFormattedDateTimeTypeConverter()
  {
    var dtFormat = CultureInfo.CurrentCulture.DateTimeFormat;
    var format = dtFormat.LongDatePattern + ' ' + dtFormat.ShortTimePattern;
    var converter = new DateTimeTypeConverter{ Format = format };
    var now = DateTime.Now;
    var value = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString(format)));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestFormattedTimeOnlyDateTimeTypeConverter()
  {
    var dtFormat = CultureInfo.CurrentCulture.DateTimeFormat;
    var format = dtFormat.ShortTimePattern;
    var converter = new DateTimeTypeConverter { Format = format, ExpectedType = typeof(TimeOnly) };
    var now = DateTime.Now;
    var value = new TimeOnly(now.Hour, now.Minute, 0);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString(format)));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestFormatInfoDateTimeTypeConverter()
  {
    var dtFormat = CultureInfo.InvariantCulture.DateTimeFormat;
    var format = dtFormat.ShortDatePattern + ' ' + dtFormat.ShortTimePattern;
    var converter = new DateTimeTypeConverter { Format = format, FormatInfo = CultureInfo.InvariantCulture.DateTimeFormat };
    var now = DateTime.Now;
    var value = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
    var str = converter.ConvertTo(value, typeof(string));
    Assert.That(str, Is.EqualTo(value.ToString(format)));
    if (str != null)
    {
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value));
    }
  }

  [Test]
  public void TestStyleDateTimeTypeConverter()
  {
    var converter = new DateTimeTypeConverter { DateTimeStyle = DateTimeStyles.AssumeUniversal };
    var now = DateTime.Now;
    var value = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
    var str = converter.ConvertTo(value, typeof(string)) as string;
    Assert.That(str, Is.EqualTo(value.ToString("yyyy-MM-dd HH:mm:ss")));
    if (str != null)
    {
      str = str.Insert(10, "T");
      var value2 = converter.ConvertFrom(str);
      Assert.That(value2, Is.EqualTo(value.AddHours(+2)));
    }
  }
}
