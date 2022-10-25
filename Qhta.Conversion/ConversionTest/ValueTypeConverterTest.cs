using System;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Text;
using System.Xml;

using Qhta.Conversion;
using Qhta.TypeUtils;

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
      if (ValueTypeConverter.XsdSimpleTypeAcceptedTypes.TryGetValue(xsdType, out var allowedTypes))
        foreach (var allowedType in allowedTypes)
          TestTypedValueTypeConverter(allowedType, xsdType);
    }

    [Test]
    public void TestKnownTypesValueTypeConverter()
    {
      foreach (var item in ValueTypeConverter.KnownTypeConvertersTypes)
      {
        TestExpectedTypeValueTypeConverter(item.Key);
      }
    }

    public void TestExpectedTypeValueTypeConverter(Type expectedType)
    {
      foreach (var xsdType in typeof(XsdSimpleType).GetEnumValues().Cast<XsdSimpleType>())
        TestTypedValueTypeConverter(expectedType, xsdType);
    }

    public void TestTypedValueTypeConverter(Type allowedType, XsdSimpleType xsdType)
    {
      foreach (var data in TestData)
      {
        var testDataType = data.Value.GetType();
        if (testDataType == allowedType)
        {
          if (data.XsdType == xsdType)
          {
            TestSingleValueTypeConverter(allowedType, xsdType, data);
          }
        }
        else if (allowedType == typeof(Array) && testDataType.IsCollection())
          if (data.XsdType == xsdType)
          {
            TestSingleValueTypeConverter(allowedType, xsdType, data);
          }

      }
    }


    private static ConverterTestData[] TestData =
    {
      new ConverterTestData{ Value = true, XsdType = XsdSimpleType.Boolean, Text="True" },
      new ConverterTestData{ Value = false, XsdType = XsdSimpleType.Boolean, Text="False" },
      new ConverterTestData{ Value = true, XsdType = XsdSimpleType.Integer, Text="1" },
      new ConverterTestData{ Value = false, XsdType = XsdSimpleType.Integer, Text="0" },
      new ConverterTestData{ Value = true, XsdType = XsdSimpleType.String, Text="on" },
      new ConverterTestData{ Value = false, XsdType = XsdSimpleType.String, Text="off" },
      new ConverterTestData{ Value = "abc", XsdType = XsdSimpleType.String, Text="abc" },
      new ConverterTestData{ Value = "abc", XsdType = XsdSimpleType.Token, Text="abc" },
      new ConverterTestData{ Value = "abc", XsdType = XsdSimpleType.NmToken, Text="abc" },
      new ConverterTestData{ Value = "abc", XsdType = XsdSimpleType.Id, Text="abc" },
      new ConverterTestData{ Value = "abc", XsdType = XsdSimpleType.IdRef, Text="abc" },
      new ConverterTestData{ Value = "abc", XsdType = XsdSimpleType.Entity, Text="abc" },
      new ConverterTestData{ Value = "1", XsdType = XsdSimpleType.Integer, Text="1" },
      new ConverterTestData{ Value = "1", XsdType = XsdSimpleType.PositiveInteger, Text="1" },
      new ConverterTestData{ Value = "0", XsdType = XsdSimpleType.NonNegativeInteger, Text="0" },
      new ConverterTestData{ Value = "0", XsdType = XsdSimpleType.NonPositiveInteger, Text="0" },
      new ConverterTestData{ Value = "-1", XsdType = XsdSimpleType.NegativeInteger, Text="-1" },
      new ConverterTestData{ Value = int.MaxValue, XsdType = XsdSimpleType.Int, Text="2147483647" },
      new ConverterTestData{ Value = uint.MaxValue, XsdType = XsdSimpleType.UnsignedInt, Text="4294967295" },
      new ConverterTestData{ Value = sbyte.MaxValue, XsdType = XsdSimpleType.Byte, Text="127" },
      new ConverterTestData{ Value = byte.MaxValue, XsdType = XsdSimpleType.UnsignedByte, Text="255" },
      new ConverterTestData{ Value = short.MaxValue, XsdType = XsdSimpleType.Short, Text="32767" },
      new ConverterTestData{ Value = ushort.MaxValue, XsdType = XsdSimpleType.UnsignedShort, Text="65535" },
      new ConverterTestData{ Value = long.MaxValue, XsdType = XsdSimpleType.Long, Text="9223372036854775807" },
      new ConverterTestData{ Value = ulong.MaxValue, XsdType = XsdSimpleType.UnsignedLong, Text="18446744073709551615" },
      new ConverterTestData{ Value = decimal.MaxValue, XsdType = XsdSimpleType.UnsignedLong, Text="79228162514264337593543950335" },
      new ConverterTestData{ Value = float.MaxValue, XsdType = XsdSimpleType.Float, Text="3.4028235E+38" },
      new ConverterTestData{ Value = double.MaxValue, XsdType = XsdSimpleType.Double, Text="1.7976931348623157E+308" },
      new ConverterTestData{ Value = int.MinValue, XsdType = XsdSimpleType.Int, Text="-2147483648" },
      new ConverterTestData{ Value = uint.MinValue, XsdType = XsdSimpleType.UnsignedInt, Text="0" },
      new ConverterTestData{ Value = sbyte.MinValue, XsdType = XsdSimpleType.Byte, Text="-128" },
      new ConverterTestData{ Value = byte.MinValue, XsdType = XsdSimpleType.UnsignedByte, Text="0" },
      new ConverterTestData{ Value = short.MinValue, XsdType = XsdSimpleType.Short, Text="-32768" },
      new ConverterTestData{ Value = ushort.MinValue, XsdType = XsdSimpleType.UnsignedShort, Text="0" },
      new ConverterTestData{ Value = long.MinValue, XsdType = XsdSimpleType.Long, Text="-9223372036854775808" },
      new ConverterTestData{ Value = ulong.MinValue, XsdType = XsdSimpleType.UnsignedLong, Text="0" },
      new ConverterTestData{ Value = decimal.MinValue, XsdType = XsdSimpleType.UnsignedLong, Text="-79228162514264337593543950335" },
      new ConverterTestData{ Value = float.MinValue, XsdType = XsdSimpleType.Float, Text="-3.4028235E+38" },
      new ConverterTestData{ Value = double.MinValue, XsdType = XsdSimpleType.Double, Text="-1.7976931348623157E+308" },
      new ConverterTestData{ Value = decimal.MaxValue, XsdType = XsdSimpleType.Integer, Text="79228162514264337593543950335" },
      new ConverterTestData{ Value = decimal.MaxValue, XsdType = XsdSimpleType.PositiveInteger, Text="79228162514264337593543950335" },
      new ConverterTestData{ Value = 0, XsdType = XsdSimpleType.NonNegativeInteger, Text="0" },
      new ConverterTestData{ Value = 0, XsdType = XsdSimpleType.NonPositiveInteger, Text="0" },
      new ConverterTestData{ Value = decimal.MinValue, XsdType = XsdSimpleType.NegativeInteger, Text="-79228162514264337593543950335" },
      new ConverterTestData{ Value = new byte[]{1,2,3}, XsdType = XsdSimpleType.Base64Binary, Text="AQID" },
      new ConverterTestData{ Value = new byte[]{1,2,3}, XsdType = XsdSimpleType.HexBinary, Text="010203" },
      new ConverterTestData{ Value = new byte[]{1,2,3}, XsdType = 0, Text="1 2 3" },
      new ConverterTestData{ Value = new string[]{"1","2","3"}, XsdType = 0, Text="1 2 3" },
      new ConverterTestData{ Value = new int[]{1,2,3}, XsdType = 0, Text="1 2 3" },
      new ConverterTestData{ Value = new Uri("HTTP://www.Contoso.com:80/thick%20and%20thin.htm"), XsdType = 0, Text="HTTP://www.Contoso.com:80/thick%20and%20thin.htm" },
      new ConverterTestData{ Value = new XmlQualifiedName("name","prefix"), XsdType = 0, Text="prefix:name" },
      new ConverterTestData{ Value = new GDate(2022, 0, 0), XsdType = XsdSimpleType.GYear, Text="2022" },
      new ConverterTestData{ Value = new GDate(2022, 2, 0), XsdType = XsdSimpleType.GYearMonth, Text="2022-02" },
      new ConverterTestData{ Value = new GDate(0, 2, 0), XsdType = XsdSimpleType.GMonth, Text="--02" },
      new ConverterTestData{ Value = new GDate(0, 2, 1), XsdType = XsdSimpleType.GMonthDay, Text="--02-01" },
      new ConverterTestData{ Value = new GDate(0, 0, 1), XsdType = XsdSimpleType.GDay, Text="---01" },
    };

    public void TestSingleValueTypeConverter(Type? expectedType, XsdSimpleType? xsdType, ConverterTestData data)
    {
      if (xsdType == 0)
        xsdType = null;
      ValueTypeConverter converter = new ValueTypeConverter { ExpectedType = expectedType, XsdType = xsdType };
      if (expectedType == null)
        converter.ExpectedType = data.Value.GetType();
      converter.Init();

      var str1 = converter.ConvertTo(data.Value, typeof(string)) as string;
      var str0 = data.Text;
      //StringAssert.AreEqualIgnoringCase(str1, str0);
      Assert.That(str1, Is.EqualTo(str0), $"Expected string was \"{str0}\" but \"{str1}\" produced" + ErrorMsgTail(converter, data.XsdType));
      var value = converter.ConvertFrom(str1);
      Assert.IsNotNull(value, $"Value expected but null produced" + ErrorMsgTail(converter, data.XsdType));
      if (expectedType != null && value != null)
      {
        if (expectedType == typeof(Array))
        {
          if (!value.GetType().IsArray)
            Assert.That(value.GetType, Is.EqualTo(expectedType),
              $"Expected type was {expectedType.Name} but {value.GetType().Name} produced" + ErrorMsgTail(converter, data.XsdType));
        }
        else
          Assert.That(value.GetType, Is.EqualTo(expectedType),
            $"Expected type was {expectedType.Name} but {value.GetType().Name} produced" + ErrorMsgTail(converter, data.XsdType));

      }
    }

    private string ErrorMsgTail(ValueTypeConverter converter, XsdSimpleType? xsdType)
    {
      return $" in {converter.InternalTypeConverter?.GetType().Name}" + (xsdType == null ? null : $" with XsdType = {xsdType}");
    }
  }
}