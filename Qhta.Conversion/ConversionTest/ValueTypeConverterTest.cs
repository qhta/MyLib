using System;
using System.ComponentModel;
using System.Diagnostics;
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
    public void TestAllSimpleTypesValueTypeConverter()
    {
      foreach (var xsdType in typeof(SimpleType).GetEnumValues().Cast<SimpleType>())
        TestSimpleTypeValueTypeConverter(xsdType);
    }

    public void TestSimpleTypeValueTypeConverter(SimpleType xsdType)
    {
      if (ValueTypeConverter.XsdSimpleTypeAcceptedTypes.TryGetValue(xsdType, out var allowedTypes))
        foreach (var allowedType in allowedTypes)
          TestTypedValueTypeConverter(allowedType, xsdType);
    }

    [Test]
    public void TestKnownTypesValueTypeConverter()
    {
      foreach (var item in ValueTypeConverter.KnownTypeConverters)
      {
        TestExpectedTypeValueTypeConverter(item.Key);
      }
    }

    public void TestExpectedTypeValueTypeConverter(Type expectedType)
    {
      foreach (var xsdType in typeof(SimpleType).GetEnumValues().Cast<SimpleType>())
        TestTypedValueTypeConverter(expectedType, xsdType);
    }

    public void TestTypedValueTypeConverter(Type allowedType, SimpleType xsdType)
    {
      foreach (var data in TestData)
      {
        var testDataType = data.Value.GetType();
        if (testDataType == allowedType)
        {
          if (data.SimpleType == xsdType)
          {
            TestSingleValueTypeConverter(allowedType, xsdType, data);
          }
        }
        else if (allowedType == typeof(Array) && testDataType.IsCollection())
          if (data.SimpleType == xsdType)
          {
            TestSingleValueTypeConverter(allowedType, xsdType, data);
          }

      }
    }


    private static ConverterTestData[] TestData =
    {
      new ConverterTestData{ Value = true, SimpleType = SimpleType.Boolean, Text="True" },
      new ConverterTestData{ Value = false, SimpleType = SimpleType.Boolean, Text="False" },
      new ConverterTestData{ Value = true, SimpleType = SimpleType.Integer, Text="1" },
      new ConverterTestData{ Value = false, SimpleType = SimpleType.Integer, Text="0" },
      new ConverterTestData{ Value = true, SimpleType = SimpleType.String, Text="on" },
      new ConverterTestData{ Value = false, SimpleType = SimpleType.String, Text="off" },
      new ConverterTestData{ Value = "abc", SimpleType = SimpleType.String, Text="abc" },
      new ConverterTestData{ Value = "abc", SimpleType = SimpleType.Token, Text="abc" },
      new ConverterTestData{ Value = "abc", SimpleType = SimpleType.NmToken, Text="abc" },
      new ConverterTestData{ Value = "abc", SimpleType = SimpleType.Id, Text="abc" },
      new ConverterTestData{ Value = "abc", SimpleType = SimpleType.IdRef, Text="abc" },
      new ConverterTestData{ Value = "abc", SimpleType = SimpleType.Entity, Text="abc" },
      new ConverterTestData{ Value = "1", SimpleType = SimpleType.Integer, Text="1" },
      new ConverterTestData{ Value = "1", SimpleType = SimpleType.PositiveInteger, Text="1" },
      new ConverterTestData{ Value = "0", SimpleType = SimpleType.NonNegativeInteger, Text="0" },
      new ConverterTestData{ Value = "0", SimpleType = SimpleType.NonPositiveInteger, Text="0" },
      new ConverterTestData{ Value = "-1", SimpleType = SimpleType.NegativeInteger, Text="-1" },
      new ConverterTestData{ Value = int.MaxValue, SimpleType = SimpleType.Int, Text="2147483647" },
      new ConverterTestData{ Value = uint.MaxValue, SimpleType = SimpleType.UnsignedInt, Text="4294967295" },
      new ConverterTestData{ Value = sbyte.MaxValue, SimpleType = SimpleType.Byte, Text="127" },
      new ConverterTestData{ Value = byte.MaxValue, SimpleType = SimpleType.UnsignedByte, Text="255" },
      new ConverterTestData{ Value = short.MaxValue, SimpleType = SimpleType.Short, Text="32767" },
      new ConverterTestData{ Value = ushort.MaxValue, SimpleType = SimpleType.UnsignedShort, Text="65535" },
      new ConverterTestData{ Value = long.MaxValue, SimpleType = SimpleType.Long, Text="9223372036854775807" },
      new ConverterTestData{ Value = ulong.MaxValue, SimpleType = SimpleType.UnsignedLong, Text="18446744073709551615" },
      new ConverterTestData{ Value = decimal.MaxValue, SimpleType = SimpleType.UnsignedLong, Text="79228162514264337593543950335" },
      new ConverterTestData{ Value = float.MaxValue, SimpleType = SimpleType.Float, Text="3.4028235E+38" },
      new ConverterTestData{ Value = double.MaxValue, SimpleType = SimpleType.Double, Text="1.7976931348623157E+308" },
      new ConverterTestData{ Value = int.MinValue, SimpleType = SimpleType.Int, Text="-2147483648" },
      new ConverterTestData{ Value = uint.MinValue, SimpleType = SimpleType.UnsignedInt, Text="0" },
      new ConverterTestData{ Value = sbyte.MinValue, SimpleType = SimpleType.Byte, Text="-128" },
      new ConverterTestData{ Value = byte.MinValue, SimpleType = SimpleType.UnsignedByte, Text="0" },
      new ConverterTestData{ Value = short.MinValue, SimpleType = SimpleType.Short, Text="-32768" },
      new ConverterTestData{ Value = ushort.MinValue, SimpleType = SimpleType.UnsignedShort, Text="0" },
      new ConverterTestData{ Value = long.MinValue, SimpleType = SimpleType.Long, Text="-9223372036854775808" },
      new ConverterTestData{ Value = ulong.MinValue, SimpleType = SimpleType.UnsignedLong, Text="0" },
      new ConverterTestData{ Value = decimal.MinValue, SimpleType = SimpleType.UnsignedLong, Text="-79228162514264337593543950335" },
      new ConverterTestData{ Value = float.MinValue, SimpleType = SimpleType.Float, Text="-3.4028235E+38" },
      new ConverterTestData{ Value = double.MinValue, SimpleType = SimpleType.Double, Text="-1.7976931348623157E+308" },
      new ConverterTestData{ Value = decimal.MaxValue, SimpleType = SimpleType.Integer, Text="79228162514264337593543950335" },
      new ConverterTestData{ Value = decimal.MaxValue, SimpleType = SimpleType.PositiveInteger, Text="79228162514264337593543950335" },
      new ConverterTestData{ Value = 0, SimpleType = SimpleType.NonNegativeInteger, Text="0" },
      new ConverterTestData{ Value = 0, SimpleType = SimpleType.NonPositiveInteger, Text="0" },
      new ConverterTestData{ Value = decimal.MinValue, SimpleType = SimpleType.NegativeInteger, Text="-79228162514264337593543950335" },
      new ConverterTestData{ Value = new byte[]{1,2,3}, SimpleType = SimpleType.Base64Binary, Text="AQID" },
      new ConverterTestData{ Value = new byte[]{1,2,3}, SimpleType = SimpleType.HexBinary, Text="010203" },
      new ConverterTestData{ Value = new byte[]{1,2,3}, SimpleType = 0, Text="1 2 3" },
      new ConverterTestData{ Value = new string[]{"1","2","3"}, SimpleType = 0, Text="1 2 3" },
      new ConverterTestData{ Value = new int[]{1,2,3}, SimpleType = 0, Text="1 2 3" },
      new ConverterTestData{ Value = new Uri("HTTP://www.Contoso.com:80/thick%20and%20thin.htm"), SimpleType = 0, Text="HTTP://www.Contoso.com:80/thick%20and%20thin.htm" },
      new ConverterTestData{ Value = new XmlQualifiedName("name","prefix"), SimpleType = 0, Text="prefix:name" },
      new ConverterTestData{ Value = new GDate(2022, 0, 0), SimpleType = SimpleType.GYear, Text="2022" },
      new ConverterTestData{ Value = new GDate(2022, 2, 0), SimpleType = SimpleType.GYearMonth, Text="2022-02" },
      new ConverterTestData{ Value = new GDate(0, 2, 0), SimpleType = SimpleType.GMonth, Text="--02" },
      new ConverterTestData{ Value = new GDate(0, 2, 1), SimpleType = SimpleType.GMonthDay, Text="--02-01" },
      new ConverterTestData{ Value = new GDate(0, 0, 1), SimpleType = SimpleType.GDay, Text="---01" },
    };

    public void TestSingleValueTypeConverter(Type? expectedType, SimpleType? xsdType, ConverterTestData data)
    {
      if (xsdType == 0)
        xsdType = null;

      ValueTypeConverter converter = new ValueTypeConverter { ExpectedType = expectedType, SimpleType = xsdType };
      if (expectedType == null)
        converter.ExpectedType = data.Value.GetType();
      if (expectedType == typeof(bool) && xsdType == SimpleType.String)
        Debug.Assert(true);
      converter.Init();

      var str1 = converter.ConvertTo(data.Value, typeof(string)) as string;
      var str0 = data.Text;
      //StringAssert.AreEqualIgnoringCase(str1, str0);
      Assert.That(str1, Is.EqualTo(str0), $"Expected string was \"{str0}\" but \"{str1}\" produced" + ErrorMsgTail(converter, data.SimpleType));
      var value = converter.ConvertFrom(str1);
      Assert.That(value is not null, $"Value expected but null produced" + ErrorMsgTail(converter, data.SimpleType));
      if (expectedType != null && value != null)
      {
        if (expectedType == typeof(Array))
        {
          if (!value.GetType().IsArray)
            Assert.That(value.GetType, Is.EqualTo(expectedType),
              $"Expected type was {expectedType.Name} but {value.GetType().Name} produced" + ErrorMsgTail(converter, data.SimpleType));
        }
        else
          Assert.That(value.GetType, Is.EqualTo(expectedType),
            $"Expected type was {expectedType.Name} but {value.GetType().Name} produced" + ErrorMsgTail(converter, data.SimpleType));

      }
    }

    private string ErrorMsgTail(ValueTypeConverter converter, SimpleType? xsdType)
    {
      return $" in {converter.InternalTypeConverter?.GetType().Name}" + (xsdType == null ? null : $" with SimpleType = {xsdType}");
    }
  }
}