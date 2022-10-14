using System.Diagnostics;
using System.Text;

using NUnit.Framework;

using Qhta.Conversion;

namespace ConversionTest;

public class Base64BinaryConverterTest
{
  [SetUp]
  public void Setup()
  {
  }

  [Test]
  public void TestNullBase64BinaryTypeConverter()
  {
    var converter = new ArrayTypeConverter{ Mode = ByteArrayConversionMode.Base64Binary};
    byte[]? bytes1 = null;
    var str = converter.ConvertTo(bytes1, typeof(string));
    Assert.That(str, Is.Null);
    var bytes2 = converter.ConvertFrom(str!);
    Assert.That(bytes2, Is.EqualTo(bytes1));
  }

  [Test]
  public void TestEmptyBase64BinaryTypeConverter()
  {
    var converter = new ArrayTypeConverter { Mode = ByteArrayConversionMode.Base64Binary };
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
  public void TestZeroBase64BinaryTypeConverter()
  {
    var converter = new ArrayTypeConverter { Mode = ByteArrayConversionMode.Base64Binary };
    byte[]? bytes1 = new byte[256];
    var str = converter.ConvertTo(bytes1, typeof(string));
    if (str != null)
    {
      var bytes2 = converter.ConvertFrom(str);
      Assert.That(bytes2, Is.EqualTo(bytes1));
    }
  }

  [Test]
  public void TestShortBase64BinaryTypeConverter()
  {
    var converter = new ArrayTypeConverter { Mode = ByteArrayConversionMode.Base64Binary };
    byte[]? bytes1 = new byte[256];
    for (int i = 0; i < 256; i++)
      bytes1[i] = (byte)i;
    var str = converter.ConvertTo(bytes1, typeof(string));
    if (str != null)
    {
      var bytes2 = converter.ConvertFrom(str);
      Assert.That(bytes2, Is.EqualTo(bytes1));
    }
  }

  [Test]
  public void TestLongBase64BinaryTypeConverter()
  {
    int lenth = 10000000;
    var converter = new ArrayTypeConverter { Mode = ByteArrayConversionMode.Base64Binary };
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
  public void TestMaxLengthBase64BinaryTypeConverter()
  {
    Assert.Throws(typeof(OutOfMemoryException), () =>
    {
      int maxLength = int.MaxValue;
      var converter = new ArrayTypeConverter { Mode = ByteArrayConversionMode.Base64Binary };
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
