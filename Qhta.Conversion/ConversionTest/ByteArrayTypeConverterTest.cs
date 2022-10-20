using System;
using System.Diagnostics;
using System.Text;

using NUnit.Framework;

using Qhta.Conversion;

namespace ConversionTest;

public class ByteArrayTypeConverterTest
{
  [SetUp]
  public void Setup()
  {
  }

  [Test]
  public void TestNullArrayTypeConverter()
  {
    var converter = new ArrayTypeConverter { XsdType = XsdSimpleType.UnsignedByte };
    byte[]? bytes1 = null;
    var str = converter.ConvertTo(bytes1, typeof(string));
    Assert.That(str, Is.Null);
    var bytes2 = converter.ConvertFrom(str!);
    Assert.That(bytes2, Is.EqualTo(bytes1));
  }

  [Test]
  public void TestEmptyArrayTypeConverter()
  {
    var converter = new ArrayTypeConverter { XsdType = XsdSimpleType.UnsignedByte };
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
  public void TestShortArrayTypeConverter()
  {
    var converter = new ArrayTypeConverter { XsdType = XsdSimpleType.UnsignedByte };
    byte[]? bytes1 = new byte[256];
    var strs = new string[256];
    for (int i = 0; i < 256; i++)
    {
      bytes1[i] = (byte)i;
      strs[i] = i.ToString();
    }
    var str0 = String.Join(" ", strs);
    var str1 = converter.ConvertTo(bytes1, typeof(string));
    Assert.That(str1, Is.EqualTo(str0));
    if (str1 != null)
    {
      var bytes2 = converter.ConvertFrom(str1);
      Assert.That(bytes2, Is.EqualTo(bytes1));
    }
  }

  [Test]
  public void TestTooLongArrayTypeConverter()
  {
    Assert.Throws(typeof(InvalidDataException), () =>
    {
      var converter = new ArrayTypeConverter { XsdType = XsdSimpleType.UnsignedByte, MaxLength = 255 };
      byte[]? bytes1 = new byte[256];
      var strs = new string[256];
      for (int i = 0; i < 256; i++)
      {
        bytes1[i] = (byte)i;
        strs[i] = i.ToString();
      }
      var str0 = String.Join(" ", strs);
      var str1 = converter.ConvertTo(bytes1, typeof(string));
      Assert.That(str1, Is.EqualTo(str0));
      if (str1 != null)
      {
        var bytes2 = converter.ConvertFrom(str1);
        Assert.That(bytes2, Is.EqualTo(bytes1));
      }
    });
  }

  [Test]
  public void TestTooShortArrayTypeConverter()
  {
    Assert.Throws(typeof(InvalidDataException), () =>
    {
      var converter = new ArrayTypeConverter { XsdType = XsdSimpleType.UnsignedByte, MinLength = 257 };
      byte[]? bytes1 = new byte[256];
      var strs = new string[256];
      for (int i = 0; i < 256; i++)
      {
        bytes1[i] = (byte)i;
        strs[i] = i.ToString();
      }
      var str0 = String.Join(" ", strs);
      var str1 = converter.ConvertTo(bytes1, typeof(string));
      Assert.That(str1, Is.EqualTo(str0));
      if (str1 != null)
      {
        var bytes2 = converter.ConvertFrom(str1);
        Assert.That(bytes2, Is.EqualTo(bytes1));
      }
    });
  }

  [Test]
  public void TestFormattedArrayTypeConverter()
  {
    var converter = new ArrayTypeConverter { XsdType = XsdSimpleType.UnsignedByte, Format = "X2" };
    byte[]? bytes1 = new byte[256];
    var strs = new string[256];
    for (int i = 0; i < 256; i++)
    {
      bytes1[i] = ((byte)i);
      strs[i] = i.ToString("X2");
    }
    var str0 = String.Join(" ", strs);
    var str1 = converter.ConvertTo(bytes1, typeof(string));
    Assert.That(str1, Is.EqualTo(str0));
    if (str1 != null)
    {
      var bytes2 = converter.ConvertFrom(str1);
      Assert.That(bytes2, Is.EqualTo(bytes1));
    }
  }

  //[Test]
  public void TestMaxLengthByteArrayTypeConverter()
  {
    var length1 = int.MaxValue; // 536870896
    var length0 = 0;
    int length;
    while (true)
    {
      length = length0 + (length1 - length0) / 2;
      TestContext.Progress.WriteLine(length);
      try
      {
        TestLongByteArrayTypeConverter(length);
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

  public void TestLongByteArrayTypeConverter(int length)
  {
    var converter = new ArrayTypeConverter { XsdType = XsdSimpleType.UnsignedByte };
    byte[] bytes1 = new byte[length];
    var str = converter.ConvertTo(bytes1, typeof(string));
    if (str != null)
    {
      var bytes2 = converter.ConvertFrom(str);
    }
  }
}
