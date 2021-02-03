using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Xml.Serialization;

namespace Qhta.RegularExpressions
{
  [ContentProperty(nameof(Items))]
  public class RegExItem
  {
    public RegExTag Tag { get; set; }

    public int Start { get; set; }

    public int Length => Str?.Length ?? 0;

    public string Str { get; set; }

    public RegExStatus Status { get; set; }


    public RegExItems Items { get; protected set; }

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
      if (Items!=null)
      foreach (var item in Items)
        item.MoveStart(delta);
    }

    public override string ToString()
    {
      var result = $"{Tag} ({Start}, {Length}) {Status}: \"{Str}\"";
      if (Inequalities != null)
        result += $" expected {Inequalities.ToString()}";
      return result;
    }

    public override bool Equals(object obj)
    {
      Inequalities = null;
      bool result = true;
      if (obj!=null && this.GetType() != obj.GetType())
      {
        AddInequality(new Inequality { Property = "Type", Obtained = this.GetType(), Expected = obj.GetType() });
        result = false;
      }

      if (obj is RegExItem other)
      {
        if (this.Tag != other.Tag)
        {
          AddInequality(new Inequality { Property = "Tag", Obtained = this.Tag, Expected = other.Tag });
          result = false;
        }
        if (this.Status != other.Status)
        {
          AddInequality(new Inequality { Property = "Status", Obtained = this.Status, Expected = other.Status });
          result = false;
        }
        if (this.Start != other.Start)
        {
          AddInequality(new Inequality { Property = "Start", Obtained = this.Start, Expected = other.Start });
          result = false;
        }
        if (this.Length != other.Length)
        {
          AddInequality(new Inequality { Property = "Length", Obtained = this.Length, Expected = other.Length });
          result = false;
        }
        if (this.Str != other.Str)
        {
          AddInequality(new Inequality { Property = "Str", Obtained = this.Str, Expected = other.Str });
          result = false;
        }
        if (this.Items != null && other.Items != null && !this.Items.Equals(other.Items))
          result = false;
        if (this.Items?.Count != other.Items.Count)
        {
          AddInequality(new Inequality { Property = "SubItems.Count", Obtained = this.Items.Count, Expected = other.Items.Count });
          result = false;
        }
        return result;
      }
      return false;
    }

    protected void AddInequality(Inequality item)
    {
      if (Inequalities == null)
        Inequalities = new Inequalities();
      Inequalities.Add(item);
    }


    public Inequalities Inequalities { get; private set; }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}
