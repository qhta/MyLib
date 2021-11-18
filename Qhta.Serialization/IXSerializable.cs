using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.Serialization
{
  public interface IXSerializable
  {
    void Serialize(IXSerializer serializer);
    void Deserialize(IXSerializer baseSerializer);
  }
}
