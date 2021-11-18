using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.Serialization
{
  [Flags]
  public enum SerializationOptions
  {
    LowercaseAttributeName = 1,
    LowercasePropertyName = 2,
    PrecedePropertyNameWithElementName = 4,
  }
}
