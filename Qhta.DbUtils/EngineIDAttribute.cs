using System;
using System.Collections.Generic;
using System.Text;

namespace Qhta.DbUtils
{
  public class EngineIDAttribute: Attribute
  {
    public EngineIDAttribute(string id)
    {
      Identifier=id;
    }

    public string Identifier { get; private set; }

  }
}
