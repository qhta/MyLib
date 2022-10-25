using System;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Text;

using Qhta.Conversion;

namespace ConversionTest
{
  public class ValueTypeConverterTest
  {
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestNullValueTypeConverter()
    {
      var converter = new ValueTypeConverter();
      string? str1 = null;
      var str2 = converter.ConvertTo(str1, typeof(string));
      Assert.That(str2, Is.EqualTo(str1));
      var str3 = converter.ConvertFrom(str2!);
      Assert.That(str3, Is.EqualTo(str2));
    }

    [Test]
    public void TestAllXsdTypesValueTypeConverter()
    {
      foreach (var xsdType in typeof(XsdSimpleType).GetEnumValues().Cast<XsdSimpleType>())
        TestXsdTypeValueTypeConverter(xsdType);
    }

    public void TestXsdTypeValueTypeConverter(XsdSimpleType xsdType)
    {
      var converter = new ValueTypeConverter { XsdType = xsdType };
      converter.Init();
      Assert.IsNotNull(converter.InternalTypeConverter);
      var allowedTypes = ValueTypeConverter.XsdSimpleTypeAcceptedTypes[xsdType];
      var expectedType = allowedTypes.FirstOrDefault();
      Assert.That(converter.ExpectedType, Is.EqualTo(expectedType));
      object? value = null;
      var str1 = converter.ConvertTo(value, typeof(string));
      Assert.IsNull(str1);
      foreach (var allowedType in allowedTypes)
        TestAllowedTypeValueTypeConverter(converter, allowedType, xsdType);
    }

    [Test]
    public void TestAllAllowedTypesValueTypeConverter()
    {
      var allowedTypes = new List<Type>();
      foreach (var pair in ValueTypeConverter.XsdSimpleTypeAcceptedTypes)
      {
        foreach (var type in pair.Value)
        {
          if (!allowedTypes.Contains(type))
          {
            allowedTypes.Add(type);
          }
        }
      }
      foreach (var type in allowedTypes)
        TestExpectedTypeValueTypeConverter(type);
    }
    public void TestExpectedTypeValueTypeConverter(Type expectedType)
    {
      var converter = new ValueTypeConverter { ExpectedType = expectedType };
      converter.Init();
      Assert.IsNotNull(converter.InternalTypeConverter);
      Assert.That(converter.ExpectedType, Is.EqualTo(expectedType));
      object? value = null;
      var str1 = converter.ConvertTo(value, typeof(string));
      Assert.IsNull(str1);
      foreach (var xsdType in typeof(XsdSimpleType).GetEnumValues().Cast<XsdSimpleType>())
        TestExpectedTypeValueTypeConverter(converter, expectedType, xsdType);
    }

    public void TestAllowedTypeValueTypeConverter(ValueTypeConverter converter, Type allowedType, XsdSimpleType xsdType)
    {
      foreach (var data in TestData)
      {
        var testType = data.ExpectedType ?? data.Value.GetType();

        if (testType == allowedType)
        {
          if (data.XsdType == null || data.XsdType == xsdType)
          {
            converter.ExpectedType = data.Value.GetType();
            converter.XsdType = xsdType;
            converter.Init();
            TestSingleValueValueTypeConverter(converter, allowedType, data);
          }
        }
      }
    }

    public void TestExpectedTypeValueTypeConverter(ValueTypeConverter converter, Type expectedType, XsdSimpleType xsdType)
    {
      foreach (var data in TestData)
      {
        var testType = data.ExpectedType ?? data.Value.GetType();
        if (testType == expectedType)
        {
          if (data.XsdType == xsdType)
          {
            converter.ExpectedType = expectedType;
            converter.XsdType = xsdType;
            converter.Init();
            TestSingleValueValueTypeConverter(converter, expectedType, data);
          }
        }
      }
    }
    private static ConverterTestData[] TestData = 
    {
      //new ConverterTestData{ Value = true, XsdType = XsdSimpleType.Boolean, Text="True" },
      //new ConverterTestData{ Value = false, XsdType = XsdSimpleType.Boolean, Text="False" },
      //new ConverterTestData{ Value = true, XsdType = XsdSimpleType.Integer, Text="1" },
      //new ConverterTestData{ Value = false, XsdType = XsdSimpleType.Integer, Text="0" },
      //new ConverterTestData{ Value = true, XsdType = XsdSimpleType.String, Text="on" },
      //new ConverterTestData{ Value = false, XsdType = XsdSimpleType.String, Text="off" },
      //new ConverterTestData{ Value = "abc", XsdType = XsdSimpleType.String, Text="abc" },
      //new ConverterTestData{ Value = "abc", XsdType = XsdSimpleType.Token, Text="abc" },
      //new ConverterTestData{ Value = "abc", XsdType = XsdSimpleType.NmToken, Text="abc" },
      //new ConverterTestData{ Value = "abc", XsdType = XsdSimpleType.Id, Text="abc" },
      //new ConverterTestData{ Value = "abc", XsdType = XsdSimpleType.IdRef, Text="abc" },
      //new ConverterTestData{ Value = "abc", XsdType = XsdSimpleType.Entity, Text="abc" },
      //new ConverterTestData{ Value = "1", XsdType = XsdSimpleType.Integer, Text="1" },
      //new ConverterTestData{ Value = "1", XsdType = XsdSimpleType.PositiveInteger, Text="1" },
      //new ConverterTestData{ Value = "0", XsdType = XsdSimpleType.NonNegativeInteger, Text="0" },
      //new ConverterTestData{ Value = "0", XsdType = XsdSimpleType.NonPositiveInteger, Text="0" },
      //new ConverterTestData{ Value = "-1", XsdType = XsdSimpleType.NegativeInteger, Text="-1" },
      //new ConverterTestData{ Value = int.MaxValue, XsdType = XsdSimpleType.Int, Text="2147483647" },
      //new ConverterTestData{ Value = uint.MaxValue, XsdType = XsdSimpleType.UnsignedInt, Text="4294967295" },
      //new ConverterTestData{ Value = sbyte.MaxValue, XsdType = XsdSimpleType.Byte, Text="127" },
      //new ConverterTestData{ Value = byte.MaxValue, XsdType = XsdSimpleType.UnsignedByte, Text="255" },
      //new ConverterTestData{ Value = short.MaxValue, XsdType = XsdSimpleType.Short, Text="32767" },
      //new ConverterTestData{ Value = ushort.MaxValue, XsdType = XsdSimpleType.UnsignedShort, Text="65535" },
      //new ConverterTestData{ Value = long.MaxValue, XsdType = XsdSimpleType.Long, Text="9223372036854775807" },
      //new ConverterTestData{ Value = ulong.MaxValue, XsdType = XsdSimpleType.UnsignedLong, Text="18446744073709551615" },
      //new ConverterTestData{ Value = decimal.MaxValue, XsdType = XsdSimpleType.UnsignedLong, Text="79228162514264337593543950335" },
      new ConverterTestData{ Value = float.MaxValue, XsdType = XsdSimpleType.Float, Text="3.4028235E+38" },
      //new ConverterTestData{ Value = double.MaxValue, XsdType = XsdSimpleType.Double, Text="3.4028235E+38" },
      //new ConverterTestData{ Value = new byte[]{1,2,3}, XsdType = XsdSimpleType.Integer, Text="-1" },
    };

    public void TestSingleValueValueTypeConverter(ValueTypeConverter converter, Type expectedType, ConverterTestData data)
    {
      var str1 = converter.ConvertTo(data.Value, typeof(string)) as string;
      var str0 = data.Text;
      //StringAssert.AreEqualIgnoringCase(str1, str0);
      Assert.That(str1, Is.EqualTo(str0), $"Expected string was \"{str0}\" but \"{str1}\" produced" + ErrorMsgTail(converter, data.XsdType));
      var value = converter.ConvertFrom(str1);
      Assert.IsNotNull(value, $"Value expected but null produced" + ErrorMsgTail(converter, data.XsdType));
      Assert.That(value.GetType, Is.EqualTo(expectedType), $"Expected type was {expectedType.Name} but {value.GetType().Name} produced" + ErrorMsgTail(converter, data.XsdType));
    }

    private string ErrorMsgTail(ValueTypeConverter converter, XsdSimpleType? xsdType)
    {
      return $" in {converter.InternalTypeConverter?.GetType().Name}" +(xsdType == null ? null : $" with XsdType = {xsdType}");
    }
  }
}