﻿namespace Qhta.RegularExpressions
{
  public enum RegExTag
  {
    LiteralChar,
    LiteralString,
    CharSet,
    CharRange,
    CharSetControlChar,
    EscapedChar,
    AnchorControl,
    DotChar,
    CharClass,
    ControlCharSeq,
    OctalSeq,
    HexadecimalSeq,
    UnicodeSeq,
    UnicodeCategorySeq,
    Quantifier,
    AltChar,
    Subexpression,
    GroupControlChar,
    GroupName,
    NamedGroup,
    BackRef,
  }
}