using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ErrorHandlingBehaviorLibrary
{
  [DataContract]
  public class InvalidOperationFault
  {
    [DataMember]
    public InvalidOperationException Exception { get; private set; }

    public InvalidOperationFault(InvalidOperationException e)
    {
      Exception = e;
    }
  }
}
