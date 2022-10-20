using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Globalization;
using Qhta.TypeUtils;

namespace Qhta.Conversion;

public class ArrayTypeConverter : TypeConverter, ITypeConverter, ILengthRestrictions
{

  public string? Format { get; set; }
  public Type? ExpectedType { get; set; }
  public XsdSimpleType? XsdType { get; set; }

  public int? MinLength { get; set; }
  public int? MaxLength { get; set; }


  private ValueTypeConverter ItemConverter { get; set; } = new ValueTypeConverter();

  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
  {
    if (value == null)
      return null;
    if (value is byte[] bytes)
    {
      if (XsdType == XsdSimpleType.Base64Binary)
        return Convert.ToBase64String(bytes);
      if (XsdType ==XsdSimpleType.HexBinary)
        return Convert.ToHexString(bytes);
    }

    var xsdType = XsdType;
    if (xsdType == XsdSimpleType.NmTokens)
      xsdType = XsdSimpleType.NmToken;
    if (xsdType == XsdSimpleType.IdRefs)
      xsdType = XsdSimpleType.IdRef;
    if (xsdType == XsdSimpleType.Entities)
      xsdType = XsdSimpleType.Entity;

    ItemConverter.Init (null, xsdType, Format, culture);

    var list = new List<string?>();
    if (value is Array array)
    {
      foreach (var item in array)
      {
        ItemConverter.XsdType = xsdType;
        list.Add((string?)ItemConverter.ConvertTo(context, culture, item, typeof(string)));
      }
    }
    return String.Join(" ", list);
  }


  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    if (value is string str)
    {
      if (XsdType == XsdSimpleType.Base64Binary)
        return Convert.FromBase64String(str);
      if (XsdType == XsdSimpleType.HexBinary)
        return Convert.FromHexString(str);


      var xsdType = XsdType;
      if (xsdType == XsdSimpleType.NmTokens)
        xsdType = XsdSimpleType.NmToken;
      if (xsdType == XsdSimpleType.IdRefs)
        xsdType = XsdSimpleType.IdRef;
      if (xsdType == XsdSimpleType.Entities)
        xsdType = XsdSimpleType.Entity;


      var expectedType = ExpectedType;
      if (ExpectedType != null && ExpectedType.IsArray(out var itemType))
        expectedType = itemType;

      ItemConverter.Init(expectedType, xsdType, Format, culture);

      if (expectedType == null)
        expectedType =  ItemConverter.ExpectedType;
      if (expectedType == null)
        throw new InvalidOperationException($"ExpectedType not specified for ArrayTypeConverter");
      var strs = str.Split(" ", StringSplitOptions.RemoveEmptyEntries);
      var result = Array.CreateInstance(expectedType, strs.Length);
      ValidateLength(result, MinLength, MaxLength);
      if (strs.Length==0)
        return result;
      for (int i = 0; i < strs.Length; i++)
        result.SetValue(ItemConverter.ConvertFrom(context, culture, strs[i]), i);
      return result;
    }
    return null;
  }

  public void ValidateLength(Array result, int? minLength, int? maxLength)
  {
    if (maxLength != null && result.Length > maxLength)
      throw new InvalidDataException($"Too many items in ArrayTypeConverter. Got {result.Length}. Expected {maxLength}.");
    if (minLength != null && result.Length < minLength)
      throw new InvalidDataException($"not enough items in ArrayTypeConverter. Got {result.Length}. Expected {minLength}.");
  }


}