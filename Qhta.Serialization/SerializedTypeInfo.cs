using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.Serialization
{
  public class SerializedTypeInfo
  {
    /// <summary>
    /// A type to serialize or deserialize
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    /// A public constructor to invoke while deserialization
    /// </summary>
    public ConstructorInfo KnownConstructor { get; set; }

    /// <summary>
    /// Known properties to serialize as XML attributes.
    /// </summary>
    public KnownPropertiesDictionary PropsAsAttributes { get; set; } = new KnownPropertiesDictionary();

    /// <summary>
    /// Known properties to serialize as XML elements.
    /// </summary>
    public KnownPropertiesDictionary PropsAsElements { get; set; } = new KnownPropertiesDictionary();

    /// <summary>
    /// Known properties to serialize as XML arrays.
    /// </summary>
    public KnownPropertiesDictionary PropsAsArrays { get; set; } = new KnownPropertiesDictionary();
  }
}
