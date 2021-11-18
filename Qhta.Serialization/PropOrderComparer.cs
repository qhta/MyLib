using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.Serialization
{
  public class PropOrderComparer: IComparer<SerializedPropertyInfo>
  {
    public int Compare(SerializedPropertyInfo x, SerializedPropertyInfo y)
    { 
      var result = x.Order.CompareTo(y.Order);
      if (result != 0)
        return result;
      return x.Name.CompareTo(y.Name);
    }
  }
}
