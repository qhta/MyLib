namespace Qhta.Xml.Serialization;

/// <summary>
/// This is a replacement for <see cref="System.Windows.Markup.ContentPropertyAttribute"/>
/// which is allowed only for .NET Framework 4.8, but not for .NET Core.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class XmlContentPropertyAttribute : Attribute
{
  /// <summary>
  /// This attribute hold the name of the property used to get/set the Xml content 
  /// of the Xml element
  /// </summary>
  /// <param name="name">A name of the public property of any type</param>
  public XmlContentPropertyAttribute(string name)
  {
    Name = name;
  }

  /// <summary>
  ///  name of the public property of any type
  /// </summary>
  public string Name { get;}
}