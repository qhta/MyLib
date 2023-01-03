using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Qhta.TypeUtils;

/// <summary>
///   A class to copy properties marked with [DataMember] attribute
/// </summary>
public class JointProperties
{
  /// <summary>
  ///   Source property info (copy from)
  /// </summary>
  public PropertyInfo SourceProp = null!;

  /// <summary>
  ///   Target property info (copy to)
  /// </summary>
  public PropertyInfo TargetProp = null!;

  /// <summary>
  ///   First - get list of common data members
  /// </summary>
  /// <param name="sourceDataType"></param>
  /// <param name="targetDataType"></param>
  /// <returns></returns>
  public static IEnumerable<JointProperties> GetJointProperties(Type sourceDataType, Type targetDataType)
  {
    var sourceProperties = sourceDataType.GetProperties().Where(item => item.GetCustomAttribute<DataMemberAttribute>() != null);
    var internProperties = targetDataType.GetProperties().Where(item => item.GetCustomAttribute<DataMemberAttribute>() != null);
    var jointProperties = sourceProperties.Join(internProperties, sourceProp => sourceProp.Name,
      targetProp => targetProp.Name, (sourceProp, targetProp) => new JointProperties { SourceProp = sourceProp, TargetProp = targetProp });
    return jointProperties;
  }

  /// <summary>
  ///   Secont - copy common data members using prepared list
  /// </summary>
  /// <param name="sourceDataObject"></param>
  /// <param name="targetDataObject"></param>
  /// <param name="jointProperties"></param>
  public static void CopyJointProperties(object sourceDataObject, object targetDataObject, IEnumerable<JointProperties> jointProperties)
  {
    foreach (var pair in jointProperties)
    {
      var value = pair.SourceProp.GetValue(sourceDataObject);
      if (TryGetTypeConverter(pair.TargetProp, out var targetTypeConverter) && targetTypeConverter != null &&
          targetTypeConverter.CanConvertFrom(pair.SourceProp.PropertyType))
        pair.TargetProp.SetValue(targetDataObject, targetTypeConverter.ConvertFrom(value));
      else
        pair.TargetProp.SetValue(targetDataObject, value);
    }
  }

  /// <summary>
  ///   Helper - enumerate data members of a single data type
  /// </summary>
  /// <param name="dataType"></param>
  /// <returns></returns>
  public static IEnumerable<PropertyInfo> GetDataProperties(Type dataType)
  {
    var dataProperties = dataType.GetProperties().Where(item => item.GetCustomAttribute<DataMemberAttribute>() != null);
    return dataProperties;
  }

  /// <summary>
  ///   Get TypeConverter instance for a specific property
  /// </summary>
  /// <param name="property"></param>
  /// <param name="converter"></param>
  /// <returns></returns>
  public static bool TryGetTypeConverter(PropertyInfo property, out TypeConverter? converter)
  {
    var typeConverterAttribute = property.PropertyType.GetCustomAttribute<TypeConverterAttribute>();
    if (typeConverterAttribute != null)
    {
      var converterTypeName = typeConverterAttribute.ConverterTypeName;
      if (converterTypeName != null)
      {
        var converterType = Type.GetType(converterTypeName);
        if (converterType != null)
        {
          var instanceProperty = converterType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
          if (instanceProperty != null)
          {
            var instance = instanceProperty.GetValue(null);
            if (instance is TypeConverter typeConverter)
            {
              converter = typeConverter;
              return true;
            }
          }
          var constructor = converterType.GetConstructor(new Type[0]);
          if (constructor != null)
          {
            var instance = constructor.Invoke(new object[0]);
            if (instance is TypeConverter typeConverter)
            {
              converter = typeConverter;
              return true;
            }
          }
        }
      }
    }
    converter = null;
    return false;
  }
}