using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Qhta.RegularExpressions
{
  public class RegExItem
  {
    public RegExTag Tag { get; set; }

    public int Start { get; set; }

    public int Length => Str?.Length ?? 0;

    public string Str { get; set; }

    public RegExStatus Status { get; set; }

    public virtual RegExItems SubItems => null;

    public char CharValue
    {
      get
      {
        switch (Tag)
        {
          case RegExTag.LiteralChar:
            return Str.FirstOrDefault();
          case RegExTag.OctalSeq:
            int n = 0;
            for (int i = 1; i < Str.Length; i++)
              n = n * 8 + Str[i] - '0';
            return (char)n;
          case RegExTag.HexadecimalSeq:
          case RegExTag.UnicodeSeq:
            if (Str.Length>2)
              return (char)Int32.Parse(Str.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            return '\0';
          case RegExTag.ControlCharSeq:
            if (Str.Length > 2)
              return (char)(Str[2] - '@');
            return '\0';
          default:
            return '\0';
        }
      }
    }

    public virtual void MoveStart(int delta)
    {
      Start += delta;
    }

    public override string ToString()
    {
      return $"{Tag} ({Start}, {Length}) {Status}: \"{Str}\"";
    }

    public override bool Equals(object obj)
    {
      if (obj is RegExItem other)
      {
        if (this.Tag != other.Tag)
          return false;
        if (this.Status != other.Status)
          return false;
        if (this.Start != other.Start)
          return false;
        if (this.Length != other.Length)
          return false;
        if (this.Str != other.Str)
          return false;
        return true;
      }
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}
