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
    var converter = new ArrayTypeConverter { Mode = ByteArrayConversionMode.HexBinary };
    byte[]? bytes1 = null;
    var str = converter.ConvertTo(bytes1, typeof(string));
    Assert.That(str, Is.Null);
    var bytes2 = converter.ConvertFrom(str!);
    Assert.That(bytes2, Is.EqualTo(bytes1));
  }

  [Test]
  public void TestEmptyHexBinaryTypeConverter()
  {
    var converter = new ArrayTypeConverter { Mode = ByteArrayConversionMode.HexBinary };
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
  public void TestZeroHexBinaryTypeConverter()
  {
    var converter = new ArrayTypeConverter { Mode = ByteArrayConversionMode.HexBinary };
    byte[]? bytes1 = new byte[256];
    var str = converter.ConvertTo(bytes1, typeof(string));
    if (str != null)
    {
      var bytes2 = converter.ConvertFrom(str);
      Assert.That(bytes2, Is.EqualTo(bytes1));
    }
  }

  [Test]
  public void TestShortHexBinaryTypeConverter()
  {
    var converter = new ArrayTypeConverter { Mode = ByteArrayConversionMode.HexBinary };
    byte[]? bytes1 = new byte[256];
    for (int i = 0; i < 256; i++)
      bytes1[i]=(byte)i;
    var str = converter.ConvertTo(bytes1, typeof(string));
    if (str != null)
    {
      var bytes2 = converter.ConvertFrom(str);
      Assert.That(bytes2, Is.EqualTo(bytes1));
    }
  }

  [Test]
  public void TestLongHexBinaryTypeConverter()
  {
    int lenth = 1000000;
    var converter = new ArrayTypeConverter { Mode = ByteArrayConversionMode.HexBinary };
    byte[]? bytes1 = new byte[lenth];
    for (int i = 0; i < lenth; i++)
      bytes1[i] = (byte)i;
    var str = converter.ConvertTo(bytes1, typeof(string));
    if (str != null)
    {
      var bytes2 = converter.ConvertFrom(str);
      Assert.That(bytes2, Is.EqualTo(bytes1));
    }
  }

  [Test]
  public void TestMaxLengthHexBinaryTypeConverter()
  {
    {
      Assert.Throws(typeof(OutOfMemoryException), () =>
      {
        int maxLength = int.MaxValue;
        var converter = new ArrayTypeConverter { Mode = ByteArrayConversionMode.HexBinary };
        byte[]? bytes1 = new byte[maxLength];
        for (int i = 0; i < maxLength; i++)
          bytes1[i] = (byte)i;
        var str = converter.ConvertTo(bytes1, typeof(string));
        if (str != null)
        {
          var bytes2 = converter.ConvertFrom(str);
          Assert.That(bytes2, Is.EqualTo(bytes1));
        }
      });
    }
  }

}
