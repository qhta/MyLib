using System.Diagnostics;
using System.Text;

using NUnit.Framework;

using Qhta.Conversion;

namespace ConversionTest;

public class HexBinaryConverterTest
{
  [SetUp]
  public void Setup()
  {
  }

  [Test]
  public void TestNullHexBinaryTypeConverter()
  {
    var converter = new ArrayTypeConverter { XsdType = XsdSimpleType.HexBinary };
    byte[]? bytes1 = null;
    var str = converter.ConvertTo(bytes1, typeof(string));
    Assert.That(str, Is.Null);
    var bytes2 = converter.ConvertFrom(str!);
    Assert.That(bytes2, Is.EqualTo(bytes1));
  }

  [Test]
  public void TestEmptyHexBinaryTypeConverter()
  {
    var converter = new ArrayTypeConverter { XsdType = XsdSimpleType.HexBinary };
    byte[]? bytes1 = new byte[0];
    var str = converter.ConvertTo(bytes1, typeof(string));
    Assert.That(str, Is.EqualTo(""));
    if (str != null)
    {
      var bytes2 = converter.ConvertFrom(str);
      Assert.That(bytes2, Is.EqualTo(bytes1));
    }
  }

  [Test]
  public void TestShortHexBinaryTypeConverter()
  {
    var converter = new ArrayTypeConverter { XsdType = XsdSimpleType.HexBinary };
    byte[]? bytes1 = new byte[256];
    for (int i = 0; i < 256; i++)
      bytes1[i]=(byte)i;
    var str0 = Convert.ToHexString(bytes1);
    var str1 = converter.ConvertTo(bytes1, typeof(string));
    Assert.That(str1, Is.EqualTo(str0));
    if (str1 != null)
    {
      var bytes2 = converter.ConvertFrom(str1);
      Assert.That(bytes2, Is.EqualTo(bytes1));
    }
  }

  //[Test]
  //public void TestLongHexBinaryTypeConverter()
  //{
  //  int length = 10_000_000;
  //  var converter = new ArrayTypeConverter { XsdType = XsdSimpleType.HexBinary };
  //  byte[]? bytes1 = new byte[length];
  //  for (int i = 0; i < length; i++)
  //    bytes1[i] = (byte)i;
  //  var str = converter.ConvertTo(bytes1, typeof(string));
  //  if (str != null)
  //  {
  //    var bytes2 = converter.ConvertFrom(str);
  //    Assert.That(bytes2, Is.EqualTo(bytes1));
  //  }
  //}

  //[Test]
  public void TestMaxLengthHexBinaryTypeConverter()
  {
    var length1 = 536870911; //536870895
    var length0 = 532676607;
    int length;
    while (true)
    {
      length = length0 + (length1 - length0) / 2;
      TestContext.Progress.WriteLine(length);
      try
      {
        TestLongHexBinaryTypeConverter(length);
        if (length == length0 || length1 - length0 <= 1)
          break;
        length0 = length;
      }
      catch (OutOfMemoryException)
      {
        length1 = length;
      }
    }
    TestContext.Out.WriteLine($"MaxLength={length}");
  }

  public void TestLongHexBinaryTypeConverter(int length)
  {
    var converter = new ArrayTypeConverter { XsdType = XsdSimpleType.HexBinary };
    byte[] bytes1 = new byte[length];
    var str = converter.ConvertTo(bytes1, typeof(string));
    if (str != null)
    {
      var bytes2 = converter.ConvertFrom(str);
    }
  }


}
