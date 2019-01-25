using System;
using System.Collections.Generic;
using System.Text;

namespace Qhta.Parsing
{
  public class Token
  {
    public TokenKind Kind { get; set; }
    public string String { get; set; }
    public object Value { get; set; }
  }
}
