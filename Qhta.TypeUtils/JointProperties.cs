using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Runtime.Serialization;

namespace Qhta.TypeUtils
{
  /// <summary>
  /// A class to copy properties marked with [DataMember] attribute
  /// </summary>
  public class JointProperties
  {
    /// <summary>
    /// Source property info (copy from)
    /// </summary>
    public PropertyInfo SourceProp;

    /// <summary>
    /// Target property info (copy to)
    /// </summary>
    public PropertyInfo TargetProp;

    /// <summary>
    /// First - get list of common data members
    /// </summary>
    /// <param name="sourceDataType"></param>
    /// <param name="targetDataType"></param>
    /// <returns></returns>
    public static IEnumerable<JointProperties> GetJointProperties(Type sourceDataType, Type targetDataType)
    {
      var sourceProperties = sourceDataType.GetProperties().Where(item => item.GetCustomAttribute<DataMemberAttribute>() != null);
      var internProperties = targetDataType.GetProperties().Where(item => item.GetCustomAttribute<DataMemberAttribute>() != null);
      var jointProperties = sourceProperties.Join(internProperties, (sourceProp) => sourceProp.Name,
        (targetProp) => targetProp.Name, (sourceProp, targetProp) => new JointProperties { SourceProp = sourceProp, TargetProp = targetProp });
      return jointProperties;
    }

    /// <summary>
    /// Secont - copy common data members using prepared list
    /// </summary>
    /// <param name="sourceDataObject"></param>
    /// <param name="targetDataObject"></param>
    /// <param name="jointProperties"></param>
    public static void CopyJointProperties(object sourceDataObject, object targetDataObject, IEnumerable<JointProperties> jointProperties)
    {
      foreach (var pair in jointProperties)
      {
        var value = pair.SourceProp.GetValue(sourceDataObject);
        pair.TargetProp.SetValue(targetDataObject, value);
      }
    }

    /// <summary>
    /// Helper - enumerate data members of a single data type
    /// </summary>
    /// <param name="dataType"></param>
    /// <returns></returns>
    public static IEnumerable<PropertyInfo> GetDataProperties(Type dataType)
    {
      var dataProperties = dataType.GetProperties().Where(item => item.GetCustomAttribute<DataMemberAttribute>() != null);
      return dataProperties;
    }

  }
}
