using System;
using System.Collections.Generic;
using System.Text;

namespace Qhta.Parsing
{
  public enum TokenKind
  {
    Whitespaces,
    Letters,
    Digits,
    Identifier,
    Number,
    HexNumber,
    FloatNumber,
    Symbol,
    CharLiteral,
    StringLiteral,
    Other,
  }
}
