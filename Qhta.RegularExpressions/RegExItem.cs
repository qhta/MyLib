using System;
using System.Collections.Generic;
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
