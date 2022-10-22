using System;
using System.ComponentModel;
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
            TestSingleValueValueTypeConverter(converter, allowedType, data);
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
            TestSingleValueValueTypeConverter(converter, expectedType, data);
        }
      }
    }
    private static ConverterTestData[] TestData = 
    {
      new ConverterTestData{ Value = true, Text="True" },
      new ConverterTestData{ Value = false, Text="False" },
      new ConverterTestData{ Value = "abc", /*XsdType = XsdSimpleType.String, */Text="abc" },
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